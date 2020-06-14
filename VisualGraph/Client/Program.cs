using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using VisualGraph.Client.Services;

namespace VisualGraph.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<Client.App>("app");
            builder.Services.AddTransient(sp =>
                new HttpClient
                {
                    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
                });
            //builder.Configuration.JsonSerializerOptions.Converters.Add(new Vector2Converter());

            /*AddAuthentication(o => {
            o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            o.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
            AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, configureOptions => {
            configureOptions.Cookie.Name = "VisualGraphCookie";
            configureOptions.Cookie.Path = "/";
            configureOptions.Cookie.HttpOnly = false;
            configureOptions.Cookie.SameSite = SameSiteMode.None;
            configureOptions.SlidingExpiration = true;
            configureOptions.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            configureOptions.LoginPath = "/account/login";
        });*/
            builder.Services.AddAuthorizationCore(options =>
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
            builder.Services.AddSingleton<AuthenticationStateProvider, AuthenticationStateService>();
            builder.Services.AddBlazoredToast();
            builder.Services.AddSingleton<IGraphService, GraphService>();
            builder.Services.AddSingleton<IAccountService, AccountService>();
            var host = builder.Build();
            await host.RunAsync();
        }
    }
}
