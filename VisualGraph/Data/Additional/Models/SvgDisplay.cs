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
        public static double ZoomStep { get; set; }
        public static double MaxZoomLevel { get; set; } 
        private double Width { get; set; }
        private double Height { get; set; }
        public double ZoomedWidth => Width / ZoomLevel;
        public double ZoomedHeight => Height / ZoomLevel;
        public Vector2 Center { get; set; } = Vector2.Zero;
        private double ZoomLevel { get; set; } = 1.0;
        public double TextSize { get; set; } = 10;
        public bool HideInformation { get; set; } = false;
        private Vector2 UpperLeft => new Vector2((float)(Center.X - (ZoomedWidth/ 2)) , (float)(Center.Y - (ZoomedHeight/ 2) ));

        public void SetHeight(double height)
        {
            Height = height;
        }
        public void SetWidth(double width)
        {
            Width = width;
        }
        public void UpdateDisplaySettings()
        {

        }
        public string ViewBox => $"{(UpperLeft.X).ToString(CultureInfo.InvariantCulture)}," +
                $" {(UpperLeft.Y).ToString(CultureInfo.InvariantCulture)}," +
                $" {(ZoomedWidth).ToString(CultureInfo.InvariantCulture) }," +
                $" {(ZoomedHeight).ToString(CultureInfo.InvariantCulture) }";
       public MarkupString GetDisplayInformationMarkupString(GraphStyleParameters graphStyleParameters)
        {
            var mstring = HideInformation ? "": $"<div id=\"ViewInformation\"><p>"+
                    $"<span>{String.Format("Window Width: {0,0:0.00}, Height {1,0:0.00}", ZoomedWidth,ZoomedHeight)}</span></br>"+
                    $"<span>{String.Format("Zoom: {0:0.00}", ZoomLevel)} Zoom step: {ZoomStep}</span></br>" +
                    $"<span>Center: {{{Center.X.ToString(CultureInfo.InvariantCulture)};{Center.X.ToString(CultureInfo.InvariantCulture)}}}</span></p></div>";
            return  new MarkupString(mstring);
        }
    }
}
