using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace VisualGraph.Data.Additional.Models
{
    public class GraphStyleParameters
    {
        #region SVG
        public double Padding { get; set; } = 3.0;
        public  double FullPadding => Padding + NodeRadius + NodeStrokeWidth;

        public string FullPaddingText => FullPadding.ToString(CultureInfo.InvariantCulture);
        #endregion
        #region Text
        public double TextSize { get; set; } = 2.1;

        public string TextSizeText => TextSize.ToString(CultureInfo.InvariantCulture);


        public string TextColor { get; set; } = "#000000";
        #endregion
        #region Node
        public double NodeRadius { get; set; } = 1.1;
        public string NodeRadiusText => NodeRadius.ToString(CultureInfo.InvariantCulture);
        public string NodeStrokeColor { get; set; } = "black";
        public double NodeStrokeWidth { get; set; } = 0.2;
        public string NodeStrokeWidthText => NodeStrokeWidth.ToString(CultureInfo.InvariantCulture);
        public string NodeFill { get; set; } = "#8cd9d9";
        #endregion
        #region Edge
        public string EdgeStrokeColor { get; set; } = "#d966ff";
        public double EdgeWidth { get; set; } = 0.2;
        public string EdgeWidthText => EdgeWidth.ToString(CultureInfo.InvariantCulture);
        #endregion
    }
}
