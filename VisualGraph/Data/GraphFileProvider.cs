using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Frontenac.Blueprints;
using Frontenac.Blueprints.Impls.TG;
using Frontenac.Blueprints.Util.IO.GraphML;
using Frontenac.Gremlinq;
using Microsoft.AspNetCore.Mvc;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data
{

    internal static class GraphFileProvider
    {

        private static string _graphdir = Path.GetFullPath("./Graphs");
        private static GraphFactory _graphFactory = new GraphFactory();

        internal static string[] GetGraphFileNames()
        {
            return Directory.GetFiles(_graphdir).OrderByDescending( x=>x.ToLower()).ToArray();
        }
        internal static void EnsureGraphDirExists()
        {
            if (!Directory.Exists(_graphdir))
            {
                Directory.CreateDirectory(_graphdir);
            }
        }
        internal static List<BasicGraph> GetBasicGraphs()
        {
            List<BasicGraph> graphs = new List<BasicGraph>();

            foreach (var file in GetGraphFileNames())
            {
                    var graph = ReadGraphMlToBasicGraph(file);
                    graphs.Add(graph);
            }
            return graphs;
        }
        internal static BasicGraph ReadGraphMlToBasicGraph(string filename)
        {
            if (filename == String.Empty || !GetGraphFileNames().Contains(filename)) return null;            
            var graph = _graphFactory.ConverToBasicGraph(ReadGraphMl(filename));
            graph.Path = filename;
            return graph;   
        }
        internal static IGraph ReadGraphMl(string filename)
        {
            if (filename == String.Empty || !GetGraphFileNames().Contains(filename)) return null;

            using (StreamReader streamReader = new StreamReader(Path.Combine(_graphdir, filename)))
            {
                TinkerGrapĥ g = new TinkerGrapĥ();
                GraphMlReader.InputGraph(g, streamReader.BaseStream);
                return g;
            }
            
        }

        internal static void WriteToGraphMlFile(BasicGraph graph, string filename)
        {
            if (filename == String.Empty)
                return;
            
            TinkerGrapĥ tinkerGraph = new TinkerGrapĥ();
            foreach(var node in graph.Nodes)
            {
                var vertex = tinkerGraph.AddVertex<Node>(node);
            }
            foreach (var edge in graph.Edges)
            {
                tinkerGraph.AddEdge(edge.Id,tinkerGraph.GetVertex(edge.StartNode.Id),tinkerGraph.GetVertex(edge.EndNode.Id),"")
                .SetProperty("weight", edge.Weight);
            }
            GraphMlWriter writer = new GraphMlWriter(tinkerGraph);
            writer.OutputGraph(_graphdir + filename);
        }
    }
}