using System;
using System.Collections.Generic;
using System.Linq;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data.CommandProcessing
{
    internal class RemoveEdgeCommand : GraphCommand
    {
        public Action<Node, Node, BasicGraphModel> Action = new Action<Node, Node, BasicGraphModel>((n0, n1, g) =>
        {
            Edge edge = n0.Edges.First(x => x.EndNode == n1 || x.StartNode == n1);
            g.Edges.Remove(edge);
        });
        public RemoveEdgeCommand()
        {
            Parameters = new Dictionary<Type, object[]>
            {
                { typeof(int), new object[2] }
            };
        }
        public override void Invoke(BasicGraphModel g)
        {
            Node node0 = g.Nodes.First(n => n.Id == (string)Parameters[typeof(string)][0]);
            Node node1 = g.Nodes.First(n => n.Id == (string)Parameters[typeof(string)][1]);
            Action.Invoke(node0, node1, g);
        }
    }
}