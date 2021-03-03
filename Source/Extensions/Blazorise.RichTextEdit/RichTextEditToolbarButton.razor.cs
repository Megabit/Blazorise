#region Using directives
using Blazorise.RichTextEdit.Providers;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.RichTextEdit
{
    public partial class RichTextEditToolbarButton : BaseComponent
    {
        #region Members

        /// <summary>
        /// The toolbar action.
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
        /// Gets or sets the value corresponding to the action.
        /// </summary>
        [Parameter] public string Value { get; set; }

        /// <summary>
        /// Gets or sets the toolbar action.
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

        #endregion
    }
}
