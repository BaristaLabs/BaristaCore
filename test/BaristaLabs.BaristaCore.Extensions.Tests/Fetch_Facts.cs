namespace BaristaLabs.BaristaCore.Extensions.Tests
{
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using BaristaLabs.BaristaCore.Modules;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using System;
    using Xunit;

    public class Fetch_Facts
    {
        public IBaristaRuntimeFactory GetRuntimeFactory()
        {
            var myMemoryModuleLoader = new InMemoryModuleLoader();
            myMemoryModuleLoader.RegisterModule(new BlobModule());
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
        public void CanPostAndGetJson()
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
                        var promise = ctx.EvaluateModule<JsObject>(script);
                        var response = ctx.Promise.Wait<JsString>(promise);
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
        public void CanPostAndGetBlob()
        {
            var script = @"
import Blob from 'barista-blob';
import fetch from 'barista-fetch';
var fn = (async () => {
    var myBlob = Blob.fromUtf8String(JSON.stringify({ foo: 'bar' }));
    var response = await fetch('https://httpbin.org/post', { method: 'POST', body: myBlob });
    var blob = await response.blob();
    return blob.toUtf8String();
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
                        var promise = ctx.EvaluateModule<JsString>(script);
                        var response = ctx.Promise.Wait<JsObject>(promise);
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
        public void CanPostAndGetArrayBuffer()
        {
            var script = @"
import fetch from 'barista-fetch';
var fn = (async () => {
    function ab2str(buf) {
      return String.fromCharCode.apply(null, new Uint8Array(buf));
    };

    function str2ab(str) {
      var buf = new ArrayBuffer(str.length);
      var bufView = new Uint8Array(buf);
      for (var i=0, strLen=str.length; i<strLen; i++) {
        bufView[i] = str.charCodeAt(i);
      }
      return buf;
    };

    var myAB = str2ab(JSON.stringify({ foo: 'bar' }));
    var response = await fetch('https://httpbin.org/post', { method: 'POST', body: myAB });
    var ab = await response.arrayBuffer();
    return ab2str(ab);
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
                        var promise = ctx.EvaluateModule<JsObject>(script);
                        var response = ctx.Promise.Wait<JsString>(promise);
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
        public void CanPostAndGetText()
        {
            var script = @"
import fetch from 'barista-fetch';
var fn = (async () => {
    var response = await fetch('https://httpbin.org/post', { method: 'POST', body: 'hello, world!' });
    var text = await response.text();
    return text;
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
                        var promise = ctx.EvaluateModule<JsObject>(script);
                        var response = ctx.Promise.Wait<JsString>(promise);
                        Assert.NotNull(response);
                        var jsonData = response.ToString();
                        Assert.True(jsonData.Length > 0);
                        dynamic data = JsonConvert.DeserializeObject(jsonData);
                        Assert.Equal("hello, world!", (string)data.data);
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
                        var promise = ctx.EvaluateModule<JsObject>(script);
                        var response = ctx.Promise.Wait<JsString>(promise);
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
                        var promise = ctx.EvaluateModule<JsObject>(script);
                        var response = ctx.Promise.Wait<JsString>(promise);
                        Assert.NotNull(response);
                        var jsonData = response.ToString();
                        Assert.True(jsonData.Length > 0);
                        dynamic data = JsonConvert.DeserializeObject(jsonData);
                        Assert.Equal("wild-caught-fish", (string)data.headers["X-Pure-And-Natural"]); //Auto-Pascal-Casing is a thing, apparently, but confirmed that it's coming from httpbin
                    }
                }
            }
        }

        [Fact]
        public void CanSetReferrer()
        {
            var script = @"
import fetch from 'barista-fetch';
var fn = (async () => {
    var response = await fetch('https://httpbin.org/post', { method: 'POST', referrer: 'http://foo', body: JSON.stringify({ foo: 'bar' }) });
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
                        var promise = ctx.EvaluateModule<JsObject>(script);
                        var response = ctx.Promise.Wait<JsString>(promise);
                        Assert.NotNull(response);
                        var jsonData = response.ToString();
                        Assert.True(jsonData.Length > 0);
                        dynamic data = JsonConvert.DeserializeObject(jsonData);
                        Assert.Equal("http://foo/", (string)data.headers.Referer);
                    }
                }
            }
        }

        [Fact]
        public void CanSetCache()
        {
            var script = @"
import fetch from 'barista-fetch';
var fn = (async () => {
    var response = await fetch('https://httpbin.org/post', { method: 'POST', cache: 'only-if-cached', body: JSON.stringify({ foo: 'bar' }) });
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
                        var promise = ctx.EvaluateModule<JsObject>(script);
                        var response = ctx.Promise.Wait<JsString>(promise);
                        Assert.NotNull(response);
                    }
                }
            }
        }

        [Fact]
        public void CanSetCredentials()
        {
            var script = @"
import fetch from 'barista-fetch';
var fn = (async () => {
    var response = await fetch('https://httpbin.org/post', { method: 'POST', credentials: 'include', body: JSON.stringify({ foo: 'bar' }) });
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
                        var promise = ctx.EvaluateModule<JsObject>(script);
                        var response = ctx.Promise.Wait<JsString>(promise);
                        Assert.NotNull(response);
                    }
                }
            }
        }

        [Fact]
        public void CanSetCookies()
        {
            var script = @"
import fetch from 'barista-fetch';
var fn = (async () => {
    var response = await fetch('https://httpbin.org/post', { method: 'POST', cookies: { foo: 'bar' }, body: JSON.stringify({ foo: 'bar' }) });
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
                        var promise = ctx.EvaluateModule<JsObject>(script);
                        var response = ctx.Promise.Wait<JsString>(promise);
                        Assert.NotNull(response);
                        var jsonData = response.ToString();
                        Assert.True(jsonData.Length > 0);
                        dynamic data = JsonConvert.DeserializeObject(jsonData);
                        Assert.Equal("foo=bar", (string)data.headers.Cookie);
                    }
                }
            }
        }

        [Fact]
        public void CanSetTimeout()
        {
            var script = @"
import fetch from 'barista-fetch';
var fn = (async () => {
    var response = await fetch('https://httpbin.org/post', { method: 'POST', timeout: 30000, body: JSON.stringify({ foo: 'bar' }) });
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
                        var promise = ctx.EvaluateModule<JsObject>(script);
                        var response = ctx.Promise.Wait<JsString>(promise);
                        Assert.NotNull(response);
                    }
                }
            }
        }

        [Fact]
        public void CanSetUserAgent()
        {
            var script = @"
import fetch from 'barista-fetch';
var fn = (async () => {
    var response = await fetch('https://httpbin.org/post', { method: 'POST', 'user-agent': 'curl/7.9.8 (i686-pc-linux-gnu) libcurl 7.9.8 (OpenSSL 0.9.6b) (ipv6 enabled)', body: JSON.stringify({ foo: 'bar' }) });
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
                        var promise = ctx.EvaluateModule<JsString>(script);
                        var response = ctx.Promise.Wait<JsString>(promise);
                        Assert.NotNull(response);
                        var jsonData = response.ToString();
                        Assert.True(jsonData.Length > 0);
                        dynamic data = JsonConvert.DeserializeObject(jsonData);
                        Assert.Equal("curl/7.9.8 (i686-pc-linux-gnu) libcurl 7.9.8 (OpenSSL 0.9.6b) (ipv6 enabled)", (string)data.headers["User-Agent"]);
                    }
                }
            }
        }

        [Fact]
        public void CanCreateAndCloneRequest()
        {
            var script = @"
        import fetch from 'barista-fetch';
        var myRequest = new fetch.Request('https://httpbin.org/get', null);
        var clonedRequest = myRequest.clone();
        var newNewRequest = new fetch.Request(clonedRequest);
        export default fetch(clonedRequest);
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
        public void CanGetResponseProperties()
        {
            var script = @"
import fetch from 'barista-fetch';
var fn = (async () => {
    var response = await fetch('https://httpbin.org/status/418');
    return response;
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
                        var promise = ctx.EvaluateModule<JsObject>(script);
                        var response = ctx.Promise.Wait<JsObject>(promise);
                        Assert.NotNull(response);
                        Assert.False(response["ok"].ToBoolean());
                        Assert.Equal(418, response["statusCode"].ToInt32());
                        Assert.Equal("I'M A TEAPOT", response["statusText"].ToString());
                        Assert.False(response["bodyUsed"].ToBoolean());
                        ctx.Promise.Wait((response["text"] as JsFunction).Call(response) as JsObject);
                        Assert.True(response["bodyUsed"].ToBoolean());
                    }
                }
            }
        }

        [Fact]
        public void CanSetRedirect()
        {
            var script = @"
import fetch from 'barista-fetch';
var fn = (async () => {
    var response = await fetch('https://httpbin.org/redirect/6', { redirect: 'error' } );
    return response;
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
                        var promise = ctx.EvaluateModule<JsObject>(script);
                        var response = ctx.Promise.Wait<JsObject>(promise);
                        Assert.NotNull(response);
                        Assert.False(response["ok"].ToBoolean());
                        Assert.Equal(302, response["statusCode"].ToInt32());
                        Assert.Equal("FOUND", response["statusText"].ToString());
                    }
                }
            }
        }

        [Fact]
        public void CanCloneResponse()
        {
            var script = @"
import fetch from 'barista-fetch';
var fn = (async () => {
    var response = await fetch('https://httpbin.org/status/418');
    var newResponse = response.clone();
    await response.text();
    return newResponse;
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
                        var promise = ctx.EvaluateModule<JsObject>(script);
                        var response = ctx.Promise.Wait<JsObject>(promise);
                        Assert.NotNull(response);
                        Assert.False(response["ok"].ToBoolean());
                        Assert.Equal(418, response["statusCode"].ToInt32());
                        Assert.False(response["bodyUsed"].ToBoolean());
                    }
                }
            }
        }

        [Fact]
        public void CanGetResponseHeaders()
        {
            var script = @"
import fetch from 'barista-fetch';
var fn = (async () => {
    var response = await fetch('https://httpbin.org/response-headers?X-Foo=bar');
    return response.headers.get('X-Foo');
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
                        var promise = ctx.EvaluateModule<JsObject>(script);
                        var response = ctx.Promise.Wait<JsObject>(promise);
                        Assert.NotNull(response);
                        Assert.Equal("bar", response.ToString());
                    }
                }
            }
        }

        [Fact]
        public void CanManipulateResponseHeaders()
        {
            var script = @"
import fetch from 'barista-fetch';
var fn = (async () => {
    var response = await fetch('https://httpbin.org/response-headers?X-Foo=bar');
    return response.headers;
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
                        var promise = ctx.EvaluateModule<JsObject>(script);
                        dynamic headers = ctx.Promise.Wait<JsObject>(promise);
                        Assert.NotNull(headers);
                        Assert.Equal("bar", (string)headers.get("X-Foo"));
                        headers.set("X-Bag", "bagman");
                        Assert.True((bool)headers.has("X-Bag"));
                        headers.append("X-Bag", "robbin");

                        //Yield statement -- this contains an auto-generated iterator.
                        var entriesIterator = headers.entries() as JsObject;
                        Assert.NotNull(entriesIterator);

                        //KeyCollection -- this is an object with an iterator property.
                        var keysIterator = headers.keys("X-Bag") as JsObject;
                        Assert.NotNull(keysIterator);

                        //yield statement -- this contains an auto-generatoed iterator.
                        var valuesIterator = headers.values("X-Bag") as JsObject;
                        Assert.NotNull(valuesIterator);

                        headers.delete("X-Bag");
                        Assert.False((bool)headers.has("X-Bag"));
                    }
                }
            }
        }

        [Fact]
        public void CanIterateHeaders()
        {
            var script = @"
import fetch from 'barista-fetch';
var fn = (async () => {
    var response = await fetch('https://httpbin.org/response-headers?X-Foo=bar&X-Foo=baz');
    var result = {};

    result.entries = [];
    for(var entry of response.headers.entries())
    {
        result.entries.push(entry);
    }

    result.keys = [];
    for(var entry of response.headers.keys('X-Foo'))
    {
        result.keys.push(entry);
    }

    result.values = [];
    for(var entry of response.headers.values('X-Foo'))
    {
        result.values.push(entry);
    }
    return result;
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
                        var promise = ctx.EvaluateModule<JsObject>(script);
                        var headersResult = ctx.Promise.Wait<JsObject>(promise);
                        Assert.NotNull(headersResult);

                        Assert.Equal(9, headersResult.GetProperty<JsObject>("entries")["length"].ToInt32());
                        Assert.Equal(9, headersResult.GetProperty<JsObject>("keys")["length"].ToInt32());
                        Assert.Equal(9, headersResult.GetProperty<JsObject>("values")["length"].ToInt32());
                    }
                }
            }
        }
    }
}
