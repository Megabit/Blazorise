#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Container for input components when <see cref="Field"/> has <see cref="Field.Horizontal"/> set to true.
    /// </summary>
    public partial class FieldBody : BaseSizableFieldComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.FieldBody() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
