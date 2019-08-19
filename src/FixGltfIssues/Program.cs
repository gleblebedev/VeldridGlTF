using System;
using System.IO;
using System.Security.Cryptography;
using CommandLine;

namespace FixGltfIssues
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = (CommandLine.Parser.Default.ParseArguments<Options>(args) as Parsed<Options>)?.Value;
            if (options == null)
                return;

            var ex = Path.GetExtension(options.Input);
            GltfFile file = new GltfFile();
            if (string.Equals(ex,".glb", StringComparison.InvariantCultureIgnoreCase))
                file.ReadGlb(options.Input);
            else if (string.Equals(ex, ".gltf", StringComparison.InvariantCultureIgnoreCase))
                file.ReadGltf(options.Input);
            else
                throw new ArgumentException("not a gltf or glb file");

            file.FixNormals();

            file.WriteGlb(options.Output);

        }
    }
}
