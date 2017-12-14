namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    public interface IBaristaRuntimeFactory : IDisposable
    {
        /// <summary>
        /// Gets the number runtimes currently being managed by the factory.
        /// </summary>
        int Count
        {
            get;
        }

        BaristaRuntime CreateRuntime(JavaScriptRuntimeAttributes attributes = JavaScriptRuntimeAttributes.None);
    }
}