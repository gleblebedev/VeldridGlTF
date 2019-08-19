using CommandLine;

namespace FixGltfIssues
{
    public class Options
    {
        [Option('i',"input",Required = true, HelpText = "Input file name (GLTF or GLB)")]
        public string Input { get; set; }
        [Option('o', "output", Required = true, HelpText = "Output file name (GLB)")]
        public string Output { get; set; }
    }
}