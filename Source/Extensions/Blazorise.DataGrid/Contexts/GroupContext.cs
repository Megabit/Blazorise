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
        Key = group.Key.ToString();
        Items = group.AsEnumerable();
    }

    /// <summary>
    /// Constructor for event handler.
    /// </summary>
    public GroupContext( IGrouping<object, TItem> group, RenderFragment<GroupContext<TItem>> groupTemplate ) : this( group )
    {
        GroupTemplate = groupTemplate;
    }

    #endregion

    #region Methods

    internal void SetExpanded( bool expanded )
    {
        Expanded = expanded;
    }

    internal void SetNestedGroup( IEnumerable<GroupContext<TItem>> nestedGroup )
        => NestedGroup = nestedGroup;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the next nested group collection if it exists.
    /// </summary>
    public IEnumerable<GroupContext<TItem>> NestedGroup { get; private set; }

    /// <summary>
    /// Gets the group key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the group values.
    /// </summary>
    public IEnumerable<TItem> Items { get; }

    /// <summary>
    /// Gets whether the group is expanded.
    /// </summary>
    public bool Expanded { get; private set; }

    /// <summary>
    /// Gets the Template for this group.
    /// </summary>
    public RenderFragment<GroupContext<TItem>> GroupTemplate { get; private set; }

    #endregion
}