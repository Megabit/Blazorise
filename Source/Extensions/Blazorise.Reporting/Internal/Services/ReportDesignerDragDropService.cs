#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDesignerDragDropService
{
    #region Methods

    internal string ResolveCommandName( ReportDefinition definition, ReportDesignerInteractionState state, ReportTableCellDropTarget tableDropTarget, ReportPanelDropTarget panelDropTarget, ReportElementDefinition textDropTarget )
    {
        return state.DraggedKind switch
        {
            ReportDesignerDragKind.Field when !string.IsNullOrWhiteSpace( state.DraggedFieldName ) && tableDropTarget is not null => "Add field to table cell",
            ReportDesignerDragKind.Field when !string.IsNullOrWhiteSpace( state.DraggedFieldName ) && panelDropTarget is not null => "Add field to panel",
            ReportDesignerDragKind.Field when !string.IsNullOrWhiteSpace( state.DraggedFieldName ) && textDropTarget is not null => "Insert field into text",
            ReportDesignerDragKind.Field when !string.IsNullOrWhiteSpace( state.DraggedFieldName ) => "Add field",
            ReportDesignerDragKind.ToolboxElement when state.DraggedElementType is not null && tableDropTarget is not null => "Add element to table cell",
            ReportDesignerDragKind.ToolboxElement when state.DraggedElementType is not null && panelDropTarget is not null => "Add element to panel",
            ReportDesignerDragKind.ToolboxElement when state.DraggedElementType is not null => "Add element",
            ReportDesignerDragKind.Element when tableDropTarget is not null && ReportDefinitionHelper.TryFindElementLocation( definition, state.DraggedElementKey, out _, out _, out _ ) => "Move element to table cell",
            ReportDesignerDragKind.Element when panelDropTarget is not null && ReportDefinitionHelper.TryFindElementLocation( definition, state.DraggedElementKey, out _, out _, out _ ) => "Move element to panel",
            ReportDesignerDragKind.Element when ReportDefinitionHelper.TryFindElementLocation( definition, state.DraggedElementKey, out _, out _, out _ ) => "Move element",
            _ => null,
        };
    }

    internal ReportDropResult Drop(
        ReportDefinition definition,
        ReportDesignerInteractionState state,
        int targetSectionIndex,
        double x,
        double y,
        ReportTableCellDropTarget tableCellDropTarget,
        ReportPanelDropTarget panelDropTarget,
        ReportTableEditor tableEditor )
    {
        if ( definition is null || state is null || targetSectionIndex < 0 || targetSectionIndex >= definition.Bands.Count )
            return new();

        ReportBandDefinition targetSection = definition.Bands[targetSectionIndex];

        return state.DraggedKind switch
        {
            ReportDesignerDragKind.Field when !string.IsNullOrWhiteSpace( state.DraggedFieldName )
                => DropField( definition, state, targetSectionIndex, targetSection, x, y, tableCellDropTarget, panelDropTarget, tableEditor ),
            ReportDesignerDragKind.ToolboxElement when state.DraggedElementType is not null
                => DropToolboxElement( state, targetSection, x, y, tableCellDropTarget, panelDropTarget, tableEditor ),
            ReportDesignerDragKind.Element when ReportDefinitionHelper.TryFindElementLocation( definition, state.DraggedElementKey, out ReportElementLocation location )
                => DropElement( definition, targetSectionIndex, targetSection, x, y, tableCellDropTarget, panelDropTarget, tableEditor, location ),
            _ => new(),
        };
    }

    internal ReportElementDefinition FindTextElementAt( ReportBandDefinition section, double x, double y )
    {
        if ( section is null )
            return null;

        for ( int i = section.Elements.Count - 1; i >= 0; i-- )
        {
            ReportElementDefinition element = section.Elements[i];

            if ( element.Suppress?.Value != true
                && element is ReportTextElementDefinition
                && x >= element.X
                && x <= element.X + element.Width
                && y >= element.Y
                && y <= element.Y + element.Height )
            {
                return element;
            }
        }

        return null;
    }

    internal bool TryFindPanelAt( ReportBandDefinition section, double x, double y, ReportElementDefinition excludedElement, out ReportPanelDropTarget target )
    {
        target = null;

        return section?.Elements is not null
            && TryFindPanelAt( section.Elements, x, y, 0, 0, excludedElement, out target );
    }

    internal ReportDesignerDragPreview CreateDragPreview( ReportDefinition definition, ReportDesignerInteractionState state, int targetSectionIndex, double x, double y )
    {
        return state.DraggedKind switch
        {
            ReportDesignerDragKind.Field when !string.IsNullOrWhiteSpace( state.DraggedFieldName ) => CreateFieldDragPreview( definition, state, targetSectionIndex, x, y ),
            ReportDesignerDragKind.ToolboxElement when state.DraggedElementType is not null => CreateDragPreview( definition, targetSectionIndex, ReportDefinitionHelper.CreateElementFromToolbox( state.DraggedElementType.Value, state.DraggedElementText, x, y ) ),
            ReportDesignerDragKind.Element when state.DraggedElement is not null => CreateDragPreview( definition, targetSectionIndex, state.DraggedElement, x, y ),
            _ => null,
        };
    }

    private ReportDropResult DropField(
        ReportDefinition definition,
        ReportDesignerInteractionState state,
        int targetSectionIndex,
        ReportBandDefinition targetSection,
        double x,
        double y,
        ReportTableCellDropTarget tableCellDropTarget,
        ReportPanelDropTarget panelDropTarget,
        ReportTableEditor tableEditor )
    {
        (string DataSourceName, string FieldName) fieldBinding = ReportDefinitionHelper.NormalizeFieldBindingForSection( definition, targetSection, state.DraggedDataSourceName, state.DraggedFieldName );

        ReportElementDefinition fieldElement = new ReportFieldElementDefinition
        {
            Name = fieldBinding.FieldName,
            Field = fieldBinding.FieldName,
            DataSource = fieldBinding.DataSourceName,
            X = tableCellDropTarget?.X ?? x,
            Y = tableCellDropTarget?.Y ?? y,
            Width = ReportDesignerConstants.DefaultDroppedFieldWidth,
            Height = ReportDesignerConstants.DefaultDroppedFieldHeight,
        };

        if ( tableCellDropTarget is not null )
        {
            tableEditor.ReplaceCellElement( tableCellDropTarget.Table, tableCellDropTarget.Cell, fieldElement );

            return new()
            {
                SelectedCellKey = tableCellDropTarget.Cell.Id,
            };
        }

        if ( panelDropTarget is not null )
        {
            ReportDefinitionHelper.PlaceElementInPanel( panelDropTarget.Panel, fieldElement, panelDropTarget.X, panelDropTarget.Y );
            panelDropTarget.Panel.Elements.Add( fieldElement );

            return SelectElement( ReportDefinitionHelper.EnsureElementId( fieldElement ) );
        }

        ReportElementDefinition textDropTarget = FindTextElementAt( targetSection, x, y );

        if ( textDropTarget is not null )
        {
            ReportExpressionFormatter.AppendFieldExpressionToText( textDropTarget, fieldBinding.DataSourceName, fieldBinding.FieldName );

            return SelectElement( ReportDefinitionHelper.EnsureElementId( textDropTarget ) );
        }

        targetSection.Elements.Add( fieldElement );

        if ( !ReportSpecialFieldResolver.IsSpecialDataSource( fieldBinding.DataSourceName )
            && !ReportFormulaFieldResolver.IsFormulaDataSource( fieldBinding.DataSourceName )
            && !ReportRunningTotalResolver.IsRunningTotalDataSource( fieldBinding.DataSourceName ) )
            ReportDetailHeaderSynchronizer.AddPageHeaderForDetailField( definition, targetSectionIndex, targetSection, fieldBinding.FieldName, x, fieldElement.Width );

        return SelectElement( ReportDefinitionHelper.EnsureElementId( fieldElement ) );
    }

    private static ReportDropResult DropToolboxElement(
        ReportDesignerInteractionState state,
        ReportBandDefinition targetSection,
        double x,
        double y,
        ReportTableCellDropTarget tableCellDropTarget,
        ReportPanelDropTarget panelDropTarget,
        ReportTableEditor tableEditor )
    {
        ReportElementDefinition toolboxElement = ReportDefinitionHelper.CreateElementFromToolbox( state.DraggedElementType.Value, state.DraggedElementText, tableCellDropTarget?.X ?? x, tableCellDropTarget?.Y ?? y );

        if ( tableCellDropTarget is not null )
        {
            tableEditor.ReplaceCellElement( tableCellDropTarget.Table, tableCellDropTarget.Cell, toolboxElement );

            return new()
            {
                SelectedCellKey = tableCellDropTarget.Cell.Id,
            };
        }

        if ( panelDropTarget is not null )
        {
            ReportDefinitionHelper.PlaceElementInPanel( panelDropTarget.Panel, toolboxElement, panelDropTarget.X, panelDropTarget.Y );
            panelDropTarget.Panel.Elements.Add( toolboxElement );

            return SelectElement( ReportDefinitionHelper.EnsureElementId( toolboxElement ) );
        }

        targetSection.Elements.Add( toolboxElement );

        return SelectElement( ReportDefinitionHelper.EnsureElementId( toolboxElement ) );
    }

    private static ReportDropResult DropElement(
        ReportDefinition definition,
        int targetSectionIndex,
        ReportBandDefinition targetSection,
        double x,
        double y,
        ReportTableCellDropTarget tableCellDropTarget,
        ReportPanelDropTarget panelDropTarget,
        ReportTableEditor tableEditor,
        ReportElementLocation location )
    {
        int sourceSectionIndex = location.SectionIndex;
        ReportElementDefinition element = location.Element;
        double originalX = element.X;
        double originalWidth = element.Width;

        if ( tableCellDropTarget is not null
            && !ReferenceEquals( tableCellDropTarget.Table, element )
            && !ReportDefinitionHelper.ContainsElement( element, tableCellDropTarget.Table ) )
        {
            location.OwnerElements.RemoveAt( location.ElementIndex );
            element.X = tableCellDropTarget.X;
            element.Y = tableCellDropTarget.Y;
            tableEditor.ReplaceCellElement( tableCellDropTarget.Table, tableCellDropTarget.Cell, element );

            return new()
            {
                SelectedCellKey = tableCellDropTarget.Cell.Id,
            };
        }

        if ( panelDropTarget is not null
            && !ReferenceEquals( panelDropTarget.Panel, element )
            && !ReportDefinitionHelper.ContainsElement( element, panelDropTarget.Panel ) )
        {
            if ( !ReferenceEquals( location.OwnerElements, panelDropTarget.Panel.Elements ) )
            {
                location.OwnerElements.RemoveAt( location.ElementIndex );
                panelDropTarget.Panel.Elements.Add( element );
            }

            ReportDefinitionHelper.PlaceElementInPanel( panelDropTarget.Panel, element, panelDropTarget.X, panelDropTarget.Y );

            return SelectElement( ReportDefinitionHelper.EnsureElementId( element ) );
        }

        location.OwnerElements.RemoveAt( location.ElementIndex );
        element.X = x;
        element.Y = y;
        targetSection.Elements.Add( element );
        ReportDetailHeaderSynchronizer.SyncMatchingPageHeaderForDetailElement( definition, sourceSectionIndex, targetSectionIndex, element, originalX, originalWidth, element.X, element.Width );

        return SelectElement( ReportDefinitionHelper.EnsureElementId( element ) );
    }

    private static bool TryFindPanelAt( IList<ReportElementDefinition> elements, double x, double y, double offsetX, double offsetY, ReportElementDefinition excludedElement, out ReportPanelDropTarget target )
    {
        target = null;

        for ( int index = elements.Count - 1; index >= 0; index-- )
        {
            if ( elements[index] is not ReportPanelElementDefinition panel
                || ReferenceEquals( panel, excludedElement )
                || panel.Suppress?.Value == true )
            {
                continue;
            }

            double panelX = offsetX + panel.X;
            double panelY = offsetY + panel.Y;

            if ( x < panelX || x > panelX + panel.Width || y < panelY || y > panelY + panel.Height )
                continue;

            if ( panel.Elements is not null && TryFindPanelAt( panel.Elements, x, y, panelX, panelY, excludedElement, out target ) )
                return true;

            target = new()
            {
                Panel = panel,
                X = Math.Max( 0, x - panelX ),
                Y = Math.Max( 0, y - panelY ),
            };

            return true;
        }

        return false;
    }

    private static ReportDropResult SelectElement( string elementKey )
    {
        return new()
        {
            PrimaryElementKey = elementKey,
            SelectedElementKeys = string.IsNullOrWhiteSpace( elementKey ) ? [] : [elementKey],
        };
    }

    private static ReportDesignerDragPreview CreateFieldDragPreview( ReportDefinition definition, ReportDesignerInteractionState state, int targetSectionIndex, double x, double y )
    {
        ReportBandDefinition targetSection = targetSectionIndex >= 0 && targetSectionIndex < definition.Bands.Count
            ? definition.Bands[targetSectionIndex]
            : null;
        (string DataSourceName, string FieldName) fieldBinding = ReportDefinitionHelper.NormalizeFieldBindingForSection( definition, targetSection, state.DraggedDataSourceName, state.DraggedFieldName );

        return new()
        {
            SectionIndex = targetSectionIndex,
            ElementType = ReportElementType.Field,
            Text = ReportExpressionFormatter.FormatFieldExpression( fieldBinding.DataSourceName, fieldBinding.FieldName ),
            X = x,
            Y = y,
            Width = ReportDesignerConstants.DefaultDroppedFieldWidth,
            Height = ReportDesignerConstants.DefaultDroppedFieldHeight,
        };
    }

    private static ReportDesignerDragPreview CreateDragPreview( ReportDefinition definition, int targetSectionIndex, ReportElementDefinition element, double? x = null, double? y = null )
    {
        return new()
        {
            SectionIndex = targetSectionIndex,
            ElementType = element.Type,
            Text = ReportElementDefinitionHelper.GetDisplayText( definition, element ),
            X = x ?? element.X,
            Y = y ?? element.Y,
            Width = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, element.Width ),
            Height = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, element.Height ),
        };
    }

    #endregion
}