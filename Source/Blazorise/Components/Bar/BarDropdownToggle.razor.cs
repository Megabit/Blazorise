#region Using directives
using System;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// Toggles the visibility or collapse of <see cref="Bar"/> component.
/// </summary>
public partial class BarDropdownToggle : BaseLinkComponent, ICloseActivator, IAsyncDisposable
{
    #region Members

    private bool jsRegistered;

    private DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef;

    private DateTime? lastKeyboardToggleTimestampUtc;

    private double indentation = 1.5d;

    private bool? toggleIconVisible;

    private Theme theme;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="BarDropdownToggle"/> constructor.
    /// </summary>
    public BarDropdownToggle()
    {
        ToggleContentClassBuilder = new( BuildToggleContentClasses );
        ToggleContentTextClassBuilder = new( BuildToggleContentTextClasses );
        ToggleContentIconClassBuilder = new( BuildToggleContentIconClasses );
        ToggleIconContainerClassBuilder = new( BuildToggleIconContainerClasses );
        ExpandedToggleIconLayerClassBuilder = new( BuildExpandedToggleIconLayerClasses );
        CollapsedToggleIconLayerClassBuilder = new( BuildCollapsedToggleIconLayerClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnFirstAfterRenderAsync()
    {
        dotNetObjectRef ??= CreateDotNetObjectRef( new CloseActivatorAdapter( this ) );

        return base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override void OnActiveChanged( bool active )
    {
        if ( !IsRouteMatchTriggerEnabled )
            return;

        _ = InvokeAsync( async () => await HandleRouteMatchTriggerAsync( active ) );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BarDropdownToggle( ParentBarDropdownState.Mode, ParentBarDropdown?.IsBarDropdownSubmenu == true ) );
        builder.Append( ClassProvider.BarDropdownToggleDisabled( ParentBarDropdownState.Mode, ParentBarDropdown?.IsBarDropdownSubmenu == true, IsDisabled ) );
        builder.Append( ClassProvider.BarDropdownToggleIcon( IsToggleIconVisible ) );

        if ( To != null )
        {
            builder.Append( ClassProvider.BarLink( ParentBarDropdownState.Mode ) );
            builder.Append( ClassProvider.LinkActive( Active ) );
        }

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        base.BuildStyles( builder );

        builder.Append( $"padding-left: {( Indentation * ParentBarDropdownState.NestedIndex ).ToString( CultureInfo.InvariantCulture )}rem", ParentBarDropdownState.IsInlineDisplay );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            if ( Rendered )
            {
                // make sure to unregister listener
                if ( jsRegistered )
                {
                    jsRegistered = false;

                    var task = JSClosableModule.Unregister( this );

                    try
                    {
                        await task;
                    }
                    catch when ( task.IsCanceled )
                    {
                    }
                    catch ( JSDisconnectedException )
                    {
                    }
                }

                DisposeDotNetObjectRef( dotNetObjectRef );
                dotNetObjectRef = null;
            }
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Handles the button click event.
    /// </summary>
    /// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
    /// <returns>Returns the awaitable task.</returns>
    protected virtual async Task ClickHandler( MouseEventArgs eventArgs )
    {
        if ( IsDisabled )
            return;

        if ( ParentBarDropdown is not null )
        {
            if ( IsToggleClickTriggerEnabled
                 && eventArgs.Detail == 0
                 && ParentBarDropdown.IsVisible )
            {
                lastKeyboardToggleTimestampUtc = null;
                return;
            }

            if ( IsToggleClickTriggerEnabled
                 && eventArgs.Detail == 0
                 && lastKeyboardToggleTimestampUtc.HasValue
                 && DateTime.UtcNow.Subtract( lastKeyboardToggleTimestampUtc.Value ).TotalMilliseconds < 500 )
            {
                lastKeyboardToggleTimestampUtc = null;
                return;
            }

            if ( IsToggleClickTriggerEnabled )
                await ParentBarDropdown.Toggle( ElementId );
        }

        await Clicked.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Handles the toggle icon click event.
    /// </summary>
    /// <returns>Returns the awaitable task.</returns>
    protected Task OnToggleIconClicked()
    {
        if ( ParentBarDropdown is not null && IsIconClickTriggerEnabled )
            return ParentBarDropdown.Toggle( ElementId );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Builds class names for toggle content wrapper.
    /// </summary>
    /// <param name="builder">Class builder.</param>
    protected virtual void BuildToggleContentClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BarDropdownToggleContent( ParentBarDropdownState?.Mode ?? BarMode.Horizontal ) );
    }

    /// <summary>
    /// Builds class names for toggle content text wrapper.
    /// </summary>
    /// <param name="builder">Class builder.</param>
    protected virtual void BuildToggleContentTextClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BarDropdownToggleContentText( ParentBarDropdownState?.Mode ?? BarMode.Horizontal ) );
    }

    /// <summary>
    /// Builds class names for toggle content icon wrapper.
    /// </summary>
    /// <param name="builder">Class builder.</param>
    protected virtual void BuildToggleContentIconClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BarDropdownToggleContentIcon( ParentBarDropdownState?.Mode ?? BarMode.Horizontal ) );
    }

    /// <summary>
    /// Builds class names for toggle icon container.
    /// </summary>
    /// <param name="builder">Class builder.</param>
    protected virtual void BuildToggleIconContainerClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BarDropdownToggleIconContainer( ParentBarDropdownState?.Mode ?? BarMode.Horizontal ) );
    }

    /// <summary>
    /// Builds class names for a single icon animation layer.
    /// </summary>
    /// <param name="builder">Class builder.</param>
    /// <param name="expandedStateLayer">True for expanded-state icon layer; otherwise collapsed-state icon layer.</param>
    protected virtual void BuildToggleIconLayerClasses( ClassBuilder builder, bool expandedStateLayer )
    {
        var isExpanded = ParentBarDropdown?.IsVisible == true;
        var barMode = ParentBarDropdownState?.Mode ?? BarMode.Horizontal;

        builder.Append( ClassProvider.BarDropdownToggleIconLayer( barMode ) );

        if ( expandedStateLayer )
        {
            builder.Append( ClassProvider.BarDropdownToggleIconLayerVisible( barMode, isExpanded ) );
            builder.Append( ClassProvider.BarDropdownToggleIconLayerHiddenExpand( barMode, !isExpanded ) );
        }
        else
        {
            builder.Append( ClassProvider.BarDropdownToggleIconLayerHiddenCollapse( barMode, isExpanded ) );
            builder.Append( ClassProvider.BarDropdownToggleIconLayerVisible( barMode, !isExpanded ) );
        }
    }

    /// <summary>
    /// Builds class names for expanded-state icon layer.
    /// </summary>
    /// <param name="builder">Class builder.</param>
    protected virtual void BuildExpandedToggleIconLayerClasses( ClassBuilder builder )
    {
        BuildToggleIconLayerClasses( builder, true );
    }

    /// <summary>
    /// Builds class names for collapsed-state icon layer.
    /// </summary>
    /// <param name="builder">Class builder.</param>
    protected virtual void BuildCollapsedToggleIconLayerClasses( ClassBuilder builder )
    {
        BuildToggleIconLayerClasses( builder, false );
    }

    /// <summary>
    /// Handler for @onkeydown event.
    /// </summary>
    /// <param name="eventArgs">Information about the keyboard down event.</param>
    /// <returns>Returns awaitable task</returns>
    protected Task KeyDownHandler( KeyboardEventArgs eventArgs )
    {
        if ( IsDisabled )
            return Task.CompletedTask;

        if ( ParentBarDropdown is not null && ( eventArgs.Key == "Enter" || eventArgs.Key == "NumpadEnter" ) )
        {
            lastKeyboardToggleTimestampUtc = DateTime.UtcNow;

            if ( !IsToggleClickTriggerEnabled )
                return Task.CompletedTask;

            if ( ParentBarDropdown.IsVisible )
                return ParentBarDropdown.Hide();

            return ParentBarDropdown.Toggle( ElementId );
        }

        if ( ParentBarDropdown is not null && ( eventArgs.Key == "Escape" || eventArgs.Key == "Esc" ) )
            return ParentBarDropdown.Hide( true );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Synchronizes dropdown visibility with the current route match status.
    /// </summary>
    /// <param name="active">True if current route matches this toggle link.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task HandleRouteMatchTriggerAsync( bool active )
    {
        if ( !HasNavigationTarget || ParentBarDropdown is null )
            return Task.CompletedTask;

        if ( active )
            return ParentBarDropdown.Show();

        return ParentBarDropdown.Hide();
    }

    /// <inheritdoc/>
    public Task<bool> IsSafeToClose( string elementId, CloseReason closeReason, bool isChildClicked )
    {
        return Task.FromResult( closeReason == CloseReason.EscapeClosing || ( ParentBarDropdown?.ShouldClose ?? true && ( elementId != ElementId && ParentBarDropdown?.SelectedBarDropdownElementId != ElementId && !isChildClicked ) ) );
    }

    /// <inheritdoc/>
    public Task Close( CloseReason closeReason )
    {
        if ( ParentBarDropdown is not null )
            return ParentBarDropdown.Hide();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the visibility styles and JS interop states.
    /// </summary>
    /// <param name="visible">True if component is visible.</param>
    protected virtual void HandleVisibilityStyles( bool visible )
    {
        if ( visible )
        {
            jsRegistered = true;

            ExecuteAfterRender( async () =>
            {
                await JSClosableModule.Register( dotNetObjectRef, ElementRef );
            } );
        }
        else
        {
            jsRegistered = false;

            ExecuteAfterRender( async () =>
            {
                await JSClosableModule.Unregister( this );
            } );
        }

        DirtyClasses();
        DirtyStyles();
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        ToggleContentClassBuilder?.Dirty();
        ToggleContentTextClassBuilder?.Dirty();
        ToggleContentIconClassBuilder?.Dirty();
        ToggleIconContainerClassBuilder?.Dirty();
        ExpandedToggleIconLayerClassBuilder?.Dirty();
        CollapsedToggleIconLayerClassBuilder?.Dirty();

        base.DirtyClasses();
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Returns true if this BarDropdown should be disabled.
    /// </summary>
    protected internal bool IsDisabled => paramDisabled.GetValueOrDefault( ParentBarItem?.Disabled ?? false );

    /// <summary>
    /// Should the toggle icon be drawn
    /// </summary>
    protected bool IsToggleIconVisible => ToggleIconVisible.GetValueOrDefault( Theme?.BarOptions?.DropdownOptions?.ToggleIconVisible ?? true );

    /// <summary>
    /// Gets the icon name for the expanded state of the dropdown.
    /// </summary>
    protected virtual IconName ExpandedToggleIconName => Theme?.BarOptions?.DropdownOptions?.ToggleExpandIconName ?? IconName.ChevronUp;

    /// <summary>
    /// Gets the icon name for the collapsed state of the dropdown.
    /// </summary>
    protected virtual IconName CollapsedToggleIconName => Theme?.BarOptions?.DropdownOptions?.ToggleCollapseIconName ?? IconName.ChevronDown;

    /// <summary>
    /// Gets the size of the toggle icon used in the dropdown options.
    /// </summary>
    protected virtual IconSize ToggleIconSize => Theme?.BarOptions?.DropdownOptions?.ToggleIconSIze ?? IconSize.ExtraSmall;

    /// <summary>
    /// Indicates whether the current instance has a navigation target.
    /// </summary>
    protected bool HasNavigationTarget => !string.IsNullOrEmpty( To );

    /// <summary>
    /// Gets a value indicating whether toggle-area click can trigger menu toggle.
    /// </summary>
    protected bool IsToggleClickTriggerEnabled => HasTrigger( BarDropdownToggleTrigger.ToggleClick );

    /// <summary>
    /// Gets a value indicating whether icon click can trigger menu toggle.
    /// </summary>
    protected bool IsIconClickTriggerEnabled => HasTrigger( BarDropdownToggleTrigger.IconClick );

    /// <summary>
    /// Gets a value indicating whether route match can trigger menu toggle state changes.
    /// </summary>
    protected bool IsRouteMatchTriggerEnabled => HasTrigger( BarDropdownToggleTrigger.RouteMatch );

    /// <summary>
    /// Gets the class names for toggle content wrapper.
    /// </summary>
    protected string ToggleContentClassNames => ToggleContentClassBuilder.Class;

    /// <summary>
    /// Gets the class names for toggle content text wrapper.
    /// </summary>
    protected string ToggleContentTextClassNames => ToggleContentTextClassBuilder.Class;

    /// <summary>
    /// Gets the class names for toggle content icon wrapper.
    /// </summary>
    protected string ToggleContentIconClassNames => ToggleContentIconClassBuilder.Class;

    /// <summary>
    /// Indicates whether toggle-area click should stop event propagation.
    /// </summary>
    protected bool ShouldStopToggleAreaPropagation => HasNavigationTarget && !IsToggleClickTriggerEnabled;

    /// <summary>
    /// Indicates whether icon click should stop event propagation.
    /// </summary>
    protected bool ShouldStopIconClickPropagation => IsIconClickTriggerEnabled;

    /// <summary>
    /// Indicates whether default navigation should be prevented on toggle-area click.
    /// </summary>
    protected virtual bool ShouldPreventDefaultOnToggleClick => HasNavigationTarget && IsToggleClickTriggerEnabled;

    /// <summary>
    /// Indicates whether default navigation should be prevented on icon click.
    /// </summary>
    protected bool ShouldPreventDefaultOnIconClick => HasNavigationTarget && IsIconClickTriggerEnabled;

    /// <summary>
    /// Gets the class names for toggle icon container.
    /// </summary>
    protected string ToggleIconContainerClassNames => ToggleIconContainerClassBuilder.Class;

    /// <summary>
    /// Gets the class names for the expanded-state icon layer.
    /// </summary>
    protected string ExpandedToggleIconLayerClassNames => ExpandedToggleIconLayerClassBuilder.Class;

    /// <summary>
    /// Gets the class names for the collapsed-state icon layer.
    /// </summary>
    protected string CollapsedToggleIconLayerClassNames => CollapsedToggleIconLayerClassBuilder.Class;

    /// <summary>
    /// Toggle content wrapper class builder.
    /// </summary>
    protected ClassBuilder ToggleContentClassBuilder { get; private set; }

    /// <summary>
    /// Toggle content text wrapper class builder.
    /// </summary>
    protected ClassBuilder ToggleContentTextClassBuilder { get; private set; }

    /// <summary>
    /// Toggle content icon wrapper class builder.
    /// </summary>
    protected ClassBuilder ToggleContentIconClassBuilder { get; private set; }

    /// <summary>
    /// Toggle icon container class builder.
    /// </summary>
    protected ClassBuilder ToggleIconContainerClassBuilder { get; private set; }

    /// <summary>
    /// Expanded-state toggle icon layer class builder.
    /// </summary>
    protected ClassBuilder ExpandedToggleIconLayerClassBuilder { get; private set; }

    /// <summary>
    /// Collapsed-state toggle icon layer class builder.
    /// </summary>
    protected ClassBuilder CollapsedToggleIconLayerClassBuilder { get; private set; }

    private BarDropdownToggleTrigger EffectiveTrigger
        => Trigger == BarDropdownToggleTrigger.Auto
            ? HasNavigationTarget
                ? BarDropdownToggleTrigger.IconClick
                : BarDropdownToggleTrigger.ToggleClick
            : Trigger;

    private bool HasTrigger( BarDropdownToggleTrigger trigger )
        => ( EffectiveTrigger & trigger ) == trigger;

    /// <summary>
    /// Gets or sets the <see cref="IJSClosableModule"/> instance.
    /// </summary>
    [Inject] public IJSClosableModule JSClosableModule { get; set; }

    /// <summary>
    /// Determines how much left padding will be applied to the dropdown toggle. (in rem unit)
    /// </summary>
    [Parameter]
    public double Indentation
    {
        get => indentation;
        set
        {
            if ( indentation == value )
                return;

            indentation = value;

            DirtyStyles();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the dropdown toggle icon is visible.
    /// </summary>
    /// <value>
    /// <c>true</c> if [show toggle]; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>Default: True</remarks>
    [Parameter]
    public bool? ToggleIconVisible
    {
        get => toggleIconVisible;
        set
        {
            if ( toggleIconVisible == value )
                return;

            toggleIconVisible = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines which interactions can trigger the dropdown toggle.
    /// </summary>
    [Parameter] public BarDropdownToggleTrigger Trigger { get; set; } = BarDropdownToggleTrigger.Auto;

    /// <summary>
    /// Gets or sets the parent dropdown state object.
    /// </summary>
    [CascadingParameter]
    public BarDropdownState ParentBarDropdownState
    {
        get;
        set
        {
            if ( field == value )
                return;

            field = value;

            if ( field.Visible && !( field.Mode == BarMode.VerticalInline && field.BarVisible ) )
            {
                HandleVisibilityStyles( true );
            }
            else
            {
                HandleVisibilityStyles( false );
            }
        }
    }

    /// <summary>
    /// Gets or sets the reference to the parent dropdown.
    /// </summary>
    [CascadingParameter] protected BarDropdown ParentBarDropdown { get; set; }

    /// <summary>
    /// Cascaded <see cref="BarItem"/> component in which this <see cref="BarDropdownToggle"/> is placed.
    /// </summary>
    [CascadingParameter] protected BarItem ParentBarItem { get; set; }

    /// <summary>
    /// Gets or sets the parent bar-item state object.
    /// </summary>
    [CascadingParameter]
    public BarItemState ParentBarItemState
    {
        get;
        set
        {
            if ( field == value )
                return;

            field = value;

            DirtyClasses();
            DirtyStyles();
        }
    }

    /// <summary>
    /// The applied theme.
    /// </summary>
    [CascadingParameter]
    protected Theme Theme
    {
        get => theme;
        set
        {
            if ( theme == value )
                return;

            theme = value;

            DirtyClasses();
        }
    }

    #endregion
}