using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Frontenac.Blueprints.Impls.TG;
using Frontenac.Blueprints.Util.IO.GraphML;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VisualGraph.Data.Additional.Interfaces;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data
{
    public class GraphService : IGraphService
    {
        ILogger<GraphService> _logger;
        public GraphService(IConfiguration config, ILogger<GraphService> logger)
        {
            _logger = logger;
            GraphFileProvider.EnsureGraphDirExists();

        }
        public Task<BasicGraph[]> GetAllGraphs()
        {
            return Task.FromResult(GraphFileProvider.GetBasicGraphs().ToArray());
        }
        public Task<BasicGraph> GetGraph(string filename)
        {
            return Task.FromResult(GraphFileProvider.GetBasicGraphs().ToArray().First(x=>x.Path.Contains(filename)));
        }

        public Task<string[]> GetGraphFilenames()
        {
            return Task.FromResult(GraphFileProvider.GetGraphFileNames());
        }
        public Task SaveGraph(BasicGraph graph,string filename = null)
        {
            filename = filename == null ? graph.Path : filename;
            filename = filename == null ? "unbenannt.xml" : filename;
            try
            {
                var task = WriteGraph(graph, filename);
                task.Start();
                task.Wait();
            }catch(Exception e)
            {
                _logger.LogError(e.Message);
            }
            return Task.CompletedTask;
        }
        private Task WriteGraph(BasicGraph graph, string filename = null)
        {
            filename = filename == null ? graph.Path : filename;
            GraphFileProvider.WriteToGraphMlFile(graph, filename);
            var wrotegraph = GraphFileProvider.ReadGraphMlToBasicGraph(filename);

            if (wrotegraph != null || !wrotegraph.Path.Contains(filename))
                return Task.FromException(new Exception("There was an error"));
            else
                return Task.CompletedTask;
        }
    }
}
