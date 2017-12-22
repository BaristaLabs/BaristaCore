namespace BaristaLabs.BaristaCore
{
    using System;

    /// <summary>
    /// Represents an object that builds a JavaScript prototype from a .Net Type.
    /// </summary>
    public interface IBaristaTypeConversionStrategy
    {
        bool TryCreatePrototypeFunction(Type typeToConvert, out JsFunction value);
    }
}
