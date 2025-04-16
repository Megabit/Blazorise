namespace Blazorise.Exporters;

/// <summary>
/// Represents an exporter that requires access to the JavaScript export module.
/// </summary>
public interface IExporterWithJsModule
{
    JSExportersModule JsExportersModule { get; set; }
}