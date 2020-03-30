using Frontenac.Blueprints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data
{
    public static class GraphFactory
    {
        
        public static BasicGraphModel ConvertToBasicGraph(IGraph iGraph)
        {
            var nodeCnt = 0;
            var posxdefault = 0;
            var posydefault = 0;
            var nodes = iGraph.GetVertices().Select(v =>
            {
                var nameProp = v.GetProperty("name");
                var posxProp = v.GetProperty("posx");
                var posyProp = v.GetProperty("posy");
                var idProp = v.GetProperty("vgid");
                if (nameProp == null) nameProp = v.Id;
                if (posxProp == null) posxProp = nodeCnt++ % 3 == 0 ? posxdefault = 0 : posxdefault += 10;
                if (posyProp == null) posyProp = nodeCnt++ % 3 == 0 ? posydefault += 10 : posydefault;

                return new Node
                {
                    Id = v.Id.ToString(),
                    Name = nameProp.ToString(),
                    Pos = new Vector2(Convert.ToSingle(posxProp.ToString()), Convert.ToSingle(posyProp.ToString()))
                };

            }).ToList();
            bool directedGraph = true;
            var edges = iGraph.GetEdges().Select(e =>
            {
                var startnode = nodes.FirstOrDefault(n => e.GetVertex(Direction.Out).Id.ToString() == n.Id);
                var endnode = nodes.FirstOrDefault(n => e.GetVertex(Direction.In).Id.ToString() == n.Id);
                try { directedGraph = Convert.ToBoolean(e.GetProperty("isdirected")); } catch { }
                var edge = new Edge
                {
                    Id = e.Id.ToString(),
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
        public static BasicGraphModel CreateNewGraphModel()
        {
            var nodes = new System.Collections.Generic.List<Data.Additional.Models.Node>{
                new Data.Additional.Models.Node() {
                    Id = "0",
                    Pos = new System.Numerics.Vector2(-10,10),
                    Name = "Knoten A"
                },
                new Data.Additional.Models.Node() {
                    Id = "1",
                    Pos = new System.Numerics.Vector2(10,-10),
                    Name = "Knoten B"

                }

            };
            var graphmodel = new BasicGraphModel()
            {
                Nodes = nodes,
                Name = "Unbenannter-Graph"
            };
            return graphmodel;
        }
        public static BasicGraphModel CreateRandomGraph(string Graphname, int nodeCount, int edgeCount =  35, double UpperBound = 50, double LowerBound = -50)
        {
            Random random = new Random();
            BasicGraphModel GraphModel = new BasicGraphModel() { Name = Graphname };
            
            for (int i = 0; i < nodeCount; i++)
            {
                GraphModel.Nodes.Add(new Node()
                {
                    Id = i.ToString(),
                    Name = i.ToString(),
                    Pos = new Vector2((float)(random.NextDouble() * (UpperBound - LowerBound) + LowerBound), (float)(random.NextDouble() * (UpperBound - LowerBound) + LowerBound)),
                });
            }
            for (int i = 0; i < edgeCount; i++)
            {
                Node node1,node2;
                node1 = GraphModel.Nodes.FirstOrDefault(x => x.Edges.Count == 0);
                node2 = GraphModel.Nodes.FirstOrDefault(x => x.Edges.Count == 0 && x != node1);
                if(node1 == null)
                {
                   var index = random.Next(0, nodeCount - 1);
                   node1 = GraphModel.Nodes[index];
                }
                if (node2 == null)
                {
                    var index = random.Next(0, nodeCount - 1);
                    node2 = GraphModel.Nodes[index];
                }

                Edge edge = new Edge() { StartNode = node1, EndNode = node2, Id = (i + 1).ToString(), Weight = random.NextDouble() * 10  };
                node1.Edges.Add(edge);
                node2.Edges.Add(edge);
                GraphModel.Edges.Add(edge);
            }
            return GraphModel;
        }
    }
}
