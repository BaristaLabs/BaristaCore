namespace BaristaLabs.BaristaCore
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public static class PlatformApis
    {
        static PlatformApis()
        {
            IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            IsDarwin = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

        public static bool IsWindows
        {
            get;
        }

        public static bool IsDarwin
        {
            get;
        }

        public static bool IsLinux
        {
            get;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long VolatileRead(ref long value)
        {
            if (IntPtr.Size == 8)
            {
                return Volatile.Read(ref value);
            }
            else
            {
                //Avoid torn long reads on 32-bit
                return Interlocked.Read(ref value);
            }
        }
    }
}
