using System;
using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;

namespace VisualGraph.Client.Shared.Models
{
    /// <summary>
    /// Diese Klasse kapselt alle Informationen für den SVG- Container in welchem Graphen gespeichert werden
    /// </summary>
    public class SvgDisplay
    {
        private double GraphWidth { get; set; }
        private double GraphHeight { get; set; }


        private Vector2 UpperLeft = Vector2.Zero;
        /// <summary>
        /// Erstellt eine Instanz der Klasse
        /// </summary>
        /// <param name="minx"></param>
        /// <param name="miny"></param>
        /// <param name="maxx"></param>
        /// <param name="maxy"></param>
        public SvgDisplay(double minx, double miny, double maxx, double maxy)
        {
            UpperLeft = new Vector2((float)minx, (float)miny);
            GraphWidth = Math.Abs(minx) + Math.Abs(maxx);
            GraphHeight = Math.Abs(miny) + Math.Abs(maxy);
        }

        /// <summary>
        /// Aktualisiert die Höhe und Weite des Displays
        /// </summary>
        /// <param name="svgPanZoomInformation"></param>
        /// <returns></returns>
        public Task<bool> UpdateDisplaySettings(SvgPanZoomInformation svgPanZoomInformation)
        {
            bool hasChanges = false;
            if (svgPanZoomInformation == null) return Task.FromResult(hasChanges);
            if (svgPanZoomInformation.PanZoomHeight != GraphHeight)
            {
                GraphHeight = svgPanZoomInformation.PanZoomHeight;
                hasChanges = true;
            }

            if (GraphWidth != svgPanZoomInformation.PanZoomWidth)
            {
                GraphWidth = svgPanZoomInformation.PanZoomWidth;
                hasChanges = true;
            }

            return Task.FromResult(hasChanges);
        }
        /// <summary>
        /// Gibt den SVG- ViewBox String aus
        /// </summary>
        public string ViewBox =>
            $"{UpperLeft.X.ToString(CultureInfo.InvariantCulture)}," +
            $"{UpperLeft.Y.ToString(CultureInfo.InvariantCulture)}," +
            $"{GraphWidth.ToString(CultureInfo.InvariantCulture) }," +
            $"{GraphHeight.ToString(CultureInfo.InvariantCulture) }";
    }
}
