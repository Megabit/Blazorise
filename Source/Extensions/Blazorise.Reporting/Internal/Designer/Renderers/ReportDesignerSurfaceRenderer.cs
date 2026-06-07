#region Using directives
using System;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportDesignerSurfaceRenderer
{
    #region Methods

    internal static void RenderElementResizeHandles( RenderTreeBuilder builder, object eventReceiver, string elementKey, Action<string, ReportElementResizeHandle, PointerEventArgs> resizeStarted )
    {
        RenderElementResizeHandle( builder, eventReceiver, elementKey, ReportElementResizeHandle.NorthWest, "nw", resizeStarted );
        RenderElementResizeHandle( builder, eventReceiver, elementKey, ReportElementResizeHandle.North, "n", resizeStarted );
        RenderElementResizeHandle( builder, eventReceiver, elementKey, ReportElementResizeHandle.NorthEast, "ne", resizeStarted );
        RenderElementResizeHandle( builder, eventReceiver, elementKey, ReportElementResizeHandle.East, "e", resizeStarted );
        RenderElementResizeHandle( builder, eventReceiver, elementKey, ReportElementResizeHandle.SouthEast, "se", resizeStarted );
        RenderElementResizeHandle( builder, eventReceiver, elementKey, ReportElementResizeHandle.South, "s", resizeStarted );
        RenderElementResizeHandle( builder, eventReceiver, elementKey, ReportElementResizeHandle.SouthWest, "sw", resizeStarted );
        RenderElementResizeHandle( builder, eventReceiver, elementKey, ReportElementResizeHandle.West, "w", resizeStarted );
    }

    internal static void RenderDragPreview( RenderTreeBuilder builder, ReportDesignerDragPreview preview )
    {
        builder.OpenElement( "div" );
        builder.Key( "drag-preview" );
        builder.Class( $"b-report-drag-preview b-report-element-{preview.ElementType.ToString().ToLowerInvariant()}" );
        builder.Style( $"left:{preview.X}px;top:{preview.Y}px;width:{preview.Width}px;height:{preview.Height}px;" );
        builder.Content( preview.Text );
        builder.CloseElement();
    }

    internal static void RenderSelectionBox( RenderTreeBuilder builder, ReportDesignerSelectionBox selectionBox, double leftOffset )
    {
        builder.OpenElement( "div" );
        builder.Key( "selection-box" );
        builder.Class( "b-report-selection-box" );
        builder.Style( $"left:{selectionBox.X + leftOffset}px;top:{selectionBox.Y}px;width:{selectionBox.Width}px;height:{selectionBox.Height}px;" );
        builder.CloseElement();
    }

    internal static void RenderTable( RenderTreeBuilder builder, ReportElementDefinition element )
    {
        builder.OpenElement( "table" );

        if ( element.Columns.Count == 0 )
        {
            builder.OpenElement( "tr" );
            builder.OpenElement( "td" );
            builder.CloseElement();
            builder.CloseElement();
        }
        else
        {
            builder.OpenElement( "tr" );
            foreach ( var column in element.Columns )
            {
                builder.OpenElement( "td" );
                builder.Style( $"width:{column.Width}px" );
                builder.Content( column.Title ?? column.Field );
                builder.CloseElement();
            }
            builder.CloseElement();
        }

        builder.CloseElement();
    }

    private static void RenderElementResizeHandle( RenderTreeBuilder builder, object eventReceiver, string elementKey, ReportElementResizeHandle handle, string handleClass, Action<string, ReportElementResizeHandle, PointerEventArgs> resizeStarted )
    {
        builder.OpenElement( "span" );
        builder.Key( handleClass );
        builder.Class( $"b-report-resize-handle b-report-resize-handle-{handleClass}" );
        builder.Attribute( "onpointerdown", EventCallback.Factory.Create<PointerEventArgs>( eventReceiver, eventArgs => resizeStarted( elementKey, handle, eventArgs ) ) );
        builder.EventPreventDefault( "onpointerdown", true );
        builder.EventStopPropagation( "onpointerdown", true );
        builder.CloseElement();
    }

    #endregion
}