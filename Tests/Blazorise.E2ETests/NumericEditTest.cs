#region Using directives
using BasicTestApp.Client;
using Blazorise.E2ETests.Infrastructure;
using Blazorise.E2ETests.Infrastructure.ServerFixtures;
using OpenQA.Selenium;
using Xunit;
using Xunit.Abstractions;
using DevHostServerProgram = BasicTestApp.Server.Program;
#endregion

namespace Blazorise.E2ETests
{
    public class NumericEditTest : BasicTestAppTestBase
    {
        public NumericEditTest( BrowserFixture browserFixture,
            ToggleExecutionModeServerFixture<DevHostServerProgram> serverFixture,
            ITestOutputHelper output )
            : base( browserFixture, serverFixture, output )
        {
            Navigate( ServerPathBase, noReload: !serverFixture.UsingAspNetHost );
            MountTestComponent<NumericEditComponent>();
        }

        [Fact]
        public void CanChangeUndefinedIntegerUsingEvent()
        {
            var paragraph = Browser.FindElement( By.Id( "int-event-initially-undefined" ) );
            var numeric = paragraph.FindElement( By.TagName( "input" ) );
            var result = paragraph.FindElement( By.Id( "int-event-initially-undefined-result" ) );

            WaitAssert.Equal( "0", () => result.Text );

            numeric.SendKeysSequentially( "100" );
            WaitAssert.Equal( "100", () => result.Text );

            numeric.SendKeys( Keys.Backspace );
            WaitAssert.Equal( "10", () => result.Text );
        }

        [Fact]
        public void CanChangeNullableIntegerUsingEvent()
        {
            var paragraph = Browser.FindElement( By.Id( "nullable-int-event-initially-null" ) );
            var numeric = paragraph.FindElement( By.TagName( "input" ) );
            var result = paragraph.FindElement( By.Id( "nullable-int-event-initially-null-result" ) );

            WaitAssert.Equal( string.Empty, () => result.Text );

            numeric.SendKeysSequentially( "100" );
            WaitAssert.Equal( "100", () => result.Text );

            numeric.SendKeys( Keys.Backspace );
            WaitAssert.Equal( "10", () => result.Text );
        }

        [Fact]
        public void CanChangeUndefinedDecimalUsingEvent()
        {
            var paragraph = Browser.FindElement( By.Id( "decimal-event-initially-undefined" ) );
            var numeric = paragraph.FindElement( By.TagName( "input" ) );
            var result = paragraph.FindElement( By.Id( "decimal-event-initially-undefined-result" ) );

            WaitAssert.Equal( "0", () => result.Text );

            numeric.SendKeysSequentially( "100" );
            WaitAssert.Equal( "100", () => result.Text );

            numeric.SendKeys( Keys.Backspace );
            WaitAssert.Equal( "10", () => result.Text );
        }

        [Fact]
        public void CanChangeNullableDecimalUsingEvent()
        {
            var paragraph = Browser.FindElement( By.Id( "nullable-decimal-event-initially-null" ) );
            var numeric = paragraph.FindElement( By.TagName( "input" ) );
            var result = paragraph.FindElement( By.Id( "nullable-decimal-event-initially-null-result" ) );

            WaitAssert.Equal( string.Empty, () => result.Text );

            numeric.SendKeysSequentially( "100" );
            WaitAssert.Equal( "100", () => result.Text );

            numeric.SendKeys( Keys.Backspace );
            WaitAssert.Equal( "10", () => result.Text );
        }

        [Fact]
        public void CanChangeValueWithStepDefault()
        {
            var paragraph = Browser.FindElement( By.Id( "step-change-default" ) );
            var numeric = paragraph.FindElement( By.TagName( "input" ) );
            var result = paragraph.FindElement( By.Id( "step-change-default-result" ) );

            WaitAssert.Equal( "1", () => result.Text );

            numeric.SendKeys( Keys.Up );
            numeric.SendKeys( Keys.Up );
            WaitAssert.Equal( "3", () => result.Text );

            numeric.SendKeys( Keys.Down );
            WaitAssert.Equal( "2", () => result.Text );
        }

        [Fact]
        public void CanChangeValueWithStepBy2()
        {
            var paragraph = Browser.FindElement( By.Id( "step-change-by-2" ) );
            var numeric = paragraph.FindElement( By.TagName( "input" ) );
            var result = paragraph.FindElement( By.Id( "step-change-by-2-result" ) );

            WaitAssert.Equal( "2", () => result.Text );

            numeric.SendKeys( Keys.Up );
            numeric.SendKeys( Keys.Up );
            WaitAssert.Equal( "6", () => result.Text );

            numeric.SendKeys( Keys.Down );
            WaitAssert.Equal( "4", () => result.Text );
        }

        [Fact]
        public void CanTypeNumberWithDotDecimalSeparator()
        {
            var paragraph = Browser.FindElement( By.Id( "decimal-separator-with-dot" ) );
            var numeric = paragraph.FindElement( By.TagName( "input" ) );
            var result = paragraph.FindElement( By.Id( "decimal-separator-with-dot-result" ) );

            WaitAssert.Equal( "42.5", () => result.Text );

            numeric.SendKeys( "6" );
            WaitAssert.Equal( "42.56", () => result.Text );

            numeric.SendKeys( Keys.Backspace );
            numeric.SendKeys( Keys.Backspace );
            WaitAssert.Equal( "42", () => result.Text );

            numeric.SendKeys( ".3" );
            WaitAssert.Equal( "42.3", () => result.Text );
        }

        [Fact]
        public void CanTypeNumberWithCommaDecimalSeparator()
        {
            var paragraph = Browser.FindElement( By.Id( "decimal-separator-with-comma" ) );
            var numeric = paragraph.FindElement( By.TagName( "input" ) );
            var result = paragraph.FindElement( By.Id( "decimal-separator-with-comma-result" ) );

            WaitAssert.Equal( "42,5", () => result.Text );

            numeric.SendKeys( "6" );
            WaitAssert.Equal( "42,56", () => result.Text );

            numeric.SendKeys( Keys.Backspace );
            numeric.SendKeys( Keys.Backspace );
            WaitAssert.Equal( "42", () => result.Text );

            numeric.SendKeys( ",3" );
            WaitAssert.Equal( "42,3", () => result.Text );
        }

        [Fact]
        public void CanTypeMinMax()
        {
            var paragraph = Browser.FindElement( By.Id( "decimal-min-max-non-nullable" ) );
            var numeric = paragraph.FindElement( By.TagName( "input" ) );
            var result = paragraph.FindElement( By.Id( "decimal-min-max-non-nullable-result" ) );

            WaitAssert.Equal( "0", () => result.Text );

            numeric.SendKeys( "2" );
            WaitAssert.Equal( "2", () => result.Text );

            result.Click(); // leave input by clicking on other element so that onblur can happen
            WaitAssert.Equal( "10", () => result.Text );

            numeric.Clear();
            numeric.SendKeys( "15" );
            WaitAssert.Equal( "15", () => result.Text );
            result.Click();
            WaitAssert.Equal( "15", () => result.Text );

            numeric.Clear();
            numeric.SendKeys( "21" );
            WaitAssert.Equal( "21", () => result.Text );
            result.Click();
            WaitAssert.Equal( "20", () => result.Text );

            numeric.Clear();
            numeric.SendKeys( "0" );
            WaitAssert.Equal( "0", () => result.Text );
            result.Click();
            WaitAssert.Equal( "10", () => result.Text );
        }
    }
}
