using Blazorise.RichTextEdit.Providers;
using Microsoft.AspNetCore.Components;

namespace Blazorise.RichTextEdit
{
    public partial class RichTextEditToolbarButton : BaseComponent
    {
        #region Members        
        /// <summary>The toolbar action</summary>
        private RichTextEditAction? action;
        #endregion

        #region Members                
        /// <summary>Gets or sets the child content.</summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>Gets or sets the value corresponding to the action.</summary>
        [Parameter] public string Value { get; set; }

        /// <summary>Gets or sets the toolbar action.</summary>
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
