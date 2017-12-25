namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class ChakraCoreFactory : IJavaScriptEngineFactory
    {
        public IJavaScriptEngine CreateJavaScriptEngine()
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
