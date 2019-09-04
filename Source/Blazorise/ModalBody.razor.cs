#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseModalBody : BaseComponent
    {
        #region Members

        private int? maxHeight;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ModalBody() );

            base.BuildClasses( builder );
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
        public int? MaxHeight
        {
            get => maxHeight;
            set
            {
                maxHeight = value;

                StyleMapper.Dirty();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
