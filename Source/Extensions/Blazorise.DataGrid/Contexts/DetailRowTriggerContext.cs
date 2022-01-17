namespace Blazorise.DataGrid
{
    public class DetailRowTriggerContext<TItem>
    {
        public DetailRowTriggerContext()
        {

        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public TItem Item { get; set; }

        /// <summary>
        /// If DetailRow is showing, and if the trigger is re-evaluated the DetailRow will be set to false.
        /// Defaults to true.
        /// </summary>
        public bool Toggleable { get; set; } = true;

        /// <summary>
        /// Sets the DetailRowTrigger conditions to trigger its evaluation.
        /// </summary>
        public DetailRowTriggerType Type { get; set; }
    }
}