using System.Text.RegularExpressions;
using Blazorise.Themes;
using Xunit;

namespace Blazorise.Tests
{
    public class ThemeGeneratorTest
    {

        public ThemeGeneratorTest()
        {
        }

        [Theory]
        [InlineData( "", "" )]
        [InlineData( "#", "" )]
        [InlineData( "123", "123" )]
        [InlineData( "#999999", "999999" )]
        [InlineData( "#FFFFFF", "FFFFFF" )]
        [InlineData( "#ABCDEF", "ABCDEF" )]
        [InlineData( "#abcdef", "abcdef" )]
        [InlineData( "abcdef", "abcdef" )]
        [InlineData( "abcdeflol", "abcdef" )]
        [InlineData( "lolabcdeflol", "abcdef" )]
        public void CachedExtractHexDigitsMatches_Returns_CorrectHexDigits( string colorInput, string expected)
        {
            var result = CachedExtractHexDigitsMatches( colorInput );

            Assert.Equal( expected, result );
        }

        [Theory]
        [InlineData( "", "" )]
        [InlineData( "#", "" )]
        [InlineData( "123", "123" )]
        [InlineData( "#999999", "999999" )]
        [InlineData( "#FFFFFF", "FFFFFF" )]
        [InlineData( "#ABCDEF", "ABCDEF" )]
        [InlineData( "#abcdef", "abcdef" )]
        [InlineData( "abcdef", "abcdef" )]
        [InlineData( "abcdeflol", "abcdef" )]
        [InlineData( "lolabcdeflol", "abcdef" )]
        public void CachedExtractHexDigitsAllocateOnlyOnce_Returns_CorrectHexDigits( string colorInput, string expected )
        {
            var result = CachedExtractHexDigitsAllocateOnlyOnce( colorInput );

            Assert.Equal( expected, result );
        }

        protected static string CachedExtractHexDigitsMatches( string input )
        {
            string newnum = string.Empty;
            var result = isHexDigit.Matches( input );
            foreach ( System.Text.RegularExpressions.Match item in result )
            {
                newnum += item.Value;
            }
            return newnum;
        }

        protected static string CachedExtractHexDigitsAllocateOnlyOnce( string input )
        {
            string newnum = "";
            foreach ( char c in input )
            {
                var charAsString = c.ToString();
                if ( isHexDigit.IsMatch( charAsString ) )
                    newnum += charAsString;
            }

            return newnum;
        }

        private static Regex isHexDigit = new( "[abcdefABCDEF\\d]+", RegexOptions.Compiled );


    }
}
