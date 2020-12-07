#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    public class Dynamic : ComponentBase
    {
        #region Methods

        protected override void BuildRenderTree( RenderTreeBuilder builder )
        {
            base.BuildRenderTree( builder );
            builder?.OpenElement( 0, TagName );

            builder.AddMultipleAttributes( 1, Attributes );
            builder.AddEventPreventDefaultAttribute( 2, "onclick", ClickPreventDefault );
            builder.AddEventStopPropagationAttribute( 3, "onclick", ClickStopPropagation );
            builder.AddContent( 4, ChildContent );
            builder.AddElementReferenceCapture( 2, capturedRef =>
            {
                ElementRef = capturedRef;
                ElementRefChanged?.Invoke( ElementRef ); // Invoke the callback for the binding to work.
            } );

            builder.CloseElement();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the element to render.
        /// </summary>
        [Parameter] public string TagName { get; set; }

        /// <summary>
        /// Gets or sets the element reference.
        /// </summary>
        [Parameter] public ElementReference ElementRef { get; set; }

        /// <summary>
        /// Notifies us that the element reference has changed.
        /// </summary>
        [Parameter] public Action<ElementReference> ElementRefChanged { get; set; }

        /// <summary>
        /// Set to true if click event need to be prevented.
        /// </summary>
        [Parameter] public bool ClickPreventDefault { get; set; }

        /// <summary>
        /// Set to true if click event need to be prevented from propagation.
        /// </summary>
        [Parameter] public bool ClickStopPropagation { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Dynamic"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Captures all the custom attribute that are not part of Blazorise component.
        /// </summary>
        [Parameter( CaptureUnmatchedValues = true )]
        public IDictionary<string, object> Attributes { get; set; }

        #endregion
    }
}
