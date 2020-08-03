using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using VisualGraph.Client.Components;
using VisualGraph.Client.Components.Additional;
using VisualGraph.Client.Shared.Models;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.Services
{
    /// <summary>
    /// Dieses Interface bietet Zugriff auf alle relevanten Komponenten im Zusammenhang mit Graphen und 
    /// deren Darstellung
    /// </summary>
    public interface IGraphService
    {
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
        /// GraphStyleParmeter erlauben es Farben und Liniensträken zur Laufzeit zu verändern
        /// </summary>
        public GraphStyleParameters GraphStyleParameters { get; set; }
        /// <summary>
        /// Dies ist die Einstellungen- Komponente. Diese ermöglichte es die GraphStyleParameter zu verändern
        /// und zu speichern
        /// </summary>
        public Settings Settings { get; set; }
        /// <summary>
        /// Das ist die Komponente, die das CSS in das HTML Dokument rendert. 
        /// </summary>
        public SettingsCSS SettingsCSS { get; set; }
        /// <summary>
        /// GraphEditForm ist das Formular zum Bearbeiten und Speichern der Graphen
        /// </summary>
        public GraphEditForm GraphEditForm { get; set; }
        /// <summary>
        /// Läd den Graphen mit dem angegebenen Namen als CurrentGraph
        /// </summary>
        /// <param name="filename">Name oder Dateipfad des Graphen</param>
        /// <returns>Task</returns>
        public Task LoadGraph(string filename);
        /// <summary>
        /// Lad die GraphStyleParameter vom Server
        /// </summary>
        /// <returns>Task</returns>
        public Task LoadGraphStyleParameters();
        /// <summary>
        /// Speichert GraphStyleParameter auf dem Server. Wenn kein Parameter angegeben wurde, 
        /// weden Defaultwerte gespeichert
        /// </summary>
        /// <param name="graphStyleParameters"></param>
        /// <returns></returns>
        public Task SaveGraphStyleParameters(GraphStyleParameters graphStyleParameters = null);
        /// <summary>
        /// Ruft das unter filename gefundene Graph Modell vom Server ab.
        /// </summary>
        /// <param name="filename">Name oder Dateipfad des Graphen</param>
        /// <returns>Das gefundene Model oder null</returns>
        public Task<BasicGraphModel> GetGraph(string filename);
        /// <summary>
        /// Ruft alle Graph- Namen vom Server ab.
        /// </summary>
        /// <returns>alle auf dem Server vorhandenen Graph- Namen</returns>
        public Task<string[]> GetGraphFilenames();
        /// <summary>
        /// Speichert den Graph unter angegebenem Dateinamen auf dem Server
        /// </summary>
        /// <param name="filename">Name oder Dateipfad des Graphen</param>
        /// <returns>true, wenn erfolgreich und false bei Fehler</returns>
        public Task<bool> SaveGraph(string filename = null);
        /// <summary>
        /// Initialisiert die svgpanzoom.js Anwendung und setzt JSInterop Referenz, damit 
        /// Objekte zwischen VisualGraph.Client.dll und JavaScript kommuniziert werden kann
        /// </summary>
        /// <param name="reference">Referenz auf die aktuelle BasicGraph- Komponente</param>
        public Task InitZoomPan(DotNetObjectReference<BasicGraph> reference);
        /// <summary>
        /// Zerstört die durch InitZoomPan erstellte instanz von svgpanzoom.js
        /// </summary>
        public Task DestroyZoomPan();
        /// <summary>
        /// Deaktiviert die Pan- Funktion um andere Drag- Funktionen zu ermöglichen
        /// </summary>
        public Task DisablePan();
        /// <summary>
        /// Aktiviert die Pan- Funktion um andere Drag- Funktionen zu ermöglichen
        /// </summary>
        public Task EnablePan();
        /// <summary>
        /// Ruft die JSInterop Methode zum Aktualisieren der BoundingBox (Konvexehülle) der svgpanzoom.js auf
        /// </summary>
        public Task UpdateBBox();
        /// <summary>
        /// Ruft die JSInterop Methode zum Anpassen der Größe des Graphen, an den svgpanzoom.js Container auf
        /// </summary>
        public Task Fit();
        /// <summary>
        /// Ruft die JSInterop Methode zum Zentrieren des graphen
        /// </summary>
        public Task Center();
        /// <summary>
        /// Führt ruft die Methoden Fit und Center auf
        /// </summary>
        public Task Crop();
        /// <summary>
        /// Ruft die JSInterop Methode "Rezize" von svgpanzoom.js auf
        /// </summary>
        public Task Resize();
        /// <summary>
        /// Ruft über JSInterop die Informationen wie Pan, Zoom und die ViewBox des scgpanzoom.js Conainers ab
        /// </summary>
        /// <returns></returns>
        public Task<SvgPanZoomInformation> GetSvgPanZoomInformation();
        /// <summary>
        /// Ruft über JSInterop die Größe des svgpanzoom.js Conainers ab
        /// </summary>
        /// <returns></returns>
        public Task<SvgContainerInformation> GetSvgContainerInformation();
        /// <summary>
        /// Übersetzt Koordinaten aus Bildschrim- Koordinatensystem ins Graph- Koordinatensystem
        /// </summary>
        /// <param name="x">X- Koordinate</param>
        /// <param name="y">Y- Koordinate</param>
        /// <returns>Punkt Graph- Koordinatensystem</returns>
        public Task<Point2> GetTranslatedCoordinate(double x, double y);
        /// <summary>
        /// Berechnet ein automatisches Layout für den aktuellen Graph
        /// </summary>
        /// <param name="scalex">Skalierung entlang der X- Achse </param>
        /// <param name="scaley">Skalierung entlang der Y- Achse </param>
        public Task LayoutGraph(double scalex = 2.2, double scaley = 2.2);
        /// <summary>
        /// Gibt die Größe des Browserfensters zurück
        /// </summary>
        /// <returns>Ganszahlige Werte für Höhe und Weite in Pixel</returns>
        public Task<BrowserSizes> GetBrowserSizes();
        /// <summary>
        /// Gibt das Renderfragment für das übergebene Graphmodel zurück
        /// </summary>
        /// <param name="graphModel">Zu rendernder Graph</param>
        /// <param name="withDefaultCallbacks">(optional) legt fest ob der Graph mit Standard Event- Callbacks gerendert werden soll. default = true</param>
        /// <returns>Renderbares Renderfragment</returns>
        public Task<RenderFragment> GetRenderFragment(BasicGraphModel graphModel, bool withDefaultCallbacks = true);
        /// <summary>
        /// Gibt das Renderfragment für das aktuell geledene Graphmodel zurück
        /// </summary>
        /// <param name="withDefaultCallbacks">(optional) legt fest ob der Graph mit Standard Event- Callbacks gerendert werden soll. default = true</param>
        /// <returns>Renderbares Renderfragment</returns>
        public Task<RenderFragment> GetRenderFragment(bool withDefaultCallbacks = true);
        /// <summary>
        /// Rendert das HTML- Styletag mit den GraphStyleParametern
        /// </summary>
        /// <returns>Renderbares Renderfragment</returns>
        public Task<RenderFragment> GetCssMarkup();
        /// <summary>
        /// Rendert das Formular zum bearbeiten der Graph- Style- Parameter
        /// </summary>
        /// <returns>Renderbares Renderfragment</returns>
        public Task<RenderFragment> GetSettingsRenderFragment();
        /// <summary>
        /// Rendert das Formular zum bearbeiten des Graphen
        /// </summary>
        /// <returns>Renderbares Renderfragment</returns>
        public Task<RenderFragment> GetEditFormRenderFragment();
       
        /// <summary>
        /// Rendert Alle GraphInternalUIs neu
        /// </summary>
        /// <returns></returns>
        public Task Rerender();
        /// <summary>
        /// Rendert einen einzelnen Knoten des Graphen neu
        /// </summary>
        /// <returns></returns>
        public Task Rerender(Node instance);
        /// <summary>
        /// Rendert eine einzelne Kante des Graphen neu
        /// </summary>
        /// <returns></returns>
        public Task Rerender(Edge instance);
        /// <summary>
        /// Rendert die Komponente des angegebenen Typs neu
        /// </summary>
        /// <typeparam name="T">Typ der neu zu rendernden Komponente. T muss GraphInternalUI sein</typeparam>
        public Task Rerender<T>() where T : GraphInternalUI;
    }
}