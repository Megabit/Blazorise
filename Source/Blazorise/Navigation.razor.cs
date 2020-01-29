#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Navigation : BaseComponent
    {
        #region Members

        private bool tabs;

        private bool cards;

        private bool pills;

        private bool vertical;

        private NavFillType fill;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Nav() );
            builder.Append( ClassProvider.NavTabs(), Tabs );
            builder.Append( ClassProvider.NavCards(), Cards );
            builder.Append( ClassProvider.NavPills(), Pills );
            builder.Append( ClassProvider.NavVertical(), Vertical );
            builder.Append( ClassProvider.NavFill( Fill ), Fill != NavFillType.None );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter]
        public bool Tabs
        {
            get => tabs;
            set
            {
                tabs = value;

                DirtyClasses();
            }
        }

        [Parameter]
        public bool Cards
        {
            get => cards;
            set
            {
                cards = value;

                DirtyClasses();
            }
        }

        [Parameter]
        public bool Pills
        {
            get => pills;
            set
            {
                pills = value;

                DirtyClasses();
            }
        }

        [Parameter]
        public bool Vertical
        {
            get => vertical;
            set
            {
                vertical = value;

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
