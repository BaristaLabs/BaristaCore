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
        /// <summary>
        /// Gets a value that indicates if the JavaScript Reference is invalid.
        /// </summary>
        public override bool IsInvalid
        {
            get
            {
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

        /// <summary>
        /// Creates a new Invalid JavaScript Reference
        /// </summary>
        protected JavaScriptReference() :
            base(IntPtr.Zero, ownsHandle: true)
        {
        }

        /// <summary>
        /// Creates a new JavaScript Reference with the specified handle.
        /// </summary>
        /// <param name="handle"></param>
        protected JavaScriptReference(IntPtr handle) :
            this()
        {
            SetHandle(handle);
        }

        /// <summary>
        /// Releases resources associated with the context.
        /// </summary>
        protected override bool ReleaseHandle()
        {
            //This method always executes as part of the object finalization process.
            //This makes it inappropriate to use for releasing JsRefs as the
            //JsRuntime is often (but not always) already collected at the point when
            //this is run. Thus:

            //Do nothing.
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsClosed)
            {
                uint count;
                var error = LibChakraCore.JsRelease(this, out count);
                Debug.Assert(error == JavaScriptErrorCode.NoError);

                //This has no effect.
                SetHandleAsInvalid();
            }

            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Represents a base implmentation of a Chakra Engine JavaScript Reference Safe Handle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class JavaScriptReference<T> : JavaScriptReference, IEquatable<JavaScriptReference<T>> where T : JavaScriptReference<T>
    {
        public JavaScriptReference()
            : base()
        {
        }

        public JavaScriptReference(IntPtr handle)
            : base(handle)
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
