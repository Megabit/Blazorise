using System.Collections.Generic;
using System.Text;

namespace Blazorise.Bulma.Providers;

public sealed class BulmaEnumerationNameBuilder<T> : IEnumerationNameBuilder<T>
    where T : Enumeration<T>
{
    public string BuildName( Enumeration<T> enumeration )
    {
        var names = new List<string>();

        AppendRecursive( enumeration, names );

        if ( names.Count == 0 )
            return string.Empty;

        if ( string.Equals( names[0], "secondary", System.StringComparison.OrdinalIgnoreCase ) )
        {
            if ( names.Count > 1 && string.Equals( names[1], "emphasis", System.StringComparison.OrdinalIgnoreCase ) )
                return "dark";

            return "light";
        }

        var lastIndex = names.Count - 1;

        if ( string.Equals( names[lastIndex], "subtle", System.StringComparison.OrdinalIgnoreCase ) )
            names[lastIndex] = "light";
        else if ( string.Equals( names[lastIndex], "emphasis", System.StringComparison.OrdinalIgnoreCase ) )
            names[lastIndex] = "dark";

        var sb = new StringBuilder();

        for ( var i = 0; i < names.Count; i++ )
        {
            var segment = names[i];

            if ( string.IsNullOrWhiteSpace( segment ) )
                continue;

            if ( sb.Length > 0 )
                sb.Append( '-' );

            sb.Append( segment );
        }

        return sb.ToString();
    }

    private static void AppendRecursive( Enumeration<T> current, ICollection<string> names )
    {
        if ( current.ParentEnumeration is { } parent )
        {
            AppendRecursive( parent, names );
        }

        if ( !string.IsNullOrWhiteSpace( current.RawName ) )
            names.Add( current.RawName );
    }
}