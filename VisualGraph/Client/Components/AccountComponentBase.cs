using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualGraph.Client.Services;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.Components
{

    public class AccountComponentBase : ComponentBase
    {
        protected System.Security.Claims.ClaimsPrincipal User { get; private set; }
        [Inject]
        protected IAccountService AccountService { get; set; }
        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        private async void AuthenticationStateProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
        {
            User = (await task).User;
            var isAuthenticated = User.Identity?.IsAuthenticated;
            if (isAuthenticated??false)
            {
                Console.WriteLine("AuthenticationStateChanged: " + this.GetType());
                StateHasChanged();
            }
        }
        protected override void OnInitialized()
        {
            AuthenticationStateProvider.AuthenticationStateChanged += AuthenticationStateProvider_AuthenticationStateChanged;
        }

        private async void updateUserModel()
        {
            User = await AccountService.GetUser();
        }
    }
}
