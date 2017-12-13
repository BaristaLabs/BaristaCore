namespace BaristaLabs.BaristaCore
{
    using System;

    /// <summary>
    /// Represents an implementation of a strategy to convert .Net class members to JavaScript values and vice versa.
    /// </summary>
    /// <remarks>
    /// Used when an non object is returned by a module
    /// </remarks>
    public interface IBaristaConversionStrategy
    {
        /// <summary>
        /// Converts the specified .Net object into a JavaScript value.
        /// </summary>
        /// <param name="valueService"></param>
        /// <param name="o"></param>
        /// <param name="jsonConverter"></param>
        /// <returns></returns>
        bool TryFromObject(IBaristaValueService valueService, object obj, out JsValue value);

        /// <summary>
        /// Converts the specified value into a .Net object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryToObject(BaristaContext context, JsValue value, out object obj);
    }
}
