#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Splitter : BaseComponent
    {
        #region Members

        private int thickness = 5;

        private Orientation orientation = Orientation.Horizontal;

        #endregion

        #region Methods

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                await JSRunner.InitializeSplitter( ElementRef, ElementId, new
                {
                    Orientation = Orientation == Orientation.Vertical ? "Vertical" : "Horizontal",
                    Thickness,
                } );
            }

            await base.OnAfterRenderAsync( firstRender );
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "b-splitter" );
            builder.Append( $"b-splitter-{( Orientation == Orientation.Vertical ? "vertical" : "horizontal" )}" );

            base.BuildClasses( builder );
        }

        protected override void BuildStyles( StyleBuilder builder )
        {
            if ( Orientation == Orientation.Vertical )
            {
                builder.Append( $"width: 100% !important;height:{Thickness}px;" );
            }
            else
            {
                builder.Append( $"width: {Thickness}px;height: 100% !important;" );
            }

            base.BuildStyles( builder );
        }

        #endregion

        #region Properties

        [Parameter]
        public int Thickness
        {
            get => thickness;
            set
            {
                thickness = value;

                DirtyStyles();
            }
        }

        [Parameter]
        public Orientation Orientation
        {
            get => orientation;
            set
            {
                orientation = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Splitter content.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
