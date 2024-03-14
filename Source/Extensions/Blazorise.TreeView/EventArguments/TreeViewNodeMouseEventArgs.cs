#region Using directives
using System;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.TreeView;

/// <summary>
/// Represents the arguments for a mouse event on a node within a TreeView control.
/// This class provides all necessary information about the event, including the node involved and the specifics of the mouse action.
/// </summary>
/// <typeparam name="TNode">The type of the node involved in the mouse event. This allows for strong typing of the node object.</typeparam>
public class TreeViewNodeMouseEventArgs<TNode> : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TreeViewNodeMouseEventArgs{TNode}"/> class.
    /// </summary>
    /// <param name="node">The node that was clicked or otherwise interacted with during the mouse event. This is the specific instance of <typeparamref name="TNode"/> that the event pertains to.</param>
    /// <param name="mouseEventArgs">The details of the mouse event, including which mouse button was pressed, the number of clicks, and the position of the mouse. This parameter provides access to the underlying <see cref="MouseEventArgs"/> instance associated with the event.</param>
    public TreeViewNodeMouseEventArgs( TNode node, MouseEventArgs mouseEventArgs )
    {
        Node = node;
        MouseEventArgs = mouseEventArgs;
    }

    /// <summary>
    /// Gets the node that is the target of the mouse event.
    /// This property provides access to the specific node within the TreeView that the event is related to.
    /// </summary>
    public TNode Node { get; }

    /// <summary>
    /// Gets the details of the mouse event, including information about which button was pressed, the state of the mouse buttons, and the position of the mouse cursor at the time of the event.
    /// </summary>
    public MouseEventArgs MouseEventArgs { get; }
}
