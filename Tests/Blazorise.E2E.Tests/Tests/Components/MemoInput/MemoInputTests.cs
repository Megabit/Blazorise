namespace Blazorise.E2E.Tests.Tests.Components.MemoInput;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class MemoInputTests : BlazorisePageTest
{
    [Test]
    public async Task AutoSize_GrowsAndShrinksWithContent()
    {
        await SelectTestComponent<MemoInputComponent>();

        ILocator textarea = Page.Locator( "#memo-autosize" );
        float initialHeight = await GetHeight( textarea );

        await textarea.FillAsync( string.Join( "\n", Enumerable.Repeat( "A line entered by the user.", 8 ) ) );
        float grownHeight = await GetHeight( textarea );

        Assert.That( grownHeight, Is.GreaterThan( initialHeight ) );

        await textarea.FillAsync( "Short value" );
        float shrunkHeight = await GetHeight( textarea );

        Assert.That( shrunkHeight, Is.LessThan( grownHeight ) );
        Assert.That( shrunkHeight, Is.GreaterThanOrEqualTo( initialHeight ) );
    }

    [Test]
    public async Task AutoSize_RespondsToProgrammaticValuesAndRuntimeToggling()
    {
        await SelectTestComponent<MemoInputComponent>();

        ILocator textarea = Page.Locator( "#memo-autosize" );
        float initialHeight = await GetHeight( textarea );

        await Page.Locator( "#memo-set-long-value" ).ClickAsync();
        float programmaticHeight = await GetHeight( textarea );

        Assert.That( programmaticHeight, Is.GreaterThan( initialHeight ) );

        await Page.Locator( "#memo-toggle-autosize" ).ClickAsync();

        await Expect( textarea ).Not.ToHaveAttributeAsync( "data-autosize", "true" );
    }

    [Test]
    public async Task AutoSize_UsesJavaScriptFallbackWhenFieldSizingIsUnsupported()
    {
        await Page.AddInitScriptAsync(
            """
            const nativeSupports = CSS.supports.bind(CSS);
            CSS.supports = (property, value) => property === 'field-sizing'
                ? false
                : nativeSupports(property, value);
            """ );

        await SelectTestComponent<MemoInputComponent>();

        ILocator textarea = Page.Locator( "#memo-autosize" );

        await Expect( textarea ).ToHaveAttributeAsync( "data-blazorise-memo-auto-sized", "true" );

        float initialHeight = await GetHeight( textarea );

        await textarea.FillAsync( string.Join( "\n", Enumerable.Repeat( "Fallback content.", 8 ) ) );
        float grownHeight = await GetHeight( textarea );

        Assert.That( grownHeight, Is.GreaterThan( initialHeight ) );

        await Page.Locator( "#memo-toggle-autosize" ).ClickAsync();
        await Expect( textarea ).Not.ToHaveAttributeAsync( "data-blazorise-memo-auto-sized", "true" );
    }

    private static Task<float> GetHeight( ILocator locator )
        => locator.EvaluateAsync<float>( "element => element.getBoundingClientRect().height" );
}