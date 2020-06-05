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

namespace VisualGraph.Server.Shared
{
    public class AccountComponent : ComponentBase
    { 
        [CascadingParameter]
        public Task<AuthenticationState> AuthTask { get; private set; }

        protected HttpClient httpclient { get; set; }
        public UserModel UserModel { get; set; } = new UserModel() { Username = "Gast" };
        [Inject] public IHttpClientFactory HttpFactory { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        
        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected async Task updateModel()
        {
            var isRefreshRequiered = false;
            if ((await AuthTask).User.Identity.IsAuthenticated)
            {
                var um = await httpclient.GetJsonAsync<UserModel>($"account/user");
                isRefreshRequiered = um.Id != UserModel.Id;
                UserModel = um;
            }
            if (isRefreshRequiered)
                StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var cookie = await JSRuntime.InvokeAsync<string>("GetCookie");
            if (httpclient == null)
            {
                httpclient = HttpFactory.CreateClient("api");
            }
            httpclient.DefaultRequestHeaders.Add("Cookie", cookie);
            if (!cookie.Contains("Visualgraph"))
            {
                await JSRuntime.InvokeAsync<string>("ClearCookie");
            }
            await updateModel();
            
        }
    }
}
