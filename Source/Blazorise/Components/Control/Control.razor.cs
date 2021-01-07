#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Control : BaseComponent
    {
        #region Members

        private bool inline;

        private ControlRole role = ControlRole.None;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ControlCheck(), Role == ControlRole.Check );
            builder.Append( ClassProvider.ControlRadio(), Role == ControlRole.Radio );
            builder.Append( ClassProvider.ControlSwitch(), Role == ControlRole.Switch );
            builder.Append( ClassProvider.ControlFile(), Role == ControlRole.File );
            builder.Append( ClassProvider.ControlText(), Role == ControlRole.Text );
            builder.Append( ClassProvider.CheckInline(), Inline );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines if the check or radio control will be inlined.
        /// </summary>
        [Parameter]
        public bool Inline
        {
            get => inline;
            set
            {
                inline = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Sets the role that affects the behaviour of the control container.
        /// </summary>
        [Parameter]
        public ControlRole Role
        {
            get => role;
            set
            {
                role = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
