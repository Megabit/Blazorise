#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
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
                .If( () => ClassProvider.ToScreenreader( Screenreader ), () => Screenreader != Screenreader.Always );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        [Parameter] public string For { get; set; }

        /// <summary>
        /// Label is used by the checkbox.
        /// </summary>
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

        /// <summary>
        /// Label is used by the file input.
        /// </summary>
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

        /// <summary>
        /// Defines the visibility for screen readers.
        /// </summary>
        [Parameter]
        public Screenreader Screenreader
        {
            get => screenreader;
            set
            {
                screenreader = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
