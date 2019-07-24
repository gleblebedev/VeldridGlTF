using System;
using System.Collections.Generic;
using System.IO;

namespace Veldrid.SPIRV.Instructions
{
    internal class TypeMatrix : TypeInstruction
    {
        public uint ColumnType { get; set; }
        public uint ColumnCount { get; set; }

        public override Op OpCode => Op.OpTypeVector;

        public override void Parse(BinaryReader reader, uint wordCount)
        {
            base.Parse(reader, wordCount);
            ColumnType = reader.ReadUInt32();
            ColumnCount = reader.ReadUInt32();
        }

        public override ResourceKind EvaluateKind(IDictionary<uint, TypeInstruction> types)
        {
            return ResourceKind.UniformBuffer;
        }
    }
}