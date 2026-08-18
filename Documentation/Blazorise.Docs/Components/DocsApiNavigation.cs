#region Using directives
using System;
using System.Text;
#endregion

namespace Blazorise.Docs.Components;

internal static class DocsApiNavigation
{
    internal static string CreateElementId( string apiName )
    {
        if ( string.IsNullOrWhiteSpace( apiName ) )
            return null;

        int genericMarkerIndex = apiName.IndexOfAny( ['<', '`'] );
        string normalizedName = genericMarkerIndex >= 0
            ? apiName[..genericMarkerIndex]
            : apiName;
        StringBuilder elementId = new( "api-" );

        foreach ( char character in normalizedName )
        {
            if ( char.IsLetterOrDigit( character ) )
            {
                elementId.Append( char.ToLowerInvariant( character ) );
            }
        }

        return elementId.Length > 4 ? elementId.ToString() : null;
    }
}