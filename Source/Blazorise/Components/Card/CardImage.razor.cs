#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A fullwidth container for a responsive image.
/// </summary>
public partial class CardImage : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.CardImage() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// The onerror attribute value that will be used to set the fallback image source if the main image fails to load.
    /// </summary>
    protected string OnError => !string.IsNullOrEmpty( FallbackSource ) ? $"this.src='{FallbackSource}'" : null;

    /// <summary>
    /// Image url.
    /// </summary>
    [Parameter] public string Source { get; set; }

    /// <summary>
    /// Alternate text for the image.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Alternate text for the image.
    /// </summary>
    /// <remarks>
    /// This parameter is retained for source compatibility. Use <see cref="Text"/> instead.
    /// </remarks>
    [Obsolete( "Use Text instead." )]
    [Parameter]
    public string Alt
    {
        get => Text;
        set => Text = value;
    }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="CardImage"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// The fallback image that will be displayed if image loading fails.
    /// </summary>
    [Parameter] public string FallbackSource { get; set; }

    #endregion
}