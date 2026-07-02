using System;
using System.ComponentModel;
using System.Globalization;

namespace Blazorise.Reporting;

/// <summary>
/// Default resolver for built-in print-focused report colors.
/// </summary>
public sealed class ReportDefaultColorResolver : IReportColorResolver
{
    #region Methods

    /// <inheritdoc />
    public ReportResolvedColor Resolve( ReportColor color )
    {
        return color.Kind switch
        {
            ReportColorKind.Default => new( 0, 0, 0, -1 ),
            ReportColorKind.Transparent => new( 0, 0, 0, 0 ),
            ReportColorKind.Rgb => new( color.Red, color.Green, color.Blue, color.Alpha ),
            ReportColorKind.Named => ResolveNamedColor( color.Name, color.Alpha ),
            _ => new( 0, 0, 0, -1 ),
        };
    }

    private static ReportResolvedColor ResolveNamedColor( string name, double alpha )
    {
        return NormalizeName( name ) switch
        {
            "black" => new( 0, 0, 0, alpha ),
            "white" => new( 255, 255, 255, alpha ),
            "red" => new( 255, 0, 0, alpha ),
            "green" => new( 0, 128, 0, alpha ),
            "blue" => new( 0, 0, 255, alpha ),
            "yellow" => new( 255, 255, 0, alpha ),
            "cyan" => new( 0, 255, 255, alpha ),
            "magenta" => new( 255, 0, 255, alpha ),
            "gray" => new( 128, 128, 128, alpha ),
            "grey" => new( 128, 128, 128, alpha ),
            "lightgray" => new( 211, 211, 211, alpha ),
            "lightgrey" => new( 211, 211, 211, alpha ),
            "darkgray" => new( 64, 64, 64, alpha ),
            "darkgrey" => new( 64, 64, 64, alpha ),
            "navy" => new( 0, 0, 128, alpha ),
            "maroon" => new( 128, 0, 0, alpha ),
            "olive" => new( 128, 128, 0, alpha ),
            "purple" => new( 128, 0, 128, alpha ),
            "teal" => new( 0, 128, 128, alpha ),
            "silver" => new( 192, 192, 192, alpha ),
            "orange" => new( 255, 165, 0, alpha ),
            _ => new( 0, 0, 0, -1 ),
        };
    }

    private static string NormalizeName( string name )
    {
        return string.IsNullOrWhiteSpace( name )
            ? string.Empty
            : name.Trim().Replace( " ", string.Empty, StringComparison.Ordinal ).Replace( "-", string.Empty, StringComparison.Ordinal ).ToLowerInvariant();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Shared default report color resolver instance.
    /// </summary>
    public static ReportDefaultColorResolver Instance { get; } = new();

    #endregion
}