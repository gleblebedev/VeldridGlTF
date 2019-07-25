using System;
using System.Collections.Generic;
using System.Linq;

namespace Veldrid.SPIRV
{
    public class SpirvCompilationResultEx: SpirvCompilationResult
    {
        private readonly ResourceLayoutDescription[] _layouts;

        public SpirvCompilationResultEx(byte[] spirvBytes, ResourceLayoutDescription[] layouts) : base(spirvBytes)
        {
            _layouts = layouts;
        }

        public ResourceLayoutDescription[] Layouts
        {
            get { return _layouts; }
        }

        public static ResourceLayoutDescription[] Merge(ResourceLayoutDescription[] a, ResourceLayoutDescription[] b)
        {
            var size = Math.Max(a.Length,b.Length);
            var res = new ResourceLayoutDescription[size];
            for (int setIndex = 0; setIndex < size; setIndex++)
            {
                if (setIndex >= a.Length)
                    res[setIndex] = b[setIndex];
                else if (setIndex >= b.Length)
                    res[setIndex] = a[setIndex];
                else
                    res[setIndex] = Merge(a[setIndex], b[setIndex]);
            }
            return res;
        }

        public static ResourceLayoutDescription Merge(ResourceLayoutDescription a, ResourceLayoutDescription b)
        {
            var size = Math.Max(a.Elements.Length, b.Elements.Length);
            var res = new ResourceLayoutElementDescription[size];
            for (int setIndex = 0; setIndex < size; setIndex++)
            {
                if (setIndex >= a.Elements.Length)
                    res[setIndex] = b.Elements[setIndex];
                else if (setIndex >= b.Elements.Length)
                    res[setIndex] = a.Elements[setIndex];
                else
                    res[setIndex] = Merge(a.Elements[setIndex], b.Elements[setIndex]);
            }
            return new ResourceLayoutDescription(res);
        }

        private static ResourceLayoutElementDescription Merge(ResourceLayoutElementDescription a, ResourceLayoutElementDescription b)
        {
            if (a.Stages == ShaderStages.None)
                return b;
            if (b.Stages == ShaderStages.None)
                return a;
            if (a.Name != b.Name)
                throw new Exception("Resource layout mismatch: expected "+a.Name+" but other shader has "+b.Name);
            if (a.Kind != b.Kind)
                throw new Exception("Resource layout mismatch: expected " + a.Name + " but other shader has " + b.Name);
            return new ResourceLayoutElementDescription(a.Name, a.Kind, a.Stages | b.Stages, a.Options | b.Options);
        }
    }
}