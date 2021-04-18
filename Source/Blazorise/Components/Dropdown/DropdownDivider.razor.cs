#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Divider that can be placed between <see cref="DropdownItem"/>'s.
    /// </summary>
    public partial class DropdownDivider : BaseComponent
    {
        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.DropdownDivider() );

            base.BuildClasses( builder );
        }

        #endregion
    }
}
