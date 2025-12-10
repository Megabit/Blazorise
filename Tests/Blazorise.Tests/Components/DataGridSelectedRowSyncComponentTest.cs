#region Using directives
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class DataGridSelectedRowSyncComponentTest : TestContext
{
    public DataGridSelectedRowSyncComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseDataGrid();
    }

    [Fact]
    public async Task Delete_Should_ClearSelectedRow_AndInvokeOnce()
    {
        var comp = RenderComponent<BasicTestApp.Client.DataGridSelectedRowSyncComponent>();

        var rows = comp.FindAll( "tbody tr.table-row-selectable" );
        await rows[0].ClickAsync();

        comp.WaitForAssertion( () =>
        {
            comp.Instance.SelectedRow.Should().NotBeNull();
            comp.Instance.SelectedRowChangedCount.Should().Be( 1 );
        } );

        await comp.Find( "#btnDelete" ).ClickAsync();

        comp.WaitForAssertion( () =>
        {
            comp.Instance.Items.Should().HaveCount( 1 );
            comp.Instance.SelectedRow.Should().BeNull();
            comp.Instance.SelectedRowChangedCount.Should().Be( 2 );
        } );
    }
}