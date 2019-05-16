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
    // based on https://github.com/chanan/BlazorStrap/blob/master/src/BlazorStrap/DynamicElement.cs
    public class Dynamic : ComponentBase
    {
        #region Members

        private IDictionary<string, object> parametersToRender;

        private RenderFragment childContent;

        #endregion

        #region Methods

        public override Task SetParametersAsync( ParameterCollection parameters )
        {
            parametersToRender = parameters.ToDictionary() as Dictionary<string, object>;

            childContent = parametersToRender.GetAndRemove<RenderFragment>( RenderTreeBuilder.ChildContent );
            TagName = parametersToRender.GetAndRemove<string>( nameof( TagName ) ) ?? throw new InvalidOperationException( $"No value was supplied for required parameter '{nameof( TagName )}'." );

            // Combine any explicitly-supplied attributes with the remaining parameters
            var attributesParam = parametersToRender.GetAndRemove<IReadOnlyDictionary<string, object>>( nameof( Attributes ) );

            if ( attributesParam != null )
            {
                foreach ( var kvp in attributesParam )
                {
                    if ( kvp.Value != null && parametersToRender.TryGetValue( kvp.Key, out var existingValue ) && existingValue != null )
                    {
                        parametersToRender[kvp.Key] = $"{existingValue} {kvp.Value}";
                    }
                    else
                    {
                        parametersToRender[kvp.Key] = kvp.Value;
                    }
                }
            }

            return base.SetParametersAsync( ParameterCollection.Empty );
        }

        protected override void BuildRenderTree( RenderTreeBuilder builder )
        {
            base.BuildRenderTree( builder );

            builder.OpenElement( 0, TagName );

            foreach ( var param in parametersToRender )
            {
                switch ( param.Value )
                {
                    case EventCallback<UIChangeEventArgs> ec:
                        builder.AddAttribute( 1, param.Key, ec );
                        break;
                    case EventCallback<UIClipboardEventArgs> ec:
                        builder.AddAttribute( 1, param.Key, ec );
                        break;
                    case EventCallback<UIDataTransferItem> ec:
                        builder.AddAttribute( 1, param.Key, ec );
                        break;
                    case EventCallback<UIErrorEventArgs> ec:
                        builder.AddAttribute( 1, param.Key, ec );
                        break;
                    case EventCallback<UIEventArgs> ec:
                        builder.AddAttribute( 1, param.Key, ec );
                        break;
                    case EventCallback<UIFocusEventArgs> ec:
                        builder.AddAttribute( 1, param.Key, ec );
                        break;
                    case EventCallback<UIKeyboardEventArgs> ec:
                        builder.AddAttribute( 1, param.Key, ec );
                        break;
                    case EventCallback<UIMouseEventArgs> ec:
                        builder.AddAttribute( 1, param.Key, ec );
                        break;
                    default:
                        builder.AddAttribute( 1, param.Key, param.Value );
                        break;
                }
            }

            builder.AddElementReferenceCapture( 2, capturedRef => { ElementRef = capturedRef; } );
            builder.AddContent( 3, childContent );
            builder.CloseElement();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the element to render.
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// Gets the <see cref="Microsoft.AspNetCore.Components.ElementRef"/>.
        /// </summary>
        public ElementRef ElementRef { get; private set; }

        /// <summary>
        /// Gets or sets the attributes to render.
        /// </summary>
        public IReadOnlyDictionary<string, object> Attributes
        {
            // The property is only declared for intellisense. It's not used at runtime.
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        #endregion
    }
}
