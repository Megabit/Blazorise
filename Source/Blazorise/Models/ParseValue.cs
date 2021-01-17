namespace Blazorise
{
    public struct ParseValue<TValue>
    {
        public static readonly ParseValue<TValue> Empty = new ParseValue<TValue>();

        public ParseValue( bool success, TValue parsedValue, string errorMessage = null )
        {
            Success = success;
            ParsedValue = parsedValue;
            ErrorMessage = errorMessage;
        }

        public bool Success { get; }

        public TValue ParsedValue { get; }

        public string ErrorMessage { get; }
    }
}
