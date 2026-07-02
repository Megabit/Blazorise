#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportRenderService
{
    #region Members

    private readonly Dictionary<string, IReadOnlyList<object>> designerSectionRenderItems = new( System.StringComparer.Ordinal );

    private ( ReportDefinition Definition, object Data ) designerSectionRenderItemsCacheKey;

    #endregion

    #region Methods

    internal IReadOnlyList<object> ResolveSectionRenderItems( ReportDefinition definition, ReportSectionDefinition section, object data, bool designMode )
    {
        if ( designMode )
            return ResolveDesignerSectionRenderItems( definition, section, data );

        var items = section.Type == ReportSectionType.Detail
            ? ReportDataResolver.ResolveItems( definition, data, section.DataSource ).ToList()
            : new List<object> { ReportDataResolver.ResolveItems( definition, data, section.DataSource ).FirstOrDefault() };

        if ( items.Count == 0 )
            items.Add( null );

        return items;
    }

    internal double GetPageWidth( ReportDefinition definition )
    {
        definition.Page = ReportPageDefinitionHelper.ResolvePage( definition.Page );

        return definition.Page.Width;
    }

    internal double GetPageContentWidth( ReportDefinition definition )
    {
        definition.Page = ReportPageDefinitionHelper.ResolvePage( definition.Page );

        return ReportPageDefinitionHelper.GetContentWidth( definition.Page );
    }

    internal string GetPreviewPageContentStyle( ReportDefinition definition )
    {
        definition.Page = ReportPageDefinitionHelper.ResolvePage( definition.Page );

        var styleBuilder = new StyleBuilder( builder =>
        {
            builder.Append( $"left:{ReportMeasurementConverter.ToCssPixelString( definition.Page.Margins.Left )}" );
            builder.Append( $"top:{ReportMeasurementConverter.ToCssPixelString( definition.Page.Margins.Top )}" );
            builder.Append( $"right:{ReportMeasurementConverter.ToCssPixelString( definition.Page.Margins.Right )}" );
            builder.Append( $"bottom:{ReportMeasurementConverter.ToCssPixelString( definition.Page.Margins.Bottom )}" );
        } );

        return styleBuilder.Styles;
    }

    internal string GetPreviewPageFooterStyle( ReportDefinition definition, ReportRenderPage renderPage, System.Func<int, ReportSectionDefinition, double> getSectionHeight )
    {
        definition.Page = ReportPageDefinitionHelper.ResolvePage( definition.Page );

        double footerHeight = renderPage.FooterSections.Sum( renderSection => getSectionHeight( renderSection.SectionIndex, renderSection.Section ) );
        var styleBuilder = new StyleBuilder( builder =>
        {
            builder.Append( $"left:{ReportMeasurementConverter.ToCssPixelString( definition.Page.Margins.Left )}" );
            builder.Append( $"right:{ReportMeasurementConverter.ToCssPixelString( definition.Page.Margins.Right )}" );
            builder.Append( $"bottom:{ReportMeasurementConverter.ToCssPixelString( definition.Page.Margins.Bottom )}" );
            builder.Append( $"height:{ReportMeasurementConverter.ToCssPixelString( footerHeight )}" );
        } );

        return styleBuilder.Styles;
    }

    internal void Invalidate()
    {
        designerSectionRenderItems.Clear();
        designerSectionRenderItemsCacheKey = default;
    }

    private IReadOnlyList<object> ResolveDesignerSectionRenderItems( ReportDefinition definition, ReportSectionDefinition section, object data )
    {
        if ( definition is null || section is null )
            return [null];

        if ( !ReferenceEquals( designerSectionRenderItemsCacheKey.Definition, definition ) || !ReferenceEquals( designerSectionRenderItemsCacheKey.Data, data ) )
        {
            Invalidate();
            designerSectionRenderItemsCacheKey = ( definition, data );
        }

        string sectionId = ReportDefinitionHelper.EnsureSectionId( section );

        if ( designerSectionRenderItems.TryGetValue( sectionId, out IReadOnlyList<object> cachedItems ) )
            return cachedItems;

        object item = ReportDataResolver.ResolveItems( definition, data, section.DataSource ).FirstOrDefault();
        IReadOnlyList<object> items = [item];
        designerSectionRenderItems[sectionId] = items;

        return items;
    }

    #endregion
}