namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    /// Represents the base class of an observer that monitors JavaScript References and notifies all active instances.
    /// </summary>
    public abstract class JavaScriptReferenceObserver<TJavaScriptReferenceWrapper, TJavaScriptReference>
        where TJavaScriptReferenceWrapper : JavaScriptReferenceWrapper<TJavaScriptReference>
        where TJavaScriptReference : JavaScriptReference<TJavaScriptReference>
    {
        private ConcurrentDictionary<IntPtr, WeakCollection<TJavaScriptReferenceWrapper>> m_handleReferences = new ConcurrentDictionary<IntPtr, WeakCollection<TJavaScriptReferenceWrapper>>();

        private readonly IJavaScriptEngine m_javaScriptEngine;

        protected IJavaScriptEngine Engine
        {
            get { return m_javaScriptEngine; }
        }

        protected JavaScriptReferenceObserver(IJavaScriptEngine engine)
        {
            if (engine == null)
                throw new ArgumentNullException(nameof(engine));

            m_javaScriptEngine = engine;
        }

        protected bool IsMonitoringHandle(IntPtr handlePtr)
        {
            return m_handleReferences.ContainsKey(handlePtr);
        }

        /// <summary>
        /// Monitors the specified JavaScript Reference
        /// </summary>
        /// <param name="reference"></param>
        public void MonitorJavaScriptReference(TJavaScriptReferenceWrapper reference)
        {
            IntPtr handlePtr = StartMonitor(reference);
            if (handlePtr == null || handlePtr == IntPtr.Zero)
                return;

            if (m_handleReferences.ContainsKey(handlePtr))
            {
                WeakCollection<TJavaScriptReferenceWrapper> references;
                if (m_handleReferences.TryGetValue(handlePtr, out references))
                {
                    if (!references.Contains(reference))
                    {
                        references.Add(reference);
                    }
                }
            }
            else
            {
                var references = new WeakCollection<TJavaScriptReferenceWrapper>();
                references.Add(reference);
                m_handleReferences.TryAdd(handlePtr, references);
            }
        }

        /// <summary>
        /// Concrete implementation of the observer.
        /// </summary>
        /// <remarks>
        /// For a runtime, this is going to be calling JsSetRuntimeBeforeCollectCallback, for Property/Context/Value, JsSetObjectBeforeCollectCallback.
        /// This is also valid for JsSetRuntimeMemoryAllocationCallback.
        /// </remarks>
        /// <param name="handle"></param>
        protected abstract IntPtr StartMonitor(TJavaScriptReferenceWrapper wrapper);

        /// <summary>
        /// Called by the implementing class when the notification event occurs.
        /// </summary>
        protected bool NotifyAll(IntPtr handle, Action<TJavaScriptReferenceWrapper> notification, bool shouldClear = false)
        {
            WeakCollection<TJavaScriptReferenceWrapper> references;
            if (!m_handleReferences.TryGetValue(handle, out references))
                return false;

            foreach (var reference in references)
            {
                notification(reference);
            }

            if (shouldClear)
                references.Clear();

            return true;
        }
    }
}
