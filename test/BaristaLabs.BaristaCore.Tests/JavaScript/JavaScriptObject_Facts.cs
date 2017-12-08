namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Xunit;

    public class JavaScriptObject_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider Provider;

        public JavaScriptObject_Facts()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddBaristaCore();

            Provider = ServiceCollection.BuildServiceProvider();
        }

        [Fact]
        public void JsObjectCanBeCreated()
        {
            using (var rt = BaristaRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        
                    }
                }
            }
        }
    }
}
