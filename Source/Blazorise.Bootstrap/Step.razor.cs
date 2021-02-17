﻿#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Bootstrap
{
    public partial class Step : Blazorise.Step
    {
        #region Constructors

        public Step()
            : base()
        {
            ContainerClassBuilder = new ClassBuilder( BuildContainerClasses );
        }

        #endregion

        #region Methods

        protected virtual void BuildContainerClasses( ClassBuilder builder )
        {
            builder.Append( "step-container" );
        }

        protected internal override void DirtyClasses()
        {
            ContainerClassBuilder.Dirty();

            base.DirtyClasses();
        }

        #endregion

        #region Properties

        protected ClassBuilder ContainerClassBuilder { get; private set; }

        protected string ContainerClassNames
            => ContainerClassBuilder.Class;

        #endregion
    }
}
