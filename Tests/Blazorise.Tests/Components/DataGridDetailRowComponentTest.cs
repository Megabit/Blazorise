#region Using directives
using System.Linq;
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components
{
    public class DataGridDetailRowComponentTest : TestContext
    {
        public DataGridDetailRowComponentTest()
        {
            BlazoriseConfig.AddBootstrapProviders( Services );
            BlazoriseConfig.JSInterop.AddDataGrid( this.JSInterop );
        }

        [Fact]
        public void DetailRow_DetailRowStartsVisible_True_ShouldRender()
        {
            // setup
            var comp = RenderComponent<DataGridDetailRowComponent>();

            // test
            var rows = comp.FindAll( "#lblFraction" );
            var rowsFraction = comp.FindAll( "tbody tr.table-row-selectable td:first-child" );

            // validate
            for ( int i = 0; i < rowsFraction.Count; i++ )
                Assert.Equal( rowsFraction[i].TextContent, rows[i].TextContent );
        }

        [Fact]
        public void DetailRow_DetailRowStartsVisible_False_ShouldNotRender()
        {
            // setup
            var comp = RenderComponent<DataGridDetailRowComponent>(
                parameters => parameters.Add( x => x.DetailRowStartsVisible, false ) );

            // test
            var rows = comp.FindAll( "#lblFraction" );

            // validate
            Assert.Equal( 0, rows.Count );
        }

        [Fact]
        public void DetailRow_OnClick_ShouldHide()
        {
            // setup
            var comp = RenderComponent<DataGridDetailRowComponent>();

            // test
            var rowsFraction = comp.FindAll( "tbody tr.table-row-selectable" );
            foreach ( var row in rowsFraction )
            {
                row.Click( detail: 1 );
            }

            var rows = comp.FindAll( "#lblFraction" );

            // validate
            Assert.Equal( 0, rows.Count );
        }

        [Fact]
        public void DetailRow_OnToggleDetailRowTrigger_ShouldTrigger()
        {
            // setup
            var comp = RenderComponent<DataGridDetailRowComponent>();

            // test
            var rowsBefore = comp.FindAll( "#lblFraction" );
            foreach ( var item in comp.Instance.InMemoryData )
            {
                comp.Instance.DataGridRef.ToggleDetailRow( item );
            }

            var rowsAfter = comp.FindAll( "#lblFraction" );

            foreach ( var item in comp.Instance.InMemoryData )
            {
                comp.Instance.DataGridRef.ToggleDetailRow( item );
            }

            var rowsAfter2 = comp.FindAll( "#lblFraction" );

            // validate
            //Start
            Assert.Equal( comp.Instance.InMemoryData.Count, rowsBefore.Count );
            Assert.Equal( 0, rowsAfter.Count );
            Assert.Equal( comp.Instance.InMemoryData.Count, rowsAfter2.Count );
        }
    }
}
