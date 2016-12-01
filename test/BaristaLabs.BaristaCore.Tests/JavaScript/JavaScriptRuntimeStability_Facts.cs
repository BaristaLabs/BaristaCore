namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Xunit;

    public class JavaScriptRuntimeStability_Facts
    {
        private IServiceProvider Provider;

        public JavaScriptRuntimeStability_Facts()
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
    }
}
