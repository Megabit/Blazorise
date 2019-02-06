#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseControl : BaseComponent
    {
        #region Members

        private bool isInline;

        private ControlRole role = ControlRole.None;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .If( () => ClassProvider.ControlCheck(), () => Role == ControlRole.Check )
                .If( () => ClassProvider.ControlRadio(), () => Role == ControlRole.Radio )
                .If( () => ClassProvider.ControlFile(), () => Role == ControlRole.File )
                .If( () => ClassProvider.ControlText(), () => Role == ControlRole.Text )
                .If( () => ClassProvider.CheckInline(), () => IsInline );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines if the check or radio control will be inlined.
        /// </summary>
        [Parameter]
        protected bool IsInline
        {
            get => isInline;
            set
            {
                isInline = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Sets the role that affects the behaviour of the control container.
        /// </summary>
        [Parameter]
        protected ControlRole Role
        {
            get => role;
            set
            {
                role = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
