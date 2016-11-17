namespace BaristaLabs.BaristaCore.Http
{
    using Microsoft.AspNetCore.Mvc.Formatters;
    using System.Collections.Generic;

    public class BaristaOptions
    {
        public BaristaOptions()
        {
            OutputFormatters = new FormatterCollection<IOutputFormatter>();
            SourceFileProviders = new List<ISourceFileProvider>();
        }

        /// <summary>
        /// Gets a list of <see cref="IOutputFormatter"/>s that are used by this application.
        /// </summary>
        public FormatterCollection<IOutputFormatter> OutputFormatters
        {
            get;
        }

        /// <summary>
        /// Gets a list of <see cref="IFileProvider"/>s that are used by this application.
        /// </summary>
        public IList<ISourceFileProvider> SourceFileProviders
        {
            get;
        }

    }
}
