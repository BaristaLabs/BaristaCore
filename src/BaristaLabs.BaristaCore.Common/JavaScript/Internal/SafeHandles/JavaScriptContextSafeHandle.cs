namespace BaristaLabs.BaristaCore.JavaScript.Internal
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public class JavaScriptContextSafeHandle : SafeHandle, IEquatable<JavaScriptContextSafeHandle>
    {
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

        public JavaScriptContextSafeHandle() :
            base(IntPtr.Zero, ownsHandle: true)
        {
        }

        /// <summary>
        ///     Releases resources associated with the context.
        /// </summary>
        protected override bool ReleaseHandle()
        {
            if (IsInvalid)
                return false;

            uint count;
            var error = LibChakraCore.JsRelease(handle, out count);

            Debug.Assert(error == JavaScriptErrorCode.NoError);
            if (count == 0)
            {
                handle = IntPtr.Zero;
                return true;
            }

            return false;
        }

        #region Equality
        /// <summary>
        ///     The equality operator for contexts.
        /// </summary>
        /// <param name="left">The first context to compare.</param>
        /// <param name="right">The second context to compare.</param>
        /// <returns>Whether the two contexts are the same.</returns>
        public static bool operator ==(JavaScriptContextSafeHandle left, JavaScriptContextSafeHandle right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, default(JavaScriptContextSafeHandle)))
            {
                return false;
            }

            if (ReferenceEquals(right, default(JavaScriptContextSafeHandle)))
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
        public static bool operator !=(JavaScriptContextSafeHandle left, JavaScriptContextSafeHandle right)
        {
            if (ReferenceEquals(left, right))
            {
                return false;
            }

            if (ReferenceEquals(left, default(JavaScriptContextSafeHandle)))
            {
                return true;
            }

            if (ReferenceEquals(right, default(JavaScriptContextSafeHandle)))
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
        public bool Equals(JavaScriptContextSafeHandle other)
        {
            if (ReferenceEquals(default(JavaScriptContextSafeHandle), other))
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
            if (ReferenceEquals(default(JavaScriptContextSafeHandle), obj))
            {
                return false;
            }

            return obj is JavaScriptContextSafeHandle && Equals((JavaScriptPropertyIdSafeHandle)obj);
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

        /// <summary>
        /// Gets an invalid context.
        /// </summary>
        public static readonly JavaScriptContextSafeHandle Invalid = new JavaScriptContextSafeHandle();
    }
}
