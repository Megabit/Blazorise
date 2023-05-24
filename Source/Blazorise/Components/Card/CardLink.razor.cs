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

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Link url.
    /// </summary>
    [Obsolete( "CardLink: Source parameter is deprecated, please use the To parameter instead." )]
    [Parameter] public string Source { get; set; }

    /// <summary>
    /// Alternative link text.
    /// </summary>
    [Obsolete( "CardLink: Alt parameter is deprecated, please use the Title parameter instead." )]
    [Parameter] public string Alt { get; set; }

    #endregion
}