using Blazorise.Modules;

namespace Blazorise.Export;

/// <summary>
/// Defines clipboard-specific interop functionality for DataGrid export targets.
/// </summary>
public interface IClipboardExportTarget
{
     IJSUtilitiesModule JSUtilitiesModule { get; set; }

}