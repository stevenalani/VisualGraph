using Frontenac.Blueprints;
using Frontenac.Blueprints.Impls.TG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace VisualGraph.Shared.Models
{
    /// <summary>
    /// Führt Attribute von Online- Graphen mit BasicGraphModels zusammen
    /// </summary>
    public class BasicGraphToGraphMlMapping
    {
        /// <summary>
        /// Ergebnis Graph
        /// </summary>
        public BasicGraphModel BasicGraphModel { get; set; }
        /// <summary>
        /// Zuweisung der Fremden Attribute an BasicGraphModel- Attribute
        /// </summary>
        public AttributeMappings Mappings { get; set; } = new AttributeMappings();
        /// <summary>
        /// Heruntergeladener Graph
        /// </summary>
        public TinkerGrapĥ tinkerGraph { get; set; }
        /// <summary>
        /// Gefundene Knoten- Attribute
        /// </summary>
        public List<string> FoundNodeKeys { get; set; }
        /// <summary>
        /// Gefundene Kanten- Attribute
        /// </summary>
        public List<string> FoundEdgeKeys { get; set; }
        /// <summary>
        /// Findet alle Attribut- Schlüssel für Kanten und Knoten
        /// </summary>
        private void update()
        {
            FoundNodeKeys = tinkerGraph.GetVertices().SelectMany(x => x.GetPropertyKeys()).Distinct().ToList();
            FoundEdgeKeys = tinkerGraph.GetEdges().SelectMany(x => x.GetPropertyKeys()).Distinct().ToList();
        }
        /// <summary>
        /// Erstellt eine neue Instanz der Klasse.
        /// Wichtig für die JSON- Serialisierung
        /// </summary>
        public BasicGraphToGraphMlMapping(){}
        /// <summary>
        /// Erstellt eine neue Instanz der Klasse
        /// </summary>
        public BasicGraphToGraphMlMapping(TinkerGrapĥ loadedGraph, BasicGraphModel graph, AttributeMappings mappings)
        {
            tinkerGraph = loadedGraph;
            BasicGraphModel = graph;
            Mappings = mappings;
            update();
        }
        /// <summary>
        /// Erstellt eine neue, initialisierte Instanz der Klasse
        /// </summary>
        public BasicGraphToGraphMlMapping(TinkerGrapĥ loadedGraph)
        {
            tinkerGraph = loadedGraph;
            BasicGraphModel = new BasicGraphModel() { Name = "From-Web" };
            update();
        }
        /// <summary>
        /// Erstellt eine neue initialisierte Instanz der Klasse
        /// </summary>
        public BasicGraphToGraphMlMapping(TinkerGrapĥ loadedGraph, BasicGraphModel graph)
        {
            tinkerGraph = loadedGraph;
            BasicGraphModel = graph;
            update();
        }

        /// <summary>
        /// Führt das Mapping aus, nachdem den Beutzer alle fremden Attribute an die
        /// anwendungsinternen Attribute gebunden hat
        /// </summary>
        public void ExecuteMappingOfValues()
        {
            bool isDirected = true;
            BasicGraphModel.Nodes = tinkerGraph.GetVertices().Select(x =>
            {
                string id;
                int idFix = 0;
                string name;
                int nameFix = 0;
                float posx;
                float posy;
                if (Mappings["Node"]["Id"] != "")
                {
                    var value = x.GetProperty(Mappings["Node"]["Id"])?.ToString();
                    id = value ?? "vgn_id" + idFix++.ToString();
                }
                else
                {
                    id = x.Id.ToString();
                }
                if (Mappings["Node"]["Name"] != "")
                {
                    var value = x.GetProperty(Mappings["Node"]["Name"])?.ToString();
                    name = value ?? "vgn_name" + nameFix++.ToString();
                }
                else
                {
                    name = x.Id.ToString();
                }
                if (Mappings["Node"]["Posx"] != "")
                {
                    var value = x.GetProperty(Mappings["Node"]["Posx"])?.ToString();
                    posx = value != null ? Convert.ToSingle(value) : 0.0f;
                }
                else
                {
                    posx = 0.0f;
                }
                if (Mappings["Node"]["Posy"] != "")
                {
                    var value = x.GetProperty(Mappings["Node"]["Posy"])?.ToString();
                    posy = value != null ? Convert.ToSingle(value) : 0.0f;
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
            BasicGraphModel.Edges = tinkerGraph.GetEdges().Select(x =>
            {
                string id;
                int idFix = 0;
                float weight;
                if (Mappings["Edge"]["Id"] != "")
                {
                    var value = x.GetProperty(Mappings["Edge"]["Id"])?.ToString();
                    id = value ?? "vge_" + idFix++.ToString();
                }
                else
                {
                    id = x.Id.ToString();
                }
                if (Mappings["Edge"]["Weight"] != "")
                {
                    var value = x.GetProperty(Mappings["Edge"]["Weight"])?.ToString();
                    weight = value != null ? Convert.ToSingle(value) : 0.0f;
                }
                else
                {
                    weight = 0.0f;
                }
                if (Mappings["Edge"]["IsDirected"] != "")
                {
                    var value = x.GetProperty(Mappings["Edge"]["IsDirected"])?.ToString();
                    isDirected = value != null ? Convert.ToBoolean(value) : true;
                }
                else
                {
                    isDirected = true;
                }
                var startnode = BasicGraphModel.Nodes.FirstOrDefault(n => x.GetVertex(Direction.Out).Id.ToString() == n.Id);
                var endnode = BasicGraphModel.Nodes.FirstOrDefault(n => x.GetVertex(Direction.In).Id.ToString() == n.Id);
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
}
