#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseTable : BaseComponent
    {
        #region Members

        private bool isFullWidth = true;

        private bool isStriped;

        private bool isBordered;

        private bool isHoverable;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Table() )
                .If( () => ClassProvider.TableFullWidth(), () => IsFullWidth )
                .If( () => ClassProvider.TableStriped(), () => IsStriped )
                .If( () => ClassProvider.TableBordered(), () => IsBordered )
                .If( () => ClassProvider.TableHoverable(), () => IsHoverable );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        [Parameter]
        protected bool IsFullWidth
        {
            get => isFullWidth;
            set
            {
                isFullWidth = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Add stripes to the table.
        /// </summary>
        [Parameter]
        protected bool IsStriped
        {
            get => isStriped;
            set
            {
                isStriped = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Add borders to all the cells.
        /// </summary>
        [Parameter]
        protected bool IsBordered
        {
            get => isBordered;
            set
            {
                isBordered = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        protected bool IsHoverable
        {
            get => isHoverable;
            set
            {
                isHoverable = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
