namespace BaristaLabs.BaristaCore.JavaScript.Internal
{
    using System.Diagnostics;

    public class JavaScriptContextSafeHandle : JavaScriptSafeHandle<JavaScriptContextSafeHandle>
    {
        /// <summary>
        /// Gets an invalid context.
        /// </summary>
        public static readonly JavaScriptContextSafeHandle Invalid = new JavaScriptContextSafeHandle();
    }
}
