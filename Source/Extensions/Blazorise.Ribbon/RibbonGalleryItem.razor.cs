#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Provides a selectable gallery preview with application-defined content.
/// </summary>
public partial class RibbonGalleryItem : BaseComponent
{
    #region Constructors

    /// <summary>
    /// Initializes the preview layout and spacing.
    /// </summary>
    public RibbonGalleryItem()
    {
        Flex = Blazorise.Flex.Shrink.Is0;
        Height = Blazorise.Height.Is100;
        Width = Blazorise.Width.Rem().Min( 5 );
        Padding = Blazorise.Padding.Is1.OnY.Is2.OnX;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if ( ParentGallery is null )
        {
            throw new InvalidOperationException( "RibbonGalleryItem must be placed inside a RibbonGallery." );
        }

        base.OnInitialized();
    }

    /// <summary>
    /// Handles selection of the preview.
    /// </summary>
    protected Task OnClickHandler() => IsDisabled ? Task.CompletedTask : ParentGallery.SelectValue( Value );

    #endregion

    #region Properties

    /// <summary>
    /// Gets whether the gallery selected this preview.
    /// </summary>
    protected bool IsSelected => Value is not null && string.Equals( SelectedValue, Value, StringComparison.Ordinal );

    /// <summary>
    /// Gets whether selection is disabled by this item or its gallery.
    /// </summary>
    protected bool IsDisabled => Disabled || ParentDisabled;

    /// <summary>
    /// Gets or sets the actual value represented by this preview.
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// Gets or sets the caption and accessible name.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Gets or sets whether this preview is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets preview content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets the fixed gallery that owns selection.
    /// </summary>
    [CascadingParameter] protected RibbonGallery ParentGallery { get; set; }

    /// <summary>
    /// Gets the current gallery selection.
    /// </summary>
    [CascadingParameter( Name = "RibbonGallerySelectedValue" )] protected string SelectedValue { get; set; }

    /// <summary>
    /// Gets whether the containing gallery is disabled.
    /// </summary>
    [CascadingParameter( Name = "RibbonGalleryDisabled" )] protected bool ParentDisabled { get; set; }

    #endregion
}