#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Table : BaseComponent
    {
        #region Members

        private bool fullWidth = true;

        private bool striped;

        private bool bordered;

        private bool hoverable;

        private bool narrow;

        private bool borderless;

        private bool responsive;

        #endregion

        #region Constructors

        public Table()
        {
            ResponsiveClassBuilder = new ClassBuilder( BuildResponsiveClasses );
        }

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Table() );
            builder.Append( ClassProvider.TableFullWidth(), FullWidth );
            builder.Append( ClassProvider.TableStriped(), Striped );
            builder.Append( ClassProvider.TableBordered(), Bordered );
            builder.Append( ClassProvider.TableHoverable(), Hoverable );
            builder.Append( ClassProvider.TableNarrow(), Narrow );
            builder.Append( ClassProvider.TableBorderless(), Borderless );

            base.BuildClasses( builder );
        }

        private void BuildResponsiveClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TableResponsive() );
        }

        #endregion

        #region Properties

        protected ClassBuilder ResponsiveClassBuilder { get; private set; }

        protected string ResponsiveClassNames => ResponsiveClassBuilder.Class;

        /// <summary>
        /// Makes the table to fill entire horizontal space.
        /// </summary>
        [Parameter]
        public bool FullWidth
        {
            get => fullWidth;
            set
            {
                fullWidth = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Adds stripes to the table.
        /// </summary>
        [Parameter]
        public bool Striped
        {
            get => striped;
            set
            {
                striped = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Adds borders to all the cells.
        /// </summary>
        [Parameter]
        public bool Bordered
        {
            get => bordered;
            set
            {
                bordered = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Adds a hover effect when mousing over rows.
        /// </summary>
        [Parameter]
        public bool Hoverable
        {
            get => hoverable;
            set
            {
                hoverable = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes the table more compact by cutting cell padding in half.
        /// </summary>
        [Parameter]
        public bool Narrow
        {
            get => narrow;
            set
            {
                narrow = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes the table without any borders.
        /// </summary>
        [Parameter]
        public bool Borderless
        {
            get => borderless;
            set
            {
                borderless = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes table responsive by adding the horizontal scroll bar.
        /// </summary>
        [Parameter]
        public bool Responsive
        {
            get => responsive;
            set
            {
                responsive = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
