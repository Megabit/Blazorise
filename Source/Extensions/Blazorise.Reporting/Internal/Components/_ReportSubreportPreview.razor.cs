#region Using directives
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a fixed-box nested report preview inside a subreport element.
/// </summary>
public partial class _ReportSubreportPreview
{
    private ReportSubreportElementDefinition SubreportElement => Element as ReportSubreportElementDefinition;

    private ReportDefinition SubreportDefinition => ReportSubreportResolver.ResolveDefinition( SubreportElement );

    private object SubreportData => ReportSubreportResolver.ResolveData( Definition, Data, Item, SubreportElement );

    private bool CanRenderNestedReport => RenderDepth < ReportSubreportResolver.MaxRenderDepth && SubreportDefinition is not null;

    private string PlaceholderText => ReportSubreportResolver.GetDisplayName( SubreportElement );

    private IReadOnlyList<ReportRenderPage> RenderPages
        => CanRenderNestedReport
            ? ReportPreviewRenderPlanner.BuildRenderPages( SubreportDefinition, SubreportData ).Take( 1 ).ToList()
            : [];

    private RenderFragment RenderSubreportSection( ReportRenderSection renderSection )
    {
        return builder =>
        {
            if ( renderSection is null )
                return;

            int sequence = 0;

            builder.OpenComponent<_ReportDesignerSection>( sequence++ );
            builder.AddAttribute( sequence++, nameof( _ReportDesignerSection.SectionKey ), $"{renderSection.Section?.Id}:{renderSection.InstanceIndex}:subreport" );
            builder.AddAttribute( sequence++, nameof( _ReportDesignerSection.Section ), renderSection.Section );
            builder.AddAttribute( sequence++, nameof( _ReportDesignerSection.Height ), renderSection.Section?.Height ?? 0 );
            builder.AddAttribute( sequence++, nameof( _ReportDesignerSection.BodyWidth ), ReportPageDefinitionHelper.GetContentWidth( ResolveSubreportPage( SubreportDefinition ) ) );
            builder.AddAttribute( sequence++, nameof( _ReportDesignerSection.ChildContent ), (RenderFragment)( childBuilder =>
            {
                if ( renderSection.RenderElements )
                {
                    int childSequence = 0;

                    foreach ( ReportElementDefinition childElement in renderSection.Section.Elements )
                    {
                        if ( ReportValueResolver.ResolveSuppress( childElement, renderSection.Section, SubreportDefinition, SubreportData, renderSection.Item ) )
                            continue;

                        childBuilder.OpenComponent<_ReportDesignerElement>( childSequence++ );
                        childBuilder.AddAttribute( childSequence++, nameof( _ReportDesignerElement.Data ), SubreportData );
                        childBuilder.AddAttribute( childSequence++, nameof( _ReportDesignerElement.Definition ), SubreportDefinition );
                        childBuilder.AddAttribute( childSequence++, nameof( _ReportDesignerElement.Section ), renderSection.Section );
                        childBuilder.AddAttribute( childSequence++, nameof( _ReportDesignerElement.Item ), renderSection.Item );
                        childBuilder.AddAttribute( childSequence++, nameof( _ReportDesignerElement.RunningTotals ), renderSection.RunningTotals );
                        childBuilder.AddAttribute( childSequence++, nameof( _ReportDesignerElement.Element ), childElement );
                        childBuilder.AddAttribute( childSequence++, nameof( _ReportDesignerElement.ElementKey ), ReportDefinitionHelper.EnsureElementId( childElement ) );
                        childBuilder.AddAttribute( childSequence++, nameof( _ReportDesignerElement.SubreportDepth ), RenderDepth + 1 );
                        childBuilder.CloseComponent();
                    }
                }
            } ) );
            builder.CloseComponent();
        };
    }

    private static string GetSubreportPageStyle( ReportDefinition definition )
    {
        ReportPageDefinition page = ResolveSubreportPage( definition );

        return $"position:relative;width:{ReportMeasurementConverter.ToCssPixelString( ReportPageDefinitionHelper.GetContentWidth( page ) )};height:{ReportMeasurementConverter.ToCssPixelString( ReportPageDefinitionHelper.GetContentHeight( page ) )}";
    }

    private static ReportPageDefinition ResolveSubreportPage( ReportDefinition definition )
    {
        ReportPageDefinition page = definition?.Page;

        return ReportPageDefinitionHelper.ResolvePage( new()
        {
            Size = page?.Size ?? ReportPageSize.A4,
            Orientation = page?.Orientation ?? ReportOrientation.Portrait,
            Width = page?.Width ?? 0,
            Height = page?.Height ?? 0,
            Margins = new()
            {
                Left = page?.Margins?.Left ?? 0,
                Top = page?.Margins?.Top ?? 0,
                Right = page?.Margins?.Right ?? 0,
                Bottom = page?.Margins?.Bottom ?? 0,
            },
        } );
    }

    /// <summary>
    /// Root report data used when resolving the subreport data source.
    /// </summary>
    [Parameter] public object Data { get; set; }

    /// <summary>
    /// Parent report definition that owns the subreport element.
    /// </summary>
    [Parameter] public ReportDefinition Definition { get; set; }

    /// <summary>
    /// Current parent report data item.
    /// </summary>
    [Parameter] public object Item { get; set; }

    /// <summary>
    /// Subreport element definition.
    /// </summary>
    [Parameter] public ReportElementDefinition Element { get; set; }

    /// <summary>
    /// Current nested subreport depth.
    /// </summary>
    [Parameter] public int RenderDepth { get; set; }
}
