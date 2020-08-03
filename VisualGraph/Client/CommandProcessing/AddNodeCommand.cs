using System;
using System.Collections.Generic;
using System.Numerics;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.CommandProcessing
{
    /// <summary>
    /// Dieser Befehl fügt dem Graph einen Knoten hinzu.
    /// </summary>
    internal class AddNodeCommand : GraphCommand
    {
        internal Action<string, double, double, BasicGraphModel> Action = new Action<string, double, double, BasicGraphModel>((name, posX, posY, graph) =>
        {
            Node newnode = new Node
            {
                Name = name,
                Pos = new Vector2((float)posX, (float)posY),
                Id = graph.Nodes.Count.ToString(),
            };
            graph.Nodes.Add(newnode);
        });
        public AddNodeCommand()
        {
            Parameters = new Dictionary<Type, object[]>
            {
                { typeof(string), new object[1] },
                { typeof(double), new object[2] }
            };
        }

        internal override void Invoke(BasicGraphModel g)
        {
            string n = (string)Parameters[typeof(string)][0];
            double x = (double)Parameters[typeof(double)][0];
            double y = (double)Parameters[typeof(double)][1];
            Action.Invoke(n, x, y, g);
        }
    }
}