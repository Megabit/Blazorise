#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Divider : BaseComponent
    {
        #region Members       

        private DividerType? type;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Divider() );
            builder.Append( ClassProvider.DividerType( ApplyDividerType ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the divider type to apply, based on current theme settings.
        /// </summary>
        protected DividerType ApplyDividerType
            => DividerType.GetValueOrDefault( Theme?.DividerOptions?.DividerType ?? Blazorise.DividerType.Solid );

        /// <summary>
        /// Defines the type and style of the divider.
        /// </summary>
        [Parameter]
        public DividerType? DividerType
        {
            get => type;
            set
            {
                type = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Defines the text of the divider when it's set as <see cref="DividerType.TextContent"/>.
        /// </summary>
        [Parameter] public string Text { get; set; }

        [CascadingParameter] public Theme Theme { get; set; }

        #endregion
    }
}
