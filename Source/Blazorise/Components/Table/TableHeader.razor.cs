#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines a set of rows defining the head of the columns of the table.
    /// </summary>
    public partial class TableHeader : BaseDraggableComponent
    {
        #region Members

        private ThemeContrast themeContrast;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TableHeader() );
            builder.Append( ClassProvider.TableHeaderThemeContrast( ThemeContrast ), ThemeContrast != ThemeContrast.None );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sets the prefered color contrast for the header.
        /// </summary>
        [Parameter]
        public ThemeContrast ThemeContrast
        {
            get => themeContrast;
            set
            {
                themeContrast = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="TableHeader"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
