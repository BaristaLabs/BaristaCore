namespace BaristaLabs.BaristaCore
{
    using System;

    public interface IBaristaContextService : IDisposable
    {
        BaristaContext CreateContext(BaristaRuntime runtime);
    }
}