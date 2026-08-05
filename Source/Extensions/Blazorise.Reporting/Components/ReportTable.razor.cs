#region Using directives
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a table element used to organize report fields and text.
/// </summary>
public partial class ReportTable
{
    #region Members

    private readonly ReportTableContext tableContext = new();

    private bool declarativeContent;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override ReportElementType ElementType => ReportElementType.Table;

    /// <inheritdoc />
    protected override bool HasDefinitionChanged( ParameterView parameters )
    {
        return base.HasDefinitionChanged( parameters )
            || parameters.IsParameterChanged( RowCount )
            || parameters.IsParameterChanged( ColumnCount )
            || parameters.TryGetParameter( ChildContent,
                value => ( value is null ) == ( ChildContent is null ),
                out ComponentParameterInfo<RenderFragment> childContentParameter ) && childContentParameter.Changed;
    }

    /// <inheritdoc />
    protected override ReportElementDefinition BuildDefinition()
    {
        ReportTableElementDefinition definition = (ReportTableElementDefinition)base.BuildDefinition();
        bool hasDeclarativeContent = ChildContent is not null;

        if ( declarativeContent != hasDeclarativeContent )
        {
            definition.Columns.Clear();
            definition.Rows.Clear();
            definition.Cells.Clear();
            declarativeContent = hasDeclarativeContent;
        }

        if ( !hasDeclarativeContent )
            Internal.ReportDefinitionHelper.EnsureTableLayout( definition, RowCount, ColumnCount );

        tableContext.Definition = definition;
        tableContext.DefinitionChanged = RegisteredContainerContext is null
            ? null
            : new System.Action( RegisteredContainerContext.NotifyDefinitionChanged );

        return definition;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Number of rows created when no explicit table rows or cells are declared.
    /// </summary>
    [Parameter] public int RowCount { get; set; } = 2;

    /// <summary>
    /// Number of columns created when no explicit table columns or cells are declared.
    /// </summary>
    [Parameter] public int ColumnCount { get; set; } = 2;

    /// <summary>
    /// Rows, cells, and nested report elements declared inside the report table.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}