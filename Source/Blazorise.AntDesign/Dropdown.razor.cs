#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.AntDesign
{
    public partial class Dropdown : Blazorise.Dropdown
    {
        #region Members

        private DomElement elementInfo;

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            ParentAddon?.Register( this );

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            elementInfo = await JSRunner.GetElementInfo( ElementRef, ElementId );

            await base.OnAfterRenderAsync( firstRender );
        }

        #endregion

        #region Properties

        public DomElement ElementInfo => elementInfo;

        [CascadingParameter] public AntDesign.Addon ParentAddon { get; set; }

        #endregion
    }
}
