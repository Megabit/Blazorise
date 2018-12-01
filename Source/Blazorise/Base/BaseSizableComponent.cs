#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise.Base
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
                .If( () => ColumnSize.Class( ClassProvider ), () => ColumnSize != null && !NeedSizableBlock );

            SizableClassMapper
                .If( () => ColumnSize.Class( ClassProvider ), () => ColumnSize != null && NeedSizableBlock );

            base.RegisterClasses();
        }

        protected internal override void Dirty()
        {
            SizableClassMapper.Dirty();

            base.Dirty();
        }

        protected override void OnInit()
        {
            // link to the parent component
            ParentField?.Hook( this );

            base.OnInit();
        }

        #endregion

        #region Properties

        protected ClassMapper SizableClassMapper { get; } = new ClassMapper();

        protected virtual bool ParentIsHorizontal => ParentField?.IsHorizontal == true;

        protected virtual bool NeedSizableBlock => ParentIsHorizontal;

        [Parameter]
        protected IFluentColumn ColumnSize
        {
            get => columnSize;
            set
            {
                columnSize = value;

                Dirty();
                ClassMapper.Dirty();
            }
        }

        [CascadingParameter] protected BaseField ParentField { get; set; }

        #endregion
    }
}
