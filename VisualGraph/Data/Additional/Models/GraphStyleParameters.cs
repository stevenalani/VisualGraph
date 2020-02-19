using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace VisualGraph.Data.Additional.Models
{
    public  class GraphStyleParameters
    {
        #region SVG
        public  double Padding { get; set; } = 3.0;
        public  double FullPadding => Padding + NodeRadius + NodeStrokeWidth;
        public  string FullPaddingText => FullPadding.ToString(CultureInfo.InvariantCulture) + "px";
        #endregion
        #region Text
        public  double TextSize { get; set; } = 1.5;
        public  string TextSizeText => TextSize.ToString(CultureInfo.InvariantCulture) + "px";
        public  string TextColor { get; set; } = "#000000";
        #endregion
        #region Node
        public  double NodeRadius { get; set; } = 1.1;
        public  string NodeRadiusText => NodeRadius.ToString(CultureInfo.InvariantCulture) + "px";
        public  string NodeStrokeColor { get; set; } = "black";
        public  double NodeStrokeWidth { get; set; } = 0.2;
        public  string NodeStrokeWidthText => NodeStrokeWidth.ToString(CultureInfo.InvariantCulture) + "px";
        public  string NodeFill { get; set; } = "#8cd9d9";

        public  string NodeActiveFill { get; set; } = "rgba(0, 79, 224, 0.7)";
        public  string NodeStartFill { get; set; } = "rgba(0, 255, 224, 0.7)";
        public  string NodeEndFill { get; set; } = "rgba(79,0,224,0.7)";


        #endregion
        #region Edge
        public  string EdgeStrokeColor { get; set; } = "#d966ff";
        public  double EdgeWidth { get; set; } = 0.2;
        public  string EdgeWidthText => EdgeWidth.ToString(CultureInfo.InvariantCulture) + "px";
        public  string EdgeRouteColor { get; set; } = "lime";
        public  double EdgeRouteWidth { get; set; } = 0.4;
        public  string EdgeRouteWidthText => EdgeRouteWidth.ToString(CultureInfo.InvariantCulture) + "px";
        public  string EdgeHighlitingColor { get; set; } = "red";
        public  double EdgeHighlitingWidth { get; set; } = 0.4;
        public  string EdgeHighlitingWidthText => EdgeHighlitingWidth.ToString(CultureInfo.InvariantCulture) + "px";

        #endregion
        public  void InitFromPoco(GraphStyleParametersPOCO styleParametersPOCO)
        {
            Padding = styleParametersPOCO.Padding;
            TextSize = styleParametersPOCO.TextSize;
            TextColor = styleParametersPOCO.TextColor;
            NodeRadius = styleParametersPOCO.NodeRadius;
            NodeStrokeColor = styleParametersPOCO.NodeStrokeColor;
            NodeStrokeWidth = styleParametersPOCO.NodeStrokeWidth;
            NodeFill = styleParametersPOCO.NodeFill;
            NodeEndFill = styleParametersPOCO.NodeEndFill;
            NodeActiveFill = styleParametersPOCO.NodeActiveFill;
            NodeStartFill = styleParametersPOCO.NodeStartFill;
            EdgeStrokeColor = styleParametersPOCO.EdgeStrokeColor;
            EdgeWidth = styleParametersPOCO.EdgeWidth;
            EdgeRouteColor = styleParametersPOCO.EdgeRouteColor;
            EdgeRouteWidth = styleParametersPOCO.EdgeRouteWidth;
            EdgeHighlitingColor = styleParametersPOCO.EdgeHighlitingColor;
            EdgeHighlitingWidth = styleParametersPOCO.EdgeHighlitingWidth;
        }
    }
}
