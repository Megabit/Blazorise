#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseLabel : BaseComponent
    {
        #region Members

        private bool isCheck;

        private bool isFile;

        private Cursor cursor;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Label(), !IsFile && !IsCheck );
            builder.Append( ClassProvider.LabelFile(), IsFile );
            builder.Append( ClassProvider.LabelCheck(), IsCheck );
            builder.Append( ClassProvider.LabelCursor( Cursor ), Cursor != Cursor.Default );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter] public string For { get; set; }

        [Parameter]
        public bool IsCheck
        {
            get => isCheck;
            set
            {
                isCheck = value;

                DirtyClasses();
            }
        }

        [Parameter]
        public bool IsFile
        {
            get => isFile;
            set
            {
                isFile = value;

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
