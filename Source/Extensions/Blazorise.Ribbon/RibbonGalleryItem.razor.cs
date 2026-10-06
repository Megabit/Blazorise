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
    /// Indicates whether this preview's value matches the containing gallery's selection.
    /// </summary>
    protected bool IsSelected => Value is not null && string.Equals( SelectedValue, Value, StringComparison.Ordinal );

    /// <summary>
    /// Indicates whether selection is disabled by this preview or the containing gallery.
    /// </summary>
    protected bool IsDisabled => Disabled || ParentDisabled;

    /// <summary>
    /// Identifies the value selected when this preview is activated and compared with <see cref="RibbonGallery.SelectedValue"/>.
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// Specifies the caption displayed below the preview and used as its accessible name and tooltip.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Prevents selecting this preview.
    /// </summary>
    /// <remarks>
    /// The containing gallery can also disable all previews. Defaults to <c>false</c>.
    /// </remarks>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Defines the visual preview displayed above the caption, such as formatted sample text.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Provides the containing gallery that owns this preview's selection.
    /// </summary>
    [CascadingParameter] protected RibbonGallery ParentGallery { get; set; }

    /// <summary>
    /// Shares the selected value from the containing gallery for determining the preview's active appearance.
    /// </summary>
    [CascadingParameter( Name = "RibbonGallerySelectedValue" )] protected string SelectedValue { get; set; }

    /// <summary>
    /// Shares the containing gallery's disabled state so that all previews respect it.
    /// </summary>
    [CascadingParameter( Name = "RibbonGalleryDisabled" )] protected bool ParentDisabled { get; set; }

    #endregion
}