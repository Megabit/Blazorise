#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Table Body element encapsulates a set of table rows, indicating that they comprise the body of the table.
    /// </summary>
    public partial class TableBody : BaseDraggableComponent
    {
        #region Members

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TableBody() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the cascaded parent table component.
        /// </summary>
        [CascadingParameter] protected Table ParentTable { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="TableBody"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
