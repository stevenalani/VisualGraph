using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Numerics;
using VisualGraph.Shared;
namespace VisualGraph.Client.Shared.Models
{
    /// <summary>
    /// Erstellt ein Koordinatensystem als HTML- Markup
    /// </summary>
    public static class CoordinateAxis
    {
        /// <summary>
        /// Farbe in der das Koordinatensystem dargestellt werden soll.
        /// </summary>
        public static string CoordinateSystemColor { get; set; } = "rgba(0,0,0,0.2)";
        /// <summary>
        /// Höhe der Markierungen entlang der Achsen
        /// </summary>
        public static double CoordinateAxisMarkerHeight { get; set; } = 0.5;
        /// <summary>
        /// Schriftgröße für Achsenbeschriftungen
        /// </summary>
        public static double CoordinateAxisFontSize { get; set; } = 1;
        /// <summary>
        /// Schritte für Achsenbeschriftung. Marker alle x Einheiten
        /// </summary>
        public static Vector2 CoordinateSystemStepsize = new Vector2(5);
        /// <summary>
        /// Verlängert die Achsen über die Knovexehülle des Graphen hinaus
        /// </summary>
        public static double CoordinateSystemPadding = 10.5;
        /// <summary>
        /// Erstellt die HTML- Struktur des Koordinatensystems
        /// </summary>
        /// <param name="minX">Kleinster X- Wert des Graphen</param>
        /// <param name="minY">Kleinster Y- Wert des Graphen</param>
        /// <param name="maxX">Größter X- Wert des Graphen</param>
        /// <param name="maxY">Größter Y- Wert des Graphen</param>
        /// <returns></returns>
        public static MarkupString GenerateForGraphRange(double minX = 0, double minY = 0, double maxX = 0, double maxY = 0)
        {
            var padding = CoordinateSystemPadding;
            minX = minX > 0 ? -padding : minX - padding;
            minY = minY > 0 ? -padding : minY - padding;
            maxX = maxX + padding;
            maxY = maxY + padding;
            var stepSizeX = 5;
            var stepSizeY = 5;
            string textmarkup = "";
            string linemarkup = $"<path ";
            linemarkup += $"d=\"M{minX.ToString(CultureInfo.InvariantCulture)} 0 ";
            for (var xstep = ((int)(minX / 10)) * 10; xstep < maxX; xstep += stepSizeX)
            {
                linemarkup += $"L{xstep} 0 ";
                textmarkup += SVGHelper.Text(xstep, -CoordinateAxisFontSize, xstep.ToString(CultureInfo.InvariantCulture), 1, CoordinateSystemColor);
            }
            linemarkup += $"\" style=\"stroke:{CoordinateSystemColor}; stroke-width:0.1; fill:none; ";
            linemarkup += $"marker-start: url(#axisarrow); ";
            linemarkup += $"marker-mid: url(#markerLineX); ";
            linemarkup += $"marker-end: url(#axisarrow);\"/>";
            linemarkup += $"<path ";
            linemarkup += $"d=\"M0 {minY.ToString(CultureInfo.InvariantCulture)} ";
            for (var ystep = ((int)(minY / 10)) * 10; ystep < maxY; ystep += stepSizeY)
            {
                linemarkup += $"L0 {ystep} ";
                textmarkup += SVGHelper.Text(CoordinateAxisFontSize, ystep + CoordinateAxisFontSize / 2, ystep.ToString(CultureInfo.InvariantCulture), 1, CoordinateSystemColor);
            }
            linemarkup += $"\" style=\"stroke:{CoordinateSystemColor}; stroke-width:0.1; fill:none; ";
            linemarkup += $"marker-start: url(#axisarrow); ";
            linemarkup += $"marker-mid: url(#markerLineX); ";
            linemarkup += $"marker-end: url(#axisarrow);\"/>";
            return new MarkupString(linemarkup + textmarkup);
        }
    }
}
