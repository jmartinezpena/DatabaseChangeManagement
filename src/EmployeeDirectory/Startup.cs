﻿namespace EmployeeDirectory
{
    using System.Reflection;
    using AutoMapper;
    using FluentValidation.AspNetCore;
    using Infrastructure;
    using MediatR;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Linq;
    using System.Net.Mime;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Newtonsoft.Json;
    using Model;
    using Serilog;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    options.Filters.Add(new AuthorizeFilter(policy));
                    options.Filters.Add<UnitOfWork>();
                    options.Filters.Add<UserContextFilter>();
                })
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>())
                .AddFeatureFolders();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ILoginService, LoginService>();
            services.AddScoped<UserContext>();
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.SameSite = SameSiteMode.Strict;
                });

            var connectionString = Configuration["Database:ConnectionString"];
            services.AddDbContext<DirectoryContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseSqlServer(connectionString);
            });
            services.AddTransient<HealthChecks.ISqlServerHealthCheck>(s => new HealthChecks.SqlServerHealthCheck(connectionString));

            var redisConfiguration = Configuration["Redis:Configuration"];
            services.AddTransient<HealthChecks.IRedisHealthCheck>(s => new HealthChecks.RedisHealthCheck(redisConfiguration));

            services.AddHealthChecks()
                .AddCheck("Self", () => HealthCheckResult.Healthy())
                .AddCheck<HealthChecks.ISqlServerHealthCheck>("SqlServer")
                .AddCheck<HealthChecks.IRedisHealthCheck>("Redis");

            var assembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(assembly);
            services.AddAutoMapper(assembly);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMapper mapper)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Home/Error");

            app.UseStaticFiles();

            app.UseHealthChecks("/health", GetHealthCheckOptions());

            app.UseHealthChecks("/liveness", GetHealthCheckLivenessOptions());

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();

            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        private static HealthCheckOptions GetHealthCheckOptions()
        {
            var options = new HealthCheckOptions
            {
                AllowCachingResponses = false,
                ResponseWriter = async (ctx, rpt) =>
                {
                    var result = JsonConvert.SerializeObject(
                        new
                        {
                            status = rpt.Status.ToString(),
                            checks = rpt.Entries.Select(e => new
                            {
                                key = e.Key, value = Enum.GetName(typeof(HealthStatus), e.Value.Status)
                            })
                        }, Formatting.None,
                        new JsonSerializerSettings() {NullValueHandling = NullValueHandling.Ignore});
                    ctx.Response.ContentType = MediaTypeNames.Application.Json;
                    await ctx.Response.WriteAsync(result);
                }
            };

            return options;
        }

        private static HealthCheckOptions GetHealthCheckLivenessOptions()
        {
            var options = new HealthCheckOptions
            {
                AllowCachingResponses = false,
                Predicate = r => r.Name.Contains("Self")
            };

            return options;
        }
    }
}
