namespace Blazorise.Utilities;

/// <summary>
/// Utility methods for building CSS selectors.
/// </summary>
public static class CssSelectorUtilities
{
    /// <summary>
    /// Builds a CSS attribute selector that matches an element by id.
    /// </summary>
    /// <param name="elementId">The element id.</param>
    /// <returns>A CSS selector, or null if the id is empty.</returns>
    public static string BuildElementIdSelector( string elementId )
    {
        if ( string.IsNullOrWhiteSpace( elementId ) )
            return null;

        return $"[id=\"{EscapeStringValue( elementId )}\"]";
    }

    /// <summary>
    /// Escapes a value that will be used inside a quoted CSS string.
    /// </summary>
    /// <param name="value">The CSS string value.</param>
    /// <returns>The escaped CSS string value.</returns>
    public static string EscapeStringValue( string value )
        => value?.Replace( "\\", "\\\\" ).Replace( "\"", "\\\"" );
}