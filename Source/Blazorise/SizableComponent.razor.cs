#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseSizableComponent : BaseComponent
    {
        #region Members

        private IFluentColumn columnSize;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .If( () => ColumnSize.Class( ClassProvider ), () => ColumnSize != null && UseColumnSizes );

            base.RegisterClasses();
        }

        protected override void OnInit()
        {
            // link to the parent component
            ParentField?.Hook( this );

            base.OnInit();
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
        protected IFluentColumn ColumnSize
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

        #endregion
    }
}
