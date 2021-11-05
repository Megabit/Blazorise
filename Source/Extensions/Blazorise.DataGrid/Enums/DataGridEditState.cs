namespace Blazorise.DataGrid
{
    /// <summary>
    /// States of the row items.
    /// </summary>
    public enum DataGridEditState
    {
        /// <summary>
        /// Default state which means the row value will just be showed.
        /// </summary>
        None,

        /// <summary>
        /// Row values will be visible in the new item edit mode.
        /// </summary>
        New,

        /// <summary>
        /// Row values will be visible in the row editing mode.
        /// </summary>
        Edit,
    }
}