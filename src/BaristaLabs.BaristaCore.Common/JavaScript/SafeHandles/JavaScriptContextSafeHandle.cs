namespace BaristaLabs.BaristaCore.JavaScript
{
    public class JavaScriptContextSafeHandle : JavaScriptReference<JavaScriptContextSafeHandle>
    {
        /// <summary>
        /// Gets an invalid context.
        /// </summary>
        public static readonly JavaScriptContextSafeHandle Invalid = new JavaScriptContextSafeHandle();
    }
}
