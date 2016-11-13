namespace BaristaLabs.BaristaCore.JavaScript.Interop.SafeHandles
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    ///     A property identifier.
    /// </summary>
    /// <remarks>
    ///     Property identifiers are used to refer to properties of JavaScript objects instead of using
    ///     strings.
    /// </remarks>
    internal class JavaScriptPropertyIdSafeHandle : SafeHandle, IEquatable<JavaScriptPropertyIdSafeHandle>
    {
        public JavaScriptPropertyIdSafeHandle() :
            base(IntPtr.Zero, ownsHandle: true)
        {
        }

        public JavaScriptPropertyIdSafeHandle(IntPtr handle) :
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
            var error = ChakraApi.Instance.JsRelease(handle, out count);
            Debug.Assert(error == JsErrorCode.JsNoError);
            return true;
        }

        /// <summary>
        ///     Gets an invalid ID.
        /// </summary>
        public static JavaScriptPropertyIdSafeHandle Invalid
        {
            get { return new JavaScriptPropertyIdSafeHandle(IntPtr.Zero); }
        }

        /// <summary>
        ///     Gets the name associated with the property ID.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     Requires an active script context.
        ///     </para>
        /// </remarks>
        public string Name
        {
            get
            {
                byte[] buffer = new byte[144];
                UIntPtr bufferLength;
                Errors.ThrowIfIs(ChakraApi.Instance.JsCopyPropertyIdUtf8(this, buffer, new UIntPtr((uint)buffer.Length), out bufferLength));
                return Encoding.UTF8.GetString(buffer, 0, (int)bufferLength.ToUInt32());
            }
        }

        /// <summary>
        ///     Gets the property ID associated with the name. 
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     Property IDs are specific to a context and cannot be used across contexts.
        ///     </para>
        ///     <para>
        ///     Requires an active script context.
        ///     </para>
        /// </remarks>
        /// <param name="name">
        ///     The name of the property ID to get or create. The name may consist of only digits.
        /// </param>
        /// <returns>The property ID in this runtime for the given name.</returns>
        public static JavaScriptPropertyIdSafeHandle FromString(string name)
        {
            JavaScriptPropertyIdSafeHandle id;
            var nameUtf8 = Encoding.UTF8.GetBytes(name);
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreatePropertyIdUtf8(nameUtf8, new UIntPtr((uint)nameUtf8.Length), out id));
            return id;
        }

        /// <summary>
        ///     The equality operator for property IDs.
        /// </summary>
        /// <param name="left">The first property ID to compare.</param>
        /// <param name="right">The second property ID to compare.</param>
        /// <returns>Whether the two property IDs are the same.</returns>
        public static bool operator ==(JavaScriptPropertyIdSafeHandle left, JavaScriptPropertyIdSafeHandle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     The inequality operator for property IDs.
        /// </summary>
        /// <param name="left">The first property ID to compare.</param>
        /// <param name="right">The second property ID to compare.</param>
        /// <returns>Whether the two property IDs are not the same.</returns>
        public static bool operator !=(JavaScriptPropertyIdSafeHandle left, JavaScriptPropertyIdSafeHandle right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        ///     Checks for equality between property IDs.
        /// </summary>
        /// <param name="other">The other property ID to compare.</param>
        /// <returns>Whether the two property IDs are the same.</returns>
        public bool Equals(JavaScriptPropertyIdSafeHandle other)
        {
            return handle == other.handle;
        }

        /// <summary>
        ///     Checks for equality between property IDs.
        /// </summary>
        /// <param name="obj">The other property ID to compare.</param>
        /// <returns>Whether the two property IDs are the same.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is JavaScriptPropertyIdSafeHandle && Equals((JavaScriptPropertyIdSafeHandle)obj);
        }

        /// <summary>
        ///     The hash code.
        /// </summary>
        /// <returns>The hash code of the property ID.</returns>
        public override int GetHashCode()
        {
            return handle.GetHashCode();
        }

        /// <summary>
        ///     Converts the property ID to a string.
        /// </summary>
        /// <returns>The name of the property ID.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
