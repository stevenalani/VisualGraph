using Frontenac.Blueprints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data
{
    public class GraphFactory
    {
        public BasicGraphModel ConverToBasicGraph(IGraph iGraph)
        {
            var nodeCnt = 0;
            var posxdefault = 0;
            var posydefault = 0;
            var nodes = iGraph.GetVertices().Select(v => {
                var nameProp = v.GetProperty("name");
                var posxProp = v.GetProperty("posx");
                var posyProp = v.GetProperty("posy");

                if (nameProp == null) nameProp = v.Id;
                if (posxProp == null) posxProp = nodeCnt++ % 3 == 0? posxdefault = 0: posxdefault += 10;
                if (posyProp == null) posyProp = nodeCnt++ % 3 == 0 ? posydefault += 10 : posydefault;

                return new Node
                {
                    Id = Convert.ToInt32(v.Id),
                    Name = nameProp.ToString(),
                    Pos = new Point2(Convert.ToDouble(posxProp.ToString()), Convert.ToDouble(posyProp.ToString()))
                };

            }).ToList();

            var edges = iGraph.GetEdges().Select(e => new Edge
            {
                Id = Convert.ToInt32(e.Id),
                StartNode = nodes.FirstOrDefault(n => Convert.ToInt32(e.GetVertex(Direction.Out).Id) == n.Id),
                EndNode = nodes.FirstOrDefault(n => Convert.ToInt32(e.GetVertex(Direction.In).Id) == n.Id),
                Weight = Convert.ToDouble(e.GetProperty("weight"))
            }).ToList();

            BasicGraphModel graph = new BasicGraphModel()
            {
                Nodes = nodes,
                Edges = edges,
            };

            return graph;
        }
    }
}
