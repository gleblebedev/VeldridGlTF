using System;
using System.Collections.Generic;
using System.IO;

namespace Veldrid.SPIRV.Instructions
{
    internal class TypeSampler : TypeInstruction
    {
        public override Op OpCode => Op.OpTypeSampler;

        public override void Parse(BinaryReader reader, uint wordCount)
        {
            base.Parse(reader, wordCount);
        }

        public override ValueTuple<string, uint?> Evaluate(IDictionary<uint, TypeInstruction> types)
        {
            return ValueTuple.Create<string, uint?>("sampler", null);
        }
    }
}