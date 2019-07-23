using System.IO;
using System.Text;

namespace Veldrid.SPIRV.Instructions
{
    internal class Name : Instruction
    {
        public uint Target { get; set; }
        public string TargetName { get; set; }

        public override Op OpCode => Op.OpName;

        public override void Parse(BinaryReader reader, uint wordCount)
        {
            Target = reader.ReadUInt32();
            TargetName = Encoding.UTF8.GetString(reader.ReadBytes(((int) wordCount - 1) * 4)).TrimEnd('\0');
        }
    }
}