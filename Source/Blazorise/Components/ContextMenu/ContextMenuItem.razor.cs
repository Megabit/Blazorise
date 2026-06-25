#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// A selectable item for a <see cref="ContextMenuBody"/>.
/// </summary>
public partial class ContextMenuItem : BaseComponent
{
    #region Members

    private bool active;

    private bool disabled;

    private bool @checked;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<bool>( nameof( Checked ), out var paramChecked ) && !paramChecked.IsEqual( @checked ) )
            @checked = paramChecked;

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ContextMenuItem() );
        builder.Append( ClassProvider.ContextMenuItemActive( Active ) );
        builder.Append( ClassProvider.ContextMenuItemDisabled( Disabled ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Handles the onclick event, if not disabled.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task ClickHandler()
    {
        if ( Disabled )
            return;

        if ( ParentGroup?.CheckMode == ContextMenuCheckMode.Radio )
        {
            await ParentGroup.SelectValue( Value );
        }
        else if ( ShowCheckbox )
        {
            await CheckedChangedHandler( !@checked );
        }

        await Clicked.InvokeAsync( Value );

        if ( ParentContextMenu?.EffectiveCloseOnClick == true && !EffectiveShowCheckbox )
            await ParentContextMenu.Hide();
    }

    /// <summary>
    /// Handles keyboard activation for menu items.
    /// </summary>
    /// <param name="eventArgs">Information about the keyboard event.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task KeyDownHandler( KeyboardEventArgs eventArgs )
    {
        if ( eventArgs.Key == "Enter" || eventArgs.Key == "NumpadEnter" || eventArgs.Key == " " )
            return ClickHandler();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the checked state change.
    /// </summary>
    /// <param name="isChecked">The new checked state.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task CheckedChangedHandler( bool isChecked )
    {
        if ( Disabled )
            return;

        @checked = isChecked;

        if ( CheckedChanged.HasDelegate )
            await CheckedChanged.InvokeAsync( isChecked );
    }

    #endregion

    #region Properties

    private bool EffectiveShowCheckbox => ShowCheckbox || ParentGroup?.CheckMode == ContextMenuCheckMode.Radio;

    private bool IsChecked => ParentGroup?.CheckMode == ContextMenuCheckMode.Radio ? ParentGroup.IsSelected( Value ) : @checked;

    private string Role => EffectiveShowCheckbox ? "menuitemcheckbox" : "menuitem";

    private string AriaDisabled => Disabled.ToString().ToLowerInvariant();

    private string AriaChecked => EffectiveShowCheckbox ? IsChecked.ToString().ToLowerInvariant() : null;

    private int ComputedTabIndex => Disabled ? -1 : TabIndex ?? 0;

    private string ContentClassNames => ClassProvider.ContextMenuItemContent();

    private string CheckClassNames => ClassProvider.ContextMenuItemCheck();

    private string ShortcutClassNames => ClassProvider.ContextMenuItemShortcut();

    /// <summary>
    /// Holds the item value.
    /// </summary>
    [Parameter] public object Value { get; set; }

    /// <summary>
    /// Specifies the icon to render before the item content.
    /// </summary>
    [Parameter] public object Icon { get; set; }

    /// <summary>
    /// Specifies shortcut text rendered at the end of the item.
    /// </summary>
    [Parameter] public string Shortcut { get; set; }

    /// <summary>
    /// Specifies the tabindex value for keyboard navigation.
    /// </summary>
    [Parameter] public int? TabIndex { get; set; }

    /// <summary>
    /// Indicates the currently active item.
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
    /// Indicates the currently disabled item.
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
    /// The item renders a checkbox.
    /// </summary>
    [Parameter] public bool ShowCheckbox { get; set; }

    /// <summary>
    /// Tracks the checked state whenever the item is in checkbox mode.
    /// </summary>
    [Parameter] public bool Checked { get; set; }

    /// <summary>
    /// Occurs after the checked state changes whenever the item is in checkbox mode.
    /// </summary>
    [Parameter] public EventCallback<bool> CheckedChanged { get; set; }

    /// <summary>
    /// Notifies when the item is clicked.
    /// </summary>
    [Parameter] public EventCallback<object> Clicked { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="ContextMenuItem"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Provides the reference to the parent <see cref="ContextMenu"/> component.
    /// </summary>
    [CascadingParameter] protected ContextMenu ParentContextMenu { get; set; }

    /// <summary>
    /// Provides the reference to the parent <see cref="ContextMenuGroup"/> component.
    /// </summary>
    [CascadingParameter] protected ContextMenuGroup ParentGroup { get; set; }

    #endregion
}