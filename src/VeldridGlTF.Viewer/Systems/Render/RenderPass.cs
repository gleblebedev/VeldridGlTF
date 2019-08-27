using System;
using Veldrid;
using Veldrid.SPIRV;
using VeldridGlTF.Viewer.Systems.Render.Uniforms;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class RenderPass : ILayoutNameResolver
    {
        private PassResourceLayout[] _resourceLayouts;

        public RenderPass(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public string Resolve(uint set, uint binding, ResourceKind kind, out ResourceLayoutElementOptions options)
        {
            options = ResourceLayoutElementOptions.None;
            if (_resourceLayouts == null || _resourceLayouts.Length <= set)
                return null;
            var layout = _resourceLayouts[set];
            var name = layout.ResolveName(binding);
            options = Resolve(name, set, binding, kind);
            return name;
        }

        public ResourceLayoutElementOptions Resolve(string name, uint set, uint binding, ResourceKind kind)
        {
            if (name == nameof(ObjectProperties))
                return ResourceLayoutElementOptions.DynamicBinding;
            return ResourceLayoutElementOptions.None;
        }
    }

    public class PassResourceLayout
    {
        public string ResolveName(uint binding)
        {
            return null;
        }
    }

    public class PassLayoutElement
    {
        private readonly ResourceLayoutElementDescription _description;
        private readonly Type _valueType;

        /// <summary>
        ///     Constructs a new PassLayoutElement.
        /// </summary>
        /// <param name="description">Element description.</param>
        /// <param name="valueType">The type of the element content.</param>
        public PassLayoutElement(
            ResourceLayoutElementDescription description,
            Type valueType)
        {
            _description = description;
            _valueType = valueType;
        }
    }
}