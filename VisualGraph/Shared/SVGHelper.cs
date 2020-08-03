using System.Globalization;

namespace VisualGraph.Shared
{
    /// <summary>
    /// Hilfsklasse um SVG- Elemente zu erzeugen
    /// </summary>
    public static class SVGHelper
    {
        /// <summary>
        /// Erzeugt eine Linie
        /// </summary>
        /// <param name="x1">X- Von</param>
        /// <param name="y1">Y- Von</param>
        /// <param name="x2">X- Nach</param>
        /// <param name="y2">Y- Nach</param>
        /// <param name="stroke">Linienfarbe (HTML-farbe (Text, rgb(), rgba(), hex))</param>
        /// <param name="strokewidth"></param>
        /// <returns></returns>
        public static string Line(double x1, double y1, double x2, double y2, string stroke = "black", double strokewidth = 0.1)
        {
            string markup = "<line ";
            markup += $"x1=\"{x1.ToString(CultureInfo.InvariantCulture)}\" ";
            markup += $"y1=\"{y1.ToString(CultureInfo.InvariantCulture)}\" ";
            markup += $"x2=\"{x2.ToString(CultureInfo.InvariantCulture)}\" ";
            markup += $"y2=\"{y2.ToString(CultureInfo.InvariantCulture)}\" ";
            markup += $"style=\"{$"stroke:{stroke}; stroke-width:{strokewidth.ToString(CultureInfo.InvariantCulture)};"}\" ";
            markup += $"/>";
            return markup;
        }
        /// <summary>
        /// Erstellt einen SVG- Text
        /// </summary>
        /// <param name="x">Position- X</param>
        /// <param name="y">Position- Y</param>
        /// <param name="text">Darzustellender Text</param>
        /// <param name="fontsize">Schriftgröße</param>
        /// <param name="color">Textfarbe</param>
        /// <returns></returns>
        public static string Text(double x, double y, string text, double fontsize = 1.0, string color = "black")
        {
            string markup = "";
            markup += $"<text ";
            markup += $"x=\"{x.ToString(CultureInfo.InvariantCulture)}\" ";
            markup += $"text-anchor=\"middle\"";
            markup += $"y=\"{y.ToString(CultureInfo.InvariantCulture)}\" ";
            markup += $"fill=\"{color}\"";
            markup += $"font-size=\"{fontsize.ToString(CultureInfo.InvariantCulture)}\" ";
            markup += $"class=\"axis-label\">{text}</text>";
            return markup;
        }
    }
}