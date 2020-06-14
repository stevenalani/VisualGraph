
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace VisualGraph.Shared.Models
{
    public class BasicGraphModel
    {
        public static int sequence = 0;

        public BasicGraphModel() { }
        public BasicGraphModel(BasicGraphModelPoco graph)
        {
            this.Nodes = graph.NodesPoco.Select(y => new Node
            {
                Pos = new Vector2(float.Parse(y.PosXTextPoco.Replace(".", ",")), float.Parse(y.PosYTextPoco.Replace(".", ","))),
                Name = y.Name,
                Id = y.Id,
                Edges = y.Edges
            }).ToList();
            this.Edges = graph.Edges;
            this.Name = graph.Name;
            this.IsDirected = graph.IsDirected;
        }

        public string Name { get; set; }
        public List<Node> Nodes { get; set; } = new List<Node>();
        public List<Edge> Edges { get; set; } = new List<Edge>();
        [System.Text.Json.Serialization.JsonIgnore]
        public Node ActiveNode => Nodes.FirstOrDefault(x => x.IsActive);
        [System.Text.Json.Serialization.JsonIgnore]
        public Edge ActiveEdge => Edges.FirstOrDefault(x => x.IsActive);
        [System.Text.Json.Serialization.JsonIgnore]
        float maxX => Nodes.Max(x => (int)Math.Ceiling(x.Pos.X));
        [System.Text.Json.Serialization.JsonIgnore]
        float minX => Nodes.Min(x => (int)Math.Floor(x.Pos.X));
        [System.Text.Json.Serialization.JsonIgnore]
        float maxY => Nodes.Max(x => (int)Math.Ceiling(x.Pos.Y));
        [System.Text.Json.Serialization.JsonIgnore]
        float minY => Nodes.Min(x => (int)Math.Floor(x.Pos.Y));
        [System.Text.Json.Serialization.JsonIgnore]
        public Vector2[] ConvexHull => new[] { new Vector2(minX, minY), new Vector2(maxX, maxY) };

        public bool IsDirected { get; set; } = true;
        public bool IsMultigraph => IsDirected && Edges.Where(x => x.StartNode != null && x.EndNode != null).Count(x => Edges.FirstOrDefault(y => y.EndNode == x.StartNode)?.StartNode == x.EndNode) > 0;

        public BasicGraphModel Clone()
        {
            BasicGraphModel model = new BasicGraphModel();

            foreach (var node in this.Nodes)
            {
                model.Nodes.Add(new Node() { Id = node.Id, Name = node.Name, Pos = new Vector2(node.Pos.X, node.Pos.Y) });
            }
            foreach (var edge in this.Edges)
            {
                model.Edges.Add(new Edge() { Id = edge.Id, Weight = edge.Weight, EndNode = model.Nodes.First(x => x.Id == edge.EndNode.Id), StartNode = model.Nodes.First(x => x.Id == edge.StartNode.Id) });
            }
            foreach (var edge in model.Edges)
            {
                edge.EndNode.Edges.Add(edge);
                edge.StartNode.Edges.Add(edge);
            }
            return model;
        }
    }
}