using System.Collections.Generic;
using System.Linq;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class ResourceSetBuilder : IResourceSetBuilder
    {
        private static readonly ResourceSetSlot[] _emptySetSlotArray = new ResourceSetSlot[0];
        private static readonly BindableResource[] _emptyBindableResourceArray = new BindableResource[0];

        private static readonly ResourceSetSlot _emptyResourceSetSlot =
            new ResourceSetSlot("", ResourceKind.UniformBuffer, ResourceLayoutElementOptions.None, null);

        private readonly ResourceFactory _factory;
        private readonly IResourceSetBuilder _fallback;
        private readonly Dictionary<string, ResourceSetSlot> _slots = new Dictionary<string, ResourceSetSlot>();

        public ResourceSetBuilder(ResourceFactory factory, params ResourceSetSlot[] slots) : this(factory, null, slots)
        {
        }

        public ResourceSetBuilder(ResourceFactory factory, IResourceSetBuilder fallback, params ResourceSetSlot[] slots)
            : this(factory, fallback, (IEnumerable<ResourceSetSlot>) slots)
        {
        }

        public ResourceSetBuilder(ResourceFactory factory, IResourceSetBuilder fallback,
            IEnumerable<ResourceSetSlot> slots)
        {
            _factory = factory;
            _slots = slots.ToDictionary(_ => _.Name ?? "");
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
                if (_fallback == null) return false;

                return _fallback.TryResolve(name, out slot);
            }

            return true;
        }

        public ResourceSetSlot[] Resolve(params string[] resources)
        {
            if (resources == null || resources.Length == 0) return _emptySetSlotArray;
            var res = new ResourceSetSlot[resources.Length];
            for (var index = 0; index < resources.Length; index++)
                if (!TryResolve(resources[index], out res[index]))
                    throw new KeyNotFoundException("Unknown bindable resource " + resources[index]);

            return res;
        }

        public BindableResource[] Resolve(BindableResource emptyBindableResource,
            ResourceLayoutElementDescription[] resources, out DynamicResource[] offsets)
        {
            offsets = null;
            if (resources == null || resources.Length == 0) return _emptyBindableResourceArray;

            List<DynamicResource> offsetList = null;
            var res = new BindableResource[resources.Length];
            for (var index = 0; index < resources.Length; index++)
            {
                if (!TryResolve(resources[index].Name, out var slot))
                    throw new KeyNotFoundException("Unknown bindable resource " + resources[index].Name);

                if (slot.IsDynamic)
                {
                    offsetList = offsetList ?? new List<DynamicResource>();
                    offsetList.Add(slot.DynamicResource);
                    res[index] = slot.GetResource() ?? emptyBindableResource;
                    //res[index] = new DeviceBufferRange((DeviceBuffer)slot.GetResource(), 256,256);
                }
                else
                {
                    res[index] = slot.GetResource() ?? emptyBindableResource;
                }

                //res[index] = slot.GetResource() ?? emptyBindableResource;
            }

            if (offsetList != null) offsets = offsetList.ToArray();
            return res;
        }

        public void Dispose()
        {
        }
    }
}