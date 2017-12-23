namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    /// Represents an object that maintains a pool of barista objects as flyweights.
    /// </summary>
    /// <remarks>
    /// When objects are received from the JavaScript engine, their wrapping flyweight objects become
    /// interned within a pool using their type and handle id as key. JavaScript References are notified
    /// when the JavaScript engine is about to collect them, and the corresponding flyweight is disposed
    /// and removed from interning.
    /// </remarks>
    public sealed class BaristaObjectPool<TBaristaObject, TJavaScriptReference> : IDisposable
        where TBaristaObject : class, IBaristaObject<TJavaScriptReference>
        where TJavaScriptReference : JavaScriptReference<TJavaScriptReference>
    {
        private ConcurrentDictionary<TJavaScriptReference, WeakReference<TBaristaObject>> m_javaScriptReferencePool = new ConcurrentDictionary<TJavaScriptReference, WeakReference<TBaristaObject>>();

        public BaristaObjectPool()
        {
        }

        /// <summary>
        /// Gets the number of objects in the pool.
        /// </summary>
        public int Count
        {
            get { return m_javaScriptReferencePool.Count; }
        }

        /// <summary>
        /// Attempts to add the specified flyweight to the pool directly.
        /// </summary>
        /// <remarks>
        /// Use on operations that have 'create'-like behavior.
        /// </remarks>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool TryAdd(TBaristaObject obj)
        {
            if (obj == null || obj.Handle == null || obj.Handle.IsInvalid)
                throw new ArgumentNullException(nameof(obj));

            return m_javaScriptReferencePool.TryAdd(obj.Handle, new WeakReference<TBaristaObject>(obj));
        }

        /// <summary>
        /// Attempts to get the specified flyweight from the pool directly
        /// </summary>
        /// <param name="jsRef"></param>
        /// <returns></returns>
        public bool TryGet(TJavaScriptReference jsRef, out TBaristaObject obj)
        {
            if (jsRef == default(TJavaScriptReference) || jsRef.IsInvalid)
                throw new ArgumentNullException(nameof(jsRef));

            if (m_javaScriptReferencePool.TryGetValue(jsRef, out WeakReference<TBaristaObject> weakObj))
            {
                return weakObj.TryGetTarget(out obj);
            }

            obj = null;
            return false;
        }

        /// <summary>
        /// Gets or adds the specified JavaScript Reference to the pool.
        /// </summary>
        /// <param name="jsRef"></param>
        /// <returns></returns>
        public TBaristaObject GetOrAdd(TJavaScriptReference jsRef, Func<TBaristaObject> flyweightFactory)
        {
            if (jsRef == default(TJavaScriptReference) || jsRef.IsInvalid)
                throw new ArgumentNullException(nameof(jsRef));

            //For the specified handle, attempt to get or add a flyweight.
            var weakReferenceToTarget = m_javaScriptReferencePool.GetOrAdd(jsRef, (ptr) =>
            {
                TBaristaObject flyweight = flyweightFactory();
                return new WeakReference<TBaristaObject>(flyweight);
            });

            if (weakReferenceToTarget.TryGetTarget(out TBaristaObject target))
            {
                //We have an existing target, dispose of the temporary one.
                if (!ReferenceEquals(jsRef, target.Handle))
                {
                    jsRef.Dispose();
                }
                return target;
            }

            //The existing flyweight has been disposed, create and add a new flyweight.
            TBaristaObject newValue = flyweightFactory();
            if (!m_javaScriptReferencePool.TryUpdate(jsRef, new WeakReference<TBaristaObject>(newValue), weakReferenceToTarget))
                throw new InvalidOperationException("Unable to get or add the JavaScript Reference.");

            return newValue;
        }

        /// <summary>
        /// Removes the specified handle from the pool, disposing of it.
        /// </summary>
        /// <param name="handle"></param>
        public void RemoveHandle(TJavaScriptReference handle)
        {
            if (m_javaScriptReferencePool.TryRemove(handle, out WeakReference<TBaristaObject> value))
            {
                if (value.TryGetTarget(out TBaristaObject jsRef))
                {
                    if (jsRef != null && !jsRef.IsDisposed)
                    {
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

        ~BaristaObjectPool()
        {
            Dispose(false);
        }
        #endregion
    }
}
