namespace Blazorise
{
    /// <summary>
    /// Interface for converting the date formats.
    /// </summary>
    public interface IDateTimeFormatConverter
    {
        /// <summary>
        /// Converts the .Net date format into the internal date format.
        /// </summary>
        /// <param name="format">A string representing the .Net date format.</param>
        /// <returns>A date format ready for internal date editor.</returns>
        string Convert( string format );
    }
}
