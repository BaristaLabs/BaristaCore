namespace BaristaLabs.BaristaCore.JavaScript
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Xunit;

    public class JavaScriptObject_Dynamic_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider Provider;

        public JavaScriptObject_Dynamic_Facts()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddBaristaCore();

            Provider = ServiceCollection.BuildServiceProvider();
        }

        [Fact]
        public void JavaScriptValueConvertsToAnIntDirectly()
        {
            using (var rt = JavaScriptRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "41+1";
                        var fn = ctx.ParseScriptText(script);

                        //Assert that the function is the script we passed in.
                        var fnText = fn.ToString();
                        Assert.Equal(script, fnText);

                        //Invoke it and get a dynamic result.
                        dynamic result = fn.Invoke();
                        Assert.True((int)result == 42);
                    }
                }
            }
        }

        [Fact]
        public void JavaScriptValueConvertsToAnIntWithCoersion()
        {
            using (var rt = JavaScriptRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "'42'";
                        var fn = ctx.ParseScriptText(script);

                        //Assert that the function is the script we passed in.
                        var fnText = fn.ToString();
                        Assert.Equal(script, fnText);

                        //Invoke it and get a dynamic result.
                        dynamic result = fn.Invoke();
                        Assert.True((int)result == 42);
                    }
                }
            }
        }

        [Fact]
        public void JavaScriptValueConvertsToADoubleDirectly()
        {
            using (var rt = JavaScriptRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "41.0+1.1";
                        var fn = ctx.ParseScriptText(script);

                        //Assert that the function is the script we passed in.
                        var fnText = fn.ToString();
                        Assert.Equal(script, fnText);

                        //Invoke it and get a dynamic result.
                        dynamic result = fn.Invoke();
                        Assert.True((double)result == 42.1);
                    }
                }
            }
        }

        [Fact]
        public void JavaScriptValueConvertsToADoubleWithCoersion()
        {
            using (var rt = JavaScriptRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "'42.1'";
                        var fn = ctx.ParseScriptText(script);

                        //Assert that the function is the script we passed in.
                        var fnText = fn.ToString();
                        Assert.Equal(script, fnText);

                        //Invoke it and get a dynamic result.
                        dynamic result = fn.Invoke();
                        Assert.True((double)result == 42.1);
                    }
                }
            }
        }
    }
}
