using System;
using System.Collections.Generic;
using System.IO;

namespace Veldrid.SPIRV.Instructions
{
    internal class TypeFloat : TypeInstruction
    {
        public uint Width { get; set; }

        public override Op OpCode => Op.OpTypeFloat;

        public override void Parse(BinaryReader reader, uint wordCount)
        {
            base.Parse(reader, wordCount);
            Width = reader.ReadUInt32();
        }

        public override ResourceKind EvaluateKind(IDictionary<uint, TypeInstruction> types)
        {
            return ResourceKind.UniformBuffer;
        }
    }
}