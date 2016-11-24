namespace BaristaLabs.BaristaCore.JavaScript.Internal
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
    public class JavaScriptPropertyIdSafeHandle : JavaScriptSafeHandle<JavaScriptPropertyIdSafeHandle>
    {
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
                if (m_objectHasBeenCollected)
                    throw new ObjectDisposedException("The underlying handle that represents the Property Id has been collected.");

                byte[] buffer = new byte[144];
                UIntPtr bufferLength;
                Errors.ThrowIfError(LibChakraCore.JsCopyPropertyIdUtf8(this, buffer, new UIntPtr((uint)buffer.Length), out bufferLength));
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
            Errors.ThrowIfError(LibChakraCore.JsCreatePropertyIdUtf8(name, new UIntPtr((uint)name.Length), out id));
            return id;
        }
        
        /// <summary>
        ///     Converts the property ID to a string.
        /// </summary>
        /// <returns>The name of the property ID.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Gets an invalid Property Id.
        /// </summary>
        public static readonly JavaScriptPropertyIdSafeHandle Invalid = new JavaScriptPropertyIdSafeHandle();
    }
}
