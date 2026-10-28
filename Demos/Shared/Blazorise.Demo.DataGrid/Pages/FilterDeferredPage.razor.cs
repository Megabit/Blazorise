#region Using directives
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

public partial class FilterDeferredPage
{
    #region Members

    private DataGrid<Employee> localGrid;

    private DataGrid<Employee> remoteGrid;

    private List<Employee> employees;

    private List<Employee> remoteEmployees = new();

    private int totalEmployees;

    private int readDataCount;

    #endregion

    #region Methods

    protected override async Task OnInitializedAsync()
    {
        employees = await EmployeeData.GetDataAsync();

        await base.OnInitializedAsync();
    }

    private Task OnApplyLocalFiltersClickedHandler() => localGrid.ApplyFilters();

    private async Task OnClearLocalFiltersClickedHandler()
    {
        await localGrid.ClearFilter();
        await localGrid.ApplyFilters();
    }

    private Task OnApplyRemoteFiltersClickedHandler() => remoteGrid.ApplyFilters();

    private async Task OnClearRemoteFiltersClickedHandler()
    {
        await remoteGrid.ClearFilter();
        await remoteGrid.ApplyFilters();
    }

    private Task OnReadDataHandler( DataGridReadDataEventArgs<Employee> args )
    {
        if ( args.CancellationToken.IsCancellationRequested )
        {
            return Task.CompletedTask;
        }

        var query = employees.AsQueryable().ApplyDataGridSearch( args );

        totalEmployees = query.Count();

        remoteEmployees = query
            .ApplyDataGridSort( args )
            .ApplyDataGridPaging( args )
            .ToList();

        readDataCount++;

        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    [Inject] private EmployeeData EmployeeData { get; set; }

    #endregion
}