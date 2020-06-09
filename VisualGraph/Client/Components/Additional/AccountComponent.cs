using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Claims;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.Components.Additional
{
    public class AccountComponent : ComponentBase
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthTask { get; private set; }

        public UserModel UserModel { get; set; } = new UserModel() { Username = "Gast" };
        [Inject] public HttpClient HttpClient { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected async Task updateModel()
        {
            var isRefreshRequiered = false;
            if ((await AuthTask).User.Identity.IsAuthenticated)
            {
                var um = await HttpClient.GetJsonAsync<UserModel>($"api/account/user");
                isRefreshRequiered = um.Id != UserModel.Id;
                UserModel = um;
            }
            if (isRefreshRequiered)
                StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var cookie = await JSRuntime.InvokeAsync<string>("GetCookie");
            HttpClient.DefaultRequestHeaders.Add("Cookie", cookie);
            /*if (!cookie.Contains("Visualgraph"))
            {
                await JSRuntime.InvokeAsync<string>("ClearCookie");
            }*/
            await updateModel();

        }
    }
}
