#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Lightweight, flexible component for showcasing hero unit style content.
/// </summary>
public partial class Jumbotron : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Jumbotron() );
        builder.Append( ClassProvider.JumbotronBackground( Background ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the content rendered inside the jumbotron.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
