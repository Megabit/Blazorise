using System;
using Blazorise;

namespace Blazorise.TreeView;

/// <summary>
/// Component classes for <see cref="TreeView{TNode}"/>.
/// </summary>
/// <typeparam name="TNode">Type of the node.</typeparam>
public sealed record class TreeViewClasses<TNode> : ComponentClasses
{
    /// <summary>
    /// Targets the node list container element.
    /// </summary>
    public string Nodes { get; init; }

    /// <summary>
    /// Targets the node wrapper element.
    /// </summary>
    public Func<TreeViewNodeContext<TNode>, string> Node { get; init; }

    /// <summary>
    /// Targets the node icon element.
    /// </summary>
    public string NodeIcon { get; init; }

    /// <summary>
    /// Targets the node title container element.
    /// </summary>
    public string NodeTitle { get; init; }

    /// <summary>
    /// Targets the node check container element.
    /// </summary>
    public string NodeCheck { get; init; }

    /// <summary>
    /// Targets the node content element.
    /// </summary>
    public string NodeContent { get; init; }
}

/// <summary>
/// Component styles for <see cref="TreeView{TNode}"/>.
/// </summary>
/// <typeparam name="TNode">Type of the node.</typeparam>
public sealed record class TreeViewStyles<TNode> : ComponentStyles
{
    /// <summary>
    /// Targets the node list container element.
    /// </summary>
    public string Nodes { get; init; }

    /// <summary>
    /// Targets the node wrapper element.
    /// </summary>
    public Func<TreeViewNodeContext<TNode>, string> Node { get; init; }

    /// <summary>
    /// Targets the node icon element.
    /// </summary>
    public string NodeIcon { get; init; }

    /// <summary>
    /// Targets the node title container element.
    /// </summary>
    public string NodeTitle { get; init; }

    /// <summary>
    /// Targets the node check container element.
    /// </summary>
    public string NodeCheck { get; init; }

    /// <summary>
    /// Targets the node content element.
    /// </summary>
    public string NodeContent { get; init; }
}