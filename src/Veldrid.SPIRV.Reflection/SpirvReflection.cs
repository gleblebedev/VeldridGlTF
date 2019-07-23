using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Veldrid.SPIRV.Instructions;

namespace Veldrid.SPIRV
{
    public class SpirvReflection
    {
        public const uint MagicNumber = 0x07230203;

        public uint Reserved { get; set; }

        public uint Bound { get; set; }

        public uint Generator { get; set; }

        public uint Version { get; set; }

        public uint Magic { get; set; }

        public List<Uniform> Uniforms { get; } = new List<Uniform>();

        public List<Input> Inputs { get; } = new List<Input>();

        public static SpirvReflection Parse(byte[] spirvBytes)
        {
            var spirvReflection = new SpirvReflection();

            using (var reader = new BinaryReader(new MemoryStream(spirvBytes)))
            {
                ParseHeader(spirvReflection, reader);

                var decorations = new List<Decorate>();
                var types = new Dictionary<uint, TypeInstruction>();
                var names = new Dictionary<uint, Name>();
                var uniforms = new List<Variable>();
                var inputs = new List<Variable>();

                while (reader.BaseStream.Position != spirvBytes.Length)
                {
                    var i = ReadInstruction(reader, spirvReflection);
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
                    var uniform = new Uniform();
                    spirvReflection.Uniforms.Add(uniform);
                    if (names.TryGetValue(variable.IdResult, out var name)) uniform.Name = name.TargetName;
                    if (types.TryGetValue(variable.IdResultType, out var type))
                        (uniform.TypeName, uniform.Size) = type.Evaluate(types);
                    foreach (var decorate in decorationLookup[variable.IdResult])
                        switch (decorate.Decoration)
                        {
                            case Decoration.DescriptorSet:
                                uniform.Set = decorate.DescriptorSet;
                                break;
                            case Decoration.Binding:
                                uniform.Binding = decorate.BindingPoint;
                                break;
                        }
                }

                foreach (var variable in inputs)
                {
                    var input = new Input();
                    spirvReflection.Inputs.Add(input);
                    if (names.TryGetValue(variable.IdResult, out var name)) input.Name = name.TargetName;
                    if (types.TryGetValue(variable.IdResultType, out var type))
                        (input.TypeName, input.Size) = type.Evaluate(types);

                    foreach (var decorate in decorationLookup[variable.IdResult])
                        switch (decorate.Decoration)
                        {
                            case Decoration.Location:
                                input.Location = decorate.Location;
                                break;
                        }
                }
            }

            return spirvReflection;
        }

        private static void ParseHeader(SpirvReflection spirvReflection, BinaryReader reader)
        {
            spirvReflection.Magic = reader.ReadUInt32();
            if (spirvReflection.Magic != MagicNumber) throw new FormatException("Not a SPIRV byte code");
            spirvReflection.Version = reader.ReadUInt32();
            if (spirvReflection.Version != 0x00010000)
                throw new FormatException("Unsupported SPIRV byte code version");
            spirvReflection.Generator = reader.ReadUInt32();
            spirvReflection.Bound = reader.ReadUInt32();
            spirvReflection.Reserved = reader.ReadUInt32();
        }

        private static Instruction ReadInstruction(BinaryReader reader, SpirvReflection spirvReflection)
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