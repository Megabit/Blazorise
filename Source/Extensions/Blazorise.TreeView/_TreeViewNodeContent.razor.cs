#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.TreeView
{
    public partial class _TreeViewNodeContent<TNode> : BaseComponent
    {
        #region Members

        private TreeViewStore<TNode> treeViewStore;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( $"{ClassProvider.Spacing( Spacing.Padding, SpacingSize.Is1, Side.All, Breakpoint.None )} cursor-pointer" );

            if ( Selected )
                builder.Append( $"{ClassProvider.BackgroundColor( Background.Primary )} {ClassProvider.TextColor( TextColor.White )}" );

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
            => TreeViewStore.SelectedNode != null && TreeViewStore.SelectedNode.Equals( Node );

        [Parameter] public TNode Node { get; set; }

        [CascadingParameter] public TreeView<TNode> Parent { get; set; }

        [CascadingParameter]
        protected TreeViewStore<TNode> TreeViewStore
        {
            get => treeViewStore;
            set
            {
                if ( treeViewStore == value )
                    return;

                treeViewStore = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}