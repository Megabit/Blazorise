﻿#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Snackbar
{
    public partial class SnackbarBody : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "snackbar-body" );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="SnackbarBody"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
