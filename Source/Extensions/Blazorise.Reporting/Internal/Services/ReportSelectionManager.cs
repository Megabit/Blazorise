#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportSelectionManager
{
    #region Members

    private readonly List<string> selectedElementKeys = [];

    #endregion

    #region Methods

    internal ReportSelectionState CaptureState( ReportDefinition definition )
    {
        var elementIds = GetSelectedElementIds( definition ).ToList();

        if ( elementIds.Count > 0 )
        {
            var primaryElementId = ReportDefinitionHelper.TryFindElementLocation( definition, PrimaryElementKey, out var sectionIndex, out _, out var element )
                ? element.Id
                : elementIds[0];

            if ( element is null )
                ReportDefinitionHelper.TryFindElementLocation( definition, primaryElementId, out sectionIndex, out _, out element );

            return new()
            {
                Type = ReportSelectionType.Element,
                SectionId = sectionIndex >= 0 ? definition.Sections[sectionIndex].Id : null,
                ElementId = primaryElementId,
                ElementIds = elementIds,
            };
        }

        if ( !string.IsNullOrWhiteSpace( SelectedCellKey )
            && ReportDefinitionHelper.TryFindTableCellLocation( definition, SelectedCellKey, out int cellSectionIndex, out _, out _, out ReportTableCellDefinition cell ) )
        {
            return new()
            {
                Type = ReportSelectionType.Cell,
                SectionId = cellSectionIndex >= 0 ? definition.Sections[cellSectionIndex].Id : null,
                CellId = cell.Id,
            };
        }

        if ( SelectedSectionIndex is not null
            && SelectedSectionIndex.Value >= 0
            && SelectedSectionIndex.Value < definition.Sections.Count )
        {
            return new()
            {
                Type = ReportSelectionType.Section,
                SectionId = definition.Sections[SelectedSectionIndex.Value].Id,
            };
        }

        return new()
        {
            Type = ReportSelectionType.Report,
        };
    }

    internal void ApplyState( ReportDefinition definition, ReportSelectionState selection )
    {
        SelectReport();

        if ( selection is null )
            return;

        if ( selection.Type == ReportSelectionType.Section )
        {
            var sectionIndex = definition.Sections.FindIndex( section => string.Equals( section.Id, selection.SectionId, StringComparison.Ordinal ) );

            if ( sectionIndex >= 0 )
                SelectSection( sectionIndex );

            return;
        }

        if ( selection.Type == ReportSelectionType.Element )
        {
            List<string> elementIds = selection.ElementIds is not null && selection.ElementIds.Count > 0
                ? selection.ElementIds
                : string.IsNullOrWhiteSpace( selection.ElementId ) ? [] : [selection.ElementId];

            foreach ( string elementId in elementIds )
            {
                if ( ReportDefinitionHelper.TryFindElementLocation( definition, elementId, out ReportElementLocation location ) )
                {
                    ReportSelected = false;
                    AddSelectedElement( ReportDefinitionHelper.EnsureElementId( location.Element ) );
                }
            }

            if ( !string.IsNullOrWhiteSpace( selection.ElementId ) && selectedElementKeys.Remove( selection.ElementId ) )
                selectedElementKeys.Insert( 0, selection.ElementId );

            return;
        }

        if ( selection.Type == ReportSelectionType.Cell )
        {
            if ( ReportDefinitionHelper.TryFindTableCellLocation( definition, selection.CellId, out _, out _, out _, out ReportTableCellDefinition cell ) )
                SelectCell( cell.Id );
        }
    }

    internal bool SelectReport()
    {
        if ( ReportSelected
             && SelectedSectionIndex is null
             && selectedElementKeys.Count == 0
             && string.IsNullOrWhiteSpace( SelectedCellKey ) )
        {
            return false;
        }

        ReportSelected = true;
        SelectedSectionIndex = null;
        SelectedCellKey = null;
        selectedElementKeys.Clear();

        return true;
    }

    internal bool SelectElement( string key, bool preserveSelection = false )
    {
        if ( IsElementSelectionCurrent( key, preserveSelection ) )
            return false;

        ReportSelected = false;
        SelectedSectionIndex = null;
        SelectedCellKey = null;

        if ( !preserveSelection )
            selectedElementKeys.Clear();

        if ( !string.IsNullOrWhiteSpace( key ) )
        {
            selectedElementKeys.Remove( key );
            selectedElementKeys.Insert( 0, key );
        }

        return true;
    }

    internal bool ToggleElementSelection( string key )
    {
        if ( string.IsNullOrWhiteSpace( key ) )
            return false;

        ReportSelected = false;
        SelectedSectionIndex = null;
        SelectedCellKey = null;

        if ( selectedElementKeys.Contains( key ) )
        {
            selectedElementKeys.Remove( key );
        }
        else
        {
            selectedElementKeys.Insert( 0, key );
        }

        if ( selectedElementKeys.Count == 0 )
        {
            ReportSelected = true;
        }

        return true;
    }

    internal bool SelectElements( IEnumerable<string> elementKeys, string primaryElementKey = null )
    {
        List<string> nextElementKeys = elementKeys
            .Where( key => !string.IsNullOrWhiteSpace( key ) )
            .Distinct( StringComparer.Ordinal )
            .ToList();
        if ( !string.IsNullOrWhiteSpace( primaryElementKey ) && nextElementKeys.Remove( primaryElementKey ) )
            nextElementKeys.Insert( 0, primaryElementKey );

        if ( IsElementSelectionCurrent( nextElementKeys ) )
            return false;

        selectedElementKeys.Clear();

        foreach ( var elementKey in nextElementKeys )
        {
            selectedElementKeys.Add( elementKey );
        }

        SelectedSectionIndex = null;
        SelectedCellKey = null;
        ReportSelected = selectedElementKeys.Count == 0;

        return true;
    }

    internal bool SelectSection( int index )
    {
        if ( !ReportSelected
             && SelectedSectionIndex == index
             && string.IsNullOrWhiteSpace( SelectedCellKey )
             && selectedElementKeys.Count == 0 )
        {
            return false;
        }

        ReportSelected = false;
        SelectedSectionIndex = index;
        SelectedCellKey = null;
        selectedElementKeys.Clear();

        return true;
    }

    internal bool SelectCell( string key )
    {
        if ( !ReportSelected
             && SelectedSectionIndex is null
             && string.Equals( SelectedCellKey, key, StringComparison.Ordinal )
             && selectedElementKeys.Count == 0 )
        {
            return false;
        }

        ReportSelected = false;
        SelectedSectionIndex = null;
        SelectedCellKey = key;
        selectedElementKeys.Clear();

        return true;
    }

    internal ReportElementDefinition FindSelectedElement( ReportDefinition definition )
    {
        if ( string.IsNullOrWhiteSpace( PrimaryElementKey ) )
            return null;

        return ReportDefinitionHelper.TryFindElementLocation( definition, PrimaryElementKey, out _, out _, out var element )
            ? element
            : null;
    }

    internal ReportTableCellDefinition FindSelectedCell( ReportDefinition definition )
    {
        if ( string.IsNullOrWhiteSpace( SelectedCellKey ) )
            return null;

        return ReportDefinitionHelper.TryFindTableCellLocation( definition, SelectedCellKey, out _, out _, out _, out ReportTableCellDefinition cell )
            ? cell
            : null;
    }

    internal IEnumerable<string> GetSelectedElementIds( ReportDefinition definition )
    {
        foreach ( var elementKey in selectedElementKeys )
        {
            if ( ReportDefinitionHelper.TryFindElementLocation( definition, elementKey, out _, out _, out var element ) )
                yield return element.Id;
        }
    }

    internal int ResolvePasteSectionIndex( ReportDefinition definition )
    {
        if ( definition.Sections.Count == 0 )
            return -1;

        if ( SelectedSectionIndex is not null
            && SelectedSectionIndex.Value >= 0
            && SelectedSectionIndex.Value < definition.Sections.Count )
        {
            return SelectedSectionIndex.Value;
        }

        if ( ReportDefinitionHelper.TryFindElementLocation( definition, PrimaryElementKey, out var sectionIndex, out _, out _ ) )
            return sectionIndex;

        return 0;
    }

    internal bool CanDeleteSelection( ReportDefinition definition )
    {
        if ( definition is null )
            return false;

        if ( selectedElementKeys.Count > 0 )
            return selectedElementKeys.Any( elementKey =>
                ReportDefinitionHelper.TryFindElementLocation( definition, elementKey, out int sectionIndex, out _, out _ )
                && sectionIndex >= 0
                && sectionIndex < definition.Sections.Count
                && !ReportValueResolver.ResolveStaticSuppress( definition.Sections[sectionIndex] ) );

        if ( !string.IsNullOrWhiteSpace( SelectedCellKey ) )
            return false;

        return CanDeleteSection( FindSelectedSection( definition ) );
    }

    internal bool IsSelectedElementSuppressed( ReportDefinition definition )
    {
        return !string.IsNullOrWhiteSpace( PrimaryElementKey )
            && ReportDefinitionHelper.TryFindElementLocation( definition, PrimaryElementKey, out int sectionIndex, out _, out _ )
            && sectionIndex >= 0
            && sectionIndex < definition.Sections.Count
            && ReportValueResolver.ResolveStaticSuppress( definition.Sections[sectionIndex] );
    }

    internal ReportSectionDefinition FindSelectedSection( ReportDefinition definition )
    {
        if ( SelectedSectionIndex is null || SelectedSectionIndex < 0 || SelectedSectionIndex >= definition.Sections.Count )
            return null;

        return definition.Sections[SelectedSectionIndex.Value];
    }

    internal bool IsElementSelected( string elementKey )
    {
        return !string.IsNullOrWhiteSpace( elementKey )
            && selectedElementKeys.Contains( elementKey );
    }

    private bool IsElementSelectionCurrent( string key, bool preserveSelection )
    {
        if ( ReportSelected || SelectedSectionIndex is not null || !string.IsNullOrWhiteSpace( SelectedCellKey ) )
            return false;

        if ( !string.Equals( PrimaryElementKey, key, StringComparison.Ordinal ) )
            return false;

        if ( string.IsNullOrWhiteSpace( key ) )
            return selectedElementKeys.Count == 0;

        return preserveSelection
            ? selectedElementKeys.Contains( key )
            : selectedElementKeys.Count == 1 && selectedElementKeys.Contains( key );
    }

    private bool IsElementSelectionCurrent( IReadOnlyList<string> elementKeys )
    {
        return ReportSelected == ( elementKeys.Count == 0 )
            && SelectedSectionIndex is null
            && string.IsNullOrWhiteSpace( SelectedCellKey )
            && selectedElementKeys.SequenceEqual( elementKeys );
    }

    private static bool CanDeleteSection( ReportSectionDefinition section )
    {
        return section is not null && !section.Default;
    }

    internal void ClearElementSelection()
    {
        selectedElementKeys.Clear();
    }

    internal void AddSelectedElement( string elementKey )
    {
        if ( string.IsNullOrWhiteSpace( elementKey ) || selectedElementKeys.Contains( elementKey ) )
            return;

        selectedElementKeys.Add( elementKey );
    }

    #endregion

    #region Properties

    internal bool ReportSelected { get; set; } = true;

    internal string PrimaryElementKey => selectedElementKeys.FirstOrDefault();

    internal string SelectedCellKey { get; set; }

    internal IReadOnlyList<string> SelectedElementKeys => selectedElementKeys;

    internal int? SelectedSectionIndex { get; set; }

    #endregion
}