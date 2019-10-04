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
    /// Base class for field and input components that can be sized in grid layout.
    /// </summary>
    /// <remarks>
    /// TODO: Currently this class is inherited by the input components. This is problematic because the sizing of
    /// input components is done by the FieldBody. See if there is a need for this class to be used by the input components!!
    /// </remarks>
    public abstract class BaseSizableComponent : BaseComponent
    {
        #region Members

        private IFluentColumn columnSize;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( ColumnSize != null && UseColumnSizes )
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

        protected virtual bool ParentIsHorizontal => ParentField?.IsHorizontal == true;

        protected virtual bool ParentIsField => ParentField != null;

        protected virtual bool ParentIsFieldBody => ParentFieldBody != null;

        /// <summary>
        /// Used to override the use of column sizes by some of the providers.
        /// </summary>
        protected virtual bool UseColumnSizes => true;

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

        [CascadingParameter] public BaseField ParentField { get; set; }

        [CascadingParameter] public BaseFieldBody ParentFieldBody { get; set; }

        [CascadingParameter] public Tooltip ParentTooltip { get; set; }

        #endregion
    }
}
