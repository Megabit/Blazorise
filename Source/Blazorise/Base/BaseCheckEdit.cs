#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseCheckEdit : BaseInputComponent<bool?>
    {
        #region Members

        private string radioGroup;

        private bool isInline;

        private Cursor cursor;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .If( () => ClassProvider.Radio(), () => RadioGroup != null )
                .If( () => ClassProvider.Check(), () => RadioGroup == null )
                .If( () => ClassProvider.CheckCursor( Cursor ), () => Cursor != Cursor.Default )
                .If( () => ClassProvider.CheckValidation( ParentValidation?.Status ?? ValidationStatus.None ), () => ParentValidation?.Status != ValidationStatus.None );

            base.RegisterClasses();
        }

        protected void HandleCheckedChanged( UIChangeEventArgs e )
        {
            InternalValue = e.Value?.ToString().ToLowerInvariant() == ( RadioGroup != null ? "on" : "true" );
            CheckedChanged.InvokeAsync( Checked );
            NullableCheckedChanged.InvokeAsync( NullableChecked );
        }

        #endregion

        #region Properties

        protected ControlRole Role => RadioGroup != null ? ControlRole.Radio : ControlRole.Check;

        protected string Type => RadioGroup != null ? "radio" : "checkbox";

        /// <summary>
        /// Gets or sets the checked flag.
        /// </summary>
        [Parameter] protected bool Checked { get => InternalValue ?? false; set => InternalValue = value; }

        /// <summary>
        /// Gets or sets the nullable value for checked flag.
        /// </summary>
        [Parameter] protected bool? NullableChecked { get => InternalValue; set => InternalValue = value; }

        /// <summary>
        /// Occurs when the check state is changed.
        /// </summary>
        [Parameter] protected EventCallback<bool> CheckedChanged { get; set; }

        /// <summary>
        /// Occurs when the check state of nullable value is changed.
        /// </summary>
        [Obsolete( "This parameter is only temporary until the issue with generic componnets is fixed. see http://git.travelsoft.hr/Travelsoft/_git/Adriagate/pullrequest/59?_a=overview" )]
        [Parameter] protected EventCallback<bool?> NullableCheckedChanged { get; set; }

        /// <summary>
        /// Sets the radio group name.
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

        [Parameter]
        protected Cursor Cursor
        {
            get => cursor;
            set
            {
                cursor = value;

                ClassMapper.Dirty();
            }
        }

        #endregion
    }
}
