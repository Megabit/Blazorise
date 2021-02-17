#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// An icon component.
    /// </summary>
    public partial class Icon : BaseComponent
    {
        #region Members

        private object name;

        private IconStyle iconStyle = IconStyle.Solid;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( IconProvider.Icon( Name, IconStyle ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// An icon provider that is responsible to give the icon a class-name.
        /// </summary>
        [Inject] protected IIconProvider IconProvider { get; set; }

        /// <summary>
        /// Icon name that can be either a string or <see cref="IconName"/>.
        /// </summary>
        [Parameter]
        public object Name
        {
            get => name;
            set
            {
                name = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Suggested icon style.
        /// </summary>
        [Parameter]
        public IconStyle IconStyle
        {
            get => iconStyle;
            set
            {
                iconStyle = value;

                DirtyClasses();
            }
        }

        #endregion
    }
}
