using Microsoft.AspNetCore.Components.Web;

namespace VisualGraph.Client.Shared.EventHandling
{
    /// <summary>
    /// Kapselt das Event- Ziel und TouchEvent
    /// </summary>
    /// <typeparam name="T">Typ des Ziels z.B. Node, Edge</typeparam>
    public class GraphTouchEventArgs<T>
    {
        /// <summary>
        /// Ziel des Events. 
        /// </summary>
        public T Target { get; set; }
        /// <summary>
        /// Touch Event objekt
        /// </summary>
        public TouchEventArgs TouchEventArgs { get; set; }
        /// <summary>
        /// Erstellt ein Objekt der Klasse.
        /// </summary>
        /// <param name="target">Ziel des Events. Node oder Edge</param>
        /// <param name="e">Event Argumente</param>
        public GraphTouchEventArgs(T target, TouchEventArgs e)
        {
            Target = target;
            TouchEventArgs = e;

        }
    }
}