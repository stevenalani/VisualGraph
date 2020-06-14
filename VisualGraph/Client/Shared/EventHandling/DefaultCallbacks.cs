using Microsoft.AspNetCore.Components.Web;
using System;
using System.Linq;
using System.Numerics;
using VisualGraph.Client.Components;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.Shared.EventHandling
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
            sender.SvgMouseMove += MoveNodeOrEdge;
            if (sender.graphService.CurrentGraphModel.ActiveNode != null && target.IsActive)
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
            deactivateDragNode((BasicGraph)sender, args.Target);
        }
        public static void DeactivateDragNode(object sender, GraphTouchEventArgs<Node> args)
        {
            deactivateDragNode((BasicGraph)sender, args.Target);
        }
        private static void deactivateDragNode(BasicGraph sender, Node target)
        {
            sender.SvgMouseMove -= MoveNodeOrEdge;
            sender.NodeDragStarted = false;
            sender.EnablePan();
        }
        public static void ToggleActiveStateNode(object sender, GraphMouseEventArgs<Node> args)
        {
            toggleActiveStateNode((BasicGraph)sender, args.Target);
        }
        public static void ToggleActiveStateNode(object sender, GraphTouchEventArgs<Node> args)
        {
            toggleActiveStateNode((BasicGraph)sender, args.Target);
        }
        private static void toggleActiveStateNode(BasicGraph sender, Node node)
        {
            if (sender.graphService.CurrentGraphModel.ActiveEdge != null)
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
        public static void ActivateDragEdge(object sender, GraphMouseEventArgs<Edge> args)
        {
            activateDragEdge((BasicGraph)sender, args.Target);
        }
        public static void ActivateDragEdge(object sender, GraphTouchEventArgs<Edge> args)
        {
            activateDragEdge((BasicGraph)sender, args.Target);
        }
        private static void activateDragEdge(BasicGraph sender, Edge target)
        {
            if (sender.graphService.CurrentGraphModel.ActiveEdge != null && target.IsActive)
            {
                sender.EdgeDragStarted = true;
                sender.DisablePan();
            }
        }
        public static void DeactivateDragEdge(object sender, MouseEventArgs args)
        {
            deactivateDragEdge((BasicGraph)sender, null);
        }
        public static void DeactivateDragEdge(object sender, GraphMouseEventArgs<Edge> args)
        {
            deactivateDragEdge((BasicGraph)sender, args.Target);
        }
        public static void DeactivateDragEdge(object sender, GraphTouchEventArgs<Edge> args)
        {
            deactivateDragEdge((BasicGraph)sender, args.Target);
        }
        private static void deactivateDragEdge(BasicGraph sender, Edge target)
        {
            sender.EdgeDragStarted = false;
            sender.EnablePan();
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
            if (sender.graphService.CurrentGraphModel.ActiveNode != null)
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
            if (_sender.NodeDragStarted || _sender.EdgeDragStarted)
            {
                if (_sender.graphService.CurrentGraphModel.ActiveNode != null)
                    moveNode((BasicGraph)sender, args.ClientX, args.ClientY);
                if (_sender.graphService.CurrentGraphModel.ActiveEdge != null)
                    moveEdge((BasicGraph)sender, args.ClientX, args.ClientY);
            }
        }
        public static void MoveNodeOrEdge(object sender, TouchEventArgs args)
        {
            var _sender = (BasicGraph)sender;
            if (_sender.NodeDragStarted)
            {
                if (_sender.graphService.CurrentGraphModel.ActiveNode != null)
                    moveNode((BasicGraph)sender, args.Touches.First().ClientX, args.Touches.First().ClientY);
                if (_sender.graphService.CurrentGraphModel.ActiveEdge != null)
                    moveEdge((BasicGraph)sender, args.Touches.First().ClientX, args.Touches.First().ClientY);
            }
        }
        private static async void moveNode(BasicGraph sender, double x, double y)
        {

            try
            {
                Point2 coords = await sender.RequestTransformedEventPosition(x, y);
                setNodePosition(sender.graphService.CurrentGraphModel.ActiveNode, coords);
                await sender.graphService.Rerender<GraphEditForm>();
            }
            catch { }

        }
        private static void setNodePosition(Node node, Point2 coords)
        {
            node.Pos.X = Convert.ToSingle(coords.X);
            node.Pos.Y = Convert.ToSingle(coords.Y);
        }
        private static async void moveEdge(BasicGraph sender, double x, double y)
        {

            try
            {
                Point2 coords = await sender.RequestTransformedEventPosition(x, y);
                Edge edge = sender.graphService.CurrentGraphModel.ActiveEdge;
                var diff = edge.Edgemiddle - new Vector2((float)coords.X, (float)coords.Y);
                var dir = Vector2.Normalize(diff);
                var dirdiff = dir - edge.Direction;
                var dot = Vector2.Dot(edge.Direction, dir);
                if (dot > 0)
                {
                    if (edge.curveScale < edge.curveScaleUpperBound)
                    {
                        edge.curveScale += 0.1f;
                    }
                }
                else if (edge.curveScale > edge.curveScaleLowerBound)
                {
                    edge.curveScale -= 0.1f;
                }
            }
            catch { }

        }

    }

}
