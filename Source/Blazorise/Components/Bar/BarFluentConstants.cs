namespace Blazorise;

/// <summary>
/// Cached fluent utility instances for bar components to reduce repeated allocations.
/// </summary>
internal static class BarFluentConstants
{
    internal static readonly IFluentFlex FlexAlignItemsCenterJustifyContentBetween = Flex.AlignItems.Center.JustifyContent.Between;
    internal static readonly IFluentFlex FlexInlineFlexAlignItemsCenterJustifyContentCenter = Flex.InlineFlex.AlignItems.Center.JustifyContent.Center;

    internal static readonly IFluentDisplay DisplayInlineFlex = Display.InlineFlex;

    internal static readonly IFluentPosition PositionRelative = Position.Relative;
    internal static readonly IFluentPosition PositionAbsoluteTopIs0StartIs0 = Position.Absolute.Top.Is0.Start.Is0;

    internal static readonly IFluentSizing WidthRem1 = Width.Rem( 1 );
    internal static readonly IFluentSizing HeightRem1 = Height.Rem( 1 );
    internal static readonly IFluentSizing WidthIs100 = Width.Is100;
    internal static readonly IFluentSizing HeightIs100 = Height.Is100;

    internal static readonly IFluentSpacing PaddingIs2OnX = Padding.Is2.OnX;
}