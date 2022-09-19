#region Using directives
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.TreeView
{
    public partial class _TreeViewNode : BaseComponent
    {
        #region Members

        private bool expanded = true;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "tree-view" );
            builder.Append( "tree-view-collapsed", !IsExpanded );

            base.BuildClasses( builder );
        }


        protected void OnToggleNode(TreeNode node)
        {
            IsExpanded = !node.IsExpanded;
            node.IsExpanded = !node.IsExpanded;

            InvokeAsync(StateHasChanged);
        }

        private Action<TreeNode, NodeStyling> ResolveNodeStylingAction( Action<TreeNode, NodeStyling> action )
        {
            return action ?? new Action<TreeNode, NodeStyling>( ( item, style ) => { return; } );
        }

        #endregion

        #region Properties

        [Parameter] public IEnumerable<TreeNode> Nodes { get; set; }

        [Parameter] public RenderFragment<TreeNode> NodeContent { get; set; }

        [Parameter] public Func<TreeNode, IEnumerable<TreeNode>> GetChildNodes { get; set; }

        [Parameter] public Func<TreeNode, bool> HasChildNodes { get; set; } = node => true;

        [Parameter] public bool IsExpanded { get; set; } = true;

        [Parameter] public bool AllExpanded { get; set; }

        /// <summary>
        /// Defines the name of the treenode expand icon.
        /// </summary>
        [Parameter] public IconName ExpandIconName { get; set; }

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
        [Parameter] public IconName CollapseIconName { get; set; }

        /// <summary>
        /// Defines the style of the treenode collapse icon.
        /// </summary>
        [Parameter] public IconStyle? CollapseIconStyle { get; set; }

        /// <summary>
        /// Defines the size of the treenode collapse icon.
        /// </summary>
        [Parameter] public IconSize? CollapseIconSize { get; set; }

        [CascadingParameter] public _TreeViewNode ParentNode { get; set; }

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
    }
}