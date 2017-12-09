namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
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
    public sealed class JavaScriptReferencePool<TJavaScriptReferenceFlyweight, TJavaScriptReference> : IDisposable
        where TJavaScriptReferenceFlyweight : JavaScriptReferenceFlyweight<TJavaScriptReference>
        where TJavaScriptReference : JavaScriptReference<TJavaScriptReference>
    {
        private ConcurrentDictionary<TJavaScriptReference, WeakReference<TJavaScriptReferenceFlyweight>> m_javaScriptReferencePool = new ConcurrentDictionary<TJavaScriptReference, WeakReference<TJavaScriptReferenceFlyweight>>();
        private readonly Action<TJavaScriptReferenceFlyweight> m_releaseJavaScriptReference;

        public JavaScriptReferencePool(Action<TJavaScriptReferenceFlyweight> releaseJavaScriptReference = null)
        {
            m_releaseJavaScriptReference = releaseJavaScriptReference;
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

            return m_javaScriptReferencePool.TryAdd(obj.Handle, new WeakReference<TJavaScriptReferenceFlyweight>(obj));
        }

        /// <summary>
        /// Gets or adds the specified JavaScript Reference to the pool.
        /// </summary>
        /// <param name="jsRef"></param>
        /// <returns></returns>
        public TJavaScriptReferenceFlyweight GetOrAdd(TJavaScriptReference jsRef, Func<TJavaScriptReferenceFlyweight> flyweightFactory)
        {
            if (jsRef == default(TJavaScriptReference) || jsRef.IsInvalid)
                throw new ArgumentNullException(nameof(jsRef));

            //For the specified handle, attempt to get or add a flyweight.
            var weakReferenceToTarget = m_javaScriptReferencePool.GetOrAdd(jsRef, (ptr) =>
            {
                TJavaScriptReferenceFlyweight flyweight = flyweightFactory();
                return new WeakReference<TJavaScriptReferenceFlyweight>(flyweight);
            });

            if (weakReferenceToTarget.TryGetTarget(out TJavaScriptReferenceFlyweight target))
            {
                //We have an existing target, dispose of the temporary one.
                if (!ReferenceEquals(jsRef, target.Handle))
                {
                    jsRef.Dispose();
                }
                return target;
            }

            //The existing flyweight has been disposed, create and add a new flyweight.
            TJavaScriptReferenceFlyweight newValue = flyweightFactory();
            if (!m_javaScriptReferencePool.TryUpdate(jsRef, new WeakReference<TJavaScriptReferenceFlyweight>(newValue), weakReferenceToTarget))
                throw new InvalidOperationException("Unable to get or add the JavaScript Reference.");

            return newValue;
        }

        /// <summary>
        /// Removes the specified handle from the pool, disposing of it.
        /// </summary>
        /// <param name="handle"></param>
        public void RemoveHandle(TJavaScriptReference handle)
        {
            if (m_javaScriptReferencePool.TryRemove(handle, out WeakReference<TJavaScriptReferenceFlyweight> value))
            {
                if (value.TryGetTarget(out TJavaScriptReferenceFlyweight jsRef))
                {
                    if (jsRef != null && !jsRef.IsDisposed)
                    {
                        m_releaseJavaScriptReference?.Invoke(jsRef);
                        jsRef.Dispose();
                    }
                }
            }
        }

        #region IDisposable
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var handle in m_javaScriptReferencePool.Keys)
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
