using Microsoft.AspNetCore.Components;

namespace VisualGraph.Client.Components.Additional
{
    /// <summary>
    /// Basisklasse für Context- Menüs (Rechtsklick- Menüs)
    /// </summary>
    public class ContextMenu : ComponentBase
    {
        /// <summary>
        /// X- Koordinate wo das Menü angezeigt werden soll
        /// </summary>
        [Parameter]
        public double PosX { get; set; }
        /// <summary>
        /// Y- Koordinate wo das Menü angezeigt werden soll
        /// </summary>
        [Parameter]
        public double PosY { get; set; }
        /// <summary>
        /// Ist das Kontextmenü sichtbar?
        /// </summary>
        protected bool visible = false;
        /// <summary>
        /// veröffentlicht die visible Varriable
        /// </summary>
        public bool IsVisible { get { return visible; } }
        /// <summary>
        /// Belendet das Menü ein oder aus.
        /// </summary>
        public void Show()
        {
            visible = !visible;
            StateHasChanged();
        }
    }
}
