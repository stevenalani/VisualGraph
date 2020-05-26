using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
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
            builder.Services.AddSingleton(sp => {
                HttpClient httpClient = new HttpClient()
                {
                    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
                };

                return httpClient;
            });
            //builder.Configuration.JsonSerializerOptions.Converters.Add(new Vector2Converter());
            builder.Services.AddBlazoredToast();
            builder.Services.AddSingleton<IGraphService, GraphService>();
            
            var host = builder.Build();
            await host.RunAsync();
        }
    }
}
