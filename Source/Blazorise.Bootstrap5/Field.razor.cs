#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Bootstrap5
{
    public partial class Field : Blazorise.Field
    {
        #region Members

        ///// <summary>
        ///// Bootstrap 5 doesn't have "form-group" any more so all fields are without marging by default.
        ///// With this I am defining the default marging if it is undefined. This is per bootstrap 5 documentation.
        ///// </summary>
        //private static readonly IFluentSpacing DefaultMargin = Blazorise.Margin.Is3.FromBottom;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            //if ( Margin == null )
            //{
            //    builder.Append( DefaultMargin.Class( ClassProvider ) );
            //}

            if ( IsFields && ColumnSize == null )
                builder.Append( ClassProvider.FieldColumn() );

            if ( ColumnSize != null )
                builder.Append( ColumnSize.Class( ClassProvider ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        #endregion
    }
}
