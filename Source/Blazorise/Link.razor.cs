#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Routing;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A component that renders an anchor tag, automatically toggling its 'active'
    /// class based on whether its 'href' matches the current URI.
    /// </summary>
    public partial class Link : BaseComponent
    {
        #region Members

        private string absoluteUri;

        private bool active;

        private string anchorTarget;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Active(), active );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            NavigationManager.LocationChanged += OnLocationChanged;

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                // To avoid leaking memory, it's important to detach any event handlers in Dispose()
                NavigationManager.LocationChanged -= OnLocationChanged;
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

                StateHasChanged();
            }
        }

        protected override void OnParametersSet()
        {
            PreventDefault = false;

            // in case the user has specified href instead of To we need to use that instead
            if ( Attributes?.ContainsKey( "href" ) == true )
                To = $"{Attributes["href"]}";

            if ( To != null && To.StartsWith( "#" ) )
            {
                // If the href contains an anchor link we don't want the default click action to occur, but
                // rather take care of the click in our own method.
                anchorTarget = To[1..];
                PreventDefault = true;
            }

            absoluteUri = To == null ? string.Empty : NavigationManager.ToAbsoluteUri( To ).AbsoluteUri;

            var shouldBeActiveNow = ShouldMatch( NavigationManager.Uri );

            if ( shouldBeActiveNow != active )
            {
                active = shouldBeActiveNow;

                DirtyClasses();
            }

            base.OnParametersSet();
        }

        protected async Task OnClickHandler()
        {
            if ( !string.IsNullOrEmpty( anchorTarget ) )
            {
                await JSRunner.ScrollIntoView( anchorTarget );
            }

            await Clicked.InvokeAsync( null );
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

            return false;
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

        #endregion

        #region Properties        

        /// <summary>
        /// Indicates if the default behavior will be prevented.
        /// </summary>
        protected bool PreventDefault { get; private set; }

        /// <summary>
        /// Gets the link target name.
        /// </summary>
        protected string TargetName => LinkHelpers.TargetName( Target );

        /// <summary>
        /// Denotes the target route of the link.
        /// </summary>
        [Parameter] public string To { get; set; }

        /// <summary>
        /// URL matching behavior for a link.
        /// </summary>
        [Parameter] public Match Match { get; set; }

        /// <summary>
        /// The target attribute specifies where to open the linked document.
        /// </summary>
        [Parameter] public Target Target { get; set; } = Target.None;

        /// <summary>
        /// Specify extra information about the element.
        /// </summary>
        [Parameter] public string Title { get; set; }

        /// <summary>
        /// Occurs when the link is clicked.
        /// </summary>
        [Parameter] public EventCallback Clicked { get; set; }

        [Inject] private NavigationManager NavigationManager { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Link"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
