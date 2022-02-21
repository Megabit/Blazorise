#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.TreeView
{
    public partial class _TreeViewNodeContent<TNode> : BaseComponent
    {
        #region Constructors

        public _TreeViewNodeContent()
        {
            selectedNodeStyling = new()
            {
                Background = Background.Primary,
                TextColor = TextColor.White
            };

            nodeStyling = new()
            {
                Background = Background.Default,
                TextColor = TextColor.Default
            };
        }

        #endregion

        #region Members

        private TreeViewState<TNode> treeViewState;

        private NodeStyling selectedNodeStyling;

        private NodeStyling nodeStyling;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( $"{ClassProvider.Spacing( Spacing.Padding, SpacingSize.Is1, Side.All, Breakpoint.None )} cursor-pointer" );

            if ( Selected )
                builder.Append( $"{ClassProvider.BackgroundColor( selectedNodeStyling.Background )} {ClassProvider.TextColor( selectedNodeStyling.TextColor )} {selectedNodeStyling.Class}" );
            else
                builder.Append( $"{ClassProvider.BackgroundColor( nodeStyling.Background )} {ClassProvider.TextColor( nodeStyling.TextColor )} {nodeStyling.Class}" );

            base.BuildClasses( builder );
        }

        protected Task OnClick()
        {
            //DirtyClasses();
            Parent?.SelectNode( Node );

            return Task.CompletedTask;
        }

        protected override Task OnParametersSetAsync()
        {
            if ( Selected )
                SelectedNodeStyling?.Invoke( Node, selectedNodeStyling );
            else
                NodeStyling?.Invoke( Node, nodeStyling );

            return base.OnParametersSetAsync();
        }

        #endregion

        #region Properties

        protected bool Selected
            => TreeViewState.SelectedNode != null && TreeViewState.SelectedNode.Equals( Node );

        [Parameter] public TNode Node { get; set; }

        [CascadingParameter] public TreeView<TNode> Parent { get; set; }

        [CascadingParameter]
        protected TreeViewState<TNode> TreeViewState
        {
            get => treeViewState;
            set
            {
                if ( treeViewState == value )
                    return;

                treeViewState = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets selected node styling.
        /// </summary>
        [Parameter] public Action<TNode, NodeStyling> SelectedNodeStyling { get; set; }

        /// <summary>
        /// Gets or sets node styling.
        /// </summary>
        [Parameter] public Action<TNode, NodeStyling> NodeStyling { get; set; }

        #endregion
    }
}