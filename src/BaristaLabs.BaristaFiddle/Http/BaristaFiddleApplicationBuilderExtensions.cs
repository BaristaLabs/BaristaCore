namespace BaristaLabs.BaristaFiddle.Http
{
    using Microsoft.AspNetCore.Builder;

    public static class BaristaFiddleApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBaristaFiddle(this IApplicationBuilder app)
        {
            return app;
        }
    }
}
