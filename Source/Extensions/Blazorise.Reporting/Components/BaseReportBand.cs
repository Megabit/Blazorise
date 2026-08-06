#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Base class for declarative report bands that register themselves with the current report page.
/// </summary>
public abstract class BaseReportBand : ComponentBase, IDisposable
{
    #region Members

    private readonly ReportBandDefinition definition = new();

    private ReportPageContext registeredPageContext;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new report band component.
    /// </summary>
    protected BaseReportBand()
    {
        SectionContext = new( definition );
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        bool definitionChanged = registeredPageContext is null
            || parameters.IsParameterChanged( Id )
            || parameters.IsParameterChanged( Name )
            || parameters.IsParameterChanged( Height )
            || parameters.IsParameterChanged( DataSource )
            || parameters.IsParameterChanged( GroupBy )
            || parameters.IsParameterChanged( Class )
            || parameters.IsParameterChanged( Style )
            || parameters.IsReportValueChanged( Suppress )
            || parameters.IsParameterChanged( ReserveSpaceWhenSuppressed )
            || parameters.IsParameterChanged( PrintOnFirstPage )
            || parameters.IsParameterChanged( PrintOnLastPage )
            || parameters.IsParameterChanged( RepeatOnEveryPage )
            || parameters.IsReportValueChanged( KeepTogether )
            || parameters.IsReportValueChanged( NewPageBefore )
            || parameters.IsReportValueChanged( NewPageAfter )
            || parameters.IsParameterChanged( BackgroundColor )
            || parameters.IsParameterChanged( BorderColor )
            || parameters.IsParameterChanged( BorderWidth );

        await base.SetParametersAsync( parameters );

        bool pageChanged = !ReferenceEquals( registeredPageContext, PageContext );

        if ( pageChanged )
        {
            registeredPageContext?.UnregisterBand( this );
            registeredPageContext = PageContext;
            SectionContext.DefinitionChanged = registeredPageContext is null
                ? null
                : new Action( registeredPageContext.NotifyDefinitionChanged );
        }

        if ( definitionChanged )
            UpdateDefinition();

        if ( definitionChanged || pageChanged )
            registeredPageContext?.RegisterBand( this, definition );
    }

    /// <inheritdoc />
    public void Dispose()
    {
        registeredPageContext?.UnregisterBand( this );
        registeredPageContext = null;
    }

    private void UpdateDefinition()
    {
        definition.Id = Id;
        definition.Name = Name;
        definition.Type = SectionType;
        definition.Height = Height;
        definition.DataSource = DataSource;
        definition.GroupBy = GroupBy;
        definition.Class = Class;
        definition.Style = Style;
        definition.Default = true;
        definition.Suppress = Suppress ?? false;
        definition.ReserveSpaceWhenSuppressed = ReserveSpaceWhenSuppressed;
        definition.PrintOnFirstPage = PrintOnFirstPage;
        definition.PrintOnLastPage = PrintOnLastPage;
        definition.RepeatOnEveryPage = RepeatOnEveryPage;
        definition.KeepTogether = KeepTogether;
        definition.NewPageBefore = NewPageBefore;
        definition.NewPageAfter = NewPageAfter;
        definition.Appearance = new()
        {
            BackgroundColor = BackgroundColor,
        };
        definition.Border = new()
        {
            Color = BorderColor,
            Width = BorderWidth,
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Section context provided to child report elements.
    /// </summary>
    internal ReportSectionContext SectionContext { get; }

    /// <summary>
    /// Band kind represented by the derived component.
    /// </summary>
    protected abstract ReportBandType SectionType { get; }

    [CascadingParameter] internal ReportPageContext PageContext { get; set; }

    /// <summary>
    /// Stable identifier used to reference the band.
    /// </summary>
    [Parameter] public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    /// <summary>
    /// Friendly band name shown in the designer.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Band height in points.
    /// </summary>
    [Parameter] public double Height { get; set; } = 60;

    /// <summary>
    /// Data source name or path used as the band field context. Detail bands repeat when this value resolves to a collection.
    /// </summary>
    [Parameter] public string DataSource { get; set; }

    /// <summary>
    /// Field expression used by group header bands to split detail rows into groups.
    /// </summary>
    [Parameter] public string GroupBy { get; set; }

    /// <summary>
    /// Additional CSS classes applied to the band.
    /// </summary>
    [Parameter] public string Class { get; set; }

    /// <summary>
    /// Inline style applied to the band.
    /// </summary>
    [Parameter] public string Style { get; set; }

    /// <summary>
    /// Excludes the band from rendered output while keeping it visible in the designer.
    /// </summary>
    [Parameter] public ReportValue<bool> Suppress { get; set; }

    /// <summary>
    /// Keeps the band height reserved when the band is suppressed.
    /// </summary>
    [Parameter] public bool ReserveSpaceWhenSuppressed { get; set; }

    /// <summary>
    /// Allows page footer bands to render on the first page.
    /// </summary>
    [Parameter] public bool PrintOnFirstPage { get; set; } = true;

    /// <summary>
    /// Allows page footer bands to render on the last page.
    /// </summary>
    [Parameter] public bool PrintOnLastPage { get; set; } = true;

    /// <summary>
    /// Allows page footer bands to repeat on every rendered page.
    /// </summary>
    [Parameter] public bool RepeatOnEveryPage { get; set; } = true;

    /// <summary>
    /// Keeps the band content together when pagination is applied.
    /// </summary>
    [Parameter] public ReportValue<bool> KeepTogether { get; set; } = false;

    /// <summary>
    /// Starts the band on a new page before rendering it.
    /// </summary>
    [Parameter] public ReportValue<bool> NewPageBefore { get; set; } = false;

    /// <summary>
    /// Starts a new page after the band is rendered.
    /// </summary>
    [Parameter] public ReportValue<bool> NewPageAfter { get; set; } = false;

    /// <summary>
    /// Background color applied to the band.
    /// </summary>
    [Parameter] public ReportColor BackgroundColor { get; set; }

    /// <summary>
    /// Border color applied around the band.
    /// </summary>
    [Parameter] public ReportColor BorderColor { get; set; }

    /// <summary>
    /// Border width applied around the band.
    /// </summary>
    [Parameter] public double? BorderWidth { get; set; }

    /// <summary>
    /// Declarative elements placed inside the band.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}