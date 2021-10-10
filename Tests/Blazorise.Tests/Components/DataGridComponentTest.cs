#region Using directives
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
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
        public void SortByField_Should_CorrectlySortRows()
        {
            // setup
            var expectedOrderedValues = new[] { "1/8", "1/4", "1/2", "3/4" };

            // test
            var comp = RenderComponent<DataGridComponent>();
            var rows = comp.FindAll( "tbody tr td" );

            // validate
            var count = 0;
            foreach ( var item in rows )
            {
                Assert.Equal( item.TextContent, expectedOrderedValues[count] );
                count++;
            }
        }
    }
}
