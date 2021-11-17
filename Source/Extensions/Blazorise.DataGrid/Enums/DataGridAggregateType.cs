namespace Blazorise.DataGrid
{
    /// <summary>
    /// Defines the supported aggregate calculation types.
    /// </summary>
    public enum DataGridAggregateType
    {
        /// <summary>
        /// Aggregation will not be calculated.
        /// </summary>
        None,

        /// <summary>
        /// Summary of numeric value.
        /// </summary>
        Sum,

        /// <summary>
        /// Average value of numeric value.
        /// </summary>
        Average,

        /// <summary>
        /// Min value of numeric value.
        /// </summary>
        Min,

        /// <summary>
        /// Max value of numeric value.
        /// </summary>
        Max,

        /// <summary>
        /// Count all values that are not null.
        /// </summary>
        Count,

        /// <summary>
        /// Count all boolean values that are true.
        /// </summary>
        TrueCount,

        /// <summary>
        /// Count all boolean values that are false.
        /// </summary>
        FalseCount,
    }
}