namespace BaristaLabs.BaristaCore.JavaScript
{
    using Internal;
    using System.Diagnostics;

    public static class Errors
    {
        /// <summary>
        /// Throws if a native method returns an error code.
        /// </summary>
        /// <param name="error">The error.</param>
        [DebuggerStepThrough]
        public static void ThrowIfError(JavaScriptErrorCode error)
        {
            if (error != JavaScriptErrorCode.NoError)
            {
                switch (error)
                {
                    case JavaScriptErrorCode.InvalidArgument:
                        throw new JavaScriptUsageException(error, "Invalid argument.");

                    case JavaScriptErrorCode.NullArgument:
                        throw new JavaScriptUsageException(error, "Null argument.");

                    case JavaScriptErrorCode.NoCurrentContext:
                        throw new JavaScriptUsageException(error, "No current context.");

                    case JavaScriptErrorCode.InExceptionState:
                        throw new JavaScriptUsageException(error, "Runtime is in exception state.");

                    case JavaScriptErrorCode.NotImplemented:
                        throw new JavaScriptUsageException(error, "Method is not implemented.");

                    case JavaScriptErrorCode.WrongThread:
                        throw new JavaScriptUsageException(error, "Runtime is active on another thread.");

                    case JavaScriptErrorCode.RuntimeInUse:
                        throw new JavaScriptUsageException(error, "Runtime is in use.");

                    case JavaScriptErrorCode.BadSerializedScript:
                        throw new JavaScriptUsageException(error, "Bad serialized script.");

                    case JavaScriptErrorCode.InDisabledState:
                        throw new JavaScriptUsageException(error, "Runtime is disabled.");

                    case JavaScriptErrorCode.CannotDisableExecution:
                        throw new JavaScriptUsageException(error, "Cannot disable execution.");

                    case JavaScriptErrorCode.AlreadyDebuggingContext:
                        throw new JavaScriptUsageException(error, "Context is already in debug mode.");

                    case JavaScriptErrorCode.HeapEnumInProgress:
                        throw new JavaScriptUsageException(error, "Heap enumeration is in progress.");

                    case JavaScriptErrorCode.ArgumentNotObject:
                        throw new JavaScriptUsageException(error, "Argument is not an object.");

                    case JavaScriptErrorCode.InProfileCallback:
                        throw new JavaScriptUsageException(error, "In a profile callback.");

                    case JavaScriptErrorCode.InThreadServiceCallback:
                        throw new JavaScriptUsageException(error, "In a thread service callback.");

                    case JavaScriptErrorCode.CannotSerializeDebugScript:
                        throw new JavaScriptUsageException(error, "Cannot serialize a debug script.");

                    case JavaScriptErrorCode.AlreadyProfilingContext:
                        throw new JavaScriptUsageException(error, "Already profiling this context.");

                    case JavaScriptErrorCode.IdleNotEnabled:
                        throw new JavaScriptUsageException(error, "Idle is not enabled.");

                    case JavaScriptErrorCode.OutOfMemory:
                        throw new JavaScriptEngineException(error, "Out of memory.");

                    case JavaScriptErrorCode.ScriptException:
                        {
                            JavaScriptErrorCode innerError = LibChakraCore.JsGetAndClearException(out JavaScriptValueSafeHandle errorObject);

                            if (innerError != JavaScriptErrorCode.NoError)
                            {
                                throw new JavaScriptFatalException(innerError);
                            }

                            throw new JavaScriptScriptException(error, errorObject, "Script threw an exception.");
                        }

                    case JavaScriptErrorCode.ScriptCompile:
                        {
                            JavaScriptErrorCode innerError = LibChakraCore.JsGetAndClearException(out JavaScriptValueSafeHandle errorObject);

                            if (innerError != JavaScriptErrorCode.NoError)
                            {
                                throw new JavaScriptFatalException(innerError);
                            }

                            throw new JavaScriptScriptException(error, errorObject, "Compile error.");
                        }

                    case JavaScriptErrorCode.ScriptTerminated:
                        throw new JavaScriptScriptException(error, JavaScriptValueSafeHandle.Invalid, "Script was terminated.");

                    case JavaScriptErrorCode.ScriptEvalDisabled:
                        throw new JavaScriptScriptException(error, JavaScriptValueSafeHandle.Invalid, "Eval of strings is disabled in this runtime.");

                    case JavaScriptErrorCode.Fatal:
                        throw new JavaScriptFatalException(error);

                    default:
                        throw new JavaScriptFatalException(error);
                }
            }
        }
    }
}
