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
            deactivateDragNode((BasicGraph)sender, null);
        }
        public static void DeactivateDragNode(object sender, GraphMouseEventArgs<Node> args)
        {
            deactivateDragNode((BasicGraph)sender,args.Target);
        }
        public static void DeactivateDragNode(object sender, GraphTouchEventArgs<Node> args)
        {
            deactivateDragNode((BasicGraph)sender, args.Target);
        }
        private static void deactivateDragNode(BasicGraph sender, Node target)
        {
            sender.NodeDragStarted = false;
            sender.EnablePan();   
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
            if(sender.graphService.CurrentGraphModel.ActiveEdge != null)
                sender.graphService.CurrentGraphModel.ActiveEdge.IsActive = false;
            var activeNode = sender.graphService.CurrentGraphModel.ActiveNode;
            if (activeNode != null)
            {
                if (activeNode.Id == node.Id)
                {
                    node.IsActive = false;
                }
                else
                {
                    activeNode.IsActive = false;
                    node.IsActive = true;
                }
            }
            else
            {
                node.IsActive = true;
            }
        }
        public static void ToggleActiveStateEdge(object sender, GraphMouseEventArgs<Edge> args)
        {
            toggleActiveStateEdge((BasicGraph)sender, args.Target);
        }
        public static void ToggleActiveStateEdge(object sender, GraphTouchEventArgs<Edge> args)
        {
            toggleActiveStateEdge((BasicGraph)sender, args.Target);
        }
        private static void toggleActiveStateEdge(BasicGraph sender, Edge edge)
        {
            if(sender.graphService.CurrentGraphModel.ActiveNode != null)
                sender.graphService.CurrentGraphModel.ActiveNode.IsActive = false;
            var activeEdge = sender.graphService.CurrentGraphModel.ActiveEdge;
            if (activeEdge != null)
            {
                if (activeEdge.Id == edge.Id)
                {
                    edge.IsActive = false;
                }
                else
                {
                    activeEdge.IsActive = false;
                    edge.IsActive = true;
                }
            }
            else
            {
                edge.IsActive = true;
            }
        }
        public static void MoveNodeOrEdge(object sender, MouseEventArgs args)
        {
            var _sender = (BasicGraph)sender;
            if (_sender.NodeDragStarted && _sender.graphService.CurrentGraphModel.ActiveNode != null)
            {
                moveNode((BasicGraph)sender, args.ClientX, args.ClientY);
            }
        }
        public static void MoveNodeOrEdge(object sender, TouchEventArgs args)
        {
            var _sender = (BasicGraph)sender;
            if (_sender.NodeDragStarted)
            {
                if(_sender.graphService.CurrentGraphModel.ActiveNode != null)
                    moveNode((BasicGraph)sender, args.Touches.First().ClientX, args.Touches.First().ClientY);
                if (_sender.graphService.CurrentGraphModel.ActiveEdge != null)
                    moveEdge((BasicGraph)sender, args.Touches.First().ClientX, args.Touches.First().ClientY);
            }
        }
        private static async void moveNode(BasicGraph sender,double x, double y) 
        {
            
                try
                {
                    Point2 coords = await sender.RequestTransformedEventPosition(x, y);
                    setNodePosition(((BasicGraph)sender).graphService.CurrentGraphModel.ActiveNode, coords);
                }
                catch { }
            
        }
        private static async void moveEdge(BasicGraph sender, double x, double y)
        {

            try
            {
                Point2 coords = await sender.RequestTransformedEventPosition(x, y);
                
            }
            catch { }

        }
        private static void setNodePosition(Node node, Point2 coords)
        {             
            node.Pos.X = Convert.ToSingle(coords.X);
            node.Pos.Y = Convert.ToSingle(coords.Y);
        }
    }
       
}
