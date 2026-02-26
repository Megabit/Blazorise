#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Transaction that applies an insert or update operation.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public class GanttEditingTransaction<TItem> : GanttTransaction<TItem>
{
    #region Constructors

    /// <summary>
    /// Creates a new editing transaction.
    /// </summary>
    public GanttEditingTransaction( Gantt<TItem> gantt, TItem targetItem, TItem editedItem, GanttEditState editState )
        : base( gantt, editedItem )
    {
        TargetItem = targetItem;
        EditState = editState;
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override Task<bool> CommitImpl()
    {
        return Gantt.CommitEditInternalAsync( TargetItem, Item, EditState );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Target item to update.
    /// </summary>
    private TItem TargetItem { get; }

    /// <summary>
    /// Edit operation state.
    /// </summary>
    private GanttEditState EditState { get; }

    #endregion
}