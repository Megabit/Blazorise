﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class CardTitle : BaseTextComponent
    {
        #region Members

        private int? size;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CardTitle( InsideHeader ) );
            builder.Append( ClassProvider.CardTitleSize( InsideHeader, Size ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates if the title is placed inside if card header.
        /// </summary>
        protected bool InsideHeader => ParentCardHeader != null;

        /// <summary>
        /// Number from 1 to 6 that defines the title size where the smaller number means larger text.
        /// </summary>
        /// <remarks>
        /// TODO: change to enum
        /// </remarks>
        [Parameter]
        public int? Size
        {
            get => size;
            set
            {
                size = value;

                DirtyClasses();
            }
        }

        [CascadingParameter] public CardHeader ParentCardHeader { get; set; }

        #endregion
    }
}
