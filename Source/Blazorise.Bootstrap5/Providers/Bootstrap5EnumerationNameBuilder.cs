using System.Text;

namespace Blazorise.Bootstrap5.Providers;

public sealed class Bootstrap5EnumerationNameBuilder<T> : IEnumerationNameBuilder<T>
    where T : Enumeration<T>
{
    public string BuildName( T enumeration )
    {
        var sb = new StringBuilder();

        AppendRecursive( enumeration, sb );

        return sb.ToString();
    }

    private static void AppendRecursive( T current, StringBuilder sb )
    {
        if ( current.ParentEnumeration is { } parent )
        {
            AppendRecursive( parent, sb );

            if ( !string.IsNullOrWhiteSpace( parent.RawName ) )
                sb.Append( '-' );
        }

        if ( !string.IsNullOrWhiteSpace( current.RawName ) )
            sb.Append( current.RawName );
    }
}