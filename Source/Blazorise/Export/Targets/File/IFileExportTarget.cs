using Blazorise.Modules;

namespace Blazorise.Export;

/// <summary>
/// Defines file-specific interop functionality for DataGrid export targets, including access to the JavaScript export module.
/// </summary>
public interface IFileExportTarget
{
    IJSUtilitiesModule JSUtilitiesModule { get; set; }
}