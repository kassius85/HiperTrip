using HiperTrip.Extensions;
using HiperTrip.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Threading.Tasks;

namespace HiperTrip.Middlewares
{
    public class CustomAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private IResultService _resultService;

        public CustomAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IResultService resultService)
        {
            _resultService = resultService;

            if (!httpContext.IsNull())
            {
                if (!await ValidaUsuarioAsync(httpContext).ConfigureAwait(true))
                {
                    return;
                }

                if (!await ValidaTokenJti(httpContext).ConfigureAwait(true))
                {
                    return;
                }
            }

            await _next(httpContext).ConfigureAwait(true);
        }

        private async Task<bool> ValidaUsuarioAsync(HttpContext context)
        {
            string codUsuario = context.GetUniqueName();

            if (string.IsNullOrEmpty(codUsuario))
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                _resultService.AddValue(false, "La identificación contenida en el token no es válida.");

                await context.Response.WriteAsync(_resultService.GetJsonProperties()).ConfigureAwait(true);

                return false;
            }

            IUsuarioService userService = context.RequestServices.GetRequiredService<IUsuarioService>();

            // Determinar si existe el usuario.
            if (!await userService.ExisteUsuario(codUsuario).ConfigureAwait(true))
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                _resultService.AddValue(false, "El usuario con el que intenta acceder no existe en el sistema.");

                await context.Response.WriteAsync(_resultService.GetJsonProperties()).ConfigureAwait(true);

                return false;
            }

            // Realizar demás validaciones de usuario activo, usuario borrado y cantidad de conexiones.

            return true;
        }

        private async Task<bool> ValidaTokenJti(HttpContext context)
        {
            string jti = context.GetTokenClaim("jti");
            
            if (string.IsNullOrEmpty(jti))
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                _resultService.AddValue(false, "El identificador del token no es válido.");
            }

            IUsuarioService userService = context.RequestServices.GetRequiredService<IUsuarioService>();

            await Task.Run(() => 5).ConfigureAwait(true);

            // Determinar si existe el jti es correcto.
            //if (!await userService.ExisteNombreUsuario(codUsuario))
            //{
            //    context.Response.ContentType = "application/json";
            //    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //    _resultService.AddValue(false, "El usuario con el que intenta acceder no existe en el sistema.");

            //    await context.Response.WriteAsync(_resultService.GetJsonProperties());

            //    return false;
            //}

            // Realizar demás validaciones de usuario activo, usuario borrado y cantidad de conexiones.

            return true;
        }
    }
}