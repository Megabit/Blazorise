#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Generates a new instance of an <see cref="Offcanvas"/>.
/// </summary>
public class OffcanvasInstance
{
    #region Constructors

    /// <summary>
    /// A default <see cref="OffcanvasInstance"/> constructor.
    /// </summary>
    /// <param name="offcanvasProvider">Parent offcanvas provider.</param>
    /// <param name="id">An id of the offcanvas instance.</param>
    /// <param name="title">Title of the offcanvas.</param>
    /// <param name="childContent">Offcanvas content.</param>
    /// <param name="offcanvasInstanceOptions">Offcanvas options.</param>
    public OffcanvasInstance( OffcanvasProvider offcanvasProvider, string id, string title, RenderFragment childContent, OffcanvasInstanceOptions offcanvasInstanceOptions )
    {
        OffcanvasId = string.IsNullOrWhiteSpace( offcanvasInstanceOptions?.ElementId )
            ? id
            : offcanvasInstanceOptions.ElementId;
        OffcanvasProvider = offcanvasProvider;
        Title = title;
        ChildContent = childContent;
        OffcanvasInstanceOptions = offcanvasInstanceOptions;
        Visible = true;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Tracks the <see cref="Offcanvas"/> reference.
    /// </summary>
    public Offcanvas OffcanvasRef { get; set; }

    /// <summary>
    /// Tracks the offcanvas id.
    /// </summary>
    public string OffcanvasId { get; set; }

    /// <summary>
    /// Controls the offcanvas visibility.
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// The offcanvas provider.
    /// </summary>
    public OffcanvasProvider OffcanvasProvider { get; }

    /// <summary>
    /// Child content to be rendered.
    /// </summary>
    public RenderFragment ChildContent { get; }

    /// <summary>
    /// Offcanvas header title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Sets the options for Offcanvas provider.
    /// </summary>
    public OffcanvasInstanceOptions OffcanvasInstanceOptions { get; }

    /// <summary>
    /// Uses the offcanvas standard structure, by setting this to true you are only in charge of providing the custom content.
    /// Defaults to true.
    /// </summary>
    public bool UseOffcanvasStructure => OffcanvasInstanceOptions?.UseOffcanvasStructure ?? OffcanvasProvider.UseOffcanvasStructure;

    /// <summary>
    /// Determines if the <see cref="OffcanvasInstance"/> should be kept in memory after it has been closed.
    /// Defaults to false.
    /// </summary>
    public bool Stateful => OffcanvasInstanceOptions?.Stateful ?? OffcanvasProvider.Stateful;

    /// <summary>
    /// Shows a close button in the offcanvas header when using the provider structure.
    /// </summary>
    public bool ShowCloseButton => OffcanvasInstanceOptions?.ShowCloseButton ?? OffcanvasProvider.ShowCloseButton;

    /// <summary>
    /// Callback executed when the close button is clicked.
    /// </summary>
    public EventCallback CloseButtonClicked => OffcanvasInstanceOptions?.CloseButtonClicked ?? OffcanvasProvider.CloseButtonClicked;

    #region Offcanvas

    /// <inheritdoc/>
    public Dictionary<string, object> Attributes => OffcanvasInstanceOptions?.Attributes ?? OffcanvasProvider.Attributes;

    /// <inheritdoc/>
    public Shadow Shadow => OffcanvasInstanceOptions?.Shadow ?? OffcanvasProvider.Shadow;

    /// <inheritdoc/>
    public Background Background => OffcanvasInstanceOptions?.Background ?? OffcanvasProvider.Background;

    /// <inheritdoc/>
    public VerticalAlignment VerticalAlignment => OffcanvasInstanceOptions?.VerticalAlignment ?? OffcanvasProvider.VerticalAlignment;

    /// <inheritdoc/>
    public TextOverflow TextOverflow => OffcanvasInstanceOptions?.TextOverflow ?? OffcanvasProvider.TextOverflow;

    /// <inheritdoc/>
    public TextWeight TextWeight => OffcanvasInstanceOptions?.TextWeight ?? OffcanvasProvider.TextWeight;

    /// <inheritdoc/>
    public TextTransform TextTransform => OffcanvasInstanceOptions?.TextTransform ?? OffcanvasProvider.TextTransform;

    /// <inheritdoc/>
    public TextAlignment TextAlignment => OffcanvasInstanceOptions?.TextAlignment ?? OffcanvasProvider.TextAlignment;

    /// <inheritdoc/>
    public TextColor TextColor => OffcanvasInstanceOptions?.TextColor ?? OffcanvasProvider.TextColor;

    /// <inheritdoc/>
    public CharacterCasing Casing => OffcanvasInstanceOptions?.Casing ?? OffcanvasProvider.Casing;

    /// <inheritdoc/>
    public IFluentOverflow Overflow => OffcanvasInstanceOptions?.Overflow ?? OffcanvasProvider.Overflow;

    /// <inheritdoc/>
    public IFluentPosition Position => OffcanvasInstanceOptions?.Position ?? OffcanvasProvider.Position;

    /// <inheritdoc/>
    public IFluentFlex Flex => OffcanvasInstanceOptions?.Flex ?? OffcanvasProvider.Flex;

    /// <inheritdoc/>
    public IFluentBorder Border => OffcanvasInstanceOptions?.Border ?? OffcanvasProvider.Border;

    /// <inheritdoc/>
    public IFluentDisplay Display => OffcanvasInstanceOptions?.Display ?? OffcanvasProvider.Display;

    /// <inheritdoc/>
    public IFluentSpacing Padding => OffcanvasInstanceOptions?.Padding ?? OffcanvasProvider.Padding;

    /// <inheritdoc/>
    public IFluentSpacing Margin => OffcanvasInstanceOptions?.Margin ?? OffcanvasProvider.Margin;

    /// <inheritdoc/>
    public IFluentSizing Height => OffcanvasInstanceOptions?.Height ?? OffcanvasProvider.Height;

    /// <inheritdoc/>
    public IFluentSizing Width => OffcanvasInstanceOptions?.Width ?? OffcanvasProvider.Width;

    /// <inheritdoc/>
    public Visibility Visibility => OffcanvasInstanceOptions?.Visibility ?? OffcanvasProvider.Visibility;

    /// <inheritdoc/>
    public bool Clearfix => OffcanvasInstanceOptions?.Clearfix ?? OffcanvasProvider.Clearfix;

    /// <inheritdoc/>
    public Float Float => OffcanvasInstanceOptions?.Float ?? OffcanvasProvider.Float;

    /// <inheritdoc/>
    public string Style => OffcanvasInstanceOptions?.Style ?? OffcanvasProvider.Style;

    /// <inheritdoc/>
    public string Class => OffcanvasInstanceOptions?.Class ?? OffcanvasProvider.Class;

    /// <summary>
    /// Occurs before the offcanvas is opened.
    /// </summary>
    public Func<OffcanvasOpeningEventArgs, Task> Opening => OffcanvasInstanceOptions?.Opening ?? OffcanvasProvider.Opening;

    /// <summary>
    /// Occurs before the offcanvas is closed.
    /// </summary>
    public Func<OffcanvasClosingEventArgs, Task> Closing => OffcanvasInstanceOptions?.Closing ?? OffcanvasProvider.Closing;

    /// <summary>
    /// Occurs after the offcanvas has opened.
    /// </summary>
    public EventCallback Opened => OffcanvasInstanceOptions?.Opened ?? OffcanvasProvider.Opened;

    /// <summary>
    /// Occurs after the offcanvas has closed.
    /// </summary>
    public EventCallback Closed => OffcanvasInstanceOptions?.Closed ?? OffcanvasProvider.Closed;

    /// <summary>
    /// Specifies the backdrop needs to be rendered for this <see cref="Offcanvas"/>.
    /// </summary>
    public bool ShowBackdrop => OffcanvasInstanceOptions?.ShowBackdrop ?? OffcanvasProvider.ShowBackdrop;

    /// <summary>
    /// Gets or sets whether the component has any animations.
    /// </summary>
    public bool Animated => OffcanvasInstanceOptions?.Animated ?? OffcanvasProvider.Animated;

    /// <summary>
    /// Gets or sets the animation duration.
    /// </summary>
    public int AnimationDuration => OffcanvasInstanceOptions?.AnimationDuration ?? OffcanvasProvider.AnimationDuration;

    /// <summary>
    /// Specifies the position of the offcanvas.
    /// </summary>
    public Placement Placement => OffcanvasInstanceOptions?.Placement ?? OffcanvasProvider.Placement;

    #endregion

    #endregion
}