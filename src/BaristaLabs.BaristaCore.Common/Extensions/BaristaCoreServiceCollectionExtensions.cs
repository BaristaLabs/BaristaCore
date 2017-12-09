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
        public static IServiceCollection AddBaristaCore(this IServiceCollection services, IBaristaModuleService moduleService = null)
        {
            IJavaScriptEngine chakraEngine = JavaScriptEngineFactory.CreateChakraEngine();
            if (moduleService == null)
                moduleService = new InMemoryModuleService();

            services.AddSingleton(chakraEngine);
            services.AddSingleton(moduleService);

            services.AddTransient<IBaristaValueFactory, BaristaValueFactory>();
            services.AddTransient<IBaristaContextFactory, BaristaContextFactory>();
            services.AddTransient<IPromiseTaskQueue, PromiseTaskQueue>();
            services.AddSingleton<IBaristaRuntimeFactory, BaristaRuntimeFactory>();

            return services;
        }

        /// <summary>
        /// Removes and disposes of BaristaCore services within the provider.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection FreeBaristaCoreServices(this IServiceCollection services)
        {
            var runtimeFactoryDescriptors = new List<ServiceDescriptor>();
            var chakraEngineDescriptors = new List<ServiceDescriptor>();

            foreach (var sd in services)
            {
                if (sd.ImplementationInstance is BaristaRuntimeFactory)
                    runtimeFactoryDescriptors.Add(sd);

                if (sd.ImplementationInstance is IJavaScriptEngine)
                    chakraEngineDescriptors.Add(sd);
            }

            foreach(var sd in runtimeFactoryDescriptors)
            {
                var runtimeFactory = sd.ImplementationInstance as BaristaRuntimeFactory;

                services.Remove(sd);
                runtimeFactory.Dispose();
                runtimeFactory = null;
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
