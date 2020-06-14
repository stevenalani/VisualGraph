using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VisualGraph.Server.Providers;
using VisualGraph.Shared.Models;

namespace VisualGraph.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ILogger<SettingsController> logger;

        public SettingsController(ILogger<SettingsController> logger)
        {
            this.logger = logger;
        }

        [HttpGet("GetGraphStyle/{username?}")]
        public async Task<GraphStyleParametersPOCO> GetGraphStyle(string username = "")
        {
            username = username != "" ? username : User.Identity.IsAuthenticated ? User.Identity.Name : "";
            var settings = await SettingsFileProvider.GetGraphStyleParameters(username);
            return settings;
        }
    }

}
