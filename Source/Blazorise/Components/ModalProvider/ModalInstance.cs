#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Generates a new instance of a Modal
    /// </summary>
    public class ModalInstance : ModalProviderBaseAttributes
    {
        /// <summary>
        /// Gets or sets the unique id of the modal instance
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Tracks the Modal Reference
        /// </summary>
        public Modal ModalRef { get; set; }

        /// <summary>
        /// The Modal Provider
        /// </summary>
        public ModalProvider ModalProvider { get; private set; }

        /// <summary>
        /// Child Content to be rendered
        /// </summary>
        public RenderFragment ChildContent { get; private set; }

        /// <summary>
        /// Modal's Header Title
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Control's the Modal Visibility
        /// </summary>
        public bool Visible { get; set; }

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
            Id = id;
            ModalProvider = modalProvider;
            Title = title;
            ChildContent = childContent;
            ModalInstanceOptions = modalInstanceOptions;
            Visible = true;
        }

        /// <summary>
        /// Sets the options for Modal Provider
        /// </summary>
        public ModalInstanceOptions ModalInstanceOptions;

        /// <summary>
        /// Uses the modal standard structure, by setting this to true you are only in charge of providing the custom content.
        /// Defaults to true.
        /// </summary>
        public bool UseModalStructure => ModalInstanceOptions?.UseModalStructure ?? ModalProvider.UseModalStructure;

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
        public override Dictionary<string, object> Attributes => ModalInstanceOptions?.Attributes ?? ModalProvider.Attributes;

        /// <inheritdoc/>
        public override Shadow Shadow => ModalInstanceOptions?.Shadow ?? ModalProvider.Shadow;

        /// <inheritdoc/>
        public override Background Background => ModalInstanceOptions?.Background ?? ModalProvider.Background;

        /// <inheritdoc/>
        public override VerticalAlignment VerticalAlignment => ModalInstanceOptions?.VerticalAlignment ?? ModalProvider.VerticalAlignment;

        /// <inheritdoc/>
        public override TextOverflow TextOverflow => ModalInstanceOptions?.TextOverflow ?? ModalProvider.TextOverflow;

        /// <inheritdoc/>
        public override TextWeight TextWeight => ModalInstanceOptions?.TextWeight ?? ModalProvider.TextWeight;

        /// <inheritdoc/>
        public override TextTransform TextTransform => ModalInstanceOptions?.TextTransform ?? ModalProvider.TextTransform;

        /// <inheritdoc/>
        public override TextAlignment TextAlignment => ModalInstanceOptions?.TextAlignment ?? ModalProvider.TextAlignment;

        /// <inheritdoc/>
        public override TextColor TextColor => ModalInstanceOptions?.TextColor ?? ModalProvider.TextColor;

        /// <inheritdoc/>
        public override CharacterCasing Casing => ModalInstanceOptions?.Casing ?? ModalProvider.Casing;

        /// <inheritdoc/>
        public override IFluentOverflow Overflow => ModalInstanceOptions?.Overflow ?? ModalProvider.Overflow;

        /// <inheritdoc/>
        public override IFluentPosition Position => ModalInstanceOptions?.Position ?? ModalProvider.Position;

        /// <inheritdoc/>
        public override IFluentFlex Flex => ModalInstanceOptions?.Flex ?? ModalProvider.Flex;

        /// <inheritdoc/>
        public override IFluentBorder Border => ModalInstanceOptions?.Border ?? ModalProvider.Border;

        /// <inheritdoc/>
        public override IFluentDisplay Display => ModalInstanceOptions?.Display ?? ModalProvider.Display;

        /// <inheritdoc/>
        public override IFluentSpacing Padding => ModalInstanceOptions?.Padding ?? ModalProvider.Padding;

        /// <inheritdoc/>
        public override IFluentSpacing Margin => ModalInstanceOptions?.Margin ?? ModalProvider.Margin;

        /// <inheritdoc/>
        public override IFluentSizing Height => ModalInstanceOptions?.Height ?? ModalProvider.Height;

        /// <inheritdoc/>
        public override IFluentSizing Width => ModalInstanceOptions?.Width ?? ModalProvider.Width;

        /// <inheritdoc/>
        public override Visibility Visibility => ModalInstanceOptions?.Visibility ?? ModalProvider.Visibility;

        /// <inheritdoc/>
        public override bool Clearfix => ModalInstanceOptions?.Clearfix ?? ModalProvider.Clearfix;

        /// <inheritdoc/>
        public override Float Float => ModalInstanceOptions?.Float ?? ModalProvider.Float;

        /// <inheritdoc/>
        public override string Style => ModalInstanceOptions?.Style ?? ModalProvider.Style;

        /// <inheritdoc/>
        public override string Class => ModalInstanceOptions?.Class ?? ModalProvider.Class;

        /// <inheritdoc/>
        public override bool Centered => ModalInstanceOptions?.Centered ?? ModalProvider.Centered;

        /// <inheritdoc/>
        public override bool Scrollable => ModalInstanceOptions?.Scrollable ?? ModalProvider.Scrollable;

        /// <inheritdoc/>
        public override ModalSize Size => ModalInstanceOptions?.Size ?? ModalProvider.Size;
    }
}
