#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Reporting.Internal;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Base class for declarative report elements that register themselves with the current report container.
/// </summary>
public abstract class BaseReportElement : ComponentBase, IDisposable
{
    #region Members

    private readonly string definitionId = Guid.NewGuid().ToString( "N" );

    private IReportElementContainerContext registeredContainerContext;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        bool definitionChanged = Definition is null || HasDefinitionChanged( parameters );

        await base.SetParametersAsync( parameters );

        bool containerChanged = !ReferenceEquals( registeredContainerContext, ContainerContext );

        if ( containerChanged )
        {
            registeredContainerContext?.UnregisterElement( this );
            registeredContainerContext = ContainerContext;
        }

        if ( definitionChanged || containerChanged )
            Definition = BuildDefinition();

        if ( definitionChanged || containerChanged )
            registeredContainerContext?.RegisterElement( this, Definition );
    }

    /// <inheritdoc />
    public void Dispose()
    {
        registeredContainerContext?.UnregisterElement( this );
        registeredContainerContext = null;
    }

    /// <summary>
    /// Determines whether parameters affecting the element definition changed.
    /// </summary>
    protected virtual bool HasDefinitionChanged( ParameterView parameters )
    {
        return parameters.IsParameterChanged( Name )
            || parameters.IsParameterChanged( X )
            || parameters.IsParameterChanged( Y )
            || parameters.IsParameterChanged( Width )
            || parameters.IsParameterChanged( Height )
            || parameters.IsReportValueChanged( CanGrow )
            || parameters.IsReportValueChanged( Suppress )
            || parameters.IsParameterChanged( SnapToGrid )
            || parameters.IsParameterChanged( ShowCollisionWarnings )
            || parameters.IsParameterChanged( Class )
            || parameters.IsParameterChanged( Style )
            || parameters.IsParameterChanged( BackgroundColor )
            || parameters.IsParameterChanged( BorderColor )
            || parameters.IsParameterChanged( BorderWidth )
            || parameters.IsParameterChanged( BorderStyle )
            || parameters.IsParameterChanged( BorderRadius )
            || parameters.IsParameterChanged( Opacity )
            || parameters.IsParameterChanged( Appearance )
            || parameters.IsParameterChanged( Border );
    }

    /// <summary>
    /// Creates or updates the registered element definition.
    /// </summary>
    /// <returns>The element definition based on the component parameters.</returns>
    protected virtual ReportElementDefinition BuildDefinition()
    {
        ReportElementDefinition definition = Definition ?? ReportElementDefinitionFactory.Create( ElementType );

        definition.Id = definitionId;
        definition.Name = Name;
        definition.X = X;
        definition.Y = Y;
        definition.Width = Width;
        definition.Height = Height;
        definition.CanGrow = CanGrow;
        definition.Suppress = Suppress;
        definition.SnapToGrid = SnapToGrid;
        definition.ShowCollisionWarnings = ShowCollisionWarnings;
        definition.Appearance = BuildAppearanceDefinition();
        definition.Border = BuildBorderDefinition();
        definition.Class = Class;
        definition.Style = Style;

        return definition;
    }

    private ReportAppearanceDefinition BuildAppearanceDefinition()
    {
        return new()
        {
            BackgroundColor = BackgroundColor.IsDefault ? Appearance?.BackgroundColor ?? ReportColor.Default : BackgroundColor,
            Opacity = Opacity ?? Appearance?.Opacity,
        };
    }

    private ReportBorderDefinition BuildBorderDefinition()
    {
        return new()
        {
            Color = BorderColor.IsDefault ? Border?.Color ?? ReportColor.Default : BorderColor,
            Width = BorderWidth ?? Border?.Width,
            Style = BorderStyle != ReportBorderStyle.Default ? BorderStyle : Border?.Style ?? ReportBorderStyle.Default,
            Radius = BorderRadius ?? Border?.Radius,
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Element kind represented by the derived component.
    /// </summary>
    protected abstract ReportElementType ElementType { get; }

    /// <summary>
    /// Element definition produced from the current component parameters.
    /// </summary>
    protected ReportElementDefinition Definition { get; private set; }

    private protected IReportElementContainerContext RegisteredContainerContext => registeredContainerContext;

    [CascadingParameter] internal IReportElementContainerContext ContainerContext { get; set; }

    /// <summary>
    /// Friendly element name shown in the designer.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Horizontal position within the containing report container, in points.
    /// </summary>
    [Parameter] public double X { get; set; }

    /// <summary>
    /// Vertical position within the containing report container, in points.
    /// </summary>
    [Parameter] public double Y { get; set; }

    /// <summary>
    /// Element width in points.
    /// </summary>
    [Parameter] public double Width { get; set; } = 90;

    /// <summary>
    /// Element height in points.
    /// </summary>
    [Parameter] public double Height { get; set; } = 18;

    /// <summary>
    /// Allows text content to expand the element vertically when rendered.
    /// </summary>
    [Parameter] public ReportValue<bool> CanGrow { get; set; } = false;

    /// <summary>
    /// Prevents the element from being edited on the designer surface and rendered in preview output.
    /// </summary>
    [Parameter] public ReportValue<bool> Suppress { get; set; } = false;

    /// <summary>
    /// Overrides the designer-level snap-to-grid behavior for this element. A null value inherits the designer setting.
    /// </summary>
    [Parameter] public bool? SnapToGrid { get; set; }

    /// <summary>
    /// Indicates whether the element participates in designer collision warnings.
    /// </summary>
    [Parameter] public bool ShowCollisionWarnings { get; set; } = true;

    /// <summary>
    /// Additional CSS classes applied to the element.
    /// </summary>
    [Parameter] public string Class { get; set; }

    /// <summary>
    /// Inline style applied to the element.
    /// </summary>
    [Parameter] public string Style { get; set; }

    /// <summary>
    /// Background color applied to the element.
    /// </summary>
    [Parameter] public ReportColor BackgroundColor { get; set; }

    /// <summary>
    /// Border color applied to the element.
    /// </summary>
    [Parameter] public ReportColor BorderColor { get; set; }

    /// <summary>
    /// Border width applied around the element.
    /// </summary>
    [Parameter] public double? BorderWidth { get; set; }

    /// <summary>
    /// Border style applied around the element.
    /// </summary>
    [Parameter] public ReportBorderStyle BorderStyle { get; set; }

    /// <summary>
    /// Border radius applied to the element corners.
    /// </summary>
    [Parameter] public double? BorderRadius { get; set; }

    /// <summary>
    /// Element opacity from 0 to 1.
    /// </summary>
    [Parameter] public double? Opacity { get; set; }

    /// <summary>
    /// Fill and opacity settings applied to the element.
    /// </summary>
    [Parameter] public ReportAppearanceDefinition Appearance { get; set; }

    /// <summary>
    /// Border settings applied around the element.
    /// </summary>
    [Parameter] public ReportBorderDefinition Border { get; set; }

    #endregion
}