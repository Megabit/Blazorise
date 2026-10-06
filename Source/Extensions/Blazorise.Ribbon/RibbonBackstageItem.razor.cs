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
    /// Indicates whether this enabled, visible entry owns the selected backstage page.
    /// </summary>
    protected bool IsSelected => CanSelect && ParentBackstageState?.SelectedItem == Name;

    /// <summary>
    /// Determines whether to create or retain this page according to the backstage rendering policy and its selection history.
    /// </summary>
    protected bool ShouldRenderContent => ParentBackstageState?.RenderMode switch
    {
        TabsRenderMode.LazyLoad => hasBeenSelected,
        TabsRenderMode.LazyReload => IsSelected && hasBeenSelected,
        _ => true,
    };

    /// <summary>
    /// Identifies the entry for page selection through <see cref="RibbonBackstage.SelectedItem"/>.
    /// </summary>
    /// <remarks>
    /// Must be non-empty and unique within the containing backstage.
    /// </remarks>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Specifies the label displayed in backstage navigation.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Specifies the icon displayed beside the label. Accepts an icon name supported by the configured icon provider.
    /// </summary>
    [Parameter] public object Icon { get; set; }

    /// <summary>
    /// Prevents selecting the page or activating the command while keeping the navigation entry visible.
    /// Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Controls whether the navigation entry is shown and can be activated. Defaults to <c>true</c>.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Occurs when the entry is activated, after selecting its page when present.
    /// </summary>
    /// <remarks>
    /// Commands do not close backstage automatically; close it from this callback when needed.
    /// </remarks>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Defines the page displayed when this entry is selected. Without content, the entry invokes its command without selecting a page.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    [CascadingParameter] internal RibbonBackstage ParentBackstage { get; set; }

    /// <summary>
    /// Shares the containing backstage's visibility, selection, and rendering policy with this entry.
    /// </summary>
    [CascadingParameter] protected RibbonBackstageState ParentBackstageState { get; set; }

    #endregion
}