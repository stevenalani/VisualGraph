using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VisualGraph.Data.Additional.Interfaces;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Blazor.Components;
using System.ComponentModel;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;
using VisualGraph.Components;

namespace VisualGraph.Data.Additional.Models
{
    public class BasicGraphModel : IGraphFile//,IRenderable
    {
        public bool IsAxisShown { get; set; } = true;
        public static int sequence = 0;

        public string Name => System.IO.Path.GetFileNameWithoutExtension(Path);
        public List<Node> Nodes { get; set; } = new List<Node>();
        public List<Edge> Edges { get; set; } = new List<Edge>();
        public string Path { get; set; }
        public Node ActiveNode => Nodes.FirstOrDefault(x => x.activeclass != "");

        double maxX => Nodes.Max(x => (int)Math.Ceiling(x.PosX));
        double minX => Nodes.Min(x => (int)Math.Ceiling(x.PosX));
        double maxY => Nodes.Max(x => (int)Math.Ceiling(x.PosY));
        double minY => Nodes.Min(x => (int)Math.Ceiling(x.PosY));
        public Point2[] ConvexHull => new[] { new Point2(minX, minY), new Point2(maxX, maxY) };

    }
}