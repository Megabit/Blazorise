#region Using directives
using System;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Defines a custom report element and its Blazor renderers.
/// </summary>
public interface IReportElementPlugin
{
    /// <summary>
    /// Gets the element metadata.
    /// </summary>
    ReportElementDescriptor Descriptor { get; }

    /// <summary>
    /// Gets the component used to render the element in the designer and HTML preview.
    /// </summary>
    Type RendererComponentType { get; }

    /// <summary>
    /// Gets the optional component used to edit plugin-owned properties.
    /// </summary>
    Type PropertiesComponentType { get; }

    /// <summary>
    /// Gets the optional renderer used for PDF output.
    /// </summary>
    IReportElementPdfRenderer PdfRenderer { get; }

    /// <summary>
    /// Creates and initializes a new element definition.
    /// </summary>
    ReportCustomElementDefinition CreateElement();
}
