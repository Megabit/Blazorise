﻿#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.AntDesign
{
    public partial class FieldBody : Blazorise.FieldBody
    {
        #region Members

        private ClassBuilder containerClassBuilder;

        #endregion

        #region Constructors

        public FieldBody()
        {
            containerClassBuilder = new ClassBuilder( BuildContainerClasses );
        }

        #endregion

        #region Methods

        protected internal override void DirtyClasses()
        {
            containerClassBuilder.Dirty();

            base.DirtyClasses();
        }

        private void BuildContainerClasses( ClassBuilder builder )
        {
            builder.Append( "ant-col" );
            builder.Append( "ant-form-item-control" );

            if ( ColumnSize != null )
            {
                builder.Append( ColumnSize.Class( ClassProvider ) );
            }
        }

        #endregion

        #region Properties

        protected override bool ColumnSizeSupported => false; // disable column-size on the body, as we're going to add it on container

        protected string ContainerClassNames => containerClassBuilder.Class;

        #endregion
    }
}
