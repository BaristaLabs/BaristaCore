namespace BaristaLabs.BaristaCore.JavaScript
{
    using Internal;
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a base implmentation of a Chakra Engine JavaScript Reference Safe Handle
    /// </summary>
    public abstract class JavaScriptReference : SafeHandle
    {
        private volatile bool m_objectHasBeenCollected = false;

        /// <summary>
        /// Gets a value that indicates if the object has been flagged as collected by the JavaScript Runtime.
        /// </summary>
        public bool IsCollected
        {
            get { return m_objectHasBeenCollected; }
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

        protected JavaScriptReference() :
            base(IntPtr.Zero, ownsHandle: true)
        {
        }

        protected JavaScriptReference(IntPtr handle) :
            this()
        {
            SetHandle(handle);
        }

        /// <summary>
        ///     Releases resources associated with the context.
        /// </summary>
        protected override bool ReleaseHandle()
        {
            //Do nothing.
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsCollected && !IsClosed)
            {
                uint count;
                var error = LibChakraCore.JsRelease(this, out count);
                Debug.Assert(error == JavaScriptErrorCode.NoError);
                SetHandleAsInvalid();
            }

            base.Dispose(disposing);
        }

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
    }

    /// <summary>
    /// Represents a base implmentation of a Chakra Engine JavaScript Reference Safe Handle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class JavaScriptReference<T> : JavaScriptReference, IEquatable<JavaScriptReference<T>> where T : JavaScriptReference<T>
    {
        protected JavaScriptReference() :
            base()
        {
        }

        protected JavaScriptReference(IntPtr handle) :
            base(handle)
        {
        }

        #region Equality
        /// <summary>
        ///     The equality operator for contexts.
        /// </summary>
        /// <param name="left">The first context to compare.</param>
        /// <param name="right">The second context to compare.</param>
        /// <returns>Whether the two contexts are the same.</returns>
        public static bool operator ==(JavaScriptReference<T> left, JavaScriptReference<T> right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, default(JavaScriptReference<T>)))
            {
                return false;
            }

            if (ReferenceEquals(right, default(JavaScriptReference<T>)))
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
        public static bool operator !=(JavaScriptReference<T> left, JavaScriptReference<T> right)
        {
            if (ReferenceEquals(left, right))
            {
                return false;
            }

            if (ReferenceEquals(left, default(JavaScriptReference<T>)))
            {
                return true;
            }

            if (ReferenceEquals(right, default(JavaScriptReference<T>)))
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
        public bool Equals(JavaScriptReference<T> other)
        {
            if (ReferenceEquals(default(JavaScriptReference<T>), other))
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
            if (ReferenceEquals(default(JavaScriptReference<T>), obj))
            {
                return false;
            }

            return obj is JavaScriptReference<T> && Equals((JavaScriptReference<T>)obj);
        }
        #endregion

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
