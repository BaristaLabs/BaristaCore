namespace BaristaLabs.BaristaCore.Extensions.Tests
{
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using BaristaLabs.BaristaCore.Modules.Fetch;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class Fetch_Facts
    {
        public IBaristaRuntimeFactory GetRuntimeFactory()
        {
            var myMemoryModuleLoader = new InMemoryModuleLoader();
            myMemoryModuleLoader.RegisterModule(new FetchModule());

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore(moduleLoader: myMemoryModuleLoader);

            var provider = serviceCollection.BuildServiceProvider();
            return provider.GetRequiredService<IBaristaRuntimeFactory>();
        }

        [Fact]
        public void CanFetchText()
        {
            var script = @"
        import fetch from 'barista-fetch';
        
        export default fetch('https://httpbin.org/get');
        ";

            var baristaRuntime = GetRuntimeFactory();

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var response = ctx.EvaluateModule<JsObject>(script);
                        var fnText = response.GetProperty<JsFunction>("text");
                        var textPromise = fnText.Call<JsObject>(response);
                        var text = ctx.Promise.Wait<JsString>(textPromise);
                        Assert.NotNull(text.ToString());
                    }
                }
            }
        }

        [Fact]
        public void CanFetchTextViaScript()
        {
            var script = @"
import fetch from 'barista-fetch';
var fn = (async () => {
    var response = await fetch('https://httpbin.org/get');
    var txt = await response.text();
    return txt;
})();
export default fn;
        ";

            var baristaRuntime = GetRuntimeFactory();

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var text = ctx.EvaluateModule<JsString>(script);
                        Assert.NotNull(text.ToString());
                    }
                }
            }
        }

        [Fact]
        public void CanFetchArrayBuffer()
        {
            Random rnd = new Random();
            var byteLength = rnd.Next(1024);

            var script = $@"
import fetch from 'barista-fetch';
        
export default fetch('https://httpbin.org/bytes/{byteLength}');
        ";

            var baristaRuntime = GetRuntimeFactory();

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var response = ctx.EvaluateModule<JsObject>(script);
                        var fnText = response.GetProperty<JsFunction>("arrayBuffer");
                        var abPromise = fnText.Call<JsObject>(response);
                        var imageBuffer = ctx.Promise.Wait<JsArrayBuffer>(abPromise);
                        var data = imageBuffer.GetArrayBufferStorage();
                        Assert.NotNull(data);
                        Assert.Equal(byteLength, data.Length);
                    }
                }
            }
        }

        [Fact]
        public void CanPostJson()
        {
            var script = @"
import fetch from 'barista-fetch';
var fn = (async () => {
    var response = await fetch('https://httpbin.org/post', { method: 'POST', body: JSON.stringify({ foo: 'bar' }) });
    var json = await response.json();
    return JSON.stringify(json);
})();
export default fn;
        ";

            var baristaRuntime = GetRuntimeFactory();

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var response = ctx.EvaluateModule<JsString>(script);
                        Assert.NotNull(response);
                        var jsonData = response.ToString();
                        Assert.True(jsonData.Length > 0);
                        dynamic data = JsonConvert.DeserializeObject(jsonData);
                        Assert.Equal("bar", (string)data.json.foo);
                    }
                }
            }
        }

        [Fact]
        public void CanSetHeadersFromPOJO()
        {
            var script = @"
import fetch from 'barista-fetch';
var fn = (async () => {
    var response = await fetch('https://httpbin.org/post', { method: 'POST', headers: { 'X-Foobar': 'foobar' }, body: JSON.stringify({ foo: 'bar' }) });
    var json = await response.json();
    return JSON.stringify(json);
})();
export default fn;
        ";

            var baristaRuntime = GetRuntimeFactory();

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var response = ctx.EvaluateModule<JsString>(script);
                        Assert.NotNull(response);
                        var jsonData = response.ToString();
                        Assert.True(jsonData.Length > 0);
                        dynamic data = JsonConvert.DeserializeObject(jsonData);
                        Assert.Equal("foobar", (string)data.headers["X-Foobar"]);
                    }
                }
            }
        }

        [Fact]
        public void CanSetHeadersFromHeadersObj()
        {
            var script = @"
import fetch from 'barista-fetch';
var fn = (async () => {
    var headers = new fetch.Headers();
    headers.append('X-Pure-and-natural', 'wild-caught-fish');
    var response = await fetch('https://httpbin.org/post', { method: 'POST', headers: headers, body: JSON.stringify({ foo: 'bar' }) });
    var json = await response.json();
    return JSON.stringify(json);
})();
export default fn;
        ";

            var baristaRuntime = GetRuntimeFactory();

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var response = ctx.EvaluateModule<JsString>(script);
                        Assert.NotNull(response);
                        var jsonData = response.ToString();
                        Assert.True(jsonData.Length > 0);
                        dynamic data = JsonConvert.DeserializeObject(jsonData);
                        //Assert.Equal("wild-caught-fish", (string)data.headers["X-Pure-and-natural"]);
                    }
                }
            }
        }
    }
}
