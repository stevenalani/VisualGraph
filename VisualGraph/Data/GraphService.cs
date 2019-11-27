using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Frontenac.Blueprints.Impls.TG;
using Frontenac.Blueprints.Util.IO.GraphML;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using VisualGraph.Data.Additional.Interfaces;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data
{
    public class GraphService : IGraphService
    {
        ILogger<GraphService> _logger;
        IJSRuntime JSRuntime;
        public event Action RefreshRequested;
        private ConcurrentDictionary<string, GraphDisplayParameters> GraphDisplayParameters = new ConcurrentDictionary<string, GraphDisplayParameters>();
        private Thread GraphParameterThread;
        private ThreadStart GraphParameterThreadArgs;
        private bool GraphParameterThreadShouldRun = false;
        private readonly double runningtime = 0.5;

        public GraphService(IConfiguration config, ILogger<GraphService> logger, IJSRuntime jsRuntime)
        {
            _logger = logger;
            JSRuntime = jsRuntime;
            GraphFileProvider.EnsureGraphDirExists();
            GraphParameterThreadArgs = new ThreadStart(QueryGraphSVGs);
        }
        public Task<BasicGraphModel[]> GetAllGraphs()
        {
            return Task.FromResult(GraphFileProvider.GetBasicGraphs().ToArray());
        }
        public Task<BasicGraphModel> GetGraph(string filename)
        {
            return Task.FromResult(GraphFileProvider.GetBasicGraphs().ToArray().First(x=>x.Path.Contains(filename)));
        }

        public Task<string[]> GetGraphFilenames()
        {
            return Task.FromResult(GraphFileProvider.GetGraphFileNames());
        }
        public async Task SaveGraph(BasicGraphModel graph, string filename = "")
        {
            if (filename == "")
            {
                filename = graph.Name;
                if (filename == "")
                    filename = "untitledgraph" + DateTime.Now.Millisecond;
            }
            try
            {
                await WriteGraph(graph, filename);
                
            }catch(Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
        private Task WriteGraph(BasicGraphModel graph,string filename)
        {
            GraphFileProvider.WriteToGraphMlFile(graph, filename);
            var wrotegraph = GraphFileProvider.ReadGraphMlToBasicGraph(filename);

            if (wrotegraph != null || !wrotegraph.Path.Contains(filename))
                return Task.FromException(new Exception("There was an error"));
            else
                return Task.CompletedTask;
        }

        public Task<BasicGraphModel> GenerateGraph(string filename, int nodecount = 20 ,int fromx = -50, int fromy = -50, int tox = 50, int toy = 50)
        {
            if (filename == "") return null;
            Random rand = new Random();
            List<Node> nodes = new List<Node>();
            List<Edge> edges = new List<Edge>();
            for (var count = 0; count < tox; count++)
            {
                var x = rand.NextDouble() + Convert.ToDouble(rand.Next(fromx, tox));
                var y = rand.NextDouble() + Convert.ToDouble(rand.Next(fromy, toy));

                nodes.Add(new Node { PosX = x, PosY = y, Id = count });

            }

            BasicGraphModel graph = new BasicGraphModel()
            {
                Nodes = nodes,
                Edges = edges,
                Path = filename,
            };
            return Task.FromResult(graph);
        }
        private async void QueryGraphSVGs()
        {
            DateTime starttime = DateTime.Now;;
            while (GraphParameterThreadShouldRun)
            {
                try
                {
                    var parameters = await JSRuntime.InvokeAsync<GraphDisplayParameters[]>("getAllSVGTransformationMatrices");
                    foreach(var param in parameters)
                    {
                        GraphDisplayParameters.AddOrUpdate(param.Name,param,(key,oldval) => { return param; } );
                    }
                }catch{}
                if (starttime.AddSeconds(runningtime) < DateTime.Now)
                    return;
            }
        }
        public async Task<GraphDisplayParameters> InitialGetGraphDisplayParameters(string graphid)
        {
            if (graphid == null || graphid == "") return null;
            var parm = await JSRuntime.InvokeAsync<GraphDisplayParameters>("getSVGTransformationMatrix", new { id = graphid });
            GraphDisplayParameters.TryAdd(parm.Name, parm);
            return parm;
        }
        public Task<GraphDisplayParameters> GetGraphDisplayParameters(string graphid)
        {
            GraphDisplayParameters outval;
            GraphDisplayParameters.TryGetValue(graphid,out outval);
            return Task.FromResult(outval);
        }
        public void CallRequestRefresh()
        {
            RefreshRequested?.Invoke();
        }

        public void StartParameterRefresh()
        {
            if (!GraphParameterThreadShouldRun )
            {
                if (GraphParameterThread != null && GraphParameterThread.IsAlive)
                    return;
                GraphParameterThreadShouldRun = true;
                GraphParameterThread = new Thread(GraphParameterThreadArgs);
                GraphParameterThread.Start();
            }

        }
        public void StopParameterRefresh()
        {
            if (GraphParameterThreadShouldRun)
            {
                GraphParameterThreadShouldRun = false;
                GraphParameterThread?.Join();
            }

        }
    }
}
