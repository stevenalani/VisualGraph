namespace VisualGraph.Client.Shared.Models
{
    /// <summary>
    /// Datenstruktur für JSInteraktion
    /// </summary>
    public class SvgContainerInformation
    {
        /// <summary>
        /// Weite des Elternelements, in welchem das SVG- Element platziert ist
        /// </summary>
        public double ParentWidth { get; set; }
        /// <summary>
        /// Höhe des Elternelements, in welchem das SVG- Element platziert ist
        /// </summary>
        public double ParentHeight { get; set; }
        /// <summary>
        /// Abstand des Elternelements zum linken Browserrand 
        /// </summary>
        public double OffsetLeft { get; set; }
        /// <summary>
        /// Abstand des Elternelements zum oberen Browserrand 
        /// </summary>
        public double OffsetTop { get; set; }
    }
    /// <summary>
    /// Informationen zur panzoom.js instanz
    /// </summary>
    public class SvgPanZoomInformation : SvgContainerInformation
    {
        /// <summary>
        /// X- Verschiebung des Graphen innerhalb des SVG
        /// </summary>
        public double PanX { get; set; }
        /// <summary>
        /// Y- Verschiebung des Graphen innerhalb des SVG
        /// </summary>
        public double PanY { get; set; }
        /// <summary>
        /// Zoomstufe des Graphen innerhalb des SVG
        /// </summary>
        public double Zoom { get; set; }
        /// <summary>
        /// Höhe des panzoom.js Containers
        /// </summary>
        public double PanZoomHeight { get; set; }
        /// <summary>
        /// Weite des panzoom.js Containers
        /// </summary>
        public double PanZoomWidth { get; set; }
        /// <summary>
        /// Höhe der SVG- Viewbox
        /// </summary>
        public double ViewBoxHeight { get; set; }
        /// <summary>
        /// Weite der SVG- Viewbox
        /// </summary>
        public double ViewBoxWidth { get; set; }
        /// <summary>
        /// X- Kootdinate des Mittelpunkt des Containers
        /// </summary>
        public double CenterX { get; set; }
        /// <summary>
        /// Y- Kootdinate des Mittelpunkt des Containers
        /// </summary>
        public double CenterY { get; set; }
    }
    /// <summary>
    /// Datenstruktur für JSInteraktion
    /// Browser- Informationen
    /// </summary>
    public class BrowserSizes
    {
        /// <summary>
        /// Browserfensterhöhe
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Browserfensterweite
        /// </summary>
        public int Height { get; set; }
    }

}
