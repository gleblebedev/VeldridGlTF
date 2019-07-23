using System.IO;

namespace Veldrid.SPIRV.Instructions
{
    internal class Variable : Instruction
    {
        public uint IdResultType { get; set; }
        public uint IdResult { get; set; }
        public StorageClass StorageClass { get; set; }
        public uint Initializer { get; set; }

        public override Op OpCode => Op.OpVariable;

        public override void Parse(BinaryReader reader, uint wordCount)
        {
            IdResultType = reader.ReadUInt32();
            IdResult = reader.ReadUInt32();
            StorageClass = (StorageClass) reader.ReadUInt32();
            if (wordCount > 3) IdResultType = reader.ReadUInt32();
        }
    }
}