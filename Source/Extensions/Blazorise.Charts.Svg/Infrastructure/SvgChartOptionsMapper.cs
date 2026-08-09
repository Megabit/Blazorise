namespace Blazorise.Charts.Svg;

internal static class SvgChartOptionsMapper
{
    #region Methods

    public static SvgChartAxisOptions CreateValueAxisOptions( SvgChartAxisOptions axis )
    {
        axis ??= new();

        return new()
        {
            Id = axis.Id,
            Position = axis.Position,
            BeginAtZero = axis.BeginAtZero,
            Min = axis.Min,
            Max = axis.Max,
            TickCount = axis.TickCount,
            Stacked = axis.Stacked,
            TickFormatter = axis.TickFormatter,
            GridLines = CreateGridLinesOptions( axis.GridLines ),
            Labels = CreateLabelsOptions( axis.Labels ),
            Title = axis.Title
        };
    }

    public static SvgChartAxisOptions CreateValueAxisOptions( SvgChartValueAxis axis )
    {
        if ( axis is null )
            return CreateValueAxisOptions( new SvgChartAxisOptions() );

        return axis.ResolveOptions( new() );
    }

    public static SvgChartAxisOptions CreateCategoryAxisOptions<TItem>( SvgChartAxisOptions options, SvgChartCategoryAxis<TItem> axis )
    {
        if ( axis is null )
            return CreateValueAxisOptions( options );

        return axis.ResolveOptions( options );
    }

    public static SvgChartAxisLabelsOptions CreateLabelsOptions( SvgChartAxisLabelsOptions labels )
    {
        if ( labels is null )
            return null;

        return new()
        {
            Visible = labels.Visible ?? true,
            Step = labels.Step,
            AutoSkip = labels.AutoSkip,
            MaxTicksLimit = labels.MaxTicksLimit,
            AutoSkipPadding = labels.AutoSkipPadding,
            AutoRotate = labels.AutoRotate,
            MaxRotation = labels.MaxRotation,
            Offset = labels.Offset,
            MaxWidth = labels.MaxWidth
        };
    }

    public static SvgChartAxisLabelsOptions CreateLabelsOptions( SvgChartAxisLabelsOptions options, SvgChartAxisLabelsOptions overrides )
    {
        if ( overrides is null )
            return CreateLabelsOptions( options );

        return new()
        {
            Visible = overrides.Visible ?? options?.Visible ?? true,
            Step = overrides.Step,
            AutoSkip = overrides.AutoSkip,
            MaxTicksLimit = overrides.MaxTicksLimit,
            AutoSkipPadding = overrides.AutoSkipPadding,
            AutoRotate = overrides.AutoRotate,
            MaxRotation = overrides.MaxRotation,
            Offset = overrides.Offset,
            MaxWidth = overrides.MaxWidth ?? options?.MaxWidth
        };
    }

    public static SvgChartAxisLabelsOptions CreateLabelsOptions( SvgChartAxisLabelsOptions options, ComponentParameterInfo<SvgChartAxisLabelsOptions> overrides )
    {
        if ( !overrides.Defined )
            return CreateLabelsOptions( options );

        return overrides.Value is null
            ? null
            : CreateLabelsOptions( options, overrides.Value );
    }

    public static SvgChartGridLinesOptions CreateGridLinesOptions( SvgChartGridLinesOptions gridLines )
    {
        if ( gridLines is null )
            return null;

        return new()
        {
            Visible = gridLines.Visible ?? true,
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
            Visible = overrides.Visible ?? options?.Visible ?? true,
            Color = overrides.Color ?? options?.Color,
            Width = overrides.Width,
            Opacity = overrides.Opacity,
            DashPattern = overrides.DashPattern ?? options?.DashPattern
        };
    }

    public static SvgChartGridLinesOptions CreateGridLinesOptions( SvgChartGridLinesOptions options, ComponentParameterInfo<SvgChartGridLinesOptions> overrides )
    {
        if ( !overrides.Defined )
            return CreateGridLinesOptions( options );

        return overrides.Value is null
            ? null
            : CreateGridLinesOptions( options, overrides.Value );
    }

    public static SvgChartTextOptions CreateTextOptions( SvgChartTextOptions options )
    {
        if ( options is null )
        {
            return new()
            {
                Visible = false,
                Position = SvgChartTextPosition.Top,
                Alignment = SvgChartTextAlignment.Center
            };
        }

        return new()
        {
            Visible = options.Visible ?? true,
            Text = options.Text,
            Position = options.Position ?? SvgChartTextPosition.Top,
            Alignment = options.Alignment ?? SvgChartTextAlignment.Center,
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
            Visible = overrides.Visible ?? options?.Visible ?? true,
            Text = overrides.Text ?? options?.Text,
            Position = overrides.Position ?? options?.Position ?? SvgChartTextPosition.Top,
            Alignment = overrides.Alignment ?? options?.Alignment ?? SvgChartTextAlignment.Center,
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