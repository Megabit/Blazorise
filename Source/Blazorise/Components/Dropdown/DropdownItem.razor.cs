#region Using directives

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;

#endregion

namespace Blazorise;

/// <summary>
/// A menu item for the <see cref="DropdownMenu"/> component.
/// </summary>
public partial class DropdownItem : BaseComponent
{
    #region Members

    private bool active;

    private bool disabled;

    /// <summary>
    /// Internal Checked Value
    /// </summary>
    protected bool @checked;

    /// <summary>
    /// Internal Checked Expression
    /// </summary>
    protected Expression<Func<bool>> checkedExpression => () => @checked;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<bool>( nameof( Checked ), out var paramChecked ) && !paramChecked.IsEqual( @checked ) )
        {
            @checked = paramChecked;
        }

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DropdownItem() );
        builder.Append( ClassProvider.DropdownItemActive( Active ) );
        builder.Append( ClassProvider.DropdownItemDisabled( Disabled ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Handles the onclick event, if not disabled.
    /// </summary>
    /// <returns></returns>
    protected async Task ClickHandler()
    {
        if ( Disabled )
            return;

        if ( ParentDropdown is not null )
        {
            if ( !ParentDropdown.WasJustToggled && !ShowCheckbox )
                await ParentDropdown.Hide( true );
        }

        if ( ShowCheckbox )
        {
            await CheckedChangedHandler( !@checked );
        }

        await Clicked.InvokeAsync( Value );
    }

    /// <summary>
    /// Handles the oncheck event, if not disabled.
    /// </summary>
    /// <param name="isChecked"></param>
    /// <returns></returns>
    protected async Task CheckedChangedHandler( bool isChecked )
    {
        if ( !Disabled )
        {
            @checked = isChecked;
            await CheckedChanged.InvokeAsync( isChecked );
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the aria-disabled attribute value.
    /// </summary>
    protected string AriaDisabled => Disabled.ToString().ToLowerInvariant();

    /// <summary>
    /// Holds the item value.
    /// </summary>
    [Parameter] public object Value { get; set; }

    /// <summary>
    /// Indicate the currently active item.
    /// </summary>
    [Parameter]
    public bool Active
    {
        get => active;
        set
        {
            if ( active == value )
                return;

            active = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Indicate the currently disabled item.
    /// </summary>
    [Parameter]
    public bool Disabled
    {
        get => disabled;
        set
        {
            if ( disabled == value )
                return;

            disabled = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Occurs when the item is clicked.
    /// </summary>
    [Parameter] public EventCallback<object> Clicked { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="DropdownItem"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the reference to the parent dropdown.
    /// </summary>
    [CascadingParameter] protected Dropdown ParentDropdown { get; set; }

    /// <summary>
    /// The dropdown renders a checkbox.
    /// </summary>
    [Parameter] public bool ShowCheckbox { get; set; }

    /// <summary>
    /// Tracks the Checked state whenever the DropdownItem is in checkbox mode.
    /// </summary>
    [Parameter] public bool Checked { get; set; }

    /// <summary>
    /// Occurs after the Checked state is changed, whenever the DropdownItem is in checkbox mode.
    /// </summary>
    [Parameter] public EventCallback<bool> CheckedChanged { get; set; }

    #endregion
}