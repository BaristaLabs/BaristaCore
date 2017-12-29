namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    [Collection("BaristaCore Tests")]
    public class JsFunction_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public JsFunction_Facts()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddBaristaCore();

            m_provider = ServiceCollection.BuildServiceProvider();
        }

        public IBaristaRuntimeFactory BaristaRuntimeFactory
        {
            get { return m_provider.GetRequiredService<IBaristaRuntimeFactory>(); }
        }

        [Fact]
        public void JsFunctionCanBeExported()
        {
            var script = @"
export default () => 'hello, world';
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var result = ctx.EvaluateModule<JsFunction>(script);
                        Assert.NotNull(result);
                        Assert.Equal(JsValueType.Function, result.Type);
                        Assert.True(result.ToString() == "() => 'hello, world'");
                    }
                }
            }
        }

        [Fact]
        public void JsFunctionsCanBeInvoked()
        {
            var script = @"
export default () => 'hello, world';
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var fn = ctx.EvaluateModule<JsFunction>(script);
                        var result = fn.Call();
                        Assert.Equal("hello, world", result.ToString());
                    }
                }
            }
        }

        [Fact]
        public void JsFunctionsThatThrowCanBeCaught()
        {
            var script = @"
export default () => { throw new Error('That is quite illogical, captain.'); };
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var fn = ctx.EvaluateModule<JsFunction>(script);

                        JsScriptException ex = null;
                        try
                        {
                            var result = fn.Call();
                        }
                        catch(JsScriptException invokeException)
                        {
                            ex = invokeException;
                        }

                        Assert.NotNull(ex);
                        Assert.Equal("That is quite illogical, captain.", ex.Message);
                    }
                }
            }
        }

        [Fact]
        public void JsNativeFunctionsCanBeCreated()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var fnAdd = ctx.ValueFactory.CreateFunction(new Func<JsObject, int, int, int>((jsThis, a, b) =>
                        {
                            return a + b;
                        }));

                        Assert.NotNull(fnAdd);
                        var jsNumA = ctx.ValueFactory.CreateNumber(37);
                        var jsNumB = ctx.ValueFactory.CreateNumber(5);
                        var result = fnAdd.Call<JsNumber>(ctx.GlobalObject, jsNumA, jsNumB);

                        Assert.Equal(42, result.ToInt32());
                    }
                }
            }
        }

        [Fact]
        public void JsNullNativeFunctionsCannotBeCreated()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            ctx.ValueFactory.CreateFunction(null);
                        });
                    }
                }
            }
        }

        [Fact]
        public void JsNativeFunctionsThatThrowCanBeHandled()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var fnBoom = ctx.ValueFactory.CreateFunction(new Action(() =>
                        {
                            throw new InvalidOperationException("A hole has been torn in the universe.");
                        }));
                        Assert.NotNull(fnBoom);

                        JsScriptException ex = null;
                        try
                        {
                            var result = fnBoom.Call();
                        }
                        catch(JsScriptException invokeException)
                        {
                            ex = invokeException;
                        }

                        Assert.NotNull(ex);
                        Assert.Equal("A hole has been torn in the universe.", ex.Message);
                    }
                }
            }
        }

        [Fact]
        public void JsFunctionsReturningThisAreUndefined()
        {
            //This is an interesting scenario when attempting to use node modules in modules.
            //Apparently in node, this returns the global object, while browsers return undefined.
            //Note the lack of parenthesis.
            var script = @"
export default (function() { return this; }());
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var result = ctx.EvaluateModule(script);
                        Assert.NotNull(result);
                        Assert.Equal(ctx.Undefined, result);

                        //If we're just running the script, the global is returned.
                        var runResultHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(rt.Engine, "(function() { return this; }())");
                        var runResult = ctx.ValueFactory.CreateValue(runResultHandle);
                        Assert.Same(ctx.GlobalObject, runResult);

                        //Thus, in order to wrap this, it must be
                        result = ctx.EvaluateModule("export default (function() { return this; }).call(Function('return this')());");
                        Assert.Same(ctx.GlobalObject, result);

                        result = ctx.EvaluateModule("export default (function() { return (function (root) { return root; })(this); }).call(Function('return this')());");
                        Assert.Same(ctx.GlobalObject, result);
                    }
                }
            }
        }

        [Fact]
        public void JsFunctionsEvalingThisReturnGlobal()
        {
            //Corollary to the above, if it's effectively an eval, the global is returned.
            var script = @"
export default (Function('return this')());
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var result = ctx.EvaluateModule(script);
                        Assert.NotNull(result);
                        Assert.Same(ctx.GlobalObject, result);
                    }
                }
            }
        }

        [Fact]
        public void JsPrototypesCanBeCreated()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var User = ctx.ValueFactory.CreateFunction(new Action<JsObject>((thisObj) => {
                            thisObj["foo"] = ctx.ValueFactory.CreateString("bar");
                        }));

                        var fnTestMe = ctx.ValueFactory.CreateFunction(new Func<JsValue>(() => {
                            return ctx.ValueFactory.CreateString("moose");
                        }));

                        var propsDeclObj = ctx.ValueFactory.CreateObject();
                        var testMeFnDecl = ctx.ValueFactory.CreateObject();
                        testMeFnDecl["value"] = fnTestMe;
                        testMeFnDecl["enumerable"] = ctx.True;
                        propsDeclObj["testMe"] = testMeFnDecl;

                        //Create a new user.
                        var newUser = ctx.Object.Create(User.Prototype, propsDeclObj);
                        
                        //Check that everything is all-ok.
                        Assert.NotNull(newUser);
                        User.Call(newUser);
                        var isInstance = ctx.InstanceOf(newUser, User);
                        Assert.Equal(2, newUser.Keys.Length);
                        Assert.Equal("testMe", newUser.Keys[0].ToString());
                        Assert.Equal("foo", newUser.Keys[1].ToString());

                        Assert.True(newUser["testMe"] is JsFunction);
                        Assert.True(newUser["foo"] is JsValue);
                        Assert.Equal("bar", newUser["foo"].ToString());

                        Assert.Equal("moose", (newUser["testMe"] as JsFunction).Call(newUser).ToString());

                        var newUserPrototype = ctx.Object.GetPrototypeOf(newUser);
                        Assert.Same(newUserPrototype, User.Prototype);

                        //Now create a constructor for users.
                        var fnUserCtor = ctx.ValueFactory.CreateFunction(new Func<JsObject, JsObject>((thisObj) => {
                            var daUser = ctx.Object.Create(User.Prototype, propsDeclObj);
                            User.Call(daUser);
                            return daUser;
                        }));

                        ctx.GlobalObject["User"] = fnUserCtor;
                        var result = ctx.EvaluateModule<JsObject>("let user = new User(); export default user;");
                        Assert.True(ctx.InstanceOf(result, User));
                        Assert.Equal(2, result.Keys.Length);
                        Assert.Equal("testMe", result.Keys[0].ToString());
                        Assert.Equal("foo", result.Keys[1].ToString());
                        var resultJson = ctx.JSON.Stringify(result);
                        Assert.False(String.IsNullOrWhiteSpace(resultJson.ToString()));
                    }
                }
            }
        }
    }
}