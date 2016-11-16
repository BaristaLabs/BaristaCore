namespace BaristaLabs.BaristaCore.JavaScript.Interop.SafeHandles
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    internal sealed class JavaScriptRuntimeSafeHandle : SafeHandle, IEquatable<JavaScriptRuntimeSafeHandle>
    {
        public JavaScriptRuntimeSafeHandle() :
            base(IntPtr.Zero, ownsHandle: true)
        {

        }

        public JavaScriptRuntimeSafeHandle(IntPtr handle) :
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
        
        protected override bool ReleaseHandle()
        {
            if (IsInvalid)
                return false;
            
            var error = ChakraApi.Instance.JsSetCurrentContext(JavaScriptContextSafeHandle.Invalid);
            Debug.Assert(error == JsErrorCode.JsNoError);

            error = ChakraApi.Instance.JsDisposeRuntime(handle);
            Debug.Assert(error == JsErrorCode.JsNoError);
            return true;
        }


        #region Equality
        /// <summary>
        ///     The equality operator for runtimes.
        /// </summary>
        /// <param name="left">The first runtime to compare.</param>
        /// <param name="right">The second runtime to compare.</param>
        /// <returns>Whether the two runtimes are the same.</returns>
        public static bool operator ==(JavaScriptRuntimeSafeHandle left, JavaScriptRuntimeSafeHandle right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, default(JavaScriptRuntimeSafeHandle)))
            {
                return false;
            }

            if (ReferenceEquals(right, default(JavaScriptRuntimeSafeHandle)))
            {
                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        ///     The inequality operator for runtimes.
        /// </summary>
        /// <param name="left">The first runtime to compare.</param>
        /// <param name="right">The second runtime to compare.</param>
        /// <returns>Whether the two runtimes are not the same.</returns>
        public static bool operator !=(JavaScriptRuntimeSafeHandle left, JavaScriptRuntimeSafeHandle right)
        {
            if (ReferenceEquals(left, right))
            {
                return false;
            }

            if (ReferenceEquals(left, default(JavaScriptRuntimeSafeHandle)))
            {
                return true;
            }

            if (ReferenceEquals(right, default(JavaScriptRuntimeSafeHandle)))
            {
                return true;
            }

            return !left.Equals(right);
        }

        /// <summary>
        ///     Checks for equality between runtimes.
        /// </summary>
        /// <param name="other">The other runtime to compare.</param>
        /// <returns>Whether the two runtimes are the same.</returns>
        public bool Equals(JavaScriptRuntimeSafeHandle other)
        {
            if (ReferenceEquals(default(JavaScriptRuntimeSafeHandle), other))
            {
                return false;
            }

            return handle == other.handle;
        }

        /// <summary>
        ///     Checks for equality between runtimes.
        /// </summary>
        /// <param name="obj">The other runtime to compare.</param>
        /// <returns>Whether the two runtimes are the same.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(default(JavaScriptRuntimeSafeHandle), obj))
            {
                return false;
            }

            return obj is JavaScriptRuntimeSafeHandle && Equals((JavaScriptRuntimeSafeHandle)obj);
        }
        #endregion

        /// <summary>
        ///     The hash code.
        /// </summary>
        /// <returns>The hash code of the runtime.</returns>
        public override int GetHashCode()
        {
            return handle.GetHashCode();
        }

        /// <summary>
        /// Gets an invalid runtime.
        /// </summary>
        public static readonly JavaScriptRuntimeSafeHandle Invalid = new JavaScriptRuntimeSafeHandle();
    }
}
