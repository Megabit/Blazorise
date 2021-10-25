#region Using directives
using System.Linq;
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components
{
    public class DataGridButtonRowComponentTest : TestContext
    {
        public DataGridButtonRowComponentTest()
        {
            BlazoriseConfig.AddBootstrapProviders( Services );
            BlazoriseConfig.JSInterop.AddButton( this.JSInterop );
            BlazoriseConfig.JSInterop.AddTextEdit( this.JSInterop );
        }

        [Fact]
        public void New_Should_AddNewItem()
        {
            // setup
            var comp = RenderComponent<DataGridButtonRowComponent>();
            var startingDataCount = comp.Instance.InMemoryData.Count;

            // test
            comp.Find( "#btnNew" ).Click();
            var allBtn = comp.FindAll( "button" );
            allBtn.First( x => x.InnerHtml == "Save" ).Click();


            var currentDataCount = comp.Instance.InMemoryData.Count;

            // validate
            Assert.Equal( startingDataCount + 1, currentDataCount );
        }

        [Fact]
        public void Edit_Should_UpdateItem()
        {
            // setup
            var updatedName = "RaulFromEdit";
            var comp = RenderComponent<DataGridButtonRowComponent>();

            // test
            comp.Find( "tr.table-row-selectable" ).Click();
            comp.Find( "#btnEdit" ).Click();


            var firstInput = comp.Find( "input" );
            firstInput.SetAttribute( "value", updatedName );
            firstInput.Input( updatedName );

            var allBtn = comp.FindAll( "button" );
            allBtn.First( x => x.InnerHtml == "Save" ).Click();

            var currentName = comp.Instance.InMemoryData[0].Name;

            // validate
            Assert.Contains( comp.Instance.InMemoryData, x => x.Name == updatedName );
        }

        [Fact]
        public void Delete_Should_DeleteItem()
        {
            // setup
            var comp = RenderComponent<DataGridButtonRowComponent>();
            var startingDataCount = comp.Instance.InMemoryData.Count;

            // test
            comp.Find( "tr.table-row-selectable" ).Click();
            comp.Find( "#btnDelete" ).Click();

            var currentDataCount = comp.Instance.InMemoryData.Count;

            // validate
            Assert.Equal( startingDataCount - 1, currentDataCount );
        }
    }
}
