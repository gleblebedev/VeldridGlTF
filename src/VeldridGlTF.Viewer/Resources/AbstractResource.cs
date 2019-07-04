﻿namespace VeldridGlTF.Viewer.Resources
{
    public class AbstractResource
    {
        public AbstractResource(ResourceId id)
        {
            Id = id;
        }

        public ResourceId Id { get; }

        public override string ToString()
        {
            return $"{Id.Container}/{Id.Id}<{GetType().Name}>";
        }
    }
}