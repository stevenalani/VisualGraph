using Blazored.Toast;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using VisualGraph.Services;
using VisualGraph.Shared;

namespace VisualGraph
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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
            
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<IGraphService, GraphService>();
            services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
            services.AddBlazoredToast();
            VGAppSettings.BaseAddress = Configuration["Hosting:BaseAddress"]+"/";
            VGAppSettings.RemoteRequestProxy = Configuration["Hosting:RemoteRequestProxy"];
            services.AddHttpClient("api", options => options.BaseAddress = new Uri(VGAppSettings.BaseAddress));
            services.AddHttpClient("publicapi", options => options.BaseAddress = new Uri("https://visualgraph.stevenalani.info"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/customlogger/visualgraph-{Date}.txt",LogLevel.Trace);
            
        
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
