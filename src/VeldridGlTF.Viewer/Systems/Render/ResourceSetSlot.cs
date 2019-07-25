using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class ResourceSetSlot
    {
        private readonly string _name;
        private readonly ResourceKind _resourceKind;
        private readonly ResourceLayoutElementOptions _options;
        private readonly BindableResource _bindableResource;

        public ResourceSetSlot(string name, ResourceKind resourceKind, BindableResource bindableResource)
        :this(name, resourceKind, ResourceLayoutElementOptions.None, bindableResource)
        {

        }

        public ResourceSetSlot(string name, ResourceKind resourceKind, ResourceLayoutElementOptions options, BindableResource bindableResource)
        {
            _name = name;
            _resourceKind = resourceKind;
            _options = options;
            _bindableResource = bindableResource;
        }

        public string Name
        {
            get { return _name; }
        }

        public ResourceLayoutElementDescription GetElementDescription(ShaderStages stages)
        {
            return new ResourceLayoutElementDescription(Name, _resourceKind, stages, _options);
        }

        public BindableResource GetResource()
        {
            return _bindableResource;
        }

        public override string ToString()
        {
            return _name ?? base.ToString();
        }
    }
}