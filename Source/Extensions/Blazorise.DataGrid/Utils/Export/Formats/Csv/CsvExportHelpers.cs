using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorise.DataGrid;

static class CsvExportHelpers
{
    internal static async Task<string> GetDataForText(List<List<string>> data, List<string> headers, ICsvExportOptions options)
    {
        var sb = new StringBuilder();

        if (options?.ExportHeader == true)
        {
            sb.AppendLine(string.Join(",", headers.Select(EscapeCsvField)));
        }

        foreach (var row in data)
        {
            sb.AppendLine(string.Join(",", row.Select(EscapeCsvField)));
        }

        await Task.CompletedTask;
        return sb.ToString();
    }

    static string EscapeCsvField(string field)
    {
        if (field.Contains('"') || field.Contains(',') || field.Contains('\n') || field.Contains('\r'))
        {
            var escaped = field.Replace("\"", "\"\"");
            return $"\"{escaped}\"";
        }

        return field;
    }
}
