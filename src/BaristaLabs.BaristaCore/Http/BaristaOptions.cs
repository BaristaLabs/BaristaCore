namespace BaristaLabs.BaristaCore.Http
{
    using Microsoft.AspNetCore.Mvc.Formatters;

    public class BaristaOptions
    {
        public BaristaOptions()
        {
            OutputFormatters = new FormatterCollection<IOutputFormatter>();
        }

        /// <summary>
        /// Gets a list of <see cref="IOutputFormatter"/>s that are used by this application.
        /// </summary>
        public FormatterCollection<IOutputFormatter> OutputFormatters
        {
            get;
        }
    }
}
