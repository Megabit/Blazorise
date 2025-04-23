using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Blazorise.Exporters.Bson;

/// <summary>
/// Exports tabular data to BSON format for file storage. It converts each row into a BSON document and wraps them in a
/// root document.
/// </summary>
public class BsonToFileExporter : BinaryExporterToFile<BsonFileExportOptions, ExportResult, TabularSourceData<object>>
{
    /// <summary>
    /// Initializes a new instance of the BsonToFileExporter class with optional export settings.
    /// </summary>
    /// <param name="options">Specifies the configuration for exporting BSON files.</param>
    public BsonToFileExporter( BsonFileExportOptions options = null )
        : base( options )
    {
    }

    /// <summary>
    /// Converts tabular data into BSON format for export.
    /// </summary>
    /// <param name="dataSource">Contains the data and column names to be transformed into BSON documents.</param>
    /// <returns>A task that resolves to a byte array representing the BSON formatted data.</returns>
    public override Task<byte[]> GetDataForExport( TabularSourceData<object> dataSource )
    {
        var bsonDocuments = new List<BsonDocument>();

        foreach ( var row in dataSource.Data )
        {
            var doc = new BsonDocument();

            for ( int i = 0; i < dataSource.ColumnNames.Count && i < row.Count; i++ )
            {
                var key = dataSource.ColumnNames[i];
                var value = row[i];

                doc.Add( key, BsonValue.Create( value ) );
            }

            bsonDocuments.Add( doc );
        }

        var rootDocument = new BsonDocument
        {
            { "data", new BsonArray(bsonDocuments) }
        };

        return Task.FromResult( rootDocument.ToBson() );
    }
}