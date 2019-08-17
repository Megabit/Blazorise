#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseFields : BaseComponent
    {
        #region Members

        private string label;

        private string help;

        private IFluentColumn columnSize;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Fields() )
                .If( () => ClassProvider.FieldsColumn(), () => ColumnSize != null )
                .If( () => ColumnSize.Class( ClassProvider ), () => ColumnSize != null );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sets the field label.
        /// </summary>
        [Parameter]
        public string Label
        {
            get => label;
            set
            {
                label = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Sets the field help-text positioned bellow the field.
        /// </summary>
        [Parameter]
        public string Help
        {
            get => help;
            set
            {
                help = value;

                ClassMapper.Dirty();
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

                ClassMapper.Dirty();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
