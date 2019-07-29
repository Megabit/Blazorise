#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    /// <summary>
    /// Sets the field label.
    /// </summary>
    public abstract class BaseFieldLabel : BaseSizableComponent
    {
        #region Members

        private bool isCheck;

        private bool isFile;

        private Screenreader screenreader = Screenreader.Always;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.FieldLabel() )
                .If( () => ClassProvider.FieldLabelHorizontal(), () => ParentIsHorizontal )
                .If( () => ClassProvider.Screenreader( Screenreader ), () => Screenreader != Screenreader.Always );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        [Parameter] protected string For { get; set; }

        /// <summary>
        /// Label is used by the checkbox.
        /// </summary>
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

        /// <summary>
        /// Label is used by the file input.
        /// </summary>
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

        /// <summary>
        /// Defines the visibility for screen readers.
        /// </summary>
        [Parameter]
        protected Screenreader Screenreader
        {
            get => screenreader;
            set
            {
                screenreader = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
