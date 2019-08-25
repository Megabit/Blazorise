#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseTooltip : BaseComponent
    {
        #region Members

        private Placement placement = Placement.Top;

        public event TooltipChangedEventHandler Changed;

        #endregion

        #region Methods

        #endregion

        #region Properties

        internal ElementReference ArrowRef { get; set; }

        [Parameter] public string Text { get; set; }

        [Parameter]
        public Placement Placement
        {
            get => placement;
            set
            {
                if ( placement != value )
                {
                    placement = value;

                    Changed?.Invoke();
                }
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
