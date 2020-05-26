using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VisualGraph.Shared.Models
{
    public class GraphStyleParametersPOCO
    {
        #region SVG
        public double Padding { get; set; }
        #endregion
        #region Text
        public double TextSize { get; set; }
        public string TextColor { get; set; }
        #endregion
        #region Node
        public double NodeRadius { get; set; }
        public string NodeStrokeColor { get; set; }
        public double NodeStrokeWidth { get; set; }
        public string NodeFill { get; set; }
        public string NodeActiveFill { get; set; }
        public string NodeStartFill { get; set; }
        public string NodeEndFill { get; set; }
        #endregion
        #region Edge
        public string EdgeActiveStrokeColor { get; set; }
        public string EdgeStrokeColor { get; set; }
        public double EdgeWidth { get; set; }
        public string EdgeRouteColor { get; set; }
        public double EdgeRouteWidth { get; set; }
        public string EdgeHighlitingColor { get; set; }
        public double EdgeHighlitingWidth { get; set; }
        #endregion
        public void Initialize(GraphStyleParameters GraphStyleParameters)
        {
            Padding = GraphStyleParameters.Padding;
            TextSize = GraphStyleParameters.TextSize;
            TextColor = GraphStyleParameters.TextColor;
            NodeRadius = GraphStyleParameters.NodeRadius;
            NodeStrokeColor = GraphStyleParameters.NodeStrokeColor;
            NodeStrokeWidth = GraphStyleParameters.NodeStrokeWidth;
            NodeFill = GraphStyleParameters.NodeFill;
            NodeActiveFill = GraphStyleParameters.NodeActiveFill;
            NodeEndFill = GraphStyleParameters.NodeEndFill;
            NodeStartFill = GraphStyleParameters.NodeStartFill;
            EdgeActiveStrokeColor = GraphStyleParameters.EdgeActiveStrokeColor;
            EdgeStrokeColor = GraphStyleParameters.EdgeStrokeColor;
            EdgeWidth = GraphStyleParameters.EdgeWidth;
            EdgeRouteColor = GraphStyleParameters.EdgeRouteColor;
            EdgeRouteWidth = GraphStyleParameters.EdgeRouteWidth;
            EdgeHighlitingColor = GraphStyleParameters.EdgeHighlitingColor;
            EdgeHighlitingWidth = GraphStyleParameters.EdgeHighlitingWidth;

        }

    }
}
