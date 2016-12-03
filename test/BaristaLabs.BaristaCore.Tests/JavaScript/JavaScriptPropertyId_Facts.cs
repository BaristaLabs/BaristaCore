namespace BaristaLabs.BaristaCore.JavaScript
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Xunit;

    public class JavaScriptPropertyId_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider Provider;

        public JavaScriptPropertyId_Facts()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddBaristaCore();

            Provider = ServiceCollection.BuildServiceProvider();
        }

        [Fact]
        public void JsPropertyIdCanBeCreated()
        {
            using (var rt = JavaScriptRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        //TODO: This might not be the final signature -- creating a propertyid requires a current context.
                        var propertyId = JavaScriptPropertyId.FromString(rt.Engine, "foo");
                        Assert.NotNull(propertyId);
                        propertyId.Dispose();
                    }
                }
            }
        }
    }
}
