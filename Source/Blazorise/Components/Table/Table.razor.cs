#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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

        private string fixedHeaderTableHeight = "250px";

        #endregion

        #region Constructors

        /// <summary>
        /// Default <see cref="Table"/> constructor.
        /// </summary>
        public Table()
        {
            TableDivClassBuilder = new ClassBuilder( BuildTableDivClasses );
            TableDivStyleBuilder = new StyleBuilder( BuildTableDivStyles );
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "b-table" );
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
        /// Builds a list of classnames for the responsive or the fixed table container element if applicable.
        /// </summary>
        /// <param name="builder">Class builder used to append the classnames.</param>
        protected virtual void BuildTableDivClasses( ClassBuilder builder )
        {
            builder.Append( "b-table-container" );
            BuildResponsiveClasses( builder );
            BuildFixedHeaderClasses( builder );
        }

        /// <summary>
        /// Builds a list of classnames for the responsive or the fixed table container element if applicable.
        /// </summary>
        /// <param name="builder">Class builder used to append the classnames.</param>
        protected virtual void BuildTableDivStyles( StyleBuilder builder )
        {
            BuildFixedHeaderStyles( builder );
        }

        private void BuildFixedHeaderStyles( StyleBuilder builder )
        {
            builder.Append( $"max-height: {FixedHeaderTableHeight}", FixedHeader );
        }

        /// <summary>
        /// Builds a list of classnames for the responsive container element if applicable.
        /// </summary>
        /// <param name="builder">Class builder used to append the classnames.</param>
        private void BuildResponsiveClasses( ClassBuilder builder )
        {
            if (responsive)
                builder.Append( ClassProvider.TableResponsive() );
        }

        /// <summary>
        /// Builds a list of classnames for the fixed table container element if applicable.
        /// </summary>
        /// <param name="builder">Class builder used to append the classnames.</param>
        private void BuildFixedHeaderClasses( ClassBuilder builder )
        {
            if ( fixedHeader )
                builder.Append( ClassProvider.TableFixedHeader() );
        }

        protected override void OnInitialized()
        {
            if ( ElementId == null )
                ElementId = IdGenerator.Generate;

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            await InitTableFixedHeader();
            await base.OnAfterRenderAsync( firstRender );
        }

        private ValueTask InitTableFixedHeader()
        { 
            if ( FixedHeader )
                return JSRunner.InitializeTableFixedHeader( ElementRef, ElementId );
            return ValueTask.CompletedTask;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Class builder used to build the classnames for responsive or fixed element.
        /// </summary>
        protected ClassBuilder TableDivClassBuilder { get; private set; }

        /// <summary>
        /// Style builder used to build the stylenames for responsive or fixed element.
        /// </summary>
        protected StyleBuilder TableDivStyleBuilder { get; private set; }

        /// <summary>
        /// Gets the classnames for a responsive or fixed element according to table configuration.
        /// </summary>
        protected string TableDivClassNames => TableDivClassBuilder.Class;


        /// <summary>
        /// Gets the stylenames for a responsive or fixed element according to table configuration.
        /// </summary>
        protected string TableDivStyleNames => TableDivStyleBuilder.Styles;

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
        /// Makes table have a fixed header and enabling a scrollbar in the table body.
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
        /// Sets table fixed header feature table max height.
        /// Defaults to 250px.
        /// </summary>
        [Parameter]
        public string FixedHeaderTableHeight
        {
            get => fixedHeaderTableHeight;
            set
            {
                fixedHeaderTableHeight = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Table"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
