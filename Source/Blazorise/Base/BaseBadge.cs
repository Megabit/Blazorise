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
