#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Label for a <see cref="Field"/> component.
    /// </summary>
    public partial class FieldLabel : BaseSizableFieldComponent
    {
        #region Members

        private Screenreader screenreader = Screenreader.Always;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.FieldLabel( IsHorizontal ) );
            builder.Append( ClassProvider.ToScreenreader( Screenreader ), Screenreader != Screenreader.Always );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the ID of an element that this label belongs to.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the reference to the parent <see cref="FieldLabel"/> component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
