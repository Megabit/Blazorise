namespace Blazorise.DataGrid
{
    /// <summary>
    /// Holds the DetailRowTrigger context configuration.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class DetailRowTriggerEventArgs<TItem>
    {
        public DetailRowTriggerEventArgs( TItem item )
        {
            Item = item;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public TItem Item { get; private set; }

        /// <summary>
        /// If DetailRow is showing, and if the trigger is re-evaluated the DetailRow will be set to false. Defaults to <c>true</c>.
        /// </summary>
        public bool Toggleable { get; set; } = true;

        /// <summary>
        /// Only a single DetailRow is shown. Visible Detail Rows will be collapsed once a new DetailRow is shown.
        /// </summary>
        public bool Single { get; set; }

        /// <summary>
        /// Gets or Sets the DetailRowTriggerType. Defaults to <see cref="DetailRowTriggerType.RowClick"/>.
        /// </summary>
        public DetailRowTriggerType DetailRowTriggerType { get; set; } = DetailRowTriggerType.RowClick;
    }
}