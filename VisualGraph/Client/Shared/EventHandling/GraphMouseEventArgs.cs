using Microsoft.AspNetCore.Components.Web;

namespace VisualGraph.Client.Shared.EventHandling
{
    /// <summary>
    /// Kapselt das Event- Ziel und MausEvents
    /// </summary>
    /// <typeparam name="T">Typ des Ziels z.B. Node, Edge</typeparam>
    public class GraphMouseEventArgs<T>
    {
        /// <summary>
        /// Ziel des Events. 
        /// </summary>
        public T Target { get; set; }
        /// <summary>
        /// Maus Event objekt
        /// </summary>
        public MouseEventArgs MouseEventArgs { get; set; }
        /// <summary>
        /// Erstellt ein Objekt der Klasse.
        /// </summary>
        /// <param name="target">Ziel des Events. Node oder Edge</param>
        /// <param name="e">Event Argumente</param>
        public GraphMouseEventArgs(T target, MouseEventArgs e)
        {
            Target = target;
            MouseEventArgs = e;

        }
    }
}
