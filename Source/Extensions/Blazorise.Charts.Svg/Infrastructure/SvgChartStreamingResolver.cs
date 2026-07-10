namespace Blazorise.Charts.Svg;

internal static class SvgChartStreamingResolver
{
    #region Methods

    public static SvgChartStreamingOptions Resolve( SvgChartStreaming streaming, SvgChartStreamingOptions fallback )
    {
        if ( streaming is null )
            return fallback ?? new() { Enabled = false };

        return streaming.ResolveOptions( fallback );
    }

    public static bool IsReversed( SvgChartStreamingOptions streaming )
    {
        return streaming?.Reverse == true;
    }

    public static bool IsAnimationEnabled( SvgChartStreamingOptions streaming )
    {
        return streaming?.Animation?.Enabled == true;
    }

    #endregion
}