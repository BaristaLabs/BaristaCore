namespace BaristaLabs.BaristaCore
{
    using System;

    /// <summary>
    /// Represents a factory that creates Barista Context instances.
    /// </summary>
    public interface IBaristaContextFactory : IDisposable
    {
        /// <summary>
        /// Returns a new context associated with the specified runtime.
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        BaristaContext CreateContext(BaristaRuntime runtime);
    }
}