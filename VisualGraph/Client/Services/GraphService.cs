using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Miscellaneous;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VisualGraph.Client.Components;
using VisualGraph.Client.Components.Additional;
using VisualGraph.Client.Shared.Models;
using VisualGraph.Shared;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.Services
{
    /// <summary>
    /// Dieser Service steuert alles, was mit Graphen zu tun hat.
    /// Der Service kommuniziert sowohl mit JavaScript auf dem Client,
    /// als auch mit dem Server der Anwendung.
    /// </summary>
    public class GraphService : IGraphService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GraphService> _logger;
        private readonly IJSRuntime JSRuntime;
        /// <summary>
        /// Gibt an wo die Einstellungen gespeichert sind
        /// </summary>
        public static string settingsFile = "settings.xml";
        /// <summary>
        /// Dieses Model ermöglicht den Zugriff auf den aktuell geladenen Graphen        
        /// </summary>
        public BasicGraphModel CurrentGraphModel { get; set; }
        /// <summary>
        /// Diese Komponente ermöglicht den Zugriff auf die Anzeigekomponente. So können zum Beispiel
        /// Entwickler schnell weitere Event Callbacks an die Komponente binden.
        /// </summary>
        public BasicGraph CurrentGraph { get; set; }
        /// <summary>
        /// Dies ist die Einstellungen- Komponente. Diese ermöglichte es die GraphStyleParameter zu verändern
        /// und zu speichern
        /// </summary>
        public Settings Settings { get; set; }
        /// <summary>
        /// GraphEditForm ist das Formular zum Bearbeiten und Speichern der Graphen
        /// </summary>
        public GraphEditForm GraphEditForm { get; set; }
        /// <summary>
        /// Das ist die Komponente, die das CSS in das HTML Dokument rendert. 
        /// </summary>
        public SettingsCSS SettingsCSS { get; set; }
        /// <summary>
        /// GraphStyleParmeter erlauben es Farben und Liniensträken zur Laufzeit zu verändern
        /// </summary>
        public GraphStyleParameters GraphStyleParameters { get; set; } = new GraphStyleParameters();
        /// <summary>
        /// Erstellt Instanz des Services
        /// </summary>
        /// <param name="logger">Dependencie Injection ASP .Net Logger</param>
        /// <param name="jsRuntime">Dependencie Injection ASP .Net JSRuntime</param>
        /// <param name="httpClient">Dependencie Injection ASP .Net HttpClient</param>
        public GraphService(ILogger<GraphService> logger, IJSRuntime jsRuntime, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _logger = logger;
            JSRuntime = jsRuntime;
            Task.Run(LoadGraphStyleParameters);
            CurrentGraphModel = GraphFactory.CreateRandomGraph("Unbenannter-Graph", 10, 20, 50, -50);
        }
        /// <summary>
        /// Ruft das unter filename gefundene Graph Modell vom Server ab.
        /// </summary>
        /// <param name="filename">Name oder Dateipfad des Graphen</param>
        /// <returns>Das gefundene Model oder null</returns>
        public async Task<BasicGraphModel> GetGraph(string filename)
        {
            var graphResponse = await _httpClient.GetFromJsonAsync<BasicGraphModelPoco>($"api/Graph/GetGraphModel/{filename}");
            graphResponse.Nodes = graphResponse.NodesPoco.Select(n => new VisualGraph.Shared.Models.Node
            {
                Name = n.Name,
                Classes = n.Classes,
                Edges = n.Edges,
                Id = n.Id,
                Pos = new System.Numerics.Vector2(float.Parse(n.PosXTextPoco.Replace(".", ",")), float.Parse(n.PosYTextPoco.Replace(".", ",")))

            }).ToList();
            foreach (var edge in graphResponse.Edges)
            {
                edge.StartNode = graphResponse.Nodes.FirstOrDefault(x => x.Id == edge.StartNode.Id);
                edge.EndNode = graphResponse.Nodes.FirstOrDefault(x => x.Id == edge.EndNode.Id);
                edge.StartNode.Edges.Add(edge);
                edge.EndNode.Edges.Add(edge);
            }
            return (BasicGraphModel)graphResponse;
        }
        /// <summary>
        /// Läd den Graphen mit dem angegebenen Namen als CurrentGraph
        /// </summary>
        /// <param name="filename">Name oder Dateipfad des Graphen</param>
        /// <returns>Task</returns>
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
        /// <summary>
        /// Ruft alle Graph- Namen vom Server ab.
        /// </summary>
        /// <returns>alle auf dem Server vorhandenen Graph- Namen</returns>
        public async Task<string[]> GetGraphFilenames()
        {
            var graphFilenames = await _httpClient.GetFromJsonAsync<IEnumerable<string>>($"api/Graph/GetGraphFilenames");
            return graphFilenames.ToArray();
        }
        /// <summary>
        /// Speichert den Graph unter angegebenem Dateinamen auf dem Server
        /// </summary>
        /// <param name="filename">Name oder Dateipfad des Graphen</param>
        /// <returns>true, wenn erfolgreich und false bei Fehler</returns>
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
                return await SaveGraph(CurrentGraphModel, filename);

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }
        private async Task<bool> SaveGraph(BasicGraphModel graph, string filename)
        {
            return await (await _httpClient.PostAsJsonAsync($"api/Graph/SaveGraph/{filename}", new BasicGraphModelPoco(graph))).Content.ReadFromJsonAsync<bool>();
        }
        /// <summary>
        /// Berechnet ein automatisches Layout für den aktuellen Graph
        /// </summary>
        /// <param name="scalex">Skalierung entlang der X- Achse </param>
        /// <param name="scaley">Skalierung entlang der Y- Achse </param>
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


                nodes.ForEach(x => geometryGraph.Nodes.Add(new Microsoft.Msagl.Core.Layout.Node(new Ellipse(1, 1, new Microsoft.Msagl.Core.Geometry.Point()), x.Id)));
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
        /// <summary>
        /// Initialisiert die svgpanzoom.js Anwendung und setzt JSInterop Referenz, damit 
        /// Objekte zwischen VisualGraph.Client.dll und JavaScript kommuniziert werden kann
        /// </summary>
        /// <param name="reference">Referenz auf die aktuelle BasicGraph- Komponente</param>
        public async Task InitZoomPan(DotNetObjectReference<BasicGraph> reference)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("InitPanZoom", new object[] { reference, CurrentGraphModel.Name });
            }
            catch { }
        }
        /// <summary>
        /// Zerstört die durch InitZoomPan erstellte instanz von svgpanzoom.js
        /// </summary>
        public async Task DestroyZoomPan()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("DestroyPanZoom");
            }
            catch { }
        }
        /// <summary>
        /// Deaktiviert die Pan- Funktion um andere Drag- Funktionen zu ermöglichen
        /// </summary>
        public async Task DisablePan()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("DisablePan");
            }
            catch { }
        }
        /// <summary>
        /// Aktiviert die Pan- Funktion um andere Drag- Funktionen zu ermöglichen
        /// </summary>
        public async Task EnablePan()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("EnablePan");
            }
            catch { }
        }
        /// <summary>
        /// Ruft über JSInterop die Informationen wie Pan, Zoom und die ViewBox des scgpanzoom.js Conainers ab
        /// </summary>
        /// <returns></returns>
        public async Task<SvgPanZoomInformation> GetSvgPanZoomInformation()
        {
            try
            {
                var svginfo = await JSRuntime.InvokeAsync<SvgPanZoomInformation>("GetPanZoomValues", new object[] { CurrentGraphModel.Name });
                return svginfo;
            }
            catch { return new SvgPanZoomInformation(); };
        }
        /// <summary>
        /// Ruft über JSInterop die Größe des svgpanzoom.js Conainers ab
        /// </summary>
        /// <returns></returns>
        public async Task<SvgContainerInformation> GetSvgContainerInformation()
        {
            try
            {
                var svginfo = await JSRuntime.InvokeAsync<SvgContainerInformation>("GetSvgContainerSizes", new object[] { CurrentGraphModel.Name });
                return svginfo;
            }
            catch { return new SvgContainerInformation(); };
        }
        /// <summary>
        /// Gibt die Größe des Browserfensters zurück
        /// </summary>
        /// <returns>Ganszahlige Werte für Höhe und Weite in Pixel</returns>
        public async Task<BrowserSizes> GetBrowserSizes()
        {
            try
            {
                return await JSRuntime.InvokeAsync<BrowserSizes>("GetBrowserSizes");
            }
            catch { return new BrowserSizes(); }
        }
        /// <summary>
        /// Übersetzt Koordinaten aus Bildschrim- Koordinatensystem ins Graph- Koordinatensystem
        /// </summary>
        /// <param name="x">X- Koordinate</param>
        /// <param name="y">Y- Koordinate</param>
        /// <returns>Punkt Graph- Koordinatensystem</returns>
        public async Task<Point2> GetTranslatedCoordinate(double x, double y)
        {
            try
            {
                var svgInfo = await GetSvgPanZoomInformation();
                var mousePos = await JSRuntime.InvokeAsync<Point2>("GetTranslatedMousePos", new object[] { new { id = CurrentGraphModel.Name, x = x - svgInfo.OffsetLeft, y = y - svgInfo.OffsetTop } });
                return mousePos;
            }
            catch { return new Point2(); }
        }
        /// <summary>
        /// Ruft die JSInterop Methode zum Aktualisieren der BoundingBox (Konvexehülle) der svgpanzoom.js auf
        /// </summary>
        public async Task UpdateBBox()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("UpdateBBox");
            }
            catch { }
        }
        /// <summary>
        /// Ruft die JSInterop Methode zum Anpassen der Größe des Graphen, an den svgpanzoom.js Container auf
        /// </summary>
        public async Task Fit()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("Fit");
            }
            catch { }
        }
        /// <summary>
        /// Ruft die JSInterop Methode zum Zentrieren des graphen
        /// </summary>
        public async Task Center()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("Center");
            }
            catch { }
        }
        /// <summary>
        /// Ruft die JSInterop Methode "Rezize" von svgpanzoom.js auf
        /// </summary>
        public async Task Resize()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("Resize");
            }
            catch { }
        }
        /// <summary>
        /// Führt ruft die Methoden Fit und Center auf
        /// </summary>
        public async Task Crop()
        {
            try
            {
                await Fit();
                await Center();
            }
            catch { }
        }
        /// <summary>
        /// Speichert GraphStyleParameter auf dem Server. Wenn kein Parameter angegeben wurde, 
        /// weden Defaultwerte gespeichert
        /// </summary>
        /// <param name="styleParameters"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Lad die GraphStyleParameter vom Server
        /// </summary>
        /// <returns>Task</returns>
        public async Task LoadGraphStyleParameters()
        {

            GraphStyleParametersPOCO styleParametersPOCO = await _httpClient.GetFromJsonAsync<GraphStyleParametersPOCO>("api/Settings/GetGraphStyle");
            GraphStyleParameters.InitFromPoco(styleParametersPOCO);
            await Rerender();
        }
        /// <summary>
        /// Gibt das Renderfragment für das aktuell geledene Graphmodel zurück
        /// </summary>
        /// <param name="withDefaultCallbacks">(optional) legt fest ob der Graph mit Standard Event- Callbacks gerendert werden soll. default = true</param>
        /// <returns>Renderbares Renderfragment</returns>
        public async Task<RenderFragment> GetRenderFragment(bool withDefaultCallbacks = true)
        {
            if (CurrentGraphModel != null)
            {
                return await BuildBasicGraphFragment(CurrentGraphModel, withDefaultCallbacks);
            }
            return null;
        }
        /// <summary>
        /// Gibt das Renderfragment für das übergebene Graphmodel zurück
        /// </summary>
        /// <param name="graph">Zu rendernder Graph</param>
        /// <param name="withDefaultCallbacks">(optional) legt fest ob der Graph mit Standard Event- Callbacks gerendert werden soll. default = true</param>
        /// <returns>Renderbares Renderfragment</returns>
        public Task<RenderFragment> GetRenderFragment(BasicGraphModel graph, bool withDefaultCallbacks = true)
        {
            var fragment = new RenderFragment(builder =>
            {
                if (graph != null)
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
        /// <summary>
        /// Rendert das HTML- Styletag mit den GraphStyleParametern
        /// </summary>
        /// <returns>Renderbares Renderfragment</returns>
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
        /// <summary>
        /// Rendert das Formular zum bearbeiten der Graph- Style- Parameter
        /// </summary>
        /// <returns>Renderbares Renderfragment</returns>
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
        /// <summary>
        /// Rendert Alle GraphInternalUIs neu
        /// </summary>
        /// <returns></returns>
        public async Task Rerender()
        {
            if (CurrentGraph != null && CurrentGraph.IsRendered)
            {
                await CurrentGraph.ChangedState();
            }
            if (SettingsCSS != null && SettingsCSS.IsRendered)
            {
                await SettingsCSS.ChangedState();
            }
            if (Settings != null && Settings.IsRendered)
            {

                await Settings.ChangedState();
            }
            if (GraphEditForm != null && GraphEditForm.IsRendered)
            {
                await GraphEditForm.ChangedState();
            }
        }
        /// <summary>
        /// Rendert einen einzelnen Knoten des Graphen neu
        /// </summary>
        /// <returns></returns>
        public async Task Rerender(VisualGraph.Shared.Models.Node node = null)
        {

            if (CurrentGraph != null && CurrentGraph.IsRendered)
            {
                NodeComponent component = CurrentGraph.NodeComponents.FirstOrDefault(x => x.Node == node);
                await component.ChangedState();
            }
        }
        /// <summary>
        /// Rendert eine einzelne Kante des Graphen neu
        /// </summary>
        /// <returns></returns>
        public async Task Rerender(VisualGraph.Shared.Models.Edge edge = null)
        {
            if (CurrentGraph != null && CurrentGraph.IsRendered)
            {
                var component = CurrentGraph.EdgeComponents.FirstOrDefault(x => x.Edge == edge);
                await component.ChangedState();
            }

        }
        /// <summary>
        /// Rendert die Komponente des angegebenen Typs neu
        /// </summary>
        /// <typeparam name="T">Typ der neu zu rendernden Komponente. T muss GraphInternalUI sein</typeparam>
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
            else if (typeof(T) == typeof(Settings) && Settings != null && Settings.IsRendered)
            {

                await Settings.ChangedState();
            }
            else if (typeof(T) == typeof(GraphEditForm) && GraphEditForm != null && GraphEditForm.IsRendered)
            {
                await GraphEditForm.ChangedState();
            }
            else if (typeof(T) == typeof(AxisComponent) && CurrentGraph != null)
            {
                await CurrentGraph.CoordinateSystemComponent.ChangedState();
            }

        }
        /// <summary>
        /// Rendert das Formular zum bearbeiten des Graphen
        /// </summary>
        /// <returns>Renderbares Renderfragment</returns>
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