using System.IO;

namespace Veldrid.SPIRV.Instructions
{
    internal class Decorate : Instruction
    {
        public uint Target { get; set; }
        public Decoration Decoration { get; set; }
        public uint? BindingPoint { get; set; }
        public uint? DescriptorSet { get; set; }
        public uint? Location { get; set; }

        public override Op OpCode => Op.OpDecorate;

        public override void Parse(BinaryReader reader, uint wordCount)
        {
            var end = reader.BaseStream.Position + wordCount * 4;
            Target = reader.ReadUInt32();
            Decoration = (Decoration) reader.ReadUInt32();
            switch (Decoration)
            {
                case Decoration.Location:
                {
                    Location = reader.ReadUInt32();
                    break;
                }

                case Decoration.DescriptorSet:
                {
                    DescriptorSet = reader.ReadUInt32();
                    break;
                }

                case Decoration.Binding:
                {
                    BindingPoint = reader.ReadUInt32();
                    break;
                }
            }

            reader.BaseStream.Position = end;
        }
    }
}