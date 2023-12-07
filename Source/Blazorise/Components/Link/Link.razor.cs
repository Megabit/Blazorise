#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// A component that renders an anchor tag, automatically toggling its 'active'
/// class based on whether its 'href' matches the current URI.
/// </summary>
public partial class Link : BaseLinkComponent, IDisposable
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Link() );
        builder.Append( ClassProvider.LinkActive( Active ) );
        builder.Append( ClassProvider.LinkUnstyled( Unstyled ) );
        builder.Append( ClassProvider.LinkStretched( Stretched ) );

        base.BuildClasses( builder );
    }

    #endregion
}