namespace BaristaLabs.BaristaCore.JavaScript
{
    /// <summary>
    /// Represents a handle to a JavaScript Context
    /// </summary>
    public sealed class JavaScriptContextSafeHandle : JavaScriptReference<JavaScriptContextSafeHandle>
    {
        /// <summary>
        /// Gets an invalid context.
        /// </summary>
        public static readonly JavaScriptContextSafeHandle Invalid = new JavaScriptContextSafeHandle();
    }
}
