namespace Blazorise.DataGrid;

/// <summary>
/// Defines file-specific interop functionality for DataGrid export targets, including access to the JavaScript export module.
/// </summary>
public interface IFileExportTarget
{
    JSDataGridModule JsDataGridModule { get; set; }
}