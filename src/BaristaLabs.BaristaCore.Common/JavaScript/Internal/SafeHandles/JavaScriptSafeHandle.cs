namespace BaristaLabs.BaristaCore.JavaScript.Internal
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a base implmentation of a JavaScript Safe Handle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class JavaScriptSafeHandle<T> : SafeHandle, IEquatable<JavaScriptSafeHandle<T>> where T : JavaScriptSafeHandle<T>
    {
        private volatile bool m_objectHasBeenCollected = false;

        /// <summary>
        /// Gets a value that indicates if the object has been flagged as collected by the JavaScript Runtime.
        /// </summary>
        public bool IsCollected
        {
            get { return m_objectHasBeenCollected; }
            protected set { m_objectHasBeenCollected = value; }
        }

        public override bool IsInvalid
        {
            get
            {
                if (m_objectHasBeenCollected)
                    return true;

                return handle == IntPtr.Zero;
            }
        }

        /// <summary>
        /// Gets the native function call which created the safe handle.
        /// </summary>
        /// <remarks>
        /// Useful when debugging.
        /// </remarks>
        public string NativeFunctionSource
        {
            get;
            set;
        }

        protected JavaScriptSafeHandle() :
            base(IntPtr.Zero, ownsHandle: true)
        {
        }

        protected JavaScriptSafeHandle(IntPtr handle) :
            base(IntPtr.Zero, ownsHandle: true)
        {
            this.handle = handle;
        }

        /// <summary>
        ///     Releases resources associated with the context.
        /// </summary>
        protected override bool ReleaseHandle()
        {
            //Do nothing.
            return true;
        }

        #region Equality
        /// <summary>
        ///     The equality operator for contexts.
        /// </summary>
        /// <param name="left">The first context to compare.</param>
        /// <param name="right">The second context to compare.</param>
        /// <returns>Whether the two contexts are the same.</returns>
        public static bool operator ==(JavaScriptSafeHandle<T> left, JavaScriptSafeHandle<T> right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, default(JavaScriptSafeHandle<T>)))
            {
                return false;
            }

            if (ReferenceEquals(right, default(JavaScriptSafeHandle<T>)))
            {
                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        ///     The inequality operator for contexts.
        /// </summary>
        /// <param name="left">The first context to compare.</param>
        /// <param name="right">The second context to compare.</param>
        /// <returns>Whether the two contexts are not the same.</returns>
        public static bool operator !=(JavaScriptSafeHandle<T> left, JavaScriptSafeHandle<T> right)
        {
            if (ReferenceEquals(left, right))
            {
                return false;
            }

            if (ReferenceEquals(left, default(JavaScriptSafeHandle<T>)))
            {
                return true;
            }

            if (ReferenceEquals(right, default(JavaScriptSafeHandle<T>)))
            {
                return true;
            }

            return !left.Equals(right);
        }

        /// <summary>
        ///     Checks for equality between contexts.
        /// </summary>
        /// <param name="other">The other context to compare.</param>
        /// <returns>Whether the two contexts are the same.</returns>
        public bool Equals(JavaScriptSafeHandle<T> other)
        {
            if (ReferenceEquals(default(JavaScriptSafeHandle<T>), other))
            {
                return false;
            }

            return handle == other.handle;
        }

        /// <summary>
        ///     Checks for equality between contexts.
        /// </summary>
        /// <param name="obj">The other context to compare.</param>
        /// <returns>Whether the two contexts are the same.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(default(JavaScriptSafeHandle<T>), obj))
            {
                return false;
            }

            return obj is JavaScriptSafeHandle<T> && Equals((JavaScriptSafeHandle<T>)obj);
        }
        #endregion

        /// <summary>
        /// Method that is executed when the runtime collects the handle.
        /// </summary>
        /// <remarks>
        /// This prevents .net from attempting to call .JsRelease on a handle that's already deallocated.
        /// </remarks>
        /// <param name="handle"></param>
        /// <param name=""></param>
        public void ObjectBeforeCollectCallback(IntPtr handle, IntPtr callbackState)
        {
            if (Equals(handle, this.handle))
            {
                m_objectHasBeenCollected = true;
                SetHandleAsInvalid();
            }
        }

        /// <summary>
        ///     The hash code.
        /// </summary>
        /// <returns>The hash code of the context.</returns>
        public override int GetHashCode()
        {
            return handle.GetHashCode();
        }
    }
}
