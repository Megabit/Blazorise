#region Using directives
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridCell<TItem> : ComponentBase
    {
        #region Members

        private static readonly Action<ValidatorEventArgs> EmptyValidator = ( args ) => { args.Status = ValidationStatus.Success; };

        #endregion

        #region Properties

        protected bool UseValidation
            => ParentDataGrid.UseValidation;

        protected bool HasValidator
            => Column.Validator != null || Column.AsyncValidator != null;

        protected bool HasValidationPattern
            => !string.IsNullOrWhiteSpace( Column.ValidationPattern );

        protected Action<ValidatorEventArgs> Validator
            => Column.Validator ?? EmptyValidator;

        protected Func<ValidatorEventArgs, CancellationToken, Task> AsyncValidator
            => Column.AsyncValidator; // AsyncValidator must be defined explicitelly. We don't want to have an EmptyAsyncValidator.

        protected string ValidationPattern
            => string.IsNullOrWhiteSpace( Column.ValidationPattern ) ? null : Column.ValidationPattern;

        protected Type ValidationHandlerType
        {
            get
            {
                if ( HasValidationPattern )
                    return typeof( PatternValidationHandler );
                else if ( HasValidator )
                    return typeof( ValidatorValidationHandler );

                // default is always data-annotations
                return typeof( DataAnnotationValidationHandler );
            }
        }

        /// <summary>
        /// Gets or sets the parent <see cref="DataGrid{TItem}"/> of the this component.
        /// </summary>
        [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

        [Parameter] public DataGridColumn<TItem> Column { get; set; }

        [Parameter] public TItem Item { get; set; }

        [Parameter] public TItem ValidationItem { get; set; }

        [Parameter] public CellEditContext<TItem> CellEditContext { get; set; }

        [Parameter] public EventCallback<object> CellValueChanged { get; set; }

        [Parameter] public bool ShowValidationFeedback { get; set; }

        #endregion
    }
}