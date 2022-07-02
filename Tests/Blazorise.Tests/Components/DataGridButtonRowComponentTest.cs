#region Using directives
using System.Linq;
using BasicTestApp.Client;
using Blazorise.DataGrid;
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
            BlazoriseConfig.JSInterop.AddDataGrid( this.JSInterop );
        }

        [Theory]
        [InlineData( DataGridEditMode.Form )]
        [InlineData( DataGridEditMode.Inline )]
        [InlineData( DataGridEditMode.Popup )]
        public void New_Should_AddNewItem( DataGridEditMode editMode )
        {
            // setup
            var comp = RenderComponent<DataGridButtonRowComponent>( parameters =>
                parameters.Add( x => x.DataGridEditMode, editMode ) );

            var startingDataCount = comp.Instance.InMemoryData.Count;

            // test
            comp.Find( "#btnNew" ).Click();
            comp.Find( "#btnSave" ).Click();


            var currentDataCount = comp.Instance.InMemoryData.Count;

            // validate
            Assert.Equal( startingDataCount + 1, currentDataCount );
        }

        [Theory]
        [InlineData( DataGridEditMode.Form )]
        [InlineData( DataGridEditMode.Inline )]
        [InlineData( DataGridEditMode.Popup )]
        public void Edit_Should_UpdateItem( DataGridEditMode editMode )
        {
            // setup
            var updatedName = "RaulFromEdit";
            var comp = RenderComponent<DataGridButtonRowComponent>( parameters =>
                parameters.Add( x => x.DataGridEditMode, editMode ) );

            // test
            comp.Find( "tr.table-row-selectable" ).Click();
            comp.Find( "#btnEdit" ).Click();


            var firstInput = comp.Find( "input" );
            firstInput.SetAttribute( "value", updatedName );
            firstInput.Input( updatedName );

            comp.Find( "#btnSave" ).Click();

            var currentName = comp.Instance.InMemoryData[0].Name;

            // validate
            Assert.Contains( comp.Instance.InMemoryData, x => x.Name == updatedName );
        }

        [Theory]
        [InlineData( DataGridEditMode.Form )]
        [InlineData( DataGridEditMode.Inline )]
        [InlineData( DataGridEditMode.Popup )]
        public void Delete_Should_DeleteItem( DataGridEditMode editMode )
        {
            // setup
            var comp = RenderComponent<DataGridButtonRowComponent>( parameters =>
                parameters.Add( x => x.DataGridEditMode, editMode ) );
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
