#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a compact icon command inside a <see cref="ContextMenuToolbar"/>.
/// </summary>
public partial class ContextMenuToolbarItem : BaseComponent
{
    #region Members

    private bool active;

    private bool disabled;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ContextMenuToolbarItem() );
        builder.Append( ClassProvider.ContextMenuToolbarItemActive( Active ) );
        builder.Append( ClassProvider.ContextMenuToolbarItemDisabled( Disabled ) );

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

        await Clicked.InvokeAsync( Value );

        if ( ParentContextMenu?.EffectiveCloseOnClick == true )
            await ParentContextMenu.Hide();
    }

    private Task ButtonClicked( MouseEventArgs eventArgs )
        => ClickHandler();

    #endregion

    #region Properties

    /// <summary>
    /// Holds the item value.
    /// </summary>
    [Parameter] public object Value { get; set; }

    /// <summary>
    /// Specifies the icon to render inside the toolbar item.
    /// </summary>
    [Parameter] public object Icon { get; set; }

    /// <summary>
    /// Specifies the toolbar item text. It is used as the default title and aria-label.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Indicates whether <see cref="Text"/> should be rendered visibly next to the icon.
    /// </summary>
    [Parameter] public bool ShowText { get; set; }

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
    /// Notifies when the toolbar item is clicked.
    /// </summary>
    [Parameter] public EventCallback<object> Clicked { get; set; }

    /// <summary>
    /// Provides the reference to the parent <see cref="ContextMenu"/> component.
    /// </summary>
    [CascadingParameter] protected ContextMenu ParentContextMenu { get; set; }

    #endregion
}