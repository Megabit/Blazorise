#region Using directives

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;

#endregion

namespace Blazorise.DataGrid
{
    public partial class _DataGridRowDisplay<TItem>
    {

        #region Properties

        /// <summary>
        /// Gets or sets whether it should display an EditRow.
        /// </summary>
        [Parameter] public bool IsEdit { get; set; }

        /// <summary>
        /// Specifes the EditRow editing mode.
        /// </summary>
        [Parameter] public DataGridEditMode EditMode { get; set; } = DataGridEditMode.Form;

        /// <summary>
        /// Gets or sets the edit item.
        /// </summary>
        [Parameter] public TItem EditItem { get; set; }

        /// <summary>
        /// Gets or sets the EditRow cell values.
        /// </summary>
        [Parameter] public Dictionary<string, CellEditContext<TItem>> EditItemCellValues { get; set; }

        /// <summary>
        /// Gets or sets whether the row should display a DetailRowTemplate.
        /// </summary>
        [Parameter] public bool DisplayDetailRowTemplate { get; set; }

        /// <summary>
        /// Gets or sets the DetailRowTemplate.
        /// </summary>
        [Parameter] public RenderFragment<TItem> DetailRowTemplate { get; set; }

        /// <summary>
        /// Item associated with the data set.
        /// </summary>
        [Parameter] public TItem Item { get; set; }

        /// <summary>
        /// List of columns used to build this row.
        /// </summary>
        [Parameter] public IEnumerable<DataGridColumn<TItem>> Columns { get; set; }

        /// <summary>
        /// List of display columns used to build the view row.
        /// </summary>
        [Parameter] public IEnumerable<DataGridColumn<TItem>> DisplayableColumns { get; set; }

        /// <summary>
        /// Occurs after the row is selected.
        /// </summary>
        [Parameter] public EventCallback<TItem> Selected { get; set; }

        /// <summary>
        /// Occurs after the row is clicked.
        /// </summary>
        [Parameter] public EventCallback<DataGridRowMouseEventArgs<TItem>> Clicked { get; set; }

        /// <summary>
        /// Occurs after the row is double clicked.
        /// </summary>
        [Parameter] public EventCallback<DataGridRowMouseEventArgs<TItem>> DoubleClicked { get; set; }

        /// <summary>
        /// Activates the multi select command.
        /// </summary>
        [Parameter] public EventCallback<MultiSelectEventArgs<TItem>> MultiSelect { get; set; }

        /// <summary>
        /// Gets or sets the applied cursor when the row is hovered over.
        /// </summary>
        [Parameter] public Cursor HoverCursor { get; set; }

        [Parameter]
        public EventCallback<TItem> Edit { get; set; }

        [Parameter]
        public EventCallback<TItem> Delete { get; set; }

        [Parameter]
        public EventCallback Save { get; set; }

        [Parameter]
        public EventCallback Cancel { get; set; }

        #endregion

    }
}