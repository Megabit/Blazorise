#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// An ordered list created using the &lt;ul&gt; element.
    /// </summary>
    public partial class OrderedList : BaseElementComponent
    {
        #region Members

        private bool unstyled;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.OrderedListUnstyled( Unstyled ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Remove the default <c>list-style</c> and left margin on list items (immediate children only).
        /// </summary>
        [Parameter]
        public bool Unstyled
        {
            get => unstyled;
            set
            {
                unstyled = value;

                DirtyClasses();
            }
        }

        #endregion
    }
}
