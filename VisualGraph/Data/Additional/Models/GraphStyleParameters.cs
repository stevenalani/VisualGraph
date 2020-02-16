using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace VisualGraph.Data.Additional.Models
{
    public static class GraphStyleParameters
    {
        #region SVG
        public static double Padding { get; set; } = 3.0;
        public static double FullPadding => Padding + NodeRadius + NodeStrokeWidth;
        public static string FullPaddingText => FullPadding.ToString(CultureInfo.InvariantCulture) + "px";
        #endregion
        #region Text
        public static double TextSize { get; set; } = 1.5;
        public static string TextSizeText => TextSize.ToString(CultureInfo.InvariantCulture) + "px";
        public static string TextColor { get; set; } = "#000000";
        #endregion
        #region Node
        public static double NodeRadius { get; set; } = 1.1;
        public static string NodeRadiusText => NodeRadius.ToString(CultureInfo.InvariantCulture) + "px";
        public static string NodeStrokeColor { get; set; } = "black";
        public static double NodeStrokeWidth { get; set; } = 0.2;
        public static string NodeStrokeWidthText => NodeStrokeWidth.ToString(CultureInfo.InvariantCulture) + "px";
        public static string NodeFill { get; set; } = "#8cd9d9";

        public static string NodeActiveFill { get; set; } = "rgba(0, 79, 224, 0.7)";
        public static string NodeStartFill { get; set; } = "rgba(0, 255, 224, 0.7)";
        public static string NodeEndFill { get; set; } = "rgba(79,0,224,0.7)";


        #endregion
        #region Edge
        public static string EdgeStrokeColor { get; set; } = "#d966ff";
        public static double EdgeWidth { get; set; } = 0.2;
        public static string EdgeWidthText => EdgeWidth.ToString(CultureInfo.InvariantCulture) + "px";
        public static string EdgeRouteColor { get; set; } = "lime";
        public static double EdgeRouteWidth { get; set; } = 0.4;
        public static string EdgeRouteWidthText => EdgeRouteWidth.ToString(CultureInfo.InvariantCulture) + "px";
        public static string EdgeHighlitingColor { get; set; } = "red";
        public static double EdgeHighlitingWidth { get; set; } = 0.4;
        public static string EdgeHighlitingWidthText => EdgeHighlitingWidth.ToString(CultureInfo.InvariantCulture) + "px";

        #endregion
        public static void InitFromPoco(GraphStyleParametersPOCO styleParametersPOCO)
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
