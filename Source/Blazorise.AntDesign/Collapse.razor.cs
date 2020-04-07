#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.AntDesign
{
    public partial class Collapse : Blazorise.Collapse
    {
        #region Members

        #endregion

        #region Constructors

        public Collapse()
        {
            ContentClassBuilder = new ClassBuilder( BuildContentClasses );
        }

        #endregion

        #region Methods

        protected override void DirtyClasses()
        {
            ContentClassBuilder.Dirty();

            base.DirtyClasses();
        }

        private void BuildContentClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CollapseContent() );
            builder.Append( ClassProvider.CollapseContentActive( Visible ) );
        }

        #endregion

        #region Properties

        protected ClassBuilder ContentClassBuilder { get; private set; }

        /// <summary>
        /// Gets dialog container class-names.
        /// </summary>
        protected string ContentClassNames => ContentClassBuilder.Class;

        #endregion
    }
}
