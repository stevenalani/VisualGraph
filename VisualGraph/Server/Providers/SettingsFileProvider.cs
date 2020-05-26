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
            var settingsfile = user != "" ? Path.Combine(Path.Combine(_settingsdir,user) , "style.xml"): Path.Combine(_settingsdir, "default.xml");
            TextReader sr = new StreamReader(settingsfile);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(GraphStyleParametersPOCO));
            GraphStyleParametersPOCO styleParametersPOCO = (GraphStyleParametersPOCO)xmlSerializer.Deserialize(sr);
            sr.Close();
            return Task.FromResult(styleParametersPOCO);
        }
    }
}
