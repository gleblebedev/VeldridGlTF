using System.IO;
using System.Text;

namespace Veldrid.SPIRV.Instructions
{
    internal class MemberName : Instruction
    {
        public uint Type { get; set; }
        public uint Member { get; set; }
        public string Name { get; set; }

        public override Op OpCode => Op.OpMemberName;

        public override void Parse(BinaryReader reader, uint wordCount)
        {
            Type = reader.ReadUInt32();
            Member = reader.ReadUInt32();
            Name = Encoding.UTF8.GetString(reader.ReadBytes(((int)wordCount - 2) * 4)).TrimEnd('\0');
        }
    }
}