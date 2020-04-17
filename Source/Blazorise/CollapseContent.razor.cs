#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class CollapseContent : BaseComponent
    {
        #region Members

        private bool visible;

        #endregion

        #region Constructors

        public CollapseContent()
        {
            BodyClassBuilder = new ClassBuilder( BuildBodyClasses );
        }

        #endregion

        #region Methods

        protected internal override void DirtyClasses()
        {
            BodyClassBuilder.Dirty();

            base.DirtyClasses();
        }

        private void BuildBodyClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CollapseContentBody() );
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CollapseContent() );
            builder.Append( ClassProvider.CollapseContentActive( Visible ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        protected ClassBuilder BodyClassBuilder { get; private set; }

        /// <summary>
        /// Gets dialog container class-names.
        /// </summary>
        protected string BodyClassNames => BodyClassBuilder.Class;

        /// <summary>
        /// Gets or sets the content visibility.
        /// </summary>
        [CascadingParameter( Name = "Collapse" )]
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
