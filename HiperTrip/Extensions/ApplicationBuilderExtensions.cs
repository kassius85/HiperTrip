using HiperTrip.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace HiperTrip.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomExceptionMiddleware>();
        }

        public static void UseCustomAuthorizationMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomAuthorizationMiddleware>();
        }

        public static void UseCustomResponseMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomResponseMiddleware>();
        }
    }
}