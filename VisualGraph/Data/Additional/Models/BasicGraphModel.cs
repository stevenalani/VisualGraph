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
using System.Numerics;

namespace VisualGraph.Data.Additional.Models
{
    public class BasicGraphModel : IGraphFile
    {
        public bool IsAxisShown { get; set; } = true;
        public static int sequence = 0;

        public string Name => System.IO.Path.GetFileNameWithoutExtension(Path);
        public List<Node> Nodes { get; set; } = new List<Node>();
        public List<Edge> Edges { get; set; } = new List<Edge>();
        public string Path { get; set; }
        public Node ActiveNode => Nodes.FirstOrDefault(x => x.Activeclass != "");

        float maxX => Nodes.Max(x => (int)Math.Ceiling(x.Pos.X));
        float minX => Nodes.Min(x => (int)Math.Ceiling(x.Pos.X));
        float maxY => Nodes.Max(x => (int)Math.Ceiling(x.Pos.Y));
        float minY => Nodes.Min(x => (int)Math.Ceiling(x.Pos.Y));
        public Vector2[] ConvexHull => new[] { new Vector2(minX, minY), new Vector2(maxX, maxY) };

        public bool IsDirectional { get; set; } = true;
        public bool IsBidirectional => IsDirectional && Edges.Where( x=> x.StartNode != null && x.EndNode != null).Count(x => Edges.FirstOrDefault(y => y.EndNode == x.StartNode)?.StartNode == x.EndNode ) > 0;
    }
}