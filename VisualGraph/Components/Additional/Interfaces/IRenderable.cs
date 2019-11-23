using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data.Additional.Interfaces
{
    public interface IRenderable
    {
        public int AddToRenderTree(RenderTreeBuilder builder,int sequence = 0);
        public GraphStyleParameters GraphStyleParameters { get; set; }
        public List<Node> Nodes { get; set; }
        public List<Edge> Edges { get; set; }
        public string Name { get; set; }
    }
}