namespace BaristaLabs.BaristaCore.JavaScript
{
    /// <summary>
    /// Represents a handle to a JavaScript PropertyId
    /// </summary>
    public sealed class JavaScriptPropertyIdSafeHandle : JavaScriptReference<JavaScriptPropertyIdSafeHandle>
    {
        /// <summary>
        /// Gets an invalid Property Id.
        /// </summary>
        public static readonly JavaScriptPropertyIdSafeHandle Invalid = new JavaScriptPropertyIdSafeHandle();
    }
}
