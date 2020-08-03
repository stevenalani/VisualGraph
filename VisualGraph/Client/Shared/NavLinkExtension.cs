using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;

namespace VisualGraph.Client.Shared
{
    /// <summary>
    /// Erweiterung für Navigationslinks in Menüs für Untermenüs
    /// </summary>
    public class NavLinkExtension : NavLink
    {
        private static readonly string _activeClass = "active";
        /// <summary>
        /// Erstellt Instanz der Klasse
        /// </summary>
        public NavLinkExtension()
        {
            ActiveClass = _activeClass;
        }
        /// <summary>
        /// Das Symbol für den Menüeintrag
        /// </summary>
        [Parameter]
        public string Icon { get; set; }
        /// <summary>
        /// Der Text für den Menüeintrag
        /// </summary>
        [Parameter]
        public string Text { get; set; }
        /// <summary>
        /// Ist dies ein Submenüeintrag
        /// </summary>
        [Parameter]
        public bool IsSubmenu { get; set; }
        /// <summary>
        /// HTML LI- Element Klasse
        /// </summary>
        [Parameter]
        public string ListItemClass { get; set; }

        private bool isActive
        {
            get
            {
                if (CssClass != null)
                    return CssClass.Contains(_activeClass);
                else
                    return false;
            }
        }
        /// <summary>
        /// Setzt die default Aktiv- Klasse
        /// </summary>
        protected override void OnInitialized()
        {
            ActiveClass = _activeClass;
            base.OnInitialized();
        }
        /// <summary>
        /// Erstellt das zu rendernde Element
        /// </summary>
        /// <param name="builder"></param>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {

            int sequenz = 0;
            builder.OpenElement(sequenz++, "li");
            builder.AddAttribute(sequenz++, "class", $"{ListItemClass} {(isActive ? _activeClass : "")}");
            builder.OpenElement(sequenz++, "a");

            builder.AddMultipleAttributes(sequenz++, AdditionalAttributes);
            builder.AddAttribute(sequenz++, "class", CssClass);

            if (Text != null && Icon != null)
            {
                builder.OpenElement(sequenz++, "span");
                builder.OpenElement(sequenz++, "span");
                builder.AddAttribute(sequenz++, "class", Icon);
                builder.AddAttribute(sequenz++, "aria-hidden", true);
                builder.CloseElement();
                builder.AddContent(sequenz++, Text);
                builder.CloseElement();
                
            }

            if (IsSubmenu)
            {
                builder.CloseElement();
                builder.OpenElement(sequenz++, "ul");
                builder.AddAttribute(sequenz++, "class", "submenu");
                builder.AddContent(sequenz++, ChildContent);
                builder.CloseElement();
            }
            else
            {
                builder.AddContent(sequenz++, ChildContent);
                builder.CloseElement();
            }

            builder.CloseElement();
        }
    }
}
