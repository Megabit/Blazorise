namespace Blazorise.Reporting.Internal;

/// <summary>
/// Reusable fluent constants used to avoid repeated fluent object allocations in Reporting components.
/// </summary>
internal static class FluentConstants
{
    // Display
    internal static readonly IFluentDisplay DisplayFlex = Display.Flex;
    internal static readonly IFluentDisplay DisplayInlineBlock = Display.InlineBlock;
    internal static readonly IFluentDisplay DisplayNone = Display.None;

    // Flex
    internal static readonly IFluentFlex FlexAlignItemsCenter = Flex.AlignItems.Center;
    internal static readonly IFluentFlex FlexColumn = Flex.Column;
    internal static readonly IFluentFlex FlexJustifyContentBetweenAlignItemsCenter = Flex.JustifyContent.Between.AlignItems.Center;
    internal static readonly IFluentFlex FlexRow = Flex.Row;
    internal static readonly IFluentFlex FlexIsWrap = Flex.Wrap;

    // Gap
    internal static readonly IFluentGap GapIs1 = Gap.Is1;
    internal static readonly IFluentGap GapIs2 = Gap.Is2;
    internal static readonly IFluentGap GapIs3 = Gap.Is3;

    // Spacing
    internal static readonly IFluentSpacing MarginIs0 = Margin.Is0;
    internal static readonly IFluentSpacing MarginIs0FromBottomIs0OnX = Margin.Is0.FromBottom.Is0.OnX;
    internal static readonly IFluentSpacing MarginIs1FromBottom = Margin.Is1.FromBottom;
    internal static readonly IFluentSpacing MarginIs1FromEnd = Margin.Is1.FromEnd;
    internal static readonly IFluentSpacing MarginIs2FromEnd = Margin.Is2.FromEnd;
    internal static readonly IFluentSpacing MarginIs2FromTop = Margin.Is2.FromTop;
    internal static readonly IFluentSpacing MarginIs3FromBottom = Margin.Is3.FromBottom;
    internal static readonly IFluentSpacing MarginIs3FromTop = Margin.Is3.FromTop;
    internal static readonly IFluentSpacing MarginIsAutoFromStart = Margin.IsAuto.FromStart;
    internal static readonly IFluentSpacing PaddingIs0 = Padding.Is0;
    internal static readonly IFluentSpacing PaddingIs1FromEnd = Padding.Is1.FromEnd;
    internal static readonly IFluentSpacing PaddingIs2 = Padding.Is2;
    internal static readonly IFluentSpacing PaddingIs3 = Padding.Is3;
    internal static readonly IFluentSpacing PaddingIs5 = Padding.Is5;

    // Sizing
    internal static readonly IFluentSizing HeightPx180 = Height.Px( 180 );
    internal static readonly IFluentSizing WidthIs100 = Width.Is100;

    // Column
    internal static readonly IFluentColumn ColumnSizeIs4 = ColumnSize.Is4;
    internal static readonly IFluentColumn ColumnSizeIs8 = ColumnSize.Is8;

    // Border
    internal static readonly IFluentBorder BorderIs1 = Border.Is1;
    internal static readonly IFluentBorder BorderIs1OnTop = Border.Is1.OnTop;

    // Background
    internal static readonly Background BackgroundLight = Background.Light;
    internal static readonly Background BackgroundWhite = Background.White;

    // Overflow
    internal static readonly IFluentOverflow OverflowAuto = Overflow.Auto;

    // Typography
    internal static readonly TextAlignment TextAlignmentCenter = TextAlignment.Center;
    internal static readonly TextColor TextColorSecondary = TextColor.Secondary;
    internal static readonly TextOverflow TextOverflowTruncate = TextOverflow.Truncate;
    internal static readonly IFluentTextSize TextSizeSmall = TextSize.Small;
    internal static readonly TextWeight TextWeightBold = TextWeight.Bold;
}
