namespace Blazorise.DataGrid
{

    /// <summary>
    /// Defines the command mode of the data grid.
    /// </summary>
    public enum DataGridCommandMode
    {
        /// <summary>
        /// Default state which means that both defined commands and button row will render(only if ButtonRowTemplate is defined).
        /// </summary>
        Default,

        /// <summary>
        /// Only defined commands will render.
        /// </summary>
        Commands,

        /// <summary>
        /// Only button row will render.
        /// </summary>
        ButtonRow
    }
}