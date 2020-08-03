using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Threading.Tasks;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.Components.Additional
{
    /// <summary>
    /// Basisklasse für alle Blazor- Komponenten, die mit dem Benutzer arbeiten.
    /// Die Klasse stellt sicher, dass Benutzerdaten immer aktuell sind und erkannt wird, wenn sich der Status (login/logout) ändert.
    /// </summary>
    public class AccountComponent : ComponentBase
    {
        /// <summary>
        /// Task zum Abfragen des Authentifizierungs- Status.
        /// </summary>
        [CascadingParameter]
        public Task<AuthenticationState> AuthTask { get; private set; }
        /// <summary>
        /// Gibt Zugriff auf den Aktuellen benutzer
        /// </summary>
        public UserModel UserModel { get; set; } = new UserModel() { Username = "Gast" };
        /// <summary>
        /// Http Client zur Kommunikation mit dem Server
        /// </summary>
        [Inject] public HttpClient HttpClient { get; set; }
        /// <summary>
        /// JavaScript Runtime zum Aufrufen von JavaScript funktionen
        /// </summary>
        [Inject] public IJSRuntime JSRuntime { get; set; }
        /// <summary>
        /// Navigation Manager zum Wecheln der URL
        /// </summary>
        [Inject] public NavigationManager NavigationManager { get; set; }
        /// <summary>
        /// Service zum Anzeigen von Toast Nachrichten
        /// </summary>
        [Inject] public IToastService ToastService { get; set; }

        /// <summary>
        /// Hält das Usermodel aktuell, indem Informationen zum eingeloggten User vom Server abgefragt werden.
        /// </summary>
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
        /// <summary>
        /// Frägt nach dem Rendern den aktuellen Login- Status des Benutzers ab
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await updateModel();   
        }
    }
}
