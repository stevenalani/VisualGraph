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
        [Inject]
        protected IAccountService AccountService { get; set; }
        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    }
}
