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
    public abstract class BaseModalBody : BaseComponent
    {
        #region Members

        private int? maxHeight;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.ModalBody() );

            base.RegisterClasses();
        }

        protected override void RegisterStyles()
        {
            StyleMapper
                .If( () => StyleProvider.ModalBodyMaxHeight( MaxHeight ?? 0 ), () => MaxHeight != null );

            base.RegisterStyles();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sets the maximum height of the modal body (in viewport size unit).
        /// </summary>
        [Parameter]
        protected int? MaxHeight
        {
            get => maxHeight;
            set
            {
                maxHeight = value;

                StyleMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
