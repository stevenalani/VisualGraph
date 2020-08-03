using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using VisualGraph.Server.Providers;
using VisualGraph.Server.Shared;

namespace VisualGraph.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            GraphFileProvider.EnsureGraphDirExists();
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var cfg = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();
                    VGAppSettings.InitFromConfiguration(cfg);
                    webBuilder.UseStartup<Startup>();
                });
    }
}
