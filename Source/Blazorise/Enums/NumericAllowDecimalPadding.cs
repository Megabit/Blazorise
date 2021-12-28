namespace Blazorise
{
    /// <summary>
    /// Defines if the decimal places should be padded with zeroes.
    /// </summary>
    public enum NumericAllowDecimalPadding
    {
        /// <summary>
        /// Always pad decimals with zeros (ie. '12.3400').
        /// </summary>
        Always,

        /// <summary>
        /// Never pad with zeros (ie. '12.34').
        /// </summary>
        Never,

        /// <summary>
        /// Pad with zeroes only when there are decimals (ie. '12' and '12.3400').
        /// </summary>
        Floats,
    }
}
