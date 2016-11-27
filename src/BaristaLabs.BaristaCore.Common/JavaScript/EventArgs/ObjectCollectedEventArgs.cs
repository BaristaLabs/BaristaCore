namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    public class ObjectBeforeCollectEventArgs : EventArgs
    {
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
