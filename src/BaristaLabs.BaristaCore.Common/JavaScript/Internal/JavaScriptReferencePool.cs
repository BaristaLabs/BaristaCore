namespace BaristaLabs.BaristaCore.JavaScript.Internal
{
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    /// Represents an object that maintains a pool of flyweight objects.
    /// </summary>
    /// <remarks>
    /// When objects are received from the JavaScript engine, their wrapping flyweight objects become
    /// interned within a pool using their type and handle id as key. JavaScript References are notified
    /// when the JavaScript engine is about to collect them, and the corresponding flyweight is disposed
    /// and removed from interning.
    /// </remarks>
    internal abstract class JavaScriptReferencePool<TJavaScriptReferenceFlyweight, TJavaScriptReference> : IDisposable
        where TJavaScriptReferenceFlyweight : JavaScriptReferenceFlyweight<TJavaScriptReference>
        where TJavaScriptReference : JavaScriptReference<TJavaScriptReference>
    {
        private readonly IJavaScriptEngine m_engine;
        private ConcurrentDictionary<IntPtr, WeakReference<TJavaScriptReferenceFlyweight>> m_javaScriptReferencePool = new ConcurrentDictionary<IntPtr, WeakReference<TJavaScriptReferenceFlyweight>>();

        /// <summary>
        /// Gets the engine associated with the pool.
        /// </summary>
        public  IJavaScriptEngine Engine
        {
            get { return m_engine; }
        }

        protected JavaScriptReferencePool(IJavaScriptEngine engine)
        {
            if (engine == null)
                throw new ArgumentNullException(nameof(engine));

            m_engine = engine;
        }

        /// <summary>
        /// Attempts to add the specified flyweight to the pool directly.
        /// </summary>
        /// <remarks>
        /// Use on operations that have 'create'-like behavior.
        /// </remarks>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool TryAdd(TJavaScriptReferenceFlyweight obj)
        {
            if (obj == null || obj.Handle == null || obj.Handle.IsInvalid)
                throw new ArgumentNullException(nameof(obj));

            IntPtr handle = obj.Handle.DangerousGetHandle();

            return m_javaScriptReferencePool.TryAdd(handle, new WeakReference<TJavaScriptReferenceFlyweight>(obj));
        }

        /// <summary>
        /// Gets or adds the specified JavaScript Reference to the pool.
        /// </summary>
        /// <param name="jsRef"></param>
        /// <returns></returns>
        public TJavaScriptReferenceFlyweight GetOrAdd(TJavaScriptReference jsRef)
        {
            if (jsRef == default(TJavaScriptReference) || jsRef.IsInvalid)
                throw new ArgumentNullException(nameof(jsRef));

            IntPtr handle = jsRef.DangerousGetHandle();

            //For the specified handle, attempt to get or add a flyweight.
            var weakReferenceToTarget = m_javaScriptReferencePool.GetOrAdd(handle, (ptr) => 
            {
                TJavaScriptReferenceFlyweight flyweight = FlyweightFactory(jsRef);
                return new WeakReference<TJavaScriptReferenceFlyweight>(flyweight);
            });

            TJavaScriptReferenceFlyweight target;
            if (weakReferenceToTarget.TryGetTarget(out target))
            {
                //We have an existing target, dispose of the temporary one.
                if (!ReferenceEquals(jsRef, target.Handle))
                {
                    jsRef.Dispose();
                }
                return target;
            }

            //The existing flyweight has been disposed, create and add a new flyweight.
            TJavaScriptReferenceFlyweight newValue = FlyweightFactory(jsRef);
            if (!m_javaScriptReferencePool.TryUpdate(handle, new WeakReference<TJavaScriptReferenceFlyweight>(newValue), weakReferenceToTarget))
                throw new InvalidOperationException("Unable to get or add the JavaScript Reference.");

            return newValue;
        }

        /// <summary>
        /// Performs any cleanup prior to disposal of the flyweight.
        /// </summary>
        /// <param name="target"></param>
        protected abstract void ReleaseJavaScriptReference(TJavaScriptReferenceFlyweight target);

        /// <summary>
        /// Removes the specified handle from the pool, disposing of it.
        /// </summary>
        /// <param name="handle"></param>
        protected void RemoveHandle(IntPtr handle)
        {
            WeakReference<TJavaScriptReferenceFlyweight> value;
            if (m_javaScriptReferencePool.TryRemove(handle, out value))
            {
                TJavaScriptReferenceFlyweight jsRef;
                if (value.TryGetTarget(out jsRef))
                {
                    if (jsRef != null && !jsRef.IsDisposed)
                    {
                        ReleaseJavaScriptReference(jsRef);
                        jsRef.Dispose();
                    }
                }
            }
        }


        /// <summary>
        /// When implemented in a concrete class, returns an appropriate flyweight object for the specified javascript reference.
        /// </summary>
        /// <param name="jsRef"></param>
        /// <returns></returns>
        protected abstract TJavaScriptReferenceFlyweight FlyweightFactory(TJavaScriptReference jsRef);

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach(IntPtr handle in m_javaScriptReferencePool.Keys)
                {
                    RemoveHandle(handle);
                }
            }
        }

        /// <summary>
        /// Disposes of the pool and all references contained within.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~JavaScriptReferencePool()
        {
            Dispose(false);
        }
        #endregion
    }
}
