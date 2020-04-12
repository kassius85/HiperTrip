using HiperTrip.Extensions;
using HiperTrip.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace HiperTrip.Middlewares
{
    public class CustomResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private IResultService _resultService;

        public CustomResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IResultService resultService)
        {
            _resultService = resultService;

            await _next(httpContext).ConfigureAwait(true);

            if (!httpContext.IsNull())
            {
                if (!await FormatResult(httpContext).ConfigureAwait(true))
                {
                    return;
                }
            }
        }

        private async Task<bool> FormatResult(HttpContext context)
        {
            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized && context.Response.ContentType.IsNull())
            {
                context.Response.ContentType = "application/json";
                _resultService.AddValue(false, "Acceso al sistema no autorizado.");

                await context.Response.WriteAsync(_resultService.GetJsonProperties()).ConfigureAwait(true);

                return false;
            }

            return true;
        }
    }
}