#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportSectionCommandService
{
    #region Methods

    internal ReportSectionDefinition CreateInsertedSection( ReportDefinition definition, ReportSectionDefinition sourceSection )
    {
        if ( definition is null || sourceSection is null )
            return null;

        return new()
        {
            Name = ReportDefinitionHelper.CreateUniqueSectionName( definition, $"{ReportDefinitionHelper.GetSectionTypeDisplayName( sourceSection.Type )} band" ),
            Type = sourceSection.Type,
            Height = sourceSection.Height,
            DataSource = sourceSection.DataSource,
            GroupBy = sourceSection.GroupBy,
            Default = false,
            Suppressed = false,
            ReserveSpaceWhenSuppressed = sourceSection.ReserveSpaceWhenSuppressed,
            PrintOnFirstPage = sourceSection.PrintOnFirstPage,
            PrintOnLastPage = sourceSection.PrintOnLastPage,
            RepeatOnEveryPage = sourceSection.RepeatOnEveryPage,
            KeepTogether = ReportValue.Create( sourceSection.KeepTogether?.Value ?? false, sourceSection.KeepTogether?.Formula ),
            NewPageBefore = ReportValue.Create( sourceSection.NewPageBefore?.Value ?? false, sourceSection.NewPageBefore?.Formula ),
            NewPageAfter = ReportValue.Create( sourceSection.NewPageAfter?.Value ?? false, sourceSection.NewPageAfter?.Formula ),
            Appearance = new()
            {
                BackgroundColor = sourceSection.Appearance?.BackgroundColor ?? ReportColor.Default,
                Opacity = sourceSection.Appearance?.Opacity,
            },
            Border = new()
            {
                Color = sourceSection.Border?.Color ?? ReportColor.Default,
                Width = sourceSection.Border?.Width,
                Radius = sourceSection.Border?.Radius,
            },
        };
    }

    internal int DeleteSection( ReportDefinition definition, int sectionIndex, ISet<string> collapsedSectionIds )
    {
        if ( definition?.Sections is null
            || sectionIndex < 0
            || sectionIndex >= definition.Sections.Count )
        {
            return -1;
        }

        ReportSectionDefinition section = definition.Sections[sectionIndex];

        if ( !ReportDefinitionHelper.CanDeleteSection( section ) )
            return -1;

        collapsedSectionIds?.Remove( ReportDefinitionHelper.EnsureSectionId( section ) );
        definition.Sections.RemoveAt( sectionIndex );

        return definition.Sections.Count == 0
            ? -1
            : System.Math.Min( sectionIndex, definition.Sections.Count - 1 );
    }

    internal void UpdateSectionSuppression( ReportSectionDefinition section, bool suppressed, ISet<string> collapsedSectionIds )
    {
        if ( section is null )
            return;

        section.Suppressed = suppressed;

        if ( suppressed )
            collapsedSectionIds?.Remove( ReportDefinitionHelper.EnsureSectionId( section ) );
    }

    #endregion
}