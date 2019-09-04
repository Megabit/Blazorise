#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseText : BaseComponent
    {
        #region Members

        private TextColor color = TextColor.None;

        private TextAlignment alignment = TextAlignment.Left;

        private TextTransform textTransform = TextTransform.None;

        private TextWeight textWeight = TextWeight.None;

        private bool isItalic = false;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TextColor( Color ), Color != TextColor.None );
            builder.Append( ClassProvider.TextAlignment( Alignment ), Alignment != TextAlignment.None );
            builder.Append( ClassProvider.TextTransform( Transform ), Transform != TextTransform.None );
            builder.Append( ClassProvider.TextWeight( Weight ), Weight != TextWeight.None );
            builder.Append( ClassProvider.TextItalic(), IsItalic );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter]
        public TextColor Color
        {
            get => color;
            set
            {
                color = value;

                Dirty();
            }
        }

        [Parameter]
        public TextAlignment Alignment
        {
            get => alignment;
            set
            {
                alignment = value;

                Dirty();
            }
        }

        [Parameter]
        public TextTransform Transform
        {
            get => textTransform;
            set
            {
                textTransform = value;

                Dirty();
            }
        }

        [Parameter]
        public TextWeight Weight
        {
            get => textWeight;
            set
            {
                textWeight = value;

                Dirty();
            }
        }

        [Parameter]
        public bool IsItalic
        {
            get => isItalic;
            set
            {
                isItalic = value;

                Dirty();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
