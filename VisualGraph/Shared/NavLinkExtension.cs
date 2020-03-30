using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VisualGraph.Shared
{
    public class NavLinkExtension : NavLink
    {
        private static readonly string _activeClass = "active";
        public NavLinkExtension()
        {
            ActiveClass = _activeClass;
        }
        [Parameter]
        public string Icon { get; set; }
        [Parameter]
        public string Text { get; set; }
        [Parameter]
        public bool IsSubmenu { get; set; }
        [Parameter]
        public string ListItemClass { get; set; }
        //[Parameter(CaptureUnmatchedValues = true)]
        // IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

        private bool isActive {
            get {
                if (CssClass != null)
                    return CssClass.Contains(_activeClass);
                else
                    return false;
            }
        }

        protected override void OnInitialized()
        {
            ActiveClass = _activeClass;
            base.OnInitialized();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {

            int sequenz = 0;
            builder.OpenElement(sequenz++, "li");
            builder.AddAttribute(sequenz++, "class", $"{ListItemClass} {(isActive?_activeClass:"")}");
            builder.OpenElement(sequenz++, "a");

            builder.AddMultipleAttributes(sequenz++, AdditionalAttributes);
            builder.AddAttribute(sequenz++, "class", CssClass);
            
            if(Text != null && Icon != null)
            {
                builder.OpenElement(sequenz++, "span");
                builder.OpenElement(sequenz++, "span");
                builder.AddAttribute(sequenz++, "class", Icon);
                builder.AddAttribute(sequenz++, "aria-hidden", true);
                builder.CloseElement();
                builder.AddContent(sequenz++, Text);
                builder.CloseElement();
                    //< span >< span class="oi oi-graph" aria-hidden="true"></span></span>
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
