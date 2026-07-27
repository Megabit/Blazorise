namespace Blazorise.Reporting.DataSources.Csv;

internal sealed class CsvReportDataSourceEditorOption
{
    #region Constructors

    public CsvReportDataSourceEditorOption( string value, string text )
    {
        Value = value;
        Text = text;
    }

    #endregion

    #region Properties

    public string Value { get; }

    public string Text { get; }

    #endregion
}