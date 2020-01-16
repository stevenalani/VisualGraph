using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VisualGraph.Data.Additional.Models
{
    public class SvgInformation
    {
        public double OffsetLeft { get; set; }
        public double OffsetTop { get; set; }
        public double PanX { get; set; }
        public double PanY { get; set; }
        public double Zoom { get; set; }
        public double PanZoomHeight { get; set; }
        public double PanZoomWidth { get; set; }
        public double ViewBoxHeight { get; set; }
        public double ViewBoxWidth { get; set; }
    }
}
