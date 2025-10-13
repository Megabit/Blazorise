using System.Text;

namespace Blazorise.FluentUI2.Providers;

public sealed class FluentUI2EnumerationNameBuilder<T> : IEnumerationNameBuilder<T>
    where T : Enumeration<T>
{
    public string BuildName( Enumeration<T> enumeration )
    {
        var sb = new StringBuilder();

        AppendRecursive( enumeration, sb );

        return sb.ToString();
    }

    private static void AppendRecursive( Enumeration<T> current, StringBuilder sb )
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