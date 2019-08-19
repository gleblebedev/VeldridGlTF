using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FixGltfIssues
{
    public class GltfFile
    {
        private JObject _content;
        private byte[] _binaryBuffer;
        public const uint Magic = 0x46546C67;
        public const uint Version = 0x00000002;

        public void ReadGlb(string fileName)
        {
            using (var fileStream = File.OpenRead(fileName))
            {
                using (var reader = new BinaryReader(fileStream))
                {
                    ReadGlb(reader);
                }
            }
        }
        public void ReadGlb(BinaryReader binaryReader)
        {
            var magic = binaryReader.ReadUInt32();
            if (magic != Magic)
                throw new FormatException();
            var version = binaryReader.ReadUInt32();
            if (version != Version)
                throw new FormatException();
            var length = (long)binaryReader.ReadUInt32();

            length -= 4 * 3;

            while (length > 0)
            {
                var chunkLength = binaryReader.ReadUInt32();
                var chunkType = binaryReader.ReadUInt32();
                var content = binaryReader.ReadBytes((int)chunkLength);
                length -= chunkLength;
                switch (chunkType)
                {
                    case 0x4E4F534A:
                        Content = JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(content));
                        break;
                    case 0x004E4942:
                        BinaryBuffer = content;
                        break;
                }

            }

        }

        public byte[] BinaryBuffer
        {
            get { return _binaryBuffer; }
            set { _binaryBuffer = value; }
        }

        public JObject Content
        {
            get { return _content; }
            set { _content = value; }
        }

        public void ReadGltf(string fileName)
        {
            Content = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(fileName));
            var buffers = (JArray) Content["buffers"];
            foreach (var buffer in buffers)
            {
                var uri = buffer["uri"].ToString();
                var bin = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(fileName)), uri);
                BinaryBuffer = File.ReadAllBytes(bin);
            }
        }

        public void WriteGlb(string fileName)
        {
            using (var fileStream = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                using (var reader = new BinaryWriter(fileStream))
                {
                    WriteGlb(reader);
                }
            }
        }

        public void WriteGlb(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Magic);
            binaryWriter.Write(Version);

            var buffers = (JArray)Content["buffers"];
            buffers.Replace(new JArray(new JObject(new JProperty("byteLength", BinaryBuffer.Length))));
            
            var content = new UTF8Encoding(false).GetBytes(Content.ToString(Formatting.None));
            byte[] padding = null;
            switch (content.Length % 4)
            {
                case 0:
                    padding = new byte[0];
                    break;
                case 1:
                    padding = new byte[]{32, 32 , 32 };
                    break;
                case 2:
                    padding = new byte[] { 32, 32};
                    break;
                case 3:
                    padding = new byte[] { 32};
                    break;
            }
            var length = 3 * 4 + (2 * 4 + content.Length+ padding.Length) + (2 * 4 + BinaryBuffer.Length);

            binaryWriter.Write(length);

            binaryWriter.Write(content.Length+ padding.Length);
            binaryWriter.Write(0x4E4F534A);
            binaryWriter.Write(content);
            binaryWriter.Write(padding);

            binaryWriter.Write(BinaryBuffer.Length);
            binaryWriter.Write(0x004E4942);
            binaryWriter.Write(BinaryBuffer);
        }

        public void FixNormals()
        {
            var meshes = (JArray)Content["meshes"];

            var tangents = new HashSet<int>();
            var normals = new HashSet<int>();

            foreach (var mesh in meshes)
            {
                var primitives = (JArray)mesh["primitives"];
                foreach (var primitive in primitives)
                {
                    var attributes = primitive["attributes"];
                    var TANGENT = attributes["TANGENT"];
                    if (TANGENT != null)
                    {
                        tangents.Add(TANGENT.Value<int>());
                    }
                    var NORMAL = attributes["NORMAL"];
                    if (NORMAL != null)
                    {
                        normals.Add(NORMAL.Value<int>());
                    }
                }
            }

            var accessors = (JArray)Content["accessors"];
            foreach (var tangent in tangents)
            {
                if (tangent == 411) Debugger.Break();
                FixTangents((JObject)accessors[tangent]);
            }
            foreach (var tangent in normals)
            {
                if (tangent == 411) Debugger.Break();
                FixNormals((JObject)accessors[tangent]);
            }
        }

        private void FixTangents(JObject accessor)
        {
            var type = accessor["type"].ToString();
            if (type == "VEC3")
            {
                FixNormals(accessor);
                return;
            }
            FixNormals(accessor);


        }
        private void FixNormals(JObject accessor)
        {
            var count = accessor["count"].Value<int>();
            var accessorByteOffset = accessor["byteOffset"]?.Value<int>() ?? 0;
            var bufferViews = (JArray)Content["bufferViews"];
            var bufferView = (JObject)bufferViews[accessor["bufferView"].Value<int>()];
            var buffer = bufferView["buffer"].Value<int>();
            var byteLength = bufferView["byteLength"].Value<int>();
            var byteOffset = bufferView["byteOffset"].Value<int>();
            var byteStride = bufferView["byteStride"].Value<int>();

            float maxx = float.MinValue;
            float maxy = float.MinValue;
            float maxz = float.MinValue;
            float minx = float.MaxValue;
            float miny = float.MaxValue;
            float minz = float.MaxValue;
            using (var readMs = new MemoryStream(BinaryBuffer))
            {
                using (var writeMs = new MemoryStream(BinaryBuffer))
                {
                    using (var reader = new BinaryReader(readMs))
                    {
                        using (var writer = new BinaryWriter(writeMs))
                        {
                            for (int i = 0; i < count; ++i)
                            {
                                var pos = byteOffset+ accessorByteOffset+i* byteStride;

                                readMs.Position = pos;
                                var x = reader.ReadSingle();
                                var y = reader.ReadSingle();
                                var z = reader.ReadSingle();
                                var v = Vector3.Normalize(new Vector3(x, y, z));
                                if (float.IsNaN(v.X) || float.IsNaN(v.Y) || float.IsNaN(v.Z))
                                    v = Vector3.UnitY;

                                writeMs.Position = pos;
                                writer.Write(v.X);
                                writer.Write(v.Y);
                                writer.Write(v.Z);

                                if (maxx < v.X) maxx = v.X;
                                if (maxy < v.Y) maxy = v.Y;
                                if (maxz < v.Z) maxz = v.Z;
                                if (minx > v.X) minx = v.X;
                                if (miny > v.Y) miny = v.Y;
                                if (minz > v.Z) minz = v.Z;
                            }
                        }

                    }
                }
            }

            var max = (JArray)accessor["max"];
            max[0] = maxx;
            max[1] = maxy;
            max[2] = maxz;
            var min = (JArray)accessor["min"];
            min[0] = minx;
            min[1] = miny;
            min[2] = minz;
        }
    }
}