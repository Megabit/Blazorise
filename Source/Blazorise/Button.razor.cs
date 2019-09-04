﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseButton : BaseSizableComponent
    {
        #region Members

        private Color color = Color.None;

        private ButtonSize size = ButtonSize.None;

        private bool isOutline;

        private bool isDisabled;

        private bool isActive;

        private bool isBlock;

        private bool isLoading;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Button() )
                .If( () => ClassProvider.ButtonColor( Color ), () => Color != Color.None && !IsOutline )
                .If( () => ClassProvider.ButtonOutline( Color ), () => Color != Color.None && IsOutline )
                .If( () => ClassProvider.ButtonSize( Size ), () => Size != ButtonSize.None )
                .If( () => ClassProvider.ButtonBlock(), () => IsBlock )
                .If( () => ClassProvider.ButtonActive(), () => IsActive )
                .If( () => ClassProvider.ButtonLoading(), () => IsLoading );

            AddonContainerClassMapper
                .If( () => ClassProvider.AddonContainer(), () => IsAddons );

            base.RegisterClasses();
        }

        protected void ClickHandler()
        {
            if ( !IsDisabled )
                Clicked.InvokeAsync( null );
        }

        protected override void OnInitialized()
        {
            // notify dropdown that the button is inside of it
            ParentDropdown?.Register( this );

            base.OnInitialized();
        }

        #endregion

        #region Properties

        protected bool IsAddons => ParentButtons?.Role == ButtonsRole.Addons || ParentDropdown?.IsGroup == true;

        protected ClassMapper AddonContainerClassMapper { get; private set; } = new ClassMapper();

        /// <summary>
        /// Occurs when the button is clicked.
        /// </summary>
        [Parameter] public EventCallback Clicked { get; set; }

        /// <summary>
        /// Defines the button type.
        /// </summary>
        [Parameter] public ButtonType Type { get; set; } = ButtonType.Button;

        /// <summary>
        /// Gets or sets the button color.
        /// </summary>
        [Parameter]
        public Color Color
        {
            get => color;
            set
            {
                color = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Changes the size of a button.
        /// </summary>
        [Parameter]
        public ButtonSize Size
        {
            get => size;
            set
            {
                size = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Makes the button to have the outlines.
        /// </summary>
        [Parameter]
        public bool IsOutline
        {
            get => isOutline;
            set
            {
                isOutline = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Makes button look inactive.
        /// </summary>
        [Parameter]
        public bool IsDisabled
        {
            get => isDisabled;
            set
            {
                isDisabled = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Makes the button to appear as pressed.
        /// </summary>
        [Parameter]
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Makes the button to span the full width of a parent.
        /// </summary>
        [Parameter]
        public bool IsBlock
        {
            get => isBlock;
            set
            {
                isBlock = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Shows the loading spinner.
        /// </summary>
        [Parameter]
        public bool IsLoading
        {
            get => isLoading;
            set
            {
                isLoading = value;

                ClassMapper.Dirty();
            }
        }

        [CascadingParameter] public BaseDropdown ParentDropdown { get; set; }

        [CascadingParameter] public BaseButtons ParentButtons { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
