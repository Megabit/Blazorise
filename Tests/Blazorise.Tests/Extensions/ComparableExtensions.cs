#region Using Directives
using Blazorise.Tests.Helpers;
using Exceptionless;
using Xunit;
#endregion
namespace Blazorise.Tests.Extensions;

public sealed class ComparableExtensions
{
    [Theory]
    [InlineData( -100, 100 )]
    public void IsBetween_ShouldReturnTrueWhenValueFallsBetweenRange( int lowerBound, int upperBound )
    {
        //ARRANGE
        var testValue = RandomData.GetInt( -99, 99 );

        //ACT
        var result = testValue.IsBetween( lowerBound, upperBound );

        //ASSERT
        Assert.True( result );
    }

    [Theory]
    [InlineData( -100, 100 )]
    public void IsBetween_ShouldReturnFalseWhenValueFallsOutsideRange( int lowerBound, int upperBound )
    {
        //ARRANGE
        const int testValue = 101;

        //ACT
        var result = testValue.IsBetween( lowerBound, upperBound );

        //ASSERT
        Assert.False( result );
    }

    [Theory]
    [InlineData( -100, 100 )]
    public void IsBetweenExclusiveLowerBound_ShouldReturnTrueWhenValueFallsBetweenRange( int lowerBound, int upperBound )
    {
        //ARRANGE
        var testValue = RandomData.GetInt( -99, 99 );

        //ACT
        var result = testValue.IsBetweenExclusiveLowerBound( lowerBound, upperBound );

        //ASSERT
        Assert.True( result );
    }

    [Theory]
    [InlineData( -100, 100 )]
    public void IsBetweenExclusiveLowerBound_ShouldReturnFalseWhenValueFallsOnLowerBound( int lowerBound, int upperBound )
    {
        //ARRANGE
        var testValue = -100;

        //ACT
        var result = testValue.IsBetweenExclusiveLowerBound( lowerBound, upperBound );

        //ASSERT
        Assert.False( result );
    }

    [Theory]
    [InlineData( -100, 100 )]
    public void IsBetweenExclusiveUpperBound_ShouldReturnTrueWhenValueFallsBetweenRange( int lowerBound, int upperBound )
    {
        //ARRANGE
        var testValue = RandomData.GetInt( -99, 99 );

        //ACT
        var result = testValue.IsBetweenExclusiveUpperBound( lowerBound, upperBound );


        //ASSERT
        Assert.True( result );
    }

    [Theory]
    [InlineData( -100, 100 )]
    public void IsBetweenExclusiveUpperBound_ShouldReturnFalseWhenValueFallsOnUpperBound( int lowerBound, int upperBound )
    {
        //ARRANGE
        var testValue = 100;

        //ACT
        var result = testValue.IsBetweenExclusiveUpperBound( lowerBound, upperBound );

        //ASSERT
        Assert.False( result );
    }

    [Theory]
    [InlineData( -100, 100 )]
    public void IsBetweenExclusiveBounds_ShouldReturnTrueWhenValueFallsBetweenRange( int lowerBound, int upperBound )
    {
        //ARRANGE
        var testValue = RandomData.GetInt( -99, 99 );

        //ACT
        var result = testValue.IsBetweenExclusiveBounds( lowerBound, upperBound );


        //ASSERT
        Assert.True( result );
    }

    [Theory]
    [InlineData( -100, 100, -100 )]
    [InlineData( -100, 100, 100 )]
    public void IsBetweenExclusiveBounds_ShouldReturnFalseWhenValueIsAnEndpoint( int lowerBound, int upperBound, int testValue )
    {
        //ARRANGE & ACT
        var result = testValue.IsBetweenExclusiveBounds( lowerBound, upperBound );

        //ASSERT
        Assert.False( result );
    }

    [Theory]
    [InlineData( -100, 100 )]
    public void IsNotBetween_ShouldReturnFalseWhenValueFallsBetweenRange( int lowerBound, int upperBound )
    {
        //ARRANGE
        var testValue = RandomData.GetInt( -99, 99 );

        //ACT
        var result = testValue.IsBetween( lowerBound, upperBound );


        //ASSERT
        Assert.True( result );
    }

    [Theory]
    [InlineData( -100, 100 )]
    public void IsNotBetween_ShouldReturnTrueWhenValueFallsOutsideRange( int lowerBound, int upperBound )
    {
        //ARRANGE
        var testValue = 101;

        //ACT
        var result = testValue.IsBetween( lowerBound, upperBound );


        //ASSERT
        Assert.False( result );
    }
}

