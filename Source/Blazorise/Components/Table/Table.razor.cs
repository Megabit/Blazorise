#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// The <see cref="Table"/> component is used for displaying tabular data.
    /// </summary>
    public partial class Table : BaseDraggableComponent
    {
        #region Members

        private bool fullWidth = true;

        private bool striped;

        private bool bordered;

        private bool hoverable;

        private bool narrow;

        private bool borderless;

        private bool responsive;

        private bool fixedHeader;

        private string fixedHeaderTableHeight = "300px";

        #endregion

        #region Constructors

        /// <summary>
        /// Default <see cref="Table"/> constructor.
        /// </summary>
        public Table()
        {
            ContainerClassBuilder = new ClassBuilder( BuildContainerClasses );
            ContainerStyleBuilder = new StyleBuilder( BuildContainerStyles );
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            await InitializeTableFixedHeader();

            await base.OnAfterRenderAsync( firstRender );
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Table() );
            builder.Append( ClassProvider.TableFullWidth(), FullWidth );
            builder.Append( ClassProvider.TableStriped(), Striped );
            builder.Append( ClassProvider.TableBordered(), Bordered );
            builder.Append( ClassProvider.TableHoverable(), Hoverable );
            builder.Append( ClassProvider.TableNarrow(), Narrow );
            builder.Append( ClassProvider.TableBorderless(), Borderless );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Builds a list of classnames for the responsive container element.
        /// </summary>
        /// <param name="builder">Class builder used to append the classnames.</param>
        protected virtual void BuildContainerClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TableResponsive(), Responsive );
            builder.Append( ClassProvider.TableFixedHeader(), FixedHeader );
        }

        /// <summary>
        /// Builds a list of styles for the responsive container element.
        /// </summary>
        /// <param name="builder">Style builder used to append the classnames.</param>
        protected virtual void BuildContainerStyles( StyleBuilder builder )
        {
            if ( FixedHeader && !string.IsNullOrEmpty( FixedHeaderTableHeight ) )
            {
                builder.Append( $"height: {FixedHeaderTableHeight};" );
            }
        }

        /// <inheritdoc/>
        protected override void DirtyStyles()
        {
            ContainerStyleBuilder.Dirty();

            base.DirtyStyles();
        }

        /// <summary>
        /// Makes sure that the table header is properly sized.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual ValueTask InitializeTableFixedHeader()
        {
            if ( FixedHeader )
                return JSRunner.InitializeTableFixedHeader( ElementRef, ElementId );

            return ValueTask.CompletedTask;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Class builder used to build the classnames for responsive element.
        /// </summary>
        protected ClassBuilder ContainerClassBuilder { get; private set; }

        /// <summary>
        /// Gets the classname for a responsive element.
        /// </summary>
        protected string ContainerClassNames => ContainerClassBuilder.Class;

        /// <summary>
        /// Style builder used to build the stylenames for responsive or fixed element.
        /// </summary>
        protected StyleBuilder ContainerStyleBuilder { get; private set; }

        /// <summary>
        /// Gets the styles for a responsive element.
        /// </summary>
        protected string ContainerStyleNames => ContainerStyleBuilder.Styles;

        /// <summary>
        /// True if table needs to be placed inside of container element.
        /// </summary>
        protected bool HasContainer => Responsive || FixedHeader;

        /// <summary>
        /// Makes the table to fill entire horizontal space.
        /// </summary>
        [Parameter]
        public bool FullWidth
        {
            get => fullWidth;
            set
            {
                fullWidth = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Adds stripes to the table.
        /// </summary>
        [Parameter]
        public bool Striped
        {
            get => striped;
            set
            {
                striped = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Adds borders to all the cells.
        /// </summary>
        [Parameter]
        public bool Bordered
        {
            get => bordered;
            set
            {
                bordered = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Adds a hover effect when mousing over rows.
        /// </summary>
        [Parameter]
        public bool Hoverable
        {
            get => hoverable;
            set
            {
                hoverable = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes the table more compact by cutting cell padding in half.
        /// </summary>
        [Parameter]
        public bool Narrow
        {
            get => narrow;
            set
            {
                narrow = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes the table without any borders.
        /// </summary>
        [Parameter]
        public bool Borderless
        {
            get => borderless;
            set
            {
                borderless = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes table responsive by adding the horizontal scroll bar.
        /// </summary>
        [Parameter]
        public bool Responsive
        {
            get => responsive;
            set
            {
                responsive = value;

                DirtyClasses();
            }
        }

        /// <summary>
        ///  Makes table have a fixed header and enabling a scrollbar in the table body.
        /// </summary>
        [Parameter]
        public bool FixedHeader
        {
            get => fixedHeader;
            set
            {
                fixedHeader = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Sets the table height when <see cref="FixedHeader"/> feature is enabled (defaults to 300px).
        /// </summary>
        [Parameter]
        public string FixedHeaderTableHeight
        {
            get => fixedHeaderTableHeight;
            set
            {
                fixedHeaderTableHeight = value;

                DirtyClasses();
                DirtyStyles();
            }
        }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Table"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
