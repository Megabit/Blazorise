using System.Text;

namespace Blazorise.Tailwind.Providers;

public sealed class TailwindEnumerationNameBuilder<T> : IEnumerationNameBuilder<T>
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