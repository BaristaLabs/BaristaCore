namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    public static class JavaScriptRuntimeFactory
    {
        public static IJavaScriptRuntime CreateChakraRuntime()
        {
            if (PlatformApis.IsWindows)
            {
                return new WindowsChakraRuntime();
            }
            else if (PlatformApis.IsDarwin || PlatformApis.IsLinux)
            {
                return new LinuxChakraRuntime();
            }

            throw new PlatformNotSupportedException("ChakraCore is currently only supported on Windows, macOS and Linux.");
        }
    }
}
