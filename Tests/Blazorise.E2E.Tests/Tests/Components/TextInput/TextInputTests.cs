namespace Blazorise.E2E.Tests.Tests.Components.TextInput;


[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class TextInputTests : BlazorisePageTest
{
    [Test]
    public async Task CanChangeText()
    {
        await SelectTestComponent<TextInputComponent>();

        var sut = Page.Locator( "#text-basic" );
        var textBox = sut.GetByRole( AriaRole.Textbox );

        await Expect( textBox ).ToHaveValueAsync( string.Empty );

        await textBox.FillAsync( "abc" );
        await Expect( textBox ).ToHaveValueAsync( "abc" );
    }

    [Test]
    public async Task CanChangeTextUsingEvent()
    {
        await SelectTestComponent<TextInputComponent>();

        var sut = Page.Locator( "#text-event-initially-blank" );
        var textBox = sut.GetByRole( AriaRole.Textbox );
        var result = sut.Locator( "#text-event-initially-blank-result" );

        await Expect( result ).ToHaveTextAsync( string.Empty );

        await textBox.FillAsync( "abcdefghijklmnopqrstuvwxyz" );
        await Expect( result ).ToHaveTextAsync( "abcdefghijklmnopqrstuvwxyz" );

        await Page.Keyboard.PressAsync( "Backspace" );
        await Expect( result ).ToHaveTextAsync( "abcdefghijklmnopqrstuvwxy" );
    }

    [Test]
    public async Task CanChangeTextUsingBind()
    {
        await SelectTestComponent<TextInputComponent>();

        var sut = Page.Locator( "#text-bind-initially-blank" );
        var textBox = sut.GetByRole( AriaRole.Textbox );
        var result = sut.Locator( "#text-bind-initially-blank-result" );

        await Expect( result ).ToHaveTextAsync( string.Empty );

        await textBox.FillAsync( "abcdefghijklmnopqrstuvwxyz" );
        await Expect( result ).ToHaveTextAsync( "abcdefghijklmnopqrstuvwxyz" );

        await Page.Keyboard.PressAsync( "Backspace" );
        await Expect( result ).ToHaveTextAsync( "abcdefghijklmnopqrstuvwxy" );
    }

}
