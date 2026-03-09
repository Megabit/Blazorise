#region Using directives
using System.Threading.Tasks;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class DataGridSelectionSyncComponentTest : TestContext
{
    public DataGridSelectionSyncComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseDataGrid();
    }

    [Fact]
    public async Task RemovingItems_Should_ClearSelectedRows()
    {
        var comp = RenderComponent<BasicTestApp.Client.DataGridSelectionSyncComponent>();

        await comp.Find( "thead input[type=checkbox]" ).ChangeAsync( "true" );

        comp.WaitForAssertion( () => comp.Instance.SelectedRows.Should().HaveCount( 3 ) );

        await comp.Find( "#btnRemove" ).ClickAsync();

        comp.WaitForAssertion( () =>
        {
            comp.Instance.Items.Should().BeEmpty();
            comp.Instance.SelectedRows.Should().BeEmpty();
        } );
    }

    [Fact]
    public async Task RowCheckbox_Should_Select_And_Unselect_Row()
    {
        var comp = RenderComponent<BasicTestApp.Client.DataGridSelectionSyncComponent>();

        await comp.Find( "tbody input[type=checkbox]" ).ChangeAsync( "true" );

        comp.WaitForAssertion( () =>
        {
            comp.Instance.SelectedRows.Should().HaveCount( 1 );
            comp.Instance.SelectedRows[0].Id.Should().Be( 1 );
        } );

        await comp.Find( "tbody input[type=checkbox]" ).ChangeAsync( "false" );

        comp.WaitForAssertion( () => comp.Instance.SelectedRows.Should().BeEmpty() );
    }
}