using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualGraph.Client.Services;

namespace VisualGraph.Client.Components.Additional
{
    
    public abstract class GraphInternalUI : ComponentBase
    {
        private bool isRendered;
        [Inject]
        public IGraphService graphService { get; set; }

        public Task ChangedState()
        {
            InvokeAsync(() => {
                StateHasChanged();
            });
            return Task.CompletedTask;
        }

        public bool IsRendered => isRendered;
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) { isRendered = true; }
            return base.OnAfterRenderAsync(firstRender);
        }
    }
}
