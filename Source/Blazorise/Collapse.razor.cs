#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Collapse : BaseComponent
    {
        #region Members

        private bool visible;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Collapse() );
            builder.Append( ClassProvider.CollapseActive( Visible ) );

            base.BuildClasses( builder );
        }

        public void Toggle()
        {
            Visible = !Visible;
            StateHasChanged();
        }

        #endregion

        #region Properties

        [Parameter]
        public bool Visible
        {
            get => visible;
            set
            {
                visible = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
