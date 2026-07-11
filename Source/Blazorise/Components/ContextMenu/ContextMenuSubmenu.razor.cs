#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a context menu item that opens a nested menu.
/// </summary>
public partial class ContextMenuSubmenu : BaseComponent
{
    #region Members

    private bool visible;

    private ClassBuilder triggerClassBuilder;

    private ClassBuilder submenuClassBuilder;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ContextMenuSubmenu() );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        triggerClassBuilder?.Dirty();
        submenuClassBuilder?.Dirty();

        base.DirtyClasses();
    }

    /// <summary>
    /// Handles submenu visibility changes from the underlying dropdown.
    /// </summary>
    /// <param name="visible">The new submenu visibility state.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private Task OnVisibleChanged( bool visible )
    {
        Visible = visible;

        return Task.CompletedTask;
    }

    private void BuildTriggerClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ContextMenuSubmenuTrigger() );
        builder.Append( ClassProvider.ContextMenuSubmenuTriggerDisabled( Disabled ) );
    }

    private void BuildSubmenuClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ContextMenuSubmenuBody() );
    }

    #endregion

    #region Properties

    private string TriggerClassNames => ( triggerClassBuilder ??= new( BuildTriggerClasses ) ).Class;

    private string SubmenuClassNames => ( submenuClassBuilder ??= new( BuildSubmenuClasses ) ).Class;

    private DropdownTrigger EffectiveTrigger => Trigger ?? ParentContextMenu?.EffectiveSubmenuTrigger ?? DropdownTrigger.All;

    private int EffectiveHoverCloseDelay => HoverCloseDelay ?? ParentContextMenu?.EffectiveSubmenuHoverCloseDelay ?? 300;

    /// <summary>
    /// Gets or sets the submenu visibility.
    /// </summary>
    protected bool Visible
    {
        get => visible;
        set
        {
            if ( visible == value )
                return;

            visible = value;
            DirtyClasses();
        }
    }

    /// <summary>
    /// Provides the reference to the parent <see cref="ContextMenu"/> component.
    /// </summary>
    [CascadingParameter] protected ContextMenu ParentContextMenu { get; set; }

    /// <summary>
    /// Specifies the submenu label.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Specifies the icon to render before the submenu label.
    /// </summary>
    [Parameter] public object Icon { get; set; }

    /// <summary>
    /// Indicates the submenu is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Defines which pointer interactions can open or close this submenu.
    /// </summary>
    [Parameter] public DropdownTrigger? Trigger { get; set; }

    /// <summary>
    /// Delay in milliseconds before hiding this hover-opened submenu after the mouse leaves it.
    /// </summary>
    [Parameter] public int? HoverCloseDelay { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="ContextMenuSubmenu"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}