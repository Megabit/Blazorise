#region Using directives
using System;
#endregion

namespace Blazorise.Scheduler.Utilities;

/// <summary>
/// Provides utility methods for working with <see cref="DateTime"/> values.
/// </summary>
public static class DateUtils
{
    /// <summary>
    /// Returns the earlier of the two specified DateTime values.
    /// </summary>
    /// <param name="first">The first DateTime value.</param>
    /// <param name="second">The second DateTime value.</param>
    /// <returns>The earlier (minimum) of the two DateTime values.</returns>
    public static DateTime Min( DateTime first, DateTime second )
    {
        return first < second ? first : second;
    }

    /// <summary>
    /// Returns the later of the two specified DateTime values.
    /// </summary>
    /// <param name="first">The first DateTime value.</param>
    /// <param name="second">The second DateTime value.</param>
    /// <returns>The later (maximum) of the two DateTime values.</returns>
    public static DateTime Max( DateTime first, DateTime second )
    {
        return first > second ? first : second;
    }
}
