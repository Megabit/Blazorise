#region Using directives
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Bulma
{
    public partial class Button : Blazorise.Button
    {
        #region Methods

        protected override void BuildRenderTree( RenderTreeBuilder builder )
        {
            if ( IsAddons || ParentIsField )
            {
                builder
                    .OpenElement( "div" )
                    .Class( "control" );
            }

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

            builder.Content( ChildContent );

            builder.CloseElement();

            if ( IsAddons || ParentIsField )
            {
                builder.CloseElement();
            }
        }

        #endregion
    }
}
