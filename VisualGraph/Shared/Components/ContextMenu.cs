using Microsoft.AspNetCore.Components;

namespace VisualGraph.Shared.Components
{
    /// <summary>
    /// Basisklasse für Kontext- Menüs
    /// </summary>
    public class ContextMenu : ComponentBase
    {
        /// <summary>
        /// Position X wo das Menü angezeigt werden soll
        /// </summary>
        [Parameter]
        public double PosX { get; set; }
        /// <summary>
        /// Position Y wo das Menü angezeigt werden soll
        /// </summary>
        [Parameter]
        public double PosY { get; set; }
        /// <summary>
        /// Gibt an ob das Menü sichtbar ist
        /// </summary>
        protected bool visible = false;
        /// <summary>
        /// Gibt an ob das Menü sichtbar ist 
        /// </summary>
        public bool IsVisible { get { return visible; } }
        /// <summary>
        /// Blendet das Menü ein oder aus.
        /// </summary>
        public void Show()
        {
            visible = !visible;
            StateHasChanged();
        }
    }
}
