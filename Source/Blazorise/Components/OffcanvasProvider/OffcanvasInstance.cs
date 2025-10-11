using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Components.OffcanvasProvider;
/// <summary>
/// Generates a new instance of a OffCanvas
/// </summary>
public class OffcanvasInstance
{
    #region Constructors

    /// <summary>
    /// A default <see cref="OffcanvasInstance"/> constructor.
    /// </summary>
    /// <param name="offcanvasProvider">Parent offCanvas provider.</param>
    /// <param name="id">An id of the offCanvas instance.</param>
    /// <param name="title">Title of the offCanvas.</param>
    /// <param name="childContent">OffCanvas child content.</param>
    /// <param name="offcanvasInstanceOptions">OffCanvas options.</param>
    public OffcanvasInstance( OffcanvasProvider offcanvasProvider, string id, string title, RenderFragment childContent, OffcanvasInstanceOptions offcanvasInstanceOptions )
    {
        OffCanvasId = string.IsNullOrWhiteSpace( offcanvasInstanceOptions?.ElementId )
            ? id
            : offcanvasInstanceOptions.ElementId;
        OffcanvasProvider = offcanvasProvider;
        Title = title;
        ChildContent = childContent;
        OffcanvasInstanceOptions = offcanvasInstanceOptions;
        Visible = false;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Tracks the OffCanvas reference.
    /// </summary>
    public Offcanvas OffCanvasRef { get; set; }

    /// <summary>
    /// Tracks the OffCanvas id.
    /// </summary>
    public string OffCanvasId { get; set; }

    /// <summary>
    /// Control's the OffCanvas visibility.
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// The OffCanvas Provider
    /// </summary>
    public OffcanvasProvider OffcanvasProvider { get; private set; }

    /// <summary>
    /// Child content to be rendered.
    /// </summary>
    public RenderFragment ChildContent { get; private set; }

    /// <summary>
    /// OffCanvas's Header Title.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Sets the options for OffCanvas Provider.
    /// </summary>
    public OffcanvasInstanceOptions OffcanvasInstanceOptions { get; private set; }

    /// <summary>
    /// Determines if the OffCanvasInstance should be kept in memory after it has been closed.
    /// Defaults to false.
    /// </summary>
    public bool Stateful => OffcanvasInstanceOptions?.Stateful ?? OffcanvasProvider.Stateful;

    public bool ShowCloseButton => OffcanvasInstanceOptions?.ShowCloseButton ?? OffcanvasProvider.ShowCloseButton;
    public EventCallback CloseButtonClicked => OffcanvasInstanceOptions?.CloseButtonClicked ?? OffcanvasProvider.CloseButtonClicked;

    public bool UseOffCanvasStructure => OffcanvasProvider.UseOffCanvasStructure;

    #region OffCanvas

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
    /// Occurs before the offCanvas is opened.
    /// </summary>
    public Func<OffcanvasOpeningEventArgs, Task> Opening => OffcanvasInstanceOptions?.Opening ?? OffcanvasProvider.Opening;

    /// <summary>
    /// Occurs before the offCanvas is closed.
    /// </summary>
    public Func<OffcanvasClosingEventArgs, Task> Closing => OffcanvasInstanceOptions?.Closing ?? OffcanvasProvider.Closing;

    /// <summary>
    /// Occurs after the offCanvas has opened.
    /// </summary>
    public EventCallback Opened => OffcanvasInstanceOptions?.Opened ?? OffcanvasProvider.Opened;

    /// <summary>
    /// Occurs after the offCanvas has closed.
    /// </summary>
    public EventCallback Closed => OffcanvasInstanceOptions?.Closed ?? OffcanvasProvider.Closed;

    /// <summary>
    /// Specifies the backdrop needs to be rendered for this <see cref="OffCanvas"/>.
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

    public Placement Placement => OffcanvasInstanceOptions?.Placement ?? OffcanvasProvider.Placement;

    #endregion

    #endregion
}