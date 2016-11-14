namespace BaristaLabs.BaristaCore.JavaScript
{
    using Interop;
    using Interop.Callbacks;
    using Interop.SafeHandles;
    using Extensions;
    using System;
    using System.Text;
    using Xunit;
    using System.Runtime.InteropServices;

    public class ChakraApi_ChakraCore_Facts
    {
        [Fact]
        public void JsCreateStringTest()
        {
            var str = "Hello, World!";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle stringHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateString(str, new UIntPtr((uint)str.Length), out stringHandle));
            
            Assert.True(stringHandle != JavaScriptValueSafeHandle.Invalid);

            stringHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCreateStringUtf8Test()
        {
            var str = "Hello, World!";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle stringHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8(str, new UIntPtr((uint)str.Length), out stringHandle));

            Assert.True(stringHandle != JavaScriptValueSafeHandle.Invalid);

            stringHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCreateStringUtf16Test()
        {
            var str = "Hello, World!";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle stringHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf16(str, new UIntPtr((uint)str.Length), out stringHandle));

            Assert.True(stringHandle != JavaScriptValueSafeHandle.Invalid);

            stringHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCopyStringTest()
        {
            var str = "Hello, World!";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle stringHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateString(str, new UIntPtr((uint)str.Length), out stringHandle));

            //Get the size
            UIntPtr size;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCopyString(stringHandle, 0, -1, null, out size));
            if ((int)size > int.MaxValue)
                throw new OutOfMemoryException("Exceeded maximum string length.");

            byte[] result = new byte[(int)size];
            UIntPtr written;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCopyString(stringHandle, 0, -1, result, out written));
            string resultStr = Encoding.ASCII.GetString(result, 0, result.Length);

            Assert.True(str == resultStr);

            stringHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCopyStringUtf8Test()
        {
            var str = "Hello, World!";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle stringHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8(str, new UIntPtr((uint)str.Length), out stringHandle));

            //Get the size
            UIntPtr size;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCopyStringUtf8(stringHandle, null, UIntPtr.Zero, out size));
            if ((int)size > int.MaxValue)
                throw new OutOfMemoryException("Exceeded maximum string length.");

            byte[] result = new byte[(int)size];
            UIntPtr written;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCopyStringUtf8(stringHandle, result, new UIntPtr((uint)result.Length), out written));
            string resultStr = Encoding.UTF8.GetString(result, 0, result.Length);

            Assert.True(str == resultStr);

            stringHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCopyStringUtf16Test()
        {
            var str = "Hello, World!";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle stringHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf16(str, new UIntPtr((uint)str.Length), out stringHandle));

            //Get the size
            UIntPtr size;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCopyStringUtf16(stringHandle, 0, -1, null, out size));
            if ((int)size * 2 > int.MaxValue)
                throw new OutOfMemoryException("Exceeded maximum string length.");

            byte[] result = new byte[(int)size * 2];
            UIntPtr written;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCopyStringUtf16(stringHandle, 0, -1, result, out written));
            string resultStr = Encoding.Unicode.GetString(result, 0, result.Length);

            Assert.True(str == resultStr);

            stringHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsParseDoesParse()
        {
            var script = "(()=>{return 6*7;})()";
            string sourceUrl = "[eval code]";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
            try
            {
                JavaScriptValueSafeHandle scriptHandle;
                Errors.ThrowIfIs(ChakraApi.Instance.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

                JavaScriptSourceContext sourceContext = new JavaScriptSourceContext();

                JavaScriptValueSafeHandle sourceUrlHandle;
                Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8(sourceUrl, new UIntPtr((uint)sourceUrl.Length), out sourceUrlHandle));

                JavaScriptValueSafeHandle resultHandle;
                Errors.ThrowIfIs(ChakraApi.Instance.JsParse(scriptHandle, sourceContext, sourceUrlHandle, JsParseScriptAttributes.JsParseScriptAttributeNone, out resultHandle));

                Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

                JsValueType valueType;
                Errors.ThrowIfIs(ChakraApi.Instance.JsGetValueType(resultHandle, out valueType));

                Assert.True(valueType == JsValueType.JsFunction);

                resultHandle.Dispose();
                sourceUrlHandle.Dispose();
                scriptHandle.Dispose();
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocAnsi(ptrScript);

                contextHandle.Dispose();
                runtimeHandle.Dispose();
            }
        }

        [Fact]
        public void JsRunDoesRun()
        {
            var script = "(()=>{return 6*7;})()";
            string sourceUrl = "[eval code]";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
            try
            {
                JavaScriptValueSafeHandle scriptHandle;
                Errors.ThrowIfIs(ChakraApi.Instance.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

                JavaScriptSourceContext sourceContext = new JavaScriptSourceContext();

                JavaScriptValueSafeHandle sourceUrlHandle;
                Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8(sourceUrl, new UIntPtr((uint)sourceUrl.Length), out sourceUrlHandle));

                JavaScriptValueSafeHandle resultHandle;
                Errors.ThrowIfIs(ChakraApi.Instance.JsRun(scriptHandle, sourceContext, sourceUrlHandle, JsParseScriptAttributes.JsParseScriptAttributeNone, out resultHandle));

                Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

                JsValueType valueType;
                Errors.ThrowIfIs(ChakraApi.Instance.JsGetValueType(resultHandle, out valueType));

                Assert.True(valueType == JsValueType.JsNumber);

                int resultValue;
                Errors.ThrowIfIs(ChakraApi.Instance.JsNumberToInt(resultHandle, out resultValue));

                Assert.True(resultValue == 42);

                resultHandle.Dispose();
                sourceUrlHandle.Dispose();
                scriptHandle.Dispose();
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocAnsi(ptrScript);

                contextHandle.Dispose();
                runtimeHandle.Dispose();
            }
        }

        [Fact]
        public void JsPropertyIdCanBeCreated()
        {
            var str = "foo";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            JavaScriptPropertyIdSafeHandle propertyHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreatePropertyIdUtf8(str, new UIntPtr((uint)str.Length), out propertyHandle));

            Assert.True(propertyHandle != JavaScriptPropertyIdSafeHandle.Invalid);

            propertyHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsPropertyIdCanBeCopied()
        {
            var str = "foo";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            JavaScriptPropertyIdSafeHandle propertyHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreatePropertyIdUtf8(str, new UIntPtr((uint)str.Length), out propertyHandle));

            //Get the size
            UIntPtr size;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCopyPropertyIdUtf8(propertyHandle, null, UIntPtr.Zero, out size));
            if ((int)size > int.MaxValue)
                throw new OutOfMemoryException("Exceeded maximum string length.");

            byte[] result = new byte[(int)size];
            UIntPtr written;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCopyPropertyIdUtf8(propertyHandle, result, new UIntPtr((uint)result.Length), out written));
            string resultStr = Encoding.UTF8.GetString(result, 0, result.Length);

            Assert.True(str == resultStr);

            propertyHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsScriptCanBeSerialized()
        {
            var script = "(()=>{return 6*7;})()";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            byte[] buffer = new byte[1024];
            ulong bufferSize = (ulong)buffer.Length;
            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
            try
            {
                JavaScriptValueSafeHandle scriptHandle;
                Errors.ThrowIfIs(ChakraApi.Instance.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

                Errors.ThrowIfIs(ChakraApi.Instance.JsSerialize(scriptHandle, buffer, ref bufferSize, JsParseScriptAttributes.JsParseScriptAttributeNone));

                Assert.True(bufferSize != (ulong)buffer.Length);
                Assert.True(buffer[0] != 0);

                scriptHandle.Dispose();
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocAnsi(ptrScript);

                contextHandle.Dispose();
                runtimeHandle.Dispose();
            }
        }

        [Fact]
        public void JsSerializedScriptCanBeParsed()
        {
            var script = "(()=>{return 6*7;})()";
            string sourceUrl = "[eval code]";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            byte[] buffer = new byte[1024];
            ulong bufferSize = (ulong)buffer.Length;

            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
            try
            {
                JavaScriptValueSafeHandle scriptHandle;
                Errors.ThrowIfIs(ChakraApi.Instance.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

                Errors.ThrowIfIs(ChakraApi.Instance.JsSerialize(scriptHandle, buffer, ref bufferSize, JsParseScriptAttributes.JsParseScriptAttributeNone));
                scriptHandle.Dispose();
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocAnsi(ptrScript);
            }

            JavaScriptSerializedLoadScriptCallback callback = (JavaScriptSourceContext sourceContext, out JavaScriptValueSafeHandle value, out JsParseScriptAttributes parseAttributes) =>
            {
                value = null;
                parseAttributes = JsParseScriptAttributes.JsParseScriptAttributeNone;
                return true;
            };

            JavaScriptValueSafeHandle sourceUrlHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8(sourceUrl, new UIntPtr((uint)sourceUrl.Length), out sourceUrlHandle));

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsParseSerialized(buffer, callback, JavaScriptSourceContext.None, sourceUrlHandle, out resultHandle));

            Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

            JsValueType valueType;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetValueType(resultHandle, out valueType));

            Assert.True(valueType == JsValueType.JsFunction);

            sourceUrlHandle.Dispose();
            resultHandle.Dispose();

            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsSerializedScriptCanBeRun()
        {
            var script = "(()=>{return 6*7;})()";
            string sourceUrl = "[eval code]";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            byte[] buffer = new byte[1024];
            ulong bufferSize = (ulong)buffer.Length;

            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
            try
            {
                JavaScriptValueSafeHandle scriptHandle;
                Errors.ThrowIfIs(ChakraApi.Instance.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

                Errors.ThrowIfIs(ChakraApi.Instance.JsSerialize(scriptHandle, buffer, ref bufferSize, JsParseScriptAttributes.JsParseScriptAttributeNone));
                scriptHandle.Dispose();
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocAnsi(ptrScript);
            }

            JavaScriptSerializedLoadScriptCallback callback = (JavaScriptSourceContext sourceContext, out JavaScriptValueSafeHandle value, out JsParseScriptAttributes parseAttributes) =>
            {
                value = null;
                parseAttributes = JsParseScriptAttributes.JsParseScriptAttributeNone;
                return true;
            };

            JavaScriptValueSafeHandle sourceUrlHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8(sourceUrl, new UIntPtr((uint)sourceUrl.Length), out sourceUrlHandle));

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsRunSerialized(buffer, callback, JavaScriptSourceContext.None, sourceUrlHandle, out resultHandle));

            Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

            JsValueType valueType;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetValueType(resultHandle, out valueType));

            Assert.True(valueType == JsValueType.JsNumber);

            int resultValue;
            Errors.ThrowIfIs(ChakraApi.Instance.JsNumberToInt(resultHandle, out resultValue));

            Assert.True(resultValue == 42);

            sourceUrlHandle.Dispose();
            resultHandle.Dispose();

            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }
    }
}
