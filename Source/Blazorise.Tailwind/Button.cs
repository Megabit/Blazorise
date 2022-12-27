#region Using directives
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Tailwind;

public class Button : Blazorise.Button
{
    #region Members

    bool collapseVisible;

    #endregion

    #region Methods

    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( ParentButtons != null )
        {
            builder.Append( "rounded-none first:rounded-l-lg last:rounded-r-lg" );
        }
        else if ( ParentCollapseHeader?.ParentCollapse != null )
        {
            // TODO v2: Obsolete, remove this in v2! We have introduced AccordionToggle instead of using regular Button for toggle of collapse in the accordion.
            if ( ParentCollapseHeader.ParentCollapse.InsideAccordion )
            {
                builder.Append( ClassProvider.AccordionToggle() );
            }

            builder.Append( ClassProvider.AccordionToggleCollapsed( CollapseVisible ) );

            return;
        }
        else
        {
            builder.Append( "rounded-lg" );
        }

        base.BuildClasses( builder );
    }

    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
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

        if ( ParentCollapseHeader?.ParentCollapse != null )
        {
            builder.AriaExpanded( ParentCollapseHeader.ParentCollapse.Visible.ToString().ToLowerInvariant() );
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

        if ( ParentCollapseHeader?.ParentCollapse != null )
        {
            builder.OpenElement( "svg" );

            builder.Attribute( "data-accordion-icon", null );

            builder.Class( CollapseVisible
                ? "w-6 h-6 shrink-0 rotate-180"
                : "w-6 h-6 shrink-0" );

            builder.Attribute( "fill", "currentColor" );
            builder.Attribute( "viewBox", "0 0 20 20" );
            builder.Attribute( "xmlns", "http://www.w3.org/2000/svg" );

            builder
                .OpenElement( "path" )
                .Attribute( "fill-rule", "evenodd" )
                .Attribute( "d", "M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" )
                .Attribute( "clip-rule", "evenodd" );
            builder.CloseElement();

            builder.CloseElement();
        }

        builder.CloseElement();
    }

    /// <inheritdoc/>
    protected override RenderFragment ProvideDefaultLoadingTemplate()
    {
        return builder =>
        {
            builder
                .OpenElement( "svg" )
                .AriaHidden( true )
                .Role( "status" )
                .Class( "inline mr-3 w-4 h-4 text-white animate-spin" )
                .Attribute( "viewBox", "0 0 100 101" )
                .Fill( "none" )
                .Attribute( "xmlns", "http://www.w3.org/2000/svg" );

            builder.OpenElement( "path" )
                .Attribute( "d", "M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z" )
                .Class( GetFillName( Color ) );

            builder.OpenElement( "path" )
                .Attribute( "d", "M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z" )
                .Fill( "currentColor" );

            builder.CloseElement();
            builder.CloseElement();
            builder.CloseElement();

            builder.Content( ChildContent );
        };
    }

    private static string GetFillName( Color color )
    {
        return color?.Name switch
        {
            "primary" => "fill-primary-200",
            "secondary" => "fill-secondary-200",
            "success" => "fill-success-200",
            "danger" => "fill-danger-200",
            "warning" => "fill-warning-200",
            "info" => "fill-info-200",
            "light" => "fill-light-700",
            "dark" => "fill-dark-200",
            "link" => "fill-primary-600",
            _ => "fill-gray-200"
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the content visibility.
    /// </summary>
    [CascadingParameter( Name = "CollapseVisible" )]
    public bool CollapseVisible
    {
        get => collapseVisible;
        set
        {
            collapseVisible = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the cascaded parent collapse header component.
    /// </summary>
    [CascadingParameter] protected CollapseHeader ParentCollapseHeader { get; set; }

    #endregion
}