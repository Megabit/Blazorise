namespace Blazorise.DataGrid;

/// <summary>
/// Contains static readonly properties for various display, margin, flex, padding, and gap settings used in UI layouts.
/// </summary>
static class Constants
{
    // Display
    internal static readonly IFluentDisplay DisplayInlineBlock = Display.InlineBlock;
    internal static readonly IFluentDisplay DisplayNone = Display.None;
    internal static readonly IFluentDisplay DisplayAlways = Display.Always;
    internal static readonly IFluentDisplay DisplayTableRow = Display.TableRow;
    internal static readonly IFluentDisplay DisplayNoneOnMobileInlineBlockOnDesktop = Display.None.OnMobile.InlineBlock.OnDesktop;
    internal static readonly IFluentDisplay DisplayInlineBlockNoneOnDesktop = Display.InlineBlock.None.OnDesktop;
    internal static readonly IFluentDisplay DisplayNoneOnMobileInlineFlexRowOnTablet = Display.None.OnMobile.InlineFlex.Row.OnTablet;
    internal static readonly IFluentDisplay DisplayInlineFlexRowNoneOnTablet = Display.InlineFlex.Row.None.OnTablet;
    internal static readonly IFluentDisplay DisplayNoneOnMobileInlineFlexRowOnDesktop = Display.None.OnMobile.InlineFlex.Row.OnDesktop;

    // Margins
    internal static readonly IFluentSpacing MarginIsAutoOnX = Margin.IsAuto.OnX;
    internal static readonly IFluentSpacing MarginIsAutoFromStart = Margin.IsAuto.FromStart;
    internal static readonly IFluentSpacing MarginIs1FromStart = Margin.Is1.FromStart;
    internal static readonly IFluentSpacing MarginIsAutoOnYIs2FromStart = Margin.IsAuto.OnY.Is2.FromStart;
    internal static readonly IFluentSpacing MarginIs0FromBottomIs3FromStart = Margin.Is0.FromBottom.Is3.FromStart;
    internal static readonly IFluentSpacing MarginIs2FromStart = Margin.Is2.FromStart;
    internal static readonly IFluentSpacing MarginIs1FromEnd = Margin.Is1.FromEnd;

    // Flex
    internal static readonly IFluentFlex FlexRow = Flex.Row;
    internal static readonly IFluentFlex FlexJustifyContentEndAlignItemsCenter = Flex.JustifyContent.End.AlignItems.Center;
    internal static readonly IFluentFlex FlexRowAlignItemsCenterJustifyContentEnd = Flex.Row.AlignItems.Center.JustifyContent.End;
    internal static readonly IFluentFlex FlexInlineFlexOnTabletJustifyContentCenterOnTablet = Flex.InlineFlex.OnTablet.JustifyContent.Center.OnTablet;
    internal static readonly IFluentFlex FlexGrowIs1 = Flex.Grow.Is1;
    internal static readonly IFluentFlex FlexInlineFlex = Flex.InlineFlex;

    // Padding
    internal static readonly IFluentSpacing PaddingIs2OnXIs2OnY = Padding.Is2.OnX.Is2.OnY;
    internal static readonly IFluentSpacing PaddingIs3 = Padding.Is3;
    internal static readonly IFluentSpacing PaddingIs2 = Padding.Is2;

    // Gap
    internal static readonly IFluentGap GapIs2 = Gap.Is2;

    // ColumnSize
    internal static readonly IFluentColumn ColumnSizeIsAuto = ColumnSize.IsAuto;
    internal static readonly IFluentColumn ColumnSizeIs12 = ColumnSize.Is12;
    internal static readonly IFluentColumn ColumnSizeIsHalfOnDesktop = ColumnSize.IsHalf.OnDesktop;
}