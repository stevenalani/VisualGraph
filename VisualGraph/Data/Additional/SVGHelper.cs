using Microsoft.AspNetCore.Blazor;
using System;
using System.Globalization;
using System.Numerics;

namespace VisualGraph.Data.Additional
{
    internal static class SVGHelper
    {
        internal static string Line(double x1, double y1, double x2, double y2, string stroke = "black", double strokewidth = 0.1)
        {
            string markup = "<line ";
            markup += $"x1=\"{x1.ToString(CultureInfo.InvariantCulture)}\" ";
            markup += $"y1=\"{y1.ToString(CultureInfo.InvariantCulture)}\" ";
            markup += $"x2=\"{x2.ToString(CultureInfo.InvariantCulture)}\" ";
            markup += $"y2=\"{y2.ToString(CultureInfo.InvariantCulture)}\" ";
            markup += $"style=\"{$"stroke:{stroke}; stroke-width:{strokewidth.ToString(CultureInfo.InvariantCulture)};"}\" ";
            markup += $"></line>";
            return markup;
        }
        internal static string RotatedTriangle(double x1, double y1, double x2, double y2, string stroke = "black", string strokewidth = "0.1")
        {
            Vector2 p0 = new Vector2((float)x1, (float)y1);
            Vector2 p1 = new Vector2((float)x2, (float)y2);
            var angle = Math.Acos(Vector2.Dot(p0, p1) / (p0.Length() * p1.Length()));
            Vector2 leftPoint = new Vector2(-1f, -2f);
            Vector2 rightPoint = new Vector2(1f, -2f);
            Vector2 peak = new Vector2(0f, 1f);
            var rotationMatrix = Matrix3x2.CreateRotation((float)angle);
            leftPoint = p1 + Vector2.Transform(leftPoint, rotationMatrix);
            rightPoint = p1 + Vector2.Transform(rightPoint, rotationMatrix);

            return $"<path d=\"M{rightPoint.X.ToString(CultureInfo.InvariantCulture)} {rightPoint.Y.ToString(CultureInfo.InvariantCulture)} L{p1.X.ToString(CultureInfo.InvariantCulture)} {p1.Y.ToString(CultureInfo.InvariantCulture)} L{leftPoint.X.ToString(CultureInfo.InvariantCulture)} {leftPoint.Y.ToString(CultureInfo.InvariantCulture)}\" stroke=\"{stroke}\" stroke-width=\"{strokewidth}\" />";

        }
        internal static string Text(double x, double y, string text, double fontsize = 1.0, string color = "black")
        {
            string markup = "";
            markup += $"<text ";
            markup += $"x=\"{x.ToString(CultureInfo.InvariantCulture)}\" ";
            markup += $"y=\"{y.ToString(CultureInfo.InvariantCulture)}\" ";
            markup += $"fill=\"{color}\"";
            markup += $"font-size=\"{fontsize.ToString(CultureInfo.InvariantCulture)}\" ";
            markup += $"class=\"axis-label\">{text}</text>";
            return markup;
        }
    }
}