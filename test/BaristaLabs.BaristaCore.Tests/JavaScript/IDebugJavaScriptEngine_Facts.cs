namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using Xunit;

    public class IDebugJavaScriptEngine_Facts
    {
        private IJavaScriptEngine Engine;

        public IDebugJavaScriptEngine_Facts()
        {
            Engine = JavaScriptEngineFactory.CreateChakraEngine();
        }

        [Fact]
        public void JsCanStartDebugging()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    bool called = false;
                    JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, IntPtr eventData, IntPtr callbackState) =>
                    {
                        called = true;
                        return true;
                    };

                    Engine.JsDiagStartDebugging(runtimeHandle, callback, IntPtr.Zero);

                    //We didn't specify any breakpoints so...
                    Assert.False(called);
                }
            }
        }

        [Fact]
        public void JsCanStopDebugging()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    bool called = false;
                    JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, IntPtr eventData, IntPtr callbackState) =>
                    {
                        called = true;
                        return true;
                    };

                    Engine.JsDiagStartDebugging(runtimeHandle, callback, IntPtr.Zero);

                    Engine.JsDiagStopDebugging(runtimeHandle);

                    //We didn't specify any breakpoints so...
                    Assert.False(called);
                }
            }
        }

        [Fact]
        public void JsCanRequestAsyncBreak()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    bool called = false;
                    JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, IntPtr eventData, IntPtr callbackState) =>
                    {
                        called = true;
                        return true;
                    };

                    Engine.JsDiagStartDebugging(runtimeHandle, callback, IntPtr.Zero);

                    Engine.JsDiagRequestAsyncBreak(runtimeHandle);

                    //We didn't specify any breakpoints so...
                    Assert.False(called);
                }
            }
        }

        [Fact]
        public void JsCanRetrieveBreakpoints()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    bool called = false;
                    JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, IntPtr eventData, IntPtr callbackState) =>
                    {
                        called = true;
                        return true;
                    };

                    Engine.JsDiagStartDebugging(runtimeHandle, callback, IntPtr.Zero);

                    var breakpoints = Engine.JsDiagGetBreakpoints();
                    Assert.True(breakpoints != JavaScriptValueSafeHandle.Invalid);

                    Engine.JsDiagStopDebugging(runtimeHandle);

                    //We didn't specify any breakpoints so...
                    Assert.False(called);
                }
            }
        }

        /// <summary>
        /// End-to-end test of debugging
        /// </summary>
        [Fact]
        public void JsCanBeDebugged()
        {
            string fibonacci = @"
function fibonacci(num){
  var a = 1, b = 0, temp;

  while (num >= 0){
    temp = a;
    a = a + b;
    b = temp;
    num--;
  }

  return b;
};

fibonacci(50);
";
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    //Get the JSON.stringify function for later...
                    var globalObjectHandle = Engine.JsGetGlobalObject();
                    var jsonPropertyHandle = Engine.JsCreatePropertyId("JSON", (ulong)"JSON".Length);
                    var stringifyPropertyHandle = Engine.JsCreatePropertyId("stringify", (ulong)"stringify".Length);
                    var jsonObjectHandle = Engine.JsGetProperty(globalObjectHandle, jsonPropertyHandle);
                    var fnStringifyHandle = Engine.JsGetProperty(jsonObjectHandle, stringifyPropertyHandle);

                    bool firstLineHit = false;
                    string fibonacciFunctionPosition = "";
                    string firstFrameProperties = "";
                    string firstFrameChildren = "";
                    string firstStackObjectName = "";

                    var numDecrementing = new List<int>();
                    int calledCount = 0;

                    //Callback that is run for each breakpoint.
                    JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, IntPtr eventData, IntPtr callbackState) =>
                    {
                        if (eventType == JavaScriptDiagDebugEventType.JsDiagDebugEventRuntimeException)
                        {
                            //When a problem comes along you must skip it
                            //Before the cream sits out too long you must skip it
                            Engine.JsDiagSetStepType(JavaScriptDiagStepType.StepIn);
                        }

                        var stackTrace = Engine.JsDiagGetStackTrace();

                        //Get the first frame in the stack.
                        var ix = Engine.JsIntToNumber(0);
                        var objFrameHandle = Engine.JsGetIndexedProperty(stackTrace, ix);

                        //Get the line and the source
                        var linePropertyHandle = Engine.JsCreatePropertyId("line", (ulong)"line".Length);
                        var lineHandle = Engine.JsGetProperty(objFrameHandle, linePropertyHandle);
                        var line = Engine.JsNumberToInt(lineHandle);

                        var functionHandlePropertyHandle = Engine.JsCreatePropertyId("functionHandle", (ulong)"functionHandle".Length);
                        var functionHandleHandle = Engine.JsGetProperty(objFrameHandle, functionHandlePropertyHandle);
                        var functionHandle = Engine.JsNumberToInt(functionHandleHandle);

                        var sourceTextPropertyHandle = Engine.JsCreatePropertyId("sourceText", (ulong)"sourceText".Length);
                        var sourceTextHandle = Engine.JsGetProperty(objFrameHandle, sourceTextPropertyHandle);
                        var sourceLength = Engine.JsGetStringLength(sourceTextHandle);

                        byte[] buffer = new byte[sourceLength];
                        var written = Engine.JsCopyString(sourceTextHandle, buffer, (ulong)sourceLength);
                        var sourceText = Encoding.UTF8.GetString(buffer);

                        //If we hit the first line...
                        if (sourceText == "fibonacci(50)")
                        {
                            firstLineHit = true;
                            //Get some information about the stack frame.
                            var stackProperties = Engine.JsDiagGetStackProperties(0);

                            //Stringify the propertyHandle.
                            var stringifiedPropertyHandle = Engine.JsCallFunction(fnStringifyHandle, new IntPtr[] { globalObjectHandle.DangerousGetHandle(), stackProperties.DangerousGetHandle() }, 2);
                            var stringifiedPropertyLength = Engine.JsGetStringLength(stringifiedPropertyHandle);

                            byte[] serializedPropertyBuffer = new byte[stringifiedPropertyLength];
                            var stringifiedPropertyWritten = Engine.JsCopyString(stringifiedPropertyHandle, serializedPropertyBuffer, (ulong)stringifiedPropertyLength);
                            firstFrameProperties = Encoding.UTF8.GetString(serializedPropertyBuffer);

                            //Get information about the 'fibonacci' local -- which should be our function.
                            var localsPropertyHandle = Engine.JsCreatePropertyId("locals", (ulong)"locals".Length);
                            var localsHandle = Engine.JsGetProperty(stackProperties, localsPropertyHandle);
                            var objLocalHandle = Engine.JsGetIndexedProperty(localsHandle, ix);

                            //I'm not impressed with the naming of these... ;)
                            var handlePropertyHandle = Engine.JsCreatePropertyId("handle", (ulong)"handle".Length);
                            var handleHandle = Engine.JsGetProperty(objLocalHandle, handlePropertyHandle);
                            var handle = Engine.JsNumberToInt(handleHandle);

                            var objectHandle = Engine.JsDiagGetObjectFromHandle((uint)handle);
                            var namePropertyHandle = Engine.JsCreatePropertyId("name", (ulong)"name".Length);
                            var nameHandle = Engine.JsGetProperty(objectHandle, namePropertyHandle);
                            var nameLength = Engine.JsGetStringLength(nameHandle);

                            byte[] nameBuffer = new byte[nameLength];
                            var nameWritten = Engine.JsCopyString(nameHandle, nameBuffer, (ulong)nameLength);
                            firstStackObjectName = Encoding.UTF8.GetString(nameBuffer);

                            //Get the child properties
                            var objectChildrenHandle = Engine.JsDiagGetProperties((uint)handle, 0, 6);

                            //Stringify the children (muahaha).
                            var stringifiedChildrenHandle = Engine.JsCallFunction(fnStringifyHandle, new IntPtr[] { globalObjectHandle.DangerousGetHandle(), objectChildrenHandle.DangerousGetHandle() }, 2);
                            var stringifiedChildrenLength = Engine.JsGetStringLength(stringifiedChildrenHandle);

                            byte[] serializedChildrenBuffer = new byte[stringifiedChildrenLength];
                            var stringifiedChildrenWritten = Engine.JsCopyString(stringifiedChildrenHandle, serializedChildrenBuffer, (ulong)stringifiedChildrenLength);
                            firstFrameChildren = Encoding.UTF8.GetString(serializedChildrenBuffer);

                            //Set the step
                            Engine.JsDiagSetStepType(JavaScriptDiagStepType.StepIn);
                        }
                        else if (sourceText == "var a = 1")
                        {
                            //this is our step.
                            //do naught.
                        }
                        else if (sourceText == "throw new Error('we did it jim!')")
                        {
                            //This is the exception we're skipping.
                            //do naught.
                        }
                        //Otherwise...
                        else
                        {
                            var evalScript = Engine.JsCreateString("num", (ulong)"num".Length);
                            var numHandle = Engine.JsDiagEvaluate(evalScript, 0, JavaScriptParseScriptAttributes.None, false);

                            var valuePropertyHandle = Engine.JsCreatePropertyId("value", (ulong)"value".Length);
                            var valueHandle = Engine.JsGetProperty(numHandle, valuePropertyHandle);
                            numDecrementing.Add(Engine.JsNumberToInt(valueHandle));
                        }

                        //Increment our call counter (that we're asserting against) and continue.
                        calledCount++;
                        return true;
                    };

                    using (var ss = new ScriptSource(Engine, fibonacci))
                    {
                        Engine.JsDiagStartDebugging(runtimeHandle, callback, IntPtr.Zero);

                        var scripts = Engine.JsDiagGetScripts();
                        Assert.NotEqual(JavaScriptValueSafeHandle.Invalid, scripts);

                        var ix = Engine.JsIntToNumber(0);
                        var objScriptHandle = Engine.JsGetIndexedProperty(scripts, ix);

                        var handleType = Engine.JsGetValueType(objScriptHandle);
                        Assert.True(handleType == JavaScriptValueType.Object);

                        //Not sure if the ScriptId varies independently of the ScriptContext cookie
                        var scriptIdPropertyHandle = Engine.JsCreatePropertyId("scriptId", (ulong)"scriptId".Length);
                        var scriptIdHandle = Engine.JsGetProperty(objScriptHandle, scriptIdPropertyHandle);

                        var scriptId = Engine.JsNumberToInt(scriptIdHandle);

                        //Assert that we can get the source for the script id.
                        var objSourceHandle = Engine.JsDiagGetSource((uint)scriptId);
                        var sourcePropertyHandle = Engine.JsCreatePropertyId("source", (ulong)"source".Length);
                        var sourceHandle = Engine.JsGetProperty(objSourceHandle, sourcePropertyHandle);

                        handleType = Engine.JsGetValueType(sourceHandle);
                        Assert.True(handleType == JavaScriptValueType.String);
                        var sourceLength = Engine.JsGetStringLength(sourceHandle);

                        byte[] buffer = new byte[sourceLength];
                        var written = Engine.JsCopyString(sourceHandle, buffer, (ulong)sourceLength);
                        var source = Encoding.UTF8.GetString(buffer);
                        Assert.Equal(fibonacci, source);


                        //Set a breakpoint with a knkown position
                        var breakPointHandle = Engine.JsDiagSetBreakpoint((uint)scriptId, 5, 0);

                        //Assert that the breakpoint has been set
                        var breakpointsHandle = Engine.JsDiagGetBreakpoints();
                        var objBreakpointHandle = Engine.JsGetIndexedProperty(breakpointsHandle, ix);
                        var breakpointIdPropertyHandle = Engine.JsCreatePropertyId("breakpointId", (ulong)"breakpointId".Length);
                        var breakpointIdHandle = Engine.JsGetProperty(objBreakpointHandle, breakpointIdPropertyHandle);
                        var linePropertyHandle = Engine.JsCreatePropertyId("line", (ulong)"line".Length);
                        var lineHandle = Engine.JsGetProperty(objBreakpointHandle, linePropertyHandle);

                        var line = Engine.JsNumberToInt(lineHandle);
                        var breakPointId = Engine.JsNumberToInt(breakpointIdHandle);

                        Assert.Equal(5, line);
                        Assert.Equal(1, breakPointId);

                        //Get/set the break on exception setting.
                        var breakOnExceptionSetting = Engine.JsDiagGetBreakOnException(runtimeHandle);
                        Assert.True(breakOnExceptionSetting == JavaScriptDiagBreakOnExceptionAttributes.Uncaught);

                        Engine.JsDiagSetBreakOnException(runtimeHandle, JavaScriptDiagBreakOnExceptionAttributes.None);
                        breakOnExceptionSetting = Engine.JsDiagGetBreakOnException(runtimeHandle);
                        Assert.True(breakOnExceptionSetting == JavaScriptDiagBreakOnExceptionAttributes.None);

                        //Get the function position
                        var fibonacciFunctionPositionHandle = Engine.JsDiagGetFunctionPosition(ss.FunctionHandle);

                        //Stringify the function position.
                        var stringifiedFibonacciHandle = Engine.JsCallFunction(fnStringifyHandle, new IntPtr[] { globalObjectHandle.DangerousGetHandle(), fibonacciFunctionPositionHandle.DangerousGetHandle() }, 2);
                        var stringifiedFibonacciLength = Engine.JsGetStringLength(stringifiedFibonacciHandle);

                        byte[] serializedFibonacciBuffer = new byte[stringifiedFibonacciLength];
                        var stringifiedFibonacciWritten = Engine.JsCopyString(stringifiedFibonacciHandle, serializedFibonacciBuffer, (ulong)stringifiedFibonacciLength);
                        fibonacciFunctionPosition = Encoding.UTF8.GetString(serializedFibonacciBuffer);

                        //Break on the first line
                        Engine.JsDiagRequestAsyncBreak(runtimeHandle);

                        var finalResult = Engine.JsCallFunction(ss.FunctionHandle, new IntPtr[] { ss.FunctionHandle.DangerousGetHandle() }, 1);
                        handleType = Engine.JsGetValueType(finalResult);

                        //Fib = 51, first break = 1, step = 1. 51+1+1 = 53
                        Assert.Equal(53, calledCount);

                        //Remove our previous breakpoint
                        Engine.JsDiagRemoveBreakpoint((uint)breakPointId);

                        //Assert the breakpoint has been removed.
                        breakpointsHandle = Engine.JsDiagGetBreakpoints();
                        objBreakpointHandle = Engine.JsGetIndexedProperty(breakpointsHandle, ix);
                        handleType = Engine.JsGetValueType(objBreakpointHandle);
                        Assert.True(handleType == JavaScriptValueType.Undefined);

                        Engine.JsDiagStopDebugging(runtimeHandle);
                    }

                    Assert.True(firstLineHit);
                    Assert.False(string.IsNullOrWhiteSpace(fibonacciFunctionPosition));
                    Assert.False(string.IsNullOrWhiteSpace(firstFrameProperties));
                    Assert.False(string.IsNullOrWhiteSpace(firstFrameChildren));
                    Assert.Equal("fibonacci", firstStackObjectName);
                    Assert.True(numDecrementing.Count == 51);
                }
            }
        }
    }
}
