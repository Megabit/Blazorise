#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Sets the options for <see cref="Offcanvas"/> instance.
/// </summary>
public class OffcanvasInstanceOptions
{
    #region Properties

    /// <summary>
    /// Uses the offcanvas standard structure, by setting this to true you are only in charge of providing the custom content.
    /// Defaults to true.
    /// </summary>
    public bool? UseOffcanvasStructure { get; set; }

    /// <summary>
    /// Determines if the offcanvas close button should be rendered when using the provider structure.
    /// </summary>
    public bool? ShowCloseButton { get; set; }

    /// <summary>
    /// Callback executed when the close button is clicked.
    /// </summary>
    public EventCallback? CloseButtonClicked { get; set; }

    /// <summary>
    /// Keeps the <see cref="OffcanvasInstance"/> in memory after it has been closed.
    /// Defaults to false.
    /// </summary>
    public bool? Stateful { get; set; }

    /// <summary>
    /// Specifies the position of the offcanvas.
    /// </summary>
    public Placement? Placement { get; set; }

    #region Offcanvas

    /// <summary>
    /// Occurs before the offcanvas is opened.
    /// </summary>
    public Func<OffcanvasOpeningEventArgs, Task> Opening { get; set; }

    /// <summary>
    /// Occurs before the offcanvas is closed.
    /// </summary>
    public Func<OffcanvasClosingEventArgs, Task> Closing { get; set; }

    /// <summary>
    /// Occurs after the offcanvas has opened.
    /// </summary>
    public EventCallback? Opened { get; set; }

    /// <summary>
    /// Occurs after the offcanvas has closed.
    /// </summary>
    public EventCallback? Closed { get; set; }

    /// <summary>
    /// Specifies the backdrop needs to be rendered for this <see cref="Offcanvas"/>.
    /// </summary>
    public bool? ShowBackdrop { get; set; }

    /// <summary>
    /// Gets or sets whether the component has any animations.
    /// </summary>
    public bool? Animated { get; set; }

    /// <summary>
    /// Gets or sets the animation duration.
    /// </summary>
    public int? AnimationDuration { get; set; }

    /// <inheritdoc/>
    public string ElementId { get; set; }

    /// <inheritdoc/>
    public string Class { get; set; }

    /// <inheritdoc/>
    public string Style { get; set; }

    /// <inheritdoc/>
    public Float? Float { get; set; }

    /// <inheritdoc/>
    public bool? Clearfix { get; set; }

    /// <inheritdoc/>
    public Visibility? Visibility { get; set; }

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
    public CharacterCasing? Casing { get; set; }

    /// <inheritdoc/>
    public TextColor TextColor { get; set; }

    /// <inheritdoc/>
    public TextAlignment? TextAlignment { get; set; }

    /// <inheritdoc/>
    public TextTransform? TextTransform { get; set; }

    /// <inheritdoc/>
    public TextWeight? TextWeight { get; set; }

    /// <inheritdoc/>
    public TextOverflow? TextOverflow { get; set; }

    /// <inheritdoc/>
    public VerticalAlignment? VerticalAlignment { get; set; }

    /// <inheritdoc/>
    public Background Background { get; set; }

    /// <inheritdoc/>
    public Shadow? Shadow { get; set; }

    /// <inheritdoc/>
    public Dictionary<string, object> Attributes { get; set; }

    #endregion

    #endregion
}