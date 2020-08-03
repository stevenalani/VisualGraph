using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VisualGraph.Server.Providers;
using VisualGraph.Shared.Models;

namespace VisualGraph.Server.Controllers
{
    /// <summary>
    /// Erlaubt Clients Zugriff auf Einstllungen
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ILogger<SettingsController> logger;
        /// <summary>
        /// Erstellt Instanz der Klasse
        /// </summary>
        /// <param name="logger"></param>
        public SettingsController(ILogger<SettingsController> logger)
        {
            this.logger = logger;
        }
        /// <summary>
        /// Gibt die GraphStyleParameter aus
        /// </summary>
        /// <param name="username">Benutzer für den das Styling abgefragt werden soll</param>
        /// <returns></returns>
        [HttpGet("GetGraphStyle/{username?}")]
        public async Task<GraphStyleParametersPOCO> GetGraphStyle(string username = "")
        {
            username = username != "" ? username : User.Identity.IsAuthenticated ? User.Identity.Name : "";
            var settings = await SettingsFileProvider.GetGraphStyleParameters(username);
            return settings;
        }
    }

}
