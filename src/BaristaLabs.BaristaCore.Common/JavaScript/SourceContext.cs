namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    using System.Threading;

    public static class SourceContext
    {
        private static IntPtr s_sourceContextId = IntPtr.Zero;

        public static IntPtr GetNextContextId()
        {
            while (true)
            {
                IntPtr mySrcContextId = s_sourceContextId;
                IntPtr incremented = (s_sourceContextId + 1);

                Interlocked.CompareExchange(ref s_sourceContextId, incremented, mySrcContextId);
                if (s_sourceContextId == incremented)
                {
                    return mySrcContextId;
                }
            }
        }
    }
}
