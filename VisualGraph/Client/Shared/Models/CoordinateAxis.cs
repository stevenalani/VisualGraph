using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Numerics;
using VisualGraph.Shared;
namespace VisualGraph.Client.Shared.Models
{
    public static class CoordinateAxis
    {
        public static string CoordinateSystemColor { get; set; } = "rgba(0,0,0,0.2)";
        public static double CoordinateAxisMarkerHeight { get; set; } = 0.5;
        public static double CoordinateAxisFontSize { get; set; } = 1;
        public static Vector2 CoordinateSystemStepsize = new Vector2(5);
        public static double CoordinateSystemPadding = 10.5;
        public static MarkupString GenerateForGraphRange(double minX = 0, double minY = 0, double maxX = 0, double maxY = 0, double padding = 10.5)
        {
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
        public static MarkupString GenerateForGraphRange2(double minX = 0, double minY = 0, double maxX = 0, double maxY = 0, double padding = 10.5)
        {


            minX = minX > 0 ? -padding : minX - padding;
            minY = minY > 0 ? -padding : minY - padding;
            maxX = maxX + padding;
            maxY = maxY + padding;

            string markup = "";
            markup += "<line ";
            markup += $"x1=\"{minX.ToString(CultureInfo.InvariantCulture)}\" ";
            markup += $"y1=\"{0}\" ";
            markup += $"x2=\"{maxX.ToString(CultureInfo.InvariantCulture)}\" ";
            markup += $"y2=\"{0}\" ";
            markup += $"style=\"{$"stroke:{CoordinateSystemColor}; stroke-width:0.1;"}\" ";
            markup += $" marker-start=\"url(#axisarrow)\" marker-end=\"url(#axisarrow)\"/>";
            markup += "<line ";
            markup += $"x1=\"{0}\" ";
            markup += $"y1=\"{minY.ToString(CultureInfo.InvariantCulture)}\" ";
            markup += $"x2=\"{0}\" ";
            markup += $"y2=\"{maxY.ToString(CultureInfo.InvariantCulture)}\" ";
            markup += $"style=\"{$"stroke:{CoordinateSystemColor}; stroke-width:0.1;"}\" ";
            markup += $" marker-start=\"url(#axisarrow)\" marker-end=\"url(#axisarrow)\"/>";
            var stepSizeX = 5;
            var stepSizeY = 5;
            var text = ((int)(minX / 10)) * 10;
            for (var xstep = text; xstep < maxX; xstep += stepSizeX)
            {
                markup += SVGHelper.Line(xstep, -CoordinateAxisMarkerHeight, xstep, CoordinateAxisMarkerHeight, CoordinateSystemColor);
                markup += SVGHelper.Text(xstep - 0.75, -CoordinateAxisFontSize, xstep.ToString(CultureInfo.InvariantCulture), 1, CoordinateSystemColor);
            }
            text = (int)(minY / 10) * 10;
            for (var ystep = text; ystep < maxY; ystep += stepSizeY)
            {
                markup += SVGHelper.Line(-CoordinateAxisMarkerHeight, ystep, CoordinateAxisMarkerHeight, ystep, CoordinateSystemColor);
                markup += SVGHelper.Text(-CoordinateAxisFontSize * 2, ystep + 0.5, ystep.ToString(CultureInfo.InvariantCulture), CoordinateAxisFontSize, CoordinateSystemColor);
            }
            MarkupString markupString = new MarkupString(markup);
            return markupString;
        }

    }
}
