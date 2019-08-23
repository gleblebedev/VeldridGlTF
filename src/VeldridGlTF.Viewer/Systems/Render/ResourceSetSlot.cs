using System;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public enum DynamicResource
    {
        None,
        ObjectProperties,
        MaxValue
    }
    public class ResourceSetSlot
    {
        private readonly string _name;
        private readonly ResourceKind _resourceKind;
        private readonly ResourceLayoutElementOptions _options;
        private readonly BindableResource _bindableResource;
        private readonly DynamicResource _dynamicResource;

        public ResourceSetSlot(string name, ResourceKind resourceKind, BindableResource bindableResource)
        :this(name, resourceKind, ResourceLayoutElementOptions.None, bindableResource)
        {

        }

        public ResourceSetSlot(string name, ResourceKind resourceKind, ResourceLayoutElementOptions options, BindableResource bindableResource, DynamicResource dynamicResource = DynamicResource.None)
        {
            if ((options & ResourceLayoutElementOptions.DynamicBinding) != 0 && dynamicResource == DynamicResource.None)
            {
                throw new ArgumentException("Dynamic resource identifier isn't set");
            }
            _name = name ?? "";
            _resourceKind = resourceKind;
            _options = options;
            _bindableResource = bindableResource;
            _dynamicResource = dynamicResource;
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsDynamic
        {
            get { return 0 != (_options & ResourceLayoutElementOptions.DynamicBinding); }
        }

        public DynamicResource DynamicResource
        {
            get { return _dynamicResource; }
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