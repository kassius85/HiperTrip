using HiperTrip.Interfaces;
using HiperTrip.Services;
using HiperTrip.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace HiperTrip.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void ConfigureCustomActionFilters(this IServiceCollection services)
        {
            //services.AddScoped<ValidationFilterAttribute>();
            //services.AddScoped<ValidationEntityExistsFilter<Usuario>>();
            //services.AddScoped<ValidationEntityExistsFilter<ClienteSistema>>();
        }

        public static void ConfigureJWTAuthentication(this IServiceCollection services, IConfigurationSection appSettingsSection)
        {
            AppSettings appSettings = appSettingsSection.Get<AppSettings>();
            byte[] key = Encoding.ASCII.GetBytes(appSettings.JwtSecretKey);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true, // Validar expiración de Token.
                    ValidateIssuerSigningKey = true, // Validar llave secreta que se utiliza.
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero // Para que no se hagan ajustes temporales en el algoritmo que valida si expiró el Token.
                };
            });
        }

        public static void ConfigureCustomServices(this IServiceCollection services)
        {
            services.AddTransient<IEmailService, EmailService>(); // Envío de correos.
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IResultService, ResultService>();
            services.AddScoped<IParamGenUsuService, ParamGenUsuService>();
            services.AddScoped<ICambioRestringidoService, CambioRestringidoService>();
        }

        public static void ConfigureSettings(this IServiceCollection services, IConfiguration Configuration, out IConfigurationSection appSettingsSection)
        {
            // Configurar objetos de configuración fuertemente tipados.
            appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            //// Configuración para envío de correo
            IConfigurationSection emailSettingsSection = Configuration.GetSection("EmailSettings");
            services.Configure<EmailSettings>(emailSettingsSection);

            //// Configuración para permitir orígenes
            //IConfigurationSection allowedOriginsSection = Configuration.GetSection("AllowedOrigins");
            //services.Configure<OriginsSettings>(allowedOriginsSection);
        }
    }
}