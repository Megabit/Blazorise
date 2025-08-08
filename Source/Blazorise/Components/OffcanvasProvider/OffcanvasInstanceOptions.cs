using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Components.OffcanvasProvider;
public class OffcanvasInstanceOptions
{
    public string? ElementId { get; set; }
    public bool? Stateful { get; set; }
    public Placement? Placement { get; set; }
    public bool? ShowCloseButton { get; set; }
    public EventCallback? CloseButtonClicked { get; set; }
    public string? Title { get; set; }
    public RenderFragment? Body { get; set; }
    public RenderFragment? Footer { get; set; }

    #region Required

    public Dictionary<string, object>? Attributes { get; set; }
    public Shadow? Shadow { get; set; }
    public Background? Background { get; set; }
    public VerticalAlignment? VerticalAlignment { get; set; }
    public TextOverflow? TextOverflow { get; set; }
    public TextWeight? TextWeight { get; set; }
    public TextTransform? TextTransform { get; set; }
    public TextAlignment? TextAlignment { get; set; }
    public TextColor? TextColor { get; set; }
    public CharacterCasing? Casing { get; set; }
    public IFluentOverflow? Overflow { get; set; }
    public IFluentPosition? Position { get; set; }
    public IFluentFlex? Flex { get; set; }
    public IFluentBorder? Border { get; set; }
    public IFluentDisplay? Display { get; set; }
    public IFluentSpacing? Padding { get; set; }
    public IFluentSpacing? Margin { get; set; }
    public IFluentSizing? Height { get; set; }
    public IFluentSizing? Width { get; set; }
    public Visibility? Visibility { get; set; }
    public bool? Clearfix { get; set; }
    public Float? Float { get; set; }
    public string? Style { get; set; }
    public string? Class { get; set; }

    public Func<OffcanvasOpeningEventArgs, Task>? Opening { get; set; }
    public EventCallback? Opened { get; set; }
    public Func<OffcanvasClosingEventArgs, Task>? Closing { get; set; }
    public EventCallback? Closed { get; set; }
    public bool? ShowBackdrop { get; set; }
    public bool? Animated { get; set; }
    public int? AnimationDuration { get; set; }

    #endregion
}