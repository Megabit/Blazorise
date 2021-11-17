//#region Using directives
//using BasicTestApp.Client;
//using Blazorise.E2ETests.Infrastructure;
//using Blazorise.E2ETests.Infrastructure.ServerFixtures;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Support.UI;
//using Xunit;
//using Xunit.Abstractions;
//using DevHostServerProgram = BasicTestApp.Server.Program;
//#endregion

//namespace Blazorise.E2ETests
//{
//    public class DateEditTest : BasicTestAppTestBase
//    {
//        public DateEditTest( BrowserFixture browserFixture,
//            ToggleExecutionModeServerFixture<DevHostServerProgram> serverFixture,
//            ITestOutputHelper output )
//            : base( browserFixture, serverFixture, output )
//        {
//            Navigate( ServerPathBase, noReload: !serverFixture.UsingAspNetHost );
//            MountTestComponent<DateEditComponent>();
//        }

//        [Fact]
//        public void CanChangeUndefinedIntegerUsingEvent()
//        {
//            var paragraph = Browser.FindElement( By.Id( "date-event-initially-undefined" ) );
//            var date = paragraph.FindElement( By.TagName( "input" ) );
//            var result = paragraph.FindElement( By.Id( "date-event-initially-undefined-result" ) );

//            WaitAssert.Equal( "0001-01-01", () => result.Text );

//            date.SendKeysSequentially( "662021" );
//            WaitAssert.Equal( "2021-06-06", () => result.Text );

//            date.SendKeys( Keys.Backspace );
//            WaitAssert.Equal( "0001-01-01", () => result.Text );
//        }
//    }
//}
