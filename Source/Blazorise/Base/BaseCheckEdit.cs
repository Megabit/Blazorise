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
    public abstract class BaseCheckEdit : BaseInputComponent
    {
        #region Members

        private string radioGroup;

        private bool isInline;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .If( () => ClassProvider.Radio(), () => RadioGroup != null )
                .If( () => ClassProvider.Check(), () => RadioGroup == null );

            base.RegisterClasses();
        }

        protected void CheckedChangedHandled( UIChangeEventArgs e )
        {
            Checked = e.Value?.ToString().ToLowerInvariant() == ( RadioGroup != null ? "on" : "true" );
            CheckedChanged?.Invoke( Checked );
        }

        #endregion

        #region Properties

        protected override bool NeedSizableBlock => ParentIsHorizontal && ParentAddons == null;

        protected ControlRole Role => RadioGroup != null ? ControlRole.Radio : ControlRole.Check;

        protected string Type => RadioGroup != null ? "radio" : "checkbox";

        /// <summary>
        /// Gets or sets the checked flag.
        /// </summary>
        [Parameter] protected bool Checked { get; set; }

        /// <summary>
        /// Occurs when the check state is changed.
        /// </summary>
        [Parameter] protected Action<bool> CheckedChanged { get; set; }

        /// <summary>
        /// Sets the field help-text postioned bellow the field.
        /// </summary>
        [Parameter]
        protected string RadioGroup
        {
            get => radioGroup;
            set
            {
                radioGroup = value;

                ClassMapper.Dirty();
            }
        }

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

        [CascadingParameter] protected BaseAddons ParentAddons { get; set; }

        #endregion
    }
}
