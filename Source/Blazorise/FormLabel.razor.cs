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

        protected override void RegisterClasses()
        {
            ClassMapper
                .If( () => ClassProvider.Label(), () => !IsFile && !IsCheck )
                .If( () => ClassProvider.LabelFile(), () => IsFile )
                .If( () => ClassProvider.LabelCheck(), () => IsCheck )
                .If( () => ClassProvider.LabelCursor( Cursor ), () => Cursor != Cursor.Default );

            base.RegisterClasses();
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

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        public bool IsFile
        {
            get => isFile;
            set
            {
                isFile = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        public Cursor Cursor
        {
            get => cursor;
            set
            {
                cursor = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
