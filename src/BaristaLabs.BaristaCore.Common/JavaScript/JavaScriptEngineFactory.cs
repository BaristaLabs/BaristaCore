namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    public static class JavaScriptEngineFactory
    {
        public static IJavaScriptEngine CreateChakraEngine()
        {
            if (PlatformApis.IsWindows)
            {
                return new WindowsChakraEngine();
            }
            else if (PlatformApis.IsDarwin || PlatformApis.IsLinux)
            {
                return new LinuxChakraEngine();
            }

            throw new PlatformNotSupportedException("ChakraCore is currently only supported on Windows, macOS and Linux.");
        }
    }
}
