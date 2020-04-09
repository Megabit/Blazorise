#region Using directives
using BasicTestApp.Client;
using Blazorise.E2ETests.Infrastructure;
using Blazorise.E2ETests.Infrastructure.ServerFixtures;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;
using DevHostServerProgram = BasicTestApp.Server.Program;
#endregion

namespace Blazorise.E2ETests
{
    public class SelectTest : BasicTestAppTestBase
    {
        public SelectTest( BrowserFixture browserFixture,
            ToggleExecutionModeServerFixture<DevHostServerProgram> serverFixture,
            ITestOutputHelper output )
            : base( browserFixture, serverFixture, output )
        {
            Navigate( ServerPathBase, noReload: !serverFixture.UsingAspNetHost );
            MountTestComponent<SelectComponent>();
        }

        [Fact]
        public void CanSelectString_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "select-string-initially-blank" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );
            var result = paragraph.FindElement( By.Id( "select-string-initially-blank-result" ) );

            WaitAssert.Equal( string.Empty, () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( string.Empty, () => result.Text );

            select.SelectByIndex( 1 );
            WaitAssert.Equal( "Oliver", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "Oliver", () => result.Text );

            select.SelectByValue( "Harry" );
            WaitAssert.Equal( "Harry", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "Harry", () => result.Text );

            select.SelectByIndex( 0 );
            WaitAssert.Equal( string.Empty, () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( string.Empty, () => result.Text );
        }

        [Fact]
        public void CanSelectString_InitiallySelected()
        {
            var paragraph = Browser.FindElement( By.Id( "select-string-initially-selected" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );
            var result = paragraph.FindElement( By.Id( "select-string-initially-selected-result" ) );

            WaitAssert.Equal( "Oliver", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "Oliver", () => result.Text );

            select.SelectByIndex( 0 );
            WaitAssert.Equal( string.Empty, () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( string.Empty, () => result.Text );

            select.SelectByValue( "Harry" );
            WaitAssert.Equal( "Harry", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "Harry", () => result.Text );
        }

        [Fact]
        public void CanSelectInt_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "select-int-initially-blank" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );
            var result = paragraph.FindElement( By.Id( "select-int-initially-blank-result" ) );

            WaitAssert.Equal( "0", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "0", () => result.Text );

            select.SelectByIndex( 1 );
            WaitAssert.Equal( "1", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "1", () => result.Text );

            select.SelectByValue( "2" );
            WaitAssert.Equal( "2", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "2", () => result.Text );

            select.SelectByValue( "0" );
            WaitAssert.Equal( "0", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "0", () => result.Text );
        }

        [Fact]
        public void CanSelectInt_InitiallySelected()
        {
            var paragraph = Browser.FindElement( By.Id( "select-int-initially-selected" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );
            var result = paragraph.FindElement( By.Id( "select-int-initially-selected-result" ) );

            WaitAssert.Equal( "1", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "1", () => result.Text );

            select.SelectByValue( "2" );
            WaitAssert.Equal( "2", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "2", () => result.Text );

            select.SelectByValue( "0" );
            WaitAssert.Equal( "0", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "0", () => result.Text );
        }

        [Fact]
        public void CanSelectNullableInt_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "select-nullable-int-initially-blank" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );
            var result = paragraph.FindElement( By.Id( "select-nullable-int-initially-blank-result" ) );

            WaitAssert.Equal( string.Empty, () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( string.Empty, () => result.Text );

            select.SelectByIndex( 1 );
            WaitAssert.Equal( "1", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "1", () => result.Text );

            select.SelectByValue( "2" );
            WaitAssert.Equal( "2", () => select.SelectedOption.GetAttribute( "value" ) );
            //WaitAssert.Equal( "2", () => result.Text );

            //select.SelectByValue( "" );
            //WaitAssert.Equal( "0", select.SelectedOption.GetAttribute( "value" ) );
            //WaitAssert.Equal( "0", result.Text );
        }

        [Fact]
        public void CanSelectNullableInt_InitiallySelected()
        {
            var paragraph = Browser.FindElement( By.Id( "select-nullable-int-initially-selected" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );
            var result = paragraph.FindElement( By.Id( "select-nullable-int-initially-selected-result" ) );

            WaitAssert.Equal( "1", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "1", () => result.Text );

            select.SelectByIndex( 2 );
            WaitAssert.Equal( "2", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "2", () => result.Text );

            select.SelectByValue( "3" );
            WaitAssert.Equal( "3", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "3", () => result.Text );
        }

        [Fact]
        public void CanSelectGuid_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "select-guid-initially-blank" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );
            var result = paragraph.FindElement( By.Id( "select-guid-initially-blank-result" ) );

            WaitAssert.Equal( "00000000-0000-0000-0000-000000000000", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "00000000-0000-0000-0000-000000000000", () => result.Text );

            select.SelectByIndex( 1 );
            WaitAssert.Equal( "413a7c18-b190-4f58-a967-338cd1566e97", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "413a7c18-b190-4f58-a967-338cd1566e97", () => result.Text );

            select.SelectByValue( "00cd0391-5e22-4729-855a-fec86267722c" );
            WaitAssert.Equal( "00cd0391-5e22-4729-855a-fec86267722c", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "00cd0391-5e22-4729-855a-fec86267722c", () => result.Text );

            select.SelectByValue( "00000000-0000-0000-0000-000000000000" );
            WaitAssert.Equal( "00000000-0000-0000-0000-000000000000", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "00000000-0000-0000-0000-000000000000", () => result.Text );
        }

        [Fact]
        public void CanSelectGuid_InitiallySelected()
        {
            var paragraph = Browser.FindElement( By.Id( "select-guid-initially-selected" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );
            var result = paragraph.FindElement( By.Id( "select-guid-initially-selected-result" ) );

            WaitAssert.Equal( "413a7c18-b190-4f58-a967-338cd1566e97", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "413a7c18-b190-4f58-a967-338cd1566e97", () => result.Text );

            select.SelectByValue( "00cd0391-5e22-4729-855a-fec86267722c" );
            WaitAssert.Equal( "00cd0391-5e22-4729-855a-fec86267722c", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "00cd0391-5e22-4729-855a-fec86267722c", () => result.Text );

            select.SelectByValue( "00000000-0000-0000-0000-000000000000" );
            WaitAssert.Equal( "00000000-0000-0000-0000-000000000000", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "00000000-0000-0000-0000-000000000000", () => result.Text );
        }

        [Fact]
        public void CanSelectNullableGuid_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "select-nullable-guid-initially-blank" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );
            var result = paragraph.FindElement( By.Id( "select-nullable-guid-initially-blank-result" ) );

            WaitAssert.Equal( string.Empty, () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( string.Empty, () => result.Text );

            select.SelectByIndex( 1 );
            WaitAssert.Equal( "413a7c18-b190-4f58-a967-338cd1566e97", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "413a7c18-b190-4f58-a967-338cd1566e97", () => result.Text );

            select.SelectByValue( "00cd0391-5e22-4729-855a-fec86267722c" );
            WaitAssert.Equal( "00cd0391-5e22-4729-855a-fec86267722c", () => select.SelectedOption.GetAttribute( "value" ) );
            //WaitAssert.Equal( "00cd0391-5e22-4729-855a-fec86267722c", () => result.Text );

            //select.SelectByValue( "" );
            //WaitAssert.Equal( "0", select.SelectedOption.GetAttribute( "value" ) );
            //WaitAssert.Equal( "0", result.Text );
        }

        [Fact]
        public void CanSelectNullableGuid_InitiallySelected()
        {
            var paragraph = Browser.FindElement( By.Id( "select-nullable-guid-initially-selected" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );
            var result = paragraph.FindElement( By.Id( "select-nullable-guid-initially-selected-result" ) );

            WaitAssert.Equal( "413a7c18-b190-4f58-a967-338cd1566e97", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "413a7c18-b190-4f58-a967-338cd1566e97", () => result.Text );

            select.SelectByIndex( 2 );
            WaitAssert.Equal( "00cd0391-5e22-4729-855a-fec86267722c", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "00cd0391-5e22-4729-855a-fec86267722c", () => result.Text );

            select.SelectByValue( "bca8ef46-abb7-4aec-b700-90b2b730a382" );
            WaitAssert.Equal( "bca8ef46-abb7-4aec-b700-90b2b730a382", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "bca8ef46-abb7-4aec-b700-90b2b730a382", () => result.Text );
        }

        [Fact]
        public void CanSelectBool_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "select-bool-initially-blank" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );
            var result = paragraph.FindElement( By.Id( "select-bool-initially-blank-result" ) );

            WaitAssert.Equal( "False", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "False", () => result.Text );

            select.SelectByIndex( 1 );
            WaitAssert.Equal( "False", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "False", () => result.Text );

            select.SelectByIndex( 0 );
            WaitAssert.Equal( "True", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "True", () => result.Text );

            select.SelectByValue( "False" );
            WaitAssert.Equal( "False", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "False", () => result.Text );

            select.SelectByValue( "True" );
            WaitAssert.Equal( "True", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "True", () => result.Text );
        }

        [Fact]
        public void CanSelectBool_InitiallySelected()
        {
            var paragraph = Browser.FindElement( By.Id( "select-bool-initially-selected" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );
            var result = paragraph.FindElement( By.Id( "select-bool-initially-selected-result" ) );

            WaitAssert.Equal( "True", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "True", () => result.Text );

            select.SelectByValue( "False" );
            WaitAssert.Equal( "False", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "False", () => result.Text );

            select.SelectByValue( "True" );
            WaitAssert.Equal( "True", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "True", () => result.Text );
        }

        [Fact]
        public void CanSelectNullableBool_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "select-nullable-bool-initially-blank" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );
            var result = paragraph.FindElement( By.Id( "select-nullable-bool-initially-blank-result" ) );

            WaitAssert.Equal( string.Empty, () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( string.Empty, () => result.Text );

            select.SelectByIndex( 1 );
            WaitAssert.Equal( "True", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "True", () => result.Text );

            select.SelectByValue( "False" );
            WaitAssert.Equal( "False", () => select.SelectedOption.GetAttribute( "value" ) );
        }

        [Fact]
        public void CanSelectNullableBool_InitiallySelected()
        {
            var paragraph = Browser.FindElement( By.Id( "select-nullable-bool-initially-selected" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );
            var result = paragraph.FindElement( By.Id( "select-nullable-bool-initially-selected-result" ) );

            WaitAssert.Equal( "True", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "True", () => result.Text );

            select.SelectByIndex( 2 );
            WaitAssert.Equal( "False", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "False", () => result.Text );

            select.SelectByValue( "True" );
            WaitAssert.Equal( "True", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "True", () => result.Text );
        }

        [Fact]
        public void CanSelectEnum_InitiallySelected()
        {
            var paragraph = Browser.FindElement( By.Id( "select-enum-initially-selected" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );
            var result = paragraph.FindElement( By.Id( "select-enum-initially-selected-result" ) );

            WaitAssert.Equal( "Mon", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "Mon", () => result.Text );

            select.SelectByIndex( 1 );
            WaitAssert.Equal( "Tue", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "Tue", () => result.Text );

            select.SelectByIndex( 2 );
            WaitAssert.Equal( "Wen", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "Wen", () => result.Text );

            select.SelectByIndex( 0 );
            WaitAssert.Equal( "Mon", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "Mon", () => result.Text );
        }

        [Fact]
        public void CanSelectNullableEnum_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "select-nullable-enum-initially-blank" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );
            var result = paragraph.FindElement( By.Id( "select-nullable-enum-initially-blank-result" ) );

            WaitAssert.Equal( string.Empty, () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( string.Empty, () => result.Text );

            select.SelectByIndex( 1 );
            WaitAssert.Equal( "Mon", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "Mon", () => result.Text );

            select.SelectByValue( "Fri" );
            WaitAssert.Equal( "Fri", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "Fri", () => result.Text );

            select.SelectByIndex( 0 );
            WaitAssert.Equal( string.Empty, () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( string.Empty, () => result.Text );
        }

        [Fact]
        public void CanSelectNullableEnum_InitiallySelected()
        {
            var paragraph = Browser.FindElement( By.Id( "select-nullable-enum-initially-selected" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );
            var result = paragraph.FindElement( By.Id( "select-nullable-enum-initially-selected-result" ) );

            WaitAssert.Equal( "Wen", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "Wen", () => result.Text );

            select.SelectByIndex( 0 );
            WaitAssert.Equal( string.Empty, () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( string.Empty, () => result.Text );

            select.SelectByValue( "Fri" );
            WaitAssert.Equal( "Fri", () => select.SelectedOption.GetAttribute( "value" ) );
            WaitAssert.Equal( "Fri", () => result.Text );
        }

        //[Fact]
        //public void CanSelectEnum_InitiallValue()
        //{
        //    var paragraphSelectString = Browser.FindElement( By.Id( "select-enum" ) );
        //    var selectString = new SelectElement( paragraphSelectString.FindElement( By.TagName( "select" ) ) );
        //    var selectStringResult = paragraphSelectString.FindElement( By.Id( "select-enum-result" ) );

        //    var selectedValue = selectString.SelectedOption.GetAttribute( "value" );

        //    WaitAssert.Equal( "Mon", selectedValue );
        //    WaitAssert.Equal( "Mon", selectStringResult.Text );
        //}

        //[Fact]
        //public void CanSelectEnum_ChangeValue()
        //{
        //    var paragraphSelectString = Browser.FindElement( By.Id( "select-enum" ) );
        //    var selectString = new SelectElement( paragraphSelectString.FindElement( By.TagName( "select" ) ) );
        //    var selectStringResult = paragraphSelectString.FindElement( By.Id( "select-enum-result" ) );

        //    selectString.SelectByIndex( 3 );
        //    WaitAssert.Equal( "Thu", () => selectStringResult.Text );
        //}

        //[Fact]
        //public void CanBindString()
        //{
        //    var paragraphSelectString = Browser.FindElement( By.Id( "select-bind-string" ) );
        //    var selectString = new SelectElement( paragraphSelectString.FindElement( By.TagName( "select" ) ) );
        //    var selectStringResult = paragraphSelectString.FindElement( By.Id( "select-bind-string-result" ) );
        //    var button = Browser.FindElement( By.TagName( "button" ) );

        //    var selectedValue = selectString.SelectedOption.GetAttribute( "value" );

        //    WaitAssert.Equal( "Harry", selectedValue );
        //    WaitAssert.Equal( "Harry", selectStringResult.Text );

        //    selectString.SelectByIndex( 2 );
        //    button.Click();

        //    selectedValue = selectString.SelectedOption.GetAttribute( "value" );

        //    WaitAssert.Equal( "Jack", () => selectedValue );
        //    WaitAssert.Equal( "Jack", () => selectStringResult.Text );

        //    selectString.SelectByIndex( 3 );

        //    selectedValue = selectString.SelectedOption.GetAttribute( "value" );

        //    WaitAssert.Equal( "George", () => selectedValue );
        //    WaitAssert.NotEqual( "George", () => selectStringResult.Text );
        //}
        //[Fact]
        //public void CanBindEnum()
        //{
        //    var paragraphSelectString = Browser.FindElement( By.Id( "select-bind-enum" ) );
        //    var selectString = new SelectElement( paragraphSelectString.FindElement( By.TagName( "select" ) ) );
        //    var selectStringResult = paragraphSelectString.FindElement( By.Id( "select-bind-enum-result" ) );
        //    var button = Browser.FindElement( By.TagName( "button" ) );

        //    var selectedValue = selectString.SelectedOption.GetAttribute( "value" );

        //    WaitAssert.Equal( "Tue", selectedValue );
        //    WaitAssert.Equal( "Tue", selectStringResult.Text );

        //    selectString.SelectByIndex( 4 );
        //    button.Click();

        //    selectedValue = selectString.SelectedOption.GetAttribute( "value" );

        //    WaitAssert.Equal( "Fri", () => selectedValue );
        //    WaitAssert.Equal( "Fri", () => selectStringResult.Text );
        //}
    }
}
