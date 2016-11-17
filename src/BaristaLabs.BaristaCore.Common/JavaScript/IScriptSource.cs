using System.Threading.Tasks;

namespace BaristaLabs.BaristaCore.JavaScript
{
    /// <summary>
    /// Represents a source where scripts are persisted. 
    /// </summary>
    public interface IScriptSource
    {
        /// <summary>
        /// Gets an integer identifier describing the script source.
        /// </summary>
        /// <remarks>
        /// Used to identify the script source instance during debugging.
        /// </remarks>
        int Cookie
        {
            get;
        }

        /// <summary>
        /// Gets the description of the script source.
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// Returns the script from the source.
        /// </summary>
        /// <returns></returns>
        Task<string> GetScriptAsync();
    }
}
