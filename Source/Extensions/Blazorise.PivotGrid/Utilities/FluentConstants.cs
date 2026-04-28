namespace Blazorise.PivotGrid.Utilities;

/// <summary>
/// Reusable fluent constants used by the PivotGrid component.
/// </summary>
internal static class FluentConstants
{
    internal static readonly IFluentBorder BorderIs1 = Border.Is1;
    internal static readonly IFluentBorder BorderIs1OnBottom = Border.Is1.OnBottom;
    internal static readonly IFluentColumn ColumnSizeIs1 = ColumnSize.Is1;
    internal static readonly IFluentColumn ColumnSizeIs2 = ColumnSize.Is2;
    internal static readonly IFluentColumn ColumnSizeIsAuto = ColumnSize.IsAuto;
    internal static readonly IFluentFlex FlexColumn = Flex.Column;
    internal static readonly IFluentFlex FlexGrowIs1 = Flex.Grow.Is1;
    internal static readonly IFluentFlex FlexRow = Flex.Row;
    internal static readonly IFluentFlex FlexRowAlignItemsCenter = Flex.AlignItems.Center;
    internal static readonly IFluentFlex FlexRowAlignItemsCenterJustifyContentBetween = Flex.Row.AlignItems.Center.JustifyContent.Between;
    internal static readonly IFluentFlex FlexRowJustifyContentEnd = Flex.Row.JustifyContent.End;
    internal static readonly IFluentGap GapIs1 = Gap.Is1;
    internal static readonly IFluentGap GapIs2 = Gap.Is2;
    internal static readonly IFluentSpacing MarginIs0FromBottomIs3FromStart = Margin.Is0.FromBottom.Is3.FromStart;
    internal static readonly IFluentSpacing MarginIs2FromStart = Margin.Is2.FromStart;
    internal static readonly IFluentSpacing MarginIs2FromTop = Margin.Is2.FromTop;
    internal static readonly IFluentSpacing MarginIsAutoFromStart = Margin.IsAuto.FromStart;
    internal static readonly IFluentOverflow OverflowAuto = Overflow.Auto;
    internal static readonly IFluentSizing HeightIs100 = Height.Is100;
    internal static readonly IFluentSizing HeightVh60 = Height.Vh( 60 );
    internal static readonly IFluentGridColumns GridColumnsAre2 = GridColumns.Are2;
    internal static readonly IFluentGridColumns GridColumnsAre3 = GridColumns.Are3;
    internal static readonly IFluentGridRows GridRowsAre2 = GridRows.Are2;
    internal static readonly IFluentSpacing PaddingIs2 = Padding.Is2;
    internal static readonly IFluentSpacing PaddingIs2OnX = Padding.Is2.OnX;
    internal static readonly IFluentSpacing PaddingIs3 = Padding.Is3;
    internal static readonly IFluentTextSize TextSizeSmall = TextSize.Small;
    internal static readonly Background BackgroundBody = Background.Body;
    internal static readonly Background BackgroundLight = Background.Light;
    internal static readonly Background BackgroundPrimarySubtle = Background.Primary.Subtle;
    internal static readonly TextColor TextColorMuted = TextColor.Muted;
}