#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Field : BaseComponent
    {
        #region Members

        private bool horizontal;

        private IFluentColumn columnSize;

        private JustifyContent justifyContent = JustifyContent.None;

        private List<BaseComponent> hookables;

        private Validation previousParentValidation;

        private readonly EventHandler<ValidationStatusChangedEventArgs> validationStatusChangedHandler;

        private ValidationStatus previousValidationStatus;

        #endregion

        #region Constructors

        public Field()
        {
            validationStatusChangedHandler += ( sender, eventArgs ) =>
            {
                OnValidationStatusChanged( sender, eventArgs );
            };
        }

        #endregion

        #region Methods

        protected override void OnParametersSet()
        {
            if ( ParentValidation != previousParentValidation )
            {
                DetachValidationStatusChangedListener();
                ParentValidation.ValidationStatusChanged += validationStatusChangedHandler;
                previousParentValidation = ParentValidation;
            }
        }

        protected override void OnInitialized()
        {
            previousValidationStatus = ParentValidation?.Status ?? ValidationStatus.None;

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                DetachValidationStatusChangedListener();
            }

            base.Dispose( disposing );
        }

        private void DetachValidationStatusChangedListener()
        {
            if ( previousParentValidation != null )
            {
                previousParentValidation.ValidationStatusChanged -= validationStatusChangedHandler;
            }
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Field() );
            builder.Append( ClassProvider.FieldHorizontal(), Horizontal );
            builder.Append( ClassProvider.FieldJustifyContent( JustifyContent ), JustifyContent != JustifyContent.None );
            builder.Append( ClassProvider.FieldValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation != null );

            base.BuildClasses( builder );
        }

        protected void OnValidationStatusChanged( object sender, ValidationStatusChangedEventArgs eventArgs )
        {
            if ( previousValidationStatus != eventArgs.Status )
            {
                previousValidationStatus = eventArgs.Status;

                DirtyClasses();

                InvokeAsync( StateHasChanged );
            }
        }

        internal void Hook( BaseComponent component )
        {
            if ( hookables == null )
                hookables = new List<BaseComponent>();

            hookables.Add( component );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines if the field is inside of <see cref="Fields"/> component.
        /// </summary>
        protected bool IsFields => ParentFields != null;

        /// <summary>
        /// Aligns the controls for horizontal form.
        /// </summary>
        [Parameter]
        public bool Horizontal
        {
            get => horizontal;
            set
            {
                horizontal = value;

                hookables?.ForEach( x => x.DirtyClasses() );

                DirtyClasses();
            }
        }

        /// <summary>
        /// Determines how much space will be used by the field inside of the grid row.
        /// </summary>
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

        /// <summary>
        /// Aligns the flexible container's items when the items do not use all available space on the main-axis (horizontally).
        /// </summary>
        [Parameter]
        public JustifyContent JustifyContent
        {
            get => justifyContent;
            set
            {
                justifyContent = value;

                DirtyClasses();
            }
        }

        [CascadingParameter] protected Fields ParentFields { get; set; }

        [CascadingParameter] protected Validation ParentValidation { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
