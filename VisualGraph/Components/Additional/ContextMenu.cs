using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VisualGraph.Components.Additional
{
    public class ContextMenu : ComponentBase
    {
        [Parameter]
        public double PosX { get; set; }
        [Parameter]
        public double PosY { get; set; }

        public bool visible = false;
    }
}
