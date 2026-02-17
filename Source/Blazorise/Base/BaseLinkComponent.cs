#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Base component that renders an anchor tag, automatically toggling its 'active'
/// state based on whether its 'href' matches the current URI.
/// </summary>
/// <remarks>
/// Any link styling must be applied by the implementation classes. 
/// </remarks>
public abstract class BaseLinkComponent : BaseComponent, IDisposable
{
    #region Members

    private bool active;

    private string anchorTarget;

    /// <summary>
    /// Captured Disabled parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramDisabled;

    private ComponentParameterInfo<string> paramTo;

    private ComponentParameterInfo<Match> paramMatch;

    private ComponentParameterInfo<Func<string, bool>> paramCustomMatch;

    private ComponentParameterInfo<bool> paramUnstyled;

    private ComponentParameterInfo<bool> paramStretched;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += OnLocationChanged;

        base.OnInitialized();
    }

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var previousTo = To;
        var previousParamDisabled = paramDisabled;

        parameters.TryGetParameter( To, out paramTo );
        parameters.TryGetParameter( Match, out paramMatch );
        parameters.TryGetParameter( CustomMatch, out paramCustomMatch );
        parameters.TryGetParameter( Unstyled, out paramUnstyled );
        parameters.TryGetParameter( Stretched, out paramStretched );
        parameters.TryGetParameter( Disabled, out paramDisabled );

        await base.SetParametersAsync( parameters );

        UpdateState( previousTo, previousParamDisabled );
    }

    private void UpdateState( string previousTo, ComponentParameterInfo<bool> previousParamDisabled )
    {
        var resolvedTo = To;

        // in case the user has specified href instead of To we need to use that instead
        if ( Attributes is not null && Attributes.TryGetValue( "href", out var href ) )
            resolvedTo = $"{href}";

        if ( !string.Equals( To, resolvedTo, StringComparison.Ordinal ) )
            To = resolvedTo;

        PreventDefault = false;
        anchorTarget = null;

        if ( resolvedTo is not null && resolvedTo.StartsWith( "#", StringComparison.Ordinal ) )
        {
            // If the href contains an anchor link we don't want the default click action to occur, but
            // rather take care of the click in our own method.
            anchorTarget = resolvedTo[1..];
            PreventDefault = true;
        }

        var shouldBeActiveNow = NavigationManager.IsMatch( resolvedTo, Match, CustomMatch );

        var classDependentParameterChanged =
            paramTo.Changed
            || paramMatch.Changed
            || paramCustomMatch.Changed
            || paramUnstyled.Changed
            || paramStretched.Changed
            || paramDisabled.Changed
            || !string.Equals( previousTo, resolvedTo, StringComparison.Ordinal )
            || previousParamDisabled.Defined != paramDisabled.Defined;

        if ( shouldBeActiveNow != active || classDependentParameterChanged )
        {
            active = shouldBeActiveNow;

            DirtyClasses();
        }
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            // To avoid leaking memory, it's important to detach any event handlers in Dispose()
            NavigationManager?.LocationChanged -= OnLocationChanged;
        }

        base.Dispose( disposing );
    }

    private void OnLocationChanged( object sender, LocationChangedEventArgs args )
    {
        // We could just re-render always, but for this component we know the
        // only relevant state change is to the active property.
        var shouldBeActiveNow = NavigationManager.IsMatch( To, Match, CustomMatch );

        if ( shouldBeActiveNow != active )
        {
            active = shouldBeActiveNow;

            DirtyClasses();

            InvokeAsync( StateHasChanged );
        }
    }

    /// <summary>
    /// Handles the link onclick event.
    /// </summary>
    /// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task OnClickHandler( MouseEventArgs eventArgs )
    {
        if ( !string.IsNullOrEmpty( anchorTarget ) )
        {
            await JSUtilitiesModule.ScrollAnchorIntoView( anchorTarget );
        }

        await Clicked.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Resolves the <see cref="To"/> value for the href attribute.
    /// </summary>
    /// <returns>Href value.</returns>
    protected string GetTo() => Disabled ? null : To;

    /// <summary>
    /// Gets the rel attribute value.
    /// </summary>
    /// <returns>
    /// Returns "noopener noreferrer" if the <see cref="Target"/> is set to <see cref="Target.Blank"/>.
    /// </returns>
    protected string GetRel() => Target == Target.Blank ? "noopener noreferrer" : null;

    #endregion

    #region Properties

    /// <summary>
    /// Indicates if the default behavior will be prevented.
    /// </summary>
    protected bool PreventDefault { get; private set; }

    /// <summary>
    /// Gets the link target name.
    /// </summary>
    protected string TargetName => Target.ToTargetString();

    /// <summary>
    /// True, if the <see cref="To"/> parameter is the same as the current route uri.
    /// </summary>
    protected bool Active => active;

    /// <summary>
    /// Gets the string representation of the <see cref="Disabled"/> value for the aria-disabled attribute.
    /// </summary>
    protected string AriaDisabledString => Disabled ? "true" : null;

    /// <summary>
    /// Gets or sets the <see cref="IJSUtilitiesModule"/> instance.
    /// </summary>
    [Inject] public IJSUtilitiesModule JSUtilitiesModule { get; set; }

    /// <summary>
    /// Gets or sets the navigation manager instance.
    /// </summary>
    [Inject] private NavigationManager NavigationManager { get; set; }

    /// <summary>
    /// Denotes the target route of the link.
    /// </summary>
    [Parameter] public string To { get; set; }

    /// <summary>
    /// URL matching behavior for a link.
    /// </summary>
    [Parameter] public Match Match { get; set; }

    /// <summary>
    /// A callback function that is used to compare current uri with the user defined uri. If defined, the <see cref="Match"/> parameter will be ignored.
    /// </summary>
    [Parameter] public Func<string, bool> CustomMatch { get; set; }

    /// <summary>
    /// The target attribute specifies where to open the linked document.
    /// </summary>
    [Parameter] public Target Target { get; set; } = Target.Default;

    /// <summary>
    /// Specify extra information about the element.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Removes default color styles from the link.
    /// </summary>
    [Parameter] public bool Unstyled { get; set; }

    /// <summary>
    /// Makes any HTML element or component clickable by “stretching” a nested link.
    /// </summary>
    [Parameter] public bool Stretched { get; set; }

    /// <summary>
    /// Makes the link look inactive by adding the disabled boolean attribute.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Occurs when the link is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Link"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}