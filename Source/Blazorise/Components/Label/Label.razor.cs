#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Label : BaseComponent
    {
        #region Members

        private LabelType type = LabelType.None;

        private Cursor cursor;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Label(), Type == LabelType.None );
            builder.Append( ClassProvider.LabelType( Type ), Type != LabelType.None );
            builder.Append( ClassProvider.LabelCursor( Cursor ), Cursor != Cursor.Default );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter] public string For { get; set; }

        [Parameter]
        public LabelType Type
        {
            get => type;
            set
            {
                type = value;

                DirtyClasses();
            }
        }

        [Parameter]
        public Cursor Cursor
        {
            get => cursor;
            set
            {
                cursor = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
