using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using VisualGraph.Client.Services;
using VisualGraph.Client.Shared.Algorithm;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.Components.Additional
{
    /// <summary>
    /// Basisklasse für Seiten die einen Algorithmus auf Graphen ausführen
    /// </summary>
    public abstract class AlgorithmPage : ComponentBase
    {
        // Klassen für die Visualisierung der Algorithmen
        /// <summary>
        /// CSS- Klasse für den Startknoten des Algorithmus
        /// </summary>
        public static string StartNodeClass = "startnode";
        /// <summary>
        /// CSS- Klasse für den Endknoten des Algorithmus
        /// </summary>
        public static string EndNodeClass = "endnode";
        /// <summary>
        /// CSS- Klasse für die günstigste Route des Algorithmus
        /// </summary>
        public static string RouteClass = "vgpath";
        /// <summary>
        /// CSS- Klasse für die Vorschau der erreichbaren Knoten und Kanten
        /// </summary>
        public static string NodeReachableClass = "vgpathpreview";
        /// <summary>
        /// CSS- Klasse für nicht erreichbare Knoten
        /// </summary>
        public static string NodeNotReachableClass = "vgnodeunreachable";
        /// <summary>
        /// CSS- Klasse für den aktuell geprüften knoten
        /// </summary>
        public static string IterationCurrentNodeClass = "iterationnode";
        /// <summary>
        /// CSS- Klasse für den aktuell geprüften knoten
        /// </summary>
        public static string IterationClass = "vgiteration";

        /// <summary>
        /// Dialog zum korrigieren negativer Kanten
        /// </summary>
        protected Modal ModalNegativeEdgeWeigths { get; set; }
        /// <summary>
        /// Objekt das das Interface IGraphAlgorithm implementiert. 
        /// </summary>
        protected IGraphAlgorithm Algorithm { get; set; }
        /// <summary>
        /// Graphname, Dateinname oder Dateipfad des initial zu ladendenden Graphen
        /// </summary>
        [Parameter]
        public string Filename { get; set; }
        /// <summary>
        /// Ermöglicht Zugriff auf alle Operationen im Zusammenhang mit graphen.
        /// </summary>
        [Inject]
        protected IGraphService GraphService { get; set; }
        /// <summary>
        /// Ermöglicht es, direkt auf JavaScript Funktionen zuzugreifen
        /// </summary>
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }
        /// <summary>
        /// Ermöglicht das einblenden von Toast Informationen
        /// </summary>
        [Inject]
        protected IToastService toastService { get; set; }
        /// <summary>
        /// Diese Liste hält die Referenzen zu den Knoten und die Kosten zu diesen Knoten
        /// </summary>
        protected List<Tuple<Node, double>> ShortestRouteToEndnode;
        /// <summary>
        /// Diese Egenschaft gibt immer das kleinste Kantengewicht zurück
        /// </summary>
        protected double LowestWeight => GraphService.CurrentGraphModel.Edges.Min(x => x.Weight);
        /// <summary>
        /// Kosten für den aktuellen Pfad zwischen Start- und Endknoten
        /// </summary>
        protected double PathCost = 0;
        /// <summary>
        /// Läd den über den Parameter übermittelten Graphen
        /// </summary>
        protected override void OnInitialized()
        {
            if (Filename != null && Filename != "")
            {
                GraphService.LoadGraph(Filename);
            }
        }
        /// <summary>
        /// Bindet die Callback- Methode zum Anzeigen der erreichbaren Knoten an das Knoten- Klick Event
        /// </summary>
        /// <param name="firstRender"></param>
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                GraphService.CurrentGraph.NodeClick += (sender, args) =>
                {
                    resetPreview();
                    resetUnreachables();
                    if (args.Target.IsActive && !GraphService.CurrentGraphModel.Nodes.Any(x => x.Classes.Contains(AlgorithmPage.StartNodeClass) && !args.Target.Classes.Contains(AlgorithmPage.StartNodeClass)))
                    {
                        DijkstraAlgorithm runner = new DijkstraAlgorithm(GraphService.CurrentGraphModel, args.Target.Id);
                        runner.Iterate(auto: true);
                        var lastresult = runner.Results.Last();
                        var nodes = lastresult.Distances.Where(x => x.Value != double.PositiveInfinity).Select(x => x.Key).ToList();
                        var previewEdges = nodes.SelectMany(x => x.Edges.Where(y => x == y.StartNode)).ToList();
                        nodes.ForEach(x => { if (!x.Classes.Contains(NodeReachableClass)) x.Classes.Add(AlgorithmPage.NodeReachableClass); });
                        previewEdges.ForEach(x => { if (!x.Classes.Contains(NodeReachableClass)) x.Classes.Add(AlgorithmPage.NodeReachableClass); });
                    }
                };
            }
        }
        /// <summary>
        /// Setzt CSS Klasse für alle errichbaren Knoten und Kanten
        /// in dem die Methode auf die Ergebnisse der letzten Algorithmus Ergebnisse zugreift
        /// </summary>
        protected void setPreview()
        {
            var lastresults = Algorithm.Results.Last();

            var allresults = Algorithm.Results;
            var nodes = allresults.SelectMany(x => x.Distances.Where(x => x.Value != double.PositiveInfinity).Select(x => x.Key)).ToList();
            nodes.ForEach(x =>
            {
                x.Classes.Remove(NodeNotReachableClass);
                x.Classes.Remove(IterationCurrentNodeClass);
                x.Classes.Remove(IterationClass);

            });

            var previewEdges = nodes.SelectMany(x => x.Edges.Where(y => x == y.EndNode && !y.StartNode.Classes.Contains(NodeNotReachableClass))).ToList();
            lastresults.CurrentNode.Classes.Add(IterationCurrentNodeClass);
            lastresults.CurrentNode.Classes.Add(IterationClass);
            previewEdges.ForEach(x =>
            {
                x.Classes.Remove(NodeNotReachableClass);
                if (!x.EndNode.Classes.Contains(IterationClass))
                {
                    x.EndNode.Classes.Add(IterationClass);
                }

            });
        }
        /// <summary>
        /// Entfernt die Vorschau- CSS- Klasssen der erreichbarten Knoten und Kanten
        /// </summary>
        protected void resetPreview()
        {
            removeClassFromNodesAndEdges(NodeReachableClass);
        }
        /// <summary>
        /// Fügt 
        /// </summary>
        protected void setUnreachables()
        {
            GraphService.CurrentGraphModel.Edges.ForEach(x => x.Classes.Add(NodeNotReachableClass));
            GraphService.CurrentGraphModel.Nodes.ForEach(x => x.Classes.Add(NodeNotReachableClass));
        }
        /// <summary>
        /// Entfernt von den nicht erreichbaren Knoten und Kanten die entsrechende CSS- Klasse 
        /// </summary>
        protected void resetUnreachables()
        {
            removeClassFromNodesAndEdges(NodeNotReachableClass);
        }
        /// <summary>
        /// Visualisiert die günstigste route
        /// </summary>
        protected void visualizeRoute()
        {
            for (int nodeindex = 0; nodeindex < ShortestRouteToEndnode.Count; nodeindex++)
            {
                var node = ShortestRouteToEndnode[nodeindex].Item1;

                if (!node.Classes.Contains(RouteClass))
                    node.Classes.Add(RouteClass);

                if (nodeindex < ShortestRouteToEndnode.Count - 1)
                {
                    var node1 = ShortestRouteToEndnode[nodeindex + 1].Item1;
                    if (!node1.Classes.Contains(RouteClass))
                        node1.Classes.Add(RouteClass);
                    Edge edge;
                    if (GraphService.CurrentGraphModel.IsDirected)
                        edge = node.Edges.FirstOrDefault(x => (x.StartNode == node && x.EndNode == node1));
                    else
                    {
                        var edges = node.Edges.Where(x => (x.StartNode == node && x.EndNode == node1 || x.StartNode == node1 && x.EndNode == node));
                        edge = edges.FirstOrDefault(x => x.Weight == edges.Min(y => y.Weight));
                    }

                    if (edge != null)
                    {
                        if (!edge.Classes.Contains(RouteClass))
                            edge.Classes.Add(RouteClass);
                        PathCost += edge.Weight;
                    }
                }
            }
        }
        /// <summary>
        /// Entfernt die übergebene CSS-Klasse von allen Kanten und Knoten
        /// </summary>
        /// <param name="classname">zu Entfernende Klasse</param>
        protected void removeClassFromNodesAndEdges(string classname)
        {
            GraphService.CurrentGraphModel.Nodes.ForEach(x => x.Classes.Remove(classname));
            GraphService.CurrentGraphModel.Edges.ForEach(x => x.Classes.Remove(classname));
        }
        /// <summary>
        /// Fügt die übergebene CSS-Klasse allen Kanten und Knoten hinzu
        /// </summary>
        /// <param name="classname">cssclass</param>
        private void addClassToNodesAndEdges(string classname)
        {
            GraphService.CurrentGraphModel.Nodes.ForEach(x => { if (!x.Classes.Contains(classname)) x.Classes.Add(classname); });
            GraphService.CurrentGraphModel.Edges.ForEach(x => { if (!x.Classes.Contains(classname)) x.Classes.Add(classname); });
        }

        /// <summary>
        /// Fürt den Algorithmus aus
        /// </summary>
        /// <param name="autostep">Gibt an ob ein Einzelschritt oder der gesamte Algorithmus ausgefürt werden soll</param>
        protected abstract void RunAlgorithm(bool autostep = false);
        /// <summary>
        /// Wenn der Algorithmus nicht mit negativen Kantengewichten umgehen kann, werden diese mit dieser Methode korrigiert. 
        /// Das kleinste Gewicht wird als "0" btreachtet.  
        /// </summary>
        protected async void CorrectEdgeWeights()
        {
            var absVal = Math.Abs(LowestWeight);
            foreach (var edge in GraphService.CurrentGraphModel.Edges)
            {
                edge.Weight += absVal;
            }
            await GraphService.Rerender();
        }
        /// <summary>
        /// Entfernt setzt die Visualisierung der günstigsten Route zurück
        /// </summary>
        protected void clearRouteClasses()
        {
            removeClassFromNodesAndEdges(RouteClass);
        }
        

        /// <summary>
        /// Setzt den Algorithmus zurück
        /// </summary>
        private void resetAlgorithm()
        {
            Algorithm = null;
            ShortestRouteToEndnode = null;
             
        }
        /// <summary>
        /// Zeigt einen Dialog an, wenn negative Kantengewichte gefunden wurden.
        /// </summary>
        protected void CheckNegativeAndShowModal()
        {
            if (!Algorithm.CanHandleNegativeEgdes && LowestWeight < 0)
            {
                if (ModalNegativeEdgeWeigths == null)
                {
                    toastService.ShowError("Der Algorithmus kann nicht auf Graphen mit negativen kantengewichten ausgeführt werden");
                    return;
                }
                    
                ModalNegativeEdgeWeigths.Show();
            }
        }

    }
}
