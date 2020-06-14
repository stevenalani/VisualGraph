using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.Services
{
    public class AccountService : IAccountService
    {
        public UserModel User { get; private set; }
        private string baseroute = "/api/Account";
        private readonly HttpClient _httpClient;
        public NavigationManager NavigationManager { get; }
        private readonly AuthenticationStateProvider authState;
        private ILogger<GraphService> _logger { get; }
        private IJSRuntime JSRuntime { get; }

        public AccountService(AuthenticationStateProvider authenticationState, NavigationManager manager, ILogger<GraphService> logger, IJSRuntime jsRuntime, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _logger = logger;
            JSRuntime = jsRuntime;
            NavigationManager = manager;
            authState = authenticationState;
        }
        public async Task<UserModel> GetUserModelFromName(string username)
        {
            var result = await _httpClient.GetFromJsonAsync<UserModel>($"{baseroute}/user/{username}");
            return result;
        }
        public async Task<ClaimsPrincipal> GetUser()
        {
            return (await authState.GetAuthenticationStateAsync()).User;
        }
        public async Task LogIn(UserModel userModel)
        {
            var response = await _httpClient.PostAsJsonAsync<UserModel>($"{baseroute}/login", userModel);
            var user = await response.Content.ReadFromJsonAsync<UserModel>();
            User = user;
            if ((await authState.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated)
            {
                _logger.LogDebug("user is authed after login");
            }
            IEnumerable<string> vals;
            response.Headers.TryGetValues("Set-Cookie", out vals);
            if (vals != null)
            {
                await JSRuntime.InvokeVoidAsync("SetCookie", vals?.LastOrDefault() ?? "");
                NavigationManager.NavigateTo(NavigationManager.Uri, true);
                user.IsAuth = true;
            }
        }

        public async Task LogOut()
        {
            await _httpClient.GetAsync($"{baseroute}/logout");
        }

        public async Task<UserModel> Register(UserModel userModel)
        {
            var response = await _httpClient.PostAsJsonAsync<UserModel>($"{baseroute}/register", userModel);
            return await response.Content.ReadFromJsonAsync<UserModel>();
        }

        public async Task<UserModel> UpdateUser(UserModel userModel)
        {
            var response = await _httpClient.PostAsJsonAsync<UserModel>($"{baseroute}/update", userModel);
            return await response.Content.ReadFromJsonAsync<UserModel>();
        }

        public async Task<bool> UserIsAuthenticated()
        {
            var state = await authState.GetAuthenticationStateAsync();
            return state.User.Identity.IsAuthenticated;
        }

        public async Task<UserModel> GetUserModel()
        {
            if (User != null)
            {
                return await _httpClient.GetFromJsonAsync<UserModel>($"{baseroute}/user/{User.Username}");
            }
            return await _httpClient.GetFromJsonAsync<UserModel>($"{baseroute}/user");
        }
        public Task<UserUpdateModel> GetUserUpdateModel()
        {
            if (User != null)
            {
                return Task.FromResult(UserUpdateModel.FromUserModel(User));
            }
            NavigationManager.NavigateTo("/");
            return Task.FromResult(new UserUpdateModel());
        }

    }
}
