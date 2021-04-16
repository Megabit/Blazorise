﻿#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// The container is a simple element that allows you to place content within a given device or viewport.
    /// </summary>
    public partial class Container : BaseComponent
    {
        #region Members

        private bool fluid;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( Fluid )
                builder.Append( ClassProvider.ContainerFluid() );
            else
                builder.Append( ClassProvider.Container() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Makes a full width container, spanning the entire width of the viewport.
        /// </summary>
        [Parameter]
        public bool Fluid
        {
            get => fluid;
            set
            {
                fluid = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Container"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
