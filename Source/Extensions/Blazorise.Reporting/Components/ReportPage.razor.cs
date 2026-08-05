#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Reporting.Internal;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a named report design page and the bands it contains.
/// </summary>
public partial class ReportPage : ComponentBase, IDisposable
{
    #region Members

    private readonly ReportPageDefinition definition = new();

    private ReportContext registeredReportContext;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new report page component.
    /// </summary>
    public ReportPage()
    {
        PageContext = new( definition, null );
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        bool definitionChanged = registeredReportContext is null
            || parameters.IsParameterChanged( Name )
            || parameters.IsParameterChanged( Size )
            || parameters.IsParameterChanged( MeasurementUnit )
            || parameters.IsParameterChanged( Orientation )
            || parameters.IsParameterChanged( Width )
            || parameters.IsParameterChanged( Height )
            || parameters.IsParameterChanged( MarginLeft )
            || parameters.IsParameterChanged( MarginTop )
            || parameters.IsParameterChanged( MarginRight )
            || parameters.IsParameterChanged( MarginBottom );

        await base.SetParametersAsync( parameters );

        bool contextChanged = !ReferenceEquals( registeredReportContext, ReportContext );

        if ( contextChanged )
        {
            registeredReportContext?.UnregisterPage( this );
            registeredReportContext = ReportContext;
            PageContext.DefinitionChanged = registeredReportContext is null
                ? null
                : new Action( registeredReportContext.NotifyDefinitionChanged );
        }

        if ( definitionChanged )
            UpdateDefinition();

        if ( definitionChanged || contextChanged )
            registeredReportContext?.RegisterPage( this, definition );
    }

    /// <inheritdoc />
    public void Dispose()
    {
        registeredReportContext?.UnregisterPage( this );
        registeredReportContext = null;
    }

    private void UpdateDefinition()
    {
        definition.Name = Name;
        definition.Size = Size;
        definition.MeasurementUnit = MeasurementUnit;
        definition.Orientation = Orientation;
        definition.Width = ReportMeasurementConverter.ToPoints( Width, MeasurementUnit );
        definition.Height = ReportMeasurementConverter.ToPoints( Height, MeasurementUnit );
        definition.Margins = new()
        {
            Left = ReportMeasurementConverter.ToPoints( MarginLeft, MeasurementUnit ),
            Top = ReportMeasurementConverter.ToPoints( MarginTop, MeasurementUnit ),
            Right = ReportMeasurementConverter.ToPoints( MarginRight, MeasurementUnit ),
            Bottom = ReportMeasurementConverter.ToPoints( MarginBottom, MeasurementUnit ),
        };
    }

    #endregion

    #region Properties

    internal ReportPageContext PageContext { get; }

    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    /// <summary>
    /// Friendly page name shown in the designer.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Named page size used when explicit dimensions are not supplied.
    /// </summary>
    [Parameter] public ReportPageSize Size { get; set; } = ReportPageSize.A4;

    /// <summary>
    /// Page orientation applied to the selected size.
    /// </summary>
    [Parameter] public ReportOrientation Orientation { get; set; } = ReportOrientation.Portrait;

    /// <summary>
    /// Unit used by the declarative page dimensions and margins.
    /// </summary>
    [Parameter] public ReportMeasurementUnit MeasurementUnit { get; set; } = ReportMeasurementUnit.Centimeter;

    /// <summary>
    /// Explicit page width in the configured measurement unit.
    /// </summary>
    [Parameter] public double? Width { get; set; }

    /// <summary>
    /// Explicit page height in the configured measurement unit.
    /// </summary>
    [Parameter] public double? Height { get; set; }

    /// <summary>
    /// Left printable page margin in the configured measurement unit.
    /// </summary>
    [Parameter] public double MarginLeft { get; set; }

    /// <summary>
    /// Top printable page margin in the configured measurement unit.
    /// </summary>
    [Parameter] public double MarginTop { get; set; }

    /// <summary>
    /// Right printable page margin in the configured measurement unit.
    /// </summary>
    [Parameter] public double MarginRight { get; set; }

    /// <summary>
    /// Bottom printable page margin in the configured measurement unit.
    /// </summary>
    [Parameter] public double MarginBottom { get; set; }

    /// <summary>
    /// Declarative report bands placed on this page.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}