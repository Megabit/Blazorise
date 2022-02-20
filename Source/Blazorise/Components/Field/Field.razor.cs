#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Wrapper for form input components like label, text, button, etc.
    /// </summary>
    public partial class Field : BaseComponent, IDisposable
    {
        #region Members

        private bool horizontal;

        private IFluentColumn columnSize;

        private JustifyContent justifyContent = JustifyContent.Default;

        private List<BaseComponent> hookables;

        private Validation previousParentValidation;

        private ValidationStatus previousValidationStatus;

        #endregion

        #region Constructors

        /// <summary>
        /// A default constructor for <see cref="Field"/>.
        /// </summary>
        public Field()
        {
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void OnParametersSet()
        {
            if ( ParentValidation != previousParentValidation )
            {
                DetachValidationStatusChangedListener();
                ParentValidation.ValidationStatusChanged += OnValidationStatusChanged;
                previousParentValidation = ParentValidation;
            }
        }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            previousValidationStatus = ParentValidation?.Status ?? ValidationStatus.None;

            base.OnInitialized();
        }

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                DetachValidationStatusChangedListener();

                if ( ParentValidation is not null )
                {
                    ParentValidation.ValidationStatusChanged -= OnValidationStatusChanged;
                }
            }

            base.Dispose( disposing );
        }

        /// <summary>
        /// Unsubscribe from <see cref="Validation.StatusChanged"/> event.
        /// </summary>
        private void DetachValidationStatusChangedListener()
        {
            if ( previousParentValidation != null )
            {
                previousParentValidation.ValidationStatusChanged -= OnValidationStatusChanged;
            }
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Field() );
            builder.Append( ClassProvider.FieldHorizontal(), Horizontal );
            builder.Append( ClassProvider.FieldJustifyContent( JustifyContent ), JustifyContent != JustifyContent.Default );
            builder.Append( ClassProvider.FieldValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation != null );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Handles the <see cref="Validation.StatusChanged"/> event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="eventArgs">Data about the <see cref="Validation"/> status change event.</param>
        protected void OnValidationStatusChanged( object sender, ValidationStatusChangedEventArgs eventArgs )
        {
            if ( previousValidationStatus != eventArgs.Status )
            {
                previousValidationStatus = eventArgs.Status;

                DirtyClasses();

                InvokeAsync( StateHasChanged );
            }
        }

        /// <summary>
        /// Notifies the field that one of it's child components needs a special treatment.
        /// </summary>
        /// <param name="component">Reference to the child component.</param>
        internal void Hook( BaseComponent component )
        {
            hookables ??= new();

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

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Field"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the reference to the parent <see cref="Fields"/> component.
        /// </summary>
        [CascadingParameter] protected Fields ParentFields { get; set; }

        /// <summary>
        /// Gets or sets the reference to the parent <see cref="Validation"/> component.
        /// </summary>
        [CascadingParameter] protected Validation ParentValidation { get; set; }

        #endregion
    }
}
