#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines a cell as header of a group of table cells.
    /// </summary>
    public partial class TableRowHeader : BaseDraggableComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TableRowHeader() );

            base.BuildClasses( builder );
        }

        protected Task ClickHandler( MouseEventArgs e )
        {
            return Clicked.InvokeAsync( EventArgsMapper.ToMouseEventArgs( e ) );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Occurs when the row header is clicked.
        /// </summary>
        [Parameter] public EventCallback<BLMouseEventArgs> Clicked { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="TableRowHeader"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
