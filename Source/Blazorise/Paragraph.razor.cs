#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseParagraph : BaseComponent
    {
        #region Members

        private TextColor color = TextColor.None;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Paragraph() )
                .If( () => ClassProvider.ParagraphColor( Color ), () => Color != TextColor.None );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        [Parameter]
        public TextColor Color
        {
            get => color;
            set
            {
                color = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
