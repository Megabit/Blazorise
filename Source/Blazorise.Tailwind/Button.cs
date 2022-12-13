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
        if ( ParentCollapseHeader?.ParentCollapse != null )
        {
            if ( ParentCollapseHeader.ParentCollapse.InsideAccordion )
            {
                var first = ParentCollapseHeader.ParentCollapse.FirstInAccordion;
                var last = ParentCollapseHeader.ParentCollapse.LastInAccordion;

                if ( first )
                {
                    builder.Append( CollapseVisible
                        ? "flex items-center justify-between w-full p-5 font-medium text-left border border-b-0 border-gray-200 rounded-t-xl focus:ring-4 focus:ring-gray-200 dark:focus:ring-gray-800 dark:border-gray-700 hover:bg-gray-100 dark:hover:bg-gray-800 bg-gray-100 dark:bg-gray-800 text-gray-900 dark:text-white"
                        : "flex items-center justify-between w-full p-5 font-medium text-left border border-b-0 border-gray-200 rounded-t-xl focus:ring-4 focus:ring-gray-200 dark:focus:ring-gray-800 dark:border-gray-700 hover:bg-gray-100 dark:hover:bg-gray-800 text-gray-500 dark:text-gray-400" );

                }
                else if ( last )
                {
                    builder.Append( CollapseVisible
                        ? "flex items-center justify-between w-full p-5 font-medium text-left border border-gray-200 focus:ring-4 focus:ring-gray-200 dark:focus:ring-gray-800 dark:border-gray-700 hover:bg-gray-100 dark:hover:bg-gray-800 bg-gray-100 dark:bg-gray-800 text-gray-900 dark:text-white"
                        : "flex items-center justify-between w-full p-5 font-medium text-left border border-gray-200 focus:ring-4 focus:ring-gray-200 dark:focus:ring-gray-800 dark:border-gray-700 hover:bg-gray-100 dark:hover:bg-gray-800 text-gray-500 dark:text-gray-400" );

                }
                else
                {
                    builder.Append( CollapseVisible
                        ? "flex items-center justify-between w-full p-5 font-medium text-left border border-b-0 border-gray-200 focus:ring-4 focus:ring-gray-200 dark:focus:ring-gray-800 dark:border-gray-700 hover:bg-gray-100 dark:hover:bg-gray-800 bg-gray-100 dark:bg-gray-800 text-gray-900 dark:text-white"
                        : "flex items-center justify-between w-full p-5 font-medium text-left border border-b-0 border-gray-200 focus:ring-4 focus:ring-gray-200 dark:focus:ring-gray-800 dark:border-gray-700 hover:bg-gray-100 dark:hover:bg-gray-800 text-gray-500 dark:text-gray-400" );
                }
            }

            builder.Append( "collapsed", !CollapseVisible );

            return;
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
            builder.OpenElement( "span" );
            builder
                .Class( "spinner-border spinner-border-sm" )
                .Role( "status" )
                .AriaHidden( "true" );
            builder.CloseElement();
            builder.Content( ChildContent );
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