#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseSimpleText : BaseComponent
    {
        #region Members

        private TextColor color = TextColor.None;

        private TextAlignment alignment = TextAlignment.Left;

        private TextTransform textTransform = TextTransform.None;

        private TextWeight textWeight = TextWeight.None;

        private bool isItalic = false;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .If( () => ClassProvider.SimpleTextColor( Color ), () => Color != TextColor.None )
                .If( () => ClassProvider.SimpleTextAlignment( Alignment ), () => Alignment != TextAlignment.None )
                .If( () => ClassProvider.SimpleTextTransform( Transform ), () => Transform != TextTransform.None )
                .If( () => ClassProvider.SimpleTextWeight( Weight ), () => Weight != TextWeight.None )
                .If( () => ClassProvider.SimpleTextItalic(), () => IsItalic );

            base.RegisterClasses();
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

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        public TextAlignment Alignment
        {
            get => alignment;
            set
            {
                alignment = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        public TextTransform Transform
        {
            get => textTransform;
            set
            {
                textTransform = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        public TextWeight Weight
        {
            get => textWeight;
            set
            {
                textWeight = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        public bool IsItalic
        {
            get => isItalic;
            set
            {
                isItalic = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
