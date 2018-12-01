#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseTitle : BaseComponent
    {
        #region Members

        private int size = 1;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Title() )
                .Add( () => ClassProvider.TitleSize( Size ) );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        [Parameter]
        private int Size
        {
            get => size;
            set
            {
                size = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
