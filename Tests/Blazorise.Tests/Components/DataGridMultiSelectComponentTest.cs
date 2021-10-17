#region Using directives
using System.Linq;
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components
{

    public class DataGridMultiSelectComponentTestBs5 : DataGridMultiSelectComponentTest
    {
        public DataGridMultiSelectComponentTestBs5() : base()
        {

        }

        protected override void AddProviders( TestServiceProvider services )
        {
            BlazoriseConfig.AddBootstrap5Providers( services );
        }
    }

    public class DataGridMultiSelectComponentTest : TestContext
    {
        public DataGridMultiSelectComponentTest()
        {
            AddProviders( Services );

        }

        protected virtual void AddProviders( TestServiceProvider services )
        {
            BlazoriseConfig.AddBootstrapProviders( services );
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
