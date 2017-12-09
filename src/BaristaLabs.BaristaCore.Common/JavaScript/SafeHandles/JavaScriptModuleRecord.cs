namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a Chakra Engine Module Record Safe Handle
    /// </summary>
    public sealed class JavaScriptModuleRecord : SafeHandle
    {
        /// <summary>
        /// Gets a value that indicates if the Module Record is invalid.
        /// </summary>
        public override bool IsInvalid
        {
            get
            {
                return handle == IntPtr.Zero;
            }
        }

        /// <summary>
        /// Gets the native function call which created the safe handle.
        /// </summary>
        /// <remarks>
        /// Useful when debugging.
        /// </remarks>
        public string NativeFunctionSource
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new Invalid JavaScript Reference
        /// </summary>
        public JavaScriptModuleRecord() :
            base(IntPtr.Zero, ownsHandle: true)
        {
        }

        /// <summary>
        /// Creates a new JavaScript Reference with the specified handle.
        /// </summary>
        /// <param name="handle"></param>
        public JavaScriptModuleRecord(IntPtr handle) :
            this()
        {
            SetHandle(handle);
        }

        /// <summary>
        /// Releases resources associated with the context.
        /// </summary>
        protected override bool ReleaseHandle()
        {
            //This method always executes as part of the object finalization process.
            //This makes it inappropriate to use for releasing JsRefs as the
            //JsRuntime is often (but not always) already collected at the point when
            //this is run. Thus:

            //Do nothing.
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            //Do Nothing.
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets an invalid Module Record.
        /// </summary>
        public static readonly JavaScriptModuleRecord Invalid = new JavaScriptModuleRecord();
    }
}
