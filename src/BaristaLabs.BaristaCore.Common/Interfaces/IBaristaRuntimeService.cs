namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    public interface IBaristaRuntimeService : IDisposable
    {
        /// <summary>
        /// Gets the runtimes currently being managed by the service.
        /// </summary>
        int Count
        {
            get;
        }

        BaristaRuntime CreateRuntime(JavaScriptRuntimeAttributes attributes = JavaScriptRuntimeAttributes.None);
    }
}