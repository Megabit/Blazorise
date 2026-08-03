#region Using directives
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid.Internal;

/// <summary>
/// Supports base data grid cell behavior in DataGrid components.
/// </summary>
public abstract class _BaseDataGridCell<TItem> : ComponentBase
{
    #region Members

    private static readonly Action<ValidatorEventArgs> EmptyValidator = ( args ) => { args.Status = ValidationStatus.Success; };

    #endregion

    #region Methods

    /// <summary>
    /// Updated the internal cell values.
    /// </summary>
    /// <param name="value">Value that is updating.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnEditValueChanged( object value )
    {
        CellEditContext.CellValue = value;

        if ( ValidationItem != null )
            Column.SetValue( ValidationItem, value );

        return CellValueChanged.InvokeAsync( value );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Controls member behavior for the base data grid cell.
    /// </summary>
    protected bool UseValidation
        => ParentDataGrid.UseValidation;

    /// <summary>
    /// Controls member behavior for the base data grid cell.
    /// </summary>
    protected bool HasValidator
        => Column.Validator != null || Column.AsyncValidator != null;

    /// <summary>
    /// Controls member behavior for the base data grid cell.
    /// </summary>
    protected bool HasValidationPattern
        => !string.IsNullOrWhiteSpace( Column.ValidationPattern );

    /// <summary>
    /// Callback invoked for member.
    /// </summary>
    protected Action<ValidatorEventArgs> Validator
        => Column.Validator ?? EmptyValidator;

    /// <summary>
    /// Callback invoked for member.
    /// </summary>
    protected Func<ValidatorEventArgs, CancellationToken, Task> AsyncValidator
        => Column.AsyncValidator; // AsyncValidator must be defined explicitelly. We don't want to have an EmptyAsyncValidator.

    /// <summary>
    /// Regular-expression pattern applied during cell validation.
    /// </summary>
    protected string ValidationPattern
        => string.IsNullOrWhiteSpace( Column.ValidationPattern ) ? null : Column.ValidationPattern;

    /// <summary>
    /// Validation Handler Type controlling how the base data grid cell behaves.
    /// </summary>
    protected Type ValidationHandlerType
    {
        get
        {
            if ( HasValidationPattern )
                return typeof( PatternValidationHandler );
            else if ( HasValidator )
                return typeof( ValidatorValidationHandler );

            if ( ParentDataGrid?.ValidationsHandlerType is not null )
                return ParentDataGrid.ValidationsHandlerType;

            // default is always data-annotations
            return typeof( DataAnnotationValidationHandler );
        }
    }

    /// <summary>
    /// Specifies the parent <see cref="DataGrid{TItem}"/> of the this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    /// <summary>
    /// Identifies the column containing this editable cell.
    /// </summary>
    [Parameter] public DataGridColumn<TItem> Column { get; set; }

    /// <summary>
    /// Identifies the row item containing the cell.
    /// </summary>
    [Parameter] public TItem Item { get; set; }

    /// <summary>
    /// Holds the model against which cell validation runs.
    /// </summary>
    [Parameter] public TItem ValidationItem { get; set; }

    /// <summary>
    /// Carries the mutable value and metadata for the cell edit.
    /// </summary>
    [Parameter] public CellEditContext<TItem> CellEditContext { get; set; }

    /// <summary>
    /// Reports a newly entered cell value.
    /// </summary>
    [Parameter] public EventCallback<object> CellValueChanged { get; set; }

    /// <summary>
    /// Controls whether validation messages appear beside the cell.
    /// </summary>
    [Parameter] public bool ShowValidationFeedback { get; set; }

    #endregion
}