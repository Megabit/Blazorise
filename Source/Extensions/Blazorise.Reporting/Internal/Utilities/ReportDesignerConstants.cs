#region Using directives
using System;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportDesignerConstants
{
    public const double DesignerBandRailWidth = 128;

    public const double DesignerBandHeaderHeight = 20;

    public const double DesignerCollapsedBandHeight = 28;

    public const double KeyboardMoveStep = 8;

    public const double AggregateElementMinimumHeight = 24;

    public const double AggregateReportFooterHeight = 80;

    public const double DefaultDroppedFieldHeight = 18;

    public const double DefaultDroppedFieldWidth = 120;

    public const double DefaultGroupFooterLineHeight = 1;

    public const double DefaultGroupFooterLineMinimumWidth = 100;

    public const double DefaultGroupFooterLinePagePadding = 80;

    public const double DefaultGroupFooterLineX = 40;

    public const double DefaultGroupFooterLineY = 10;

    public const double DefaultGroupSectionHeight = 36;

    public const double DefaultGroupHeaderElementHeight = 18;

    public const double DefaultGroupHeaderElementWidth = 180;

    public const double DefaultGroupHeaderElementX = 30;

    public const double DefaultGroupHeaderElementY = 6;

    public const double DefaultPageWidthFallback = 794;

    public const double DragPreviewChangeTolerance = 0.1;

    public const double ElementBaselineFontRatio = 0.8;

    public const int DragPreviewFrameThrottleMilliseconds = 16;

    public const int DragPreviewFreeDropThrottleMilliseconds = 40;

    public const int SelectionBoxFrameThrottleMilliseconds = 16;

    public const int MinimumBatchElementCount = 2;

    public const double PasteElementOffset = 16;

    public const int SuppressSelectionClickMilliseconds = 300;

    public static readonly TimeSpan DragPreviewFrameThrottle = TimeSpan.FromMilliseconds( DragPreviewFrameThrottleMilliseconds );

    public static readonly TimeSpan DragPreviewFreeDropThrottle = TimeSpan.FromMilliseconds( DragPreviewFreeDropThrottleMilliseconds );

    public static readonly TimeSpan SelectionBoxFrameThrottle = TimeSpan.FromMilliseconds( SelectionBoxFrameThrottleMilliseconds );

}