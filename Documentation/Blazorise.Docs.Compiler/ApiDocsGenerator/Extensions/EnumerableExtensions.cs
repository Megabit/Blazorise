#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Docs.Compiler.ApiDocsGenerator.Helpers;
#endregion

namespace Blazorise.Docs.Compiler.ApiDocsGenerator.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// This looks much cleaner then wrapping all the stuff in string.Join. (check the usage)
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string StringJoin( this IEnumerable<string> source, string separator )
    {
        try
        {
            return source == null ? "" : string.Join( separator, source );
        }
        catch
        {
        }

        return "";
    }
}