#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
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

        [Parameter] protected string For { get; set; }

        [Parameter]
        protected bool IsCheck
        {
            get => isCheck;
            set
            {
                isCheck = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        protected bool IsFile
        {
            get => isFile;
            set
            {
                isFile = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        protected Cursor Cursor
        {
            get => cursor;
            set
            {
                cursor = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
