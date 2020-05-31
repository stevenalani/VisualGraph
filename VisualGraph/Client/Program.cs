using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Blazored.Toast;
using VisualGraph.Client.Services;
using System.Net.Http;

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
            builder.Services.AddSingleton<AuthenticationStateProvider,AuthenticationStateService>();

            builder.Services.AddBlazoredToast();
            builder.Services.AddSingleton<IGraphService, GraphService>();
            builder.Services.AddSingleton<IAccountService, AccountService>();
            builder.Services.AddAuthorizationCore();

            var host = builder.Build();
            await host.RunAsync();
        }
    }
}
