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

        public override ValueTuple<string, uint?> Evaluate(IDictionary<uint, TypeInstruction> types)
        {
            switch (Width)
            {
                case 32:
                    return ValueTuple.Create("float", (uint?) Width / 8);
                case 64:
                    return ValueTuple.Create("double", (uint?) Width / 8);
                default:
                    return ValueTuple.Create((string) null, (uint?) Width / 8);
            }
        }
    }
}