namespace BaristaLabs.BaristaCore
{
    using System;

    public interface IBaristaContextFactory : IDisposable
    {
        BaristaContext CreateContext(BaristaRuntime runtime);
    }
}