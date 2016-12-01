namespace BaristaLabs.BaristaCore.Http
{
    using Microsoft.AspNetCore.Mvc.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;

    public static class BaristaServiceCollectionExtensions
    {
        public static IServiceCollection AddBarista(this IServiceCollection services)
        {
            // Options
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<BaristaOptions>, BaristaCoreBaristaOptionsSetup>());

            services.TryAddSingleton<IHttpResponseStreamWriterFactory, MemoryPoolHttpResponseStreamWriterFactory>();
            services.TryAddSingleton<JavaScriptValueResultExecutor>();

            return services;
        }
    }
}
