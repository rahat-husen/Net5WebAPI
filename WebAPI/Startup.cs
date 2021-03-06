using AspNetCoreRateLimit;
using Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Net5WebAPI.Extensions;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.ActionFilters;
using WebAPI.Extensions;

namespace WebAPI
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
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory() + "/nlog.config"));
            services.ConfigureCors();
            services.ConfigureIIsIntegration();
            services.ConfigureLoggerService();
            services.ConfigureSqlContext(Configuration);
            services.ConfigureRepositoryManager();
            services.ConfigureVersioning();
            services.AddAutoMapper(typeof(Startup));
            services.AddControllers(config => {
                config.RespectBrowserAcceptHeader = true;
                config.ReturnHttpNotAcceptable = true;
                config.CacheProfiles.Add("120SecDuration", new CacheProfile { Duration = 120 });
                }).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
            services.AddCustomMediaTypes();
            services.AddScoped<ValidationFilterAttribute>();
            //services.ConfigureResponseCaching();
            //services.ConfigureHttpCacheHeaders();
            //services.AddMemoryCache();
            //services.ConfigureRateLimitingOptions();
            services.AddHttpContextAccessor();
            services.AddAuthentication();
            services.ConfigureIdentity();
            services.ConfigureJWT(Configuration);
            services.AddScoped<IAuthenticationManager, AuthenticationManager>();
            services.ConfigureRedisCache();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPI", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Place to add JWT with Bearer",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                       new OpenApiSecurityScheme
                       {
                           Reference=new OpenApiReference
                           {
                               Type=ReferenceType.SecurityScheme,
                               Id="Bearer"
                           },
                           Name="Bearer",
                       },
                       new List<string>()
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILoggerManager logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI v1"));
            }
            else
            {
                app.UseHsts();
            }
            app.ConfigureExceptionHandler(logger);
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });
            //app.UseResponseCaching();
            //app.UseHttpCacheHeaders();
            //app.UseIpRateLimiting();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
