using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VisualGraph.Data.Additional.Interfaces;

namespace VisualGraph.Data.Additional.Models
{
    public static class CoordinateAxis
    {
        public static string CoordinateSystemColor { get; set; } = "rgba(0,0,0,0.2)";
        public static double CoordinateAxisMarkerHeight { get; set; } = 0.5;
        public static int AddToRenderTree(RenderTreeBuilder builder, int sequence, double minX, double minY , double maxX, double maxY)
        {
            builder.OpenElement(sequence++, "line");
            builder.AddAttribute(sequence++, "x1", minX.ToString(CultureInfo.InvariantCulture));
            builder.AddAttribute(sequence++, "y1", 0);
            builder.AddAttribute(sequence++, "x2", maxX.ToString(CultureInfo.InvariantCulture));
            builder.AddAttribute(sequence++, "y2", 0);
            builder.AddAttribute(sequence++, "style", $"stroke:{CoordinateSystemColor}; stroke-width:0.1;");
            builder.CloseElement();

            builder.OpenElement(sequence++, "line");
            builder.AddAttribute(sequence++, "x1", 0);
            builder.AddAttribute(sequence++, "y1", minY.ToString(CultureInfo.InvariantCulture));
            builder.AddAttribute(sequence++, "x2", 0);
            builder.AddAttribute(sequence++, "y2", maxY.ToString(CultureInfo.InvariantCulture));
            builder.AddAttribute(sequence++, "style", $"stroke:{CoordinateSystemColor}; stroke-width:0.1;");
            builder.CloseElement();

            var stepSizeX = 5;
            var stepSizeY = 5;
            var text = ((int)(minX/10))*10;
            for (var xstep = text; xstep < maxX; xstep += stepSizeX)
            {
                builder.OpenElement(sequence++, "line");
                builder.AddAttribute(sequence++, "x1", xstep.ToString(CultureInfo.InvariantCulture));
                builder.AddAttribute(sequence++, "y1", (-CoordinateAxisMarkerHeight).ToString(CultureInfo.InvariantCulture));
                builder.AddAttribute(sequence++, "x2", xstep.ToString(CultureInfo.InvariantCulture));
                builder.AddAttribute(sequence++, "y2", (CoordinateAxisMarkerHeight).ToString(CultureInfo.InvariantCulture));
                builder.AddAttribute(sequence++, "style", $"stroke:{CoordinateSystemColor}; stroke-width:0.1;");
                builder.CloseElement();

                builder.OpenElement(sequence++, "text");
                builder.AddAttribute(sequence++, "x", Convert.ToDouble(xstep).ToString(CultureInfo.InvariantCulture));
                builder.AddAttribute(sequence++, "y", Convert.ToDouble(-0.5).ToString(CultureInfo.InvariantCulture));
                builder.AddAttribute(sequence++, "fill", CoordinateSystemColor);
                builder.AddAttribute(sequence++, "font-size", "1");
                builder.AddAttribute(sequence++, "class", "axis-label");
                builder.AddContent(sequence++, ((int)(xstep)).ToString(CultureInfo.InvariantCulture));
                builder.CloseElement();

            }
            text = ((int)(minY / 10)) * 10;
            for (var ystep = text; ystep < maxY; ystep += stepSizeY)
            {
                builder.OpenElement(sequence++, "line");
                builder.AddAttribute(sequence++, "x1", (-CoordinateAxisMarkerHeight).ToString(CultureInfo.InvariantCulture));
                builder.AddAttribute(sequence++, "y1", ystep.ToString(CultureInfo.InvariantCulture));
                builder.AddAttribute(sequence++, "x2", (CoordinateAxisMarkerHeight).ToString(CultureInfo.InvariantCulture));
                builder.AddAttribute(sequence++, "y2", ystep.ToString(CultureInfo.InvariantCulture));
                builder.AddAttribute(sequence++, "style", $"stroke:{CoordinateSystemColor}; stroke-width:0.1;");
                builder.CloseElement();
                builder.OpenElement(sequence++, "text");
                builder.AddAttribute(sequence++, "x", Convert.ToDouble(-1.5).ToString(CultureInfo.InvariantCulture));
                builder.AddAttribute(sequence++, "y", Convert.ToDouble(ystep).ToString(CultureInfo.InvariantCulture));
                builder.AddAttribute(sequence++, "fill", CoordinateSystemColor);
                builder.AddAttribute(sequence++, "class", "axis-label");
                builder.AddAttribute(sequence++, "font-size", "1");
                builder.AddContent(sequence++, ((int)(ystep)).ToString(CultureInfo.InvariantCulture));
                builder.CloseElement();

            }

            return sequence;
        }
    }
}
