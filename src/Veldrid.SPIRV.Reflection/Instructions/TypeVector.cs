using System;
using System.Collections.Generic;
using System.IO;

namespace Veldrid.SPIRV.Instructions
{
    internal class TypeVector : TypeInstruction
    {
        public uint ComponentType { get; set; }
        public uint ComponentCount { get; set; }

        public override Op OpCode => Op.OpTypeVector;

        public override void Parse(BinaryReader reader, uint wordCount)
        {
            base.Parse(reader, wordCount);
            ComponentType = reader.ReadUInt32();
            ComponentCount = reader.ReadUInt32();
        }
        public override ResourceKind EvaluateKind(IDictionary<uint, TypeInstruction> types)
        {
            return ResourceKind.UniformBuffer;
        }
    }
}