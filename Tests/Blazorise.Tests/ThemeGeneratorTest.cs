using System.Text;
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
        public void ExtractHexDigits_Returns_CorrectHexDigits( string colorInput, string expected )
        {
            var result = MockThemeGenerator.ExtractHexDigitsTest( colorInput );

            Assert.Equal( expected, result );
        }


        public class MockThemeGenerator : ThemeGenerator
        {
            public MockThemeGenerator( IThemeCache themeCache ) : base( themeCache )
            {
            }

            internal static string ExtractHexDigitsTest( string input )
                => ExtractHexDigits( input );

            protected override void GenerateAlertStyles( StringBuilder sb, Theme theme, ThemeAlertOptions options )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateAlertVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inBorderColor, string inColor, ThemeAlertOptions options )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateBackgroundVariantStyles( StringBuilder sb, Theme theme, string variant )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateBadgeStyles( StringBuilder sb, Theme theme, ThemeBadgeOptions options )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateBadgeVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateBarStyles( StringBuilder sb, Theme theme, ThemeBarOptions options )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateBorderVariantStyles( StringBuilder sb, Theme theme, string variant )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateBreadcrumbStyles( StringBuilder sb, Theme theme, ThemeBreadcrumbOptions options )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateButtonOutlineVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions options )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateButtonStyles( StringBuilder sb, Theme theme, ThemeButtonOptions options )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateButtonVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions options )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateCardStyles( StringBuilder sb, Theme theme, ThemeCardOptions options )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateDropdownStyles( StringBuilder sb, Theme theme, ThemeDropdownOptions options )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateInputStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateInputVariantStyles( StringBuilder sb, Theme theme, string variant, string color )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateListGroupItemStyles( StringBuilder sb, Theme theme, ThemeListGroupItemOptions options )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateListGroupItemVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inColor, ThemeListGroupItemOptions options )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateModalStyles( StringBuilder sb, Theme theme, ThemeModalOptions options )
            {
                throw new System.NotImplementedException();
            }

            protected override void GeneratePaginationStyles( StringBuilder sb, Theme theme, ThemePaginationOptions options )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateParagraphVariantStyles( StringBuilder sb, Theme theme, string variant, string color )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateRatingStyles( StringBuilder sb, Theme theme, ThemeRatingOptions options )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateRatingVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeRatingOptions ratingOptions )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateSpacingStyles( StringBuilder sb, Theme theme, ThemeSpacingOptions options )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateStepsStyles( StringBuilder sb, Theme theme, ThemeStepsOptions options )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateStepsVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeStepsOptions stepsOptions )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateSwitchVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeSwitchOptions switchOptions )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateTableVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inBorderColor )
            {
                throw new System.NotImplementedException();
            }

            protected override void GenerateTabsStyles( StringBuilder sb, Theme theme, ThemeTabsOptions options )
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
