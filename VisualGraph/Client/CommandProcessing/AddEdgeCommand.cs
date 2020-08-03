using System;
using System.Collections.Generic;
using System.Linq;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.CommandProcessing
{
    /// <summary>
    /// Dieser Befehl für Skripte und die Console weist den CommandProcessor an, eine Kante über Knoten IDs zum Graphen hinzuzufügen
    /// </summary>
    internal class AddEdgeCommand : GraphCommand
    {
        internal Action<string, string, double, BasicGraphModel> Action = new Action<string, string, double, BasicGraphModel>((nodeId1, nodeId2, weight, graph) =>
        {
            Node node = graph.Nodes.FirstOrDefault(n => n.Id == nodeId1);
            Node node1 = graph.Nodes.FirstOrDefault(n => n.Id == nodeId2);
            var edge = new Edge
            {
                StartNode = node,
                EndNode = node1,
                Id = graph.Edges.Count.ToString(),
                Weight = weight,
            };
            node.Edges.Add(edge);
            node1.Edges.Add(edge);
            graph.Edges.Add(edge);
        });
        public AddEdgeCommand()
        {
            Parameters = new Dictionary<Type, object[]>
            {
                { typeof(int), new object[2] },
                { typeof(double), new object[1] }
            };
        }

        internal override void Invoke(BasicGraphModel graph)
        {
            string n0id = (string)Parameters[typeof(string)][0];
            string n1id = (string)Parameters[typeof(string)][1];
            double weight = (double)Parameters[typeof(double)][0];
            Action.Invoke(n0id, n1id, weight, graph);
        }
    }
}