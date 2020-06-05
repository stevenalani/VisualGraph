using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using VisualGraph.Server.Services;
using VisualGraph.Server.Shared;

namespace VisualGraph.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration,IWebHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;
            httpsPort = configuration.GetValue<int>("HTTPS_PORT");

        }

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;
        private readonly int httpPort;
        private readonly int httpsPort;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(o => {
                o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, configureOptions => {
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
            });
            services.AddBlazoredToast();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddControllersWithViews().AddControllersAsServices();
            //services.AddScoped<AuthenticationStateProvider, AuthenticationStateProviderService>();
            VGAppSettings.BaseAddress =  $"https://localhost:{httpsPort}/api/";
            VGAppSettings.RemoteRequestProxy = Configuration["Hosting:RemoteRequestProxy"];
            VGAppSettings.InitFromConfiguration(Configuration);
            services.AddHttpClient("api", options => { 
                options.BaseAddress = new Uri(VGAppSettings.BaseAddress);
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
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("account/{path}","/_Host");
                endpoints.MapFallbackToFile("index.html");
               
            });
        }
    }
}
