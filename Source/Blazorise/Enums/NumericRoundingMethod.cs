namespace Blazorise
{
    /// <summary>
    /// Defines the rounding method to use.
    /// </summary>
    public enum NumericRoundingMethod
    {
        /// <summary>
        /// Round-Half-Up Symmetric (default).
        /// </summary>
        HalfUpSymmetric,

        /// <summary>
        /// Round-Half-Up Asymmetric.
        /// </summary>
        HalfUpAsymmetric,

        /// <summary>
        /// Round-Half-Down Symmetric.
        /// </summary>
        HalfDownSymmetric,

        /// <summary>
        /// Round-Half-Down Asymmetric.
        /// </summary>
        HalfDownAsymmetric,

        /// <summary>
        /// Round-Half-Even "Bankers Rounding".
        /// </summary>
        HalfEvenBankersRounding,

        /// <summary>
        /// Round Up "Round-Away-From-Zero".
        /// </summary>
        UpRoundAwayFromZero,

        /// <summary>
        /// Round Down "Round-Toward-Zero" - same as truncate.
        /// </summary>
        DownRoundTowardZero,

        /// <summary>
        /// Round to Ceiling "Toward Positive Infinity".
        /// </summary>
        ToCeilingTowardPositiveInfinity,

        /// <summary>
        /// Round to Floor "Toward Negative Infinity".
        /// </summary>
        ToFloorTowardNegativeInfinity,

        /// <summary>
        /// Rounds to the nearest .05 => same as <see cref="ToNearest05Alt"/> used in 1.9X and still valid.
        /// </summary>
        ToNearest05,

        /// <summary>
        /// Rounds up to next .05.
        /// </summary>
        ToNearest05Alt,

        /// <summary>
        /// Rounds up to next .05.
        /// </summary>
        UpToNext05,

        /// <summary>
        /// Rounds down to next .05.
        /// </summary>
        DownToNext05,
    }
}
