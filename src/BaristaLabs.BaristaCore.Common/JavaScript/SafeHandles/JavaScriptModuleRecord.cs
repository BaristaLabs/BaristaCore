namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    /// <summary>
    /// Represents a Chakra Engine Module Record Safe Handle
    /// </summary>
    public sealed class JavaScriptModuleRecord : JavaScriptReference
    {
        public JavaScriptModuleRecord()
            : base()
        {
        }

        public JavaScriptModuleRecord(IntPtr ptr)
            : base(ptr)
        {
        }

        /// <summary>
        /// Gets an invalid Module Record.
        /// </summary>
        public static readonly JavaScriptModuleRecord Invalid = new JavaScriptModuleRecord();
    }
}
