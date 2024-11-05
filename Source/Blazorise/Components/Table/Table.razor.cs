#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

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

    private bool fixedColumns;

    private string fixedHeaderTableHeight = "300px";

    private string fixedHeaderTableMaxHeight = "300px";

    private bool resizable;

    private TableResponsiveMode responsiveMode;

    #endregion

    #region Constructors

    /// <summary>
    /// Default <see cref="Table"/> constructor.
    /// </summary>
    public Table()
    {
        ContainerClassBuilder = new( BuildContainerClasses );
        ContainerStyleBuilder = new( BuildContainerStyles );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await InitializeTableFixedHeader();

        await RecalculateResize();

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-table" );

        builder.Append( ClassProvider.Table() );
        builder.Append( ClassProvider.TableResponsiveMode( ResponsiveMode ) );
        builder.Append( ClassProvider.TableFullWidth( FullWidth ) );
        builder.Append( ClassProvider.TableStriped( Striped ) );
        builder.Append( ClassProvider.TableBordered( Bordered ) );
        builder.Append( ClassProvider.TableHoverable( Hoverable ) );
        builder.Append( ClassProvider.TableNarrow( Narrow ) );
        builder.Append( ClassProvider.TableBorderless( Borderless ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds a list of classnames for the responsive container element.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    protected virtual void BuildContainerClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TableResponsive( Responsive ) );
        builder.Append( ClassProvider.TableFixedHeader( FixedHeader ) );
        builder.Append( ClassProvider.TableFixedColumns( FixedColumns ) );
    }

    /// <summary>
    /// Builds a list of styles for the responsive container element.
    /// </summary>
    /// <param name="builder">Style builder used to append the classnames.</param>
    protected virtual void BuildContainerStyles( StyleBuilder builder )
    {
        if ( FixedHeader )
        {
            if ( !string.IsNullOrEmpty( FixedHeaderTableHeight ) )
                builder.Append( $"height: {FixedHeaderTableHeight};" );

            if ( !string.IsNullOrEmpty( FixedHeaderTableMaxHeight ) )
                builder.Append( $"max-height: {FixedHeaderTableMaxHeight};" );
        }
    }

    /// <inheritdoc/>
    internal protected override void DirtyClasses()
    {
        ContainerClassBuilder.Dirty();

        base.DirtyClasses();
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
            return JSModule.InitializeFixedHeader( ElementRef, ElementId );

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// If Table is resizable. 
    /// Resizable columns should be constantly recalculated to keep up with the current Table's height dimensions.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async ValueTask RecalculateResize()
    {
        if ( resizable )
        {
            await DestroyResizable();
            await InitializeResizable();
        }
    }

    /// <summary>
    /// If table has <see cref="FixedHeader"/> enabled, it will scroll position to the provided pixels.
    /// </summary>
    /// <param name="pixels">Offset in pixels from the top of the table.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask ScrollToPixels( int pixels )
    {
        if ( FixedHeader )
        {
            return JSModule.ScrollTableToPixels( ElementRef, ElementId, pixels );
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// If table has <see cref="FixedHeader"/> enabled, it will scroll position to the provided row.
    /// </summary>
    /// <param name="row">Zero-based index of table row to scroll to.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask ScrollToRow( int row )
    {
        if ( FixedHeader )
        {
            return JSModule.ScrollTableToRow( ElementRef, ElementId, row );
        }

        return ValueTask.CompletedTask;
    }

    private ValueTask InitializeResizable()
        => JSModule.InitializeResizable( ElementRef, ElementId, ResizeMode );

    private ValueTask DestroyResizable()
        => JSModule.DestroyResizable( ElementRef, ElementId );

    private ValueTask DestroyFixedHeader()
        => JSModule.DestroyFixedHeader( ElementRef, ElementId );

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
    protected bool HasContainer => Responsive || FixedHeader || Resizable || FixedColumns;

    /// <summary>
    /// Gets or sets the <see cref="IJSTableModule"/> instance.
    /// </summary>
    [Inject] public IJSTableModule JSModule { get; set; }

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
    /// <remarks>
    /// In some cases <see cref="Dropdown"/> component placed inside of a table marked with <see cref="Responsive"/>
    /// flag might not show dropdown menu properly. To make it work you might need to add some
    /// <see href="https://stackoverflow.com/questions/49346755/bootstrap-4-drop-down-menu-in-table">additional CSS rules</see>.
    /// </remarks>
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
    /// Makes table have a fixed header and enables a scrollbar in the table body.
    /// </summary>
    [Parameter]
    public bool FixedHeader
    {
        get => fixedHeader;
        set
        {
            if ( fixedHeader == value )
                return;

            fixedHeader = value;

            DirtyClasses();

            if ( !fixedHeader )
                ExecuteAfterRender( () => DestroyFixedHeader().AsTask() );
        }
    }

    /// <summary>
    /// Makes table have a fixed set of columns. This will make it so that the table columns could be fixed to the side of the table.
    /// </summary>
    [Parameter]
    public bool FixedColumns
    {
        get => fixedColumns;
        set
        {
            if ( fixedColumns == value )
                return;

            fixedColumns = value;

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
    /// Sets the table max height when <see cref="FixedHeader"/> feature is enabled (defaults to 300px).
    /// </summary>
    [Parameter]
    public string FixedHeaderTableMaxHeight
    {
        get => fixedHeaderTableMaxHeight;
        set
        {
            fixedHeaderTableMaxHeight = value;

            DirtyClasses();
            DirtyStyles();
        }
    }

    /// <summary>
    /// Gets or sets whether users can resize Table's columns.
    /// </summary>
    [Parameter]
    public bool Resizable
    {
        get => resizable;
        set
        {
            if ( resizable == value )
                return;

            resizable = value;

            DirtyClasses();

            if ( !resizable )
                ExecuteAfterRender( () => DestroyResizable().AsTask() );
        }
    }

    /// <summary>
    /// Gets or sets whether the user can resize on header or columns.
    /// </summary>
    [Parameter] public TableResizeMode ResizeMode { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Table"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the Table's responsive mode.
    /// </summary>
    [Parameter]
    public TableResponsiveMode ResponsiveMode
    {
        get => responsiveMode;
        set
        {
            if ( responsiveMode == value )
                return;

            responsiveMode = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets whether the Fixed Columns feature tries to resync the columns constantly.
    /// <para>This feature might have a performance impact</para>
    /// </summary>
    [Parameter] public bool FixedColumnsSync { get; set; }
    #endregion
}