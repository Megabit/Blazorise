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
        /// Ctor
        /// </summary>
        /// <param name="modalProvider"></param>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="childContent"></param>
        /// <param name="modalProviderOptions"></param>
        public ModalInstance( ModalProvider modalProvider, string id, string title, RenderFragment childContent, ModalProviderOptions modalProviderOptions )
        {
            Id = id;
            ModalProvider = modalProvider;
            Title = title;
            ChildContent = childContent;
            ModalProviderOptions = modalProviderOptions;
            Visible = true;
        }

        /// <summary>
        /// Sets the options for Modal Provider
        /// </summary>
        public ModalProviderOptions ModalProviderOptions;

        /// <summary>
        /// Uses the modal standard structure, by setting this to true you are only in charge of providing the custom content.
        /// Defaults to true.
        /// </summary>
        public bool UseModalStructure() => ModalProviderOptions?.UseModalStructure ?? ModalProvider.UseModalStructure;

        /// <summary>
        /// If true modal will scroll to top when opened.
        /// </summary>
        public bool ScrollToTop() => ModalProviderOptions?.ScrollToTop ?? ModalProvider.ScrollToTop;

        /// <summary>
        /// Occurs before the modal is opened.
        /// </summary>
        public Func<ModalOpeningEventArgs, Task> Opening() => ModalProviderOptions?.Opening ?? ModalProvider.Opening;

        /// <summary>
        /// Occurs before the modal is closed.
        /// </summary>
        public Func<ModalClosingEventArgs, Task> Closing() => ModalProviderOptions?.Closing ?? ModalProvider.Closing;

        /// <summary>
        /// Occurs after the modal has opened.
        /// </summary>
        public EventCallback Opened() => ModalProviderOptions?.Opened ?? ModalProvider.Opened;

        /// <summary>
        /// Occurs after the modal has closed.
        /// </summary>
        public EventCallback Closed() => ModalProviderOptions?.Closed ?? ModalProvider.Closed;

        /// <summary>
        /// Specifies the backdrop needs to be rendered for this <see cref="Modal"/>.
        /// </summary>
        public bool ShowBackdrop() => ModalProviderOptions?.ShowBackdrop ?? ModalProvider.ShowBackdrop;

        /// <summary>
        /// Gets or sets whether the component has any animations.
        /// </summary>
        public bool Animated() => ModalProviderOptions?.Animated ?? ModalProvider.Animated;

        /// <summary>
        /// Gets or sets the animation duration.
        /// </summary>
        public int AnimationDuration() => ModalProviderOptions?.AnimationDuration ?? ModalProvider.AnimationDuration;

        /// <summary>
        /// Defines how the modal content will be rendered.
        /// </summary>
        public ModalRenderMode RenderMode() => ModalProviderOptions?.RenderMode ?? ModalProvider.RenderMode;

        /// <summary>
        /// Defines if the modal should keep the input focus at all times.
        /// </summary>
        public bool? FocusTrap() => ModalProviderOptions?.FocusTrap ?? ModalProvider.FocusTrap;

        /// <inheritdoc/>
        public override Dictionary<string, object> Attributes => ModalProviderOptions?.Attributes ?? ModalProvider.Attributes;

        /// <inheritdoc/>
        public override Shadow Shadow => ModalProviderOptions?.Shadow ?? ModalProvider.Shadow;

        /// <inheritdoc/>
        public override Background Background => ModalProviderOptions?.Background ?? ModalProvider.Background;

        /// <inheritdoc/>
        public override VerticalAlignment VerticalAlignment => ModalProviderOptions?.VerticalAlignment ?? ModalProvider.VerticalAlignment;

        /// <inheritdoc/>
        public override TextOverflow TextOverflow => ModalProviderOptions?.TextOverflow ?? ModalProvider.TextOverflow;

        /// <inheritdoc/>
        public override TextWeight TextWeight => ModalProviderOptions?.TextWeight ?? ModalProvider.TextWeight;

        /// <inheritdoc/>
        public override TextTransform TextTransform => ModalProviderOptions?.TextTransform ?? ModalProvider.TextTransform;

        /// <inheritdoc/>
        public override TextAlignment TextAlignment => ModalProviderOptions?.TextAlignment ?? ModalProvider.TextAlignment;

        /// <inheritdoc/>
        public override TextColor TextColor => ModalProviderOptions?.TextColor ?? ModalProvider.TextColor;

        /// <inheritdoc/>
        public override CharacterCasing Casing => ModalProviderOptions?.Casing ?? ModalProvider.Casing;

        /// <inheritdoc/>
        public override IFluentOverflow Overflow => ModalProviderOptions?.Overflow ?? ModalProvider.Overflow;

        /// <inheritdoc/>
        public override IFluentPosition Position => ModalProviderOptions?.Position ?? ModalProvider.Position;

        /// <inheritdoc/>
        public override IFluentFlex Flex => ModalProviderOptions?.Flex ?? ModalProvider.Flex;

        /// <inheritdoc/>
        public override IFluentBorder Border => ModalProviderOptions?.Border ?? ModalProvider.Border;

        /// <inheritdoc/>
        public override IFluentDisplay Display => ModalProviderOptions?.Display ?? ModalProvider.Display;

        /// <inheritdoc/>
        public override IFluentSpacing Padding => ModalProviderOptions?.Padding ?? ModalProvider.Padding;

        /// <inheritdoc/>
        public override IFluentSpacing Margin => ModalProviderOptions?.Margin ?? ModalProvider.Margin;

        /// <inheritdoc/>
        public override IFluentSizing Height => ModalProviderOptions?.Height ?? ModalProvider.Height;

        /// <inheritdoc/>
        public override IFluentSizing Width => ModalProviderOptions?.Width ?? ModalProvider.Width;

        /// <inheritdoc/>
        public override Visibility Visibility => ModalProviderOptions?.Visibility ?? ModalProvider.Visibility;

        /// <inheritdoc/>
        public override bool Clearfix => ModalProviderOptions?.Clearfix ?? ModalProvider.Clearfix;

        /// <inheritdoc/>
        public override Float Float => ModalProviderOptions?.Float ?? ModalProvider.Float;

        /// <inheritdoc/>
        public override string Style => ModalProviderOptions?.Style ?? ModalProvider.Style;

        /// <inheritdoc/>
        public override string Class => ModalProviderOptions?.Class ?? ModalProvider.Class;

        /// <inheritdoc/>
        public override bool Centered => ModalProviderOptions?.Centered ?? ModalProvider.Centered;


        /// <inheritdoc/>
        public override bool Scrollable => ModalProviderOptions?.Scrollable ?? ModalProvider.Scrollable;


        /// <inheritdoc/>
        public override ModalSize Size => ModalProviderOptions?.Size ?? ModalProvider.Size;

    }

}
