#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Row : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Row() );

            base.BuildClasses( builder );
        }

        protected override void BuildStyles( StyleBuilder builder )
        {
            builder.Append( StyleProvider.RowGutter( Gutter ), Gutter != default );

            base.BuildStyles( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Row grid spacing - we recommend setting Horizontal and/or Vertical it to (16 + 8n). (n stands for natural number.)
        /// </summary>
        [Parameter] public (int Horizontal, int Vertical) Gutter { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
