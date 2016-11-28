namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Xunit;

    public class JavaScriptRuntimeStability_Facts
    {
        private IServiceProvider Provider;

        public JavaScriptRuntimeStability_Facts()
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
    }
}
