#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Addon : BaseComponent
    {
        #region Members

        private AddonType addonType = AddonType.Body;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Addon( AddonType ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the location and behaviour of addon container.
        /// </summary>
        [Parameter]
        public AddonType AddonType
        {
            get => addonType;
            set
            {
                addonType = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
