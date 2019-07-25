using System;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class RenderPass
    {
        private ResourceLayout[] _resourceLayouts;

        public RenderPass(string name)
        {
            Name = name;
        }
        public string Name { get; private set; }
    }

    public class PassResourceLayout
    {
        public PassResourceLayout()
        {
            
        }
    }

    public class PassLayoutElement
    {
        private readonly ResourceLayoutElementDescription _description;
        private readonly Type _valueType;

        /// <summary>
        /// Constructs a new PassLayoutElement.
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