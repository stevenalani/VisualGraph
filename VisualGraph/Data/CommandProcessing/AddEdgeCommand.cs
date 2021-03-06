﻿using System;
using System.Collections.Generic;
using System.Linq;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data.CommandProcessing
{
    internal class AddEdgeCommand : GraphCommand
    {
        public Action<string, string, double, BasicGraphModel> Action = new Action<string, string, double, BasicGraphModel>((n0, n1, w, g) =>
        {
            Node node = g.Nodes.FirstOrDefault(n => n.Id == n0);
            Node node1 = g.Nodes.FirstOrDefault(n => n.Id == n1);
            var edge = new Edge
            {
                StartNode = node,
                EndNode = node1,
                Id = g.Edges.Count.ToString(),
                Weight = w,
            };
            node.Edges.Add(edge);
            node1.Edges.Add(edge);
            g.Edges.Add(edge);
        });
        public AddEdgeCommand()
        {
            Parameters = new Dictionary<Type, object[]>
            {
                { typeof(int), new object[2] },
                { typeof(double), new object[1] }
            };
        }

        public override void Invoke(BasicGraphModel g)
        {
            string n0id = (string)Parameters[typeof(string)][0];
            string n1id = (string)Parameters[typeof(string)][1];
            double weight = (double)Parameters[typeof(double)][0];
            Action.Invoke(n0id, n1id, weight, g);
        }
    }
}