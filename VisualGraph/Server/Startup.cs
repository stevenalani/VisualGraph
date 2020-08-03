using Blazored.Toast;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Security.Claims;
using VisualGraph.Server.Shared;

namespace VisualGraph.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, configureOptions =>
            {
                configureOptions.Cookie.Name = "VisualGraphCookie";
                configureOptions.Cookie.Path = "/";
                configureOptions.Cookie.HttpOnly = false;
                configureOptions.Cookie.SameSite = SameSiteMode.None;
                configureOptions.SlidingExpiration = true;
                configureOptions.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                configureOptions.LoginPath = "/account/login";
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsVisualGraphMember", policyBuilder =>
                {
                    policyBuilder
                        .RequireAuthenticatedUser()
                        .RequireClaim(ClaimTypes.Name)
                        .RequireClaim(ClaimTypes.GivenName)
                        .RequireClaim(ClaimTypes.NameIdentifier)
                        .Build();
                });
                options.AddPolicy("IsVisualGraphAdmin", policyBuilder =>
                {
                    policyBuilder
                        .RequireAuthenticatedUser()
                        .RequireRole(new[] { VGAppSettings.AdminRole, VGAppSettings.SuperUserRole })
                        .Build();
                });
            });
            services.AddBlazoredToast();
            services.AddControllers().AddJsonOptions(options => {
                options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;                
            });
            services.AddRazorPages();

            services.AddHttpClient("api", options =>
            {
                options.BaseAddress = new Uri(VGAppSettings.BaseAddress + ":" + VGAppSettings.HttpsPort);
            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();

            app.Use(async (context, next) =>
            {
                var u = context.User;
                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
