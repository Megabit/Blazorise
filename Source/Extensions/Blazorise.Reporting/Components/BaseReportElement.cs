#region Using directives
using System;
using Blazorise.Reporting.Internal;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Base class for declarative report elements that register themselves with the current report container.
/// </summary>
public abstract class BaseReportElement : ComponentBase
{
    #region Members

    private readonly string definitionId = Guid.NewGuid().ToString( "N" );

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if ( ContainerContext is null )
            return;

        Definition = BuildDefinition();
        ContainerContext.AddElement( Definition );
    }

    /// <summary>
    /// Creates the element definition registered with the containing report band.
    /// </summary>
    /// <returns>A new element definition based on the component parameters.</returns>
    protected virtual ReportElementDefinition BuildDefinition()
    {
        ReportElementDefinition definition = ReportElementDefinitionFactory.Create( ElementType );

        definition.Id = definitionId;
        definition.Name = Name;
        definition.X = X;
        definition.Y = Y;
        definition.Width = Width;
        definition.Height = Height;
        definition.CanGrow = CanGrow;
        definition.Suppress = Suppress;
        definition.SnapToGrid = SnapToGrid;
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
    /// Overrides the report-level snap-to-grid behavior for this element. A null value inherits the report setting.
    /// </summary>
    [Parameter] public ReportValue<bool?> SnapToGrid { get; set; } = (bool?)null;

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