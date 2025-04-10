using MongoDB.Bson;
namespace Blazorise.DataGrid.Exporters.Bson;
public class BsonToFileExporter( BsonFileExportOptions? options=null ) : DataGridExporterToFileBinary<BsonFileExportOptions, ExportResult >( options )
{
    public override async Task<byte[]> GetDataForExport(List<List<object>> data, List<string> columnNames)
    {
        var bsonDocuments = new List<BsonDocument>();

        foreach (var row in data)
        {
            var doc = new BsonDocument();

            for (int i = 0; i < columnNames.Count && i < row.Count; i++)
            {
                var key = columnNames[i];
                var value = row[i];

                doc.Add(key, BsonValue.Create(value));
            }

            bsonDocuments.Add(doc);
        }

        var rootDocument = new BsonDocument
                           {
                           { "data", new BsonArray(bsonDocuments) }
                           };

        return rootDocument.ToBson();
    }

}

public class BsonFileExportOptions : FileExportOptions
{
    public override string FileExtension { get; init; } = "bson";
    public override string MimeType { get; init; } = "application/bson";
    public bool IncludeTypeInformation { get; init; } = true;
}







