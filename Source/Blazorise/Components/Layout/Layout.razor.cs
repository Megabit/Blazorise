#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Layout : BaseComponent
    {
        #region Members

        private bool sider;

        private bool loading;

        private string loadingClass;

        #endregion

        #region Constructors

        public Layout()
        {
            LoadingClassBuilder = new ClassBuilder( BuildLoadingClasses );
        }

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Layout() );
            builder.Append( ClassProvider.LayoutHasSider(), Sider );
            builder.Append( ClassProvider.LayoutRoot(), ParentLayout == null );

            base.BuildClasses( builder );
        }

        protected void BuildLoadingClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.LayoutLoading(), string.IsNullOrEmpty( LoadingClass ) );
            builder.Append( LoadingClass );
        }

        internal protected override void DirtyClasses()
        {
            LoadingClassBuilder.Dirty();

            base.DirtyClasses();
        }

        #endregion

        #region Properties

        protected string LoadingClassNames => LoadingClassBuilder.Class;

        protected ClassBuilder LoadingClassBuilder { get; private set; }

        /// <summary>
        /// Indicates that layout will contain sider container.
        /// </summary>
        [Parameter]
        public bool Sider
        {
            get => sider;
            set
            {
                sider = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// If true, an overlay will be created so the user cannot click anything until set to false.
        /// </summary>
        [Parameter]
        public bool Loading
        {
            get => loading;
            set
            {
                loading = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Sets the custom classname for loading element.
        /// </summary>
        [Parameter]
        public string LoadingClass
        {
            get => loadingClass;
            set
            {
                loadingClass = value;

                DirtyClasses();
            }
        }

        [Parameter] public EventCallback<bool> LoadingChanged { get; set; }

        [CascadingParameter] protected Layout ParentLayout { get; set; }

        [Parameter] public RenderFragment LoadingTemplate { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
