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
    public abstract class BaseNavigation : BaseComponent
    {
        #region Members

        private bool isTabs;

        private bool isCards;

        private bool isPills;

        private bool isVertical;

        private NavFillType fill;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Nav() )
                .If( () => ClassProvider.NavTabs(), () => IsTabs )
                .If( () => ClassProvider.NavCards(), () => IsCards )
                .If( () => ClassProvider.NavPills(), () => IsPills )
                .If( () => ClassProvider.NavVertical(), () => IsVertical )
                .If( () => ClassProvider.NavFill( Fill ), () => Fill != NavFillType.None );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        [Parameter]
        protected bool IsTabs
        {
            get => isTabs;
            set
            {
                isTabs = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        protected bool IsCards
        {
            get => isCards;
            set
            {
                isCards = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        protected bool IsPills
        {
            get => isPills;
            set
            {
                isPills = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        protected bool IsVertical
        {
            get => isVertical;
            set
            {
                isVertical = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        protected NavFillType Fill
        {
            get => fill;
            set
            {
                fill = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
