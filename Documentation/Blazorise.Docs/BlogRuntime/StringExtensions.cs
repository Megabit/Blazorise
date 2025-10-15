using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Blazorise.Docs.BlogRuntime;

public static class StringExtensions
{
    public static string ToLfLineEndings( this string value )
    {
        if ( value == null )
            return null;

        return Regex.Replace( value, @"\r?\n", "\n" );
    }
}