namespace BaristaLabs.BaristaCore
{
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
        /// <param name="context"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool TryFromObject(BaristaContext context, object obj, out JsValue value);

        /// <summary>
        /// Converts the specified value into a .Net object.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryToObject(BaristaContext context, JsValue value, out object obj);
    }
}
