using System;
using System.Collections.Generic;
using System.IO;

namespace Veldrid.SPIRV.Instructions
{
    internal class TypePointer : TypeInstruction
    {
        public StorageClass StorageClass { get; set; }
        public uint Type { get; set; }

        public override Op OpCode => Op.OpTypePointer;

        public override void Parse(BinaryReader reader, uint wordCount)
        {
            base.Parse(reader, wordCount);
            StorageClass = (StorageClass) reader.ReadUInt32();
            Type = reader.ReadUInt32();
        }
        public override ResourceKind EvaluateKind(IDictionary<uint, TypeInstruction> types)
        {
            if (!types.TryGetValue(Type, out var valueType))
                return base.EvaluateKind(types);
            return valueType.EvaluateKind(types);
        }
    }
}