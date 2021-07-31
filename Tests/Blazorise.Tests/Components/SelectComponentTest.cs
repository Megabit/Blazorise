﻿using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components
{
    public class SelectComponentTest : TestContext
    {
        public SelectComponentTest()
        {
            BlazoriseConfig.AddBootstrapProviders( Services );
        }

        [Fact]
        public void CanSelectString_InitiallyBlank()
        {
            // setup
            var comp = RenderComponent<SelectComponent>();
            var paragraph = comp.Find( "#select-string-initially-blank" );
            var select = comp.Find( "#select-string-initially-blank select" );
            var result = comp.Find( "#select-string-initially-blank-result" );

            Assert.Equal( string.Empty, select.GetAttribute( "value" ) );
            Assert.Equal( string.Empty, result.InnerHtml );

            // test 1
            select.Change( "Oliver" );
            Assert.Equal( "Oliver", select.GetAttribute( "value" ) );
            Assert.Equal( "Oliver", result.InnerHtml );

            // test 2
            select.Change( "Harry" );
            Assert.Equal( "Harry", select.GetAttribute( "value" ) );
            Assert.Equal( "Harry", result.InnerHtml );

            // test 3
            select.Change( string.Empty );
            Assert.Equal( string.Empty, select.GetAttribute( "value" ) );
            Assert.Equal( string.Empty, result.InnerHtml );
        }

        [Fact]
        public void CanSelectString_InitiallySelected()
        {
            // setup
            var comp = RenderComponent<SelectComponent>();
            var paragraph = comp.Find( "#select-string-initially-selected" );
            var select = comp.Find( "#select-string-initially-selected select" );
            var result = comp.Find( "#select-string-initially-selected-result" );

            Assert.Equal( "Oliver", select.GetAttribute( "value" ) );
            Assert.Equal( "Oliver", result.InnerHtml );

            // test 1
            select.Change( string.Empty );
            Assert.Equal( string.Empty, select.GetAttribute( "value" ) );
            Assert.Equal( string.Empty, result.InnerHtml );

            // test 2
            select.Change( "Harry" );
            Assert.Equal( "Harry", select.GetAttribute( "value" ) );
            Assert.Equal( "Harry", result.InnerHtml );
        }

        [Fact]
        public void CanSelectInt_InitiallyBlank()
        {
            // setup
            var comp = RenderComponent<SelectComponent>();
            var paragraph = comp.Find( "#select-int-initially-blank" );
            var select = comp.Find( "#select-int-initially-blank select" );
            var result = comp.Find( "#select-int-initially-blank-result" );

            Assert.Equal( "0", select.GetAttribute( "value" ) );
            Assert.Equal( "0", result.InnerHtml );

            // test 1
            select.Change( "1" );
            Assert.Equal( "1", select.GetAttribute( "value" ) );
            Assert.Equal( "1", result.InnerHtml );

            // test 2
            select.Change( "2" );
            Assert.Equal( "2", select.GetAttribute( "value" ) );
            Assert.Equal( "2", result.InnerHtml );

            // test 3
            select.Change( "0" );
            Assert.Equal( "0", select.GetAttribute( "value" ) );
            Assert.Equal( "0", result.InnerHtml );
        }

        [Fact]
        public void CanSelectInt_InitiallySelected()
        {
            // setup
            var comp = RenderComponent<SelectComponent>();
            var paragraph = comp.Find( "#select-int-initially-selected" );
            var select = comp.Find( "#select-int-initially-selected select" );
            var result = comp.Find( "#select-int-initially-selected-result" );

            Assert.Equal( "1", select.GetAttribute( "value" ) );
            Assert.Equal( "1", result.InnerHtml );

            // test 1
            select.Change( "2" );
            Assert.Equal( "2", select.GetAttribute( "value" ) );
            Assert.Equal( "2", result.InnerHtml );

            // test 2
            select.Change( "0" );
            Assert.Equal( "0", select.GetAttribute( "value" ) );
            Assert.Equal( "0", result.InnerHtml );
        }

        [Fact]
        public void CanSelectNullableInt_InitiallyBlank()
        {
            // setup
            var comp = RenderComponent<SelectComponent>();
            var paragraph = comp.Find( "#select-nullable-int-initially-blank" );
            var select = comp.Find( "#select-nullable-int-initially-blank select" );
            var result = comp.Find( "#select-nullable-int-initially-blank-result" );

            Assert.Equal( string.Empty, select.GetAttribute( "value" ) );
            Assert.Equal( string.Empty, result.InnerHtml );

            // test 1
            select.Change( "1" );
            Assert.Equal( "1", select.GetAttribute( "value" ) );
            Assert.Equal( "1", result.InnerHtml );

            // test 2
            select.Change( "2" );
            Assert.Equal( "2", select.GetAttribute( "value" ) );
            Assert.Equal( "2", result.InnerHtml );

            // test 3
            select.Change( "0" );
            Assert.Equal( "0", select.GetAttribute( "value" ) );
            Assert.Equal( "0", result.InnerHtml );
        }

        [Fact]
        public void CanSelectNullableInt_InitiallySelected()
        {
            // setup
            var comp = RenderComponent<SelectComponent>();
            var paragraph = comp.Find( "#select-nullable-int-initially-selected" );
            var select = comp.Find( "#select-nullable-int-initially-selected select" );
            var result = comp.Find( "#select-nullable-int-initially-selected-result" );

            Assert.Equal( "1", select.GetAttribute( "value" ) );
            Assert.Equal( "1", result.InnerHtml );

            // test 1
            select.Change( "2" );
            Assert.Equal( "2", select.GetAttribute( "value" ) );
            Assert.Equal( "2", result.InnerHtml );

            // test 2
            select.Change( "3" );
            Assert.Equal( "3", select.GetAttribute( "value" ) );
            Assert.Equal( "3", result.InnerHtml );
        }

        [Fact]
        public void CanSelectGuid_InitiallyBlank()
        {
            // setup
            var comp = RenderComponent<SelectComponent>();
            var paragraph = comp.Find( "#select-guid-initially-blank" );
            var select = comp.Find( "#select-guid-initially-blank select" );
            var result = comp.Find( "#select-guid-initially-blank-result" );

            Assert.Equal( "00000000-0000-0000-0000-000000000000", select.GetAttribute( "value" ) );
            Assert.Equal( "00000000-0000-0000-0000-000000000000", result.InnerHtml );

            // test 1
            select.Change( "413a7c18-b190-4f58-a967-338cd1566e97" );
            Assert.Equal( "413a7c18-b190-4f58-a967-338cd1566e97", select.GetAttribute( "value" ) );
            Assert.Equal( "413a7c18-b190-4f58-a967-338cd1566e97", result.InnerHtml );

            // test 2
            select.Change( "00cd0391-5e22-4729-855a-fec86267722c" );
            Assert.Equal( "00cd0391-5e22-4729-855a-fec86267722c", select.GetAttribute( "value" ) );
            Assert.Equal( "00cd0391-5e22-4729-855a-fec86267722c", result.InnerHtml );

            // test 3
            select.Change( "00000000-0000-0000-0000-000000000000" );
            Assert.Equal( "00000000-0000-0000-0000-000000000000", select.GetAttribute( "value" ) );
            Assert.Equal( "00000000-0000-0000-0000-000000000000", result.InnerHtml );
        }

        [Fact]
        public void CanSelectGuid_InitiallySelected()
        {
            // setup
            var comp = RenderComponent<SelectComponent>();
            var paragraph = comp.Find( "#select-guid-initially-selected" );
            var select = comp.Find( "#select-guid-initially-selected select" );
            var result = comp.Find( "#select-guid-initially-selected-result" );

            Assert.Equal( "413a7c18-b190-4f58-a967-338cd1566e97", select.GetAttribute( "value" ) );
            Assert.Equal( "413a7c18-b190-4f58-a967-338cd1566e97", result.InnerHtml );

            // test 2
            select.Change( "00cd0391-5e22-4729-855a-fec86267722c" );
            Assert.Equal( "00cd0391-5e22-4729-855a-fec86267722c", select.GetAttribute( "value" ) );
            Assert.Equal( "00cd0391-5e22-4729-855a-fec86267722c", result.InnerHtml );

            // test 3
            select.Change( "00000000-0000-0000-0000-000000000000" );
            Assert.Equal( "00000000-0000-0000-0000-000000000000", select.GetAttribute( "value" ) );
            Assert.Equal( "00000000-0000-0000-0000-000000000000", result.InnerHtml );
        }

        [Fact]
        public void CanSelectNullableGuid_InitiallyBlank()
        {
            // setup
            var comp = RenderComponent<SelectComponent>();
            var paragraph = comp.Find( "#select-nullable-guid-initially-blank" );
            var select = comp.Find( "#select-nullable-guid-initially-blank select" );
            var result = comp.Find( "#select-nullable-guid-initially-blank-result" );

            Assert.Equal( string.Empty, select.GetAttribute( "value" ) );
            Assert.Equal( string.Empty, result.InnerHtml );

            // test 1
            select.Change( "413a7c18-b190-4f58-a967-338cd1566e97" );
            Assert.Equal( "413a7c18-b190-4f58-a967-338cd1566e97", select.GetAttribute( "value" ) );
            Assert.Equal( "413a7c18-b190-4f58-a967-338cd1566e97", result.InnerHtml );

            // test 2
            select.Change( "00cd0391-5e22-4729-855a-fec86267722c" );
            Assert.Equal( "00cd0391-5e22-4729-855a-fec86267722c", select.GetAttribute( "value" ) );
            Assert.Equal( "00cd0391-5e22-4729-855a-fec86267722c", result.InnerHtml );

            // test 3
            select.Change( null );
            Assert.Equal( string.Empty, select.GetAttribute( "value" ) );
            Assert.Equal( string.Empty, result.InnerHtml );
        }

        [Fact]
        public void CanSelectNullableGuid_InitiallySelected()
        {
            // setup
            var comp = RenderComponent<SelectComponent>();
            var paragraph = comp.Find( "#select-nullable-guid-initially-selected" );
            var select = comp.Find( "#select-nullable-guid-initially-selected select" );
            var result = comp.Find( "#select-nullable-guid-initially-selected-result" );

            Assert.Equal( "413a7c18-b190-4f58-a967-338cd1566e97", select.GetAttribute( "value" ) );
            Assert.Equal( "413a7c18-b190-4f58-a967-338cd1566e97", result.InnerHtml );

            // test 1
            select.Change( "00cd0391-5e22-4729-855a-fec86267722c" );
            Assert.Equal( "00cd0391-5e22-4729-855a-fec86267722c", select.GetAttribute( "value" ) );
            Assert.Equal( "00cd0391-5e22-4729-855a-fec86267722c", result.InnerHtml );

            // test 2
            select.Change( "bca8ef46-abb7-4aec-b700-90b2b730a382" );
            Assert.Equal( "bca8ef46-abb7-4aec-b700-90b2b730a382", select.GetAttribute( "value" ) );
            Assert.Equal( "bca8ef46-abb7-4aec-b700-90b2b730a382", result.InnerHtml );

            // test 3
            select.Change( null );
            Assert.Equal( string.Empty, select.GetAttribute( "value" ) );
            Assert.Equal( string.Empty, result.InnerHtml );
        }

        [Fact]
        public void CanSelectBool_InitiallyBlank()
        {
            // setup
            var comp = RenderComponent<SelectComponent>();
            var paragraph = comp.Find( "#select-bool-initially-blank" );
            var select = comp.Find( "#select-bool-initially-blank select" );
            var result = comp.Find( "#select-bool-initially-blank-result" );

            Assert.Equal( "False", select.GetAttribute( "value" ) );
            Assert.Equal( "False", result.InnerHtml );

            // test 1
            select.Change( "False" );
            Assert.Equal( "False", select.GetAttribute( "value" ) );
            Assert.Equal( "False", result.InnerHtml );

            // test 2
            select.Change( "True" );
            Assert.Equal( "True", select.GetAttribute( "value" ) );
            Assert.Equal( "True", result.InnerHtml );
        }

        [Fact]
        public void CanSelectBool_InitiallySelected()
        {
            // setup
            var comp = RenderComponent<SelectComponent>();
            var paragraph = comp.Find( "#select-bool-initially-selected" );
            var select = comp.Find( "#select-bool-initially-selected select" );
            var result = comp.Find( "#select-bool-initially-selected-result" );

            Assert.Equal( "True", select.GetAttribute( "value" ) );
            Assert.Equal( "True", result.InnerHtml );

            // test 1
            select.Change( "False" );
            Assert.Equal( "False", select.GetAttribute( "value" ) );
            Assert.Equal( "False", result.InnerHtml );

            // test 2
            select.Change( "True" );
            Assert.Equal( "True", select.GetAttribute( "value" ) );
            Assert.Equal( "True", result.InnerHtml );
        }

        [Fact]
        public void CanSelectNullableBool_InitiallyBlank()
        {
            // setup
            var comp = RenderComponent<SelectComponent>();
            var paragraph = comp.Find( "#select-nullable-bool-initially-blank" );
            var select = comp.Find( "#select-nullable-bool-initially-blank select" );
            var result = comp.Find( "#select-nullable-bool-initially-blank-result" );

            Assert.Equal( string.Empty, select.GetAttribute( "value" ) );
            Assert.Equal( string.Empty, result.InnerHtml );

            // test 1
            select.Change( "True" );
            Assert.Equal( "True", select.GetAttribute( "value" ) );
            Assert.Equal( "True", result.InnerHtml );

            // test 2
            select.Change( "False" );
            Assert.Equal( "False", select.GetAttribute( "value" ) );
            Assert.Equal( "False", result.InnerHtml );
        }

        [Fact]
        public void CanSelectNullableBool_InitiallySelected()
        {
            // setup
            var comp = RenderComponent<SelectComponent>();
            var paragraph = comp.Find( "#select-nullable-bool-initially-selected" );
            var select = comp.Find( "#select-nullable-bool-initially-selected select" );
            var result = comp.Find( "#select-nullable-bool-initially-selected-result" );

            Assert.Equal( "True", select.GetAttribute( "value" ) );
            Assert.Equal( "True", result.InnerHtml );

            // test 1
            select.Change( "False" );
            Assert.Equal( "False", select.GetAttribute( "value" ) );
            Assert.Equal( "False", result.InnerHtml );

            // test 2
            select.Change( "True" );
            Assert.Equal( "True", select.GetAttribute( "value" ) );
            Assert.Equal( "True", result.InnerHtml );
        }

        [Fact]
        public void CanSelectEnum_InitiallySelected()
        {
            // setup
            var comp = RenderComponent<SelectComponent>();
            var paragraph = comp.Find( "#select-enum-initially-selected" );
            var select = comp.Find( "#select-enum-initially-selected select" );
            var result = comp.Find( "#select-enum-initially-selected-result" );

            Assert.Equal( "Mon", select.GetAttribute( "value" ) );
            Assert.Equal( "Mon", result.InnerHtml );

            // test 1
            select.Change( "Tue" );
            Assert.Equal( "Tue", select.GetAttribute( "value" ) );
            Assert.Equal( "Tue", result.InnerHtml );

            // test 2
            select.Change( "Wen" );
            Assert.Equal( "Wen", select.GetAttribute( "value" ) );
            Assert.Equal( "Wen", result.InnerHtml );

            // test 2
            select.Change( "Mon" );
            Assert.Equal( "Mon", select.GetAttribute( "value" ) );
            Assert.Equal( "Mon", result.InnerHtml );
        }

        [Fact]
        public void CanSelectNullableEnum_InitiallyBlank()
        {
            // setup
            var comp = RenderComponent<SelectComponent>();
            var paragraph = comp.Find( "#select-nullable-enum-initially-blank" );
            var select = comp.Find( "#select-nullable-enum-initially-blank select" );
            var result = comp.Find( "#select-nullable-enum-initially-blank-result" );

            Assert.Equal( string.Empty, select.GetAttribute( "value" ) );
            Assert.Equal( string.Empty, result.InnerHtml );

            // test 1
            select.Change( "Mon" );
            Assert.Equal( "Mon", select.GetAttribute( "value" ) );
            Assert.Equal( "Mon", result.InnerHtml );

            // test 2
            select.Change( "Fri" );
            Assert.Equal( "Fri", select.GetAttribute( "value" ) );
            Assert.Equal( "Fri", result.InnerHtml );

            // test 3
            select.Change( null );
            Assert.Equal( string.Empty, select.GetAttribute( "value" ) );
            Assert.Equal( string.Empty, result.InnerHtml );
        }

        [Fact]
        public void CanSelectNullableEnum_InitiallySelected()
        {
            // setup
            var comp = RenderComponent<SelectComponent>();
            var paragraph = comp.Find( "#select-nullable-enum-initially-selected" );
            var select = comp.Find( "#select-nullable-enum-initially-selected select" );
            var result = comp.Find( "#select-nullable-enum-initially-selected-result" );

            Assert.Equal( "Wen", select.GetAttribute( "value" ) );
            Assert.Equal( "Wen", result.InnerHtml );

            // test 1
            select.Change( "Mon" );
            Assert.Equal( "Mon", select.GetAttribute( "value" ) );
            Assert.Equal( "Mon", result.InnerHtml );

            // test 2
            select.Change( null );
            Assert.Equal( string.Empty, select.GetAttribute( "value" ) );
            Assert.Equal( string.Empty, result.InnerHtml );

            // test 3
            select.Change( "Fri" );
            Assert.Equal( "Fri", select.GetAttribute( "value" ) );
            Assert.Equal( "Fri", result.InnerHtml );
        }
    }
}
