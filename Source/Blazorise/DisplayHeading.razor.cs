#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseDisplayHeading : BaseComponent
    {
        #region Members

        private DisplayHeadingSize displayHeadingSize = DisplayHeadingSize.Is2;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.DisplayHeadingSize( Size ) );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        [Parameter]
        public DisplayHeadingSize Size
        {
            get => displayHeadingSize;
            set
            {
                displayHeadingSize = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
