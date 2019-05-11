#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
#endregion

namespace Blazorise
{
    public class Dynamic : IComponent
    {
        #region Members

        private RenderHandle renderHandle;

        private RenderFragment childContent;

        private string tagName;

        private IDictionary<string, object> attributes;

        #endregion

        #region Methods

        public void Configure( RenderHandle renderHandle )
        {
            this.renderHandle = renderHandle;
        }

        public Task SetParametersAsync( ParameterCollection parameters )
        {
            attributes = parameters.ToDictionary() as Dictionary<string, object>;

            childContent = attributes.GetAndRemove<RenderFragment>( RenderTreeBuilder.ChildContent );
            tagName = attributes.GetAndRemove<string>( "TagName" );

            //foreach ( var att in attributes )
            //{
            //    if ( attributesToRender == null )
            //        attributesToRender = new Dictionary<string, object>();

            //    if ( att.Value == null )
            //        continue;

            //    if ( attributes.TryGetValue( att.Key, out var existingValue ) && existingValue != null )
            //    //{
            //    //    // concat the same attributes
            //    //    attributesToRender[att.Key] = existingValue.ToString() + " " + att.Value.ToString();
            //    //}
            //    //else
            //    {
            //        attributesToRender[att.Key] = att.Value;
            //    }

            //    //Console.WriteLine( $"{kv.Key}: {kv.Value}" );
            //}

            renderHandle.Render( Render );

            return Task.CompletedTask;
        }

        private void Render( RenderTreeBuilder builder )
        {
            builder.OpenElement( 0, tagName );

            // Pass through all other attributes unchanged
            foreach ( var attribute in attributes )
                builder.AddAttribute( 0, attribute.Key, attribute.Value );

            //builder.AddElementReferenceCapture( 2, capturedRef => { ElementRef = capturedRef; } );

            // Pass through any child content unchanged
            builder.AddContent( 2, childContent );

            builder.CloseElement();
        }

        #endregion

        #region Properties

        ///// <summary>
        ///// Gets or sets the name of the element to render.
        ///// </summary>
        //public string TagName { get; set; }

        ///// <summary>
        ///// Gets the <see cref="Microsoft.AspNetCore.Blazor.ElementRef"/>.
        ///// </summary>
        //public ElementRef ElementRef { get; private set; }

        #endregion
    }
}
