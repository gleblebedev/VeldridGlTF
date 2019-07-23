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

        public override ValueTuple<string, uint?> Evaluate(IDictionary<uint, TypeInstruction> types)
        {
            var stringBuilder = new StringBuilder();
            uint totalSize = 0;
            foreach (var type in MemberTypes)
            {
                if (!types.TryGetValue(type, out var memberType))
                    return EmptyEvaulation;
                var (memberName, memberSize) = memberType.Evaluate(types);
                if (memberSize == null)
                    return EmptyEvaulation;
                if (stringBuilder.Length != 0)
                    stringBuilder.Append(",");
                stringBuilder.Append(memberName);
                totalSize += memberSize.Value;
            }

            return ValueTuple.Create<string, uint?>(stringBuilder.ToString(), totalSize);
        }
    }
}