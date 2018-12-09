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
    public abstract class BaseHeading : BaseComponent
    {
        #region Members

        private HeadingSize headingSize = HeadingSize.Is3;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Heading( headingSize ) );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        [Parameter]
        private HeadingSize Size
        {
            get => headingSize;
            set
            {
                headingSize = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
