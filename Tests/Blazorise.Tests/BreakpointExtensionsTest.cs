#region Using directives
using Xunit;
#endregion

namespace Blazorise.Tests;

public class BreakpointExtensionsTest
{
    [Theory]
    [InlineData( Breakpoint.ExtraSmall, Breakpoint.Mobile )]
    [InlineData( Breakpoint.Small, Breakpoint.Tablet )]
    [InlineData( Breakpoint.Medium, Breakpoint.Desktop )]
    [InlineData( Breakpoint.Large, Breakpoint.Widescreen )]
    [InlineData( Breakpoint.ExtraLarge, Breakpoint.FullHD )]
    [InlineData( Breakpoint.ExtraExtraLarge, Breakpoint.QuadHD )]
    public void Normalize_ShouldMapAliases_ToCanonicalBreakpoints( Breakpoint breakpoint, Breakpoint expected )
    {
        Assert.Equal( expected, breakpoint.Normalize() );
    }

    [Theory]
    [InlineData( "mobile", Breakpoint.Mobile )]
    [InlineData( "sm", Breakpoint.Tablet )]
    [InlineData( "medium", Breakpoint.Desktop )]
    [InlineData( "large", Breakpoint.Widescreen )]
    [InlineData( "xl", Breakpoint.FullHD )]
    [InlineData( "extra-extra-large", Breakpoint.QuadHD )]
    public void ParseBreakpoint_ShouldHandleCanonicalAndAliasNames( string breakpointName, Breakpoint expected )
    {
        Assert.Equal( expected, breakpointName.ParseBreakpoint() );
    }

    [Theory]
    [InlineData( Breakpoint.Small, "tablet", false )]
    [InlineData( Breakpoint.Medium, "tablet", true )]
    [InlineData( Breakpoint.ExtraLarge, "fullhd", false )]
    [InlineData( Breakpoint.QuadHD, "fullhd", true )]
    public void IsBroken_ShouldRespectAliasedBreakpoints( Breakpoint requestedBreakpoint, string currentBreakpoint, bool expected )
    {
        Assert.Equal( expected, BreakpointActivatorAdapter.IsBroken( requestedBreakpoint, currentBreakpoint ) );
    }
}