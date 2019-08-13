#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridRow<TItem> : BaseComponent
    {
        #region Members

        /// <summary>
        /// Holds the internal value for every cell in the row.
        /// </summary>
        protected Dictionary<string, CellEditContext> cellsValues = new Dictionary<string, CellEditContext>();

        #endregion

        #region Methods

        protected override Task OnFirstAfterRenderAsync()
        {
            // initialise all internal cell values
            foreach ( var column in Columns )
            {
                if ( column.ColumnType == DataGridColumnType.Command )
                    continue;

                cellsValues.Add( column.ElementId, new CellEditContext
                {
                    CellValue = column.GetValue( Item ),
                } );
            }

            return base.OnFirstAfterRenderAsync();
        }

        protected internal Task OnSelectedCommand()
        {
            return Selected.InvokeAsync( Item );
        }

        protected internal Task OnEditCommand()
        {
            return Edit.InvokeAsync( Item );
        }

        protected internal Task OnDeleteCommand()
        {
            return Delete.InvokeAsync( Item );
        }

        protected internal Task OnSaveCommand()
        {
            return Save.InvokeAsync( Item );
        }

        protected internal Task OnCancelCommand()
        {
            return Cancel.InvokeAsync( Item );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Item associated with the data set.
        /// </summary>
        [Parameter] public TItem Item { get; set; }

        /// <summary>
        /// List of columns used to build this row.
        /// </summary>
        [Parameter] public IEnumerable<BaseDataGridColumn<TItem>> Columns { get; set; }

        [CascadingParameter] protected BaseDataGrid<TItem> ParentDataGrid { get; set; }

        /// <summary>
        /// Occurs after the row is selected.
        /// </summary>
        [Parameter] public EventCallback<TItem> Selected { get; set; }

        /// <summary>
        /// Activates the edit command for current item.
        /// </summary>
        [Parameter] public EventCallback<TItem> Edit { get; set; }

        /// <summary>
        /// Activates the delete command for current item.
        /// </summary>
        [Parameter] public EventCallback<TItem> Delete { get; set; }

        /// <summary>
        /// Activates the save command.
        /// </summary>
        [Parameter] public EventCallback Save { get; set; }

        /// <summary>
        /// Activates the cancel command.
        /// </summary>
        [Parameter] public EventCallback Cancel { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
