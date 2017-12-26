namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class JavaScriptReference_Facts
    {
        private IJavaScriptEngine Engine;

        public JavaScriptReference_Facts()
        {
            var chakraCoreFactory = new ChakraCoreFactory();
            Engine = chakraCoreFactory.CreateJavaScriptEngine();
        }

        [Fact]
        public void JsReferencesIndicateNativeFunctionSource()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                Assert.Equal("JsCreateRuntime", runtimeHandle.NativeFunctionSource);
            }
        }

        [Fact]
        public void JsValueSafeHandlesAreNotSingletons()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var trueHandle = Engine.JsGetTrueValue();
                    var anotherTrueHandle = Engine.JsBoolToBoolean(true);

                    Assert.Equal(trueHandle, anotherTrueHandle);
                    Assert.NotSame(trueHandle, anotherTrueHandle);
                }
            }
        }
    }
}
