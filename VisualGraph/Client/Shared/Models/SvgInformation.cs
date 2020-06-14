namespace VisualGraph.Client.Shared.Models
{
    public class SvgContainerInformation
    {
        public double ParentWidth { get; set; }
        public double ParentHeight { get; set; }
        public double OffsetLeft { get; set; }
        public double OffsetTop { get; set; }
    }
    public class SvgPanZoomInformation : SvgContainerInformation
    {
        public double PanX { get; set; }
        public double PanY { get; set; }
        public double Zoom { get; set; }
        public double PanZoomHeight { get; set; }
        public double PanZoomWidth { get; set; }
        public double ViewBoxHeight { get; set; }
        public double ViewBoxWidth { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
    }
    public class BrowserSizes
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

}
