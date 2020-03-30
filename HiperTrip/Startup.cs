using AutoMapper;
using HiperTrip.Extensions;
using HiperTrip.Filters;
using HiperTrip.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace HiperTrip
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Disable Automatic Model State Validation
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // Configurar Inyección de dependencias (Dependency Injection) para el contexto de BD.
            services.AddDbContext<DbHiperTripContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionString")));

            // Configurar Custom Action Filters
            services.ConfigureCustomActionFilters();

            // Añadir CORS.
            services.AddCors();

            // Añadir controladores
            services.AddControllers();

            // Configurar MVC
            services.AddMvc(config =>
            {
                config.Filters.Add<ModifyResponseFilter>(); // Registrar Action Filter de forma global.
            }).SetCompatibilityVersion(CompatibilityVersion.Latest)
              .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(30); // En ambiente productivo una vez comprobado que no da problemas se debe definir a unos 365 días.
                // options.ExcludedHosts.Add("www.example.com"); // Excluir una url.
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                options.HttpsPort = 443;
            });

            // Configurar AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Configurar Settings.
            services.ConfigureSettings(Configuration, out IConfigurationSection appSettingsSection);

            // Configurar autenticación JWT.
            services.ConfigureJWTAuthentication(appSettingsSection);

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

            // Configurar Inyección de dependencias (Dependency Injection) para los servicios.
            services.ConfigureCustomServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Manejo de errores gloabal.
            app.ConfigureCustomExceptionMiddleware();

            // global cors policy
            app.UseCors(x => x.AllowAnyMethod()
                              .AllowAnyHeader()
                              .WithOrigins("")
                              .AllowCredentials());

            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // Validaciones de usuario según token.
            app.ConfigureCustomAuthorizationMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}