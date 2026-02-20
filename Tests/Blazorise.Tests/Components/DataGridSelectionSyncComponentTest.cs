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
}