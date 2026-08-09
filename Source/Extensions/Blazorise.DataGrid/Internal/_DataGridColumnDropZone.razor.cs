#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Localization;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.DataGrid.Internal;

/// <summary>
/// Supports data grid column drop zone behavior in DataGrid components.
/// </summary>
public partial class _DataGridColumnDropZone<TItem> : ComponentBase, IDisposable
{
    #region Methods

    /// <summary>
    /// Creates a data grid column drop zone instance.
    /// </summary>
    public _DataGridColumnDropZone()
    {
        classBuilder = new( BuildClasses );
        styleBuilder = new( BuildStyles );
    }

    private Task OnDrop( DragEventArgs e )
    {
        if ( ParentDataGrid.columnDragStarted is not null )
        {
            return ColumnAdded.InvokeAsync( ParentDataGrid.columnDragStarted );
        }

        return Task.CompletedTask;
    }

    private Task RemoveColumn( DataGridColumn<TItem> column )
    {
        return ColumnRemoved.InvokeAsync( column );
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChanged;

        base.OnInitialized();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        LocalizerService.LocalizationChanged -= OnLocalizationChanged;
    }

    private async void OnLocalizationChanged( object sender, EventArgs e )
    {
        await InvokeAsync( StateHasChanged );
    }
    #endregion

    #region Builders

    private ClassBuilder classBuilder;
    private StyleBuilder styleBuilder;

    private string classValue;
    private string styleValue;

    /// <summary>
    /// Computed CSS classes for the column drop target.
    /// </summary>
    protected string ClassNames
        => classBuilder.Class;

    /// <summary>
    /// Computed CSS styles for the column drop target.
    /// </summary>
    protected string StyleNames
        => styleBuilder.Styles;

    private void DirtyClasses()
    {
        classBuilder?.Dirty();
    }

    private void DirtyStyles()
    {
        styleBuilder?.Dirty();
    }

    private void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-datagrid-drop-zone" );

        if ( !string.IsNullOrWhiteSpace( Class ) )
        {
            builder.Append( Class );
        }
    }

    private void BuildStyles( StyleBuilder builder )
    {
        if ( !string.IsNullOrWhiteSpace( Style ) )
        {
            builder.Append( Style.Trim().TrimEnd( ';' ) );
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the DI registered <see cref="ITextLocalizer"/> for <see cref="DataGrid{TItem}"/> />.
    /// </summary>
    [Inject] protected ITextLocalizer<DataGrid<TItem>> Localizer { get; set; }

    /// <summary>
    /// Refreshes drop-zone text after localization changes.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Connects the drop zone to the grid managing column drag operations.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    /// <summary>
    /// Reports a column removed from the drop zone.
    /// </summary>
    [Parameter] public EventCallback<DataGridColumn<TItem>> ColumnRemoved { get; set; }

    /// <summary>
    /// Reports a column dropped into the zone.
    /// </summary>
    [Parameter] public EventCallback<DataGridColumn<TItem>> ColumnAdded { get; set; }

    /// <summary>
    /// Lists the columns currently held by the drop zone.
    /// </summary>
    [Parameter] public IEnumerable<DataGridColumn<TItem>> Columns { get; set; }

    /// <summary>
    /// Additional CSS class for the drop zone element.
    /// </summary>
    [Parameter]
    public string Class
    {
        get => classValue;
        set
        {
            if ( classValue.IsEqual( value ) )
                return;

            classValue = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Additional styles for the drop zone element.
    /// </summary>
    [Parameter]
    public string Style
    {
        get => styleValue;
        set
        {
            if ( styleValue.IsEqual( value ) )
                return;

            styleValue = value;

            DirtyStyles();
        }
    }

    #endregion
}