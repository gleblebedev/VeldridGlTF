namespace Veldrid.SPIRV
{
    public class Uniform
    {
        public string Name { get; internal set; }
        public uint? Set { get; internal set; }
        public uint? Binding { get; internal set; }
        public uint? Size { get; internal set; }
        public string TypeName { get; internal set; }

        public override string ToString()
        {
            return $"layout(set={Set}, binding={Binding}) uniform {TypeName} {Name}";
        }
    }

    public class Input
    {
        public string Name { get; internal set; }
        public uint? Location { get; internal set; }
        public uint? Size { get; internal set; }
        public string TypeName { get; internal set; }

        public override string ToString()
        {
            return $"layout(location={Location}) in {TypeName} {Name}";
        }
    }
}