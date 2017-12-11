namespace BaristaLabs.BaristaCore
{
    using System;

    /// <summary>
    /// Represents a service that manages Barista Context instances.
    /// </summary>
    public interface IBaristaContextService : IDisposable
    {
        /// <summary>
        /// Returns a new context associated with the specified runtime.
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        BaristaContext CreateContext(BaristaRuntime runtime);
    }
}