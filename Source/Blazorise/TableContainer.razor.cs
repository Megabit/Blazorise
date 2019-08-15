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

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Table() )
                .If( () => ClassProvider.TableFullWidth(), () => IsFullWidth )
                .If( () => ClassProvider.TableStriped(), () => IsStriped )
                .If( () => ClassProvider.TableBordered(), () => IsBordered )
                .If( () => ClassProvider.TableHoverable(), () => IsHoverable )
                .If( () => ClassProvider.TableNarrow(), () => IsNarrow )
                .If( () => ClassProvider.TableBorderless(), () => IsBorderless );

            base.RegisterClasses();
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

                ClassMapper.Dirty();
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

                ClassMapper.Dirty();
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

                ClassMapper.Dirty();
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

                ClassMapper.Dirty();
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

                ClassMapper.Dirty();
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

                ClassMapper.Dirty();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
