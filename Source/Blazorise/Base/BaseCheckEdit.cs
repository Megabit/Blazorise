#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseCheckEdit : BaseInputComponent<bool>
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
                .If( () => ClassProvider.Check(), () => RadioGroup == null )
                .If( () => ClassProvider.CheckValidation( ParentValidation?.Status ?? ValidationStatus.None ), () => ParentValidation?.Status != ValidationStatus.None );

            base.RegisterClasses();
        }

        protected void CheckedChangedHandled( UIChangeEventArgs e )
        {
            Checked = e.Value?.ToString().ToLowerInvariant() == ( RadioGroup != null ? "on" : "true" );
            CheckedChanged?.Invoke( Checked );
        }

        #endregion

        #region Properties

        protected ControlRole Role => RadioGroup != null ? ControlRole.Radio : ControlRole.Check;

        protected string Type => RadioGroup != null ? "radio" : "checkbox";

        /// <summary>
        /// Gets or sets the checked flag.
        /// </summary>
        [Parameter] protected bool Checked { get => InternalValue; set => InternalValue = value; }

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

        #endregion
    }
}
