using System;
using System.Linq;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.CommandProcessing
{
    internal class RemoveNodeCommand : GraphCommand
    {
        public Action<Node, Node, BasicGraphModel, double> Action = new Action<Node, Node, BasicGraphModel, double>((n0, n1, g, w) =>
        {
            var edge = new Edge
            {
                StartNode = n0,
                EndNode = n1,
                Id = g.Edges.Max(e => e.Id) + 1,
                Weight = w,
            };
            n0.Edges.Add(edge);
            n1.Edges.Add(edge);
            g.Edges.Add(edge);
        });

        public override void Invoke(BasicGraphModel g)
        {
            throw new NotImplementedException();
        }
    }
}
