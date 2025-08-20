using Blazorise.Modules;

namespace Blazorise.Exporters;

/// <summary>
/// Represents an exporter that requires access to the JavaScript export module.
/// </summary>
public interface IExporterWithJsModule
{
    IJSUtilitiesModule JSUtilitiesModule { get; set; }
}