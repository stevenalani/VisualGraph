using Microsoft.AspNetCore.Components;
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
        public static double CoordinateAxisFontSize { get; set; } = 1;

        public static MarkupString GenerateForGraphRange(double minX = 0, double minY = 0, double maxX = 0,  double maxY = 0, double padding = 10.5)
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
                markup += VisualGraph.Data.Additional.SVGHelper.Line(xstep, -CoordinateAxisMarkerHeight,xstep, CoordinateAxisMarkerHeight, CoordinateSystemColor);
                markup += VisualGraph.Data.Additional.SVGHelper.Text(xstep - 0.75, -CoordinateAxisFontSize, xstep.ToString(CultureInfo.InvariantCulture),1, CoordinateSystemColor);                
            }
            text = ((int)(minY / 10)) * 10;
            for (var ystep = text; ystep < maxY; ystep += stepSizeY)
            {
                markup += VisualGraph.Data.Additional.SVGHelper.Line(-CoordinateAxisMarkerHeight,ystep, CoordinateAxisMarkerHeight,ystep, CoordinateSystemColor);
                markup += VisualGraph.Data.Additional.SVGHelper.Text(-CoordinateAxisFontSize * 2, ystep + 0.5,ystep.ToString(CultureInfo.InvariantCulture), CoordinateAxisFontSize, CoordinateSystemColor);
            }
            MarkupString markupString = new MarkupString(markup);
            return markupString; 
        }
        public static int AddToRenderTree(RenderTreeBuilder builder, int sequence, double minX, double minY, double maxX, double maxY)
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
            var text = ((int)(minX / 10)) * 10;
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

        public static int AddToRenderTree(Microsoft.AspNetCore.Blazor.RenderTree.RenderTreeBuilder builder, int sequence, double minX, double minY, double maxX, double maxY)
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
            var text = ((int)(minX / 10)) * 10;
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
