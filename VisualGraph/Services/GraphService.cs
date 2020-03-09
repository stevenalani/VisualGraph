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
using VisualGraph.Services.Interfaces;
using VisualGraph.Data.Additional.Models;
using VisualGraph.Data;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Core.Routing;

namespace VisualGraph.Services
{
    public class GraphService : IGraphService
    {
        ILogger<GraphService> _logger;
        IJSRuntime JSRuntime;
        public static string settingsFile = "settings.xml";
        public BasicGraphModel CurrentGraphModel { get; set; } 
        public BasicGraph CurrentGraph { get; set; } 
        public Settings Settings { get; set; } 
        public GraphEditForm GraphEditForm { get; set; } 
        public SettingsCSS SettingsCSS { get; set; } 
        public GraphStyleParameters GraphStyleParameters { get; set; } = new GraphStyleParameters();

        public GraphService(IConfiguration config, ILogger<GraphService> logger, IJSRuntime jsRuntime)
        {
            _logger = logger;
            JSRuntime = jsRuntime;
            GraphFileProvider.EnsureGraphDirExists();
            LoadGraphStyleParameters();
            CurrentGraphModel = CreateNewGraphModel();
        }
        public async Task<BasicGraphModel[]> GetAllGraphs()
        {
            return (await GraphFileProvider.GetBasicGraphs()).ToArray();
        }
        public async Task<BasicGraphModel> GetGraph(string filename)
        {
            return await GraphFileProvider.GetBasicGraph(filename);
        }
        public async Task LoadGraph(string filename)
        {
            BasicGraphModel graph;
            if (filename == "Neuer Graph")
            {
                graph = CreateNewGraphModel();
            }
            else
            {
                graph = await GetGraph(filename);
            }
            CurrentGraphModel = graph;
            await Rerender();
            
        }
        private BasicGraphModel CreateNewGraphModel()
        {
            var nodes = new System.Collections.Generic.List<Data.Additional.Models.Node>{
                new Data.Additional.Models.Node() {
                    Id = "0",
                    Pos = new System.Numerics.Vector2(-10,10),
                    Name = "Knoten A"
                },
                new Data.Additional.Models.Node() {
                    Id = "1",
                    Pos = new System.Numerics.Vector2(10,-10),
                    Name = "Knoten B"

                }
                
            };
            var graphmodel = new BasicGraphModel()
            {
                Nodes = nodes,
                Name = "Unbenannter-Graph"
            };
            return graphmodel;
        }
        public async Task<string[]> GetGraphFilenames()
        {
            return await GraphFileProvider.GetGraphFileNames();
        }
        public async Task<bool> SaveGraph(BasicGraphModel graph, string filename = "")
        {
            if (filename == "")
            {
                filename = graph.Name;
                if (filename.Trim() == "")
                    filename = "untitledgraph" + DateTime.Now.Millisecond;
            }
            try
            {
                var wrongEdges = CurrentGraphModel.Edges.Where(x => x.StartNode.Id == "-1" || x.EndNode.Id == "-1").ToList();
                var wrongNodes = CurrentGraphModel.Nodes.Where(x => x.Id == "-1").ToList();
                foreach (var node in wrongNodes)
                {
                    CurrentGraphModel.Nodes.Remove(node);
                }
                foreach (var edge in wrongEdges)
                {
                    CurrentGraphModel.Edges.Remove(edge);
                }
                return await WriteGraph(graph, filename);

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }
        private async Task<bool> WriteGraph(BasicGraphModel graph, string filename)
        {
            return await GraphFileProvider.WriteToGraphMlFile(graph, filename);
        }

        public Task LayoutGraph(double scalex = 2.2, double scaley = 2.2)
        {
            try
            {
                GeometryGraph geometryGraph = new GeometryGraph();
                var nodes = CurrentGraphModel.Edges.Where(x => x.StartNode != null || x.EndNode != null).SelectMany(x =>
                {
                    if (x.StartNode != null && x.EndNode != null) return new[] { x.StartNode, x.EndNode };
                    else if (x.StartNode != null) return new[] { x.StartNode };
                    else return new[] { x.EndNode };
                }).ToList();


                nodes.ForEach(x => geometryGraph.Nodes.Add(new Microsoft.Msagl.Core.Layout.Node(new Ellipse(1, 1, new Point()), x.Id)));
                CurrentGraphModel.Edges.ForEach(x =>
                {
                    var node1 = geometryGraph.Nodes.FirstOrDefault(n => n.UserData.ToString() == x.StartNode.Id);
                    var node2 = geometryGraph.Nodes.FirstOrDefault(n => n.UserData.ToString() == x.EndNode.Id);
                    geometryGraph.Edges.Add(new Microsoft.Msagl.Core.Layout.Edge(node1, node2));
                });

                var settings = new Microsoft.Msagl.Layout.MDS.MdsLayoutSettings
                {
                    ScaleX = scalex,
                    ScaleY = scaley
                };

                LayoutHelpers.CalculateLayout(geometryGraph, settings, null);
                nodes.ForEach(x =>
                {
                    var node = geometryGraph.FindNodeByUserData(x.Id);
                    x.Pos.X = (float)node.Center.X;
                    x.Pos.Y = (float)node.Center.Y;
                });
            }
            catch { }
            Console.WriteLine("Layouting done");
            return Task.CompletedTask;
        }
        public async Task InitZoomPan(DotNetObjectReference<BasicGraph> reference)
        {
            await JSRuntime.InvokeVoidAsync("InitPanZoom", new object[] { reference, CurrentGraphModel.Name });
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

        public async Task<SvgPanZoomInformation> GetSvgPanZoomInformation()
        {
            var svginfo = await JSRuntime.InvokeAsync<SvgPanZoomInformation>("GetPanZoomValues", new object[] { CurrentGraphModel.Name });
            return svginfo;
        }
        public async Task<SvgContainerInformation> GetSvgContainerInformation()
        {
            var svginfo = await JSRuntime.InvokeAsync<SvgContainerInformation>("GetSvgContainerSizes", new object[] { CurrentGraphModel.Name });
            return svginfo;
        }
        public async Task<BrowserSizes> GetBrowserSizes()
        {
            return await JSRuntime.InvokeAsync<BrowserSizes>("GetBrowserSizes");
        }
        public async Task<Point2> GetTranslatedMousePos(double x, double y)
        {
            var svgInfo = await GetSvgPanZoomInformation();
            var mousePos = await JSRuntime.InvokeAsync<Point2>("GetTranslatedMousePos", new object[] { new { id = CurrentGraphModel.Name, x = x - svgInfo.OffsetLeft, y = y - svgInfo.OffsetTop } });
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
        public async Task<RenderFragment> GetRenderFragment(bool withDefaultCallbacks = true)
        {
            if(CurrentGraphModel != null)
            {
                return await BuildBasicGraphFragment(CurrentGraphModel,withDefaultCallbacks);
            }
            return null;
        }
        public Task<RenderFragment> GetRenderFragment(BasicGraphModel Graph, bool withDefaultCallbacks = true)
        {
            var fragment = new RenderFragment(builder =>
            {
                if (Graph != null)
                {
                    builder.OpenComponent<BasicGraph>(0);
                    //builder.AddAttribute(1, "GraphModel", Graph);
                    builder.AddComponentReferenceCapture(1,
                        inst =>
                        {
                            CurrentGraph = (BasicGraph)inst;
                            if (withDefaultCallbacks)
                            {
                                CurrentGraph.RegisterDefaultCallbacks();
                            }
                        });
                    builder.CloseComponent();
                }
            });
            return Task.FromResult(fragment);
        }
        private Task<RenderFragment> BuildBasicGraphFragment(BasicGraphModel graphModel, bool withDefaultCallbacks = true)
        {
            var fragment = new RenderFragment(builder =>
            {
                if (graphModel != null)
                {
                    builder.OpenComponent<BasicGraph>(0);
                    builder.AddComponentReferenceCapture(1,
                        inst =>
                        {
                            CurrentGraph = (BasicGraph)inst;
                            if (withDefaultCallbacks)
                            {
                                CurrentGraph.RegisterDefaultCallbacks();
                            }
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
                builder.AddComponentReferenceCapture(2,
                inst =>
                {
                    SettingsCSS = (SettingsCSS)inst;
                });
                builder.CloseComponent();

            });
            return Task.FromResult(fragment);
        }
        public Task<RenderFragment> GetSettingsRenderFragment()
        {
            var fragment = new RenderFragment(builder =>
            {

                builder.OpenComponent<Settings>(0);
                builder.AddComponentReferenceCapture(1,
                inst =>
                {
                    Settings = (Settings)inst;
                });
                builder.CloseComponent();

            });
            return Task.FromResult(fragment);
        }
        public async Task Rerender()
        {
            if(CurrentGraph != null && await CurrentGraph?.IsRendered)
            {
                await CurrentGraph.ChangedState();
            }
            if (SettingsCSS != null && await SettingsCSS?.IsRendered) {
                await SettingsCSS.ChangedState();
            }
            if (Settings != null && await Settings?.IsRendered) {
                
                await Settings.ChangedState();
            }
            if (GraphEditForm != null && await GraphEditForm?.IsRendered) {
                await GraphEditForm.ChangedState();
            }
        }

        public Task<RenderFragment> GetEditFormRenderFragment()
        {
            var fragment = new RenderFragment(builder =>
            {

                builder.OpenComponent<GraphEditForm>(0);
                builder.AddComponentReferenceCapture(1,
                inst =>
                {
                    GraphEditForm = (GraphEditForm)inst;
                });
                builder.CloseComponent();

            });
            return Task.FromResult(fragment);
        }
    }

}
