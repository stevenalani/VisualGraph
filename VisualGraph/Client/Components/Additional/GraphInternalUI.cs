using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using VisualGraph.Client.Services;

namespace VisualGraph.Client.Components.Additional
{
    /// <summary>
    /// Basisklasse für alle UserInterfaces, welche mit Graphen arbeiten. Diese Klasse hilft, 
    /// die verschiedenen UIs eventgesteuert neu zu rendern
    /// </summary>
    public abstract class GraphInternalUI : ComponentBase
    {
        private bool isRendered;
        /// <summary>
        /// Ermöglicht Zugriff auf alle Operationen im Zusammenhang mit graphen.
        /// </summary>
        [Inject]
        public IGraphService graphService { get; set; }
        /// <summary>
        /// Weißt Blazor an, die Komponente neu zu rendern
        /// </summary>
        /// <returns></returns>
        public Task ChangedState()
        {
            InvokeAsync(() =>
            {
                StateHasChanged();
            });
            return Task.CompletedTask;
        }
        /// <summary>
        /// Gibt an ob die Komponente bereits gerendert wurde
        /// </summary>
        public bool IsRendered => isRendered;
        /// <summary>
        /// Stellt nach dem Rendern an isRendered auf true
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) { isRendered = true; }
            return base.OnAfterRenderAsync(firstRender);
        }
    }
}
