using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data
{
    public class GraphCommandProcessor
    {
        public GraphCommandProcessor(BasicGraphModel model)
        {
            this.model = model;
        }
        private BasicGraphModel model { get; set; }
        private GraphCommandInterpreter graphCommandInterpreter = new GraphCommandInterpreter();
        public string Process(string input)
        {
            try
            {
                var command = graphCommandInterpreter.InterpretCommand(input);
                if (command == null) return $"No command was not found in {input}";
                graphCommandInterpreter.InterpretAndSetCommandParameters(command, input);
                command.Invoke(model);
                return "Done: " + input;
            }
            catch
            {
                return $"there went something wrong! please check your input: {input}";
            }
        }
    }
    public class GraphCommandInterpreter
    {
        private Dictionary<string, Dictionary<string, Type>> BaseKeywords = new Dictionary<string, Dictionary<string, Type>>()
        {
            { "add", new Dictionary<string, Type>()
                {
                    { "node", typeof(AddNodeCommand) },
                    { "edgebyids", typeof(AddEdgeCommand) },
                    { "edgebynames", typeof(AddEdgeByNamesCommand) },
                }
            },
            { "del", new Dictionary<string, Type>()
                {
                    {"node",typeof(RemoveNodeCommand)},
                    {"edgebyids",typeof(RemoveEdgeCommand)},
                    {"edgebynames",typeof(RemoveEdgeCommand)},
                }
            },
        };
        public GraphCommand InterpretCommand(string inputstring)
        {
            Regex regex = new Regex(@"(\b[A-z]+\b[^,])");
            var match = regex.Matches(inputstring);
            if (match.Count > 0)
            {

                var commandType = BaseKeywords[match[0].Value.Trim().ToLower()][match[1].Value.Trim().ToLower()];
                dynamic instance = commandType.Assembly.CreateInstance(commandType.FullName);
                return instance;
            }
            return null;
        }
        public void InterpretAndSetCommandParameters(GraphCommand command, string inputstring)
        {
            Regex regex = new Regex(@"(\b[A-z]+\b[^,])");
            var matches = regex.Matches(inputstring);
            if (matches.Count > 0)
            {
                var parameters = inputstring.Replace(matches[0].Value + matches[1].Value, "").Split(',').ToList<string>();
                for (int i = 0; i < command.Parameters.Count; i++)
                {
                    var paramKey = command.Parameters.ElementAt(i).Key;
                    for (int j = 0; j < command.Parameters[paramKey].Length; j++)
                    {
                        var currentparameter = parameters.Take(1).First();
                        if (paramKey == typeof(int))
                        {
                            command.Parameters[paramKey][j] = Convert.ToInt32(currentparameter);

                        }
                        else if (paramKey == typeof(double))
                        {
                            command.Parameters[paramKey][j] = Convert.ToDouble(currentparameter.Replace('.', ','));
                        }
                        else
                        {
                            command.Parameters[paramKey][j] = currentparameter;
                        }
                        parameters.RemoveAt(0);
                    }
                }
            }
        }
    }

    internal class AddEdgeCommand : GraphCommand
    {
        public Action<int, int, double, BasicGraphModel> Action = new Action<int, int, double, BasicGraphModel>((n0, n1, w, g) =>
        {
            Node node = g.Nodes.FirstOrDefault(n => n.Id == n0);
            Node node1 = g.Nodes.FirstOrDefault(n => n.Id == n1);
            var edge = new Edge
            {
                StartNode = node,
                EndNode = node1,
                Id = g.Edges.Count > 0 ? g.Edges.Max(e => e.Id) + 1 : 1,
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
            int n0id = (int)Parameters[typeof(int)][0];
            int n1id = (int)Parameters[typeof(int)][1];
            double weight = (double)Parameters[typeof(double)][0];
            Action.Invoke(n0id, n1id, weight, g);
        }
    }
    internal class AddEdgeByNamesCommand : AddEdgeCommand
    {
        public new Action<int, int, double, BasicGraphModel> Action = new Action<int, int, double, BasicGraphModel>((n0, n1, w, g) =>
        {
            Node node = g.Nodes.FirstOrDefault(n => n.Id == n0);
            Node node1 = g.Nodes.FirstOrDefault(n => n.Id == n1);
            var edge = new Edge
            {
                StartNode = node,
                EndNode = node1,
                Id = g.Edges.Count > 0 ? g.Edges.Max(e => e.Id) + 1 : 1,
                Weight = w,
            };
            node.Edges.Add(edge);
            node1.Edges.Add(edge);
            g.Edges.Add(edge);
        });
        public AddEdgeByNamesCommand()
        {
            Parameters = new Dictionary<Type, object[]>
            {
                { typeof(string), new object[2] },
                { typeof(double), new object[1] }
            };
        }

        public override void Invoke(BasicGraphModel g)
        {
            string n0name = Parameters[typeof(string)][0].ToString().Trim();
            string n1name = Parameters[typeof(string)][1].ToString().Trim();

            int n0id = g.Nodes.First(x => x.Name == n0name).Id;
            int n1id = g.Nodes.First(x => x.Name == n1name).Id;
            double weight = (double)Parameters[typeof(double)][0];
            Action.Invoke(n0id, n1id, weight, g);
        }
    }
    internal class AddNodeCommand : GraphCommand
    {
        public Action<string, double, double, BasicGraphModel> Action = new Action<string, double, double, BasicGraphModel>((n, x, y, g) =>
        {
            Node newnode = new Node
            {
                Name = n,
                Pos = new Vector2((float)x, (float)y),
                Id = g.Nodes.Count > 0 ? g.Nodes.Max(nn => nn.Id) + 1 : 0,
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
            Node node0 = g.Nodes.First(n => n.Id == (int)Parameters[typeof(int)][0]);
            Node node1 = g.Nodes.First(n => n.Id == (int)Parameters[typeof(int)][1]);
            Action.Invoke(node0, node1, g);
        }
    }
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

    abstract public class GraphCommand
    {
        public abstract void Invoke(BasicGraphModel g);
        internal Dictionary<Type, object[]> Parameters;
    }
}
