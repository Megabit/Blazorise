#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportSelectionManager
{
    #region Members

    private readonly HashSet<string> selectedElementKeys = new( StringComparer.Ordinal );

    #endregion

    #region Methods

    internal ReportSelectionState CaptureState( ReportDefinition definition )
    {
        var elementIds = GetSelectedElementIds( definition ).ToList();

        if ( elementIds.Count > 0 )
        {
            var primaryElementId = ReportDefinitionHelper.TryFindElementLocation( definition, SelectedElementKey, out var sectionIndex, out _, out var element )
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
                    SelectedElementKeys.Add( ReportDefinitionHelper.EnsureElementId( location.Element ) );
                }
            }

            SelectedElementKey = SelectedElementKeys.Contains( selection.ElementId )
                ? selection.ElementId
                : SelectedElementKeys.FirstOrDefault();

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
             && string.IsNullOrWhiteSpace( SelectedElementKey )
             && string.IsNullOrWhiteSpace( SelectedCellKey )
             && SelectedElementKeys.Count == 0 )
        {
            return false;
        }

        ReportSelected = true;
        SelectedSectionIndex = null;
        SelectedElementKey = null;
        SelectedCellKey = null;
        SelectedElementKeys.Clear();

        return true;
    }

    internal bool SelectElement( string key, bool preserveSelection = false )
    {
        if ( IsElementSelectionCurrent( key, preserveSelection ) )
            return false;

        ReportSelected = false;
        SelectedSectionIndex = null;
        SelectedElementKey = key;
        SelectedCellKey = null;

        if ( !preserveSelection )
            SelectedElementKeys.Clear();

        if ( !string.IsNullOrWhiteSpace( key ) )
            SelectedElementKeys.Add( key );

        return true;
    }

    internal bool ToggleElementSelection( string key )
    {
        if ( string.IsNullOrWhiteSpace( key ) )
            return false;

        ReportSelected = false;
        SelectedSectionIndex = null;
        SelectedCellKey = null;

        if ( SelectedElementKeys.Contains( key ) )
        {
            SelectedElementKeys.Remove( key );

            if ( string.Equals( SelectedElementKey, key, StringComparison.Ordinal ) )
                SelectedElementKey = SelectedElementKeys.FirstOrDefault();
        }
        else
        {
            SelectedElementKeys.Add( key );
            SelectedElementKey = key;
        }

        if ( SelectedElementKeys.Count == 0 )
        {
            ReportSelected = true;
            SelectedElementKey = null;
        }

        return true;
    }

    internal bool SelectElements( IEnumerable<string> elementKeys, string primaryElementKey = null )
    {
        List<string> nextElementKeys = elementKeys
            .Where( key => !string.IsNullOrWhiteSpace( key ) )
            .Distinct( StringComparer.Ordinal )
            .ToList();
        string nextPrimaryElementKey = !string.IsNullOrWhiteSpace( primaryElementKey ) && nextElementKeys.Contains( primaryElementKey )
            ? primaryElementKey
            : nextElementKeys.FirstOrDefault();

        if ( IsElementSelectionCurrent( nextElementKeys, nextPrimaryElementKey ) )
            return false;

        SelectedElementKeys.Clear();

        foreach ( var elementKey in nextElementKeys )
        {
            SelectedElementKeys.Add( elementKey );
        }

        SelectedElementKey = nextPrimaryElementKey;
        SelectedSectionIndex = null;
        SelectedCellKey = null;
        ReportSelected = SelectedElementKeys.Count == 0;

        return true;
    }

    internal bool SelectSection( int index )
    {
        if ( !ReportSelected
             && SelectedSectionIndex == index
             && string.IsNullOrWhiteSpace( SelectedElementKey )
             && string.IsNullOrWhiteSpace( SelectedCellKey )
             && SelectedElementKeys.Count == 0 )
        {
            return false;
        }

        ReportSelected = false;
        SelectedSectionIndex = index;
        SelectedElementKey = null;
        SelectedCellKey = null;
        SelectedElementKeys.Clear();

        return true;
    }

    internal bool SelectCell( string key )
    {
        if ( !ReportSelected
             && SelectedSectionIndex is null
             && string.IsNullOrWhiteSpace( SelectedElementKey )
             && string.Equals( SelectedCellKey, key, StringComparison.Ordinal )
             && SelectedElementKeys.Count == 0 )
        {
            return false;
        }

        ReportSelected = false;
        SelectedSectionIndex = null;
        SelectedElementKey = null;
        SelectedCellKey = key;
        SelectedElementKeys.Clear();

        return true;
    }

    internal ReportElementDefinition FindSelectedElement( ReportDefinition definition )
    {
        if ( string.IsNullOrWhiteSpace( SelectedElementKey ) )
            return null;

        return ReportDefinitionHelper.TryFindElementLocation( definition, SelectedElementKey, out _, out _, out var element )
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
        List<string> elementKeys = SelectedElementKeys.Count > 0
            ? SelectedElementKeys.ToList()
            : string.IsNullOrWhiteSpace( SelectedElementKey ) ? [] : [SelectedElementKey];

        foreach ( var elementKey in elementKeys )
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

        if ( ReportDefinitionHelper.TryFindElementLocation( definition, SelectedElementKey, out var sectionIndex, out _, out _ ) )
            return sectionIndex;

        return 0;
    }

    internal bool CanDeleteSelection( ReportDefinition definition )
    {
        if ( definition is null )
            return false;

        if ( !string.IsNullOrWhiteSpace( SelectedElementKey ) )
            return !IsSelectedElementSuppressed( definition ) && FindSelectedElement( definition ) is not null;

        if ( !string.IsNullOrWhiteSpace( SelectedCellKey ) )
            return false;

        return CanDeleteSection( FindSelectedSection( definition ) );
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
            && ( string.Equals( SelectedElementKey, elementKey, StringComparison.Ordinal ) || SelectedElementKeys.Contains( elementKey ) );
    }

    internal bool IsCellSelected( string cellKey )
    {
        return !string.IsNullOrWhiteSpace( cellKey )
            && string.Equals( SelectedCellKey, cellKey, StringComparison.Ordinal );
    }

    private bool IsElementSelectionCurrent( string key, bool preserveSelection )
    {
        if ( ReportSelected || SelectedSectionIndex is not null || !string.IsNullOrWhiteSpace( SelectedCellKey ) )
            return false;

        if ( !string.Equals( SelectedElementKey, key, StringComparison.Ordinal ) )
            return false;

        if ( string.IsNullOrWhiteSpace( key ) )
            return SelectedElementKeys.Count == 0;

        return preserveSelection
            ? SelectedElementKeys.Contains( key )
            : SelectedElementKeys.Count == 1 && SelectedElementKeys.Contains( key );
    }

    private bool IsElementSelectionCurrent( IReadOnlyList<string> elementKeys, string primaryElementKey )
    {
        return ReportSelected == ( elementKeys.Count == 0 )
            && SelectedSectionIndex is null
            && string.IsNullOrWhiteSpace( SelectedCellKey )
            && string.Equals( SelectedElementKey, primaryElementKey, StringComparison.Ordinal )
            && SelectedElementKeys.SetEquals( elementKeys );
    }

    private static bool CanDeleteSection( ReportSectionDefinition section )
    {
        return section is not null && !section.Default;
    }

    internal bool IsSelectedElementSuppressed( ReportDefinition definition )
    {
        return !string.IsNullOrWhiteSpace( SelectedElementKey )
            && ReportDefinitionHelper.TryFindElementLocation( definition, SelectedElementKey, out var sectionIndex, out _, out _ )
            && definition.Sections[sectionIndex].Suppressed;
    }

    #endregion

    #region Properties

    internal bool ReportSelected { get; set; } = true;

    internal string SelectedElementKey { get; set; }

    internal string SelectedCellKey { get; set; }

    internal HashSet<string> SelectedElementKeys => selectedElementKeys;

    internal int? SelectedSectionIndex { get; set; }

    #endregion
}