#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Base component for all the input component types.
    /// </summary>
    public abstract class BaseInputComponent<TValue> : BaseSizableComponent
    {
        #region Members

        protected TValue internalValue;

        private Size size = Size.None;

        private bool isReadonly;

        private bool isDisabled;

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            // link to the parent component
            if ( ParentValidation != null )
            {
                ParentValidation.InitInputValue( internalValue );

                ParentValidation.Validated += OnValidated;
            }

            base.OnInitialized();
        }

        public override void Dispose()
        {
            if ( ParentValidation != null )
            {
                // To avoid leaking memory, it's important to detach any event handlers in Dispose()
                ParentValidation.Validated -= OnValidated;
            }

            base.Dispose();
        }

        private void OnValidated( ValidatedEventArgs e )
        {
            Dirty();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Internal input value.
        /// </summary>
        protected TValue InternalValue
        {
            get
            {
                return internalValue;
            }
            set
            {
                internalValue = value;

                ParentValidation?.UpdateInputValue( value );
            }
        }

        /// <summary>
        /// Sets the size of the input control.
        /// </summary>
        [Parameter]
        public Size Size
        {
            get => size;
            set
            {
                size = value;

                Dirty();
            }
        }

        /// <summary>
        /// Add the readonly boolean attribute on an input to prevent modification of the input’s value.
        /// </summary>
        [Parameter]
        public bool IsReadonly
        {
            get => isReadonly;
            set
            {
                isReadonly = value;

                Dirty();
            }
        }

        /// <summary>
        /// Add the disabled boolean attribute on an input to prevent user interactions and make it appear lighter.
        /// </summary>
        [Parameter]
        public bool IsDisabled
        {
            get => isDisabled;
            set
            {
                isDisabled = value;

                Dirty();
            }
        }

        /// <summary>
        /// Placeholder for validation messages.
        /// </summary>
        [Parameter] public RenderFragment Feedback { get; set; }

        /// <summary>
        /// Input content.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Parent validation container.
        /// </summary>
        [CascadingParameter] public BaseValidation ParentValidation { get; set; }

        #endregion
    }
}
