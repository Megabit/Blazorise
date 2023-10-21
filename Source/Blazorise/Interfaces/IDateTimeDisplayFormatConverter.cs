namespace Blazorise;

/// <summary>
/// Interface for converting the date formats for the display mask.
/// </summary>
public interface IDateTimeDisplayFormatConverter
{
    /// <summary>
    /// Converts the .NET date format into the internal date format.
    /// </summary>
    /// <param name="format">A string representing the .NET date format.</param>
    /// <returns>A date format ready for internal date editor.</returns>
    string Convert( string format );
}