#region Using directives
using System;
using Blazorise.Markdown.Providers;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Markdown
{
    /// <summary>
    /// Defines the action button for the <see cref="Markdown"/> toolbar.
    /// </summary>
    public partial class MarkdownToolbarButton : BaseComponent, IDisposable
    {
        #region Members

        /// <summary>
        /// The toolbar action.
        /// </summary>
        private MarkdownAction? action;

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            if ( ParentMarkdown is not null )
            {
                ParentMarkdown.AddMarkdownToolbarButton( this );
            }

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentMarkdown is not null )
                {
                    ParentMarkdown.RemoveMarkdownToolbarButton( this );
                }
            }

            base.Dispose( disposing );
        }

        /// <summary>
        /// Builds the classes.
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( Action.HasValue )
            {
                builder.Append( MarkdownActionProvider.Class( Action, Name ) );
            }

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the predefined toolbar action. If undefined <see cref="Name"/> will be used.
        /// </summary>
        [Parameter]
        public MarkdownAction? Action
        {
            get => action;
            set
            {
                action = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the custom name corresponding to the action.
        /// </summary>
        [Parameter] public string Name { get; set; }

        /// <summary>
        /// Gets or sets the custom value corresponding to the action.
        /// </summary>
        [Parameter] public object Value { get; set; }

        /// <summary>
        /// Gets or sets the custom icon name corresponding to the action.
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the small tooltip that appears via the <c>title=""</c> attribute.
        /// </summary>
        [Parameter] public string Title { get; set; }

        /// <summary>
        /// If true, separator will be placed before the button.
        /// </summary>
        [Parameter] public bool Separator { get; set; }

        /// <summary>
        /// Gets or sets the parent markdown instance.
        /// </summary>
        [CascadingParameter] protected Markdown ParentMarkdown { get; set; }

        #endregion
    }
}
