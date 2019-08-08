using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Veldrid.SPIRV.Instructions;

namespace Veldrid.SPIRV
{
    public static class SpirvReflection2
    {
        public const uint MagicNumber = 0x07230203;

        public static SpirvCompilationResultEx[] CompileGlslToSpirv(params ShaderArgs[] shaders)
        {
            return CompileGlslToSpirv((ILayoutNameResolver)null, shaders);
        }

        public static SpirvCompilationResultEx[] CompileGlslToSpirv(ILayoutNameResolver nameNameResolver, params ShaderArgs[] shaders)
        {
            var res = new SpirvCompilationResultEx[shaders.Length];
            
            for (var index = 0; index < shaders.Length; index++)
            {
                var shader = shaders[index];
                var compilationResult = SpirvCompilation.CompileGlslToSpirv(shader.Source, shader.FileName,
                    shader.Stage,
                    new GlslCompileOptions(true));
                var layout = Parse(compilationResult.SpirvBytes, shader.Stage, nameNameResolver);
                res[index] = new SpirvCompilationResultEx(compilationResult.SpirvBytes, layout.ToArray());
            }

            return res;
        }
  
        public static ValueTuple<SpirvCompilationResultEx, SpirvCompilationResultEx> CompileGlslToSpirv(string vertex, string fragment, ILayoutNameResolver nameNameResolver = null)
        {
            var res = CompileGlslToSpirv(
                nameNameResolver,
                new[] {
                    new ShaderArgs {FileName = "vert.glsl", Source = vertex, Stage = ShaderStages.Vertex},
                    new ShaderArgs {FileName = "frag.glsl", Source = fragment, Stage = ShaderStages.Fragment}
                });
            return ValueTuple.Create(res[0], res[1]);
        }

        public static IList<ResourceLayoutDescription> Parse(byte[] spirvBytes, ShaderStages stage, ILayoutNameResolver nameNameResolver = null)
        {
            var sets = new List<IList<ResourceLayoutElementDescription>>();
            using (var reader = new BinaryReader(new MemoryStream(spirvBytes)))
            {
                ParseHeader(reader);

                var decorations = new List<Decorate>();
                var types = new Dictionary<uint, TypeInstruction>();
                var names = new Dictionary<uint, Name>();
                var uniforms = new List<Variable>();
                var inputs = new List<Variable>();
                var memberNames = new List<MemberName>();

                while (reader.BaseStream.Position != spirvBytes.Length)
                {
                    var i = ReadInstruction(reader);
                    if (i != null)
                        switch (i.OpCode)
                        {
                            case Op.OpTypeVoid:
                            case Op.OpTypeFloat:
                            case Op.OpTypeVector:
                            case Op.OpTypeMatrix:
                            case Op.OpTypeStruct:
                            case Op.OpTypePointer:
                            case Op.OpTypeImage:
                            case Op.OpTypeSampler:
                            {
                                var type = (TypeInstruction) i;
                                types.Add(type.IdResult, type);
                                break;
                            }

                            case Op.OpName:
                            {
                                var name = (Name) i;
                                names.Add(name.Target, name);
                                break;
                            }
                            case Op.OpMemberName:
                            {
                                var name = (MemberName)i;
                                memberNames.Add(name);
                                break;
                            }
                            case Op.OpDecorate:
                            {
                                var decorate = (Decorate) i;
                                decorations.Add(decorate);
                                break;
                            }

                            case Op.OpVariable:
                            {
                                var variable = (Variable) i;
                                switch (variable.StorageClass)
                                {
                                    case StorageClass.Uniform:
                                        uniforms.Add(variable);
                                        break;
                                    case StorageClass.UniformConstant:
                                        uniforms.Add(variable);
                                        break;
                                    case StorageClass.Input:
                                        inputs.Add(variable);
                                        break;
                                }

                                break;
                            }
                        }
                }

                var decorationLookup = decorations.ToLookup(_ => _.Target);
                foreach (var variable in uniforms)
                {
                    string uniformName = null;
                    uint set = 0;
                    uint binding = 0;
                    if (names.TryGetValue(variable.IdResult, out var name)) uniformName = name.TargetName;
                    foreach (var decorate in decorationLookup[variable.IdResult])
                    {
                        switch (decorate.Decoration)
                        {
                            case Decoration.DescriptorSet:
                                set = decorate.DescriptorSet.Value;
                                break;
                            case Decoration.Binding:
                                binding = decorate.BindingPoint.Value;
                                break;
                        }
                    }

                    var kind = ResourceKind.UniformBuffer;
                    if (types.TryGetValue(variable.IdResultType, out var type))
                    {
                        kind = type.EvaluateKind(types);
                        if (string.IsNullOrEmpty(uniformName))
                            uniformName = GetTypeName(type, types, names);
                    }

                    if (uniformName == null && nameNameResolver != null)
                    {
                        uniformName = nameNameResolver.Resolve(set,binding,kind);
                    }
                    var element = new ResourceLayoutElementDescription(uniformName, kind, stage, ResourceLayoutElementOptions.None);

                    while (sets.Count <= set) sets.Add(new List<ResourceLayoutElementDescription>());
                    var setCollection = sets[(int)set];
                    while (setCollection.Count <= binding) setCollection.Add(new ResourceLayoutElementDescription("", ResourceKind.UniformBuffer, ShaderStages.None));
                    setCollection[(int)binding] = element;
                }
            }

            return sets.Select(_=>new ResourceLayoutDescription(_.ToArray())).ToList();
        }

        private static string GetTypeName(TypeInstruction type, Dictionary<uint, TypeInstruction> types, Dictionary<uint, Name> names)
        {
            if (names.TryGetValue(type.IdResult, out var name))
                return name.TargetName;
            var pointer = type as TypePointer;
            if (pointer != null && types.TryGetValue(pointer.Type, out var valueType))
                return GetTypeName(valueType, types, names);
            return null;
        }

        private static void ParseHeader(BinaryReader reader)
        {
            var Magic = reader.ReadUInt32();
            if (Magic != MagicNumber) throw new FormatException("Not a SPIRV byte code");
            var Version = reader.ReadUInt32();
            if (Version != 0x00010000)
                throw new FormatException("Unsupported SPIRV byte code version");
            var Generator = reader.ReadUInt32();
            var Bound = reader.ReadUInt32();
            var Reserved = reader.ReadUInt32();
        }

        private static Instruction ReadInstruction(BinaryReader reader)
        {
            var instruction = reader.ReadUInt32();
            var opCode = (Op) (instruction & 0x0FFFF);
            //Console.WriteLine(opCode);
            var wordCount = instruction >> 16;
            if (wordCount == 0) throw new FormatException("Instruction size can't be 0");
            --wordCount;
            switch (opCode)
            {
                case Op.OpTypePointer:
                {
                    var type = new TypePointer();
                    type.Parse(reader, wordCount);
                    return type;
                }

                case Op.OpTypeVoid:
                {
                    var type = new TypeVoid();
                    type.Parse(reader, wordCount);
                    return type;
                }

                case Op.OpTypeSampler:
                {
                    var type = new TypeSampler();
                    type.Parse(reader, wordCount);
                    return type;
                }

                case Op.OpTypeImage:
                {
                    var type = new TypeImage();
                    type.Parse(reader, wordCount);
                    return type;
                }

                case Op.OpTypeFloat:
                {
                    var type = new TypeFloat();
                    type.Parse(reader, wordCount);
                    return type;
                }

                case Op.OpTypeVector:
                {
                    var type = new TypeVector();
                    type.Parse(reader, wordCount);
                    return type;
                }

                case Op.OpTypeMatrix:
                {
                    var type = new TypeMatrix();
                    type.Parse(reader, wordCount);
                    return type;
                }

                case Op.OpTypeStruct:
                {
                    var type = new TypeStruct();
                    type.Parse(reader, wordCount);
                    return type;
                }

                case Op.OpName:
                {
                    var name = new Name();
                    name.Parse(reader, wordCount);
                    return name;
                }

                case Op.OpMemberName:
                {
                    var name = new MemberName();
                    name.Parse(reader, wordCount);
                    return name;
                }

                case Op.OpDecorate:
                {
                    var decoration = new Decorate();
                    decoration.Parse(reader, wordCount);
                    return decoration;
                }

                case Op.OpVariable:
                {
                    var variable = new Variable();
                    variable.Parse(reader, wordCount);
                    return variable;
                }

                default:
                    reader.BaseStream.Position += wordCount * 4;
                    return null;
            }
        }
    }
}