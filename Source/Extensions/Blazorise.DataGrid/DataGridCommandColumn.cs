#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

public partial class DataGridCommandColumn<TItem> : DataGridColumn<TItem>
{
    #region Properties

    public override DataGridColumnType ColumnType => DataGridColumnType.Command;

    /// <summary>
    /// Template to customize new command button.
    /// </summary>
    [Parameter] public RenderFragment<NewCommandContext<TItem>> NewCommandTemplate { get; set; }

    /// <summary>
    /// Template to customize edit command button.
    /// </summary>
    [Parameter] public RenderFragment<EditCommandContext<TItem>> EditCommandTemplate { get; set; }

    /// <summary>
    /// Template to customize save command button.
    /// </summary>
    [Parameter] public RenderFragment<CommandContext<TItem>> SaveCommandTemplate { get; set; }

    /// <summary>
    /// Template to customize save batch command button.
    /// </summary>
    [Parameter] public RenderFragment<CommandContext> SaveBatchCommandTemplate { get; set; }

    /// <summary>
    /// Template to customize cancel batch command button.
    /// </summary>
    [Parameter] public RenderFragment<CommandContext> CancelBatchCommandTemplate { get; set; }

    /// <summary>
    /// Template to customize cancel command button.
    /// </summary>
    [Parameter] public RenderFragment<CommandContext<TItem>> CancelCommandTemplate { get; set; }

    /// <summary>
    /// Template to customize delete command button.
    /// </summary>
    [Parameter] public RenderFragment<DeleteCommandContext<TItem>> DeleteCommandTemplate { get; set; }

    /// <summary>
    /// Template to customize clear-filter command button.
    /// </summary>
    [Parameter] public RenderFragment<CommandContext<TItem>> ClearFilterCommandTemplate { get; set; }

    /// <summary>
    /// Handles the visibility of new command button.
    /// </summary>
    [Parameter] public bool NewCommandAllowed { get; set; } = true;

    /// <summary>
    /// Handles the visibility of edit command button.
    /// </summary>
    [Parameter] public bool EditCommandAllowed { get; set; } = true;

    /// <summary>
    /// Handles the visibility of save command button.
    /// </summary>
    [Parameter] public bool SaveCommandAllowed { get; set; } = true;

    /// <summary>
    /// Handles the visibility of save batch command button.
    /// </summary>
    [Parameter] public bool SaveBatchCommandAllowed { get; set; } = true;

    /// <summary>
    /// Handles the visibility of cancel batch command button.
    /// </summary>
    [Parameter] public bool CancelBatchCommandAllowed { get; set; } = true;

    /// <summary>
    /// Handles the visibility of cancel command button.
    /// </summary>
    [Parameter] public bool CancelCommandAllowed { get; set; } = true;

    /// <summary>
    /// Handles the visibility of delete command button.
    /// </summary>
    [Parameter] public bool DeleteCommandAllowed { get; set; } = true;

    /// <summary>
    /// Handles the visibility of clear-filter command button.
    /// </summary>
    [Parameter] public bool ClearFilterCommandAllowed { get; set; } = true;

    #endregion
}