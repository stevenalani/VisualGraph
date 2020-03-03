using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace VisualGraph.Data.Additional.Models
{
    public class SvgDisplay
    {
        public static BrowserSizes BrowserSizes { get; set; }
        public static double ZoomStep { get; set; }
        public static double MaxZoomLevel { get; set; }
        private double GraphWidth { get; set; }
        private double GraphHeight { get; set; }

        public float Ratio => (float)(GraphWidth / GraphHeight);
        public float ParentContainerRatio => (float)(ParentWidth / ParentHeight);
        /*public string RatioLimit { 
            get { 
                string result = string.Empty;
                if(ParentWidth != 0 && 0 != ParentHeight)
                if(ParentContainerRatio < 1)
                {
                    if(Ratio < 1)
                    {
                        result = $"max-width:{ParentWidth}px;";
                    }
                    else
                    {
                        result = $"max-height:{ParentHeight}px;";
                    }
                }
                else
                {
                    if (Ratio < 1)
                    {
                        result = $"max-height:{ParentHeight}px;";
                    }
                    else
                    {
                        result = $"max-width:{ParentWidth}px;";
                    }
                }
                return result;
            } 
        }*/

        public Vector2 Center = new Vector2(0);
        private double ZoomLevel { get; set; } = 1.0;
        public double TextSize { get; set; } = 10;
        public bool HideInformation { get; set; } = false;
        private Vector2 UpperLeft = Vector2.Zero;
        private double ParentWidth { get; set; }
        private double ParentHeight { get; set; }

        public SvgDisplay(double minx, double miny, double maxx, double maxy)
        {
            UpperLeft = new Vector2((float)minx, (float)miny);
            GraphWidth = Math.Abs(minx) + Math.Abs(maxx);
            GraphHeight = Math.Abs(miny) + Math.Abs(maxy);
            Center = new Vector2((float)(maxx - minx) / 2, (float)(maxy - miny) / 2);
        }
        public SvgDisplay()
        {
            Center = new Vector2(0,0);
        }
        public void SetHeight(double height)
        {
            GraphHeight = height;
        }
        public void SetWidth(double width)
        {
            GraphWidth = width;
        }
        public bool UpdateDisplaySettings(SvgPanZoomInformation svgPanZoomInformation)
        {
            bool hasChanges = false;
            if (svgPanZoomInformation == null) return hasChanges;
            if(svgPanZoomInformation.PanZoomHeight != GraphHeight)
            {
                GraphHeight = svgPanZoomInformation.PanZoomHeight;
                hasChanges = true;
            }
                
            if(GraphWidth != svgPanZoomInformation.PanZoomWidth)
            {
                GraphWidth = svgPanZoomInformation.PanZoomWidth;
                hasChanges = true;
            }
                
            if(Center.X != (float)svgPanZoomInformation.CenterX)
            {
                Center.X = (float)svgPanZoomInformation.CenterX;
                hasChanges = true;
            }
                
            if(Center.Y != (float)svgPanZoomInformation.CenterY)
            {
                Center.Y = (float)svgPanZoomInformation.CenterY;
                hasChanges = true;
            }
                
            if(ZoomLevel != svgPanZoomInformation.Zoom)
            {
                ZoomLevel = svgPanZoomInformation.Zoom;
                hasChanges = true;
            }

            return hasChanges;
        }
        public bool UpdateDisplaySettings(SvgContainerInformation svgContainerInformation)
        {
            bool hasChanges = false;
            if (svgContainerInformation == null) return hasChanges;
            if (ParentWidth != svgContainerInformation.ParentWidth)
            {
                ParentWidth = svgContainerInformation.ParentWidth;
                hasChanges = true;
            }
            if (ParentHeight != svgContainerInformation.ParentHeight)
            {
                ParentHeight = svgContainerInformation.ParentHeight;
                hasChanges = true;
            }
            return hasChanges;
        }
        public bool UpdateDisplaySettings(Vector2[] ConvexHull)
        {
            GraphWidth = Math.Abs(ConvexHull[0].X) + Math.Abs(ConvexHull[1].X);
            GraphHeight = Math.Abs(ConvexHull[0].Y) + Math.Abs(ConvexHull[1].Y);
            return true;
        }
        public string ViewBox => 
            $"{(UpperLeft.X).ToString(CultureInfo.InvariantCulture)}," +
            $"{(UpperLeft.Y).ToString(CultureInfo.InvariantCulture)}," +
            $"{(GraphWidth).ToString(CultureInfo.InvariantCulture) }," +
            $"{(GraphHeight).ToString(CultureInfo.InvariantCulture) }";
       public MarkupString GetDisplayInformationMarkupString()
        {
            var mstring = HideInformation ? "": $"<div id=\"ViewInformation\"><p>"+
                    $"<span>{String.Format("Window Width: {0,0:0.00}, Height {1,0:0.00}", GraphWidth,GraphHeight)}</span></br>"+
                    $"<span>{String.Format("Zoom: {0:0.00}", ZoomLevel)} Zoom step: {ZoomStep}</span></br>" +
                    $"<span>Center: {{{Center.X.ToString(CultureInfo.InvariantCulture)};{Center.Y.ToString(CultureInfo.InvariantCulture)}}}</span></p></div>";
            return  new MarkupString(mstring);
        }
    }
}
