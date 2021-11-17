#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A wrapper that represents a row in a flexbox grid.
    /// </summary>
    public partial class Row : BaseComponent
    {
        #region Members

        private IFluentRowColumns rowColumns;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Row() );
            builder.Append( ClassProvider.RowNoGutters(), NoGutters );

            if ( RowColumns != null && RowColumns.HasSizes )
                builder.Append( RowColumns.Class( ClassProvider ) );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected override void BuildStyles( StyleBuilder builder )
        {
            builder.Append( StyleProvider.RowGutter( Gutter ), Gutter != default );

            base.BuildStyles( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the number of columns to show in a row.
        /// </summary>
        [Parameter]
        public IFluentRowColumns RowColumns
        {
            get => rowColumns;
            set
            {
                rowColumns = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Row grid spacing - we recommend setting Horizontal and/or Vertical it to (16 + 8n). (n stands for natural number.)
        /// </summary>
        [Parameter] public (int Horizontal, int Vertical) Gutter { get; set; }

        /// <summary>
        /// Removes the negative margins from row and the horizontal padding from all immediate children columns.
        /// </summary>
        [Parameter] public bool NoGutters { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Row"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
