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

        internal static Task<string[]> GetGraphFileNames()
        {
            return Task.FromResult(Directory.GetFiles(_graphdir).Select(x => Path.GetFileNameWithoutExtension(x)).OrderByDescending(x => x.ToLower()).ToArray());
        }
        internal static Task EnsureGraphDirExists()
        {
            if (!Directory.Exists(_graphdir))
            {
                Directory.CreateDirectory(_graphdir);
            }
            return Task.CompletedTask;
        }
        internal static async Task<List<BasicGraphModel>> GetBasicGraphs()
        {
            List<BasicGraphModel> graphs = new List<BasicGraphModel>();

            foreach (var file in await GetGraphFileNames())
            {
                    var graph = await ReadGraphMlToBasicGraph(file);
                    graphs.Add(graph);
            }
            return graphs;
        }
        internal static async Task<BasicGraphModel> GetBasicGraph(string filename)
        {

                return await ReadGraphMlToBasicGraph(filename);
        }
        internal static async Task<BasicGraphModel> ReadGraphMlToBasicGraph(string filepath)
        {
            var filenames = await GetGraphFileNames();
            if (filepath == String.Empty || ! filenames.Contains(filepath)) 
                return null;
            
            if (!Path.IsPathFullyQualified(filepath))
                filepath = Path.Combine(_graphdir, filepath);
            var extension = Path.GetExtension(filepath);
            if (extension == string.Empty)
                filepath += ".xml";
            var tinkergraph = await ReadGraphMl(filepath);
            var graph = _graphFactory.ConvertToBasicGraph(tinkergraph);
            graph.Path = filepath;
            return graph;   
        }
        internal static async Task<TinkerGrapĥ> ReadGraphMl(string filepath)
        {
            var filenames = await GetGraphFileNames();
            if (filepath == String.Empty || !filenames.Any(f => filepath.Contains(f))) return null;

            using (StreamReader streamReader = new StreamReader(filepath))
            {
                TinkerGrapĥ g = new TinkerGrapĥ();
                GraphMlReader.InputGraph(g, streamReader.BaseStream);
                return g;
            }
            
        }

        internal static Task WriteToGraphMlFile(BasicGraphModel graph, string filename)
        {
            if (filename == String.Empty)
                return Task.FromException(new Exception("no filename was given"));
            filename = Path.Combine(_graphdir, filename + ".xml");
            TinkerGrapĥ tinkerGraph = new TinkerGrapĥ();
            
            foreach(var node in graph.Nodes)
            {
                var vertex = tinkerGraph.AddVertex((int)node.Id);
                vertex.SetProperty("posx", node.Pos.X);
                vertex.SetProperty("posy", node.Pos.Y);
                vertex.SetProperty("name", node.Name!=null?node.Name:" ");
                vertex.SetProperty("vgid", node.Id);
            }
            foreach (var edge in graph.Edges)
            {
                IVertex start = tinkerGraph.GetVertex((int)edge.StartNode.Id);
                IVertex end = tinkerGraph.GetVertex((int)edge.EndNode.Id);
                var edge_ = tinkerGraph.AddEdge((int)edge.Id, start, end , $"{edge.Weight}");
                edge_.SetProperty("weight", edge.Weight);
                edge_.SetProperty("vgid", edge.Id);
                edge_.SetProperty("isdirected", graph.IsDirectional);
            }
            GraphMlWriter writer = new GraphMlWriter(tinkerGraph);
            using (var fos = File.Open(filename, FileMode.Create))
            {
                writer.OutputGraph(fos);
            }
            return Task.CompletedTask;
        }
    }
}