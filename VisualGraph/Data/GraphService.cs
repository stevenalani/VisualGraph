using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Frontenac.Blueprints.Impls.TG;
using Frontenac.Blueprints.Util.IO.GraphML;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Miscellaneous;
using VisualGraph.Data.Additional.Interfaces;
using VisualGraph.Data.Additional.Models;
using Edge = VisualGraph.Data.Additional.Models.Edge;
using Node = VisualGraph.Data.Additional.Models.Node;

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

                nodes.Add(new Node { Pos = new Vector2((float)x, (float)y), Id = count });

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

        public async Task<BasicGraphModel> LayoutGraph(BasicGraphModel GraphModel)
        {
            try
            {
                StopParameterRefresh();
                GeometryGraph geometryGraph = new GeometryGraph();

                var nodes = GraphModel.Edges.Where(x => x.StartNode != null || x.EndNode != null).SelectMany(x => {
                    if (x.StartNode != null && x.EndNode != null) return new[] { x.StartNode, x.EndNode };
                    else if (x.StartNode != null) return new[] { x.StartNode };
                    else return new[] { x.EndNode };
                }).ToList();


                nodes.ForEach(x => geometryGraph.Nodes.Add(new Microsoft.Msagl.Core.Layout.Node(CurveFactory.CreateCircle(1, new Microsoft.Msagl.Core.Geometry.Point()), x.Id)));
                GraphModel.Edges.ForEach(x => {
                    var node1 = geometryGraph.Nodes.FirstOrDefault(n => (int)n.UserData == x.StartNode.Id);
                    var node2 = geometryGraph.Nodes.FirstOrDefault(n => (int)n.UserData == x.EndNode.Id);
                    geometryGraph.Edges.Add(new Microsoft.Msagl.Core.Layout.Edge(node1, node2) { Length = x.Weight, UserData = x.Id });
                });

                var settings = new Microsoft.Msagl.Layout.MDS.MdsLayoutSettings();
                LayoutHelpers.CalculateLayout(geometryGraph, settings, null);
                nodes.ForEach(x =>
                {
                    var node = geometryGraph.FindNodeByUserData(x.Id);
                    x.Pos.X = (float)node.Center.X;
                    x.Pos.Y = (float)node.Center.Y;
                });
            }
            catch { }
            return GraphModel;
        }
        public async Task InitZoomPan(string graphid)
        {
            await JSRuntime.InvokeVoidAsync("InitPanZoom", new object[] { graphid });
        }
        public async Task DestroyZoomPan()
        {
            await JSRuntime.InvokeVoidAsync("DestroyPanZoom");
        }
        public async Task DisablePan()
        {
            await JSRuntime.InvokeVoidAsync("DisablePan");
        }
        public async Task EnablePan()
        {
            await JSRuntime.InvokeVoidAsync("EnablePan");
        }

        public async Task<SvgInformation> GetSvgInformation(string graphname)
        {
            var svginfo = await JSRuntime.InvokeAsync<SvgInformation>("GetPanZoomValues", new object[] { graphname });
            return svginfo;
        }

        public async Task<Point2> GetTranslatedMousePos(string graphname, double x, double y)
        {
            var svgInfo = await GetSvgInformation(graphname);
            var mousePos = await JSRuntime.InvokeAsync<Point2>("GetTranslatedMousePos", new object[] { new { id = graphname, x = x - svgInfo.OffsetLeft, y = y - svgInfo.OffsetTop } }  );
            return mousePos;
        }
    }
}
