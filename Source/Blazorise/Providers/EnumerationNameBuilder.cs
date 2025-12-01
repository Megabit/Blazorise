using System.Text;

namespace Blazorise;

/// <summary>
/// Provides the default implementation of the <see cref="IEnumerationNameBuilder{T}"/> interface,
/// which builds an enumeration name by traversing its parent hierarchy and joining parts with spaces.
/// </summary>
/// <typeparam name="T">The type of the enumeration.</typeparam>
public sealed class EnumerationNameBuilder<T> : IEnumerationNameBuilder<T>
    where T : Enumeration<T>
{
    /// <summary>
    /// Builds the final name for the specified enumeration instance by concatenating
    /// the names of all parent enumerations, separated by spaces.
    /// </summary>
    /// <param name="enumeration">The enumeration instance for which to build the name.</param>
    /// <returns>
    /// A space-separated string containing the concatenated names of all parent enumerations
    /// and the current enumeration.
    /// </returns>
    public string BuildName( Enumeration<T> enumeration )
    {
        var sb = new StringBuilder();

        BuildRecursive( enumeration, sb );

        return sb.ToString();
    }

    /// <summary>
    /// Recursively builds the enumeration name by traversing its parent hierarchy
    /// and appending each name to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="current">The current enumeration being processed.</param>
    /// <param name="sb">The <see cref="StringBuilder"/> used to construct the final name.</param>
    private static void BuildRecursive( Enumeration<T> current, StringBuilder sb )
    {
        if ( current.ParentEnumeration is { } parent )
        {
            BuildRecursive( parent, sb );
            if ( !string.IsNullOrWhiteSpace( parent.RawName ) )
                sb.Append( ' ' );
        }

        if ( !string.IsNullOrWhiteSpace( current.RawName ) )
            sb.Append( current.RawName );
    }
}