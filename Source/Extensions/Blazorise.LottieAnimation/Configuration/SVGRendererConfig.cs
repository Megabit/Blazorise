namespace Blazorise.LottieAnimation;

public record SVGRendererConfig : BaseRendererConfig
{
    public string?                           Description         { get; init; }
    public FilterSizeConfig?                 FilterSize          { get; init; }
    public bool?                             Focusable           { get; init; }
    public bool?                             HideOnTransparent   { get; init; }
    public PreserveAspectRatioConfiguration? PreserveAspectRatio { get; init; }
    public bool?                             ProgressiveLoad     { get; init; }
    public string?                           Title               { get; init; }
    public bool?                             ViewBoxOnly         { get; init; }
    public string?                           ViewBoxSize         { get; init; }
}