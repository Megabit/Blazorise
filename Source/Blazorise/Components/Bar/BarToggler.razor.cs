#region Using directives
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Controls the visibility state of the <see cref="Blazorise.Bar"/> component.
    /// </summary>
    public partial class BarToggler : BaseComponent
    {
        #region Members

        private BarState parentBarState;

        private BarTogglerMode mode = BarTogglerMode.Normal;

        private Bar bar;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarToggler( ParentBarState?.Mode ?? BarMode.Horizontal, Mode ) );
            builder.Append( ClassProvider.BarTogglerCollapsed( ParentBarState?.Mode ?? BarMode.Horizontal, Mode, Bar != null ? Bar.Visible : ParentBarState.Visible ) );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected override void BuildStyles( StyleBuilder builder )
        {
            if ( Bar != null )
            {
                builder.Append( "display: inline-flex" );
            }

            base.BuildStyles( builder );
        }

        /// <summary>
        /// Handles the toggler onclick event.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected async Task ClickHandler()
        {
            if ( Clicked.HasDelegate )
            {
                await Clicked.InvokeAsync();
            }

            if ( Bar != null )
            {
                await Bar.Toggle();

                DirtyClasses();
            }
            else if ( ParentBar != null )
            {
                await ParentBar.Toggle();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Occurs when the button is clicked.
        /// </summary>
        [Parameter] public EventCallback Clicked { get; set; }

        /// <summary>
        /// Provides options for inline or popout styles. Only supported by Vertical Bar. Uses inline by default.
        /// </summary>
        [Parameter]
        public BarTogglerMode Mode
        {
            get => mode;
            set
            {
                if ( mode == value )
                    return;

                mode = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Controls which <see cref="Bar"/> will be toggled. Uses parent <see cref="Bar"/> by default. 
        /// </summary>
        [Parameter]
        public Bar Bar
        {
            get => bar;
            set
            {
                if ( bar == value )
                    return;

                bar = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="BarToggler"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Cascaded <see cref="Bar"/> component state object.
        /// </summary>
        [CascadingParameter]
        protected BarState ParentBarState
        {
            get => parentBarState;
            set
            {
                if ( parentBarState == value )
                    return;

                parentBarState = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Cascaded <see cref="Bar"/> component.
        /// </summary>
        [CascadingParameter] protected Bar ParentBar { get; set; }

        #endregion
    }
}
