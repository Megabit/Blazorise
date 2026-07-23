#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Groups related context menu items and can coordinate radio checked state.
/// </summary>
public partial class ContextMenuGroup : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ContextMenuGroup() );

        base.BuildClasses( builder );
    }

    internal bool IsSelected( object value )
        => Equals( SelectedValue, value );

    internal async Task SelectValue( object value )
    {
        if ( CheckMode != ContextMenuCheckMode.Radio || Equals( SelectedValue, value ) )
            return;

        SelectedValue = value;

        await SelectedValueChanged.InvokeAsync( value );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="ContextMenuGroup"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Defines how child items handle checked state.
    /// </summary>
    [Parameter] public ContextMenuCheckMode CheckMode { get; set; }

    /// <summary>
    /// Gets or sets the currently selected value when <see cref="CheckMode"/> is <see cref="ContextMenuCheckMode.Radio"/>.
    /// </summary>
    [Parameter] public object SelectedValue { get; set; }

    /// <summary>
    /// Occurs after the selected value changes.
    /// </summary>
    [Parameter] public EventCallback<object> SelectedValueChanged { get; set; }

    #endregion
}