using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using VisualGraph.Shared.Models;

namespace VisualGraph.Server.Services
{
    public class AuthenticationStateProviderService : ServerAuthenticationStateProvider
    {
        private readonly IJSRuntime _jsruntime;
        private readonly HttpClient _hhtpClient;
        private AuthenticationState lastAuthenticationState;


        public AuthenticationStateProviderService(ILoggerFactory loggerFactory, IHttpClientFactory http)
        {
            _hhtpClient = http.CreateClient("api");
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsPrincipal user;
            var result = await _hhtpClient.GetFromJsonAsync<UserModel>("account/user");


            if (result.Id != "")
            {
                // Create a ClaimsPrincipal for the user
                var identity = new ClaimsIdentity(new[]
                {
                   new Claim(ClaimTypes.Name, result.Username),
                   new Claim(ClaimTypes.GivenName, "cn=" + result.Firstname),
                    new Claim(ClaimTypes.NameIdentifier, result.Id),
                }, "VisualGraphCookie");
                foreach (var role in result.Roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
                user = new ClaimsPrincipal(identity);
            }
            else
            {
                user = new ClaimsPrincipal(new ClaimsIdentity());
            }
            var newstate = new AuthenticationState(user);
            if (lastAuthenticationState == null || lastAuthenticationState.User.Identity?.IsAuthenticated != newstate.User.Identity.IsAuthenticated)
            {

                NotifyAuthenticationStateChanged(Task.FromResult(newstate));
            }
            lastAuthenticationState = newstate;
            return lastAuthenticationState;
        }
    }
}
