using System;
using System.Collections.Generic;
using System.IO;

namespace Veldrid.SPIRV.Instructions
{
    internal class TypeImage : TypeInstruction
    {
        public uint SampledType { get; set; }
        public Dim Dim { get; set; }

        public override Op OpCode => Op.OpTypeImage;

        public override void Parse(BinaryReader reader, uint wordCount)
        {
            var end = reader.BaseStream.Position + wordCount * 4;
            base.Parse(reader, wordCount);
            SampledType = reader.ReadUInt32();
            Dim = (Dim) reader.ReadUInt32();
            reader.BaseStream.Position = end;
        }

        public override ResourceKind EvaluateKind(IDictionary<uint, TypeInstruction> types)
        {
            return ResourceKind.TextureReadOnly;
        }
    }
}