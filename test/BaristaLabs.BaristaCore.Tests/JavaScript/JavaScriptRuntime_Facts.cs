namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Xunit;

    public class JavaScriptRuntime_Facts
    {
        private IServiceProvider Provider;

        public JavaScriptRuntime_Facts()
        {
            var serviceCollection = new ServiceCollection();

            var chakraEngine = JavaScriptEngineFactory.CreateChakraEngine();

            serviceCollection.AddSingleton(chakraEngine);
            serviceCollection.AddSingleton(new JavaScriptRuntime.JavaScriptRuntimeObserver(chakraEngine));

            Provider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public void JavaScriptRuntimeCanBeConstructed()
        {
            using (var rt = JavaScriptRuntime.CreateJavaScriptRuntime(Provider))
            {
            }
            Assert.True(true);
        }

        [Fact]
        public void JavaScriptRuntimeShouldReturnRuntimeMemoryUsage()
        {
            ulong memoryUsage;
            using (var rt = JavaScriptRuntime.CreateJavaScriptRuntime(Provider))
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

            using (var rt = JavaScriptRuntime.CreateJavaScriptRuntime(Provider))
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
