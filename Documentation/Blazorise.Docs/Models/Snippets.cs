#region Using directives
using System.Linq;
using System.Reflection;
#endregion

namespace Blazorise.Docs.Models;

public static partial class Snippets
{
    private static FieldInfo[] snippetsFields = typeof( Snippets ).GetFields( BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField );

    public static string GetCode( string component )
    {
        var field = snippetsFields.FirstOrDefault( f => f.Name == component );

        if ( field == null )
            return $"Snippet for component '{component}' not found!";

        return (string)field.GetValue( null );
    }

}