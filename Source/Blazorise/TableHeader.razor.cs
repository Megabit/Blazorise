#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseTableHeader : BaseComponent
    {
        #region Members

        private ThemeContrast themeContrast;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.TableHeader() )
                .If( () => ClassProvider.TableHeaderThemeContrast( ThemeContrast ), () => ThemeContrast != ThemeContrast.None );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sets the background color to the header.
        /// </summary>
        [Parameter]
        protected ThemeContrast ThemeContrast
        {
            get => themeContrast;
            set
            {
                themeContrast = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
