using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Components.Additional
{
    
    public abstract class GraphInternalUI : ComponentBase
    {

        private bool isRendered;
        public Task ChangedState()
        {
            InvokeAsync(() => {
                StateHasChanged();
            });
            return Task.CompletedTask;
        }

        public Task<bool> IsRendered => Task.FromResult(isRendered);
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) { isRendered = true; }
            return base.OnAfterRenderAsync(firstRender);
        }
    }
}
