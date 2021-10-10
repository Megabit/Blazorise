#region Using directives
using System.Linq;
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
            var rows = comp.FindAll( "tbody tr td:nth-child(2)" );

            // validate
            var count = 0;
            foreach ( var item in rows )
            {
                Assert.Equal( item.TextContent, expectedOrderedValues[count] );
                count++;
            }
        }

        [Fact]
        public void New_Should_AddNewItem()
        {
            // setup
            var comp = RenderComponent<DataGridComponent>();
            var startingDataCount = comp.Instance.InMemoryData.Count;

            // test
            comp.Find( "#btnNew" ).Click();
            comp.Find( "#btnSave" ).Click();

            var currentDataCount = comp.Instance.InMemoryData.Count;

            // validate
            Assert.Equal( startingDataCount + 1, currentDataCount );
        }

        [Fact]
        public void Edit_Should_UpdateItem()
        {
            // setup
            var updatedName = "RaulFromEdit";
            var comp = RenderComponent<DataGridComponent>();

            // test
            comp.Find( "#btnEdit" ).Click( detail: 1 );
            var firstInput = comp.Find( "input" );
            firstInput.SetAttribute( "value", updatedName );
            firstInput.Input( updatedName );
            comp.Find( "#btnSave" ).Click( detail: 1 );

            var currentName = comp.Instance.InMemoryData[0].Name;

            // validate
            Assert.Contains( comp.Instance.InMemoryData, x => x.Name == updatedName );
        }

        [Fact]
        public void Delete_Should_DeleteItem()
        {
            // setup
            var comp = RenderComponent<DataGridComponent>();
            var startingDataCount = comp.Instance.InMemoryData.Count;

            // test
            comp.Find( "#btnDelete" ).Click();

            var currentDataCount = comp.Instance.InMemoryData.Count;

            // validate
            Assert.Equal( startingDataCount - 1, currentDataCount );
        }
    }
}
