namespace BaristaLabs.BaristaCore.Http
{
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.Extensions.Options;

    public class BaristaCoreBaristaOptionsSetup : IConfigureOptions<BaristaOptions>
    {
        public void Configure(BaristaOptions options)
        {
            //TODO: Change these to our own formatters

            // Set up default output formatters.
            options.OutputFormatters.Add(new HttpNoContentOutputFormatter());
            options.OutputFormatters.Add(new StringOutputFormatter());
            options.OutputFormatters.Add(new StreamOutputFormatter());
        }
    }
}
