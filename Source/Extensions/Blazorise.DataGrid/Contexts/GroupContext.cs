#region Using directives
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Context for a group.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class GroupContext<TItem>
{
    #region Constructors

    /// <summary>
    /// Constructor for event handler.
    /// </summary>

    public GroupContext( IGrouping<object, TItem> group )
    {
        Key = group.Key;
        Items = group.AsEnumerable();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the group key.
    /// </summary>
    public object Key { get; }

    /// <summary>
    /// Gets the group values.
    /// </summary>
    public IEnumerable<TItem> Items { get; }

    #endregion
}