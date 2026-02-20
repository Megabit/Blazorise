#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Generates a new instance of a Modal
/// </summary>
public class ModalInstance
{
    #region Constructors

    /// <summary>
    /// A default <see cref="ModalInstance"/> constructor.
    /// </summary>
    /// <param name="modalProvider">Parent modal provider.</param>
    /// <param name="id">An id of the modal instance.</param>
    /// <param name="title">Title of the modal.</param>
    /// <param name="childContent">Modal content.</param>
    /// <param name="modalInstanceOptions">Modal options.</param>
    public ModalInstance( ModalProvider modalProvider, string id, string title, RenderFragment childContent, ModalInstanceOptions modalInstanceOptions )
    {
        ModalId = string.IsNullOrWhiteSpace( modalInstanceOptions?.ElementId )
            ? id
            : modalInstanceOptions.ElementId;
        ModalProvider = modalProvider;
        Title = title;
        ChildContent = childContent;
        ModalInstanceOptions = modalInstanceOptions;
        Visible = true;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Tracks the Modal reference.
    /// </summary>
    public Modal ModalRef { get; set; }

    /// <summary>
    /// Tracks the Modal id.
    /// </summary>
    public string ModalId { get; set; }

    /// <summary>
    /// Control's the Modal visibility.
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// The Modal Provider
    /// </summary>
    public ModalProvider ModalProvider { get; private set; }

    /// <summary>
    /// Child content to be rendered.
    /// </summary>
    public RenderFragment ChildContent { get; private set; }

    /// <summary>
    /// Modal's Header Title.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Sets the options for Modal Provider.
    /// </summary>
    public ModalInstanceOptions ModalInstanceOptions { get; private set; }

    /// <summary>
    /// Uses the modal standard structure, by setting this to true you are only in charge of providing the custom content.
    /// Defaults to true.
    /// </summary>
    public bool UseModalStructure => ModalInstanceOptions?.UseModalStructure ?? ModalProvider.UseModalStructure;

    /// <summary>
    /// Determines if the ModalInstance should be kept in memory after it has been closed.
    /// Defaults to false.
    /// </summary>
    public bool Stateful => ModalInstanceOptions?.Stateful ?? ModalProvider.Stateful;

    #region Modal

    /// <inheritdoc/>
    public Dictionary<string, object> Attributes => ModalInstanceOptions?.Attributes ?? ModalProvider.Attributes;

    /// <inheritdoc/>
    public Shadow Shadow => ModalInstanceOptions?.Shadow ?? ModalProvider.Shadow;

    /// <inheritdoc/>
    public Background Background => ModalInstanceOptions?.Background ?? ModalProvider.Background;

    /// <inheritdoc/>
    public VerticalAlignment VerticalAlignment => ModalInstanceOptions?.VerticalAlignment ?? ModalProvider.VerticalAlignment;

    /// <inheritdoc/>
    public TextOverflow TextOverflow => ModalInstanceOptions?.TextOverflow ?? ModalProvider.TextOverflow;

    /// <inheritdoc/>
    public TextWeight TextWeight => ModalInstanceOptions?.TextWeight ?? ModalProvider.TextWeight;

    /// <inheritdoc/>
    public TextTransform TextTransform => ModalInstanceOptions?.TextTransform ?? ModalProvider.TextTransform;

    /// <inheritdoc/>
    public TextAlignment TextAlignment => ModalInstanceOptions?.TextAlignment ?? ModalProvider.TextAlignment;

    /// <inheritdoc/>
    public TextColor TextColor => ModalInstanceOptions?.TextColor ?? ModalProvider.TextColor;

    /// <inheritdoc/>
    public CharacterCasing Casing => ModalInstanceOptions?.Casing ?? ModalProvider.Casing;

    /// <inheritdoc/>
    public IFluentOverflow Overflow => ModalInstanceOptions?.Overflow ?? ModalProvider.Overflow;

    /// <inheritdoc/>
    public IFluentPosition Position => ModalInstanceOptions?.Position ?? ModalProvider.Position;

    /// <inheritdoc/>
    public IFluentFlex Flex => ModalInstanceOptions?.Flex ?? ModalProvider.Flex;

    /// <inheritdoc/>
    public IFluentBorder Border => ModalInstanceOptions?.Border ?? ModalProvider.Border;

    /// <inheritdoc/>
    public IFluentDisplay Display => ModalInstanceOptions?.Display ?? ModalProvider.Display;

    /// <inheritdoc/>
    public IFluentSpacing Padding => ModalInstanceOptions?.Padding ?? ModalProvider.Padding;

    /// <inheritdoc/>
    public IFluentSpacing Margin => ModalInstanceOptions?.Margin ?? ModalProvider.Margin;

    /// <inheritdoc/>
    public IFluentSizing Height => ModalInstanceOptions?.Height ?? ModalProvider.Height;

    /// <inheritdoc/>
    public IFluentSizing Width => ModalInstanceOptions?.Width ?? ModalProvider.Width;

    /// <inheritdoc/>
    public Visibility Visibility => ModalInstanceOptions?.Visibility ?? ModalProvider.Visibility;

    /// <inheritdoc/>
    public bool Clearfix => ModalInstanceOptions?.Clearfix ?? ModalProvider.Clearfix;

    /// <inheritdoc/>
    public Float Float => ModalInstanceOptions?.Float ?? ModalProvider.Float;

    /// <inheritdoc/>
    public string Style => ModalInstanceOptions?.Style ?? ModalProvider.Style;

    /// <inheritdoc/>
    public string Class => ModalInstanceOptions?.Class ?? ModalProvider.Class;

    /// <summary>
    /// If true modal will scroll to top when opened.
    /// </summary>
    public bool ScrollToTop => ModalInstanceOptions?.ScrollToTop ?? ModalProvider.ScrollToTop;

    /// <summary>
    /// Occurs before the modal is opened.
    /// </summary>
    public Func<ModalOpeningEventArgs, Task> Opening => ModalInstanceOptions?.Opening ?? ModalProvider.Opening;

    /// <summary>
    /// Occurs before the modal is closed.
    /// </summary>
    public Func<ModalClosingEventArgs, Task> Closing => ModalInstanceOptions?.Closing ?? ModalProvider.Closing;

    /// <summary>
    /// Occurs after the modal has opened.
    /// </summary>
    public EventCallback Opened => ModalInstanceOptions?.Opened ?? ModalProvider.Opened;

    /// <summary>
    /// Occurs after the modal has closed.
    /// </summary>
    public EventCallback Closed => ModalInstanceOptions?.Closed ?? ModalProvider.Closed;

    /// <summary>
    /// Specifies the backdrop needs to be rendered for this <see cref="Modal"/>.
    /// </summary>
    public bool ShowBackdrop => ModalInstanceOptions?.ShowBackdrop ?? ModalProvider.ShowBackdrop;

    /// <summary>
    /// Gets or sets whether the component has any animations.
    /// </summary>
    public bool Animated => ModalInstanceOptions?.Animated ?? ModalProvider.Animated;

    /// <summary>
    /// Gets or sets the animation duration.
    /// </summary>
    public int AnimationDuration => ModalInstanceOptions?.AnimationDuration ?? ModalProvider.AnimationDuration;

    /// <summary>
    /// Defines how the modal content will be rendered.
    /// </summary>
    public ModalRenderMode RenderMode => ModalInstanceOptions?.RenderMode ?? ModalProvider.RenderMode;

    /// <summary>
    /// Defines if the modal should keep the input focus at all times.
    /// </summary>
    public bool? FocusTrap => ModalInstanceOptions?.FocusTrap ?? ModalProvider.FocusTrap;

    /// <inheritdoc/>
    public bool Centered => ModalInstanceOptions?.Centered ?? ModalProvider.Centered;

    /// <inheritdoc/>
    public bool Scrollable => ModalInstanceOptions?.Scrollable ?? ModalProvider.Scrollable;

    /// <inheritdoc/>
    public ModalSize Size => ModalInstanceOptions?.Size ?? ModalProvider.Size;

    #endregion

    #endregion
}