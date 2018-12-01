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
    public abstract class BaseCard : BaseContainerComponent
    {
        #region Members

        private bool isWhiteText;

        private Background background = Background.None;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Card() )
                .If( () => ClassProvider.CardWhiteText(), () => IsWhiteText )
                .If( () => ClassProvider.CardBackground( Background ), () => Background != Background.None );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        [Parameter]
        protected bool IsWhiteText
        {
            get => isWhiteText;
            set
            {
                isWhiteText = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        protected Background Background
        {
            get => background;
            set
            {
                background = value;

                ClassMapper.Dirty();
            }
        }

        #endregion
    }
}
