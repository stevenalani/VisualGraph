using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VisualGraph.Data.Additional.Interfaces;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Blazor.Components;

namespace VisualGraph.Data.Additional.Models
{
    public class BasicGraph : IRenderable, IGraphFile
    {
        public bool IsAxisShown { get; set; } = true;
        private static int sequence = 0;
        public string Name { get; set; }
        public List<Node> Nodes { get; set; } = new List<Node>();
        public List<Edge> Edges { get; set; } = new List<Edge>();
        public GraphStyleParameters GraphStyleParameters { get; set; } = new GraphStyleParameters();
        public string Path { get; set; }


        public int AddToRenderTree(RenderTreeBuilder builder,int sequence = 0)
        {
            double maxX = Nodes.Max(x => (int)Math.Ceiling(x.PosX));
            double minX = Nodes.Min(x => (int)Math.Ceiling(x.PosX));
            double maxY = Nodes.Max(x => (int)Math.Ceiling(x.PosY));
            double minY = Nodes.Min(x => (int)Math.Ceiling(x.PosY));

            minX -= (GraphStyleParameters.FullPadding);
            minY -= (GraphStyleParameters.FullPadding);
            maxX += (GraphStyleParameters.FullPadding);
            maxY += (GraphStyleParameters.FullPadding);

            builder.OpenElement(sequence++, "svg");
            builder.AddAttribute(sequence++, "viewBox", $"{(minX - GraphStyleParameters.Padding).ToString(CultureInfo.InvariantCulture)}," +
                $" {(minY - GraphStyleParameters.Padding).ToString(CultureInfo.InvariantCulture)}," +
                $" {(50 + maxX - minX).ToString(CultureInfo.InvariantCulture) }," +
                $" {(maxY - minY + GraphStyleParameters.Padding).ToString(CultureInfo.InvariantCulture)}");

            if (IsAxisShown)
                CoordinateAxis.AddToRenderTree(builder, sequence, minX, minY, maxX, maxY);

            foreach (var edge in Edges)
            {
                var posx1 = edge.StartNode.PosX.ToString(CultureInfo.InvariantCulture);
                var posy1 = edge.StartNode.PosY.ToString(CultureInfo.InvariantCulture);
                var posx2 = edge.EndNode.PosX.ToString(CultureInfo.InvariantCulture);
                var posy2 = edge.EndNode.PosY.ToString(CultureInfo.InvariantCulture);
                var halfwayx = ((Convert.ToDouble(posx1) + Convert.ToDouble(posx2)) / 2).ToString(CultureInfo.InvariantCulture);
                var halfwayy = ((Convert.ToDouble(posy1) + Convert.ToDouble(posy2)) / 2).ToString(CultureInfo.InvariantCulture);
                var weight = Convert.ToSingle(edge.Weight).ToString(CultureInfo.InvariantCulture);

                builder.OpenElement(sequence++, "line");
                builder.AddAttribute(sequence++, "x1", posx1);
                builder.AddAttribute(sequence++, "y1", posy1);
                builder.AddAttribute(sequence++, "x2", posx2);
                builder.AddAttribute(sequence++, "y2", posy2);
                builder.AddAttribute(sequence++, "style", $"stroke:{GraphStyleParameters.EdgeStrokeColor};stroke-width:{GraphStyleParameters.EdgeWidth.ToString(CultureInfo.InvariantCulture)};");
                builder.CloseElement();

                builder.OpenElement(sequence++, "text");
                builder.AddAttribute(sequence++, "x", halfwayx);
                builder.AddAttribute(sequence++, "y", halfwayy);
                builder.AddAttribute(sequence++, "fill", GraphStyleParameters.TextColor);
                builder.AddAttribute(sequence++, "font-size", GraphStyleParameters.TextSize.ToString(CultureInfo.InvariantCulture));
                builder.AddContent(sequence++, weight);
                builder.CloseElement();
            }
            foreach (var node in Nodes)
            {
                var posx = Convert.ToDouble(node.PosX).ToString(CultureInfo.InvariantCulture);
                var posy = Convert.ToDouble(node.PosY).ToString(CultureInfo.InvariantCulture);
                var name = node.Name;
                builder.OpenElement(sequence++, "circle");
                builder.AddAttribute(sequence++, "cx", posx);
                builder.AddAttribute(sequence++, "cy", posy);
                builder.AddAttribute(sequence++, "r", GraphStyleParameters.NodeRadius.ToString(CultureInfo.InvariantCulture));
                builder.AddAttribute(sequence++, "stroke", GraphStyleParameters.NodeStrokeColor);
                builder.AddAttribute(sequence++, "stroke-width", GraphStyleParameters.NodeStrokeWidth.ToString(CultureInfo.InvariantCulture));
                builder.AddAttribute(sequence++, "fill", GraphStyleParameters.NodeFill);
                builder.CloseElement();
                builder.OpenElement(sequence++, "text");
                builder.AddAttribute(sequence++, "x", (Convert.ToDouble(posx) + GraphStyleParameters.NodeRadius).ToString(CultureInfo.InvariantCulture));
                builder.AddAttribute(sequence++, "y", (Convert.ToDouble(posy) + GraphStyleParameters.NodeRadius).ToString(CultureInfo.InvariantCulture));
                builder.AddAttribute(sequence++, "fill", GraphStyleParameters.TextColor);
                builder.AddAttribute(sequence++, "font-size", GraphStyleParameters.TextSize.ToString(CultureInfo.InvariantCulture));
                builder.AddContent(sequence++, name);
                builder.CloseElement();
            }

            builder.CloseElement();
            return sequence;
        }

    }
}