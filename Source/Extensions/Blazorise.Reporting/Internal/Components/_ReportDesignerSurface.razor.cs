#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using DesignerConstants = Blazorise.Reporting.Internal.ReportDesignerConstants;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the interactive report design surface in its own render boundary.
/// </summary>
public partial class _ReportDesignerSurface
{
    #region Members

    private readonly ReportDesignerDragDropService dragDropService = new();

    private readonly ReportTableResizeService tableResizeService = new();

    private bool designerDropInProgress;

    private _ReportDesignerPage designerPageRef;

    private DotNetObjectReference<_ReportDesignerSurface> dotNetObjectReference;

    private JSReportingModule reportingModule;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            if ( reportingModule is not null )
            {
                try
                {
                    await reportingModule.StopSectionResize();
                    await reportingModule.DisposeAsync();
                }
                catch ( JSDisconnectedException )
                {
                }
            }

            dotNetObjectReference?.Dispose();
        }

        await base.DisposeAsync( disposing );
    }

    internal void BeginFieldDrag( string dataSourceName, string fieldName )
    {
        ReportDesignerInteractionService.BeginFieldDrag( designerState, dataSourceName, fieldName );
    }

    internal void BeginToolboxElementDrag( ReportElementType elementType, string text )
    {
        if ( elementType == ReportElementType.Subreport && !CanInsertSubreportElement )
            return;

        ReportDesignerInteractionService.BeginToolboxElementDrag( designerState, elementType, text );
    }

    internal bool IsExternalDesignerDragActive()
    {
        return ReportDesignerInteractionService.IsExternalDesignerDragActive( designerState );
    }

    internal Task BeginElementPointerDrag( string elementKey, PointerEventArgs eventArgs )
    {
        if ( eventArgs.CtrlKey )
        {
            ToggleElementSelection( elementKey );
            designerState.SuppressNextElementClickKey = elementKey;
            return Task.CompletedTask;
        }

        if ( !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, elementKey, out var sectionIndex, out _, out var element )
            || element.Suppress?.Value == true )
            return Task.CompletedTask;

        ReportDesignerInteractionService.TryBeginElementPointerDrag(
            designerState,
            elementKey,
            element,
            sectionIndex,
            eventArgs,
            IsSnapToGridEnabled( element ),
            CaptureElementPointerItems( EffectiveDefinition, elementKey ).ToList() );

        SelectElement( elementKey, preserveSelection: selectionManager.IsElementSelected( elementKey ) && selectionManager.SelectedElementKeys.Count > 1 );

        return Task.CompletedTask;
    }

    internal async Task BeginElementPointerResize( string elementKey, ReportElementResizeHandle handle, PointerEventArgs eventArgs )
    {
        if ( !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, elementKey, out var sectionIndex, out _, out var element )
            || element.Suppress?.Value == true )
            return;

        bool started = ReportDesignerInteractionService.TryBeginElementPointerResize(
            designerState,
            elementKey,
            element,
            sectionIndex,
            handle,
            eventArgs,
            IsSnapToGridEnabled( element ),
            CaptureElementPointerItems( EffectiveDefinition, elementKey ).ToList() );

        SelectElement( elementKey, preserveSelection: selectionManager.IsElementSelected( elementKey ) && selectionManager.SelectedElementKeys.Count > 1 );

        if ( started )
            await StartDocumentElementResize( eventArgs.ClientX, eventArgs.ClientY, eventArgs.PointerId );
    }

    internal Task BeginElementPointerResize( string elementKey, int handle, PointerEventArgs eventArgs )
    {
        return BeginElementPointerResize( elementKey, (ReportElementResizeHandle)handle, eventArgs );
    }

    internal Task BeginTablePointerResize( string tableKey, string cellKey, ReportTableResizeKind kind, int index, PointerEventArgs eventArgs )
    {
        if ( !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, tableKey, out int sectionIndex, out _, out ReportElementDefinition element )
            || element is not ReportTableElementDefinition table
            || table.Suppress?.Value == true )
        {
            return Task.CompletedTask;
        }

        tableEditor.EnsureGrid( table );

        if ( kind == ReportTableResizeKind.Column && ( index < 0 || index >= table.Columns.Count ) )
            return Task.CompletedTask;

        if ( kind == ReportTableResizeKind.Row && ( index < 0 || index >= table.Rows.Count ) )
            return Task.CompletedTask;

        bool resizesTable = kind == ReportTableResizeKind.Column
            ? index >= table.Columns.Count - 1
            : index >= table.Rows.Count - 1;
        double adjacentOriginalSize = !resizesTable
            ? kind == ReportTableResizeKind.Column
                ? table.Columns[index + 1].Width
                : table.Rows[index + 1].Height
            : 0;

        designerState.DraggedKind = ReportDesignerDragKind.None;
        designerState.DraggedDataSourceName = null;
        designerState.DraggedFieldName = null;
        designerState.DraggedElementType = null;
        designerState.DraggedElementText = null;
        designerState.DraggedElementKey = tableKey;
        designerState.DraggedElement = table;
        designerState.DragPreview = null;
        designerState.LastDragPreviewRenderTime = DateTime.MinValue;
        designerState.ElementPointerDrag = null;
        designerState.ElementPointerResize = null;
        designerState.SectionPointerResize = null;
        designerState.TablePointerResize = new()
        {
            TableKey = tableKey,
            CellKey = cellKey,
            SectionIndex = sectionIndex,
            Kind = kind,
            Index = index,
            OriginalSize = kind == ReportTableResizeKind.Column ? table.Columns[index].Width : table.Rows[index].Height,
            AdjacentOriginalSize = adjacentOriginalSize,
            TargetSize = kind == ReportTableResizeKind.Column ? table.Columns[index].Width : table.Rows[index].Height,
            StartClientX = eventArgs.ClientX,
            StartClientY = eventArgs.ClientY,
            SnapToGrid = IsSnapToGridEnabled( table ),
            ResizesTable = resizesTable,
        };

        if ( !string.IsNullOrWhiteSpace( cellKey ) )
            SelectTableCell( cellKey );
        else
            SelectElement( tableKey );

        return InvokeAsync( StateHasChanged );
    }

    internal async Task BeginSectionPointerResize( int sectionIndex, PointerEventArgs eventArgs )
    {
        if ( TryResolveElementResizeFromSectionResize( sectionIndex, eventArgs, out string elementKey, out ReportElementResizeHandle handle ) )
        {
            await BeginElementPointerResize( elementKey, handle, eventArgs );
            return;
        }

        var definition = EffectiveDefinition;

        if ( sectionIndex < 0 || sectionIndex >= definition.Bands.Count )
            return;

        var section = definition.Bands[sectionIndex];

        if ( ReportValueResolver.ResolveStaticSuppress( section ) )
            return;

        designerState.DraggedKind = ReportDesignerDragKind.None;
        designerState.DraggedDataSourceName = null;
        designerState.DraggedFieldName = null;
        designerState.DraggedElementType = null;
        designerState.DraggedElementText = null;
        designerState.DraggedElementKey = null;
        designerState.DraggedElement = null;
        designerState.DragPreview = null;
        designerState.ElementPointerDrag = null;
        designerState.ElementPointerResize = null;
        designerState.TablePointerResize = null;
        designerState.SectionPointerResize = new()
        {
            SectionIndex = sectionIndex,
            OriginalHeight = section.Height,
            TargetHeight = section.Height,
            StartClientY = eventArgs.ClientY,
        };

        SelectSection( sectionIndex );

        await StartDocumentSectionResize( eventArgs.ClientY, eventArgs.PointerId );
        await InvokeAsync( StateHasChanged );
    }

    private bool TryResolveElementResizeFromSectionResize( int sectionIndex, PointerEventArgs eventArgs, out string elementKey, out ReportElementResizeHandle handle )
    {
        elementKey = null;
        handle = default;

        ReportDefinition definition = EffectiveDefinition;

        if ( definition is null || sectionIndex < 0 || sectionIndex >= definition.Bands.Count )
            return false;

        ReportBandDefinition section = definition.Bands[sectionIndex];
        double pointerX = ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetX );
        double handleTolerance = ReportMeasurementConverter.FromCssPixelValue( 8 );

        foreach ( string selectedElementKey in GetSelectedElementKeysForResizeHitTest() )
        {
            if ( !ReportDefinitionHelper.TryFindElementLocation( definition, selectedElementKey, out int selectedSectionIndex, out _, out ReportElementDefinition element )
                || selectedSectionIndex != sectionIndex
                || element.Type == ReportElementType.Line )
            {
                continue;
            }

            double elementBottom = element.Y + ReportLayoutGeometry.GetElementRenderHeight( element );

            if ( Math.Abs( elementBottom - section.Height ) > handleTolerance )
                continue;

            double elementLeft = element.X;
            double elementRight = element.X + element.Width;

            if ( pointerX < elementLeft - handleTolerance || pointerX > elementRight + handleTolerance )
                continue;

            if ( Math.Abs( pointerX - elementLeft ) <= handleTolerance )
            {
                elementKey = selectedElementKey;
                handle = ReportElementResizeHandle.SouthWest;
                return true;
            }

            if ( Math.Abs( pointerX - elementRight ) <= handleTolerance )
            {
                elementKey = selectedElementKey;
                handle = ReportElementResizeHandle.SouthEast;
                return true;
            }

            elementKey = selectedElementKey;
            handle = ReportElementResizeHandle.South;
            return true;
        }

        return false;
    }

    private IEnumerable<string> GetSelectedElementKeysForResizeHitTest()
    {
        foreach ( string selectedElementKey in selectionManager.SelectedElementKeys )
        {
            yield return selectedElementKey;
        }
    }

    internal Task PreviewElementPointerInteraction( int targetSectionIndex, PointerEventArgs eventArgs )
    {
        if ( designerState.SelectionBox is not null )
            return PreviewSelectionBox( eventArgs );

        if ( designerState.SectionPointerResize is not null )
            return PreviewSectionPointerResize( eventArgs );

        if ( designerState.TablePointerResize is not null )
            return PreviewTablePointerResize( eventArgs );

        if ( designerState.ElementPointerResize is not null )
            return PreviewElementPointerResize( eventArgs );

        return PreviewElementPointerDrag( targetSectionIndex, eventArgs );
    }

    internal Task CompleteElementPointerInteraction( int targetSectionIndex, PointerEventArgs eventArgs )
    {
        if ( designerState.SelectionBox is not null )
            return CompleteSelectionBox( eventArgs );

        if ( designerState.SectionPointerResize is not null )
            return CompleteSectionPointerResize( eventArgs );

        if ( designerState.TablePointerResize is not null )
            return CompleteTablePointerResize( eventArgs );

        if ( designerState.ElementPointerResize is not null )
            return CompleteElementPointerResize( eventArgs );

        return CompleteElementPointerDrag( targetSectionIndex, eventArgs );
    }

    internal Task CancelElementPointerInteraction()
    {
        if ( designerState.SelectionBox is not null )
            return CancelSelectionBox();

        if ( designerState.SectionPointerResize is not null )
            return CancelSectionPointerResize();

        if ( designerState.TablePointerResize is not null )
            return CancelTablePointerResize();

        if ( designerState.ElementPointerResize is not null )
            return CancelElementPointerResize();

        return CancelElementPointerDrag();
    }

    private Task OnElementPointerCancel( PointerEventArgs eventArgs )
        => CancelElementPointerInteraction();

    internal Task BeginSelectionBox( int sectionIndex, PointerEventArgs eventArgs )
    {
        bool selectionBoxStarted = ReportDesignerInteractionService.TryBeginSelectionBox(
            designerState,
            EffectiveDefinition,
            sectionIndex,
            eventArgs,
            GetSectionOffsetY( EffectiveDefinition, sectionIndex ),
            GetDesignerContentHeight( EffectiveDefinition ) );

        if ( selectionBoxStarted )
        {
            _ = CloseContextMenu();
        }

        return Task.CompletedTask;
    }

    private async Task PreviewSelectionBox( PointerEventArgs eventArgs )
    {
        if ( designerState.SelectionBox is null )
            return;

        double previousX = designerState.SelectionBox.X;
        double previousY = designerState.SelectionBox.Y;
        double previousWidth = designerState.SelectionBox.Width;
        double previousHeight = designerState.SelectionBox.Height;

        ReportDesignerInteractionService.UpdateSelectionBox( designerState, EffectiveDefinition, eventArgs, GetDesignerContentHeight( EffectiveDefinition ) );

        if ( !ReportDesignerInteractionService.CanRenderSelectionBoxPreview( designerState, previousX, previousY, previousWidth, previousHeight ) )
            return;

        await UpdateDesignerSelectionOverlay();
    }

    internal Task PreviewPageSelectionBox( PointerEventArgs eventArgs )
    {
        return designerState.SelectionBox is null
            ? Task.CompletedTask
            : PreviewSelectionBox( eventArgs );
    }

    private async Task CompleteSelectionBox( PointerEventArgs eventArgs )
    {
        if ( designerState.SelectionBox is null )
            return;

        ReportDesignerInteractionService.UpdateSelectionBox( designerState, EffectiveDefinition, eventArgs, GetDesignerContentHeight( EffectiveDefinition ) );

        ReportDesignerSelectionBox completedSelectionBox = ReportDesignerInteractionService.CompleteSelectionBox( designerState );
        await ClearDesignerInteractionOverlays();

        if ( !completedSelectionBox.HasMoved )
        {
            await InvokeAsync( StateHasChanged );
            return;
        }

        var selectedKeys = FindElementsInsideSelectionBox( EffectiveDefinition, completedSelectionBox ).ToList();

        if ( completedSelectionBox.Additive )
            selectedKeys.InsertRange( 0, selectionManager.SelectedElementKeys );

        if ( selectedKeys.Count > 0 )
        {
            SelectElements( selectedKeys.Distinct( StringComparer.Ordinal ) );
        }
        else
        {
            SelectSection( completedSelectionBox.SectionIndex );
        }

        SuppressNextSelectionClick();

        await InvokeAsync( StateHasChanged );
    }

    internal Task CompletePageSelectionBox( PointerEventArgs eventArgs )
    {
        return designerState.SelectionBox is null
            ? Task.CompletedTask
            : CompleteSelectionBox( eventArgs );
    }

    private async Task CancelSelectionBox()
    {
        ReportDesignerInteractionService.CompleteSelectionBox( designerState );
        await ClearDesignerInteractionOverlays();

        await InvokeAsync( StateHasChanged );
    }

    internal Task CancelPageSelectionBox()
    {
        return designerState.SelectionBox is null
            ? Task.CompletedTask
            : CancelSelectionBox();
    }

    private Task OnPageSelectionPointerCancel( PointerEventArgs eventArgs )
        => CancelPageSelectionBox();

    private async Task PreviewElementPointerDrag( int targetSectionIndex, PointerEventArgs eventArgs )
    {
        if ( designerState.ElementPointerDrag is null || designerState.DraggedKind != ReportDesignerDragKind.Element )
            return;

        var preview = CreateElementPointerDragPreview( targetSectionIndex, eventArgs );

        if ( preview is null )
            return;

        var samePreviewPosition = designerState.DragPreview is not null
            && designerState.DragPreview.SectionIndex == preview.SectionIndex
            && Math.Abs( designerState.DragPreview.X - preview.X ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( designerState.DragPreview.Y - preview.Y ) < DesignerConstants.DragPreviewChangeTolerance;

        if ( samePreviewPosition )
            return;

        var now = DateTime.UtcNow;

        if ( !designerState.ElementPointerDrag.SnapToGrid
            && designerState.DragPreview is not null
            && designerState.DragPreview.SectionIndex == preview.SectionIndex
            && now - designerState.LastDragPreviewRenderTime < DesignerConstants.DragPreviewFrameThrottle )
        {
            return;
        }

        designerState.ElementPointerDrag.TargetSectionIndex = preview.SectionIndex;
        designerState.ElementPointerDrag.TargetX = preview.X;
        designerState.ElementPointerDrag.TargetY = preview.Y;
        designerState.ElementPointerDrag.HasMoved = true;
        designerState.DragPreview = preview;
        designerState.LastDragPreviewRenderTime = now;

        await UpdateDesignerDragOverlay( preview );
    }

    private async Task CompleteElementPointerDrag( int targetSectionIndex, PointerEventArgs eventArgs )
    {
        if ( designerState.ElementPointerDrag is null || designerState.DraggedKind != ReportDesignerDragKind.Element )
            return;

        var pointerDrag = designerState.ElementPointerDrag;
        var preview = CreateElementPointerDragPreview( targetSectionIndex, eventArgs ) ?? designerState.DragPreview;

        if ( preview is not null )
        {
            pointerDrag.TargetSectionIndex = preview.SectionIndex;
            pointerDrag.TargetX = preview.X;
            pointerDrag.TargetY = preview.Y;
        }

        var moved = pointerDrag.HasMoved
            && ( pointerDrag.TargetSectionIndex != pointerDrag.SourceSectionIndex
                || Math.Abs( pointerDrag.TargetX - pointerDrag.OriginalX ) > .1
                || Math.Abs( pointerDrag.TargetY - pointerDrag.OriginalY ) > .1 );

        var definition = EffectiveDefinition;
        var canMove = pointerDrag.SourceSectionIndex >= 0
            && pointerDrag.SourceSectionIndex < definition.Bands.Count
            && pointerDrag.TargetSectionIndex >= 0
            && pointerDrag.TargetSectionIndex < definition.Bands.Count
            && ReportDefinitionHelper.TryFindElementLocation( definition, pointerDrag.ElementKey, out _, out _, out _ );

        if ( !moved || !canMove )
        {
            ClearDragState();
            await ClearDesignerInteractionOverlays();
            await InvokeAsync( StateHasChanged );
            return;
        }

        await ClearDesignerInteractionOverlays();

        bool moveToTableCell = TryFindElementPointerDragTableCellTarget( definition, pointerDrag, out _ );

        await ExecuteDesignerCommand( new( moveToTableCell ? "Move element to table cell" : "Move element", () =>
        {
            var definition = EffectiveDefinition;

            if ( !TryMoveElementPointerDragToTableCell( definition, pointerDrag ) )
            {
                ReportDesignerInteractionService.ApplyElementPointerDrag( definition, pointerDrag, sectionIndex => GetSectionOffsetY( definition, sectionIndex ) );
                SelectElements( pointerDrag.SelectedElements.Select( item => item.ElementKey ), pointerDrag.ElementKey );
            }

            SuppressNextSelectionClick();
            designerState.DragPreview = null;
            ClearDragState();

            return Task.CompletedTask;
        }, RefreshSurface: false ) );
    }

    private bool TryMoveElementPointerDragToTableCell( ReportDefinition definition, ReportElementPointerDragState pointerDrag )
    {
        if ( !TryFindElementPointerDragTableCellTarget( definition, pointerDrag, out ReportTableCellDropTarget tableCellDropTarget )
             || !ReportDefinitionHelper.TryFindElementLocation( definition, pointerDrag.ElementKey, out ReportElementLocation location ) )
        {
            return false;
        }

        ReportElementDefinition element = location.Element;

        if ( ReferenceEquals( tableCellDropTarget.Table, element ) )
            return false;

        location.OwnerElements.RemoveAt( location.ElementIndex );
        element.X = tableCellDropTarget.X;
        element.Y = tableCellDropTarget.Y;
        tableEditor.ReplaceCellElement( tableCellDropTarget.Table, tableCellDropTarget.Cell, element );
        SelectTableCell( tableCellDropTarget.Cell.Id );

        return true;
    }

    private bool TryFindElementPointerDragTableCellTarget( ReportDefinition definition, ReportElementPointerDragState pointerDrag, out ReportTableCellDropTarget target )
    {
        target = null;

        if ( definition is null
             || pointerDrag is null
             || pointerDrag.SelectedElements.Count != 1
             || pointerDrag.TargetSectionIndex < 0
             || pointerDrag.TargetSectionIndex >= definition.Bands.Count )
        {
            return false;
        }

        double pointerX = pointerDrag.TargetX + pointerDrag.PointerOffsetX;
        double pointerY = pointerDrag.TargetY + pointerDrag.PointerOffsetY;

        if ( !tableEditor.TryFindCellAt( definition.Bands[pointerDrag.TargetSectionIndex], pointerX, pointerY, out target ) )
            return false;

        return !ReportDefinitionHelper.TryFindElementLocation( definition, pointerDrag.ElementKey, out ReportElementLocation location )
            || !ReferenceEquals( target.Table, location.Element );
    }

    private async Task CancelElementPointerDrag()
    {
        if ( designerState.ElementPointerDrag is null )
            return;

        ClearDragState();
        await ClearDesignerInteractionOverlays();

        await InvokeAsync( StateHasChanged );
    }

    private async Task PreviewTablePointerResize( PointerEventArgs eventArgs )
    {
        if ( designerState.TablePointerResize is null )
            return;

        ReportDesignerDragPreview preview = CreateTablePointerResizePreview( eventArgs );

        if ( preview is null )
            return;

        bool samePreviewSize = designerState.DragPreview is not null
            && Math.Abs( designerState.DragPreview.X - preview.X ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( designerState.DragPreview.Y - preview.Y ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( designerState.DragPreview.Width - preview.Width ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( designerState.DragPreview.Height - preview.Height ) < DesignerConstants.DragPreviewChangeTolerance;

        if ( samePreviewSize )
            return;

        DateTime now = DateTime.UtcNow;

        if ( !designerState.TablePointerResize.SnapToGrid
            && designerState.DragPreview is not null
            && now - designerState.LastDragPreviewRenderTime < DesignerConstants.DragPreviewFrameThrottle )
        {
            return;
        }

        designerState.TablePointerResize.TargetSize = ResolveTablePointerResizeTargetSize( eventArgs );
        designerState.TablePointerResize.HasResized = true;
        designerState.DragPreview = preview;
        designerState.LastDragPreviewRenderTime = now;

        await UpdateDesignerDragOverlay( preview );
    }

    private async Task CompleteTablePointerResize( PointerEventArgs eventArgs )
    {
        if ( designerState.TablePointerResize is null )
            return;

        ReportTablePointerResizeState pointerResize = designerState.TablePointerResize;
        pointerResize.TargetSize = ResolveTablePointerResizeTargetSize( eventArgs );

        bool resized = pointerResize.HasResized
            && Math.Abs( pointerResize.TargetSize - pointerResize.OriginalSize ) > .1;

        if ( !resized
            || !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, pointerResize.TableKey, out _, out _, out ReportElementDefinition element )
            || element is not ReportTableElementDefinition table )
        {
            ClearDragState();
            await ClearDesignerInteractionOverlays();
            await InvokeAsync( StateHasChanged );
            return;
        }

        await ClearDesignerInteractionOverlays();

        await ExecuteDesignerCommand( new( pointerResize.Kind == ReportTableResizeKind.Column ? "Resize table column" : "Resize table row", () =>
        {
            if ( ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, pointerResize.TableKey, out int sectionIndex, out _, out ReportElementDefinition element )
                && element is ReportTableElementDefinition table )
            {
                ApplyTablePointerResize( table, pointerResize );
                ReportLayoutGeometry.GrowSectionToFitElement( EffectiveDefinition.Bands[sectionIndex], table );

                if ( !string.IsNullOrWhiteSpace( pointerResize.CellKey ) )
                    SelectTableCell( pointerResize.CellKey );
                else
                    SelectElement( pointerResize.TableKey );
            }

            designerState.DragPreview = null;
            ClearDragState();

            return Task.CompletedTask;
        }, RefreshSurface: false ) );
    }

    private async Task CancelTablePointerResize()
    {
        if ( designerState.TablePointerResize is null )
            return;

        ClearDragState();
        await ClearDesignerInteractionOverlays();

        await InvokeAsync( StateHasChanged );
    }

    private ReportDesignerDragPreview CreateTablePointerResizePreview( PointerEventArgs eventArgs )
    {
        if ( designerState.TablePointerResize is null
            || !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, designerState.TablePointerResize.TableKey, out _, out _, out ReportElementDefinition element )
            || element is not ReportTableElementDefinition table )
        {
            return null;
        }

        tableEditor.EnsureGrid( table );

        double targetSize = ResolveTablePointerResizeTargetSize( eventArgs );

        return tableResizeService.CreatePreview( table, designerState.TablePointerResize, targetSize, tableEditor );
    }

    private double ResolveTablePointerResizeTargetSize( PointerEventArgs eventArgs )
    {
        return tableResizeService.ResolveTargetSize( designerState.TablePointerResize, eventArgs.ClientX, eventArgs.ClientY, ApplyDesignerGrid );
    }

    private void ApplyTablePointerResize( ReportTableElementDefinition table, ReportTablePointerResizeState pointerResize )
    {
        tableResizeService.ApplyResize( table, pointerResize, tableEditor );
    }

    private ReportDesignerDragPreview CreateElementPointerDragPreview( int targetSectionIndex, PointerEventArgs eventArgs )
    {
        return ReportDesignerInteractionService.CreateElementDragPreview(
            EffectiveDefinition,
            designerState.ElementPointerDrag,
            designerState.DraggedElement,
            targetSectionIndex,
            eventArgs.ClientX,
            eventArgs.ClientY,
            sectionIndex => GetSectionOffsetY( EffectiveDefinition, sectionIndex ),
            value => ApplyDesignerGrid( value, designerState.ElementPointerDrag?.SnapToGrid ?? designerState.SnapToGrid ) );
    }

    private async Task PreviewElementPointerResize( PointerEventArgs eventArgs )
    {
        await PreviewElementPointerResize( eventArgs.ClientX, eventArgs.ClientY );
    }

    private async Task PreviewElementPointerResize( double clientX, double clientY )
    {
        if ( designerState.ElementPointerResize is null || designerState.DraggedElement is null || designerState.DraggedKind != ReportDesignerDragKind.Element )
            return;

        var preview = CreateElementPointerResizePreview( clientX, clientY );

        if ( preview is null )
            return;

        var samePreviewSize = designerState.DragPreview is not null
            && Math.Abs( designerState.DragPreview.X - preview.X ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( designerState.DragPreview.Y - preview.Y ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( designerState.DragPreview.Width - preview.Width ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( designerState.DragPreview.Height - preview.Height ) < DesignerConstants.DragPreviewChangeTolerance;

        if ( samePreviewSize )
            return;

        var now = DateTime.UtcNow;

        if ( !designerState.ElementPointerResize.SnapToGrid
            && designerState.DragPreview is not null
            && now - designerState.LastDragPreviewRenderTime < DesignerConstants.DragPreviewFrameThrottle )
        {
            return;
        }

        designerState.ElementPointerResize.TargetX = preview.X;
        designerState.ElementPointerResize.TargetY = preview.Y;
        designerState.ElementPointerResize.TargetWidth = preview.Width;
        designerState.ElementPointerResize.TargetHeight = preview.Height;
        designerState.ElementPointerResize.HasResized = true;
        designerState.DragPreview = preview;
        designerState.LastDragPreviewRenderTime = now;

        await UpdateDesignerDragOverlay( preview );
    }

    private async Task CompleteElementPointerResize( PointerEventArgs eventArgs )
    {
        await CompleteElementPointerResize( eventArgs.ClientX, eventArgs.ClientY );
    }

    private async Task CompleteElementPointerResize( double clientX, double clientY )
    {
        if ( designerState.ElementPointerResize is null || designerState.DraggedKind != ReportDesignerDragKind.Element )
            return;

        var pointerResize = designerState.ElementPointerResize;
        var preview = CreateElementPointerResizePreview( clientX, clientY ) ?? designerState.DragPreview;

        if ( preview is not null )
        {
            pointerResize.TargetX = preview.X;
            pointerResize.TargetY = preview.Y;
            pointerResize.TargetWidth = preview.Width;
            pointerResize.TargetHeight = preview.Height;
        }

        var resized = pointerResize.HasResized
            && ( Math.Abs( pointerResize.TargetX - pointerResize.OriginalX ) > .1
                || Math.Abs( pointerResize.TargetY - pointerResize.OriginalY ) > .1
                || Math.Abs( pointerResize.TargetWidth - pointerResize.OriginalWidth ) > .1
                || Math.Abs( pointerResize.TargetHeight - pointerResize.OriginalHeight ) > .1 );

        if ( !resized || !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, pointerResize.ElementKey, out _, out _, out _ ) )
        {
            ClearDragState();
            await ClearDesignerInteractionOverlays();
            await InvokeAsync( StateHasChanged );
            return;
        }

        await ClearDesignerInteractionOverlays();

        await ExecuteDesignerCommand( new( "Resize element", () =>
        {
            ReportDesignerInteractionService.ApplyElementPointerResize( EffectiveDefinition, pointerResize );

            foreach ( ReportElementPointerItemState item in pointerResize.SelectedElements )
            {
                if ( ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, item.ElementKey, out _, out _, out ReportElementDefinition resizedElement ) )
                    if ( resizedElement is ReportTableElementDefinition resizedTable )
                        ReportDefinitionHelper.ScaleTableLayout( resizedTable, item.OriginalWidth, item.OriginalHeight );
            }

            SelectElements( pointerResize.SelectedElements.Select( item => item.ElementKey ), pointerResize.ElementKey );
            SuppressNextSelectionClick();
            designerState.DragPreview = null;
            ClearDragState();

            return Task.CompletedTask;
        }, RefreshSurface: false ) );
    }

    private async Task CancelElementPointerResize()
    {
        if ( designerState.ElementPointerResize is null )
            return;

        ClearDragState();
        await ClearDesignerInteractionOverlays();

        await InvokeAsync( StateHasChanged );
    }

    private async Task PreviewSectionPointerResize( PointerEventArgs eventArgs )
    {
        await PreviewSectionPointerResize( eventArgs.ClientY );
    }

    private async Task PreviewSectionPointerResize( double clientY )
    {
        if ( designerState.SectionPointerResize is null )
            return;

        var height = CreateSectionPointerResizeHeight( clientY );

        if ( Math.Abs( designerState.SectionPointerResize.TargetHeight - height ) < DesignerConstants.DragPreviewChangeTolerance )
            return;

        designerState.SectionPointerResize.TargetHeight = height;

        await UpdateDesignerSectionResizePreview( designerState.SectionPointerResize );
    }

    private async Task CompleteSectionPointerResize( PointerEventArgs eventArgs )
    {
        await CompleteSectionPointerResize( eventArgs.ClientY );
    }

    private async Task CompleteSectionPointerResize( double clientY )
    {
        if ( designerState.SectionPointerResize is null )
            return;

        var pointerResize = designerState.SectionPointerResize;
        pointerResize.TargetHeight = CreateSectionPointerResizeHeight( pointerResize, clientY );
        designerState.SectionPointerResize = null;

        try
        {
            var resized = Math.Abs( pointerResize.TargetHeight - pointerResize.OriginalHeight ) > .1;

            var definition = EffectiveDefinition;
            var canResize = pointerResize.SectionIndex >= 0
                && pointerResize.SectionIndex < definition.Bands.Count
                && !ReportValueResolver.ResolveStaticSuppress( definition.Bands[pointerResize.SectionIndex] );

            if ( !resized || !canResize )
                return;

            await ExecuteDesignerCommand( new( "Resize band", () =>
            {
                var definition = EffectiveDefinition;

                if ( pointerResize.SectionIndex >= 0
                    && pointerResize.SectionIndex < definition.Bands.Count
                    && !ReportValueResolver.ResolveStaticSuppress( definition.Bands[pointerResize.SectionIndex] ) )
                {
                    definition.Bands[pointerResize.SectionIndex].Height = pointerResize.TargetHeight;
                    SelectSection( pointerResize.SectionIndex );
                }

                return Task.CompletedTask;
            }, RefreshSurface: false ) );
        }
        finally
        {
            await CommitDesignerSectionResizePreview();
            await InvokeAsync( StateHasChanged );
        }
    }

    private async Task CancelSectionPointerResize()
    {
        if ( designerState.SectionPointerResize is null )
            return;

        designerState.SectionPointerResize = null;
        await ClearDesignerSectionResizePreview();

        await InvokeAsync( StateHasChanged );
    }

    private double CreateSectionPointerResizeHeight( double clientY )
    {
        if ( designerState.SectionPointerResize is null )
            return 0;

        return CreateSectionPointerResizeHeight( designerState.SectionPointerResize, clientY );
    }

    private double CreateSectionPointerResizeHeight( ReportSectionPointerResizeState pointerResize, double clientY )
    {
        var section = pointerResize.SectionIndex >= 0 && pointerResize.SectionIndex < EffectiveDefinition.Bands.Count
            ? EffectiveDefinition.Bands[pointerResize.SectionIndex]
            : null;

        return ReportDesignerInteractionService.CreateSectionResizeHeight( pointerResize, clientY, GetMinimumSectionHeight( section ), ApplyDesignerGrid );
    }

    /// <summary>
    /// Previews a document-level band resize while the pointer is moving.
    /// </summary>
    /// <param name="clientY">Current document pointer Y coordinate.</param>
    [JSInvokable]
    public Task OnDocumentSectionResizeMove( double clientY )
    {
        return InvokeAsync( () => PreviewSectionPointerResize( clientY ) );
    }

    /// <summary>
    /// Completes a document-level band resize and commits the final band height.
    /// </summary>
    /// <param name="clientY">Final document pointer Y coordinate.</param>
    [JSInvokable]
    public Task OnDocumentSectionResizeEnd( double clientY )
    {
        return InvokeAsync( () => CompleteSectionPointerResize( clientY ) );
    }

    /// <summary>
    /// Cancels the active document-level band resize.
    /// </summary>
    [JSInvokable]
    public Task OnDocumentSectionResizeCancel()
    {
        return InvokeAsync( CancelSectionPointerResize );
    }

    /// <summary>
    /// Previews a document-level element resize while the pointer is moving.
    /// </summary>
    /// <param name="clientX">Current document pointer X coordinate.</param>
    /// <param name="clientY">Current document pointer Y coordinate.</param>
    [JSInvokable]
    public Task OnDocumentElementResizeMove( double clientX, double clientY )
    {
        return InvokeAsync( () => PreviewElementPointerResize( clientX, clientY ) );
    }

    /// <summary>
    /// Completes a document-level element resize and commits the final element size.
    /// </summary>
    /// <param name="clientX">Final document pointer X coordinate.</param>
    /// <param name="clientY">Final document pointer Y coordinate.</param>
    [JSInvokable]
    public Task OnDocumentElementResizeEnd( double clientX, double clientY )
    {
        return InvokeAsync( () => CompleteElementPointerResize( clientX, clientY ) );
    }

    /// <summary>
    /// Cancels the active document-level element resize.
    /// </summary>
    [JSInvokable]
    public Task OnDocumentElementResizeCancel()
    {
        return InvokeAsync( CancelElementPointerResize );
    }

    private async Task StartDocumentSectionResize( double startClientY, long pointerId )
    {
        EnsureReportingModule();
        dotNetObjectReference ??= DotNetObjectReference.Create( this );

        await DocumentObserver.EnsureInitializedAsync();
        await reportingModule.StartSectionResize( dotNetObjectReference, startClientY, pointerId );
    }

    private async Task StartDocumentElementResize( double startClientX, double startClientY, long pointerId )
    {
        EnsureReportingModule();
        dotNetObjectReference ??= DotNetObjectReference.Create( this );

        await DocumentObserver.EnsureInitializedAsync();
        await reportingModule.StartElementResize( dotNetObjectReference, startClientX, startClientY, pointerId );
    }

    private void EnsureReportingModule()
    {
        reportingModule ??= new( JSRuntime, VersionProvider, BlazoriseOptions );
    }

    private async Task UpdateDesignerSelectionOverlay()
    {
        if ( designerState.SelectionBox is null )
            return;

        EnsureReportingModule();

        await reportingModule.UpdateDesignerSelectionOverlay(
            designerPageRef.Element,
            ReportMeasurementConverter.ToCssPixelValue( designerState.SelectionBox.X ) + GetSelectionBoxLeftOffset(),
            ReportMeasurementConverter.ToCssPixelValue( designerState.SelectionBox.Y + GetDesignerSectionBodyTopOffset() ),
            ReportMeasurementConverter.ToCssPixelValue( designerState.SelectionBox.Width ),
            ReportMeasurementConverter.ToCssPixelValue( designerState.SelectionBox.Height ) );
    }

    private async Task UpdateDesignerDragOverlay( ReportDesignerDragPreview preview )
    {
        if ( preview is null )
            return;

        EnsureReportingModule();

        await reportingModule.UpdateDesignerDragOverlay(
            designerPageRef.Element,
            preview.ElementType.ToString(),
            preview.Text,
            ReportMeasurementConverter.ToCssPixelValue( preview.X ) + GetSelectionBoxLeftOffset(),
            ReportMeasurementConverter.ToCssPixelValue( GetElementPageY( EffectiveDefinition, preview.SectionIndex, preview.Y ) ),
            ReportMeasurementConverter.ToCssPixelValue( preview.Width ),
            ReportMeasurementConverter.ToCssPixelValue( preview.Height ) );
    }

    private async Task ClearDesignerInteractionOverlays()
    {
        if ( reportingModule is null )
            return;

        await reportingModule.ClearDesignerInteractionOverlays( designerPageRef.Element );
    }

    private async Task UpdateDesignerSectionResizePreview( ReportSectionPointerResizeState pointerResize )
    {
        if ( pointerResize is null || EffectiveDefinition is null || pointerResize.SectionIndex < 0 || pointerResize.SectionIndex >= EffectiveDefinition.Bands.Count )
            return;

        EnsureReportingModule();

        string sectionId = ReportDefinitionHelper.EnsureBandId( EffectiveDefinition.Bands[pointerResize.SectionIndex] );
        double sectionOffsetY = GetSectionOffsetY( EffectiveDefinition, pointerResize.SectionIndex );
        double sectionHeight = GetDesignerSectionHeight( pointerResize.SectionIndex, EffectiveDefinition.Bands[pointerResize.SectionIndex] );

        await reportingModule.UpdateDesignerSectionResizePreview(
            designerPageRef.Element,
            sectionId,
            ReportMeasurementConverter.ToCssPixelValue( sectionHeight ),
            ReportMeasurementConverter.ToCssPixelValue( sectionOffsetY ) );
    }

    private async Task ClearDesignerSectionResizePreview()
    {
        if ( reportingModule is null )
            return;

        await reportingModule.ClearDesignerSectionResizePreview( designerPageRef.Element );
    }

    private async Task CommitDesignerSectionResizePreview()
    {
        if ( reportingModule is null )
            return;

        await reportingModule.CommitDesignerSectionResizePreview( designerPageRef.Element );
    }

    private ReportDesignerDragPreview CreateElementPointerResizePreview( double clientX, double clientY )
    {
        return ReportDesignerInteractionService.CreateElementResizePreview(
            EffectiveDefinition,
            designerState.ElementPointerResize,
            designerState.DraggedElement,
            clientX,
            clientY,
            value => ApplyDesignerGrid( value, designerState.ElementPointerResize?.SnapToGrid ?? designerState.SnapToGrid ) );
    }

    internal async Task PreviewDesignerDrag( int targetSectionIndex, ElementReference sectionBodyElement, DragEventArgs eventArgs )
    {
        if ( designerState.DraggedKind == ReportDesignerDragKind.None )
            return;

        var offset = await GetDesignerDragOffset( sectionBodyElement, eventArgs );
        bool useSnapToGrid = designerState.DraggedKind == ReportDesignerDragKind.Element
            ? IsSnapToGridEnabled( designerState.DraggedElement )
            : designerState.SnapToGrid;
        var preview = CreateDragPreview( targetSectionIndex, ApplyDesignerGrid( offset.X, useSnapToGrid ), ApplyDesignerGrid( offset.Y, useSnapToGrid ) );

        if ( preview is null )
            return;

        var samePreviewPosition = designerState.DragPreview is not null
            && designerState.DragPreview.SectionIndex == preview.SectionIndex
            && Math.Abs( designerState.DragPreview.X - preview.X ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( designerState.DragPreview.Y - preview.Y ) < DesignerConstants.DragPreviewChangeTolerance;

        if ( samePreviewPosition )
        {
            return;
        }

        var now = DateTime.UtcNow;

        if ( !designerState.SnapToGrid
            && designerState.DragPreview is not null
            && designerState.DragPreview.SectionIndex == preview.SectionIndex
            && now - designerState.LastDragPreviewRenderTime < DesignerConstants.DragPreviewFreeDropThrottle )
        {
            return;
        }

        designerState.DragPreview = preview;
        designerState.LastDragPreviewRenderTime = now;

        await UpdateDesignerDragOverlay( preview );
    }

    private async Task<(double X, double Y)> GetDesignerDragOffset( ElementReference sectionBodyElement, DragEventArgs eventArgs )
    {
        EnsureReportingModule();

        var offset = await reportingModule.GetElementOffset( sectionBodyElement, eventArgs.ClientX, eventArgs.ClientY );

        return offset is { Length: >= 2 }
            ? (Math.Max( 0, ReportMeasurementConverter.FromCssPixelValue( offset[0] ) ), Math.Max( 0, ReportMeasurementConverter.FromCssPixelValue( offset[1] ) ))
            : (Math.Max( 0, ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetX ) ), Math.Max( 0, ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetY ) ));
    }

    internal async Task DropDesignerItem( int targetSectionIndex, ElementReference sectionBodyElement, DragEventArgs eventArgs )
    {
        var definition = EffectiveDefinition;

        if ( targetSectionIndex < 0 || targetSectionIndex >= definition.Bands.Count )
            return;

        designerDropInProgress = true;

        try
        {
            var offset = await GetDesignerDragOffset( sectionBodyElement, eventArgs );
            bool useSnapToGrid = designerState.DraggedKind == ReportDesignerDragKind.Element
                ? IsSnapToGridEnabled( designerState.DraggedElement )
                : designerState.SnapToGrid;
            var x = ApplyDesignerGrid( offset.X, useSnapToGrid );
            var y = ApplyDesignerGrid( offset.Y, useSnapToGrid );
            var tableDropTarget = tableEditor.TryFindCellAt( definition.Bands[targetSectionIndex], x, y, out ReportTableCellDropTarget cellDropTarget )
                ? cellDropTarget
                : null;
            var fieldDropTarget = designerState.DraggedKind == ReportDesignerDragKind.Field
                ? dragDropService.FindTextElementAt( definition.Bands[targetSectionIndex], x, y )
                : null;

            var commandName = dragDropService.ResolveCommandName( definition, designerState, tableDropTarget, fieldDropTarget );

            if ( commandName is null )
                return;

            await ClearDesignerInteractionOverlays();

            await ExecuteDesignerCommand( new( commandName, () =>
            {
                var definition = EffectiveDefinition;
                var targetSection = definition.Bands[targetSectionIndex];
                tableEditor.TryFindCellAt( targetSection, x, y, out ReportTableCellDropTarget tableCellDropTarget );
                ReportDropResult result = dragDropService.Drop( definition, designerState, targetSectionIndex, x, y, tableCellDropTarget, tableEditor );

                if ( !string.IsNullOrWhiteSpace( result.SelectedCellKey ) )
                    SelectTableCell( result.SelectedCellKey );
                else if ( result.SelectedElementKeys.Count > 0 )
                    SelectElements( result.SelectedElementKeys, result.PrimaryElementKey );

                selectionManager.SelectedSectionIndex = null;
                designerState.DragPreview = null;
                ClearDragState();

                return Task.CompletedTask;
            } ) );
        }
        finally
        {
            designerDropInProgress = false;

            if ( designerState.DraggedKind != ReportDesignerDragKind.None || designerState.DragPreview is not null )
                await ClearDesignerDrag();
        }
    }

    private ReportDesignerDragPreview CreateDragPreview( int targetSectionIndex, double x, double y )
    {
        return dragDropService.CreateDragPreview( EffectiveDefinition, designerState, targetSectionIndex, x, y );
    }

    internal async Task ClearDesignerDrag()
    {
        var requiresRender = designerState.DraggedKind != ReportDesignerDragKind.None || designerState.DragPreview is not null;

        ClearDragState();
        await ClearDesignerInteractionOverlays();

        if ( requiresRender )
            await InvokeAsync( StateHasChanged );
    }

    internal Task CompleteExternalDrag()
    {
        return designerDropInProgress
            ? Task.CompletedTask
            : ClearDesignerDrag();
    }

    private double ApplyDesignerGrid( double value )
        => Designer.ApplyDesignerGrid( value );

    private double ApplyDesignerGrid( double value, bool useSnapToGrid )
        => Designer.ApplyDesignerGrid( value, useSnapToGrid );

    private IEnumerable<ReportElementPointerItemState> CaptureElementPointerItems( ReportDefinition definition, string activeElementKey )
        => Designer.CaptureElementPointerItems( definition, activeElementKey );

    private void ClearDragState()
        => Designer.ClearDragState();

    private Task CloseContextMenu()
        => Designer.CloseContextMenu();

    private Task ExecuteDesignerCommand( ReportDesignerCommand command )
        => Designer.ExecuteDesignerCommand( command );

    private IEnumerable<string> FindElementsInsideSelectionBox( ReportDefinition definition, ReportDesignerSelectionBox selectionBox )
        => Designer.FindElementsInsideSelectionBox( definition, selectionBox );

    private double GetDesignerContentHeight( ReportDefinition definition )
        => Designer.GetDesignerContentHeight( definition );

    private double GetDesignerSectionBodyTopOffset()
        => Designer.GetDesignerSectionBodyTopOffset();

    private double GetDesignerSectionHeight( int sectionIndex, ReportBandDefinition section )
        => Designer.GetDesignerSectionHeight( sectionIndex, section );

    private double GetElementPageY( ReportDefinition definition, int sectionIndex, double elementY )
        => Designer.GetElementPageY( definition, sectionIndex, elementY );

    private static double GetMinimumSectionHeight( ReportBandDefinition section )
        => _ReportDesigner.GetMinimumSectionHeight( section );

    private double GetSectionOffsetY( ReportDefinition definition, int sectionIndex )
        => Designer.GetSectionOffsetY( definition, sectionIndex );

    private double GetSelectionBoxLeftOffset()
        => Designer.GetSelectionBoxLeftOffset();

    private bool IsSnapToGridEnabled( ReportElementDefinition element )
        => Designer.IsSnapToGridEnabled( element );

    private void SelectElement( string key, bool preserveSelection = false )
        => Designer.SelectElement( key, preserveSelection );

    private void SelectElements( IEnumerable<string> elementKeys, string primaryElementKey = null )
        => Designer.SelectElements( elementKeys, primaryElementKey );

    private void SelectSection( int sectionIndex )
        => Designer.SelectSection( sectionIndex );

    private void SelectTableCell( string cellKey )
        => Designer.SelectTableCell( cellKey );

    private void SuppressNextSelectionClick()
        => Designer.SuppressNextSelectionClick();

    private void ToggleElementSelection( string key )
        => Designer.ToggleElementSelection( key );

    #endregion

    #region Properties

    private ReportDesignerInteractionState designerState => Designer.InteractionState;

    private ReportSelectionManager selectionManager => Designer.Selection;

    private ReportTableEditor tableEditor => Designer.TableEditor;

    private ReportDefinition EffectiveDefinition => Definition;

    private bool CanInsertSubreportElement => Designer.CanInsertSubreport;

    /// <summary>
    /// Owning report designer.
    /// </summary>
    [Parameter, EditorRequired] public _ReportDesigner Designer { get; set; }

    [Parameter, EditorRequired] public ReportDefinition Definition { get; set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Inject] private IVersionProvider VersionProvider { get; set; }

    [Inject] private BlazoriseOptions BlazoriseOptions { get; set; }

    [Inject] private IDocumentObserver DocumentObserver { get; set; }

    #endregion
}