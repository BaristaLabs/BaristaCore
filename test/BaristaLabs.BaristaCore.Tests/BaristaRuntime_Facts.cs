namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.JavaScript;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Xunit;

    public class BaristaRuntime_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public BaristaRuntime_Facts()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddBaristaCore();

            m_provider = ServiceCollection.BuildServiceProvider();
        }

        public IBaristaRuntimeService BaristaRuntimeService
        {
            get { return m_provider.GetRequiredService<IBaristaRuntimeService>(); }
        }

        [Fact]
        public void JavaScriptRuntimeCanBeConstructed()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
            }
            Assert.True(true);
        }

        [Fact]
        public void JavaScriptRuntimeShouldReturnRuntimeMemoryUsage()
        {
            ulong memoryUsage;
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                memoryUsage = rt.RuntimeMemoryUsage;
            }
            Assert.True(memoryUsage > 0);
        }

        [Fact]
        public void JavaScriptRuntimeShouldFireMemoryChangingCallbacks()
        {
            int changeCount = 0;
            void handler(object sender, JavaScriptMemoryEventArgs e)
            {
                changeCount++;
            }

            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                rt.MemoryAllocationChanging += handler;
                using (var ctx = rt.CreateContext())
                {
                }
                rt.MemoryAllocationChanging -= handler;
            }
            Assert.True(changeCount > 0);
        }

        [Fact]
        public void JavaScriptRuntimeShouldFireGarbageCollectionCallbacks()
        {
            int collectingCount = 0;
            EventHandler<EventArgs> handler = (sender, e) =>
            {
                collectingCount++;
            };

            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                rt.GarbageCollecting += handler;
                rt.CollectGarbage();
                rt.GarbageCollecting -= handler;
            }

            Assert.True(collectingCount > 0);
        }

        [Fact]
        public void JavaScriptRuntimeShouldIndicateDisposedState()
        {
            var rt = BaristaRuntimeService.CreateRuntime();
            rt.Dispose();

            Assert.True(rt.IsDisposed);

            //Explicitly Dispose of the RuntimePool of the provider to check full disposal.
            ServiceCollection.FreeBaristaCoreServices();
        }
    }
}
