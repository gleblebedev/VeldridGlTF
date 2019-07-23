using System.IO;

namespace Veldrid.SPIRV.Instructions
{
    internal abstract class Instruction
    {
        public abstract Op OpCode { get; }
        public abstract void Parse(BinaryReader reader, uint wordCount);

        public override string ToString()
        {
            return OpCode.ToString();
        }
    }
}