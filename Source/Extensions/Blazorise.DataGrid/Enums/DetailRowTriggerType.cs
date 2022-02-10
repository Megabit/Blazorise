namespace Blazorise.DataGrid
{
    /// <summary>
    /// Defines the DetailRowTriggerType of the DataGrid's DetailRow.
    /// </summary>
    public enum DetailRowTriggerType
    {
        /// <summary>
        /// Trigger is manually controlled by invoking the Datagrid's ToggleDetailRow.
        /// </summary>
        Manual,

        /// <summary>
        /// Triggers on row click.
        /// </summary>
        RowClick,
    }
}