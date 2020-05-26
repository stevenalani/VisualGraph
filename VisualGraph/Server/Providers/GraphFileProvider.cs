using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Frontenac.Blueprints;
using Frontenac.Blueprints.Impls.TG;
using Frontenac.Blueprints.Util.IO.GraphML;
using Frontenac.Gremlinq;
using VisualGraph.Shared.Models;
using VisualGraph.Shared;

namespace VisualGraph.Server.Providers
{

    internal static class GraphFileProvider
    {
        private static string _graphMlHeader = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
        private static string _graphMlClosingTag = "</graphml>";
        private static string _graphdir = Path.GetFullPath("./Graphs");

        internal static Task<string[]> GetGraphFileNames()
        {
            return Task.FromResult(Directory.GetFiles(_graphdir, "*.xml", SearchOption.AllDirectories).Select(x => Path.GetFileNameWithoutExtension(x)).OrderByDescending(x => x.ToLower()).ToArray());
        }
        internal static Task<string[]> GetUserGraphFileNames(string username)
        {
            return Task.FromResult(Directory.GetFiles(Path.Combine(username, _graphdir), "*.xml", SearchOption.AllDirectories).Select(x => Path.GetFileNameWithoutExtension(x)).OrderByDescending(x => x.ToLower()).ToArray());
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
            if (filepath == string.Empty || !filenames.Contains(filepath))
                return null;

            if (!Path.IsPathFullyQualified(filepath))
                filepath = Path.Combine(_graphdir, filepath);
            var extension = Path.GetExtension(filepath);
            if (extension == string.Empty)
                filepath += ".xml";
            var tinkergraph = await ReadGraphMl(filepath);
            var graph = GraphFactory.ConvertToBasicGraph(tinkergraph);
            graph.Name = Path.GetFileNameWithoutExtension(filepath);
            return graph;
        }
        internal static async Task<TinkerGrapĥ> ReadGraphMl(string filepath)
        {
            var filenames = await GetGraphFileNames();
            if (filepath == string.Empty || !filenames.Any(f => filepath.Contains(f))) return null;

            using (StreamReader streamReader = new StreamReader(filepath))
            {
                TinkerGrapĥ g = new TinkerGrapĥ();
                GraphMlReader.InputGraph(g, streamReader.BaseStream);
                return g;
            }

        }

        internal static Task<bool> WriteToGraphMlFile(BasicGraphModel graph, string filename)
        {
            if (filename == string.Empty)
                return Task.FromResult(false);
            filename = Path.Combine(_graphdir, filename + ".xml");
            TinkerGrapĥ tinkerGraph = new TinkerGrapĥ();

            foreach (var node in graph.Nodes)
            {
                var vertex = tinkerGraph.AddVertex(node.Id);
                vertex.SetProperty("posx", node.Pos.X);
                vertex.SetProperty("posy", node.Pos.Y);
                vertex.SetProperty("name", node.Name != null ? node.Name : " ");
                vertex.SetProperty("vgid", node.Id);
            }
            foreach (var edge in graph.Edges)
            {
                IVertex start = tinkerGraph.GetVertex(edge.StartNode.Id);
                IVertex end = tinkerGraph.GetVertex(edge.EndNode.Id);
                var edge_ = tinkerGraph.AddEdge(edge.Id, start, end, $"{edge.Weight}");
                edge_.SetProperty("weight", edge.Weight);
                edge_.SetProperty("vgid", edge.Id);
                edge_.SetProperty("isdirected", graph.IsDirected);
            }
            GraphMlWriter writer = new GraphMlWriter(tinkerGraph);
            using (var fos = File.Open(filename, FileMode.Create))
            {
                writer.OutputGraph(fos);
            }
            return Task.FromResult(true);
        }
        internal static async Task<BasicGraphToGraphMlMapping> LoadBasicGraphFromUrl(string url, string graphname)
        {
            if (!graphname.Contains(".xml"))
                graphname += ".xml";
            var tmpgraphpath = Path.Combine(_graphdir, "tmp_" + graphname);
            var graphpath = Path.Combine(_graphdir, graphname);
            using (var client = new WebClient())
            {
                var proxyConfig = VGAppSettings.RemoteRequestProxy;
                if (proxyConfig != "")
                {
                    client.Proxy = new WebProxy(proxyConfig);
                }
                client.DownloadFile(url, tmpgraphpath);
            }
            await IsolateGraphInFile(tmpgraphpath, graphpath);
            var tinkerGraph = await ReadGraphMl(graphpath);
            BasicGraphModel graph = new BasicGraphModel();
            graph.Name = Path.GetFileNameWithoutExtension(graphpath);
            BasicGraphToGraphMlMapping graphToGraphMlMapping = new BasicGraphToGraphMlMapping(tinkerGraph, graph);
            File.Delete(tmpgraphpath);
            return graphToGraphMlMapping;
        }
        private static async Task IsolateGraphInFile(string tmpfilepath, string filepath)
        {

            var fileContent = File.ReadAllText(tmpfilepath);

            fileContent = fileContent.Replace("&lt;", "<").Replace("&gt;", ">");
            var graphbeginning = fileContent.IndexOf(_graphMlHeader);
            fileContent = fileContent.Remove(0, graphbeginning);
            var graphend = fileContent.IndexOf(_graphMlClosingTag);
            fileContent = fileContent.Remove(graphend + _graphMlClosingTag.Length);
            fileContent = await ensureEdgeIdsAreSet(fileContent);
            fileContent = await ensureEdgeLabelsAreSet(fileContent);
            fileContent = await ensureEdgedefaultIsDirected(fileContent);
            fileContent = await ensureEndtagsAreSet(fileContent, "edge");

            File.WriteAllText(filepath, fileContent);
        }
        private static Task<string> ensureEdgeIdsAreSet(string fileContent)
        {
            string regex = "<edge .*id=\".*/>";
            if (!Regex.IsMatch(fileContent, regex))
            {
                int id = 0;
                var result = Regex.Replace(fileContent, "edge ", m => string.Format("{0}id=\"{1}\" ", m.Value, id++), RegexOptions.IgnoreCase);
                return Task.FromResult(result);
            }
            return Task.FromResult(fileContent);
        }
        private static Task<string> ensureEdgeLabelsAreSet(string fileContent)
        {
            string regex = "<edge .*label=\".*/>";
            if (!Regex.IsMatch(fileContent, regex))
            {
                int labelno = 0;
                var result = Regex.Replace(fileContent, "target=\".+\"", m => string.Format("{0} label=\"{1}\"", m.Value, labelno++), RegexOptions.IgnoreCase);
                return Task.FromResult(result);
            }
            return Task.FromResult(fileContent);
        }
        private static Task<string> ensureEdgedefaultIsDirected(string fileContent)
        {
            string regex = "edgedefault=\"undirected\"";
            if (Regex.IsMatch(fileContent, regex))
            {
                var result = fileContent.Replace(regex, regex.Replace("undirected", "directed"));
                return Task.FromResult(result);
            }
            return Task.FromResult(fileContent);
        }
        private static Task<string> ensureEndtagsAreSet(string fileContent, string elementName)
        {

            var result = Regex.Replace(fileContent, $"(?<=<{elementName}.*)/>", m => string.Format("{1}", m.Value, $"></{elementName }>"), RegexOptions.IgnoreCase);
            return Task.FromResult(result);
        }
    }
}