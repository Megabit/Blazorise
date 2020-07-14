﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Addons : BaseComponent
    {
        #region Members

        private IFluentColumn columnSize;

        private List<Button> registeredButtons;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Addons() );
            builder.Append( ClassProvider.AddonsHasButton( registeredButtons?.Count > 0 ) );

            base.BuildClasses( builder );
        }

        protected override void OnAfterRender( bool firstRender )
        {
            if ( firstRender && registeredButtons?.Count > 0 )
            {
                DirtyClasses();
                StateHasChanged();
            }

            base.OnAfterRender( firstRender );
        }

        internal void Register( Button button )
        {
            if ( button == null )
                return;

            if ( registeredButtons == null )
                registeredButtons = new List<Button>();

            if ( !registeredButtons.Contains( button ) )
            {
                registeredButtons.Add( button );
            }
        }

        internal void UnRegister( Button button )
        {
            if ( button == null )
                return;

            if ( registeredButtons != null && registeredButtons.Contains( button ) )
            {
                registeredButtons.Remove( button );
            }
        }

        #endregion

        #region Properties

        [Parameter]
        public IFluentColumn ColumnSize
        {
            get => columnSize;
            set
            {
                columnSize = value;

                DirtyClasses();
            }
        }

        protected virtual bool ParentIsHorizontal => ParentField?.Horizontal == true;

        [CascadingParameter] protected Field ParentField { get; set; }

        //protected bool IsInFieldBody => ParentFieldBody != null;

        [Parameter] public RenderFragment ChildContent { get; set; }

        //[CascadingParameter] protected BaseFieldBody ParentFieldBody { get; set; }

        #endregion
    }
}
