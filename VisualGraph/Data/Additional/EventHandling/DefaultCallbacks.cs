using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualGraph.Components;
using VisualGraph.Data.Additional.EventHandling;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data.Additional.EventHandling
{
    public static class DefaultCallbacks
    {
        public static void ActivateDragNode(object sender, GraphMouseEventArgs<Node> args)
        {
            activateDragNode((BasicGraph)sender, args.Target);
        }
        public static void ActivateDragNode(object sender, GraphTouchEventArgs<Node> args)
        {
            activateDragNode((BasicGraph)sender, args.Target);
        }
        private static void activateDragNode(BasicGraph sender, Node target)
        {
            if (((BasicGraph)sender).graphService.CurrentGraphModel.ActiveNode != null && target.IsActive)
            {
                sender.NodeDragStarted = true;
                sender.DisablePan();
            }
        }
        public static void DeactivateDragNode(object sender, MouseEventArgs args)
        {
            Console.WriteLine("Callback Event");
            deactivateDragNode((BasicGraph)sender, null);
        }
        public static void DeactivateDragNode(object sender, GraphMouseEventArgs<Node> args)
        {
            Console.WriteLine("Callback Event");
            deactivateDragNode((BasicGraph)sender,args.Target);
        }
        public static void DeactivateDragNode(object sender, GraphTouchEventArgs<Node> args)
        {
            Console.WriteLine("Callback Event");
            deactivateDragNode((BasicGraph)sender, args.Target);
        }
        private static void deactivateDragNode(BasicGraph sender, Node target)
        {
            Console.WriteLine("Callback Event Method");
            sender.NodeDragStarted = false;
            sender.EnablePan();   
        }
        public static void DeactivateDragNode(object sender, TouchEventArgs args)
        {
            if (((BasicGraph)sender).graphService.CurrentGraphModel.ActiveNode != null)
            {
                ((BasicGraph)sender).NodeDragStarted = false;
                ((BasicGraph)sender).EnablePan();
            }
        }

        public static void ToggleActiveStateNode(object sender, GraphMouseEventArgs<Node>args)
        {

            toggleActiveStateNode((BasicGraph)sender, args.Target);
        }
        public static void ToggleActiveStateNode(object sender, GraphTouchEventArgs<Node> args)
        {
            toggleActiveStateNode((BasicGraph)sender, args.Target);
        }
        private static void toggleActiveStateNode(BasicGraph sender,Node node)
        {                
            var activeNode = sender.graphService.CurrentGraphModel.ActiveNode;
            Console.WriteLine($"Active Node: {activeNode?.Name}" );
            if (activeNode != null)
            {
                if (activeNode.Id == node.Id)
                {
                    Console.WriteLine($"deselected");
                    node.IsActive = false;
                }
                else
                {
                    Console.WriteLine($"switched");
                    activeNode.IsActive = false;
                    node.IsActive = true;
                }
            }
            else
            {
                Console.WriteLine("simple pick");
                node.IsActive = true;
            }
        }
        public static void MoveNode(object sender, MouseEventArgs args)
        {
            moveNode((BasicGraph)sender, args.ClientX, args.ClientY);
        }
        public static void MoveNode(object sender, TouchEventArgs args)
        {
            moveNode((BasicGraph)sender, args.Touches.First().ClientX, args.Touches.First().ClientY);
        }
        private static async void moveNode(BasicGraph sender,double x, double y) 
        {
            if (sender.NodeDragStarted && sender.graphService.CurrentGraphModel.ActiveNode != null)
            {
                try
                {
                    Point2 coords = await sender.RequestTransformedEventPosition(x, y);
                    setNodePosition(((BasicGraph)sender).graphService.CurrentGraphModel.ActiveNode, coords);
                }
                catch { }
            }
        }
        private static void setNodePosition(Node node, Point2 coords)
        {             
            node.Pos.X = Convert.ToSingle(coords.X);
            node.Pos.Y = Convert.ToSingle(coords.Y);
        }
    }
       
}
