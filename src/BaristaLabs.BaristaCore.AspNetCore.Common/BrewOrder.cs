namespace BaristaLabs.BaristaCore.AspNetCore
{
    using BaristaLabs.BaristaCore.ModuleLoaders;

    public class BrewOrder
    {
        public BrewOrder()
        {
            Language = ResourceKind.JavaScript;
        }

        /// <summary>
        /// Gets or sets the base url that will be used for web resource module loaders.
        /// </summary>
        public string BaseUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path associated with the request.
        /// </summary>
        public string Path
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the code that will be executed
        /// </summary>
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the language of the code.
        /// </summary>
        public ResourceKind Language
        {
            get;
            set;
        }

        public bool IsCodeSet
        {
            get;
            set;
        }
    }
}
