using System;
using System.Collections.Generic;
using Blazorise.ApiDocsGenerator.Helpers;

public static class EnumerableExtensions
{
    /// <summary>
    /// This looks much cleaner then wrapping all the stuff in string.Join. (check the usage)
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string StringJoin(this IEnumerable<string> source, string separator)
    {
        try
        {
            return source == null ? "" : string.Join(" ", source);
        }
        catch ( Exception e )
        {
            Logger.LogAlways(e.Message);
        }
        return "";

    }
}