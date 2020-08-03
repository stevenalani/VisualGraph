using System.Globalization;

namespace VisualGraph.Shared.Models
{
    /// <summary>
    /// Steuert die Darstellung der Graphen
    /// </summary>
    public class GraphStyleParameters
    {
        #region SVG
        /// <summary>
        /// Abstand zu den Seiten des SVG- Conatiners 
        /// </summary>
        public double Padding { get; set; } = 3.0;
        /// <summary>
        /// Abstand inkl Node- Radius und Node- Konturstärke
        /// </summary>
        public double FullPadding => Padding + NodeRadius + NodeStrokeWidth;
        /// <summary>
        /// Abstand (FullPadding) als InvariantCulture-  String in px
        /// </summary>
        public string FullPaddingText => FullPadding.ToString(CultureInfo.InvariantCulture) + "px";
        #endregion
        #region Text
        /// <summary>
        /// SVG- Textgröße
        /// </summary>
        public double TextSize { get; set; } = 1.5;
        /// <summary>
        /// Textgröße als InvariantCulture- String in px
        /// </summary>
        public string TextSizeText => TextSize.ToString(CultureInfo.InvariantCulture) + "px";
        /// <summary>
        /// Textfarbe
        /// </summary>
        public string TextColor { get; set; } = "#000000";
        #endregion
        #region Node
        /// <summary>
        /// Knotenradius
        /// </summary>
        public double NodeRadius { get; set; } = 2.2;
        /// <summary>
        /// Knotenradius als InvariantCulture- String in px
        /// </summary>
        public string NodeRadiusText => NodeRadius.ToString(CultureInfo.InvariantCulture) + "px";
        /// <summary>
        /// Knoten- Konturfarbe (rgb(r,g,b), rgba(r,g,b,a), #rrggbb oder HTML- Farbdefinitionen)
        /// </summary>
        public string NodeStrokeColor { get; set; } = "rgba(0, 79, 224, 0.9)";
        /// <summary>
        /// Knoten- Konturstärke
        /// </summary>
        public double NodeStrokeWidth { get; set; } = 0.2;
        /// <summary>
        /// Knoten- Konturstärke als InvariantCulture- String in px
        /// </summary>
        public string NodeStrokeWidthText => NodeStrokeWidth.ToString(CultureInfo.InvariantCulture) + "px";
        /// <summary>
        /// Knoten- Füllfarbe (rgb(r,g,b), rgba(r,g,b,a), #rrggbb oder HTML- Farbdefinitionen)
        /// </summary>
        public string NodeFill { get; set; } = "rgba(16,200, 255)";
        /// <summary>
        /// Aktiver Knoten- Füllfarbe (rgb(r,g,b), rgba(r,g,b,a), #rrggbb oder HTML- Farbdefinitionen)
        /// </summary>
        public string NodeActiveFill { get; set; } = "rgba(0, 79, 224,1)";
        /// <summary>
        /// Startknoten- Füllfarbe (rgb(r,g,b), rgba(r,g,b,a), #rrggbb oder HTML- Farbdefinitionen)
        /// </summary>
        public string NodeStartFill { get; set; } = "rgba(50, 255, 0, 1)";
        /// <summary>
        /// Endknoten- Füllfarbe (rgb(r,g,b), rgba(r,g,b,a), #rrggbb oder HTML- Farbdefinitionen)
        /// </summary>
        public string NodeEndFill { get; set; } = "rgba(255,50,0,1)";


        #endregion
        #region Edge
        /// <summary>
        /// Konturfarbe aktive Kante (rgb(r,g,b), rgba(r,g,b,a), #rrggbb oder HTML- Farbdefinitionen)
        /// </summary>
        public string EdgeActiveStrokeColor { get; set; } = "rgba(0, 79, 224, 1)";
        /// <summary>
        /// Kantenkonturfarbe (rgb(r,g,b), rgba(r,g,b,a), #rrggbb oder HTML- Farbdefinitionen)
        /// </summary>
        public string EdgeStrokeColor { get; set; } = "rgba(125,150,125,1)";
        /// <summary>
        /// Kantenlinienstärke
        /// </summary>
        public double EdgeWidth { get; set; } = 0.2;
        /// <summary>
        /// Kantenlinienstärke als InvariantCulture- String in px
        /// </summary>
        public string EdgeWidthText => EdgeWidth.ToString(CultureInfo.InvariantCulture) + "px";
        /// <summary>
        /// Farbe für Kanten auf der berechneten Route
        /// </summary>
        public string EdgeRouteColor { get; set; } = "rgba(125,255,125,1)";
        /// <summary>
        /// Kantenlinienstärke für Kanten auf der berechneten Route
        /// </summary>
        public double EdgeRouteWidth { get; set; } = 0.4;
        /// <summary>
        /// Kantenlinienstärke für Kanten auf der berechneten Route als InvariantCulture- String in px
        /// </summary>
        public string EdgeRouteWidthText => EdgeRouteWidth.ToString(CultureInfo.InvariantCulture) + "px";
        /// <summary>
        /// Hervorhebungsfarbe für Kanten
        /// </summary>
        public string EdgeHighlitingColor { get; set; } = "rgba(255,100,100,1)";
        /// <summary>
        /// Kantenlinienstärke für hervorgehobene Kanten
        /// </summary>
        public double EdgeHighlitingWidth { get; set; } = 0.4;
        /// <summary>
        /// Kantenlinienstärke für hervorgehobene Kanten als InvariantCulture- String in px
        /// </summary>
        public string EdgeHighlitingWidthText => EdgeHighlitingWidth.ToString(CultureInfo.InvariantCulture) + "px";

        #endregion
        /// <summary>
        /// Initialisiert die Werte aus Ergebnissen von API- Abfragen
        /// </summary>
        /// <param name="styleParametersPOCO"></param>
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
