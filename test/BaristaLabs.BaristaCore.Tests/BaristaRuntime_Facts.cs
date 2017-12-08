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
        private readonly IServiceProvider Provider;

        public BaristaRuntime_Facts()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddBaristaCore();
            
            Provider = ServiceCollection.BuildServiceProvider();
        }

        [Fact]
        public void JavaScriptRuntimeCanBeConstructed()
        {
            using (var rt = BaristaRuntime.CreateRuntime(Provider))
            {
            }
            Assert.True(true);
        }

        [Fact]
        public void JavaScriptRuntimeShouldReturnRuntimeMemoryUsage()
        {
            ulong memoryUsage;
            using (var rt = BaristaRuntime.CreateRuntime(Provider))
            {
                memoryUsage = rt.RuntimeMemoryUsage;
            }
            Assert.True(memoryUsage > 0);
        }

        [Fact]
        public void JavaScriptRuntimeShouldFireMemoryChangingCallbacks()
        {
            int changeCount = 0;
            EventHandler<JavaScriptMemoryEventArgs> handler = (sender, e) =>
            {
                changeCount++;
            };

            using (var rt = BaristaRuntime.CreateRuntime(Provider))
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

            using (var rt = BaristaRuntime.CreateRuntime(Provider))
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
            var rt = BaristaRuntime.CreateRuntime(Provider);
            rt.Dispose();

            Assert.True(rt.IsDisposed);

            //Explicitly Dispose of the RuntimePool of the provider to check full disposal.
            ServiceCollection.FreeBaristaCoreServices();
        }
    }
}
