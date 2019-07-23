using System;
using System.Collections.Generic;
using System.IO;

namespace Veldrid.SPIRV.Instructions
{
    internal class TypeMatrix : TypeInstruction
    {
        public uint ColumnType { get; set; }
        public uint ColumnCount { get; set; }

        public override Op OpCode => Op.OpTypeVector;

        public override void Parse(BinaryReader reader, uint wordCount)
        {
            base.Parse(reader, wordCount);
            ColumnType = reader.ReadUInt32();
            ColumnCount = reader.ReadUInt32();
        }

        public override ValueTuple<string, uint?> Evaluate(IDictionary<uint, TypeInstruction> types)
        {
            if (!types.TryGetValue(ColumnType, out var columnType))
                return EmptyEvaulation;
            string name = null;
            var (columnName, columnSize) = columnType.Evaluate(types);
            if (columnType.OpCode == Op.OpTypeVector)
            {
                var column = (TypeVector) columnType;
                if (columnName == "vec3" && ColumnCount == 3)
                    name = "mat3";
                else if (columnName == "vec4" && ColumnCount == 4)
                    name = "mat4";
                else if (columnName == "vec2" && ColumnCount == 2)
                    name = "mat2";
            }

            if (columnSize.HasValue) return ValueTuple.Create<string, uint?>(name, columnSize.Value * ColumnCount);
            return ValueTuple.Create<string, uint?>(name, null);
        }
    }
}