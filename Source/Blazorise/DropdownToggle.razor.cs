﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    public abstract class BaseDropdownToggle : BaseComponent, ICloseActivator, IDisposable
    {
        #region Members

        private bool isOpen;

        private bool isSplit;

        private bool isRegistered;

        private DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef;

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            // link to the parent component
            Dropdown?.Hook( this );

            base.OnInitialized();
        }

        protected override async Task OnFirstAfterRenderAsync()
        {
            dotNetObjectRef ??= JSRunner.CreateDotNetObjectRef( new CloseActivatorAdapter( this ) );

            await base.OnFirstAfterRenderAsync();
        }

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.DropdownToggle() )
                .If( () => ClassProvider.DropdownToggleColor( Color ), () => Color != Color.None && !IsOutline )
                .If( () => ClassProvider.DropdownToggleOutline( Color ), () => Color != Color.None && IsOutline )
                .If( () => ClassProvider.DropdownToggleSize( Size ), () => Size != ButtonSize.None )
                .If( () => ClassProvider.DropdownToggleSplit(), () => IsSplit );

            base.RegisterClasses();
        }

        public void Dispose()
        {
            // make sure to unregister listener
            if ( isRegistered )
            {
                isRegistered = false;

                JSRunner.UnregisterClosableComponent( this );
                JSRunner.DisposeDotNetObjectRef( dotNetObjectRef );
            }
        }

        protected void ClickHandler()
        {
            Dropdown?.Toggle();
        }

        public bool SafeToClose( string elementId, bool isEscapeKey )
        {
            return isEscapeKey || elementId != ElementId;
        }

        public void Close()
        {
            Dropdown?.Close();
        }

        #endregion

        #region Properties

        protected bool IsGroup => Dropdown?.IsGroup == true;

        /// <summary>
        /// Gets or sets the dropdown color.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.None;

        /// <summary>
        /// Gets or sets the dropdown size.
        /// </summary>
        [Parameter] public ButtonSize Size { get; set; } = ButtonSize.None;

        /// <summary>
        /// Handles the visibility of dropdown toggle.
        /// </summary>
        [Parameter]
        public bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;

                if ( isOpen )
                {
                    isRegistered = true;

                    JSRunner.RegisterClosableComponent( dotNetObjectRef, ElementId );
                }
                else
                {
                    isRegistered = false;

                    JSRunner.UnregisterClosableComponent( this );
                }

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Button outline.
        /// </summary>
        [Parameter] public bool IsOutline { get; set; }

        /// <summary>
        /// Handles the visibility of split button.
        /// </summary>
        [Parameter]
        public bool IsSplit
        {
            get => isSplit;
            set
            {
                isSplit = value;

                ClassMapper.Dirty();
            }
        }

        [CascadingParameter] public BaseDropdown Dropdown { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
