#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BasePagination : BaseComponent
    {
        #region Members

        private Size size = Size.None;

        private Alignment alignment = Alignment.None;

        private Background background = Background.None;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Pagination() )
                .If( () => ClassProvider.PaginationSize( Size ), () => Size != Size.None )
                .If( () => ClassProvider.FlexAlignment( Alignment ), () => Alignment != Alignment.None )
                .If( () => ClassProvider.BackgroundColor( Background ), () => Background != Background.None );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the pagination size.
        /// </summary>
        [Parameter]
        protected Size Size
        {
            get => size;
            set
            {
                size = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Gets or sets the pagination alignment.
        /// </summary>
        [Parameter]
        protected Alignment Alignment
        {
            get => alignment;
            set
            {
                alignment = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Gets or sets the pagination background color.
        /// </summary>
        [Parameter]
        protected Background Background
        {
            get => background;
            set
            {
                background = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
