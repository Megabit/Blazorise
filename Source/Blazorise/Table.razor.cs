#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseTable : BaseComponent
    {
        #region Members

        private bool isFullWidth = true;

        private bool isStriped;

        private bool isBordered;

        private bool isHoverable;

        private bool isNarrow;

        private bool isBorderless;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Table() );
            builder.Append( ClassProvider.TableFullWidth(), IsFullWidth );
            builder.Append( ClassProvider.TableStriped(), IsStriped );
            builder.Append( ClassProvider.TableBordered(), IsBordered );
            builder.Append( ClassProvider.TableHoverable(), IsHoverable );
            builder.Append( ClassProvider.TableNarrow(), IsNarrow );
            builder.Append( ClassProvider.TableBorderless(), IsBorderless );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Makes the table to fill entire horizontal space.
        /// </summary>
        [Parameter]
        public bool IsFullWidth
        {
            get => isFullWidth;
            set
            {
                isFullWidth = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Adds stripes to the table.
        /// </summary>
        [Parameter]
        public bool IsStriped
        {
            get => isStriped;
            set
            {
                isStriped = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Adds borders to all the cells.
        /// </summary>
        [Parameter]
        public bool IsBordered
        {
            get => isBordered;
            set
            {
                isBordered = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Adds a hover effect when mousing over rows.
        /// </summary>
        [Parameter]
        public bool IsHoverable
        {
            get => isHoverable;
            set
            {
                isHoverable = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes the table more compact by cutting cell padding in half.
        /// </summary>
        [Parameter]
        public bool IsNarrow
        {
            get => isNarrow;
            set
            {
                isNarrow = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes the table without any borders.
        /// </summary>
        [Parameter]
        public bool IsBorderless
        {
            get => isBorderless;
            set
            {
                isBorderless = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
