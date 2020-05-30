using Microsoft.AspNetCore.Components;

namespace Blazorise.RichTextEdit
{
    public partial class RichTextEditToolbarGroup : BaseComponent
    {
        #region Properties        
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
            builder.Append( "ql-formats" );

            base.BuildClasses( builder );
        }
        #endregion
    }
}
