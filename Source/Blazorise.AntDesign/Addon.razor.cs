#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.AntDesign
{
    public partial class Addon : Blazorise.Addon
    {
        #region Members

        private bool hasDropdown;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "ant-input-group-addon-dropdown", hasDropdown );

            base.BuildClasses( builder );
        }

        protected override Task OnFirstAfterRenderAsync()
        {
            if ( hasDropdown )
            {
                DirtyClasses();
                StateHasChanged();
            }

            return base.OnFirstAfterRenderAsync();
        }

        internal void Register( AntDesign.Dropdown dropdown )
        {
            if ( dropdown != null )
                hasDropdown = true;
        }

        #endregion

        #region Properties

        #endregion
    }
}
