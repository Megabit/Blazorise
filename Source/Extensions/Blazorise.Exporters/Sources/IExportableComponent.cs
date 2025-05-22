using Blazorise.Modules;

namespace Blazorise.Exporters;

/// <summary>
/// Represents a contract that defines the properties and methods required for components that can be exported.
/// </summary>
public interface IExportableComponent
{
    /// <summary>
    /// Gets or sets the <see cref="IJSUtilitiesModule"/> instance used for exporting data.
    /// </summary>
    IJSUtilitiesModule JSUtilitiesModule { get; set; }
}