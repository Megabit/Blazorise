namespace Blazorise.DataGrid.Utils;

/// <summary>
/// Supports js interop function behavior in DataGrid components.
/// </summary>
public static class JSInteropFunction
{
    /// <summary>
    /// JavaScript namespace containing DataGrid interop functions.
    /// </summary>
    public const string NAMESPACE = "blazoriseDataGrid.";

    /// <summary>
    /// Supports virtualize behavior in DataGrid components.
    /// </summary>
    public static class Virtualize
    {
        /// <summary>
        /// Base namespace for virtualization helpers.
        /// </summary>
        public const string BASE = NAMESPACE + "virtualize.";
        /// <summary>
        /// Identifier for restoring scroll position after editing.
        /// </summary>
        public const string ON_EDIT_SET_SCROLL = BASE + "onEditSetScroll";
    }
}