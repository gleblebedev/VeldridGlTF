using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer1.Resources
{
    [TestFixture]
    public class ResourceHandlerTestFixture
    {
        class Disposable:IDisposable
        {
            public void Dispose()
            {
                IsDisposed = true;
            }

            public bool IsDisposed { get; private set; }
        }
        [Test]
        public async Task Resolve_TypeWithDependencies_SuccessfulyResolved()
        {
            var source = new ManualResourceHandler<int>(ResourceId.Null);
            var values = new List<Disposable>();
            var h = new ResourceHandler<Disposable>(ResourceId.Null, async _=>
            {
                await _.ResolveDependencyAsync(source);
                var v= new Disposable();
                values.Add(v);
                return v;
            }, null);

            Assert.AreEqual(0, values.Count);
            Assert.AreEqual(TaskStatus.WaitingForActivation, h.Status);

            source.SetValue(1);

            Assert.AreEqual(1, values.Count);
            Assert.False(values[0].IsDisposed);

            source.SetValue(2);

            Assert.AreEqual(TaskStatus.RanToCompletion, h.Status);
            Assert.AreEqual(2, values.Count);
            Assert.True(values[0].IsDisposed);
            Assert.False(values[1].IsDisposed);

            h.Dispose();

            Assert.AreEqual(2, values.Count);
            Assert.True(values[0].IsDisposed);
            Assert.True(values[1].IsDisposed);
        }
    }
}