#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseBadge : BaseComponent
    {
        #region Members

        private bool isPill;

        private Color color = Color.None;

        private string link;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Badge() )
                .If( () => ClassProvider.BadgeColor( Color ), () => Color != Color.None )
                .If( () => ClassProvider.BadgePill(), () => IsPill );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Make the badge more rounded.
        /// </summary>
        [Parameter]
        protected bool IsPill
        {
            get => isPill;
            set
            {
                isPill = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Sets the badge contextual color.
        /// </summary>
        [Parameter]
        protected Color Color
        {
            get => color;
            set
            {
                color = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Create a badge link and provide actionable badges with hover and focus states.
        /// </summary>
        [Parameter]
        protected string Link
        {
            get => link;
            set
            {
                link = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
