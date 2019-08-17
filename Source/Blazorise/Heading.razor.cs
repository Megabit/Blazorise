#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseHeading : BaseComponent
    {
        #region Members

        private HeadingSize headingSize = HeadingSize.Is3;

        private TextColor textColor = TextColor.None;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Heading( headingSize ) )
                .If( () => ClassProvider.HeadingTextColor( TextColor ), () => TextColor != TextColor.None );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        [Parameter]
        public HeadingSize Size
        {
            get => headingSize;
            set
            {
                headingSize = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        public TextColor TextColor
        {
            get => textColor;
            set
            {
                textColor = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
