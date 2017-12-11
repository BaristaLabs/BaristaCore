namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    public interface IBaristaRuntimeService : IDisposable
    {
        BaristaRuntime CreateRuntime(JavaScriptRuntimeAttributes attributes = JavaScriptRuntimeAttributes.None);
    }
}