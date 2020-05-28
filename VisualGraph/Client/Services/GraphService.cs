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
using System.Text.Json;
using System.Xml.Serialization;
using Microsoft.Msagl.Core.Geometry;
using VisualGraph.Shared;
using VisualGraph.Shared.Models;
using VisualGraph.Client.Components;
using VisualGraph.Client.Shared;
using System.Net.Http;
using System.Net.Http.Json;


using System.Collections.Generic;
using VisualGraph.Client.Shared.Models;
using System.Globalization;
using VisualGraph.Client.Components.Additional;
using Node = VisualGraph.Shared.Models.Node;
using VisualGraph.Shared.Models.Interfaces;

namespace VisualGraph.Client.Services
{
    public class GraphService : IGraphService
    {
        private readonly HttpClient _httpClient;
        ILogger<GraphService> _logger;
        IJSRuntime JSRuntime;
        public static string settingsFile = "settings.xml";
        public BasicGraphModel CurrentGraphModel { get; set; } 
        public BasicGraph CurrentGraph { get; set; } 
        public Settings Settings { get; set; } 
        public GraphEditForm GraphEditForm { get; set; } 
        public SettingsCSS SettingsCSS { get; set; } 
        public GraphStyleParameters GraphStyleParameters { get; set; } = new GraphStyleParameters();

        public GraphService(IConfiguration config, ILogger<GraphService> logger, IJSRuntime jsRuntime, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _logger = logger;
            JSRuntime = jsRuntime;
            Task.Run(LoadGraphStyleParameters);
            CurrentGraphModel = GraphFactory.CreateRandomGraph("Unbenannter-Graph",10,20,50,-50);
        }
        public async Task<BasicGraphModel[]> GetAllGraphs()
        {
            return (await _httpClient.GetFromJsonAsync<IEnumerable<BasicGraphModelPoco>>("api/Graph/GetGraphModels")).ToArray();
        }
        public async Task<BasicGraphModel> GetGraph(string filename)
        {
            var graphResponse = await _httpClient.GetFromJsonAsync<BasicGraphModelPoco>($"api/Graph/GetGraphModel/{filename}");
            graphResponse.Nodes = graphResponse.NodesPoco.Select(n => new VisualGraph.Shared.Models.Node
            {
                Name = n.Name,
                Classes = n.Classes,
                Edges = n.Edges,
                Id = n.Id,
                Pos = new System.Numerics.Vector2(float.Parse(n.PosXTextPoco.Replace(".",",")), float.Parse(n.PosYTextPoco.Replace(".", ",")))
                
            }).ToList();
            foreach(var edge in graphResponse.Edges)
            {
                edge.StartNode = graphResponse.Nodes.FirstOrDefault(x => x.Id == edge.StartNode.Id);
                edge.EndNode = graphResponse.Nodes.FirstOrDefault(x => x.Id == edge.EndNode.Id);
                edge.StartNode.Edges.Add(edge);
                edge.EndNode.Edges.Add(edge);
            }
            Console.WriteLine(graphResponse.Edges[0].StartNode.Id + " -> " + graphResponse.Edges[0].EndNode.Id);
            

            return (BasicGraphModel)graphResponse;
        }
        public async Task LoadGraph(string filename)
        {
            BasicGraphModel graph;
            if (filename == null || filename == "Neuer Graph")
            {
                graph = GraphFactory.CreateNewGraphModel();
            }
            else
            {
                graph = await GetGraph(filename);
            }
            CurrentGraphModel = graph;
            await Rerender();
            
        }

        public async Task<string[]> GetGraphFilenames()
        {
            //Get
            var graphFilenames = await _httpClient.GetFromJsonAsync<IEnumerable<string>>($"api/Graph/GetGraphFilenames");
            return graphFilenames.ToArray();
        }
        public async Task<bool> SaveGraph(string filename = "")
        {
            if (filename == "" || filename == null)
            {
                filename = CurrentGraphModel.Name;
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
                return await WriteGraph(CurrentGraphModel, filename);

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }
        private async Task<bool> WriteGraph(BasicGraphModel graph, string filename)
        {
            
            return await (await _httpClient.PostAsJsonAsync($"api/Graph/SaveGraph/{filename}",new BasicGraphModelPoco(graph))).Content.ReadFromJsonAsync<bool>();
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
            return Task.CompletedTask;
        }
        public async Task InitZoomPan(DotNetObjectReference<BasicGraph> reference)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("InitPanZoom", new object[] { reference, CurrentGraphModel.Name });
            }
            catch { }
        }
        public async Task DestroyZoomPan()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("DestroyPanZoom");
                }
            catch { }
        }
        public async Task DisablePan()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("DisablePan");
            }
            catch { }
        }
        public async Task EnablePan()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("EnablePan");
            }
            catch { }
        }

        public async Task<SvgPanZoomInformation> GetSvgPanZoomInformation()
        {
            try
            {
                var svginfo = await JSRuntime.InvokeAsync<SvgPanZoomInformation>("GetPanZoomValues", new object[] { CurrentGraphModel.Name });
                return svginfo;
            }
            catch { return new SvgPanZoomInformation(); };
        }
        public async Task<SvgContainerInformation> GetSvgContainerInformation()
        {
            try
            {
                var svginfo = await JSRuntime.InvokeAsync<SvgContainerInformation>("GetSvgContainerSizes", new object[] { CurrentGraphModel.Name });
            return svginfo;
            }
            catch { return new SvgContainerInformation(); };
        }
        public async Task<BrowserSizes> GetBrowserSizes()
        {
            try
            {
                return await JSRuntime.InvokeAsync<BrowserSizes>("GetBrowserSizes");
            }
            catch { return new BrowserSizes(); }
        }
        public async Task<Point2> GetTranslatedMousePos(double x, double y)
        {
            try
            {
                var svgInfo = await GetSvgPanZoomInformation();
                var mousePos = await JSRuntime.InvokeAsync<Point2>("GetTranslatedMousePos", new object[] { new { id = CurrentGraphModel.Name, x = x - svgInfo.OffsetLeft, y = y - svgInfo.OffsetTop } });
                return mousePos;
            }
            catch { return new Point2(); }
        }
        public async Task UpdateBBox()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("UpdateBBox");
            }
            catch { }
        }

        public async Task Fit()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("Fit");
            }
            catch { }
        }
        public async Task Center()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("Center");
            }
            catch { }
        }
        public async Task Resize()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("Resize");
            }
            catch { }
        }
        public async Task Crop()
        {
            try
            {
                await Fit();
                await Center();
            }
            catch { }
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
        public async Task LoadGraphStyleParameters()
        {
            GraphStyleParametersPOCO styleParametersPOCO = await _httpClient.GetFromJsonAsync<GraphStyleParametersPOCO>("api/Settings/GetGraphStyle");
            GraphStyleParameters.InitFromPoco(styleParametersPOCO);
            await Rerender();
            //SaveGraphStyleParameters();
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
       

        public Task<RenderFragment> GetCssMarkup()
        {
            var fragment = new RenderFragment(builder =>
            {
                builder.OpenComponent<SettingsCSS>(0);
                builder.AddComponentReferenceCapture(1,
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
            if(CurrentGraph != null && CurrentGraph.IsRendered)
            {
                await CurrentGraph.ChangedState();
            }
            if (SettingsCSS != null && SettingsCSS.IsRendered) {
                await SettingsCSS.ChangedState();
            }
            if (Settings != null && Settings.IsRendered) {
                
                await Settings.ChangedState();
            }
            if (GraphEditForm != null && GraphEditForm.IsRendered) {
                await GraphEditForm.ChangedState();
            }
        }
        public async Task Rerender(VisualGraph.Shared.Models.Node node = null) 
        {
            
                if (CurrentGraph != null && CurrentGraph.IsRendered)
                {
                    Console.WriteLine(node.Name);
                    NodeComponent component = CurrentGraph.NodeComponents.FirstOrDefault(x => x.Node.Name == node.Name);
                    Console.WriteLine(component.Node.Name);
                    await component.ChangedState();
                }
            
        }
        public async Task Rerender(VisualGraph.Shared.Models.Edge edge = null)
        {
            if (CurrentGraph != null && CurrentGraph.IsRendered)
            {
                var component = CurrentGraph.EdgeComponents.SingleOrDefault(x => x.Edge == edge);
                Console.WriteLine(component.GetType().Name);
                if (component != null && component.IsRendered)
                {
                   await component.ChangedState();
                }
            }
            
        }
        public async Task Rerender<T>() where T : GraphInternalUI
        {
            if (typeof(T) == typeof(BasicGraph) && CurrentGraph != null && CurrentGraph.IsRendered)
            {
                await CurrentGraph.ChangedState();
            }
            else if (typeof(T) == typeof(SettingsCSS) && SettingsCSS != null && SettingsCSS.IsRendered)
            {
                await SettingsCSS.ChangedState();
            }
            else if(typeof(T) == typeof(Settings) && Settings != null && Settings.IsRendered)
            {

                await Settings.ChangedState();
            }
            else if(typeof(T) == typeof(GraphEditForm) && GraphEditForm != null && GraphEditForm.IsRendered)
            {
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

        public Task UseGraphModel(BasicGraphModel graphModel, bool withDefaultCallbacks = true)
        {
            CurrentGraphModel = graphModel;
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
            return Task.CompletedTask;
        }

    }

}
