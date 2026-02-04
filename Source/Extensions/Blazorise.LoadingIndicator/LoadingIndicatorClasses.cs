namespace Blazorise.LoadingIndicator;

/// <summary>
/// Component classes for <see cref="LoadingIndicator"/>.
/// </summary>
public sealed record LoadingIndicatorClasses : ComponentClasses
{
    /// <summary>
    /// Targets the indicator overlay element.
    /// </summary>
    public string Indicator { get; set; }
}

/// <summary>
/// Component styles for <see cref="LoadingIndicator"/>.
/// </summary>
public sealed record LoadingIndicatorStyles : ComponentStyles
{
    /// <summary>
    /// Targets the indicator overlay element.
    /// </summary>
    public string Indicator { get; set; }
}