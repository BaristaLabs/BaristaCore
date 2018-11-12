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
        public static void ThrowIfError(JsErrorCode error)
        {
            if (error != JsErrorCode.NoError)
            {
                switch (error)
                {
                    case JsErrorCode.InvalidArgument:
                        throw new JsUsageException(error, "Invalid argument.");

                    case JsErrorCode.NullArgument:
                        throw new JsUsageException(error, "Null argument.");

                    case JsErrorCode.NoCurrentContext:
                        throw new JsUsageException(error, "No current context.");

                    case JsErrorCode.InExceptionState:
                        throw new JsUsageException(error, "Runtime is in exception state.");

                    case JsErrorCode.NotImplemented:
                        throw new JsUsageException(error, "Method is not implemented.");

                    case JsErrorCode.WrongThread:
                        throw new JsUsageException(error, "Runtime is active on another thread.");

                    case JsErrorCode.RuntimeInUse:
                        throw new JsUsageException(error, "Runtime is in use.");

                    case JsErrorCode.BadSerializedScript:
                        throw new JsUsageException(error, "Bad serialized script.");

                    case JsErrorCode.InDisabledState:
                        throw new JsUsageException(error, "Runtime is disabled.");

                    case JsErrorCode.CannotDisableExecution:
                        throw new JsUsageException(error, "Cannot disable execution.");

                    case JsErrorCode.AlreadyDebuggingContext:
                        throw new JsUsageException(error, "Context is already in debug mode.");

                    case JsErrorCode.HeapEnumInProgress:
                        throw new JsUsageException(error, "Heap enumeration is in progress.");

                    case JsErrorCode.ArgumentNotObject:
                        throw new JsUsageException(error, "Argument is not an object.");

                    case JsErrorCode.InProfileCallback:
                        throw new JsUsageException(error, "In a profile callback.");

                    case JsErrorCode.InThreadServiceCallback:
                        throw new JsUsageException(error, "In a thread service callback.");

                    case JsErrorCode.CannotSerializeDebugScript:
                        throw new JsUsageException(error, "Cannot serialize a debug script.");

                    case JsErrorCode.AlreadyProfilingContext:
                        throw new JsUsageException(error, "Already profiling this context.");

                    case JsErrorCode.IdleNotEnabled:
                        throw new JsUsageException(error, "Idle is not enabled.");

                    case JsErrorCode.OutOfMemory:
                        throw new JsEngineException(error, "Out of memory.");

                    case JsErrorCode.ScriptException:
                        {
                            JsErrorCode innerError = LibChakraCore.JsGetAndClearException(out JavaScriptValueSafeHandle errorObject);

                            //The following throws a really bad error, but if you set and get the error again, it works, but with no actual metadata :-/
                            //JsErrorCode innerError = LibChakraCore.JsGetAndClearExceptionWithMetadata(out JavaScriptValueSafeHandle metadataObject);
                            //innerError = LibChakraCore.JsGetPropertyIdFromName("exception", out JavaScriptPropertyIdSafeHandle propertyId);
                            //innerError = LibChakraCore.JsGetProperty(metadataObject, propertyId, out JavaScriptValueSafeHandle errorObject);

                            if (innerError != JsErrorCode.NoError)
                            {
                                throw new JsFatalException(innerError);
                            }

                            innerError = LibChakraCore.JsSetException(errorObject);
                            if (innerError != JsErrorCode.NoError)
                            {
                                throw new JsFatalException(innerError);
                            }

                            throw new JsScriptException(error, errorObject, "Script threw an exception.");
                        }

                    case JsErrorCode.ScriptCompile:
                        {
                            JsErrorCode innerError;
                            innerError = LibChakraCore.JsHasException(out bool hasException);
                            //We attempted to clear the exception, but the result of that action was an exception.
                            if (innerError != JsErrorCode.NoError)
                            {
                                throw new JsFatalException(innerError);
                            }

                            // Only throw an exception if the runtime is currently in an exception state
                            // Parse errors are inheritly continuable.
                            if (hasException)
                            {
                                innerError = LibChakraCore.JsGetAndClearException(out JavaScriptValueSafeHandle errorObject);

                                //We attempted to clear the exception, but the result of that action was an exception.
                                if (innerError != JsErrorCode.NoError)
                                {
                                    throw new JsFatalException(innerError);
                                }

                                innerError = LibChakraCore.JsSetException(errorObject);
                                if (innerError != JsErrorCode.NoError)
                                {
                                    throw new JsFatalException(innerError);
                                }

                                throw new JsScriptException(error, errorObject, "Compile error.");
                            }
                        }
                        break;
                    case JsErrorCode.ScriptTerminated:
                        throw new JsScriptException(error, JavaScriptValueSafeHandle.Invalid, "Script was terminated.");

                    case JsErrorCode.ScriptEvalDisabled:
                        throw new JsScriptException(error, JavaScriptValueSafeHandle.Invalid, "Eval of strings is disabled in this runtime.");
                    
                    case JsErrorCode.Fatal:
                        throw new JsFatalException(error);

                    default:
                        throw new JsFatalException(error);
                }
            }
        }
    }
}
