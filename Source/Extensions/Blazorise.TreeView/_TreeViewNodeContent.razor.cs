#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.TreeView
{
    public partial class _TreeViewNodeContent<TNode> : BaseComponent
    {
        #region Members

        private TreeViewState<TNode> treeViewState;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( $"{ClassProvider.Spacing( Spacing.Padding, SpacingSize.Is1, Side.All, Breakpoint.None )} cursor-pointer" );

            if ( Selected )
                if( string.IsNullOrWhiteSpace( SelectedNodeClass ) )
                    builder.Append( $"{ClassProvider.BackgroundColor( Background.Primary )} {ClassProvider.TextColor( TextColor.White )}" );
                else
                    builder.Append( $"{SelectedNodeClass}" );

            base.BuildClasses( builder );
        }

        protected Task OnClick()
        {
            //DirtyClasses();
            Parent?.SelectNode( Node );

            return Task.CompletedTask;
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
        /// Gets or sets selected node class.
        /// </summary>
        [Parameter] public string SelectedNodeClass { get; set; }

        #endregion
    }
}