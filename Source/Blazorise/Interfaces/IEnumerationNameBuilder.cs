namespace Blazorise;

/// <summary>
/// Defines a contract for building the final name of an <see cref="Enumeration{T}"/> instance.
/// </summary>
/// <typeparam name="T">The type of the enumeration.</typeparam>
public interface IEnumerationNameBuilder<T> where T : Enumeration<T>
{
    /// <summary>
    /// Builds the final name for the specified enumeration instance.
    /// </summary>
    /// <param name="enumeration">The enumeration instance for which to build the name.</param>
    /// <returns>
    /// A formatted string representing the complete name of the enumeration,
    /// typically including any parent enumeration names joined by a delimiter.
    /// </returns>
    string BuildName( T enumeration );
}