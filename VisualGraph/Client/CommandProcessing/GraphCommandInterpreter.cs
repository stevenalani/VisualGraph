using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace VisualGraph.Client.CommandProcessing
{
    /// <summary>
    /// Diese Klasse findet definierte Befehle in einem String und setzt im String gefundene Parameter.
    /// </summary>
    internal class GraphCommandInterpreter
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
            // TODO: Implement and define Remove Commands
            /*{ "del", new Dictionary<string, Type>()
                {
                    {"node",typeof(RemoveNodeCommand)},
                    {"edgebyids",typeof(RemoveEdgeCommand)},
                    {"edgebynames",typeof(RemoveEdgeCommand)},
                }
            },*/
        };
        internal GraphCommand InterpretCommand(string inputstring)
        {
            Regex regex = new Regex(@"(\b[A-z]+\b[^,])");
            var match = regex.Matches(inputstring);
            if (match.Count > 0)
            {
                var commandType = BaseKeywords[match[0].Value.Trim().ToLower()][match[1].Value.Trim().ToLower()];
                Console.WriteLine($"regex 1: {match[0].Value.Trim().ToLower()}");
                Console.WriteLine($"regex 2: {match[1].Value.Trim().ToLower()}");
                Console.WriteLine($"Name: {commandType.FullName}");
                try
                {
                    dynamic instance = commandType.Assembly.CreateInstance(commandType.FullName);
                    return instance;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
                
            }
            return null;
        }
        internal void InterpretAndSetCommandParameters(GraphCommand command, string inputstring)
        {
            Regex regex = new Regex(@"(\b[A-z]+\b[^,])");
            var matches = regex.Matches(inputstring);
            if (matches.Count > 0)
            {
                var parameters = inputstring.Replace(matches[0].Value + matches[1].Value, "").Split(',').ToList();
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
}