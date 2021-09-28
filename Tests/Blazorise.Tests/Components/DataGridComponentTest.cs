#region Using directives
using BasicTestApp.Client;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Blazorise.Tests.Helpers;
using Bunit;
using Microsoft.AspNetCore.Routing;
using Xunit;
#endregion

namespace Blazorise.Tests.Components
{
    public class DataGridComponentTest : TestContext
    {
        public DataGridComponentTest()
        {
            BlazoriseConfig.AddBootstrapProviders( Services );
        }

        [Fact]
        public void DetailRow_OnClick_ShouldRender()
        {
            // Arrange
            var cut = RenderComponent<DataGridComponent>();

            // Act
            var tableRow = cut.Find( "tr.table-row-selectable" );
            tableRow.Click( detail: 1 );

            // Assert
            var detailRow = cut.Find( "span.detail-row" );
            Assert.NotNull( detailRow );
            Assert.NotNull( cut.Instance.SelectedRow );
        }

    }
}
