namespace Blazorise.Charts.Svg;

internal static class SvgChartOptionsMapper
{
    #region Methods

    public static SvgChartAxisOptions CreateValueAxisOptions( SvgChartAxisOptions axis )
    {
        return new()
        {
            Id = axis.Id,
            Position = axis.Position,
            BeginAtZero = axis.BeginAtZero,
            Min = axis.Min,
            Max = axis.Max,
            TickCount = axis.TickCount,
            GridLines = CreateGridLinesOptions( axis.GridLines ),
            Labels = CreateLabelsOptions( axis.Labels ),
            Title = axis.Title
        };
    }

    public static SvgChartAxisOptions CreateValueAxisOptions( SvgChartValueAxis axis )
    {
        return new()
        {
            Id = axis.Id,
            Position = axis.Position,
            BeginAtZero = axis.BeginAtZero,
            Min = axis.Min,
            Max = axis.Max,
            TickCount = axis.TickCount,
            GridLines = CreateGridLinesOptions( axis.GridLines ),
            Labels = new(),
            Title = axis.Title
        };
    }

    public static SvgChartAxisOptions CreateCategoryAxisOptions<TItem>( SvgChartAxisOptions options, SvgChartCategoryAxis<TItem> axis )
    {
        if ( axis is null )
            return CreateValueAxisOptions( options );

        return new()
        {
            Id = axis.Id,
            Position = axis.Position,
            BeginAtZero = options.BeginAtZero,
            Min = options.Min,
            Max = options.Max,
            TickCount = options.TickCount,
            GridLines = CreateGridLinesOptions( options.GridLines, axis.GridLines ),
            Labels = CreateLabelsOptions( options.Labels, axis.LabelsOptions ),
            Title = axis.Title
        };
    }

    public static SvgChartAxisLabelsOptions CreateLabelsOptions( SvgChartAxisLabelsOptions labels )
    {
        if ( labels is null )
            return null;

        return new()
        {
            Visible = labels.Visible,
            Step = labels.Step,
            Offset = labels.Offset
        };
    }

    public static SvgChartAxisLabelsOptions CreateLabelsOptions( SvgChartAxisLabelsOptions options, SvgChartAxisLabelsOptions overrides )
    {
        if ( overrides is null )
            return CreateLabelsOptions( options );

        return new()
        {
            Visible = overrides.Visible,
            Step = overrides.Step,
            Offset = overrides.Offset
        };
    }

    public static SvgChartGridLinesOptions CreateGridLinesOptions( SvgChartGridLinesOptions gridLines )
    {
        if ( gridLines is null )
            return null;

        return new()
        {
            Visible = gridLines.Visible,
            Color = gridLines.Color,
            Width = gridLines.Width,
            Opacity = gridLines.Opacity,
            DashPattern = gridLines.DashPattern
        };
    }

    public static SvgChartGridLinesOptions CreateGridLinesOptions( SvgChartGridLinesOptions options, SvgChartGridLinesOptions overrides )
    {
        if ( overrides is null )
            return CreateGridLinesOptions( options );

        return new()
        {
            Visible = overrides.Visible,
            Color = overrides.Color ?? options?.Color,
            Width = overrides.Width,
            Opacity = overrides.Opacity,
            DashPattern = overrides.DashPattern ?? options?.DashPattern
        };
    }

    public static SvgChartTextOptions CreateTextOptions( SvgChartTextOptions options )
    {
        if ( options is null )
            return new() { Visible = false };

        return new()
        {
            Visible = options.Visible,
            Text = options.Text,
            Position = options.Position,
            Alignment = options.Alignment,
            Padding = CreateSpacing( options.Padding ),
            Font = CreateFontOptions( options.Font ),
            Opacity = options.Opacity
        };
    }

    public static SvgChartTextOptions CreateTextOptions( SvgChartTextOptions options, SvgChartTextOptions overrides )
    {
        if ( overrides is null )
            return CreateTextOptions( options );

        return new()
        {
            Visible = overrides.Visible,
            Text = overrides.Text ?? options?.Text,
            Position = overrides.Position,
            Alignment = overrides.Alignment,
            Padding = CreateSpacing( overrides.Padding ?? options?.Padding ),
            Font = CreateFontOptions( options?.Font, overrides.Font ),
            Opacity = overrides.Opacity ?? options?.Opacity
        };
    }

    public static SvgChartFontOptions CreateFontOptions( SvgChartFontOptions options )
    {
        if ( options is null )
            return null;

        return new()
        {
            Family = options.Family,
            Size = options.Size,
            Weight = options.Weight,
            Color = options.Color
        };
    }

    public static SvgChartFontOptions CreateFontOptions( SvgChartFontOptions options, SvgChartFontOptions overrides )
    {
        if ( overrides is null )
            return CreateFontOptions( options );

        return new()
        {
            Family = overrides.Family ?? options?.Family,
            Size = overrides.Size ?? options?.Size,
            Weight = overrides.Weight ?? options?.Weight,
            Color = SvgChartRenderHelpers.IsDefaultColor( overrides.Color ) ? options?.Color : overrides.Color
        };
    }

    public static SvgChartSpacing CreateSpacing( SvgChartSpacing spacing )
    {
        if ( spacing is null )
            return new();

        return new()
        {
            Top = spacing.Top,
            End = spacing.End,
            Bottom = spacing.Bottom,
            Start = spacing.Start
        };
    }

    #endregion
}