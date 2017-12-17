namespace BaristaLabs.BaristaCore
{
    using System;

    public class BaristaObjectBeforeCollectEventArgs : EventArgs
    {
        public BaristaObjectBeforeCollectEventArgs(IntPtr handle, IntPtr callbackState)
        {
            Handle = handle;
            CallbackState = callbackState;
        }

        public IntPtr Handle
        {
            get;
            set;
        }

        public IntPtr CallbackState
        {
            get;
            set;
        }
    }
}
