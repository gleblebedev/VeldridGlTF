using System.Collections;
using System.Collections.Generic;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Components
{
    public class MaterialCollection : IList<IResourceHandler<IMaterial>>
    {
        private readonly Model _model;

        private readonly List<IResourceHandler<IMaterial>> _materials = new List<IResourceHandler<IMaterial>>(1);

        public MaterialCollection(Model model)
        {
            _model = model;
        }


        public void Add(IResourceHandler<IMaterial> item)
        {
            _materials.Add(item);
            _model.InvalidateRenderContext();
        }


        public void Clear()
        {
            _materials.Clear();
        }

        public bool Contains(IResourceHandler<IMaterial> item)
        {
            return _materials.Contains(item);
        }


        public void CopyTo(IResourceHandler<IMaterial>[] array, int arrayIndex)
        {
            _materials.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IResourceHandler<IMaterial>> GetEnumerator()
        {
            return _materials.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _materials.GetEnumerator();
        }


        public int IndexOf(IResourceHandler<IMaterial> item)
        {
            return _materials.IndexOf(item);
        }


        public void Insert(int index, IResourceHandler<IMaterial> item)
        {
            _materials.Insert(index, item);
            _model.InvalidateRenderContext();
        }


        public bool Remove(IResourceHandler<IMaterial> item)
        {
            _model.InvalidateRenderContext();
            return _materials.Remove(item);
        }


        public void RemoveAt(int index)
        {
            _model.InvalidateRenderContext();
            _materials.RemoveAt(index);
        }


        public int Count => _materials.Count;

        public IResourceHandler<IMaterial> this[int index]
        {
            get => _materials[index];
            set
            {
                _model.InvalidateRenderContext();
                _materials[index] = value;
            }
        }

        public bool IsReadOnly => false;
    }
}