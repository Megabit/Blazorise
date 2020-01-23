#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Badge : BaseComponent
    {
        #region Members

        private bool isPill;

        private Color color = Color.None;

        private string link;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Badge() );
            builder.Append( ClassProvider.BadgeColor( Color ), Color != Color.None );
            builder.Append( ClassProvider.BadgePill(), IsPill );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Make the badge more rounded.
        /// </summary>
        [Parameter]
        public bool IsPill
        {
            get => isPill;
            set
            {
                isPill = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Sets the badge contextual color.
        /// </summary>
        [Parameter]
        public Color Color
        {
            get => color;
            set
            {
                color = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Create a badge link and provide actionable badges with hover and focus states.
        /// </summary>
        [Parameter]
        public string Link
        {
            get => link;
            set
            {
                link = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
