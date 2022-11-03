#region Using directives
using System.Threading.Tasks;
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;
using static System.Net.Mime.MediaTypeNames;
#endregion

namespace Blazorise.Tests.Components
{
    public class AutocompleteComponentTest : AutocompleteBaseComponentTest
    {
        public AutocompleteComponentTest()
        {
            BlazoriseConfig.AddBootstrapProviders( Services );
            BlazoriseConfig.JSInterop.AddTextEdit( this.JSInterop );
            BlazoriseConfig.JSInterop.AddUtilities( this.JSInterop );
            BlazoriseConfig.JSInterop.AddClosable( this.JSInterop );
            BlazoriseConfig.JSInterop.AddDropdown( this.JSInterop );
        }

        [Fact]
        public async Task SelectedValueChanged_ShouldOnlyTrigger_WhenValueHasBeenFound()
        {
            var changedCount = 0;
            var comp = RenderComponent<AutocompleteComponent>( p =>
            p.Add( x => x.SelectedValueChanged, ( x ) => changedCount++ ) );

            var autoComplete = comp.Find( ".b-is-autocomplete input" );
            var input = "Portugal";

            await Input( autoComplete, input );

            Assert.Equal( 1, changedCount );
        }

        [Fact]
        public async Task SelectedValueChanged_ShouldOnlyTrigger_IfValueIsAlreadySet_But_ValueHasNotBeenFound()
        {
            var changedCount = 0;
            var selectedValue = "PT";
            var comp = RenderComponent<AutocompleteComponent>( p =>
            {
                p.Add( x => x.SelectedValue, selectedValue );
                p.Add( x => x.SelectedValueChanged, ( x ) => { selectedValue = x; changedCount++; } );
            }

            );

            var autoComplete = comp.Find( ".b-is-autocomplete input" );
            var input = "A Random Value!";

            await Input( autoComplete, input );

            Assert.Equal( 1, changedCount );
            Assert.Equal( default, selectedValue );
        }

        [Fact]
        public async Task SelectedTextChanged_ShouldOnlyTrigger_WhenValueHasBeenFound()
        {
            var changedCount = 0;
            var comp = RenderComponent<AutocompleteComponent>( p =>
            p.Add( x => x.SelectedTextChanged, ( x ) => changedCount++ ) );

            var autoComplete = comp.Find( ".b-is-autocomplete input" );
            var input = "Portugal";

            await Input( autoComplete, input );

            Assert.Equal( 1, changedCount );
        }

        [Fact]
        public async Task SelectedTextChanged_ShouldOnlyTrigger_IfValueIsAlreadySet_But_TextHasNotBeenFound()
        {
            var changedCount = 0;
            var selectedValue = "Portugal";
            var comp = RenderComponent<AutocompleteComponent>( p =>
            {
                p.Add( x => x.SelectedText, selectedValue );
                p.Add( x => x.SelectedTextChanged, ( x ) => { selectedValue = x; changedCount++; } );
            }

            );

            var autoComplete = comp.Find( ".b-is-autocomplete input" );
            var input = "A Random Value!";

            await Input( autoComplete, input );

            Assert.Equal( 1, changedCount );
            Assert.Equal( default, selectedValue );
        }

        [Fact]
        public void Focus_ShouldFocus()
        {
            TestFocus<AutocompleteComponent>( ( comp ) => comp.Instance.AutoCompleteRef.Focus() );
        }

        [Fact]
        public void Clear_ShouldReset()
        {
            TestClear<AutocompleteComponent>( ( comp ) => comp.Instance.AutoCompleteRef.Clear(), ( comp ) => comp.Instance.SelectedText );
        }

        [Fact]
        public void InitialSelectedValue_ShouldSet_SelectedText()
        {
            TestInitialSelectedValue<AutocompleteComponent>( ( comp ) => comp.Instance.SelectedText );
        }

        [Theory]
        [InlineData( "Portugal" )]
        [InlineData( "Antarctica" )]
        [InlineData( "United Kingdom" )]
        [InlineData( "China" )]
        public void SelectValue_ShouldSet( string expectedText )
        {
            TestSelectValue<AutocompleteComponent>( expectedText, ( comp ) => comp.Instance.SelectedText );
        }

        [Theory]
        [InlineData( "MyCustomValue" )]
        public void FreeTypedValue_ShouldSet( string freeTyped )
        {
            TestFreeTypedValue<AutocompleteComponent>( freeTyped, ( comp ) => comp.Instance.SelectedText );
        }

        [Theory]
        [InlineData( true, "Portuga", "Portugal" )]
        [InlineData( true, "Chin", "China" )]
        [InlineData( true, "United King", "United Kingdom" )]
        [InlineData( false, "Portuga", "Portuga" )]
        [InlineData( false, "Chin", "Chin" )]
        [InlineData( false, "United King", "United King" )]
        public void FreeTypedValue_AutoPreSelect_ShouldSet( bool autoPreSelect, string freeTyped, string expectedText )
        {
            TestFreeTypedValue_AutoPreSelect<AutocompleteComponent>( autoPreSelect, freeTyped, expectedText, ( comp ) => comp.Instance.SelectedText );
        }

        [Fact]
        public void AutoPreSelect_True_Should_AutoPreSelectFirstItem()
        {
            TestHasPreselection<AutocompleteComponent>();
        }

        [Fact]
        public void AutoPreSelect_False_ShouldNot_AutoPreSelectFirstItem()
        {
            TestHasNotPreselection<AutocompleteComponent>();
        }

        [Fact]
        public void MinLength_0_ShouldShowOptions_OnFocus()
        {
            TestMinLen0ShowsOptions<AutocompleteComponent>();
        }

        [Fact]
        public void MinLength_BiggerThen0_ShouldNotShowOptions_OnFocus()
        {
            TestMinLenBiggerThen0DoesNotShowOptions<AutocompleteComponent>();
        }

        [Theory]
        [InlineData( "CN", "China" )]
        [InlineData( "PT", "Portugal" )]
        [InlineData( "GB", "United Kingdom" )]
        public void ProgramaticallySetSelectedValue_ShouldSet_SelectedText( string selectedValue, string expectedSelectedText )
        {
            TestProgramaticallySetSelectedValue<AutocompleteComponent>( ( comp ) => comp.Instance.SelectedText, selectedValue, expectedSelectedText );
        }
    }
}
