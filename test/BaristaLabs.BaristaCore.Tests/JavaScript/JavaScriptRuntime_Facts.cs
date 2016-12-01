namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Xunit;

    public class JavaScriptRuntime_Facts
    {
        private IServiceProvider Provider;

        public JavaScriptRuntime_Facts()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore();
            
            Provider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public void JavaScriptRuntimeCanBeConstructed()
        {
            using (var rt = JavaScriptRuntime.CreateRuntime(Provider))
            {
            }
            Assert.True(true);
        }

        [Fact]
        public void JavaScriptRuntimeShouldReturnRuntimeMemoryUsage()
        {
            ulong memoryUsage;
            using (var rt = JavaScriptRuntime.CreateRuntime(Provider))
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

            using (var rt = JavaScriptRuntime.CreateRuntime(Provider))
            {
                rt.MemoryChanging += handler;
                using (var ctx = rt.CreateContext())
                {
                }
                rt.MemoryChanging -= handler;
            }
            Assert.True(changeCount > 0);
        }
    }
}
