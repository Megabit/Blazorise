#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.TreeView
{
    public partial class TreeView : BaseComponent
    {
        #region Members

        private TreeViewState<TreeNode> state = new()
        {
        };

        #endregion

        #region Methods

        public void SelectNode(TreeNode node)
        {
            SelectedNode = node;

            InvokeAsync( StateHasChanged );
        }

        #endregion

        #region Properties

        [Parameter] public bool ExpandAll { get; set; }

        /// <summary>
        /// Defines the name of the treenode expand icon.
        /// </summary>
        [Parameter] public IconName ExpandIconName { get; set; } = IconName.ChevronRight;

        /// <summary>
        /// Defines the style of the treenode expand icon.
        /// </summary>
        [Parameter] public IconStyle? ExpandIconStyle { get; set; }

        /// <summary>
        /// Defines the size of the treenode expand icon.
        /// </summary>
        [Parameter] public IconSize? ExpandIconSize { get; set; }

        /// <summary>
        /// Defines the name of the treenode collapse icon.
        /// </summary>
        [Parameter] public IconName CollapseIconName { get; set; } = IconName.ChevronDown;

        /// <summary>
        /// Defines the style of the treenode collapse icon.
        /// </summary>
        [Parameter] public IconStyle? CollapseIconStyle { get; set; }

        /// <summary>
        /// Defines the size of the treenode collapse icon.
        /// </summary>
        [Parameter] public IconSize? CollapseIconSize { get; set; }

        /// <summary>
        /// Collection of child TreeView items (child nodes)
        /// </summary>
        [Parameter] public IEnumerable<TreeNode> Nodes { get; set; }

        /// <summary>
        /// Template to display content for the node
        /// </summary>
        [Parameter] public RenderFragment<TreeNode> NodeContent { get; set; }

        /// <summary>
        /// Currently selected TreeView item/node.
        /// </summary>
        [Parameter]
        public TreeNode SelectedNode
        {
            get => state.SelectedNode;
            set
            {
                if ( state.SelectedNode.IsEqual( value ) )
                    return;

                state.SelectedNode = value;

                SelectedNodeChanged.InvokeAsync( state.SelectedNode );

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs when the selected TreeView node has changed.
        /// </summary>
        [Parameter] public EventCallback<TreeNode> SelectedNodeChanged { get; set; }

        /// <summary>
        /// Gets the list of child nodes for each node.
        /// </summary>
        [Parameter] public Func<TreeNode, IEnumerable<TreeNode>> GetChildNodes { get; set; }

        /// <summary>
        /// Indicates if the node has child elements.
        /// </summary>
        [Parameter] public Func<TreeNode, bool> HasChildNodes { get; set; } = node => true;

        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets selected node styling.
        /// </summary>
        [Parameter] public Action<TreeNode, NodeStyling> SelectedNodeStyling { get; set; }

        /// <summary>
        /// Gets or sets node styling.
        /// </summary>
        [Parameter] public Action<TreeNode, NodeStyling> NodeStyling { get; set; }

        #endregion

        protected override void OnInitialized()
        {
            if (ExpandAll)
            {
                ExpandAllNodes(Nodes);
            }

            base.OnInitialized();
        }

        private void ExpandAllNodes(IEnumerable<TreeNode> nodes)
        {
            foreach (var item in nodes)
            {
                item.IsExpanded = true;
                if (item.Children != null)
                {
                    ExpandAllNodes(item.Children);
                }
            }
        }
    }
}