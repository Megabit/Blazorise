#region Using directives
using BasicTestApp.Client;
using Blazorise.UnitTests.Infrastructure;
using Blazorise.UnitTests.Infrastructure.ServerFixtures;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;
using DevHostServerProgram = BasicTestApp.Server.Program;
#endregion

namespace Blazorise.UnitTests
{
    public class SelectEditTest : BasicTestAppTestBase
    {
        public SelectEditTest( BrowserFixture browserFixture,
            ToggleExecutionModeServerFixture<DevHostServerProgram> serverFixture,
            ITestOutputHelper output )
            : base( browserFixture, serverFixture, output )
        {
            Navigate( ServerPathBase, noReload: !serverFixture.UsingAspNetHost );
            MountTestComponent<SelectEditComponent>();
        }

        [Fact]
        public void CanSelectString_InitiallValue()
        {
            var paragraphSelectString = Browser.FindElement( By.Id( "select-string" ) );
            var selectString = new SelectElement( paragraphSelectString.FindElement( By.TagName( "select" ) ) );
            var selectStringResult = paragraphSelectString.FindElement( By.Id( "select-string-result" ) );

            var selectedValue = selectString.SelectedOption.GetAttribute( "value" );

            Assert.Equal( "Oliver", selectedValue );
            Assert.Equal( "Oliver", selectStringResult.Text );
        }

        [Fact]
        public void CanSelectString_ChangeValue()
        {
            var paragraphSelectString = Browser.FindElement( By.Id( "select-string" ) );
            var selectString = new SelectElement( paragraphSelectString.FindElement( By.TagName( "select" ) ) );
            var selectStringResult = paragraphSelectString.FindElement( By.Id( "select-string-result" ) );

            selectString.SelectByIndex( 2 );
            WaitAssert.Equal( "Jack", () => selectStringResult.Text );
        }

        [Fact]
        public void CanSelectInt_InitiallValue()
        {
            var paragraphSelectString = Browser.FindElement( By.Id( "select-int" ) );
            var selectString = new SelectElement( paragraphSelectString.FindElement( By.TagName( "select" ) ) );
            var selectStringResult = paragraphSelectString.FindElement( By.Id( "select-int-result" ) );

            var selectedValue = selectString.SelectedOption.GetAttribute( "value" );

            Assert.Equal( "1", selectedValue );
            Assert.Equal( "1", selectStringResult.Text );
        }


        [Fact]
        public void CanSelectInt_ChangeValue()
        {
            var paragraphSelectString = Browser.FindElement( By.Id( "select-int" ) );
            var selectString = new SelectElement( paragraphSelectString.FindElement( By.TagName( "select" ) ) );
            var selectStringResult = paragraphSelectString.FindElement( By.Id( "select-int-result" ) );

            selectString.SelectByIndex( 3 );
            WaitAssert.Equal( "4", () => selectStringResult.Text );
        }

        [Fact]
        public void CanSelectEnum_InitiallValue()
        {
            var paragraphSelectString = Browser.FindElement( By.Id( "select-enum" ) );
            var selectString = new SelectElement( paragraphSelectString.FindElement( By.TagName( "select" ) ) );
            var selectStringResult = paragraphSelectString.FindElement( By.Id( "select-enum-result" ) );

            var selectedValue = selectString.SelectedOption.GetAttribute( "value" );

            Assert.Equal( "Mon", selectedValue );
            Assert.Equal( "Mon", selectStringResult.Text );
        }

        [Fact]
        public void CanSelectEnum_ChangeValue()
        {
            var paragraphSelectString = Browser.FindElement( By.Id( "select-enum" ) );
            var selectString = new SelectElement( paragraphSelectString.FindElement( By.TagName( "select" ) ) );
            var selectStringResult = paragraphSelectString.FindElement( By.Id( "select-enum-result" ) );

            selectString.SelectByIndex( 3 );
            WaitAssert.Equal( "Thu", () => selectStringResult.Text );
        }

        [Fact]
        public void CanBindString()
        {
            var paragraphSelectString = Browser.FindElement( By.Id( "select-bind-string" ) );
            var selectString = new SelectElement( paragraphSelectString.FindElement( By.TagName( "select" ) ) );
            var selectStringResult = paragraphSelectString.FindElement( By.Id( "select-bind-string-result" ) );
            var button = Browser.FindElement( By.TagName( "button" ) );

            var selectedValue = selectString.SelectedOption.GetAttribute( "value" );

            Assert.Equal( "Harry", selectedValue );
            Assert.Equal( "Harry", selectStringResult.Text );

            selectString.SelectByIndex( 2 );
            button.Click();

            selectedValue = selectString.SelectedOption.GetAttribute( "value" );

            WaitAssert.Equal( "Jack", () => selectedValue );
            WaitAssert.Equal( "Jack", () => selectStringResult.Text );

            selectString.SelectByIndex( 3 );

            selectedValue = selectString.SelectedOption.GetAttribute( "value" );

            WaitAssert.Equal( "George", () => selectedValue );
            WaitAssert.NotEqual( "George", () => selectStringResult.Text );
        }
        [Fact]
        public void CanBindEnum()
        {
            var paragraphSelectString = Browser.FindElement( By.Id( "select-bind-enum" ) );
            var selectString = new SelectElement( paragraphSelectString.FindElement( By.TagName( "select" ) ) );
            var selectStringResult = paragraphSelectString.FindElement( By.Id( "select-bind-enum-result" ) );
            var button = Browser.FindElement( By.TagName( "button" ) );

            var selectedValue = selectString.SelectedOption.GetAttribute( "value" );

            Assert.Equal( "Tue", selectedValue );
            Assert.Equal( "Tue", selectStringResult.Text );

            selectString.SelectByIndex( 4 );
            button.Click();

            selectedValue = selectString.SelectedOption.GetAttribute( "value" );

            WaitAssert.Equal( "Fri", () => selectedValue );
            WaitAssert.Equal( "Fri", () => selectStringResult.Text );
        }
    }
}
