namespace BaristaLabs.BaristaCore.JavaScript.Interop
{
    using Interfaces;

    using System;

    /// <summary>
    /// Represents a Platform-Specific ChakraApi Factory.
    /// </summary>
    internal sealed partial class ChakraApi
    {
        private static readonly Lazy<IChakraApi> SharedInstance = new Lazy<IChakraApi>(Load);

        private ChakraApi()
        {
        }

        public static IChakraApi Instance
        {
            get
            {
                return SharedInstance.Value;
            }
        }

        private static IChakraApi Load()
        {
            if (PlatformApis.IsWindows)
            {
                return new ChakraApi_Windows();
            }
            else if (PlatformApis.IsDarwin)
            {
                return new ChakraApi_Darwin();
            }
            else if (PlatformApis.IsLinux)
            {
                return new ChakraApi_Linux();
            }

            throw new PlatformNotSupportedException("ChakraCore is currently only supported on Windows, macOS and Linux.");
        }
    }
}
