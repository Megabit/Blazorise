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
    protected override void OnInitialized()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        if ( Source is not null && To is null )
            To = Source;
#pragma warning restore CS0618 // Type or member is obsolete

#pragma warning disable CS0618 // Type or member is obsolete
        if ( Alt is not null && Title is null )
            Title = Alt;
#pragma warning restore CS0618 // Type or member is obsolete

        base.OnInitialized();
    }

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