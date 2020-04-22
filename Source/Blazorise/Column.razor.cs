#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Column : BaseContainerComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Column() );

            base.BuildClasses( builder );
        }

        protected override void BuildStyles( StyleBuilder builder )
        {
            builder.Append( StyleProvider.ColumnGutter( Gutter ), Gutter != default );

            base.BuildStyles( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Column grid spacing, we recommend setting it to (16 + 8n). (n stands for natural number.)
        /// </summary>
        [CascadingParameter] public (int Horizontal, int Vertical) Gutter { get; set; }

        #endregion
    }
}
