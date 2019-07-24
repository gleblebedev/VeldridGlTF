using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Veldrid.SPIRV.Instructions
{
    internal class TypeStruct : TypeInstruction
    {
        public uint[] MemberTypes { get; set; }

        public override Op OpCode => Op.OpTypeStruct;

        public override void Parse(BinaryReader reader, uint wordCount)
        {
            base.Parse(reader, wordCount);
            MemberTypes = new uint[wordCount - 1];
            for (var index = 0; index < MemberTypes.Length; index++) MemberTypes[index] = reader.ReadUInt32();
        }

        public override ResourceKind EvaluateKind(IDictionary<uint, TypeInstruction> types)
        {
            return ResourceKind.UniformBuffer;
        }
    }
}