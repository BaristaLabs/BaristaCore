namespace BaristaLabs.BaristaCore.JavaScript
{
    /// <summary>
    /// Represents a JavaScript Weak Reference. Weak References are not released if invalid.
    /// </summary>
    public sealed class JavaScriptWeakReferenceSafeHandle : JavaScriptReference<JavaScriptRuntimeSafeHandle>
    {
        protected override void Dispose(bool disposing)
        {
            //Do not call the base implementation as we have no references to free.
            //base.Dispose(disposing);
        }

        /// <summary>
        /// Gets an invalid weak reference.
        /// </summary>
        public static readonly JavaScriptWeakReferenceSafeHandle Invalid = new JavaScriptWeakReferenceSafeHandle();
    }
}
