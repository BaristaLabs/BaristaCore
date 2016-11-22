namespace BaristaLabs.BaristaCore.JavaScript
{
    using Internal;

    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using Xunit;

    public class ChakraApi_ChakraCore_Facts
    {
        private IJavaScriptRuntime Jsrt;

        public ChakraApi_ChakraCore_Facts()
        {
            Jsrt = JavaScriptRuntimeFactory.CreateChakraRuntime();
        }

        [Fact]
        public void JsCreateStringTest()
        {
            var str = "Hello, World!";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle stringHandle;
            Errors.ThrowIfError(Jsrt.JsCreateString(str, new UIntPtr((uint)str.Length), out stringHandle));
            
            Assert.True(stringHandle != JavaScriptValueSafeHandle.Invalid);

            JavaScriptValueType handleType;
            Errors.ThrowIfError(Jsrt.JsGetValueType(stringHandle, out handleType));

            Assert.True(handleType == JavaScriptValueType.String);

            stringHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCreateStringUtf8Test()
        {
            var str = "Hello, World!";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle stringHandle;
            Errors.ThrowIfError(Jsrt.JsCreateStringUtf8(str, new UIntPtr((uint)str.Length), out stringHandle));

            Assert.True(stringHandle != JavaScriptValueSafeHandle.Invalid);

            JavaScriptValueType handleType;
            Errors.ThrowIfError(Jsrt.JsGetValueType(stringHandle, out handleType));

            Assert.True(handleType == JavaScriptValueType.String);

            stringHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCreateStringUtf16Test()
        {
            var str = "Hello, World!";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle stringHandle;
            Errors.ThrowIfError(Jsrt.JsCreateStringUtf16(str, new UIntPtr((uint)str.Length), out stringHandle));

            Assert.True(stringHandle != JavaScriptValueSafeHandle.Invalid);

            JavaScriptValueType handleType;
            Errors.ThrowIfError(Jsrt.JsGetValueType(stringHandle, out handleType));

            Assert.True(handleType == JavaScriptValueType.String);

            stringHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCopyStringTest()
        {
            var str = "Hello, World!";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle stringHandle;
            Errors.ThrowIfError(Jsrt.JsCreateString(str, new UIntPtr((uint)str.Length), out stringHandle));

            //Get the size
            UIntPtr size;
            Errors.ThrowIfError(Jsrt.JsCopyString(stringHandle, 0, -1, null, out size));
            if ((int)size > int.MaxValue)
                throw new OutOfMemoryException("Exceeded maximum string length.");

            byte[] result = new byte[(int)size];
            UIntPtr written;
            Errors.ThrowIfError(Jsrt.JsCopyString(stringHandle, 0, -1, result, out written));
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
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle stringHandle;
            Errors.ThrowIfError(Jsrt.JsCreateStringUtf8(str, new UIntPtr((uint)str.Length), out stringHandle));

            //Get the size
            UIntPtr size;
            Errors.ThrowIfError(Jsrt.JsCopyStringUtf8(stringHandle, null, UIntPtr.Zero, out size));
            if ((int)size > int.MaxValue)
                throw new OutOfMemoryException("Exceeded maximum string length.");

            byte[] result = new byte[(int)size];
            UIntPtr written;
            Errors.ThrowIfError(Jsrt.JsCopyStringUtf8(stringHandle, result, new UIntPtr((uint)result.Length), out written));
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
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle stringHandle;
            Errors.ThrowIfError(Jsrt.JsCreateStringUtf16(str, new UIntPtr((uint)str.Length), out stringHandle));

            //Get the size
            UIntPtr size;
            Errors.ThrowIfError(Jsrt.JsCopyStringUtf16(stringHandle, 0, -1, null, out size));
            if ((int)size * 2 > int.MaxValue)
                throw new OutOfMemoryException("Exceeded maximum string length.");

            byte[] result = new byte[(int)size * 2];
            UIntPtr written;
            Errors.ThrowIfError(Jsrt.JsCopyStringUtf16(stringHandle, 0, -1, result, out written));
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
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
            try
            {
                JavaScriptValueSafeHandle scriptHandle;
                Errors.ThrowIfError(Jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

                JavaScriptSourceContext sourceContext = new JavaScriptSourceContext();

                JavaScriptValueSafeHandle sourceUrlHandle;
                Errors.ThrowIfError(Jsrt.JsCreateStringUtf8(sourceUrl, new UIntPtr((uint)sourceUrl.Length), out sourceUrlHandle));

                JavaScriptValueSafeHandle resultHandle;
                Errors.ThrowIfError(Jsrt.JsParse(scriptHandle, sourceContext, sourceUrlHandle, JavaScriptParseScriptAttributes.None, out resultHandle));

                Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

                JavaScriptValueType valueType;
                Errors.ThrowIfError(Jsrt.JsGetValueType(resultHandle, out valueType));

                Assert.True(valueType == JavaScriptValueType.Function);

                resultHandle.Dispose();
                sourceUrlHandle.Dispose();
                scriptHandle.Dispose();
            }
            finally
            {
                Marshal.FreeHGlobal(ptrScript);

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
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
            try
            {
                JavaScriptValueSafeHandle scriptHandle;
                Errors.ThrowIfError(Jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

                JavaScriptSourceContext sourceContext = new JavaScriptSourceContext();

                JavaScriptValueSafeHandle sourceUrlHandle;
                Errors.ThrowIfError(Jsrt.JsCreateStringUtf8(sourceUrl, new UIntPtr((uint)sourceUrl.Length), out sourceUrlHandle));

                JavaScriptValueSafeHandle resultHandle;
                Errors.ThrowIfError(Jsrt.JsRun(scriptHandle, sourceContext, sourceUrlHandle, JavaScriptParseScriptAttributes.None, out resultHandle));

                Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

                JavaScriptValueType valueType;
                Errors.ThrowIfError(Jsrt.JsGetValueType(resultHandle, out valueType));

                Assert.True(valueType == JavaScriptValueType.Number);

                int resultValue;
                Errors.ThrowIfError(Jsrt.JsNumberToInt(resultHandle, out resultValue));

                Assert.True(resultValue == 42);

                resultHandle.Dispose();
                sourceUrlHandle.Dispose();
                scriptHandle.Dispose();
            }
            finally
            {
                Marshal.FreeHGlobal(ptrScript);

                contextHandle.Dispose();
                runtimeHandle.Dispose();
            }
        }

        [Fact]
        public void JsPropertyIdCanBeCreated()
        {
            var str = "foo";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptPropertyIdSafeHandle propertyHandle;
            Errors.ThrowIfError(Jsrt.JsCreatePropertyIdUtf8(str, new UIntPtr((uint)str.Length), out propertyHandle));

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
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptPropertyIdSafeHandle propertyHandle;
            Errors.ThrowIfError(Jsrt.JsCreatePropertyIdUtf8(str, new UIntPtr((uint)str.Length), out propertyHandle));

            //Get the size
            UIntPtr size;
            Errors.ThrowIfError(Jsrt.JsCopyPropertyIdUtf8(propertyHandle, null, UIntPtr.Zero, out size));
            if ((int)size > int.MaxValue)
                throw new OutOfMemoryException("Exceeded maximum string length.");

            byte[] result = new byte[(int)size];
            UIntPtr written;
            Errors.ThrowIfError(Jsrt.JsCopyPropertyIdUtf8(propertyHandle, result, new UIntPtr((uint)result.Length), out written));
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
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            byte[] buffer = new byte[1024];
            ulong bufferSize = (ulong)buffer.Length;
            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
            try
            {
                JavaScriptValueSafeHandle scriptHandle;
                Errors.ThrowIfError(Jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

                Errors.ThrowIfError(Jsrt.JsSerialize(scriptHandle, buffer, ref bufferSize, JavaScriptParseScriptAttributes.None));

                Assert.True(bufferSize != (ulong)buffer.Length);
                Assert.True(buffer[0] != 0);

                scriptHandle.Dispose();
            }
            finally
            {
                Marshal.FreeHGlobal(ptrScript);

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
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            byte[] buffer = new byte[1024];
            ulong bufferSize = (ulong)buffer.Length;

            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
            try
            {
                JavaScriptValueSafeHandle scriptHandle;
                Errors.ThrowIfError(Jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

                Errors.ThrowIfError(Jsrt.JsSerialize(scriptHandle, buffer, ref bufferSize, JavaScriptParseScriptAttributes.None));
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

            JavaScriptValueSafeHandle sourceUrlHandle;
            Errors.ThrowIfError(Jsrt.JsCreateStringUtf8(sourceUrl, new UIntPtr((uint)sourceUrl.Length), out sourceUrlHandle));

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfError(Jsrt.JsParseSerialized(buffer, callback, JavaScriptSourceContext.None, sourceUrlHandle, out resultHandle));

            Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

            JavaScriptValueType valueType;
            Errors.ThrowIfError(Jsrt.JsGetValueType(resultHandle, out valueType));

            Assert.True(valueType == JavaScriptValueType.Function);

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
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            byte[] buffer = new byte[1024];
            ulong bufferSize = (ulong)buffer.Length;

            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
            try
            {
                JavaScriptValueSafeHandle scriptHandle;
                Errors.ThrowIfError(Jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

                Errors.ThrowIfError(Jsrt.JsSerialize(scriptHandle, buffer, ref bufferSize, JavaScriptParseScriptAttributes.None));
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

            JavaScriptValueSafeHandle sourceUrlHandle;
            Errors.ThrowIfError(Jsrt.JsCreateStringUtf8(sourceUrl, new UIntPtr((uint)sourceUrl.Length), out sourceUrlHandle));

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfError(Jsrt.JsRunSerialized(buffer, callback, JavaScriptSourceContext.None, sourceUrlHandle, out resultHandle));

            Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

            JavaScriptValueType valueType;
            Errors.ThrowIfError(Jsrt.JsGetValueType(resultHandle, out valueType));

            Assert.True(valueType == JavaScriptValueType.Number);

            int resultValue;
            Errors.ThrowIfError(Jsrt.JsNumberToInt(resultHandle, out resultValue));

            Assert.True(resultValue == 42);

            sourceUrlHandle.Dispose();
            resultHandle.Dispose();

            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }
    }
}
