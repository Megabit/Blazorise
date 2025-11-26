#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Wrapper for a card links.
/// </summary>
public partial class CardLink : BaseLinkComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.CardLink() );
        builder.Append( ClassProvider.CardLinkActive( Active ) );
        builder.Append( ClassProvider.CardLinkUnstyled( Unstyled ) );

        base.BuildClasses( builder );
    }

    #endregion
}