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
        public static void ActivateNode(object sender, GraphMouseEventArgs<Node> args)
        {
            if (((BasicGraph)sender).GraphModel.ActiveNode != null)
            {
                if (((BasicGraph)sender).GraphModel.ActiveNode.Id == args.Target.Id)
                {
                    args.Target.IsActive = false;
                }
                else
                {
                    ((BasicGraph)sender).GraphModel.ActiveNode.IsActive = false;
                    args.Target.IsActive = true;
                }
                ((BasicGraph)sender).NodeDragStarted = true;

                ((BasicGraph)sender).DisablePan();
            }
        }
        public static void DeactivateNode(object sender, MouseEventArgs args)
        {
            Console.WriteLine("clicked");
            if (((BasicGraph)sender).GraphModel.ActiveNode != null)
            {
                Console.WriteLine("is one active");
                ((BasicGraph)sender).GraphModel.ActiveNode.IsActive = false;
                ((BasicGraph)sender).NodeDragStarted = false;
                ((BasicGraph)sender).EnablePan();
                Console.WriteLine("worked");
            }
        }

        public static void ActivateNode(object sender, GraphTouchEventArgs<Node> args)
        {
            Node node = args.Target;
            if (((BasicGraph)sender).GraphModel.ActiveNode != null)
            {
                if (((BasicGraph)sender).GraphModel.ActiveNode.Id == node.Id)
                {
                    node.IsActive = false;
                }
                else
                {
                    ((BasicGraph)sender).GraphModel.ActiveNode.IsActive = false;
                    node.IsActive = true;
                }
            }
            else
            {
                node.IsActive = true;
            }
            ((BasicGraph)sender).NodeDragStarted = true;
            ((BasicGraph)sender).DisablePan();
        } 
    }
       
}
