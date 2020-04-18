#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class TableHeader : BaseComponent
    {
        #region Members

        private ThemeContrast themeContrast;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TableHeader() );
            builder.Append( ClassProvider.TableHeaderThemeContrast( ThemeContrast ), ThemeContrast != ThemeContrast.None );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sets the background color to the header.
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

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
