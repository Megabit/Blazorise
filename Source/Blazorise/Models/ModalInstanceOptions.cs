#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Sets the options for Modal instance.
/// </summary>
public class ModalInstanceOptions
{
    #region Properties

    /// <summary>
    /// Uses the modal standard structure, by setting this to true you are only in charge of providing the custom content.
    /// Defaults to true.
    /// </summary>
    public bool? UseModalStructure { get; set; }

    /// <summary>
    /// Keeps the ModalInstance in memory after it has been closed.
    /// Defaults to false.
    /// </summary>
    public bool? Stateful { get; set; }

    #region Modal

    /// <summary>
    /// If true modal will scroll to top when opened.
    /// </summary>
    public bool? ScrollToTop { get; set; }

    /// <summary>
    /// Occurs before the modal is opened.
    /// </summary>
    public Func<ModalOpeningEventArgs, Task> Opening { get; set; }

    /// <summary>
    /// Occurs before the modal is closed.
    /// </summary>
    public Func<ModalClosingEventArgs, Task> Closing { get; set; }

    /// <summary>
    /// Occurs after the modal has opened.
    /// </summary>
    public EventCallback? Opened { get; set; }

    /// <summary>
    /// Occurs after the modal has closed.
    /// </summary>
    public EventCallback? Closed { get; set; }

    /// <summary>
    /// Specifies the backdrop needs to be rendered for this <see cref="Modal"/>.
    /// </summary>
    public bool? ShowBackdrop { get; set; }

    /// <summary>
    /// Gets or sets the animation duration.
    /// </summary>
    public bool? Animated { get; set; }

    /// <summary>
    /// Gets or sets whether the component has any animations.
    /// </summary>
    public int? AnimationDuration { get; set; }

    /// <summary>
    /// Defines how the modal content will be rendered.
    /// </summary>
    public ModalRenderMode? RenderMode { get; set; }

    /// <summary>
    /// Defines if the modal should keep the input focus at all times.
    /// </summary>
    public bool? FocusTrap { get; set; }

    /// <inheritdoc/>
    public ElementReference ElementRef { get; set; }

    /// <inheritdoc/>
    public string ElementId { get; set; }

    /// <inheritdoc/>
    public string Class { get; set; }

    /// <inheritdoc/>
    public string Style { get; set; }

    /// <inheritdoc/>
    public Float Float { get; set; }

    /// <inheritdoc/>
    public bool Clearfix { get; set; }

    /// <inheritdoc/>
    public Visibility Visibility { get; set; }

    /// <inheritdoc/>
    public IFluentSizing Width { get; set; }

    /// <inheritdoc/>
    public IFluentSizing Height { get; set; }

    /// <inheritdoc/>
    public IFluentSpacing Margin { get; set; }

    /// <inheritdoc/>
    public IFluentSpacing Padding { get; set; }

    /// <inheritdoc/>
    public IFluentDisplay Display { get; set; }

    /// <inheritdoc/>
    public IFluentBorder Border { get; set; }

    /// <inheritdoc/>
    public IFluentFlex Flex { get; set; }

    /// <inheritdoc/>
    public IFluentPosition Position { get; set; }

    /// <inheritdoc/>
    public IFluentOverflow Overflow { get; set; }

    /// <inheritdoc/>
    public CharacterCasing Casing { get; set; }

    /// <inheritdoc/>
    public TextColor TextColor { get; set; }

    /// <inheritdoc/>
    public TextAlignment TextAlignment { get; set; }

    /// <inheritdoc/>
    public TextTransform TextTransform { get; set; }

    /// <inheritdoc/>
    public TextWeight TextWeight { get; set; }

    /// <inheritdoc/>
    public TextOverflow TextOverflow { get; set; }

    /// <inheritdoc/>
    public VerticalAlignment VerticalAlignment { get; set; }

    /// <inheritdoc/>
    public Background Background { get; set; }

    /// <inheritdoc/>
    public Shadow Shadow { get; set; }

    /// <inheritdoc/>
    public Dictionary<string, object> Attributes { get; set; }

    #endregion

    #region ModalContent

    /// <summary>
    /// Centers the modal content vertically.
    /// </summary>
    /// <remarks>
    /// Only considered if <see cref="UseModalStructure"/> is set.
    /// </remarks>
    public bool? Centered { get; set; }

    /// <summary>
    /// Scrolls the modal content independent of the page itself.
    /// </summary>
    /// <remarks>
    /// Only considered if <see cref="UseModalStructure"/> is set.
    /// </remarks>
    public bool? Scrollable { get; set; }

    /// <summary>
    /// Changes the size of the modal content.
    /// </summary>
    /// <remarks>
    /// Only considered if <see cref="UseModalStructure"/> is set.
    /// </remarks>
    public ModalSize? Size { get; set; }

    #endregion

    #endregion
}