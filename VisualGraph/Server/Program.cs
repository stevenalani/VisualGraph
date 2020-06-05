﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VisualGraph.Server.Providers;

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
                    webBuilder.UseStartup<Startup>();
                });
    }
}
