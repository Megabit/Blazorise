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
    public abstract class BaseCardSubtitle : BaseComponent
    {
        #region Members

        private int size = 6;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.CardSubtitle() )
                .Add( () => ClassProvider.CardSubtitleSize( Size ) );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        [Parameter]
        protected int Size
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
