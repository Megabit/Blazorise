#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseField : BaseComponent
    {
        #region Members

        private bool isHorizontal;

        private IFluentColumn columnSize;

        private JustifyContent justifyContent = JustifyContent.None;

        private List<BaseComponent> hookables;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Field() )
                .If( () => ClassProvider.FieldHorizontal(), () => IsHorizontal )
                .If( () => ClassProvider.JustifyContent( JustifyContent ), () => JustifyContent != JustifyContent.None );

            base.RegisterClasses();
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
        internal bool IsHorizontal
        {
            get => isHorizontal;
            set
            {
                isHorizontal = value;

                hookables?.ForEach( x => x.Dirty() );

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Determines how much space will be used by the field inside of the grid row.
        /// </summary>
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

        /// <summary>
        /// Aligns the flexible container's items when the items do not use all available space on the main-axis (horizontally).
        /// </summary>
        [Parameter]
        protected JustifyContent JustifyContent
        {
            get => justifyContent;
            set
            {
                justifyContent = value;

                ClassMapper.Dirty();
            }
        }

        [CascadingParameter] protected BaseFields ParentFields { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
