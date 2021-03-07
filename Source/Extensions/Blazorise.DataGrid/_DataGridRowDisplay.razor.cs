#region Using directives

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;

#endregion

namespace Blazorise.DataGrid
{
    public partial class _DataGridRowDisplay<TItem> : _DataGridRow<TItem>
    {

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

    }
}