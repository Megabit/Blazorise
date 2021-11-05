namespace Blazorise.DataGrid
{
    /// <summary>
    /// Defines the column editor type.
    /// </summary>
    public enum DataGridColumnType
    {
        /// <summary>
        /// Column can accept string values.
        /// </summary>
        Text,

        /// <summary>
        /// Column can accept only numeric values.
        /// </summary>
        Numeric,

        /// <summary>
        /// Column can accept only boolean values.
        /// </summary>
        Check,

        /// <summary>
        /// Column can accept only datetime values.
        /// </summary>
        Date,

        /// <summary>
        /// Column can be used to select multiple values.
        /// </summary>
        Select,

        /// <summary>
        /// Column is used only to represent commands like edit, save, etc.
        /// </summary>
        Command,

        /// <summary>
        /// Column is used only to represent the multiselect command.
        /// </summary>
        MultiSelect,
    }
}