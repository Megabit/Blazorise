#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
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

        /// <summary>
        /// Sets the white text when using the darker background.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the bar background color.
        /// </summary>
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
