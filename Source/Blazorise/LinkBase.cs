#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Routing;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A component that renders an anchor tag, automatically toggling its 'active'
    /// class based on whether its 'href' matches the current URI.
    /// </summary>
    /// <remarks>
    /// This is a copy of Blazor implementation but without adding a fixed class for bootstrap.
    /// </remarks>
    public class LinkBase : IComponent, IDisposable
    {
        #region Members

        private const string defaultActiveClass = "active";

        private RenderHandle renderHandle;

        private RenderFragment childContent;

        private string tagName;

        private string className;

        private string hrefAbsolute;

        private bool isActive;

        private IDictionary<string, object> attributes;

        #endregion

        #region Events

        private void OnLocationChanged( object sender, LocationChangedEventArgs args )
        {
            // We could just re-render always, but for this component we know the
            // only relevant state change is to the _isActive property.
            var shouldBeActiveNow = ShouldMatch( args.Location );

            if ( shouldBeActiveNow != isActive )
            {
                isActive = shouldBeActiveNow;
                renderHandle.Render( Render );
            }
        }

        #endregion

        #region Methods

        public void Configure( RenderHandle renderHandle )
        {
            this.renderHandle = renderHandle;

            UriHelper.OnLocationChanged += OnLocationChanged;
        }

        public void Dispose()
        {
            // To avoid leaking memory, it's important to detach any event handlers in Dispose()
            UriHelper.OnLocationChanged -= OnLocationChanged;
        }

        public Task SetParametersAsync( ParameterCollection parameters )
        {
            attributes = parameters.ToDictionary() as Dictionary<string, object>;

            // Capture the parameters we want to do special things with, plus all as a dictionary
            childContent = GetAndRemove<RenderFragment>( attributes, RenderTreeBuilder.ChildContent );
            tagName = GetAndRemove<string>( attributes, "TagName" );
            className = GetAndRemove<string>( attributes, "class" );
            Match = GetAndRemove<Match>( attributes, "Match" );

            //parameters.TryGetValue( RenderTreeBuilder.ChildContent, out childContent );
            //parameters.TryGetValue( "TagName", out tagName );
            //parameters.TryGetValue( "class", out className );
            parameters.TryGetValue( "href", out string href );

            //Match = parameters.GetValueOrDefault( nameof( Match ), Match.Prefix );


            // Update computed state and render
            hrefAbsolute = href == null ? string.Empty : UriHelper.ToAbsoluteUri( href ).AbsoluteUri;
            isActive = ShouldMatch( UriHelper.GetAbsoluteUri() );

            renderHandle.Render( Render );

            return Task.CompletedTask;
        }

        private static T GetAndRemove<T>( IDictionary<string, object> values, string key )
        {
            if ( values.TryGetValue( key, out var value ) )
            {
                values.Remove( key );
                return (T)value;
            }
            else
            {
                return default;
            }
        }

        private void Render( RenderTreeBuilder builder )
        {
            builder.OpenElement( 0, tagName );

            // Set class attribute
            builder.AddAttribute( 0, "class", CombineWithSpace( className, isActive ? ClassProvider.Active() : null ) );

            // Pass through all other attributes unchanged
            foreach ( var att in attributes )
                builder.AddAttribute( 0, att.Key, att.Value );

            //// Pass through all other attributes unchanged
            //foreach ( var att in allAttributes.Where( att => att.Key != "TagName" && att.Key != "class" && att.Key != nameof( RenderTreeBuilder.ChildContent ) ) )
            //{
            //    builder.AddAttribute( 0, att.Key, att.Value );
            //}

            // Pass through any child content unchanged
            builder.AddContent( 1, childContent );

            builder.CloseElement();
        }

        private bool ShouldMatch( string currentUriAbsolute )
        {
            if ( EqualsHrefExactlyOrIfTrailingSlashAdded( currentUriAbsolute ) )
            {
                return true;
            }

            if ( Match == Match.Prefix && IsStrictlyPrefixWithSeparator( currentUriAbsolute, hrefAbsolute ) )
            {
                return true;
            }

            return false;
        }

        private bool EqualsHrefExactlyOrIfTrailingSlashAdded( string currentUriAbsolute )
        {
            if ( string.Equals( currentUriAbsolute, hrefAbsolute, StringComparison.Ordinal ) )
            {
                return true;
            }

            if ( currentUriAbsolute.Length == hrefAbsolute.Length - 1 )
            {
                // Special case: highlight links to http://host/path/ even if you're
                // at http://host/path (with no trailing slash)
                //
                // This is because the router accepts an absolute URI value of "same
                // as base URI but without trailing slash" as equivalent to "base URI",
                // which in turn is because it's common for servers to return the same page
                // for http://host/vdir as they do for host://host/vdir/ as it's no
                // good to display a blank page in that case.
                if ( hrefAbsolute[hrefAbsolute.Length - 1] == '/'
                    && hrefAbsolute.StartsWith( currentUriAbsolute, StringComparison.Ordinal ) )
                {
                    return true;
                }
            }

            return false;
        }

        private string CombineWithSpace( string str1, string str2 ) => str1 == null ? str2 : ( str2 == null ? str1 : $"{str1} {str2}" );

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
        /// Gets or sets a value representing the URL matching behavior.
        /// </summary>
        [Parameter] Match Match { get; set; }

        [Inject] private IUriHelper UriHelper { get; set; }

        [Inject] private IClassProvider ClassProvider { get; set; }

        #endregion
    }
}
