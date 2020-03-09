using Frontenac.Blueprints;
using Frontenac.Blueprints.Impls.TG;
using Microsoft.Msagl.Core.Layout.ProximityOverlapRemoval.ConjugateGradient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace VisualGraph.Data.Additional.Models
{
    internal class BasicGraphToGraphMlMapping : Tuple<BasicGraphModel, Mappings>
    {
        public BasicGraphModel BasicGraph => Item1;
        public Mappings Mappings => Item2;

        private TinkerGrapĥ tinkerGraph;

        public List<string> FoundNodeKeys => tinkerGraph.GetVertices().SelectMany( x => x.GetPropertyKeys()).Distinct().ToList();
        public List<string> FoundEdgeKeys => tinkerGraph.GetEdges().SelectMany(x => x.GetPropertyKeys()).Distinct().ToList();
        public BasicGraphToGraphMlMapping(TinkerGrapĥ loadedGraph, BasicGraphModel graph, Mappings mappings) : base(graph, mappings)
        {
            tinkerGraph = loadedGraph;
        }
        public BasicGraphToGraphMlMapping(TinkerGrapĥ loadedGraph) : base(new BasicGraphModel(), new Mappings())
        {
            tinkerGraph = loadedGraph;
        }
        public BasicGraphToGraphMlMapping(TinkerGrapĥ loadedGraph, BasicGraphModel graph):base(graph,new Mappings())
        {
            tinkerGraph = loadedGraph;
        }


        public void ExecuteMappingOfValues()
        {
            bool isDirected = true;
            BasicGraph.Nodes = tinkerGraph.GetVertices().Select(x => {
                string id;
                int idFix = 0;
                string name;
                int nameFix = 0;
                float posx;
                float posy;
                if (Mappings["Node"]["Id"] != "")
                {
                    var value = x.GetProperty(this.Mappings["Node"]["Id"])?.ToString();
                    id = value?? "vgn_id"+(idFix++).ToString();
                }
                else
                {
                    id = x.Id.ToString();
                }
                if (Mappings["Node"]["Name"] != "")
                {
                    var value = x.GetProperty(this.Mappings["Node"]["Name"])?.ToString();
                    name = value ?? "vgn_name" + (nameFix++).ToString();
                }
                else
                {
                    name = x.Id.ToString();
                }
                if (Mappings["Node"]["Posx"] != "")
                {
                    var value = x.GetProperty(this.Mappings["Node"]["Posx"])?.ToString();
                    posx = value != null ? Convert.ToSingle(value) : 0.0f;
                }
                else
                {
                    posx = 0.0f;
                }
                if (Mappings["Node"]["Posy"] != "")
                {
                    var value = x.GetProperty(this.Mappings["Node"]["Posy"])?.ToString();
                    posy = value != null ? Convert.ToSingle(value): 0.0f;
                }
                else
                {
                    posy = 0.0f;
                }
                return new Node
                {
                    Id = id,
                    Name = name,
                    Pos = new Vector2(posx, posy)
                };
            }).ToList();
            BasicGraph.Edges = tinkerGraph.GetEdges().Select(x => {
                string id;
                int idFix = 0;
                float weight;
                if (Mappings["Edge"]["Id"] != "")
                {
                    var value = x.GetProperty(this.Mappings["Edge"]["Id"])?.ToString();
                    id = value ?? "vge_" +(idFix++).ToString();
                }
                else
                {
                    id = x.Id.ToString();
                }
                if (Mappings["Edge"]["Weight"] != "")
                {
                    var value = x.GetProperty(this.Mappings["Edge"]["Weight"])?.ToString();
                    weight = value != null?Convert.ToSingle(value): 0.0f;
                }
                else
                {
                    weight = 0.0f;
                }
                if (Mappings["Edge"]["IsDirected"] != "")
                {
                    var value = x.GetProperty(this.Mappings["Edge"]["IsDirected"])?.ToString();
                    isDirected = value != null?Convert.ToBoolean(value):true;
                }
                else
                {
                    isDirected = true;
                }
                var startnode = BasicGraph.Nodes.FirstOrDefault(n => x.GetVertex(Direction.Out).Id.ToString() == n.Id);
                var endnode = BasicGraph.Nodes.FirstOrDefault(n => x.GetVertex(Direction.In).Id.ToString() == n.Id);
                var edge = new Edge()
                {
                    Id = id,
                    Weight = weight,
                    StartNode = startnode,
                    EndNode = endnode
                };
                startnode.Edges.Add(edge);
                endnode.Edges.Add(edge);
                return edge;
            }).ToList();
        }
    }
    internal class Mappings : Dictionary<string, Dictionary<string, string>>
    {
        public Mappings() : base()
        {
            Add("Node", new Dictionary<string, string>() {
                {"Id","" },
                {"Name","" },
                {"Posx","" },
                {"Posy","" }
            });
            Add("Edge", new Dictionary<string, string>() {
                {"Id","" },
                {"Weight","" },
                {"IsDirected","" },
            });
        }
    }
}
