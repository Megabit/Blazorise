namespace Blazorise.E2E.Tests.Tests.Extensions.DataGrid;

public class DataGridRapidEditingTests : BlazorisePageTest
{
    [TestCase( 0, false, "1", "234.56", "1,234.56", "1234.56" )]
    [TestCase( 0, true, "1", "234.56", "1,234.56", "1234.56" )]
    [TestCase( 1, false, "1", "234.56", "1,234.56", "1234.56" )]
    [TestCase( 1, true, "1", "234.56", "1,234.56", "1234.56" )]
    [TestCase( 1, false, "1", "234", "1,234.00", "1234.00" )]
    [TestCase( 2, false, "1", "234,56", "1.234,56", "1234.56" )]
    [TestCase( 2, true, "1", "234,56", "1.234,56", "1234.56" )]
    [TestCase( 3, false, "1", "234", "1,234", "1234" )]
    [TestCase( 4, false, "a", "bc", "abc", "abc" )]
    public async Task TypingIntoSelectedCell_ShouldContinueAfterFirstCharacter( int columnIndex, bool useValidation, string firstKey, string remainingText, string editorValue, string savedValue )
    {
        await SelectTestComponent<DataGridRapidEditingComponent>();

        if ( useValidation )
        {
            await Page.Locator( "#useValidation" ).CheckAsync();
        }

        // Edit mode inserts a validation summary row before the data row.
        var cell = Page.Locator( "tbody tr[data-row-index='0']" ).Locator( "td" ).Nth( columnIndex );
        await cell.ClickAsync();
        await Expect( cell ).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync( firstKey );

        var input = cell.Locator( "input" );
        await Expect( input ).ToBeFocusedAsync();

        if ( columnIndex < 3 )
        {
            await Expect( input ).ToHaveValueAsync( columnIndex == 2 ? "1,00" : "1.00" );
            Assert.AreEqual( 1, await input.EvaluateAsync<int>( "element => element.selectionStart" ) );
            Assert.AreEqual( 1, await input.EvaluateAsync<int>( "element => element.selectionEnd" ) );
        }

        await Page.Keyboard.TypeAsync( remainingText );

        await Expect( input ).ToHaveValueAsync( editorValue );
        await Page.Keyboard.PressAsync( "Enter" );
        await Expect( input ).ToHaveCountAsync( 0 );
        await Expect( cell ).ToHaveTextAsync( savedValue );
    }

    [TestCase( 0 )]
    [TestCase( 1 )]
    public async Task EnterIntoNumericCell_ShouldStillSelectExistingValue( int columnIndex )
    {
        await SelectTestComponent<DataGridRapidEditingComponent>();

        var cell = Page.Locator( "tbody tr[data-row-index='0']" ).Locator( "td" ).Nth( columnIndex );
        await cell.ClickAsync();
        await Expect( cell ).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync( "Enter" );

        var input = cell.Locator( "input" );
        await Expect( input ).ToBeFocusedAsync();
        await Expect( input ).ToHaveValueAsync( "9,876.54" );
        Assert.AreEqual( 0, await input.EvaluateAsync<int>( "element => element.selectionStart" ) );
        Assert.AreEqual( 8, await input.EvaluateAsync<int>( "element => element.selectionEnd" ) );
        await Page.Keyboard.TypeAsync( "42" );
        // Replacing the entire selection also replaces the padded decimals while editing.
        await Expect( input ).ToHaveValueAsync( "42" );
        await Page.Keyboard.PressAsync( "Escape" );

        await Expect( input ).ToHaveCountAsync( 0 );
        await Expect( cell ).ToHaveTextAsync( "9876.54" );
    }
}