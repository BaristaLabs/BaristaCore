namespace BaristaLabs.BaristaCore.AspNetCore
{
    using BaristaLabs.BaristaCore.AspNetCore.Middleware;
    using Microsoft.AspNetCore.Builder;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBaristaCore(this IApplicationBuilder app)
        {
            app.Map("/api/BaristaCore", (innerApp) =>
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
