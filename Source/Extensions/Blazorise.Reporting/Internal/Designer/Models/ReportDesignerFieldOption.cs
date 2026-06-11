#region Using directives
using System;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Represents a field candidate shown by designer dialogs.
/// </summary>
public sealed class ReportDesignerFieldOption
{
    #region Properties

    /// <summary>
    /// Index of the detail band that provides the field context.
    /// </summary>
    public int SourceSectionIndex { get; set; }

    /// <summary>
    /// Data source name or path used to resolve the field values.
    /// </summary>
    public string DataSourceName { get; set; }

    /// <summary>
    /// Field path resolved inside the data source context.
    /// </summary>
    public string FieldName { get; set; }

    /// <summary>
    /// Label shown for the field in designer dialogs.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Field value type used to determine supported aggregate functions.
    /// </summary>
    public Type DataType { get; set; }

    #endregion
}