using System;
using System.Collections.Generic;
using System.Numerics;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.CommandProcessing
{
    internal class AddNodeCommand : GraphCommand
    {
        public Action<string, double, double, BasicGraphModel> Action = new Action<string, double, double, BasicGraphModel>((n, x, y, g) =>
        {
            Node newnode = new Node
            {
                Name = n,
                Pos = new Vector2((float)x, (float)y),
                Id = g.Nodes.Count.ToString(),
            };
            g.Nodes.Add(newnode);
        });
        public AddNodeCommand()
        {
            Parameters = new Dictionary<Type, object[]>
            {
                { typeof(string), new object[1] },
                { typeof(double), new object[2] }
            };
        }

        public override void Invoke(BasicGraphModel g)
        {
            string n = (string)Parameters[typeof(string)][0];
            double x = (double)Parameters[typeof(double)][0];
            double y = (double)Parameters[typeof(double)][1];
            Action.Invoke(n, x, y, g);
        }
    }
}