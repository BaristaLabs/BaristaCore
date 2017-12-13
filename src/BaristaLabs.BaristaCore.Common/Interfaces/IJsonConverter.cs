namespace BaristaLabs.BaristaCore
{
    /// <summary>
    /// Represents an object that can convert to and from JSON natively.
    /// </summary>
    public interface IJsonConverter
    {
        /// <summary>
        /// Converts the specified object into a JSON formatted string.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string Stringify(object obj);

        /// <summary>
        /// Parses the specified JSON formatted string into an object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        object Parse(string json);
    }
}
