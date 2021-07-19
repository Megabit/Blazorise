namespace Blazorise
{
    /// <summary>
    /// Provides the information about the parse operation.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public struct ParseValue<TValue>
    {
        /// <summary>
        /// Gets the default value.
        /// </summary>
        public static readonly ParseValue<TValue> Empty = new();

        /// <summary>
        /// A default <see cref="ParseValue{TValue}"/> constructor.
        /// </summary>
        /// <param name="success">The result of parse operation.</param>
        /// <param name="parsedValue">Parsed value.</param>
        /// <param name="errorMessage">Error message if parse had failed.</param>
        public ParseValue( bool success, TValue parsedValue, string errorMessage = null )
        {
            Success = success;
            ParsedValue = parsedValue;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets true if parse operation was successful.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets the parsed value.
        /// </summary>
        public TValue ParsedValue { get; }

        /// <summary>
        /// Gets the error message if parse had failed.
        /// </summary>
        public string ErrorMessage { get; }
    }
}
