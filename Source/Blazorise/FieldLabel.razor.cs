#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Sets the field label.
    /// </summary>
    public partial class FieldLabel : BaseSizableComponent
    {
        #region Members

        private Screenreader screenreader = Screenreader.Always;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.FieldLabel() );
            builder.Append( ClassProvider.FieldLabelHorizontal(), ParentIsHorizontal );
            builder.Append( ClassProvider.ToScreenreader( Screenreader ), Screenreader != Screenreader.Always );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter] public string For { get; set; }

        /// <summary>
        /// Defines the visibility for screen readers.
        /// </summary>
        [Parameter]
        public Screenreader Screenreader
        {
            get => screenreader;
            set
            {
                screenreader = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
