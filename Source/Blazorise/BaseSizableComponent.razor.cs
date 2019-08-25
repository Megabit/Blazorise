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
    public abstract class BaseSizableComponent : BaseComponent, IDisposable
    {
        #region Members

        private IFluentColumn columnSize;

        #endregion

        #region Methods

        public virtual void Dispose()
        {
            if ( ParentTooltip != null )
            {
                ExecuteAfterRender( async () =>
                {
                    await JSRunner.DestroyTooltip( ElementId );
                } );
                //ParentTooltip.Changed -= OnTooltipChanged;
            }
        }

        protected override void RegisterClasses()
        {
            ClassMapper
                .If( () => ColumnSize.Class( ClassProvider ), () => ColumnSize != null && UseColumnSizes );
            //.If( () => ClassProvider.Tooltip(), () => ParentTooltip != null )
            //.If( () => ClassProvider.TooltipPlacement( ParentTooltip.Placement ), () => ParentTooltip != null );

            base.RegisterClasses();
        }

        protected override void OnInitialized()
        {
            // link to the parent component
            ParentField?.Hook( this );

            if ( ParentTooltip != null )
            {
                ExecuteAfterRender( async () =>
                {
                    await JSRunner.InitializeTooltip( ElementId, ElementRef, ParentTooltip.ElementRef, ParentTooltip.ArrowRef, ClassProvider.ToPlacement( ParentTooltip.Placement ) );
                } );

                //ParentTooltip.Changed += OnTooltipChanged;
            }

            base.OnInitialized();
        }

        //private void OnTooltipChanged()
        //{
        //    ClassMapper.Dirty();
        //}

        #endregion

        #region Properties

        protected virtual bool ParentIsHorizontal => ParentField?.IsHorizontal == true;

        protected virtual bool ParentIsField => ParentField != null;

        protected virtual bool ParentIsFieldBody => ParentFieldBody != null;

        protected string DataTooltip => ParentTooltip?.Text;

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

                ClassMapper.Dirty();
            }
        }

        [CascadingParameter] protected BaseField ParentField { get; set; }

        [CascadingParameter] protected BaseFieldBody ParentFieldBody { get; set; }

        [CascadingParameter] protected Tooltip ParentTooltip { get; set; }

        #endregion
    }
}
