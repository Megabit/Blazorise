#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Declares a backstage navigation entry and an optional application-owned page.
/// </summary>
public partial class RibbonBackstageItem : BaseComponent
{
    #region Events

    internal event Action HeadingChanged;

    #endregion

    #region Members

    private bool hasBeenSelected;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if ( ParentBackstage is null )
        {
            throw new InvalidOperationException( "RibbonBackstageItem must be placed inside a RibbonBackstage." );
        }

        ParentBackstage.RegisterItem( this );
    }

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var nameChanged = parameters.IsParameterChanged( Name );
        var disabledChanged = parameters.IsParameterChanged( Disabled );
        var visibleChanged = parameters.IsParameterChanged( Visible );

        var hasPageChanged = parameters.TryGetValue<RenderFragment>( nameof( ChildContent ), out var paramChildContent )
            && ( paramChildContent is null ) != ( ChildContent is null );

        await base.SetParametersAsync( parameters );

        ParentBackstage.ValidateItem( this );

        if ( nameChanged || disabledChanged || visibleChanged || hasPageChanged )
        {
            ParentBackstage.Refresh();
        }

        HeadingChanged?.Invoke();
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        hasBeenSelected |= IsSelected && ParentBackstageState.Visible;

        base.OnParametersSet();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentBackstage?.UnregisterItem( this );
        }

        base.Dispose( disposing );
    }

    internal Task OnClickHandler( MouseEventArgs eventArgs )
        => CanActivate && ParentBackstage.Visible ? HandleClick( eventArgs ) : Task.CompletedTask;

    /// <summary>
    /// Selects the page, when present, before invoking the item callback.
    /// </summary>
    protected virtual async Task HandleClick( MouseEventArgs eventArgs )
    {
        if ( HasPage )
        {
            await ParentBackstage.SelectItem( Name );
        }

        await Clicked.InvokeAsync( eventArgs );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    internal bool HasPage => ChildContent is not null;

    internal bool CanActivate => Visible && !Disabled;

    internal bool CanSelect => HasPage && CanActivate;

    internal string ContentElementId => $"{ElementId}-content";

    /// <summary>
    /// Gets whether this item owns the selected page.
    /// </summary>
    protected bool IsSelected => CanSelect && ParentBackstageState?.SelectedItem == Name;

    /// <summary>
    /// Gets whether to create page content under the rendering policy.
    /// </summary>
    protected bool ShouldRenderContent => ParentBackstageState?.RenderMode switch
    {
        TabsRenderMode.LazyLoad => hasBeenSelected,
        TabsRenderMode.LazyReload => IsSelected && hasBeenSelected,
        _ => true,
    };

    /// <summary>
    /// Gets or sets the unique item name.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Gets or sets the navigation label.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Gets or sets an icon supported by the configured icon provider.
    /// </summary>
    [Parameter] public object Icon { get; set; }

    /// <summary>
    /// Gets or sets whether the item is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets whether the item is available.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Occurs when the entry is activated. Commands do not close backstage automatically.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Gets or sets the page content. An item without content invokes its command only.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    [CascadingParameter] internal RibbonBackstage ParentBackstage { get; set; }

    /// <summary>
    /// Gets the containing backstage state.
    /// </summary>
    [CascadingParameter] protected RibbonBackstageState ParentBackstageState { get; set; }

    #endregion
}