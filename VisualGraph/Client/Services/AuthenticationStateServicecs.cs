using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.Services
{
    public class AuthenticationStateService : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private AuthenticationState authenticationState;

        public AuthenticationStateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsPrincipal user;


            var result =  await _httpClient.GetFromJsonAsync<UserModel>("api/Account/user");


            if (result.Id != "")
            {
                // Create a ClaimsPrincipal for the user
                var identity = new ClaimsIdentity(new[]
                {
                   new Claim(ClaimTypes.Name, result.Username),
                   new Claim(ClaimTypes.GivenName, "cn=" + result.Firstname),
                    new Claim(ClaimTypes.NameIdentifier, result.Id),
                }, "VisualGraphCookie");

                user = new ClaimsPrincipal(identity);
            }

            else

            {
                user = new ClaimsPrincipal(); // Not logged in
            }

            return await Task.FromResult(new AuthenticationState(user));
        }
    }
}
