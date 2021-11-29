#region Using directives
using System.Linq;
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components
{

    public class DataGridMultiSelectComponentTest : TestContext
    {
        public DataGridMultiSelectComponentTest()
        {
            BlazoriseConfig.AddBootstrapProviders( Services );
            BlazoriseConfig.JSInterop.AddDataGrid( this.JSInterop );
        }

        [Fact]
        public void MultipleSelectAll_Should_Select_Unselect_AllRows()
        {
            var comp = RenderComponent<DataGridMultiSelectComponent>();
            comp.Find( "input[type=checkbox]:first-child" ).Change( true );
            var allCheckbox = comp.FindAll( "input[type=checkbox]" );
            foreach ( var checkbox in allCheckbox )
            {
                Assert.NotNull( checkbox.GetAttribute( "checked" ) );
            }

            comp.Find( "input[type=checkbox]:first-child" ).Change( false );
            allCheckbox = comp.FindAll( "input[type=checkbox]" );
            foreach ( var checkbox in allCheckbox )
            {
                Assert.Null( checkbox.GetAttribute( "checked" ) );
            }
        }
    }
}
