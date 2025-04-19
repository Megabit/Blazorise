using MongoDB.Bson;
namespace Blazorise.Exporters.Bson;
public class BsonToFileExporter( BsonFileExportOptions? options=null ) : BinaryExporterToFile<BsonFileExportOptions, ExportResult, TabularSourceData<object>>( options )
{
    public override async Task<byte[]> GetDataForExport( TabularSourceData<object> dataSource )
    {
        var bsonDocuments = new List<BsonDocument>();

        foreach (var row in dataSource.Data )
        {
            var doc = new BsonDocument();

            for (int i = 0; i < dataSource.ColumnNames.Count && i < row.Count; i++)
            {
                var key = dataSource.ColumnNames[i];
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







