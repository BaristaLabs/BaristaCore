namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    public interface IBaristaRuntimeFactory : IDisposable
    {
        BaristaRuntime CreateRuntime(JavaScriptRuntimeAttributes attributes = JavaScriptRuntimeAttributes.None);
    }
}