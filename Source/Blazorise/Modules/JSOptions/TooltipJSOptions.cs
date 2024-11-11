namespace Blazorise;

internal class TooltipInitializeJSOptions
{
    public string Text { get; set; }
    public string Placement { get; set; }
    public bool Multiline { get; set; }
    public bool AlwaysActive { get; set; }
    public bool ShowArrow { get; set; }
    public bool Fade { get; set; }
    public int FadeDuration { get; set; }
    public string Trigger { get; set; }
    public string TriggerTargetId { get; set; }
    public string MaxWidth { get; set; }
    public bool AutodetectInline { get; set; }
    public int? ZIndex { get; set; }
    public bool Interactive { get; set; }
    public string AppendTo { get; set; }
    public TooltipDelay Delay { get; set; }
}

internal class TooltipDelay
{
    public int Show { get; set; }
    public int Hide { get; set; }
}

