#region Using directives
using System;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportTableResizeService
{
    #region Methods

    internal ReportDesignerDragPreview CreatePreview( ReportTableElementDefinition table, ReportTablePointerResizeState pointerResize, double targetSize, ReportTableEditor tableEditor )
    {
        if ( table is null || pointerResize is null || tableEditor is null )
            return null;

        tableEditor.EnsureGrid( table );

        if ( pointerResize.Kind == ReportTableResizeKind.Column )
        {
            double x = table.X + tableEditor.GetColumnOffset( table, pointerResize.Index ) + targetSize;

            return new()
            {
                SectionIndex = pointerResize.SectionIndex,
                ElementType = ReportElementType.Line,
                X = x,
                Y = table.Y,
                Width = 1,
                Height = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, table.Height ),
            };
        }

        double y = table.Y + tableEditor.GetRowOffset( table, pointerResize.Index ) + targetSize;

        return new()
        {
            SectionIndex = pointerResize.SectionIndex,
            ElementType = ReportElementType.Line,
            X = table.X,
            Y = y,
            Width = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, table.Width ),
            Height = 1,
        };
    }

    internal double ResolveTargetSize( ReportTablePointerResizeState pointerResize, double clientX, double clientY, Func<double, double> applyGrid )
    {
        if ( pointerResize is null )
            return 0;

        double delta = pointerResize.Kind == ReportTableResizeKind.Column
            ? ReportMeasurementConverter.FromCssPixelValue( clientX - pointerResize.StartClientX )
            : ReportMeasurementConverter.FromCssPixelValue( clientY - pointerResize.StartClientY );
        double size = pointerResize.OriginalSize + delta;

        if ( pointerResize.SnapToGrid )
            size = applyGrid( size );

        double minimumSize = ReportLayoutGeometry.DefaultMinimumElementSize;
        double maximumSize = pointerResize.ResizesTable
            ? double.MaxValue
            : Math.Max( minimumSize, pointerResize.OriginalSize + pointerResize.AdjacentOriginalSize - minimumSize );

        return Math.Clamp( size, minimumSize, maximumSize );
    }

    internal void ApplyResize( ReportTableElementDefinition table, ReportTablePointerResizeState pointerResize, ReportTableEditor tableEditor )
    {
        if ( table is null || pointerResize is null || tableEditor is null )
            return;

        tableEditor.EnsureGrid( table );

        if ( pointerResize.Kind == ReportTableResizeKind.Column )
            ApplyColumnResize( table, pointerResize );
        else
            ApplyRowResize( table, pointerResize );

        tableEditor.NormalizeGrid( table );
    }

    private static void ApplyColumnResize( ReportTableElementDefinition table, ReportTablePointerResizeState pointerResize )
    {
        if ( pointerResize.Index < 0 || pointerResize.Index >= table.Columns.Count )
            return;

        if ( pointerResize.ResizesTable )
        {
            double delta = pointerResize.TargetSize - table.Columns[pointerResize.Index].Width;
            table.Columns[pointerResize.Index].Width = pointerResize.TargetSize;
            table.Width = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, table.Width + delta );
        }
        else if ( pointerResize.Index + 1 < table.Columns.Count )
        {
            double totalWidth = table.Columns[pointerResize.Index].Width + table.Columns[pointerResize.Index + 1].Width;
            double minimumWidth = ReportLayoutGeometry.DefaultMinimumElementSize;
            double maximumWidth = Math.Max( minimumWidth, totalWidth - minimumWidth );
            double width = Math.Clamp( pointerResize.TargetSize, minimumWidth, maximumWidth );
            table.Columns[pointerResize.Index].Width = width;
            table.Columns[pointerResize.Index + 1].Width = totalWidth - width;
        }
    }

    private static void ApplyRowResize( ReportTableElementDefinition table, ReportTablePointerResizeState pointerResize )
    {
        if ( pointerResize.Index < 0 || pointerResize.Index >= table.Rows.Count )
            return;

        if ( pointerResize.ResizesTable )
        {
            double delta = pointerResize.TargetSize - table.Rows[pointerResize.Index].Height;
            table.Rows[pointerResize.Index].Height = pointerResize.TargetSize;
            table.Height = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, table.Height + delta );
        }
        else if ( pointerResize.Index + 1 < table.Rows.Count )
        {
            double totalHeight = table.Rows[pointerResize.Index].Height + table.Rows[pointerResize.Index + 1].Height;
            double minimumHeight = ReportLayoutGeometry.DefaultMinimumElementSize;
            double maximumHeight = Math.Max( minimumHeight, totalHeight - minimumHeight );
            double height = Math.Clamp( pointerResize.TargetSize, minimumHeight, maximumHeight );
            table.Rows[pointerResize.Index].Height = height;
            table.Rows[pointerResize.Index + 1].Height = totalHeight - height;
        }
    }

    #endregion
}