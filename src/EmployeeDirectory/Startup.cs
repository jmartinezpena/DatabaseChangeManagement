namespace EmployeeDirectory
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

            var assembly = Assembly.GetExecutingAssembly();

            services.AddDbContext<DirectoryContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseSqlServer(connectionString);
            });
            services.AddMediatR(assembly);
            services.AddAutoMapper(assembly);
            Mapper.Configuration.AssertConfigurationIsValid();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Home/Error");

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }
    }
}
