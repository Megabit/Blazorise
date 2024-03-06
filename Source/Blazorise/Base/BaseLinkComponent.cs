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

    private string absoluteUri;

    private bool active;

    private string anchorTarget;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += OnLocationChanged;

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        PreventDefault = false;

        // in case the user has specified href instead of To we need to use that instead
        if ( Attributes is not null && Attributes.TryGetValue( "href", out var href ) )
            To = $"{href}";

        if ( To is not null && To.StartsWith( "#" ) )
        {
            // If the href contains an anchor link we don't want the default click action to occur, but
            // rather take care of the click in our own method.
            anchorTarget = To[1..];
            PreventDefault = true;
        }

        absoluteUri = GetAbsoluteUri( To );

        var shouldBeActiveNow = ShouldMatch( NavigationManager.Uri );

        if ( shouldBeActiveNow != active )
        {
            active = shouldBeActiveNow;

            DirtyClasses();
        }

        base.OnParametersSet();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            // To avoid leaking memory, it's important to detach any event handlers in Dispose()
            if ( NavigationManager is not null )
            {
                NavigationManager.LocationChanged -= OnLocationChanged;
            }
        }

        base.Dispose( disposing );
    }

    private void OnLocationChanged( object sender, LocationChangedEventArgs args )
    {
        // We could just re-render always, but for this component we know the
        // only relevant state change is to the active property.
        var shouldBeActiveNow = ShouldMatch( args.Location );

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

    private bool ShouldMatch( string currentUriAbsolute )
    {
        if ( EqualsHrefExactlyOrIfTrailingSlashAdded( currentUriAbsolute ) )
        {
            return true;
        }

        if ( Match == Match.Prefix && IsStrictlyPrefixWithSeparator( currentUriAbsolute, absoluteUri ) )
        {
            return true;
        }

        if ( Match == Match.Custom && CustomMatch is not null && CustomMatch.Invoke( currentUriAbsolute ) )
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Converts a relative URI into an absolute one (by resolving it relative to the
    /// current absolute URI).
    /// </summary>
    /// <param name="relativeUri">The relative URI.</param>
    /// <returns>The absolute URI.</returns>
    private string GetAbsoluteUri( string relativeUri )
    {
        try
        {
            if ( relativeUri is null )
                return string.Empty;

            if ( relativeUri.StartsWith( "mailto:", StringComparison.OrdinalIgnoreCase ) )
                return relativeUri;

            return NavigationManager.ToAbsoluteUri( To ).AbsoluteUri;
        }
        catch
        {
            return relativeUri;
        }
    }

    private bool EqualsHrefExactlyOrIfTrailingSlashAdded( string currentUriAbsolute )
    {
        if ( string.Equals( currentUriAbsolute, absoluteUri, StringComparison.Ordinal ) )
        {
            return true;
        }

        if ( currentUriAbsolute.Length == absoluteUri.Length - 1 )
        {
            // Special case: highlight links to http://host/path/ even if you're
            // at http://host/path (with no trailing slash)
            //
            // This is because the router accepts an absolute URI value of "same
            // as base URI but without trailing slash" as equivalent to "base URI",
            // which in turn is because it's common for servers to return the same page
            // for http://host/vdir as they do for host://host/vdir/ as it's no
            // good to display a blank page in that case.
            if ( absoluteUri[^1] == '/'
                 && absoluteUri.StartsWith( currentUriAbsolute, StringComparison.Ordinal ) )
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsStrictlyPrefixWithSeparator( string value, string prefix )
    {
        var prefixLength = prefix.Length;
        if ( value.Length > prefixLength )
        {
            return value.StartsWith( prefix, StringComparison.Ordinal )
                   && (
                       // Only match when there's a separator character either at the end of the
                       // prefix or right after it.
                       // Example: "/abc" is treated as a prefix of "/abc/def" but not "/abcdef"
                       // Example: "/abc/" is treated as a prefix of "/abc/def" but not "/abcdef"
                       prefixLength == 0
                       || !char.IsLetterOrDigit( prefix[prefixLength - 1] )
                       || !char.IsLetterOrDigit( value[prefixLength] )
                   );
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Resolves the <see cref="To"/> value for the href attribute.
    /// </summary>
    /// <returns>Href value.</returns>
    protected string GetTo() => Disabled ? null : To;

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
    /// A callback function that is used to compare current uri with the user defined uri. Must enable <see cref="Match.Custom"/> to be used.
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
