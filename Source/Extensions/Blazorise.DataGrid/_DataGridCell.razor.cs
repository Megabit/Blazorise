#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
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

        #region Methods

        #endregion

        #region Properties

        protected bool HasValidator
            => Column.Validator != null;

        protected bool HasValidationPattern
            => !string.IsNullOrWhiteSpace( Column.ValidationPattern );

        protected Action<ValidatorEventArgs> Validator
            => Column.Validator ?? EmptyValidator;

        protected string ValidationPattern
            => string.IsNullOrWhiteSpace( Column.ValidationPattern ) ? null : Column.ValidationPattern;

        [Parameter] public DataGridColumn<TItem> Column { get; set; }

        [Parameter] public TItem Item { get; set; }

        [Parameter] public CellEditContext<TItem> CellEditContext { get; set; }

        [Parameter] public EventCallback<object> CellValueChanged { get; set; }

        [Parameter] public bool ShowValidationFeedback { get; set; }

        #endregion
    }
}
