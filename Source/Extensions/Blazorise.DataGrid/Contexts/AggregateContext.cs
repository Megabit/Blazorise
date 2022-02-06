namespace Blazorise.DataGrid
{
    /// <summary>
    /// Context for calculated aggregate value.
    /// </summary>
    public class AggregateContext<TItem>
    {
        /// <summary>
        /// Default context constructor.
        /// </summary>
        /// <param name="field">Column field name.</param>
        /// <param name="value">Calculated value.</param>
        public AggregateContext( string field, object value )
        {
            Field = field;
            Value = value;
        }

        /// <summary>
        /// Gets the column field name.
        /// </summary>
        public string Field { get; }

        /// <summary>
        /// Gets the aggregate value.
        /// </summary>
        public object Value { get; }
    }
}