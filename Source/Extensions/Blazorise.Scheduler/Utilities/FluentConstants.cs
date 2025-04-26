namespace Blazorise.Scheduler.Utilities;

/// <summary>
/// Contains a collection of constants for various UI properties such as flex, sizing, column, spacing, gap, typography,
/// position, and border. These constants facilitate consistent styling across the application, and are used to reduce
/// allocation of new objects.
/// </summary>
internal static class FluentConstants
{
    // Flex
    internal static readonly IFluentFlex FlexAlignItemsCenter = Flex.AlignItems.Center;
    internal static readonly IFluentFlex FlexColumn = Flex.Column;
    internal static readonly IFluentFlex FlexColumnAlignItemsCenterGrowIs1ShrinkIs1 = Flex.Column.AlignItems.Center.Grow.Is1.Shrink.Is1;
    internal static readonly IFluentFlex FlexColumnAlignItemsStretch = Flex.Column.AlignItems.Stretch;
    internal static readonly IFluentFlex FlexColumnJustifyContentCenterAlignItemsCenter = Flex.Column.JustifyContent.Center.AlignItems.Center;
    internal static readonly IFluentFlex FlexColumnWrap = Flex.Column.Wrap;
    internal static readonly IFluentFlex FlexDefault = Flex.Default;
    internal static readonly IFluentFlex FlexDefaultGrowIs1 = Flex.Default.Grow.Is1;
    internal static readonly IFluentFlex FlexDefaultGrowIs1ShrinkIs1 = Flex.Default.Grow.Is1.Shrink.Is1;
    internal static readonly IFluentFlex FlexGrowIs1 = Flex.Grow.Is1;
    internal static readonly IFluentFlex FlexInlineFlexAlignItemsCenter = Flex.InlineFlex.AlignItems.Center;
    internal static readonly IFluentFlex FlexInlineFlexJustifyContentBetweenAlignItemsCenter = Flex.InlineFlex.JustifyContent.Between.AlignItems.Center;
    internal static readonly IFluentFlex FlexJustifyContentBetweenAlignItemsCenter = Flex.JustifyContent.Between.AlignItems.Center;
    internal static readonly IFluentFlex FlexJustifyContentBetweenAlignItemsStart = Flex.JustifyContent.Between.AlignItems.Start;
    internal static readonly IFluentFlex FlexJustifyContentEnd = Flex.JustifyContent.End;

    // Sizing
    internal static readonly IFluentSizing HeightIs100 = Height.Is100;
    internal static readonly IFluentSizing WidthIs100 = Width.Is100;

    // Column
    internal static readonly IFluentColumn ColumnSizeIs12OnMobileIs6OnTablet = ColumnSize.Is12.OnMobile.Is6.OnTablet;
    internal static readonly IFluentColumn ColumnSizeIs3 = ColumnSize.Is3;
    internal static readonly IFluentColumn ColumnSizeIs9 = ColumnSize.Is9;

    // Spacing
    internal static readonly IFluentSpacing MarginIs0FromBottom = Margin.Is0.FromBottom;
    internal static readonly IFluentSpacing MarginIs1 = Margin.Is1;
    internal static readonly IFluentSpacing MarginIs1FromStart = Margin.Is1.FromStart;
    internal static readonly IFluentSpacing MarginIs2FromEnd = Margin.Is2.FromEnd;
    internal static readonly IFluentSpacing MarginIsAuto = Margin.IsAuto;
    internal static readonly IFluentSpacing PaddingIs1 = Padding.Is1;
    internal static readonly IFluentSpacing PaddingIs1FromEnd = Padding.Is1.FromEnd;
    internal static readonly IFluentSpacing PaddingIs1FromStart = Padding.Is1.FromStart;
    internal static readonly IFluentSpacing PaddingIs1OnX = Padding.Is1.OnX;
    internal static readonly IFluentSpacing PaddingIs2 = Padding.Is2;
    internal static readonly IFluentSpacing PaddingIs3 = Padding.Is3;
    internal static readonly IFluentSpacing PaddingIs3OnXIs1FromTop = Padding.Is3.OnX.Is1.FromTop;

    // Gap
    internal static readonly IFluentGap GapIs1 = Gap.Is1;
    internal static readonly IFluentGap GapIs2 = Gap.Is2;

    // Typography
    internal static readonly IFluentTextSize TextSizeExtraSmall = TextSize.ExtraSmall;
    internal static readonly IFluentTextSize TextSizeSmall = TextSize.Small;
    internal static readonly IFluentTextSize TextSizeMedium = TextSize.Medium;
    internal static readonly IFluentTextSize TextSizeLarge = TextSize.Large;

    // Position
    internal static readonly IFluentPosition PositionAbsolute = Position.Absolute;
    internal static readonly IFluentPosition PositionRelative = Position.Relative;

    // Border
    internal static readonly IFluentBorder BorderIs1 = Border.Is1;
    internal static readonly IFluentBorder BorderIs1Dark = Border.Is1.Dark;
    internal static readonly IFluentBorder BorderIs1OnBottom = Border.Is1.OnBottom;
    internal static readonly IFluentBorder BorderIs1OnBottomIs1OnStart = Border.Is1.OnBottom.Is1.OnStart;
    internal static readonly IFluentBorder BorderIs1OnBottomIs1OnStartIs1OnTop = Border.Is1.OnBottom.Is1.OnStart.Is1.OnTop;
    internal static readonly IFluentBorder BorderIs1OnBottomIs1OnTop = Border.Is1.OnBottom.Is1.OnTop;
    internal static readonly IFluentBorder BorderIs1OnBottomIs1OnTopIs1OnStart = Border.Is1.OnBottom.Is1.OnTop.Is1.OnStart;
    internal static readonly IFluentBorder BorderRounded = Border.Rounded;
}
