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
    /// Gets or sets the gallery's accessible name.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Gets or sets the selected preview value.
    /// </summary>
    [Parameter] public string SelectedValue { get; set; }

    /// <summary>
    /// Occurs when a preview is selected.
    /// </summary>
    [Parameter] public EventCallback<string> SelectedValueChanged { get; set; }

    /// <summary>
    /// Gets or sets whether selection is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the gallery items.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets optional gallery footer actions.
    /// </summary>
    [Parameter] public RenderFragment FooterContent { get; set; }

    /// <summary>
    /// Gets the containing ribbon state.
    /// </summary>
    [CascadingParameter] protected RibbonState ParentRibbonState { get; set; }

    #endregion
}