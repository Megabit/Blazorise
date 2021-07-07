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
    public class RadioGroupTest : BasicTestAppTestBase
    {
        public RadioGroupTest( BrowserFixture browserFixture,
               ToggleExecutionModeServerFixture<DevHostServerProgram> serverFixture,
               ITestOutputHelper output )
               : base( browserFixture, serverFixture, output )
        {
            Navigate( ServerPathBase, noReload: !serverFixture.UsingAspNetHost );
            MountTestComponent<RadioGroupComponent>();
        }

        [Fact]
        public void CanCheckString_InitiallyChecked()
        {
            var paragraph = Browser.FindElement( By.Id( "radiogroup-event-initially-selected" ) );
            var radioR = paragraph.FindElement( By.ClassName( "radioR" ) );
            var radioG = paragraph.FindElement( By.ClassName( "radioG" ) );
            var radioB = paragraph.FindElement( By.ClassName( "radioB" ) );
            var result = paragraph.FindElement( By.Id( "radiogroup-event-initially-selected-result" ) );

            WaitAssert.Equal( "true", () => radioG.GetAttribute( "checked" ) );
            WaitAssert.Equal( "green", () => result.Text );
            WaitAssert.Equal( null, () => radioR.GetAttribute( "checked" ) );
            WaitAssert.Equal( null, () => radioB.GetAttribute( "checked" ) );

            radioR.Click();
            WaitAssert.Equal( null, () => radioG.GetAttribute( "checked" ) );
            WaitAssert.Equal( "true", () => radioR.GetAttribute( "checked" ) );
            WaitAssert.Equal( "red", () => result.Text );

            radioB.Click();
            WaitAssert.Equal( null, () => radioR.GetAttribute( "checked" ) );
            WaitAssert.Equal( "true", () => radioB.GetAttribute( "checked" ) );
            WaitAssert.Equal( "blue", () => result.Text );
        }
    }
}
