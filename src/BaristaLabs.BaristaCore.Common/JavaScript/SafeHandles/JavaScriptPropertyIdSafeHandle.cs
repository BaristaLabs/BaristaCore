namespace BaristaLabs.BaristaCore.JavaScript
{
    /// <summary>
    ///     A property identifier.
    /// </summary>
    /// <remarks>
    ///     Property identifiers are used to refer to properties of JavaScript objects instead of using
    ///     strings.
    /// </remarks>
    public class JavaScriptPropertyIdSafeHandle : JavaScriptReference<JavaScriptPropertyIdSafeHandle>
    {
        /// <summary>
        /// Gets an invalid Property Id.
        /// </summary>
        public static readonly JavaScriptPropertyIdSafeHandle Invalid = new JavaScriptPropertyIdSafeHandle();
    }
}
