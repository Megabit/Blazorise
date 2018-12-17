#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseButton : BaseSizableComponent
    {
        #region Members

        private Color color = Color.None;

        private Size size = Size.None;

        private bool isOutline;

        private bool isDisabled;

        private bool isActive;

        private bool isBlock;

        #endregion

        #region Methods

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( AddonContainerClassMapper != null )
                {
                    AddonContainerClassMapper.Dispose();
                    AddonContainerClassMapper = null;
                }
            }

            base.Dispose( disposing );
        }

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Button() )
                .If( () => ClassProvider.ButtonColor( Color ), () => Color != Color.None )
                .If( () => ClassProvider.ButtonOutline( Color ), () => IsOutline )
                .If( () => ClassProvider.ButtonSize( Size ), () => Size != Size.None )
                .If( () => ClassProvider.ButtonBlock(), () => IsBlock )
                .If( () => ClassProvider.ButtonActive(), () => IsActive );

            AddonContainerClassMapper
                .If( () => ClassProvider.AddonContainer(), () => IsAddons );

            base.RegisterClasses();
        }

        protected void ClickHandler()
        {
            if ( !IsDisabled )
                Clicked?.Invoke();
        }

        protected override void OnInit()
        {
            // notify dropdown that the button is inside of it
            ParentDropdown?.Register( this );

            base.OnInit();
        }

        #endregion

        #region Properties

        protected bool IsAddons => ParentButtons?.Role == ButtonsRole.Addons || ParentDropdown?.IsGroup == true;

        protected ClassMapper AddonContainerClassMapper { get; private set; } = new ClassMapper();

        /// <summary>
        /// Occurs when the button is clicked.
        /// </summary>
        [Parameter] protected Action Clicked { get; set; }

        /// <summary>
        /// Defines the button type.
        /// </summary>
        [Parameter] protected ButtonType Type { get; set; } = ButtonType.Button;

        /// <summary>
        /// Gets or sets the button color.
        /// </summary>
        [Parameter]
        protected Color Color
        {
            get => color;
            set
            {
                color = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Gets or sets the button size.
        /// </summary>
        [Parameter]
        protected Size Size
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
        protected bool IsOutline
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
        protected bool IsDisabled
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
        protected bool IsActive
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
        protected bool IsBlock
        {
            get => isBlock;
            set
            {
                isBlock = value;

                ClassMapper.Dirty();
            }
        }

        [CascadingParameter] protected BaseDropdown ParentDropdown { get; set; }

        [CascadingParameter] protected BaseButtons ParentButtons { get; set; }

        [CascadingParameter] protected BaseAddons ParentAddons { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
