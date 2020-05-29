using Blazorise.RichTextEdit.Providers;
using Microsoft.AspNetCore.Components;

namespace Blazorise.RichTextEdit
{
    public partial class RichTextEditToolbarSelect : BaseComponent
    {
        #region Members
        /// <summary>The select action.</summary>
        private RichTextEditAction? action;
        #endregion

        #region Properties        
        /// <summary>Gets or sets the select action.</summary>
        [Parameter]
        public RichTextEditAction? Action
        {
            get => action;
            set
            {
                action = value;
                DirtyClasses();
            }
        }

        /// <summary>Gets or sets the child content.</summary>
        [Parameter] public RenderFragment ChildContent { get; set; }
        #endregion

        #region Methods        
        /// <summary>
        /// Builds the classes.
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected override void BuildClasses( ClassBuilder builder )
        {
            base.BuildClasses( builder );

            if ( Action.HasValue )
            {
                builder.Append( RichTextEditActionClassProvider.Class( action ) );
            }
        }
        #endregion
    }
}
