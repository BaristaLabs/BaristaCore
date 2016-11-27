namespace BaristaLabs.BaristaCore.JavaScript
{
    using Internal;

    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using Xunit;

    public class ChakraApi_ChakraCore_Facts
    {
        private IJavaScriptEngine Jsrt;

        public ChakraApi_ChakraCore_Facts()
        {
            Jsrt = JavaScriptEngineFactory.CreateChakraEngine();
        }

        [Fact]
        public void JsCreateStringTest()
        {
            var str = "Hello, World!";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var stringHandle = Jsrt.JsCreateString(str, new UIntPtr((uint)str.Length));

                    Assert.True(stringHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(stringHandle);
                    Assert.True(handleType == JavaScriptValueType.String);

                    stringHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCreateStringUtf8Test()
        {
            var str = "Hello, World!";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var stringHandle = Jsrt.JsCreateStringUtf8(str, new UIntPtr((uint)str.Length));

                    Assert.True(stringHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(stringHandle);
                    Assert.True(handleType == JavaScriptValueType.String);

                    stringHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCreateStringUtf16Test()
        {
            var str = "Hello, World!";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var stringHandle = Jsrt.JsCreateStringUtf16(str, new UIntPtr((uint)str.Length));

                    Assert.True(stringHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(stringHandle);
                    Assert.True(handleType == JavaScriptValueType.String);

                    stringHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCopyStringTest()
        {
            var str = "Hello, World!";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var stringHandle = Jsrt.JsCreateString(str, new UIntPtr((uint)str.Length));

                    //Get the size
                    var size = Jsrt.JsCopyString(stringHandle, 0, -1, null);
                    if ((int)size > int.MaxValue)
                        throw new OutOfMemoryException("Exceeded maximum string length.");

                    byte[] result = new byte[(int)size];
                    var written = Jsrt.JsCopyString(stringHandle, 0, -1, result);
                    string resultStr = Encoding.ASCII.GetString(result, 0, result.Length);

                    Assert.True(str == resultStr);

                    stringHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCopyStringUtf8Test()
        {
            var str = "Hello, World!";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var stringHandle = Jsrt.JsCreateStringUtf8(str, new UIntPtr((uint)str.Length));

                    //Get the size
                    var size = Jsrt.JsCopyStringUtf8(stringHandle, null, UIntPtr.Zero);
                    if ((int)size > int.MaxValue)
                        throw new OutOfMemoryException("Exceeded maximum string length.");

                    byte[] result = new byte[(int)size];
                    var written = Jsrt.JsCopyStringUtf8(stringHandle, result, new UIntPtr((uint)result.Length));
                    string resultStr = Encoding.UTF8.GetString(result, 0, result.Length);

                    Assert.True(str == resultStr);

                    stringHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCopyStringUtf16Test()
        {
            var str = "Hello, World!";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var stringHandle = Jsrt.JsCreateStringUtf16(str, new UIntPtr((uint)str.Length));

                    //Get the size
                    var size = Jsrt.JsCopyStringUtf16(stringHandle, 0, -1, null);
                    if ((int)size * 2 > int.MaxValue)
                        throw new OutOfMemoryException("Exceeded maximum string length.");

                    byte[] result = new byte[(int)size * 2];
                    var written = Jsrt.JsCopyStringUtf16(stringHandle, 0, -1, result);
                    string resultStr = Encoding.Unicode.GetString(result, 0, result.Length);

                    Assert.True(str == resultStr);

                    stringHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsParseDoesParse()
        {
            var script = "(()=>{return 6*7;})()";
            string sourceUrl = "[eval code]";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
                    try
                    {
                        var scriptHandle = Jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero);
                        var sourceContext = JavaScriptSourceContext.None;
                        var sourceUrlHandle = Jsrt.JsCreateStringUtf8(sourceUrl, new UIntPtr((uint)sourceUrl.Length));
                        var resultHandle = Jsrt.JsParse(scriptHandle, sourceContext, sourceUrlHandle, JavaScriptParseScriptAttributes.None);

                        Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

                        var handleType = Jsrt.JsGetValueType(resultHandle);
                        Assert.True(handleType == JavaScriptValueType.Function);

                        resultHandle.Dispose();
                        sourceUrlHandle.Dispose();
                        scriptHandle.Dispose();
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptrScript);
                    }
                }
            }
        }

        [Fact]
        public void JsRunDoesRun()
        {
            var script = "(()=>{return 6*7;})()";
            string sourceUrl = "[eval code]";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
                    try
                    {
                        var scriptHandle = Jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero);

                        JavaScriptSourceContext sourceContext = JavaScriptSourceContext.None;

                        var sourceUrlHandle = Jsrt.JsCreateStringUtf8(sourceUrl, new UIntPtr((uint)sourceUrl.Length));

                        var resultHandle = Jsrt.JsRun(scriptHandle, sourceContext, sourceUrlHandle, JavaScriptParseScriptAttributes.None);
                        Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

                        var handleType = Jsrt.JsGetValueType(resultHandle);
                        Assert.True(handleType == JavaScriptValueType.Number);

                        var resultValue = Jsrt.JsNumberToInt(resultHandle);
                        Assert.True(resultValue == 42);

                        resultHandle.Dispose();
                        sourceUrlHandle.Dispose();
                        scriptHandle.Dispose();
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptrScript);
                    }
                }
            }
        }

        [Fact]
        public void JsPropertyIdCanBeCreated()
        {
            var str = "foo";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var propertyHandle = Jsrt.JsCreatePropertyIdUtf8(str, new UIntPtr((uint)str.Length));

                    Assert.True(propertyHandle != JavaScriptPropertyIdSafeHandle.Invalid);

                    propertyHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsPropertyIdCanBeCopied()
        {
            var str = "foo";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var propertyHandle = Jsrt.JsCreatePropertyIdUtf8(str, new UIntPtr((uint)str.Length));

                    //Get the size
                    var size = Jsrt.JsCopyPropertyIdUtf8(propertyHandle, null, UIntPtr.Zero);
                    if ((int)size > int.MaxValue)
                        throw new OutOfMemoryException("Exceeded maximum string length.");

                    byte[] result = new byte[(int)size];
                    var written = Jsrt.JsCopyPropertyIdUtf8(propertyHandle, result, new UIntPtr((uint)result.Length));
                    string resultStr = Encoding.UTF8.GetString(result, 0, result.Length);

                    Assert.True(str == resultStr);

                    propertyHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsScriptCanBeSerialized()
        {
            var script = "(()=>{return 6*7;})()";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    byte[] buffer = new byte[1024];
                    ulong bufferSize = (ulong)buffer.Length;
                    IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
                    try
                    {
                        var scriptHandle = Jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero);

                        Jsrt.JsSerialize(scriptHandle, buffer, ref bufferSize, JavaScriptParseScriptAttributes.None);

                        Assert.True(bufferSize != (ulong)buffer.Length);
                        Assert.True(buffer[0] != 0);

                        scriptHandle.Dispose();
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptrScript);
                    }
                }
            }
        }

        [Fact]
        public void JsSerializedScriptCanBeParsed()
        {
            var script = "(()=>{return 6*7;})()";
            string sourceUrl = "[eval code]";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    byte[] buffer = new byte[1024];
                    ulong bufferSize = (ulong)buffer.Length;

                    IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
                    try
                    {
                        var scriptHandle = Jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero);

                        Jsrt.JsSerialize(scriptHandle, buffer, ref bufferSize, JavaScriptParseScriptAttributes.None);
                        scriptHandle.Dispose();
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptrScript);
                    }

                    JavaScriptSerializedLoadScriptCallback callback = (JavaScriptSourceContext sourceContext, out JavaScriptValueSafeHandle value, out JavaScriptParseScriptAttributes parseAttributes) =>
                    {
                        value = null;
                        parseAttributes = JavaScriptParseScriptAttributes.None;
                        return true;
                    };

                    var sourceUrlHandle = Jsrt.JsCreateStringUtf8(sourceUrl, new UIntPtr((uint)sourceUrl.Length));
                    var resultHandle = Jsrt.JsParseSerialized(buffer, callback, JavaScriptSourceContext.None, sourceUrlHandle);

                    Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(resultHandle);
                    Assert.True(handleType == JavaScriptValueType.Function);

                    resultHandle.Dispose();
                    sourceUrlHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsSerializedScriptCanBeRun()
        {
            var script = "(()=>{return 6*7;})()";
            string sourceUrl = "[eval code]";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    byte[] buffer = new byte[1024];
                    ulong bufferSize = (ulong)buffer.Length;

                    IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
                    try
                    {
                        var scriptHandle = Jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero);

                        Jsrt.JsSerialize(scriptHandle, buffer, ref bufferSize, JavaScriptParseScriptAttributes.None);
                        scriptHandle.Dispose();
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptrScript);
                    }

                    JavaScriptSerializedLoadScriptCallback callback = (JavaScriptSourceContext sourceContext, out JavaScriptValueSafeHandle value, out JavaScriptParseScriptAttributes parseAttributes) =>
                    {
                        value = null;
                        parseAttributes = JavaScriptParseScriptAttributes.None;
                        return true;
                    };

                    var sourceUrlHandle = Jsrt.JsCreateStringUtf8(sourceUrl, new UIntPtr((uint)sourceUrl.Length));
                    var resultHandle = Jsrt.JsRunSerialized(buffer, callback, JavaScriptSourceContext.None, sourceUrlHandle);

                    Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(resultHandle);
                    Assert.True(handleType == JavaScriptValueType.Number);

                    var resultValue = Jsrt.JsNumberToInt(resultHandle);

                    Assert.True(resultValue == 42);

                    sourceUrlHandle.Dispose();
                    resultHandle.Dispose();

                }
            }
        }
    }
}
