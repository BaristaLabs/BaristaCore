namespace BaristaLabs.BaristaCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    /// <summary>
    /// A collection of weak references to objects. By default, weak references are purged by iteration/count operations, not by add/remove operations.
    /// </summary>
    /// <typeparam name="T">The type of object to hold weak references to.</typeparam>
    /// <remarks>
    /// <para>Since the collection holds weak references to the actual objects, the collection is comprised of both living and dead references. Living references refer to objects that have not been garbage collected, and may be used as normal references. Dead references refer to objects that have been garbage collected.</para>
    /// <para>Dead references do consume resources; each dead reference is a garbage collection handle.</para>
    /// <para>Dead references may be cleaned up by a <see cref="Purge"/> operation. Some properties and methods cause a purge as a side effect; the member documentation specifies whether a purge takes place.</para>
    /// </remarks>
    public sealed class WeakCollection<T> : ICollection<T> where T : class
    {
        /// <summary>
        /// The actual collection of strongly-typed weak references.
        /// </summary>
        private List<WeakReference<T>> m_list;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakCollection{T}"/> class that is empty.
        /// </summary>
        public WeakCollection()
        {
            m_list = new List<WeakReference<T>>();
        }

        /// <summary>
        /// Gets a sequence of live objects from the collection, causing a purge.
        /// </summary>
        public IEnumerable<T> LiveList
        {
            get
            {
                List<T> ret = new List<T>(m_list.Count);
                ret.AddRange(UnsafeLiveList);
                return ret;
            }
        }

        /// <summary>
        /// Gets a complete sequence of objects from the collection. Does not cause a purge. Null entries represent dead objects.
        /// </summary>
        public IEnumerable<T> CompleteList
        {
            get
            {
                return m_list.Select(x =>
                {
                    T target;
                    if (x.TryGetTarget(out target))
                        return target;
                    return null;
                });
            }
        }

        /// <summary>
        /// Gets a sequence of live objects from the collection without causing a purge.
        /// </summary>
        public IEnumerable<T> LiveListWithoutPurge
        {
            get
            {
                return CompleteList.Where(x => x != null);
            }
        }

        /// <summary>
        /// Gets the number of live and dead entries in the collection. Does not cause a purge. O(1).
        /// </summary>
        public int CompleteCount
        {
            get
            {
                return m_list.Count;
            }
        }

        /// <summary>
        /// Gets the number of dead entries in the collection. Does not cause a purge. O(n).
        /// </summary>
        public int DeadCount
        {
            get
            {
                return CompleteList.Count(x => x == null);
            }
        }

        /// <summary>
        /// Gets the number of live entries in the collection, causing a purge. O(n).
        /// </summary>
        public int LiveCount
        {
            get
            {
                return UnsafeLiveList.Count();
            }
        }

        /// <summary>
        /// Gets the number of live entries in the collection without causing a purge. O(n).
        /// </summary>
        public int LiveCountWithoutPurge
        {
            get
            {
                return CompleteList.Count(x => x != null);
            }
        }

        #region ICollection<T> Properties

        /// <summary>
        /// Gets the number of live entries in the collection, causing a purge. O(n).
        /// </summary>
        int ICollection<T>.Count
        {
            get { return LiveCount; }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read only.
        /// </summary>
        bool ICollection<T>.IsReadOnly
        {
            get { return ((ICollection<T>)m_list).IsReadOnly; }
        }

        #endregion

        /// <summary>
        /// Gets a sequence of live objects from the collection, causing a purge. The entire sequence MUST always be enumerated!
        /// </summary>
        private IEnumerable<T> UnsafeLiveList
        {
            get
            {
                //Iterate through the list, when a 'dead' reference is found, shift the list at that position left.
                int writeIndex = 0;
                for (int readIndex = 0; readIndex != m_list.Count; ++readIndex)
                {
                    WeakReference<T> weakReference = m_list[readIndex];
                    T weakDelegate;
                    if (weakReference.TryGetTarget(out weakDelegate))
                    {
                        yield return weakDelegate;

                        if (readIndex != writeIndex)
                        {
                            m_list[writeIndex] = m_list[readIndex];
                        }

                        ++writeIndex;
                    }
                }

                m_list.RemoveRange(writeIndex, m_list.Count - writeIndex);
            }
        }

        /// <summary>
        /// Adds a weak reference to an object to the collection. Does not cause a purge.
        /// </summary>
        /// <param name="item">The object to add a weak reference to.</param>
        public void Add(T item)
        {
            m_list.Add(new WeakReference<T>(item));
        }

        /// <summary>
        /// Removes a weak reference to an object from the collection. Does not cause a purge.
        /// </summary>
        /// <param name="item">The object to remove a weak reference to.</param>
        /// <returns>True if the object was found and removed; false if the object was not found.</returns>
        public bool Remove(T item)
        {
            for (int i = 0; i != m_list.Count; ++i)
            {
                WeakReference<T> weakReference = m_list[i];
                T weakDelegate;
                if (weakReference.TryGetTarget(out weakDelegate))
                {
                    if (weakDelegate == item)
                    {
                        m_list.RemoveAt(i);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Removes all dead objects from the collection.
        /// </summary>
        public void Purge()
        {
            UnsafeLiveList.All( x => true);
        }

        /// <summary>
        /// Empties the collection.
        /// </summary>
        public void Clear()
        {
            m_list.Clear();
        }

        #region ICollection<T> Methods

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate.</param>
        /// <returns>True if the collection contains a specific value; false if it does not.</returns>
        public bool Contains(T item)
        {
            return LiveListWithoutPurge.Contains(item);
        }

        /// <summary>
        /// Copies all live objects to an array.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="arrayIndex">The index to begin writing into the array.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            List<T> ret = new List<T>(m_list.Count);
            ret.AddRange(UnsafeLiveList);
            ret.CopyTo(array, arrayIndex);
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Gets a sequence of live objects from the collection, causing a purge.
        /// </summary>
        /// <returns>The sequence of live objects.</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return LiveList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Gets a sequence of live objects from the collection, causing a purge.
        /// </summary>
        /// <returns>The sequence of live objects.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        #endregion
    }
}
