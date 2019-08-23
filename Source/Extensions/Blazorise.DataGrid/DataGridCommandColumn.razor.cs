#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class BaseDataGridCommandColumn<TItem> : BaseDataGridColumn<TItem>
    {
        #region Members

        #endregion

        #region Methods

        #endregion

        #region Properties

        public override DataGridColumnType ColumnType => DataGridColumnType.Command;

        /// <summary>
        /// Template to customize new command button.
        /// </summary>
        [Parameter] public RenderFragment<CommandContext> NewCommandTemplate { get; set; }

        /// <summary>
        /// Template to customize edit command button.
        /// </summary>
        [Parameter] public RenderFragment<CommandContext<TItem>> EditCommandTemplate { get; set; }

        /// <summary>
        /// Template to customize save command button.
        /// </summary>
        [Parameter] public RenderFragment<CommandContext<TItem>> SaveCommandTemplate { get; set; }

        /// <summary>
        /// Template to customize cancel command button.
        /// </summary>
        [Parameter] public RenderFragment<CommandContext<TItem>> CancelCommandTemplate { get; set; }

        /// <summary>
        /// Template to customize delete command button.
        /// </summary>
        [Parameter] public RenderFragment<CommandContext<TItem>> DeleteCommandTemplate { get; set; }

        /// <summary>
        /// Template to customize clear-filter command button.
        /// </summary>
        [Parameter] public RenderFragment<CommandContext> ClearFilterCommandTemplate { get; set; }

        #endregion
    }
}
