#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Base class for field and input components that can be sized in grid layout.
    /// </summary>
    public abstract class BaseSizableFieldComponent : BaseComponent
    {
        #region Members

        private IFluentColumn columnSize;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( ColumnSize != null && ColumnSizeSupported )
                builder.Append( ColumnSize.Class( ClassProvider ) );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            // link to the parent component
            ParentField?.Hook( this );

            base.OnInitialized();
        }

        #endregion

        #region Properties

        /// <summary>
        /// True if component is inside of a <see cref="Field"/> marked as horizontal.
        /// </summary>
        protected virtual bool IsHorizontal => ParentField?.Horizontal == true;

        /// <summary>
        /// True if component is inside of a <see cref="Field"/>.
        /// </summary>
        protected virtual bool IsInsideField => ParentField != null;

        /// <summary>
        /// Used to override the use of column sizes by some of the providers.
        /// </summary>
        protected virtual bool ColumnSizeSupported => true;

        /// <summary>
        /// Defines the column size inside of a <see cref="Field"/> component.
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

        [CascadingParameter] protected Field ParentField { get; set; }

        #endregion
    }
}
