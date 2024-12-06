#region Using directives
using System.Text;
#endregion

namespace Blazorise.Docs.Compiler.ApiDocsGenerator.Helpers;
//todo: remove this, as it served only as the "real" source generator.
/// <summary>
/// This looks silly, but works surprisingly well.
/// I haven't found a way how to log in source generator (without building the context project (Blazorise) - which takes time).
/// This will emit Log.txt.cs file (cannot be named wout .cs) with log messages in cs comments (to make sure it's valid "C# code" )
/// 🤷‍♂️
/// </summary>
public static class Logger
{
    private static readonly StringBuilder _logBuilder = new();


    public static string LogMessages => $"""
                                         /*
                                         {_logBuilder}
                                         */
                                         """;
    public static bool IsOn { get; set; }

    public static void Log( string message )
    {
        if ( !IsOn )
            return;
        LogAlways( message );
    }

    public static void LogAlways( string message )
    {
        _logBuilder.AppendLine( message );

    }
}

