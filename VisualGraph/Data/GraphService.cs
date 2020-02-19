using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Miscellaneous;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VisualGraph.Components;
using VisualGraph.Data.Additional.Interfaces;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data
{
    public class GraphService : IGraphService
    {
        ILogger<GraphService> _logger;
        IJSRuntime JSRuntime;
        public static string settingsFile = "settings.xml";

        public BasicGraph CurrentGraph { get; private set; }
        public SettingsCSS SettingsCSS { get; private set; }
        public GraphStyleParameters GraphStyleParameters { get; private set; } = new GraphStyleParameters();

        
        public GraphService(IConfiguration config, ILogger<GraphService> logger, IJSRuntime jsRuntime)
        {
            _logger = logger;
            JSRuntime = jsRuntime;
            LoadGraphStyleParameters();
            GraphFileProvider.EnsureGraphDirExists();
        }
        public async Task<BasicGraphModel[]> GetAllGraphs()
        {
            return (await GraphFileProvider.GetBasicGraphs()).ToArray();
        }
        public async Task<BasicGraphModel> GetGraph(string filename)
        {
            return await GraphFileProvider.GetBasicGraph(filename);
        }

        public async Task<string[]> GetGraphFilenames()
        {
            return await GraphFileProvider.GetGraphFileNames();
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

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
        private async Task WriteGraph(BasicGraphModel graph, string filename)
        {
            await GraphFileProvider.WriteToGraphMlFile(graph, filename);
        }

        public Task<BasicGraphModel> LayoutGraph(BasicGraphModel GraphModel)
        {
            try
            {
                GeometryGraph geometryGraph = new GeometryGraph();

                var nodes = GraphModel.Edges.Where(x => x.StartNode != null || x.EndNode != null).SelectMany(x =>
                {
                    if (x.StartNode != null && x.EndNode != null) return new[] { x.StartNode, x.EndNode };
                    else if (x.StartNode != null) return new[] { x.StartNode };
                    else return new[] { x.EndNode };
                }).ToList();


                nodes.ForEach(x => geometryGraph.Nodes.Add(new Microsoft.Msagl.Core.Layout.Node(CurveFactory.CreateCircle(1, new Microsoft.Msagl.Core.Geometry.Point()), x.Id)));
                GraphModel.Edges.ForEach(x =>
                {
                    var node1 = geometryGraph.Nodes.FirstOrDefault(n => (int)n.UserData == x.StartNode.Id);
                    var node2 = geometryGraph.Nodes.FirstOrDefault(n => (int)n.UserData == x.EndNode.Id);
                    geometryGraph.Edges.Add(new Microsoft.Msagl.Core.Layout.Edge(node1, node2) { Length = x.Weight, UserData = x.Id, Weight = (int)x.Weight });
                });

                var settings = new Microsoft.Msagl.Layout.MDS.MdsLayoutSettings() { RemoveOverlaps = true, IdealEdgeLength = new IdealEdgeLengthSettings() { } };
                LayoutHelpers.CalculateLayout(geometryGraph, settings, null);
                nodes.ForEach(x =>
                {
                    var node = geometryGraph.FindNodeByUserData(x.Id);
                    x.Pos.X = (float)node.Center.X;
                    x.Pos.Y = (float)node.Center.Y;
                });
            }
            catch { }
            return Task.FromResult(GraphModel);
        }
        public async Task InitZoomPan(DotNetObjectReference<BasicGraph> reference, string graphid)
        {
            await JSRuntime.InvokeVoidAsync("InitPanZoom", new object[] { reference, graphid });
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

        public async Task<SvgPanZoomInformation> GetSvgPanZoomInformation(string graphname)
        {
            var svginfo = await JSRuntime.InvokeAsync<SvgPanZoomInformation>("GetPanZoomValues", new object[] { graphname });
            return svginfo;
        }
        public async Task<SvgContainerInformation> GetSvgContainerInformation(string graphname)
        {
            var svginfo = await JSRuntime.InvokeAsync<SvgContainerInformation>("GetSvgContainerSizes", new object[] { graphname });
            return svginfo;
        }

        public async Task<Point2> GetTranslatedMousePos(string graphname, double x, double y)
        {
            var svgInfo = await GetSvgPanZoomInformation(graphname);
            var mousePos = await JSRuntime.InvokeAsync<Point2>("GetTranslatedMousePos", new object[] { new { id = graphname, x = x - svgInfo.OffsetLeft, y = y - svgInfo.OffsetTop } });
            return mousePos;
        }
        public async Task UpdateBBox()
        {
            await JSRuntime.InvokeVoidAsync("UpdateBBox");
        }

        public async Task Fit()
        {
            await JSRuntime.InvokeVoidAsync("Fit");
        }
        public async Task Center()
        {
            await JSRuntime.InvokeVoidAsync("Center");
        }
        public async Task Resize()
        {
            await JSRuntime.InvokeVoidAsync("Resize");
        }
        public async Task Crop()
        {
            await Fit();
            await Center();
        }

        public Task SaveGraphStyleParameters(GraphStyleParameters styleParameters = null)
        {
            var styleParametersPOCO = new GraphStyleParametersPOCO();
            if (styleParameters == null)
            {
                styleParametersPOCO.Initialize(GraphStyleParameters);
            }
            else
            {
                styleParametersPOCO.Initialize(styleParameters);
            }
            TextWriter textWriter = new StreamWriter(settingsFile);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(GraphStyleParametersPOCO));
            xmlSerializer.Serialize(textWriter, styleParametersPOCO);
            textWriter.Close();
            return Task.CompletedTask;
        }
        public Task LoadGraphStyleParameters()
        {
            if (File.Exists(settingsFile))
            {
                TextReader sr = new StreamReader(settingsFile);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(GraphStyleParametersPOCO));
                GraphStyleParametersPOCO styleParametersPOCO = (GraphStyleParametersPOCO)xmlSerializer.Deserialize(sr);
                sr.Close();
                GraphStyleParameters.InitFromPoco(styleParametersPOCO);

            }
            else
            {
                SaveGraphStyleParameters();
            }
            return Task.CompletedTask;
        }
        public async Task Rerender()
        {
            if(CurrentGraph != null)
                await CurrentGraph.ChangedState();
            if( SettingsCSS != null)
                await SettingsCSS.ChangedState();
        }
        public Task<RenderFragment> GetRenderFragment(BasicGraphModel Graph)
        {
            var fragment = new RenderFragment(builder =>
            {
                if (Graph != null)
                {
                    builder.OpenComponent<BasicGraph>(0);
                    builder.AddAttribute(1, "GraphModel", Graph);
                    builder.AddComponentReferenceCapture(2,
                        inst =>
                        {
                            CurrentGraph = (BasicGraph)inst;
                            CurrentGraph.RegisterDefaultCallbacks();
                        });
                    builder.CloseComponent();
                }
            });
            return Task.FromResult(fragment);
        }

        public Task<RenderFragment> GraphStyeTag()
        {
            var fragment = new RenderFragment(builder =>
            {
                    
                builder.OpenComponent<SettingsCSS>(0);
                builder.AddAttribute(1, "StyleParameters", GraphStyleParameters);
                builder.AddComponentReferenceCapture(2,
                inst =>
                {
                    SettingsCSS = (SettingsCSS)inst;
                });
                builder.CloseComponent();
                
            });
            return Task.FromResult(fragment);
        }

    }
}
