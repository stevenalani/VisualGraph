using System.Collections.Generic;
using System.Linq;

namespace VisualGraph.Shared.Models
{
    /// <summary>
    /// Model Klasse für API- Aufrufe/Antworten
    /// </summary>
    public class BasicGraphModelPoco : BasicGraphModel
    {
        public BasicGraphModelPoco()
        {
        }
        public BasicGraphModelPoco(BasicGraphModel x)
        {
            this.NodesPoco = x.Nodes.Select(y => new NodePoco(y)).ToList();
            this.Nodes = x.Nodes;
            this.Edges = x.Edges;
            this.Name = x.Name;
            this.IsDirected = x.IsDirected;
        }

        public List<NodePoco> NodesPoco { get; set; } = new List<NodePoco>();
    }
}