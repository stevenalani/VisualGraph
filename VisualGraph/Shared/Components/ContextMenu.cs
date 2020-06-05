using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VisualGraph.Shared.Components
{
    public class ContextMenu : ComponentBase
    {
        [Parameter]
        public double PosX { get; set; }
        [Parameter]
        public double PosY { get; set; }

        protected bool visible = false;
        public bool IsVisible { get { return visible; } }
        public void Show()
        {
            visible = !visible;
            StateHasChanged();
        }
    }
}
