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
            var filenames = Directory.GetFiles(_graphdir).Select(x => Path.GetFileNameWithoutExtension(x)).OrderByDescending(x => x.ToLower()).ToArray();
            return filenames;
        }
        internal static void EnsureGraphDirExists()
        {
            if (!Directory.Exists(_graphdir))
            {
                Directory.CreateDirectory(_graphdir);
            }
        }
        internal static List<BasicGraphModel> GetBasicGraphs()
        {
            List<BasicGraphModel> graphs = new List<BasicGraphModel>();

            foreach (var file in GetGraphFileNames())
            {
                    var graph = ReadGraphMlToBasicGraph(file);
                    graphs.Add(graph);
            }
            return graphs;
        }
        internal static BasicGraphModel ReadGraphMlToBasicGraph(string filepath)
        {
            if (filepath == String.Empty || !GetGraphFileNames().Contains(filepath)) 
                return null;
            
            if (!Path.IsPathFullyQualified(filepath))
                filepath = Path.Combine(_graphdir, filepath);
            var extension = Path.GetExtension(filepath);
            if (extension == string.Empty)
                filepath += ".xml";
            var tinkergraph = ReadGraphMl(filepath);
            var graph = _graphFactory.ConvertToBasicGraph(tinkergraph);
            graph.Path = filepath;
            return graph;   
        }
        internal static IGraph ReadGraphMl(string filepath)
        {
            if (filepath == String.Empty || !GetGraphFileNames().Any(f => filepath.Contains(f))) return null;

            using (StreamReader streamReader = new StreamReader(filepath))
            {
                TinkerGrapĥ g = new TinkerGrapĥ();
                GraphMlReader.InputGraph(g, streamReader.BaseStream);
                return g;
            }
            
        }

        internal static void WriteToGraphMlFile(BasicGraphModel graph, string filename)
        {
            if (filename == String.Empty)
                return;
            filename = Path.Combine(_graphdir, filename + ".xml");
            TinkerGrapĥ tinkerGraph = new TinkerGrapĥ();
            foreach(var node in graph.Nodes)
            {
                var vertex = tinkerGraph.AddVertex(null);
                vertex.SetProperty("posx", node.Pos.X);
                vertex.SetProperty("posy", node.Pos.Y);
                vertex.SetProperty("name", node.Name);
            }
            foreach (var edge in graph.Edges)
            {
                IVertex start = tinkerGraph.GetVertex(edge.StartNode.Id);
                IVertex end = tinkerGraph.GetVertex(edge.EndNode.Id);
                var edge_ = tinkerGraph.AddEdge(edge.Id, start, end , $"{edge.Weight}");
                edge_.SetProperty("weight", edge.Weight);
            }
            GraphMlWriter writer = new GraphMlWriter(tinkerGraph);
            using (var fos = File.Open(filename, FileMode.Create))
            {
                writer.OutputGraph(fos);
            }
                
        }
    }
}