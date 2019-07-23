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

        public override ValueTuple<string, uint?> Evaluate(IDictionary<uint, TypeInstruction> types)
        {
            switch (Dim)
            {
                case Dim.Dim1D:
                    return ValueTuple.Create<string, uint?>("texture1D", null);
                case Dim.Dim2D:
                    return ValueTuple.Create<string, uint?>("texture2D", null);
                case Dim.Dim3D:
                    return ValueTuple.Create<string, uint?>("texture3D", null);
                case Dim.Cube:
                    return ValueTuple.Create<string, uint?>("textureCube", null);
            }

            return EmptyEvaulation;
        }
    }
}