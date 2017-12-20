namespace BaristaLabs.BaristaCore.Extensions
{
    using JavaScript;
    using Microsoft.Extensions.DependencyInjection;
    using System.Collections.Generic;

    /// <summary>
    /// Provides IServiceCollection Extension Methods
    /// </summary>
    public static class BaristaCoreServiceCollectionExtensions
    {
        /// <summary>
        /// Register BaristaCore services with the specified ServiceCollection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBaristaCore(this IServiceCollection services, IBaristaModuleLoader moduleService = null)
        {
            if (moduleService == null)
                moduleService = new InMemoryModuleLoader();

            services.AddSingleton<IJavaScriptEngineFactory, ChakraCoreFactory>();
            services.AddSingleton((provider) =>
            {
                var jsEngineFactory = provider.GetRequiredService<IJavaScriptEngineFactory>();
                return jsEngineFactory.CreateJavaScriptEngine();
            });
            services.AddSingleton(moduleService);
            services.AddSingleton<IBaristaValueFactoryBuilder, BaristaValueFactoryBuilder>();
            services.AddSingleton<IBaristaRuntimeFactory, BaristaRuntimeFactory>();
            
            services.AddTransient<IBaristaContextFactory, BaristaContextFactory>();
            services.AddTransient<IBaristaModuleRecordFactory, BaristaModuleRecordFactory>();
            services.AddTransient<IBaristaConversionStrategy, BaristaConversionStrategy>();
            services.AddTransient<IPromiseTaskQueue, PromiseTaskQueue>();

            return services;
        }

        /// <summary>
        /// Removes and disposes of BaristaCore services within the provider.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection FreeBaristaCoreServices(this IServiceCollection services)
        {
            var runtimeServiceDescriptors = new List<ServiceDescriptor>();
            var chakraEngineDescriptors = new List<ServiceDescriptor>();

            foreach (var sd in services)
            {
                if (sd.ImplementationInstance is BaristaRuntimeFactory)
                    runtimeServiceDescriptors.Add(sd);

                if (sd.ImplementationInstance is IJavaScriptEngine)
                    chakraEngineDescriptors.Add(sd);
            }

            foreach(var sd in runtimeServiceDescriptors)
            {
                var runtimeService = sd.ImplementationInstance as BaristaRuntimeFactory;

                services.Remove(sd);
                runtimeService.Dispose();
                runtimeService = null;
            }

            foreach (var sd in chakraEngineDescriptors)
            {
                var javaScriptEngine = sd.ImplementationInstance as IJavaScriptEngine;

                services.Remove(sd);
            }

            return services;
        }
    }
}
