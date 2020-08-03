using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VisualGraph.Server.Providers;
using VisualGraph.Server.Shared;
using VisualGraph.Shared.Models;

namespace VisualGraph.Server.Controllers
{
    /// <summary>
    /// Übermittelt und Empfängt Graphen zu/von Clients
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GraphController : ControllerBase
    {

        private readonly ILogger<GraphController> logger;
        /// <summary>
        /// Erstellt Instanz der Klasse
        /// </summary>
        /// <param name="logger">Dependency Injection ASP .Net Logger</param>
        public GraphController(ILogger<GraphController> logger)
        {
            this.logger = logger;
        }
        /// <summary>
        /// Läd Graphmodel über den Namen
        /// </summary>
        /// <param name="graphname">Name des zu suchenden Graphen</param>
        /// <returns>Gefundener Graph oder null</returns>
        [HttpGet("GetGraphModel/{graphname}")]
        public async Task<BasicGraphModelPoco> GetGraphModel(string graphname)
        {
            if (graphname == string.Empty || graphname == null)
            {
                return null;
            }
            BasicGraphModel model = await GraphFileProvider.GetBasicGraph(graphname);
            BasicGraphModelPoco modelPoco = new BasicGraphModelPoco(model);
            return modelPoco;
        }
        /// <summary>
        /// Gibt auf dem Server gespeicherte GraphModels zurück
        /// </summary>
        /// <returns>Liste aller GraphModels</returns>
        [HttpGet]
        public async Task<IEnumerable<BasicGraphModelPoco>> GetGraphModels()
        {
            var models = (await GraphFileProvider.GetBasicGraphs()).Select(x => new BasicGraphModelPoco(x));

            return models;
        }
        /// <summary>
        /// Findet alle Graphnamen
        /// </summary>
        /// <returns>Liste mit allen Namen der gefundenen Graphen</returns>
        [HttpGet("GetGraphFilenames")]
        public async Task<IEnumerable<string>> GetGraphFilenames()
        {
            var graphnames = await GraphFileProvider.GetGraphFileNames();
            return graphnames;
        }
        /// <summary>
        /// Speichert den übermittelten Graphen unter dem angegebenen Dateinamen
        /// </summary>
        /// <param name="filename">Dateiname für Speicherung</param>
        /// <param name="graph">Graphmodel</param>
        /// <returns>true, wenn erfolgreich. Sonst false</returns>
        [Authorize(Policy = "IsVisualGraphMember")]
        [HttpPost("SaveGraph/{filename}")]
        public async Task<bool> SaveGraph([FromRoute] string filename, [FromBody] BasicGraphModelPoco graph)
        {
            return await GraphFileProvider.WriteToGraphMlFile(new BasicGraphModel(graph), filename);
        }
        /// <summary>
        /// Lädt Graphen aus dem Internet
        /// </summary>
        /// <param name="url">URL des Graphen</param>
        /// <param name="name">Name das Geaphen</param>
        /// <returns>Mapping der Attribute des heruntergeladenen Graphen</returns>
        [HttpGet("LoadFromWeb")]
        public async Task<BasicGraphToGraphMlMapping> LoadFromWeb([FromQuery] string url, [FromQuery] string name)
        {
            if (url != null && url != string.Empty)
            {
                if (name == null || name == string.Empty)
                {
                    name = "unknownFromWeb";
                }
                var mapping = await GraphFileProvider.LoadBasicGraphFromUrl(url, name);
                return mapping;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Läd Daten aus dem Internet
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Webinhalt als String</returns>
        [HttpGet("GetWebData")]
        public Task<string> getWebData([FromQuery] string url)
        {
            string result = string.Empty;
            using (var client = new WebClient())
            {
                var proxyConfig = VGAppSettings.RemoteRequestProxy;
                if (proxyConfig != "")
                {
                    client.Proxy = new WebProxy(proxyConfig);
                }
                try
                {
                    result = client.DownloadString(url);
                }
                catch {}
            }
            return Task.FromResult(result);
        }

    }
}
