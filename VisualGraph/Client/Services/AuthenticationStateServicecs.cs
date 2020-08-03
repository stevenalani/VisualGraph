using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.Services
{
    /// <summary>
    /// Dieser Service frägt vom Server den angemeldeten User ab und authentifiziert Anwender.
    /// </summary>
    public class AuthenticationStateService : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private AuthenticationState lastAuthenticationState;
        /// <summary>
        /// Erstellt eine Instanz der Klasse
        /// </summary>
        /// <param name="httpClient">Dependency Injection des ASP .Net HttpClient</param>
        public AuthenticationStateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        /// <summary>
        /// Frägt den Aktuellen Benutzer vom Server ab
        /// </summary>
        /// <returns>AuthenticationState welcher angibt ob der Benutzer anthentifiziert wurde</returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsPrincipal user;
            var result = await _httpClient.GetFromJsonAsync<UserModel>("api/Account/user");


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
                lastAuthenticationState = new AuthenticationState(user);
                NotifyAuthenticationStateChanged(Task.FromResult(lastAuthenticationState));
            }
            return await Task.FromResult(lastAuthenticationState);
        }

    }
}
