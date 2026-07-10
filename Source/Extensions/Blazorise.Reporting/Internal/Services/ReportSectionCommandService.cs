#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportSectionCommandService
{
    #region Methods

    internal ReportBandDefinition CreateInsertedSection( ReportDefinition definition, ReportBandDefinition sourceSection )
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
            Suppress = false,
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

    internal int DeleteSection( ReportDefinition definition, int sectionIndex, ISet<string> collapsedBandIds )
    {
        if ( definition?.Bands is null
            || sectionIndex < 0
            || sectionIndex >= definition.Bands.Count )
        {
            return -1;
        }

        ReportBandDefinition section = definition.Bands[sectionIndex];

        if ( !ReportDefinitionHelper.CanDeleteSection( section ) )
            return -1;

        collapsedBandIds?.Remove( ReportDefinitionHelper.EnsureBandId( section ) );
        definition.Bands.RemoveAt( sectionIndex );

        return definition.Bands.Count == 0
            ? -1
            : System.Math.Min( sectionIndex, definition.Bands.Count - 1 );
    }

    internal void UpdateSectionSuppression( ReportBandDefinition section, bool suppressed, ISet<string> collapsedBandIds )
    {
        if ( section is null )
            return;

        ReportValueResolver.SetStaticSuppress( section, suppressed );

        if ( suppressed )
            collapsedBandIds?.Remove( ReportDefinitionHelper.EnsureBandId( section ) );
    }

    #endregion
}