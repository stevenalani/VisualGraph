using System;
using System.Collections.Generic;
using System.Linq;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.CommandProcessing
{
    /// <summary>
    /// Dieser Befehl für Skripte und die Console weist den CommandProcessor an, eine Kante zum Graphen hinzuzufügen
    /// </summary>
    internal class AddEdgeByNamesCommand : AddEdgeCommand
    {
        internal new Action<string, string, double, BasicGraphModel> Action = new Action<string, string, double, BasicGraphModel>((nodename1, nodename2, weight, graph) =>
        {
            Node _node = graph.Nodes.FirstOrDefault(n => n.Id == nodename1);
            Node _node1 = graph.Nodes.FirstOrDefault(n => n.Id == nodename2);
            var edge = new Edge
            {
                StartNode = _node,
                EndNode = _node1,
                Id = graph.Edges.Count.ToString(),
                Weight = weight,
            };
            _node.Edges.Add(edge);
            _node1.Edges.Add(edge);
            graph.Edges.Add(edge);
        });
        public AddEdgeByNamesCommand()
        {
            Parameters = new Dictionary<Type, object[]>
            {
                { typeof(string), new object[2] },
                { typeof(double), new object[1] }
            };
        }

        internal override void Invoke(BasicGraphModel graph)
        {
            string n0name = Parameters[typeof(string)][0].ToString().Trim();
            string n1name = Parameters[typeof(string)][1].ToString().Trim();

            string n0id = graph.Nodes.First(x => x.Name == n0name).Id;
            string n1id = graph.Nodes.First(x => x.Name == n1name).Id;
            double weight = (double)Parameters[typeof(double)][0];
            Action.Invoke(n0id, n1id, weight, graph);
        }
    }
}
