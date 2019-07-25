using System.Collections.Generic;
using System.Linq;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class ResourceSetBuilder: IResourceSetBuilder
    {
        private readonly ResourceFactory _factory;
        private readonly Dictionary<string, ResourceSetSlot> _slots = new Dictionary<string, ResourceSetSlot>();
        private readonly IResourceSetBuilder _fallback;
        private readonly static ResourceSetSlot[] _emptySetSlotArray = new ResourceSetSlot[0];
        private readonly static BindableResource[] _emptyBindableResourceArray = new BindableResource[0];
        private readonly static ResourceSetSlot _emptyResourceSetSlot = new ResourceSetSlot("",ResourceKind.UniformBuffer, ResourceLayoutElementOptions.None, null);

        public ResourceSetBuilder(ResourceFactory factory, params ResourceSetSlot[] slots) : this(factory,null,slots)
        {
        }

        public ResourceSetBuilder(ResourceFactory factory, IResourceSetBuilder fallback, params ResourceSetSlot[] slots)
        {
            _factory = factory;
            _slots = slots.ToDictionary(_=>_.Name??"");
            _fallback = fallback;
        }

        public bool TryResolve(string name, out ResourceSetSlot slot)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                slot = _emptyResourceSetSlot;
                return true;
            }
            if (!_slots.TryGetValue(name, out slot))
            {
                if (_fallback == null)
                {
                    return false;
                }

                return _fallback.TryResolve(name, out slot);
            }

            return true;
        }

        public ResourceSetSlot[] Resolve(params string[] resources)
        {
            if (resources == null || resources.Length == 0)
            {
                return _emptySetSlotArray;
            }
            var res = new ResourceSetSlot[resources.Length];
            for (var index = 0; index < resources.Length; index++)
            {
                if (!TryResolve(resources[index], out res[index]))
                {
                    throw new KeyNotFoundException("Unknown bindable resource "+resources[index]);
                }
            }

            return res;
        }
        public BindableResource[] Resolve(BindableResource emptyBindableResource, params ResourceLayoutElementDescription[] resources)
        {
            if (resources == null || resources.Length == 0)
            {
                return _emptyBindableResourceArray;
            }
            var res = new BindableResource[resources.Length];
            for (var index = 0; index < resources.Length; index++)
            {
                if (!TryResolve(resources[index].Name, out var slot))
                {
                    throw new KeyNotFoundException("Unknown bindable resource " + resources[index]);
                }

                res[index] = slot.GetResource() ?? emptyBindableResource;
            }

            return res;
        }
        public void Dispose()
        {
        }
    }
}