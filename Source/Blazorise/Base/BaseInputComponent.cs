#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseInputComponent : BaseSizableComponent
    {
        #region Members

        private Size size = Size.None;

        private bool isReadonly;

        private bool isDisabled;

        #endregion

        #region Methods

        protected override void OnInit()
        {
            // link to the parent component
            ParentField?.Hook( this );

            base.OnInit();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sets the size of the input control.
        /// </summary>
        [Parameter]
        protected Size Size
        {
            get => size;
            set
            {
                size = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Add the readonly boolean attribute on an input to prevent modification of the input’s value.
        /// </summary>
        [Parameter]
        protected bool IsReadonly
        {
            get => isReadonly;
            set
            {
                isReadonly = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Add the disabled boolean attribute on an input to prevent user interactions and make it appear lighter.
        /// </summary>
        [Parameter]
        protected bool IsDisabled
        {
            get => isDisabled;
            set
            {
                isDisabled = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }

    public abstract class BaseInputComponent<TValue> : BaseInputComponent
    {
        #region Members

        private TValue value;

        #endregion

        #region Methods

        protected override void OnInit()
        {
            ParentValidation?.InitInputValue( value );

            if ( ParentValidation != null )
            {
                ParentValidation.Validated += OnValidated;
            }

            base.OnInit();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentValidation != null )
                {
                    ParentValidation.Validated -= OnValidated;
                }
            }

            base.Dispose( disposing );
        }

        private void OnValidated( ValidatedEventArgs e )
        {
            ClassMapper.Dirty();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Internal input value.
        /// </summary>
        protected TValue Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;

                ParentValidation?.UpdateInputValue( value );
            }
        }

        /// <summary>
        /// Placeholder for validation messages.
        /// </summary>
        [Parameter] protected RenderFragment Feedback { get; set; }

        /// <summary>
        /// Parent validation container.
        /// </summary>
        [CascadingParameter] protected BaseValidation ParentValidation { get; set; }

        #endregion
    }
}
