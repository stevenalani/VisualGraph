using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VisualGraph.Shared.Models;

namespace VisualGraph.Server.Providers
{
    public static class SettingsFileProvider
    {

        private static string _settingsdir = Path.GetFullPath("./Settings");        
        public static Task<GraphStyleParametersPOCO> GetGraphStyleParameters(string user = "") {
            var settingsDir = user != "" ? Path.Combine(_settingsdir,user): _settingsdir;
            string settingsfile = Path.Combine(_settingsdir, "default.xml");
            if (Directory.Exists(settingsDir))
            {
                Path.Combine(_settingsdir, "style.xml");
            }
            TextReader sr = new StreamReader(settingsfile);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(GraphStyleParametersPOCO));
            GraphStyleParametersPOCO styleParametersPOCO = (GraphStyleParametersPOCO)xmlSerializer.Deserialize(sr);
            sr.Close();
            return Task.FromResult(styleParametersPOCO);
        }
        
    }
}
