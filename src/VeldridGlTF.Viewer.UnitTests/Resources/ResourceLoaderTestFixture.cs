using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer1.Resources
{
    [TestFixture]
    class ResourceLoaderTestFixture
    {
        [Test]
        public async Task Resolve_TypeWithDependencies_SuccessfulyResolved()
        {
            var rm =new ResourceManager();
            rm.Register(new GenAsyncA<TypeA>(new TypeA() { Value = 12}));
            rm.Register(new GenAsyncA<TypeB>(new TypeB() { Value = 15 }));
            rm.Register(new Loader<TypeC>(async (ResourceContext context) => 
            {
                var _a = await context.ResolveDependencyAsync<TypeA>(new ResourceId("A"));
                var _b = await context.ResolveDependencyAsync<TypeB>(new ResourceId("B"));
                return new TypeC() { Value = _a.Value + _b.Value };
            }));

            var r = rm.Resolve<TypeC>(new ResourceId("C"));
            var res = await r.GetAsync();
            Assert.AreEqual(12+15, res.Value);

            var a = rm.Resolve<TypeA>(new ResourceId("A"));
            var b = rm.Resolve<TypeA>(new ResourceId("B"));
            //TODO: invalidate resource
        }

        [Test]
        public async Task Resolve_TypeWithDependenciesAndDependenciesChanged_ValueIsRevaluated()
        {
            var rm = new ResourceManager();
            var typeA = new TypeA() { Value = 12 };
            var typeB = new TypeB() { Value = 15 };
            rm.Register(new GenAsyncA<TypeA>(typeA));
            rm.Register(new GenAsyncA<TypeB>(typeB));
            rm.Register(new Loader<TypeC>(async (ResourceContext context) =>
            {
                var _a = await context.ResolveDependencyAsync<TypeA>(new ResourceId("A"));
                var _b = await context.ResolveDependencyAsync<TypeB>(new ResourceId("B"));
                return new TypeC() { Value = _a.Value + _b.Value };
            }));

            var r = rm.Resolve<TypeC>(new ResourceId("C"));
            var res = await r.GetAsync();
            Assert.AreEqual(typeA.Value+typeB.Value, res.Value);

            typeA.Value += 10;
            typeA.Value += 20;

            ((ResourceHandler<TypeA>)rm.Resolve<TypeA>(new ResourceId("A"))).Invalidate();
            ((ResourceHandler<TypeA>)rm.Resolve<TypeA>(new ResourceId("B"))).Invalidate();

            res = await r.GetAsync();
            Assert.AreEqual(typeA.Value + typeB.Value, res.Value);
        }

        [Test]
        public async Task Resolve_DependencyInvalidatedWhileWeWereWaitingOnIt_ValueIsRevaluated()
        {
            var rm = new ResourceManager();
            var taskA = new TaskCompletionSource<TypeA>();
            rm.Register(new Loader<TypeA>(async (ResourceContext context) => { return await taskA.Task; }));
            rm.Register(new Loader<TypeC>(async (ResourceContext context) =>
            {
                var _ = await context.ResolveDependencyAsync<TypeA>(new ResourceId("A"));
                if (context.Token.IsCancellationRequested)
                    return default(TypeC);
                return new TypeC() {Value = _.Value};
            }));

            var handler = rm.Resolve<TypeC>(new ResourceId("A"));
            Assert.AreNotEqual(TaskStatus.RanToCompletion, handler.Status);
            var a = rm.Resolve<TypeA>(new ResourceId("A"));
            ((ResourceHandler<TypeA>)a).Invalidate();
            taskA.SetResult(new TypeA(){Value = 42});
            var res = await handler.GetAsync();
            Assert.AreEqual(42, res.Value);
        }

        [Test]
        public async Task ResolveTwice_LoaderThrowsException_LoadAsyncExecutedOnce()
        {
            var rm = new ResourceManager();
            int counter = 0;
            rm.Register(new Loader<TypeC>((ResourceContext context) =>
            {
                ++counter; throw new NotImplementedException(); }));

            var r = rm.Resolve<TypeC>(new ResourceId("C"));
            try
            {
                await r.GetAsync();
            }
            catch (Exception)
            {

            }
            try
            {
                await r.GetAsync();
            }
            catch (Exception)
            {

            }

            Assert.AreEqual(1, counter);
        }

        class GenAsyncA<T> : ResourceLoader<T>
        {
            private readonly T _value;

            public GenAsyncA(T value)
            {
                _value = value;
            }
            public override Task<T> LoadAsync(ResourceContext context)
            {
                return Task.Run(() => _value);
            }
        }

        class Loader<T> : ResourceLoader<T>
        {
            private readonly Func<ResourceContext, Task<T>> _loader;

            public Loader(Func<ResourceContext, Task<T>> loader)
            {
                _loader = loader;
            }
            public override Task<T> LoadAsync(ResourceContext context)
            {
                return _loader(context);
            }
        }

        class TypeA
        {
            public int Value;
        }
        class TypeB
        {
            public int Value;
        }
        class TypeC
        {
            public int Value;
        }
    }
}
