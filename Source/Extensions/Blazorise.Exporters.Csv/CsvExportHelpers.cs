using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorise.Exporters.Csv;

/// <summary>
/// Generates a CSV formatted string from provided data and headers. It includes an option to export headers and escapes
/// special characters.
/// </summary>
public static class CsvExportHelpers
{
    /// <summary>
    /// Generates a CSV formatted string from provided data and headers, with optional header export.
    /// </summary>
    /// <param name="data">Contains the rows of data to be included in the CSV output.</param>
    /// <param name="headers">Defines the column headers for the CSV output if header export is enabled.</param>
    /// <param name="options">Specifies options that control the export behavior, such as whether to include headers.</param>
    /// <returns>Returns a string representing the CSV formatted data.</returns>
    public static Task<string> GetDataForText( List<List<string>> data, List<string> headers, ICsvExportOptions options )
    {
        var sb = new StringBuilder();

        if ( options?.ExportHeader == true )
        {
            sb.AppendLine( string.Join( ",", headers.Select( EscapeCsvField ) ) );
        }

        foreach ( var row in data )
        {
            sb.AppendLine( string.Join( ",", row.Select( EscapeCsvField ) ) );
        }

        return Task.FromResult( sb.ToString() );
    }

    static string EscapeCsvField( string field )
    {
        if ( field.Contains( '"' ) || field.Contains( ',' ) || field.Contains( '\n' ) || field.Contains( '\r' ) )
        {
            var escaped = field.Replace( "\"", "\"\"" );

            return $"\"{escaped}\"";
        }

        return field;
    }
}