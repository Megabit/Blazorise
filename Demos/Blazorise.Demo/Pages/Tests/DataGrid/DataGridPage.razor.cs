#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.DataGrid;
using Blazorise.DataGrid.Utils;
using Blazorise.Shared.Data;
using Blazorise.Shared.Models;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Demo.Pages.Tests.DataGrid;

public partial class DataGridPage
{
    #region Members

    private Employee editModel = new();

    private DataGridEditMode editMode = DataGridEditMode.Form;
    private DataGridSortMode sortMode = DataGridSortMode.Multiple;
    private DataGridSelectionMode selectionMode = DataGridSelectionMode.Single;
    private DataGridCommandMode commandsMode = DataGridCommandMode.Commands;
    private TableResizeMode resizableMode = TableResizeMode.Header;
    private DataGridFilterMode filterMode = DataGridFilterMode.Default;

    private DataGrid<Employee> dataGrid;
    public int currentPage { get; set; } = 1;

    private bool editable = true;
    private bool fixedHeader = false;
    private bool virtualize = false;
    private bool resizable = true;
    private bool sortable = true;
    private bool filterable = true;
    private bool showPager = true;
    private bool showPageSizes = true;
    private bool readDataMode = false;
    private bool showButtonRow = true;

    private Employee selectedEmployee;
    private List<Employee> selectedEmployees;

    private List<Employee> employeeList;
    private int totalEmployees;

    private string selectedCityFilter;

    private Random random = new();

    private List<Employee> dataModels = new();
    private List<Employee> inMemoryDataModels;

    [Inject] private EmployeeData EmployeeData { get; set; }

    #endregion

    #region Methods

    protected async Task OnReadDataModeChanged( bool readDataMode )
    {
        this.readDataMode = readDataMode;
        await InvokeAsync( StateHasChanged );
        await Task.Yield();
        await dataGrid.Reload();
    }

    protected override async Task OnInitializedAsync()
    {
        inMemoryDataModels = await EmployeeData.GetDataAsync();
        dataModels = inMemoryDataModels.Take( 50 ).ToList();
        totalEmployees = dataModels.Count;
        await base.OnInitializedAsync();
    }

    public void OnVirtualizeChanged( bool toVirtualize )
    {
        virtualize = toVirtualize;
        if ( virtualize )
            dataModels = inMemoryDataModels.ToList();
        else
            dataModels = inMemoryDataModels.Take( 50 ).ToList();
    }

    public void CheckEmail( ValidatorEventArgs validationArgs )
    {
        ValidationRule.IsEmail( validationArgs );

        if ( validationArgs.Status == ValidationStatus.Error )
        {
            validationArgs.ErrorText = "Email has to be a valid Email";
        }
    }

    public void CheckFirstName( ValidatorEventArgs validationArgs )
    {
        ValidationRule.IsNotEmpty( validationArgs );

        if ( validationArgs.Status == ValidationStatus.Error )
        {
            validationArgs.ErrorText = "First name has to be provided";
        }
    }

    private void OnEmployeeNewItemDefaultSetter( Employee employee )
    {
        employee.Salary = 100.0M;
        employee.IsActive = true;
    }

    private async Task OnRowInserting( CancellableRowChange<Employee, Dictionary<string, object>> e )
    {
        try
        {
            var employee = e.NewItem;

            employee.Id = dataModels?.Max( x => x.Id ) + 1 ?? 1;

            dataModels.Add( employee );
            await dataGrid.Reload();
        }
        catch ( Exception )
        {
            e.Cancel = true;
        }
    }

    private async Task OnRowUpdating( CancellableRowChange<Employee, Dictionary<string, object>> e )
    {
        try
        {
            var idx = dataModels.FindIndex( x => x == e.OldItem );
            dataModels[idx] = e.NewItem;
            await dataGrid.Reload();
        }
        catch ( Exception )
        {
            e.Cancel = true;
        }
    }

    private async Task OnRowRemoving( CancellableRowChange<Employee> e )
    {
        try
        {
            if ( dataModels.Contains( e.NewItem ) )
            {
                dataModels.Remove( e.NewItem );
                await dataGrid.Reload();
            }
        }
        catch ( Exception )
        {
            e.Cancel = true;
        }
    }

    private void OnRowInserted( SavedRowItem<Employee, Dictionary<string, object>> e )
    {
        //var employee = e.NewItem;

        //employee.Id = dataModels?.Max( x => x.Id ) + 1 ?? 1;

        //dataModels.Add( employee );
    }

    private void OnRowUpdated( SavedRowItem<Employee, Dictionary<string, object>> e )
    {
        //var idx = dataModels.FindIndex( x => x == e.OldItem );
        //dataModels[idx] = e.NewItem;
    }

    private void OnRowRemoved( Employee model )
    {
        //if ( dataModels.Contains( model ) )
        //{
        //    dataModels.Remove( model );
        //}
    }

    private string customFilterValue;

    private Task OnCustomFilterValueChanged( string e )
    {
        customFilterValue = e;
        return dataGrid.Reload();
    }

    private bool OnCustomFilter( Employee model )
    {
        if ( string.IsNullOrEmpty( customFilterValue ) )
            return true;

        return
            model.FirstName?.Contains( customFilterValue, StringComparison.OrdinalIgnoreCase ) == true
            || model.LastName?.Contains( customFilterValue, StringComparison.OrdinalIgnoreCase ) == true
            || model.Email?.Contains( customFilterValue, StringComparison.OrdinalIgnoreCase ) == true;
    }

    private async Task OnReadData( DataGridReadDataEventArgs<Employee> e )
    {
        if ( !e.CancellationToken.IsCancellationRequested )
        {
            List<Employee> response = null;

            var filteredData = await FilterData( e.Columns );

            // this can be call to anything, in this case we're calling a fictional api
            if ( e.ReadDataMode is DataGridReadDataMode.Virtualize )
                response = filteredData.Skip( e.VirtualizeOffset ).Take( e.VirtualizeCount ).ToList();
            else if ( e.ReadDataMode is DataGridReadDataMode.Paging )
                response = filteredData.Skip( ( e.Page - 1 ) * e.PageSize ).Take( e.PageSize ).ToList();
            else
                throw new Exception( "Unhandled ReadDataMode" );

            await Task.Delay( random.Next( 100 ) );
            if ( !e.CancellationToken.IsCancellationRequested )
            {
                totalEmployees = filteredData.Count;
                employeeList = new List<Employee>( response ); // an actual data for the current page
            }
        }
    }

    /// <summary>
    /// Simple demo purpose example filter
    /// </summary>
    /// <param name="dataGridColumns"></param>
    /// <returns></returns>
    public Task<List<Employee>> FilterData( IEnumerable<DataGridColumnInfo> dataGridColumns )
    {
        var filteredData = dataModels.ToList();
        var sortByColumns = dataGridColumns.Where( x => x.SortDirection != SortDirection.Default );
        var firstSort = true;
        if ( sortByColumns?.Any() ?? false )
        {
            IOrderedEnumerable<Employee> sortedCols = null;
            foreach ( var sortByColumn in sortByColumns.OrderBy( x => x.SortIndex ) )
            {
                var valueGetter = FunctionCompiler.CreateValueGetter<Employee>( sortByColumn.SortField );

                if ( firstSort )
                {
                    if ( sortByColumn.SortDirection == SortDirection.Ascending )
                        sortedCols = dataModels.OrderBy( x => valueGetter( x ) );
                    else
                        sortedCols = dataModels.OrderByDescending( x => valueGetter( x ) );

                    firstSort = false;
                }
                else
                {
                    if ( sortByColumn.SortDirection == SortDirection.Ascending )
                        sortedCols = sortedCols.ThenBy( x => valueGetter( x ) );
                    else
                        sortedCols = sortedCols.ThenByDescending( x => valueGetter( x ) );
                }
            }
            filteredData = sortedCols.ToList();
        }

        if ( dataGrid.CustomFilter != null )
            filteredData = filteredData.Where( item => item != null && dataGrid.CustomFilter( item ) ).ToList();

        foreach ( var column in dataGridColumns.Where( x => !string.IsNullOrWhiteSpace( x.SearchValue?.ToString() ) ) )
        {
            var valueGetter = FunctionCompiler.CreateValueGetter<Employee>( column.Field );
            filteredData = filteredData.Where( x => valueGetter( x )?.ToString().IndexOf( column.SearchValue.ToString(), StringComparison.OrdinalIgnoreCase ) >= 0 ).ToList();
        }
        return Task.FromResult( filteredData );
    }

    private Task Reset()
    {
        currentPage = 1;
        return dataGrid.Reload();
    }

    private bool OnGenderCustomFilter( object itemValue, object searchValue )
    {
        if ( searchValue is string genderFilter )
        {
            return genderFilter == "*" || genderFilter == itemValue?.ToString();
        }

        return true;
    }

    private void OnFilteredDataChanged( DataGridFilteredDataEventArgs<Employee> eventArgs )
    {
        Console.WriteLine( $"Filter changed > Items: {eventArgs.FilteredItems}; Total: {eventArgs.TotalItems};" );
    }

    private void OnSortChanged( DataGridSortChangedEventArgs eventArgs )
    {
        var sort = string.Equals( eventArgs.ColumnFieldName, eventArgs.FieldName, StringComparison.Ordinal )
            ? string.Empty
            : $" (SortField: {eventArgs.FieldName})";
        Console.WriteLine( $"Sort changed > Field: {eventArgs.ColumnFieldName}{sort}; Direction: {eventArgs.SortDirection};" );
    }

    private string TitleFromGender( string gender )
    {
        return gender switch
        {
            "M" => "Mr.",
            "F" => "Mrs.",
            _ => string.Empty,
        };
    }

    private string TitleToName( string title, object name )
    {
        if ( string.IsNullOrEmpty( title ) )
            return $"{name}";

        return $"{title} {name}";
    }

    #endregion
}