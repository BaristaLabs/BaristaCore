namespace BaristaLabs.BaristaCore
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a module that returns a script that will be parsed when imported.
    /// </summary>
    public class BaristaScriptModule : IBaristaScriptModule
    {
        /// <summary>
        /// Gets or sets the name of the script module.
        /// </summary>
        public virtual string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the description of the script module.
        /// </summary>
        public virtual string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the script that will be executed.
        /// </summary>
        public virtual string Script
        {
            get;
            set;
        }

        public virtual Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return Task.FromResult<object>(Script);
        }
    }
}
