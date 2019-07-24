using System;
using System.Collections.Generic;
using System.IO;

namespace Veldrid.SPIRV.Instructions
{
    internal abstract class TypeInstruction : Instruction
    {
        protected static readonly ValueTuple<string, uint?> EmptyEvaulation = new ValueTuple<string, uint?>(null, null);

        public uint IdResult { get; set; }

        public override void Parse(BinaryReader reader, uint wordCount)
        {
            IdResult = reader.ReadUInt32();
        }

        public virtual ResourceKind EvaluateKind(IDictionary<uint, TypeInstruction> types)
        {
            return ResourceKind.UniformBuffer;
        }
    }
}