namespace BaristaLabs.BaristaCore.JavaScript
{
    using Internal;
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    /// Manages the lifecycle of JavaScriptReference objects.
    /// </summary>
    public static class JavaScriptObjectManager
    {
        private static ConcurrentDictionary<IntPtr, WeakCollection<JavaScriptObjectBeforeCollectCallback>> s_objectBeforeCollect = new ConcurrentDictionary<IntPtr, WeakCollection<JavaScriptObjectBeforeCollectCallback>>();

        public static void MonitorJavaScriptObjectLifetime<T>(T handle, JavaScriptObjectBeforeCollectCallback callback, IntPtr callbackState = default(IntPtr)) where T : JavaScriptReference<T>
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
                        if (!callbacks.Contains(callback))
                        {
                            callbacks.Add(callback);
                        }
                    }
                }
                else
                {
                    var callbacks = new WeakCollection<JavaScriptObjectBeforeCollectCallback>();
                    callbacks.Add(callback);
                    s_objectBeforeCollect.TryAdd(handlePtr, callbacks);
                }
            }
        }

        public static void DisposeWhenCollected<T>(T handle) where T : JavaScriptReference<T>
        {
            IntPtr ptr = handle.DangerousGetHandle();
            MonitorJavaScriptObjectLifetime(handle, (IntPtr handlePtr, IntPtr callbackState) =>
            {
                if (ptr == handlePtr)
                    handle.Dispose();
            });
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
