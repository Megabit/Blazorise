#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a column inside a report table element.
/// </summary>
public partial class ReportTableColumn : ComponentBase, IDisposable
{
    #region Members

    private readonly ReportTableColumnDefinition definition = new();

    private ReportTableContext registeredTableContext;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        bool definitionChanged = registeredTableContext is null
            || parameters.IsParameterChanged( Title )
            || parameters.IsParameterChanged( Field )
            || parameters.IsParameterChanged( Format )
            || parameters.IsParameterChanged( Width );

        await base.SetParametersAsync( parameters );

        bool contextChanged = !ReferenceEquals( registeredTableContext, TableContext );

        if ( contextChanged )
        {
            if ( registeredTableContext?.Definition is not null )
                registeredTableContext.Definition.Columns.Remove( definition );

            registeredTableContext = TableContext;

            if ( registeredTableContext?.Definition is not null )
                registeredTableContext.Definition.Columns.Add( definition );
        }

        if ( definitionChanged )
        {
            definition.Title = Title;
            definition.Field = Field;
            definition.Format = Format;
            definition.Width = Width;
        }

        if ( definitionChanged || contextChanged )
            registeredTableContext?.NotifyDefinitionChanged();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if ( registeredTableContext?.Definition?.Columns.Remove( definition ) == true )
            registeredTableContext.NotifyDefinitionChanged();

        registeredTableContext = null;
    }

    #endregion

    #region Properties

    [CascadingParameter] internal ReportTableContext TableContext { get; set; }

    /// <summary>
    /// Header text displayed for the table column.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Field name rendered by cells in this column.
    /// </summary>
    [Parameter] public string Field { get; set; }

    /// <summary>
    /// Format applied to column values.
    /// </summary>
    [Parameter] public ReportFormatDefinition Format { get; set; }

    /// <summary>
    /// Column width in points.
    /// </summary>
    [Parameter] public double Width { get; set; } = 90;

    #endregion
}