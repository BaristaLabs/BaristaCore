namespace BaristaLabs.BaristaCore.JavaScript.Internal
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public class JavaScriptValueSafeHandle : SafeHandle, IEquatable<JavaScriptValueSafeHandle>
    {

        public JavaScriptValueSafeHandle() :
            base(IntPtr.Zero, ownsHandle: true)
        {
        }

        public JavaScriptValueSafeHandle(IntPtr handle) :
            base(handle, true)
        {
        }

        public override bool IsInvalid
        {
            get
            {
                return handle == IntPtr.Zero;
            }
        }

        /// <summary>
        ///     Releases resources associated with the property ID.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     Requires an active script context.
        ///     </para>
        /// </remarks>
        protected override bool ReleaseHandle()
        {
            if (IsInvalid)
                return false;

            uint count;
            var error = LibChakraCore.JsRelease(handle, out count);
            Debug.Assert(error == JavaScriptErrorCode.NoError);

            return true;
        }

        #region Equality
        /// <summary>
        ///     The equality operator for values.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>Whether the two values are the same.</returns>
        public static bool operator ==(JavaScriptValueSafeHandle left, JavaScriptValueSafeHandle right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, default(JavaScriptValueSafeHandle)))
            {
                return false;
            }

            if (ReferenceEquals(right, default(JavaScriptValueSafeHandle)))
            {
                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        ///     The inequality operator for values.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>Whether the two values are not the same.</returns>
        public static bool operator !=(JavaScriptValueSafeHandle left, JavaScriptValueSafeHandle right)
        {
            if (ReferenceEquals(left, right))
            {
                return false;
            }

            if (ReferenceEquals(left, default(JavaScriptValueSafeHandle)))
            {
                return true;
            }

            if (ReferenceEquals(right, default(JavaScriptValueSafeHandle)))
            {
                return true;
            }

            return !left.Equals(right);
        }

        /// <summary>
        ///     Checks for equality between values.
        /// </summary>
        /// <param name="other">The other value to compare.</param>
        /// <returns>Whether the two values are the same.</returns>
        public bool Equals(JavaScriptValueSafeHandle other)
        {
            if (ReferenceEquals(default(JavaScriptValueSafeHandle), other))
            {
                return false;
            }

            return handle == other.handle;
        }

        /// <summary>
        ///     Checks for equality between values.
        /// </summary>
        /// <param name="obj">The other value to compare.</param>
        /// <returns>Whether the two values are the same.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(default(JavaScriptValueSafeHandle), obj))
            {
                return false;
            }

            return obj is JavaScriptValueSafeHandle && Equals((JavaScriptValueSafeHandle)obj);
        }
        #endregion

        /// <summary>
        ///     The hash code.
        /// </summary>
        /// <returns>The hash code of the property ID.</returns>
        public override int GetHashCode()
        {
            return handle.GetHashCode();
        }

        /// <summary>
        /// Gets an invalid value.
        /// </summary>
        public static readonly JavaScriptValueSafeHandle Invalid = new JavaScriptValueSafeHandle();
    }
}
