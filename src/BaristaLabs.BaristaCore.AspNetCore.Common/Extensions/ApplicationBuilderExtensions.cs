namespace BaristaLabs.BaristaCore.AspNetCore
{
    using BaristaLabs.BaristaCore.AspNetCore.Middleware;
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// Extension methods for <see cref="IApplicationBuilder"/> to add BaristaCore to the request execution pipeline.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds BaristaCore to the <see cref="IApplicationBuilder"/> request execution pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="baristaEvalPath"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseBaristaCore(this IApplicationBuilder app, string baristaEvalPath = null)
        {
            if (string.IsNullOrWhiteSpace(baristaEvalPath))
                baristaEvalPath = "/api/barista";
            
            app.Map(baristaEvalPath, (innerApp) =>
            {
                innerApp.UseMiddleware<TakeOrderMiddleware>();
                innerApp.UseMiddleware<TampMiddleware>();
                innerApp.UseMiddleware<BrewMiddleware>();
                innerApp.UseMiddleware<ServeMiddleware>();
            });

            return app;
        }
    }
}
