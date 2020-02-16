using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VisualGraph.Data;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (File.Exists(GraphService.settingsFile))
            {
                TextReader sr = new StreamReader(GraphService.settingsFile);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(GraphStyleParametersPOCO));
                GraphStyleParametersPOCO styleParametersPOCO = (GraphStyleParametersPOCO)xmlSerializer.Deserialize(sr);
                sr.Close();
                GraphStyleParameters.InitFromPoco(styleParametersPOCO);
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
