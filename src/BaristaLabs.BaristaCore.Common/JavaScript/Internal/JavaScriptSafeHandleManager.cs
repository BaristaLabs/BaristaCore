namespace BaristaLabs.BaristaCore.JavaScript.Internal
{
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    /// Manages the lifecycle of JavaScriptSafeHandle objects.
    /// </summary>
    internal static class JavaScriptSafeHandleManager
    {
        private static ConcurrentDictionary<IntPtr, WeakCollection<JavaScriptObjectBeforeCollectCallback>> s_objectBeforeCollect = new ConcurrentDictionary<IntPtr, WeakCollection<JavaScriptObjectBeforeCollectCallback>>();

        internal static void MonitorJavaScriptSafeHandle<T>(T handle, IntPtr callbackState = default(IntPtr)) where T : JavaScriptSafeHandle<T>
        {
            if (callbackState == default(IntPtr))
                callbackState = IntPtr.Zero;

            IntPtr handlePtr = handle.DangerousGetHandle();
            if (handlePtr == IntPtr.Zero)
                return;

            //Certain values, such as undefined, null, true, false, etc, cannot have a callback set. InvalidArgument is returned in these situations.
            var errorCode = LibChakraCore.JsSetObjectBeforeCollectCallback(handle, callbackState, OnObjectBeforeCollect);
            if (errorCode == JavaScriptErrorCode.NoError)
            {

                //If any other error occured, throw it.
                Errors.ThrowIfError(errorCode);

                if (s_objectBeforeCollect.ContainsKey(handlePtr))
                {
                    WeakCollection<JavaScriptObjectBeforeCollectCallback> callbacks;
                    if (s_objectBeforeCollect.TryGetValue(handlePtr, out callbacks))
                    {
                        if (!callbacks.Contains(handle.ObjectBeforeCollectCallback))
                        {
                            callbacks.Add(handle.ObjectBeforeCollectCallback);
                        }
                    }
                }
                else
                {
                    var callbacks = new WeakCollection<JavaScriptObjectBeforeCollectCallback>();
                    callbacks.Add(handle.ObjectBeforeCollectCallback);
                    s_objectBeforeCollect.TryAdd(handlePtr, callbacks);
                }
            }
        }

        private static void OnObjectBeforeCollect(IntPtr handle, IntPtr callbackState)
        {
            WeakCollection<JavaScriptObjectBeforeCollectCallback> callbacks;
            if (s_objectBeforeCollect.TryGetValue(handle, out callbacks))
            {
                foreach (var objectBeforeCollectCallback in callbacks)
                {
                    objectBeforeCollectCallback.Invoke(handle, callbackState);
                }
                callbacks.Clear();
            }
        }
    }
}
