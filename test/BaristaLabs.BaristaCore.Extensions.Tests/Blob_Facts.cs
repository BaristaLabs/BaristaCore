namespace BaristaLabs.BaristaCore.Extensions.Tests
{
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using BaristaLabs.BaristaCore.Modules;
    using Microsoft.Extensions.DependencyInjection;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class Blob_Facts
    {
        public IBaristaRuntimeFactory GetRuntimeFactory()
        {
            var myMemoryModuleLoader = new InMemoryModuleLoader();
            myMemoryModuleLoader.RegisterModule(new BlobModule());

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore(moduleLoader: myMemoryModuleLoader);

            var provider = serviceCollection.BuildServiceProvider();
            return provider.GetRequiredService<IBaristaRuntimeFactory>();
        }

        [Fact]
        public void CanCreateBlob()
        {
            var script = @"
        import Blob from 'barista-blob';
        var aFileParts = ['<a id=""a""><b id=""b"">hey!</b></a>']; // an array consisting of a single DOMString
        var oMyBlob = new Blob(aFileParts, { type : 'text/html'}); // the blob
        export default oMyBlob;
        ";

            var baristaRuntime = GetRuntimeFactory();

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var response = ctx.EvaluateModule<JsObject>(script);
                        Assert.NotNull(response);
                        if (response.TryGetBean(out JsExternalObject exObj) && exObj.Target is Blob myBlob)
                        {
                            Assert.Equal(32, myBlob.Size);
                            Assert.Equal("text/html", myBlob.Type);

                            Assert.Equal(@"<a id=""a""><b id=""b"">hey!</b></a>", Encoding.UTF8.GetString(myBlob.Data));
                        }
                        else
                        {
                            Assert.True(false, "Result object should have a blob bean");
                        }
                    }
                }
            }
        }

        [Fact]
        public void CanSliceBlob()
        {
            var script = @"
        import Blob from 'barista-blob';
        var aFileParts = ['<a id=""a""><b id=""b"">hey!</b></a>']; // an array consisting of a single DOMString
        var oMyBlob = new Blob(aFileParts, { type : 'text/html'}); // the blob
        var slicedBlob = oMyBlob.slice(10, 28, 'text/html');
        export default slicedBlob;
        ";

            var baristaRuntime = GetRuntimeFactory();

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var response = ctx.EvaluateModule<JsObject>(script);
                        Assert.NotNull(response);
                        if (response.TryGetBean(out JsExternalObject exObj) && exObj.Target is Blob myBlob)
                        {
                            Assert.Equal(18, myBlob.Size);
                            Assert.Equal("text/html", myBlob.Type);

                            Assert.Equal(@"<b id=""b"">hey!</b>", Encoding.UTF8.GetString(myBlob.Data));
                        }
                        else
                        {
                            Assert.True(false, "Result object should have a blob bean");
                        }
                    }
                }
            }
        }
    }
}