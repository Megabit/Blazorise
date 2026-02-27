namespace Blazorise.Gantt.Utilities;

/// <summary>
/// Reusable fluent constants used to avoid repeated fluent object allocations in the Gantt chart.
/// </summary>
internal static class FluentConstants
{
    // Flex
    internal static readonly IFluentFlex FlexAlignItemsCenter = Flex.AlignItems.Center;
    internal static readonly IFluentFlex FlexAlignItemsCenterNoWrap = Flex.AlignItems.Center.NoWrap;
    internal static readonly IFluentFlex FlexAlignItemsCenterWrap = Flex.AlignItems.Center.Wrap;
    internal static readonly IFluentFlex FlexColumn = Flex.Column;
    internal static readonly IFluentFlex FlexDefault = Flex.Default;
    internal static readonly IFluentFlex FlexGrowIs1 = Flex.Grow.Is1;
    internal static readonly IFluentFlex FlexJustifyContentBetweenAlignItemsCenter = Flex.JustifyContent.Between.AlignItems.Center;
    internal static readonly IFluentFlex FlexJustifyContentBetweenAlignItemsCenterWrap = Flex.JustifyContent.Between.AlignItems.Center.Wrap;

    // Sizing
    internal static readonly IFluentSizing HeightIs100 = Height.Is100;
    internal static readonly IFluentSizing WidthIs100 = Width.Is100;

    // Column
    internal static readonly IFluentColumn ColumnSizeIs12OnMobileIs6OnTablet = ColumnSize.Is12.OnMobile.Is6.OnTablet;

    // Spacing
    internal static readonly IFluentSpacing MarginIs0FromBottom = Margin.Is0.FromBottom;
    internal static readonly IFluentSpacing MarginIs1FromEnd = Margin.Is1.FromEnd;
    internal static readonly IFluentSpacing MarginIs1OnX = Margin.Is1.OnX;
    internal static readonly IFluentSpacing MarginIs2OnX = Margin.Is2.OnX;
    internal static readonly IFluentSpacing MarginIs1OnXIsAutoFromStart = Margin.Is1.OnX.IsAuto.FromStart;
    internal static readonly IFluentSpacing MarginIs2FromEnd = Margin.Is2.FromEnd;
    internal static readonly IFluentSpacing PaddingIs1OnX = Padding.Is1.OnX;
    internal static readonly IFluentSpacing PaddingIs2OnX = Padding.Is2.OnX;
    internal static readonly IFluentSpacing PaddingIs3 = Padding.Is3;

    // Gap
    internal static readonly IFluentGap GapIs1 = Gap.Is1;
    internal static readonly IFluentGap GapIs2 = Gap.Is2;

    // Display
    internal static readonly IFluentDisplay DisplayInlineBlock = Display.InlineBlock;

    // Typography
    internal static readonly IFluentTextSize TextSizeSmall = TextSize.Small;

    // Border
    internal static readonly IFluentBorder BorderIs0 = Border.Is0;
    internal static readonly IFluentBorder BorderIs1 = Border.Is1;
    internal static readonly IFluentBorder BorderIs1OnBottom = Border.Is1.OnBottom;
    internal static readonly IFluentBorder BorderIs1OnEnd = Border.Is1.OnEnd;
    internal static readonly IFluentBorder BorderRounded = Border.Rounded;

    // Position
    internal static readonly IFluentPosition PositionStickyTopIs0 = Position.Sticky.Top.Is0;
}