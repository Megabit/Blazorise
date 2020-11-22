#region Using directives
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Bootstrap
{
    public class Button : Blazorise.Button
    {
        #region Methods

        protected override void BuildRenderTree( RenderTreeBuilder builder )
        {
            builder
                .OpenElement( Type.ToButtonTagName() )
                .Id( ElementId )
                .Type( Type.ToButtonTypeString() )
                .Class( ClassNames )
                .Style( StyleNames )
                .Disabled( Disabled )
                .AriaPressed( Active );

            if ( Type == ButtonType.Link && To != null )
            {
                builder
                    .Role( "button" )
                    .Href( To )
                    .Target( Target );
            }
            else
            {
                builder.OnClick( this, Clicked );
            }

            builder.Attributes( Attributes );
            builder.ElementReferenceCapture( capturedRef => ElementRef = capturedRef );

            if ( LoadingContent == null)
            {
                if ( Loading )
                {
                    CreateDefaultLoadingAnimation( builder );
                }   
                
                builder.Content( ChildContent );
            }
            else
            {
                builder.Content( Loading ? LoadingContent : ChildContent);
            }

            builder.CloseElement();
        }
        
        private void CreateDefaultLoadingAnimation( RenderTreeBuilder builder )
        {
            builder.OpenElement( "span" );
            builder
                .Class( "spinner-border spinner-border-sm" )
                .Role( "status" )
                .AriaHidden( "true" );
            builder.CloseElement();
        }

        #endregion
    }
}
