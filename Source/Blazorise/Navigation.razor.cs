#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
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

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Nav() );
            builder.Append( ClassProvider.NavTabs(), IsTabs );
            builder.Append( ClassProvider.NavCards(), IsCards );
            builder.Append( ClassProvider.NavPills(), IsPills );
            builder.Append( ClassProvider.NavVertical(), IsVertical );
            builder.Append( ClassProvider.NavFill( Fill ), Fill != NavFillType.None );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter]
        public bool IsTabs
        {
            get => isTabs;
            set
            {
                isTabs = value;

                DirtyClasses();
            }
        }

        [Parameter]
        public bool IsCards
        {
            get => isCards;
            set
            {
                isCards = value;

                DirtyClasses();
            }
        }

        [Parameter]
        public bool IsPills
        {
            get => isPills;
            set
            {
                isPills = value;

                DirtyClasses();
            }
        }

        [Parameter]
        public bool IsVertical
        {
            get => isVertical;
            set
            {
                isVertical = value;

                DirtyClasses();
            }
        }

        [Parameter]
        public NavFillType Fill
        {
            get => fill;
            set
            {
                fill = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
