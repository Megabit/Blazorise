#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Displays selectable command previews, such as document styles.
/// </summary>
public partial class RibbonGallery : BaseComponent
{
    #region Members

    private Div containerRef;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes the gallery layout and width constraint.
    /// </summary>
    public RibbonGallery()
    {
        Flex = Blazorise.Flex.Column.Shrink.Is0;
        Width = Blazorise.Width.Rem().Min( 0 ).Max( 32 );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            ElementRef = containerRef.ElementRef;
        }

        return base.OnAfterRenderAsync( firstRender );
    }

    /// <summary>
    /// Selects a gallery value through the same path as user interaction.
    /// </summary>
    /// <param name="value">
    /// Value of the item to select.
    /// </param>
    public virtual async Task SelectValue( string value )
    {
        if ( Disabled || string.Equals( SelectedValue, value, StringComparison.Ordinal ) )
        {
            return;
        }

        SelectedValue = value;

        await SelectedValueChanged.InvokeAsync( value );
        await InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the accessible name of the gallery, such as Document styles.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Specifies the <see cref="RibbonGalleryItem.Value"/> of the selected preview.
    /// </summary>
    [Parameter] public string SelectedValue { get; set; }

    /// <summary>
    /// Occurs when a different gallery value is selected. Supplies the selected value for two-way binding.
    /// </summary>
    [Parameter] public EventCallback<string> SelectedValueChanged { get; set; }

    /// <summary>
    /// Prevents selecting any preview in the gallery. Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Defines the selectable previews using <see cref="RibbonGalleryItem"/> components.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Defines optional actions displayed below the previews, such as managing or creating styles.
    /// </summary>
    [Parameter] public RenderFragment FooterContent { get; set; }

    /// <summary>
    /// Shares the containing ribbon's selection, display mode, and rendering policy with this component.
    /// </summary>
    [CascadingParameter] protected RibbonState ParentRibbonState { get; set; }

    #endregion
}