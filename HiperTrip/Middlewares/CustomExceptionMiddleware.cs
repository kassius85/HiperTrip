using Entities.Enums;
using Interfaces.Contracts;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace HiperTrip.Middlewares
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private IResultService _resultService;

        public CustomExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IResultService resultService)
        {
            _resultService = resultService;

            try
            {
                await _next(httpContext).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex).ConfigureAwait(true);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            _resultService.AddValue(Resultado.Error, exception.Message + " " + exception.GetType());

            return context.Response.WriteAsync(_resultService.GetJsonProperties());
        }
    }
}