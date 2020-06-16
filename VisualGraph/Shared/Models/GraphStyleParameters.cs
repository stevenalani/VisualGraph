using System.Globalization;

namespace VisualGraph.Shared.Models
{
    public class GraphStyleParameters
    {
        #region SVG
        public double Padding { get; set; } = 3.0;
        public double FullPadding => Padding + NodeRadius + NodeStrokeWidth;
        public string FullPaddingText => FullPadding.ToString(CultureInfo.InvariantCulture) + "px";
        #endregion
        #region Text
        public double TextSize { get; set; } = 1.5;
        public string TextSizeText => TextSize.ToString(CultureInfo.InvariantCulture) + "px";
        public string TextColor { get; set; } = "#000000";
        #endregion
        #region Node
        public double NodeRadius { get; set; } = 2.2;
        public string NodeRadiusText => NodeRadius.ToString(CultureInfo.InvariantCulture) + "px";
        public string NodeStrokeColor { get; set; } = "rgba(0, 79, 224, 0.9)";
        public double NodeStrokeWidth { get; set; } = 0.2;
        public string NodeStrokeWidthText => NodeStrokeWidth.ToString(CultureInfo.InvariantCulture) + "px";
        public string NodeFill { get; set; } = "rgba(16,200, 255)";

        public string NodeActiveFill { get; set; } = "rgba(0, 79, 224,1)";
        public string NodeStartFill { get; set; } = "rgba(50, 255, 0, 1)";
        public string NodeEndFill { get; set; } = "rgba(255,50,0,1)";


        #endregion
        #region Edge
        public string EdgeActiveStrokeColor { get; set; } = "rgba(0, 79, 224, 1)";
        public string EdgeStrokeColor { get; set; } = "rgba(125,150,125,1)";
        public double EdgeWidth { get; set; } = 0.2;
        public string EdgeWidthText => EdgeWidth.ToString(CultureInfo.InvariantCulture) + "px";
        public string EdgeRouteColor { get; set; } = "rgba(125,255,125,1)";
        public double EdgeRouteWidth { get; set; } = 0.4;
        public string EdgeRouteWidthText => EdgeRouteWidth.ToString(CultureInfo.InvariantCulture) + "px";
        public string EdgeHighlitingColor { get; set; } = "rgba(255,100,100,1)";
        public double EdgeHighlitingWidth { get; set; } = 0.4;
        public string EdgeHighlitingWidthText => EdgeHighlitingWidth.ToString(CultureInfo.InvariantCulture) + "px";

        #endregion
        public void InitFromPoco(GraphStyleParametersPOCO styleParametersPOCO)
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
            EdgeActiveStrokeColor = styleParametersPOCO.EdgeActiveStrokeColor;
            EdgeStrokeColor = styleParametersPOCO.EdgeStrokeColor;
            EdgeWidth = styleParametersPOCO.EdgeWidth;
            EdgeRouteColor = styleParametersPOCO.EdgeRouteColor;
            EdgeRouteWidth = styleParametersPOCO.EdgeRouteWidth;
            EdgeHighlitingColor = styleParametersPOCO.EdgeHighlitingColor;
            EdgeHighlitingWidth = styleParametersPOCO.EdgeHighlitingWidth;
        }
    }
}
