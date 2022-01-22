namespace Blazorise.DataGrid
{
    public class DetailRowTriggerContext<TItem>
    {

        public DetailRowTriggerContext( TItem item )
        {
            Item = item;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public TItem Item { get; private set; }

        /// <summary>
        /// If DetailRow is showing, and if the trigger is re-evaluated the DetailRow will be set to false.
        /// Defaults to true.
        /// </summary>
        public bool Toggleable { get; set; } = true;

    }
}