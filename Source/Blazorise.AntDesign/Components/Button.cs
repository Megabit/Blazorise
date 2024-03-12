#region Using directives
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.AntDesign.Components;

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
            .AriaPressed( Active )
            .TabIndex( TabIndex );

        if ( Type == ButtonType.Link )
        {
            builder
                .Role( "button" )
                .Href( To )
                .Target( Target );

            if ( Disabled )
            {
                builder
                    .TabIndex( -1 )
                    .AriaDisabled( "true" );
            }
        }

        builder.OnClick( this, EventCallback.Factory.Create<MouseEventArgs>( this, ClickHandler ) );
        builder.OnClickPreventDefault( Type == ButtonType.Link && To != null && To.StartsWith( "#" ) );

        builder.Attributes( Attributes );
        builder.ElementReferenceCapture( capturedRef => ElementRef = capturedRef );

        if ( Loading && LoadingTemplate != null )
        {
            builder.Content( LoadingTemplate );
        }
        else
        {
            builder.Content( ChildContent );
        }

        builder.CloseElement();

        if ( IsAddons || ParentIsField )
        {
            builder.CloseElement();
        }
    }

    /// <inheritdoc/>
    protected override RenderFragment ProvideDefaultLoadingTemplate()
    {
        return builder =>
        {
            builder
                .OpenElement( "span" )
                .Role( "img" )
                .AriaLabel( "loading" )
                .Class( "anticon anticon-loading" );

            builder
                .OpenElement( "svg" )
                .Attribute( "viewBox", "0 0 1024 1024" )
                .Attribute( "focusable", "false" )
                .Class( "anticon-spin" )
                .Data( "icon", "loading" )
                .Width( "1em" )
                .Height( "1em" )
                .Fill( "currentColor" )
                .AriaHidden( "true" );

            builder
                .OpenElement( "path" )
                .Attribute( "d", "M988 548c-19.9 0-36-16.1-36-36 0-59.4-11.6-117-34.6-171.3a440.45 440.45 0 00-94.3-139.9 437.71 437.71 0 00-139.9-94.3C629 83.6 571.4 72 512 72c-19.9 0-36-16.1-36-36s16.1-36 36-36c69.1 0 136.2 13.5 199.3 40.3C772.3 66 827 103 874 150c47 47 83.9 101.8 109.7 162.7 26.7 63.1 40.2 130.2 40.2 199.3.1 19.9-16 36-35.9 36z" )
                .CloseElement();

            builder.CloseElement();
            builder.CloseElement();

            builder
                .OpenElement( "span" )
                .Content( ChildContent )
                .CloseElement();
        };
    }

    #endregion
}