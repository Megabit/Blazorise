#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Field() );
            builder.Append( ClassProvider.FieldHorizontal(), Horizontal );
            builder.Append( ClassProvider.ToJustifyContent( JustifyContent ), JustifyContent != JustifyContent.None );

            base.BuildClasses( builder );
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

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
