namespace BaristaLabs.BaristaCore.Extensions
{
    using JavaScript;
    using Microsoft.Extensions.DependencyInjection;

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
        public static IServiceCollection AddBaristaCore(this IServiceCollection services)
        {
            IJavaScriptEngine chakraEngine = JavaScriptEngineFactory.CreateChakraEngine();

            services.AddSingleton(chakraEngine);
            services.AddSingleton(new JavaScriptRuntime.JavaScriptRuntimeObserver(chakraEngine));

            return services;
        }
    }
}
