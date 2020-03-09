using Frontenac.Blueprints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data
{
    public class GraphFactory
    {
        public BasicGraphModel ConvertToBasicGraph(IGraph iGraph)
        {
            var nodeCnt = 0;
            var posxdefault = 0;
            var posydefault = 0;
            var nodes = iGraph.GetVertices().Select(v => {
                var nameProp = v.GetProperty("name");
                var posxProp = v.GetProperty("posx");
                var posyProp = v.GetProperty("posy");
                var idProp = v.GetProperty("vgid");
                if (nameProp == null) nameProp = v.Id;
                if (posxProp == null) posxProp = nodeCnt++ % 3 == 0? posxdefault = 0: posxdefault += 10;
                if (posyProp == null) posyProp = nodeCnt++ % 3 == 0 ? posydefault += 10 : posydefault;

                return new Node
                {
                    Id = v.Id.ToString(),
                    Name = nameProp.ToString(),
                    Pos = new Vector2(Convert.ToSingle(posxProp.ToString()), Convert.ToSingle(posyProp.ToString()))
                };

            }).ToList();
            bool directedGraph = true;
            var edges = iGraph.GetEdges().Select(e => {
                var startnode = nodes.FirstOrDefault(n => e.GetVertex(Direction.Out).Id.ToString() == n.Id);
                var endnode = nodes.FirstOrDefault(n => e.GetVertex(Direction.In).Id.ToString() == n.Id);
                try { directedGraph = Convert.ToBoolean(e.GetProperty("isdirected")); } catch { }
                var edge = new Edge
                {
                    Id = e.GetProperty("vgid").ToString(),
                    StartNode = startnode,
                    EndNode = endnode,
                    Weight = Convert.ToDouble(e.GetProperty("weight"))
                };
                startnode.Edges.Add(edge);
                endnode.Edges.Add(edge);
                return edge;
            }).ToList();

            BasicGraphModel graph = new BasicGraphModel()
            {
                Nodes = nodes,
                Edges = edges,
                IsDirected = directedGraph,
            };

            return graph;
        }
    }
}
