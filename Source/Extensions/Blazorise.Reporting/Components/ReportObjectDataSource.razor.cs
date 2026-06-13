#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares an object model data source available to report bands and fields.
/// </summary>
public partial class ReportObjectDataSource : ReportDataSourceComponentBase
{
    #region Methods

    /// <inheritdoc />
    protected override ReportDataSourceDefinition CreateDataSourceDefinition()
    {
        return new()
        {
            Name = Name,
            Type = ObjectReportDataSourceProvider.ProviderType,
            Data = Data,
            Schema = Schema,
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Name used by report bands and fields to reference this object data source.
    /// </summary>
    [Parameter] public string Name { get; set; } = "Default";

    /// <summary>
    /// Object or enumerable exposed to the report designer and preview renderer, including nested objects and collections.
    /// </summary>
    [Parameter] public object Data { get; set; }

    /// <summary>
    /// Optional field schema used instead of schema inferred from the object model.
    /// </summary>
    [Parameter] public ReportDataSourceSchema Schema { get; set; }

    #endregion
}