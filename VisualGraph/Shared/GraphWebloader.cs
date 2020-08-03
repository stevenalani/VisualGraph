using Frontenac.Blueprints.Impls.TG;
using Frontenac.Blueprints.Util.IO.GraphML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisualGraph.Shared.Models;

namespace VisualGraph.Shared
{
    /// <summary>
    /// Lädt Graphen aus dem Internet
    /// </summary>
    public static class GraphWebloader
    {
        private static string _graphMlHeader = "<?xml version=\"1.0\"";
        private static string _graphMlClosingTag = "</graphml>";
        /// <summary>
        /// Extrahiert wichtige Daten aus einer Zeichenkette und erstellt ein Graph Model daraus
        /// </summary>
        /// <param name="graphstring">Graph als Zeichenkette</param>
        /// <param name="graphname">Name für das neue Model</param>
        /// <returns></returns>
        public static async Task<BasicGraphToGraphMlMapping> LoadBasicGraphFromUrl(string graphstring, string graphname)
        {
            if (!graphname.Contains(".xml"))
                graphname += ".xml";
                       
            graphstring = await IsolateGraphInFile(graphstring);
            var tinkerGraph = await ReadGraphMl(graphstring);
            BasicGraphToGraphMlMapping graphToGraphMlMapping = new BasicGraphToGraphMlMapping(tinkerGraph);
            return graphToGraphMlMapping;
        }
        private static async Task<string> IsolateGraphInFile(string graphstring)
        {
            graphstring = graphstring.Replace("&lt;", "<").Replace("&gt;", ">");
            var graphbeginning = graphstring.IndexOf(_graphMlHeader);
            graphstring = graphstring.Remove(0, graphbeginning);
            var graphend = graphstring.IndexOf(_graphMlClosingTag);
            if (graphstring.Length > graphend + _graphMlClosingTag.Length)
            {
                graphstring = graphstring.Remove(graphend + _graphMlClosingTag.Length);
            }
            graphstring = await ensureEdgeIdsAreSet(graphstring);
            graphstring = await ensureEdgeLabelsAreSet(graphstring);
            graphstring = await ensureEdgedefaultIsDirected(graphstring);
            graphstring = await ensureEndtagsAreSet(graphstring, "edge");

            return graphstring;
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
        internal static Task<TinkerGrapĥ> ReadGraphMl(string graphstring)
        {
            byte[] data = Encoding.UTF8.GetBytes(graphstring);
            TinkerGrapĥ g = new TinkerGrapĥ();
            using (Stream stream = new MemoryStream(data)) {
                GraphMlReader.InputGraph(g, stream);
            }
            return Task.FromResult(g);
            

        }
    }
}
