namespace BaristaLabs.BaristaCore.Extensions.Tests
{
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using BaristaLabs.BaristaCore.Modules;
    using Microsoft.Extensions.DependencyInjection;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class Xml_Facts
    {
        public IBaristaRuntimeFactory GetRuntimeFactory()
        {
            var myMemoryModuleLoader = new InMemoryModuleLoader();
            myMemoryModuleLoader.RegisterModule(new Xml2JsModule());

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore(moduleLoader: myMemoryModuleLoader);

            var provider = serviceCollection.BuildServiceProvider();
            return provider.GetRequiredService<IBaristaRuntimeFactory>();
        }

        [Fact]
        public void CanConvertJsonToXml()
        {
            var script = @"
import xml2js from 'xml2js';
var json = { foo: 'bar' };
export default xml2js.toXml(JSON.stringify(json));
";

            var baristaRuntime = GetRuntimeFactory();

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var response = ctx.EvaluateModule(script);
                        Assert.NotNull(response);
                        Assert.IsType<JsString>(response);
                        Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-16\"?><foo>bar</foo>", response.ToString());
                    }
                }
            }
        }

        [Fact]
        public void JsonToXmlNullArgumentThrows()
        {
            var script = @"
import xml2js from 'xml2js';
var json = { foo: 'bar' };
export default xml2js.toXml(null);
";

            var baristaRuntime = GetRuntimeFactory();

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.Throws<JsScriptException>(() =>
                        {
                            var response = ctx.EvaluateModule(script);
                        });
                    }
                }
            }
        }

        [Fact]
        public void CanConvertXmlToJson()
        {
            var script = @"
import xml2js from 'xml2js';
var xml = '<?xml version=""1.0"" encoding=""utf-16""?><foo>bar</foo>';
export default JSON.stringify(xml2js.toJson(xml, { object: true }));
";

            var baristaRuntime = GetRuntimeFactory();

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var response = ctx.EvaluateModule(script);
                        Assert.NotNull(response);
                        Assert.IsType<JsString>(response);
                        Assert.Equal("{\"?xml\":{\"@version\":\"1.0\",\"@encoding\":\"utf-16\"},\"foo\":\"bar\"}", response.ToString());
                    }
                }
            }
        }

        [Fact]
        public void XmlToJsonNullArgumentThrows()
        {
            var script = @"
import xml2js from 'xml2js';
var json = { foo: 'bar' };
export default xml2js.toJson(null, { object: true });
";

            var baristaRuntime = GetRuntimeFactory();

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.Throws<JsScriptException>(() =>
                        {
                            var response = ctx.EvaluateModule(script);
                        });
                    }
                }
            }
        }
    }
}
