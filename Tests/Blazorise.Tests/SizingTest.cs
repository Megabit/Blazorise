#region Using directives
using Blazorise.Bootstrap.Providers;
using Xunit;
#endregion

namespace Blazorise.Tests;

public class SizingTest
{
    IClassProvider classProvider;

    IStyleProvider styleProvider;

    public SizingTest()
    {
        classProvider = new BootstrapClassProvider();
        styleProvider = new BootstrapStyleProvider();
    }

    [Theory]
    [InlineData( "w-25", SizingSize.Is25 )]
    [InlineData( "w-33", SizingSize.Is33 )]
    [InlineData( "w-50", SizingSize.Is50 )]
    [InlineData( "w-66", SizingSize.Is66 )]
    [InlineData( "w-75", SizingSize.Is75 )]
    [InlineData( "w-100", SizingSize.Is100 )]
    [InlineData( "w-auto", SizingSize.Auto )]
    public void AreWidth( string expected, SizingSize sizingSize )
    {
        var sizing = new FluentSizing( SizingType.Width );

        sizing.WithSize( sizingSize );

        var classname = sizing.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Theory]
    [InlineData( "h-25", SizingSize.Is25 )]
    [InlineData( "h-33", SizingSize.Is33 )]
    [InlineData( "h-50", SizingSize.Is50 )]
    [InlineData( "h-66", SizingSize.Is66 )]
    [InlineData( "h-75", SizingSize.Is75 )]
    [InlineData( "h-100", SizingSize.Is100 )]
    [InlineData( "h-auto", SizingSize.Auto )]
    public void AreHeight( string expected, SizingSize sizingSize )
    {
        var sizing = new FluentSizing( SizingType.Height );

        sizing.WithSize( sizingSize );

        var classname = sizing.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Fact]
    public void IsWidth_Max100()
    {
        var sizing = new FluentSizing( SizingType.Width );

        sizing.WithSize( SizingSize.Is100 );
        sizing.WithMax();

        var classname = sizing.Class( classProvider );

        Assert.Equal( "mw-100", classname );
    }

    [Fact]
    public void IsWidth_MinViewport()
    {
        var sizing = new FluentSizing( SizingType.Width );

        sizing.WithSize( SizingSize.Is100 );
        sizing.WithMin();
        sizing.WithViewport();

        var classname = sizing.Class( classProvider );

        Assert.Equal( "min-vw-100", classname );
    }

    [Theory]
    [InlineData( SizingType.Width, "width: 8rem" )]
    [InlineData( SizingType.Height, "height: 8rem" )]
    public void RemShorthand_UsesContextualSizingType( SizingType sizingType, string expected )
    {
        FluentCssValue sizing = 8.Rem();

        string propertyName = sizingType == SizingType.Width
            ? "width"
            : "height";

        string style = sizing.Style( propertyName, true );

        Assert.Equal( expected, style );
    }

    [Theory]
    [InlineData( SizingType.Width, "width: 50%" )]
    [InlineData( SizingType.Height, "height: 50%" )]
    public void PercentShorthand_UsesContextualSizingType( SizingType sizingType, string expected )
    {
        FluentCssValue sizing = 50.Percent();

        string propertyName = sizingType == SizingType.Width
            ? "width"
            : "height";

        string style = sizing.Style( propertyName, true );

        Assert.Equal( expected, style );
    }

    [Theory]
    [InlineData( SizingType.Width, "100% - 2rem", "width: calc(100% - 2rem)" )]
    [InlineData( SizingType.Height, "calc(100vh - 4rem)", "height: calc(100vh - 4rem)" )]
    public void Calc_BuildsSizingStyle( SizingType sizingType, string expression, string expected )
    {
        IFluentSizing sizing = sizingType == SizingType.Width
            ? Width.Calc( expression )
            : Height.Calc( expression );

        string style = sizing.Style( styleProvider );

        Assert.Equal( expected, style );
    }

    [Fact]
    public void RemShorthand_CanBeUsedByCompatibleUtilities()
    {
        IFluentSizing sizing = 3.Rem();
        IFluentGap gap = 3.Rem();
        IFluentTextSize textSize = 2.Rem();

        Assert.Equal( "width: 3rem", ( (FluentCssValue)sizing ).Style( "width", true ) );
        Assert.Equal( "gap: 3rem", ( (FluentCssValue)gap ).Style( "gap" ) );
        Assert.Equal( "font-size: 2rem", ( (FluentCssValue)textSize ).Style( "font-size" ) );
    }

    [Fact]
    public void RemShorthand_PreservesSizingMinMax()
    {
        FluentCssValue sizing = 8.Rem();

        sizing.Min( 4 ).Max( 12 );

        Assert.Equal( "width: 8rem; min-width: 4rem; max-width: 12rem", sizing.Style( "width", true ) );
    }

    [Fact]
    public void ExplicitUtilityBuilders_CreateCssValues()
    {
        IFluentGap gap = Gap.Rem( 5 );
        IFluentTextSize textSize = TextSize.Rem( 2 );

        Assert.Equal( "gap: 5rem", ( (FluentCssValue)gap ).Style( "gap" ) );
        Assert.Equal( "font-size: 2rem", ( (FluentCssValue)textSize ).Style( "font-size" ) );
    }

    [Theory]
    [InlineData( SizingType.Width, "width: 8rem; min-width: 4rem; max-width: 12rem" )]
    [InlineData( SizingType.Height, "height: 8rem; min-height: 4rem; max-height: 12rem" )]
    public void ExplicitSizingBuilders_PreserveMinMax( SizingType sizingType, string expected )
    {
        IFluentSizingStyle sizing = sizingType == SizingType.Width
            ? Width.Rem( 8 )
            : Height.Rem( 8 );

        sizing.Min( 4 ).Max( 12 );

        Assert.Equal( expected, sizing.Style( styleProvider ) );
    }
}