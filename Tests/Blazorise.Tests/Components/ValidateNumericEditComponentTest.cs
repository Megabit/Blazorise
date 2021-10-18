using System.Diagnostics;
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components
{
    public class ValidateNumericEditComponentTest : TestContext
    {
        public ValidateNumericEditComponentTest()
        {
            BlazoriseConfig.AddBootstrapProviders( Services );
            BlazoriseConfig.JSInterop.AddNumericEdit( this.JSInterop );
        }

        [Fact]
        public void CanValidateNumeric_InitiallyBlank()
        {
            // setup
            var comp = RenderComponent<ValidateNumericEditComponent>();
            var paragraph = comp.Find( "#validate-numeric-initially-blank" );
            var edit = comp.Find( "#validate-numeric-initially-blank input" );

            Debug.WriteLine( comp.Markup );
            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

            // test 1
            edit.Input( "1" );
            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

            // test 2
            edit.Input( string.Empty );
            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );
        }

        [Fact]
        public void CanValidateNumeric_InitiallyPopulated()
        {
            // setup
            var comp = RenderComponent<ValidateNumericEditComponent>();
            var paragraph = comp.Find( "#validate-numeric-initially-populated" );
            var edit = comp.Find( "#validate-numeric-initially-populated input" );

            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

            // test 1
            edit.Input( string.Empty );
            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

            // test 2
            edit.Input( "2" );
            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );
        }

        [Fact]
        public void CanValidateNumericWithBind_InitiallyBlank()
        {
            // setup
            var comp = RenderComponent<ValidateNumericEditComponent>();
            var paragraph = comp.Find( "#validate-numeric-with-bind-initially-blank" );
            var edit = comp.Find( "#validate-numeric-with-bind-initially-blank input" );

            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

            // test 1
            edit.Input( "1" );
            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

            // test 2
            edit.Input( string.Empty );
            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );
        }

        [Fact]
        public void CanValidateNumericWithBind_InitiallyPopulated()
        {
            // setup
            var comp = RenderComponent<ValidateNumericEditComponent>();
            var paragraph = comp.Find( "#validate-numeric-with-bind-initially-populated" );
            var edit = comp.Find( "#validate-numeric-with-bind-initially-populated input" );

            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

            // test 1
            edit.Input( string.Empty );
            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

            // test 2
            edit.Input( "2" );
            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );
        }

        [Fact]
        public void CanValidateNumericWithEvent_InitiallyBlank()
        {
            // setup
            var comp = RenderComponent<ValidateNumericEditComponent>();
            var paragraph = comp.Find( "#validate-numeric-with-event-initially-blank" );
            var edit = comp.Find( "#validate-numeric-with-event-initially-blank input" );

            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

            // test 1
            edit.Input( "1" );
            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

            // test 2
            edit.Input( string.Empty );
            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );
        }

        [Fact]
        public void CanValidateNumericWithEvent_InitiallyPopulated()
        {
            // setup
            var comp = RenderComponent<ValidateNumericEditComponent>();
            var paragraph = comp.Find( "#validate-numeric-with-event-initially-populated" );
            var edit = comp.Find( "#validate-numeric-with-event-initially-populated input" );

            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

            // test 1
            edit.Input( string.Empty );
            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

            // test 2
            edit.Input( "2" );
            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );
        }
    }
}
