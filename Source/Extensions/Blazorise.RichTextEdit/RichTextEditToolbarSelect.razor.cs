#region Using directives
using Blazorise.RichTextEdit.Providers;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.RichTextEdit
{
    public partial class RichTextEditToolbarSelect : BaseComponent
    {
        #region Members

        /// <summary>
        /// The select action.
        /// </summary>
        private RichTextEditAction? action;

        #endregion

        #region Methods

        /// <summary>
        /// Builds the classes.
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( Action.HasValue )
            {
                builder.Append( RichTextEditActionClassProvider.Class( action ) );
            }

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the select action.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the child content.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
