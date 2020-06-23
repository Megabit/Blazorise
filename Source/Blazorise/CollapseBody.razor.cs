#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class CollapseBody : BaseComponent
    {
        #region Members

        private bool visible;

        #endregion

        #region Constructors

        public CollapseBody()
        {
            ContentClassBuilder = new ClassBuilder( BuildBodyClasses );
        }

        #endregion

        #region Methods

        protected internal override void DirtyClasses()
        {
            ContentClassBuilder.Dirty();

            base.DirtyClasses();
        }

        private void BuildBodyClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CollapseBodyContent() );
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CollapseBody() );
            builder.Append( ClassProvider.CollapseBodyActive( Visible ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        protected ClassBuilder ContentClassBuilder { get; private set; }

        /// <summary>
        /// Gets dialog container class-names.
        /// </summary>
        protected string ContentClassNames => ContentClassBuilder.Class;

        /// <summary>
        /// Gets or sets the content visibility.
        /// </summary>
        [CascadingParameter( Name = "CollapseVisible" )]
        public bool Visible
        {
            get => visible;
            set
            {
                visible = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
