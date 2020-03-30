using HiperTrip.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace HiperTrip.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomExceptionMiddleware>();
        }

        public static void ConfigureCustomAuthorizationMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomAuthorizationMiddleware>();
        }
    }
}