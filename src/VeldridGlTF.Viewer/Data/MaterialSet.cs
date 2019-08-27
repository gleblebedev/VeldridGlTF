using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Data
{
    public class MaterialSet : IReadOnlyList<IResourceHandler<IMaterial>>
    {
        private readonly IResourceHandler<IMaterial>[] _materials;

        public MaterialSet(params IResourceHandler<IMaterial>[] materials)
        {
            _materials = materials;
        }

        public MaterialSet(IEnumerable<IResourceHandler<IMaterial>> materials)
        {
            _materials = materials.ToArray();
        }

        public IEnumerator<IResourceHandler<IMaterial>> GetEnumerator()
        {
            return ((IList<IResourceHandler<IMaterial>>) _materials).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _materials.GetEnumerator();
        }

        public int Count => _materials.Length;

        public IResourceHandler<IMaterial> this[int index] => _materials[index];

        public override string ToString()
        {
            return string.Join(";", (IEnumerable<IResourceHandler<IMaterial>>) _materials);
        }
    }
}