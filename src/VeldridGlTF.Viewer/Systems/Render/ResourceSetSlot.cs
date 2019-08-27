using System;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public enum DynamicResource
    {
        None,
        ObjectProperties,
        JointMatrices,
        JointNormalMatrices,
        MaxValue
    }

    public class ResourceSetSlot
    {
        private readonly BindableResource _bindableResource;
        private readonly ResourceLayoutElementOptions _options;
        private readonly ResourceKind _resourceKind;

        public ResourceSetSlot(string name, ResourceKind resourceKind, BindableResource bindableResource)
            : this(name, resourceKind, ResourceLayoutElementOptions.None, bindableResource)
        {
        }

        public ResourceSetSlot(string name, ResourceKind resourceKind, ResourceLayoutElementOptions options,
            BindableResource bindableResource, DynamicResource dynamicResource = DynamicResource.None)
        {
            if ((options & ResourceLayoutElementOptions.DynamicBinding) != 0 && dynamicResource == DynamicResource.None)
                throw new ArgumentException("Dynamic resource identifier isn't set");
            Name = name ?? "";
            _resourceKind = resourceKind;
            _options = options;
            _bindableResource = bindableResource;
            DynamicResource = dynamicResource;
        }

        public string Name { get; }

        public bool IsDynamic => 0 != (_options & ResourceLayoutElementOptions.DynamicBinding);

        public DynamicResource DynamicResource { get; }

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
            return Name ?? base.ToString();
        }
    }
}