using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components
{
    public class ValidateTextEditComponentTest : TestContext
    {
        public ValidateTextEditComponentTest()
        {
            BlazoriseConfig.AddBootstrapProviders( Services );
        }

        [Fact]
        public void CanValidateText_InitiallyBlank()
        {
            // setup
            var comp = RenderComponent<ValidateTextEditComponent>();
            var paragraph = comp.Find( "#validate-text-initially-blank" );
            var edit = comp.Find( "#validate-text-initially-blank input" );

            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

            // test 1
            edit.Input( "a" );
            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

            // test 2
            edit.Input( string.Empty );
            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );
        }

        [Fact]
        public void CanValidateText_InitiallyPopulated()
        {
            // setup
            var comp = RenderComponent<ValidateTextEditComponent>();
            var paragraph = comp.Find( "#validate-text-initially-populated" );
            var edit = comp.Find( "#validate-text-initially-populated input" );

            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

            // test 1
            edit.Input( string.Empty );
            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

            // test 2
            edit.Input( "b" );
            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );
        }

        [Fact]
        public void CanValidateTextWithBind_InitiallyBlank()
        {
            // setup
            var comp = RenderComponent<ValidateTextEditComponent>();
            var paragraph = comp.Find( "#validate-text-with-bind-initially-blank" );
            var edit = comp.Find( "#validate-text-with-bind-initially-blank input" );

            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

            // test 1
            edit.Input( "a" );
            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

            // test 2
            edit.Input( string.Empty );
            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );
        }

        [Fact]
        public void CanValidateTextWithBind_InitiallyPopulated()
        {
            // setup
            var comp = RenderComponent<ValidateTextEditComponent>();
            var paragraph = comp.Find( "#validate-text-with-bind-initially-populated" );
            var edit = comp.Find( "#validate-text-with-bind-initially-populated input" );

            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

            // test 1
            edit.Input( string.Empty );
            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

            // test 2
            edit.Input( "b" );
            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );
        }

        [Fact]
        public void CanValidateTextWithEvent_InitiallyBlank()
        {
            // setup
            var comp = RenderComponent<ValidateTextEditComponent>();
            var paragraph = comp.Find( "#validate-text-with-event-initially-blank" );
            var edit = comp.Find( "#validate-text-with-event-initially-blank input" );

            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

            // test 1
            edit.Input( "a" );
            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

            // test 2
            edit.Input( string.Empty );
            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );
        }

        [Fact]
        public void CanValidateTextWithEvent_InitiallyPopulated()
        {
            // setup
            var comp = RenderComponent<ValidateTextEditComponent>();
            var paragraph = comp.Find( "#validate-text-with-event-initially-populated" );
            var edit = comp.Find( "#validate-text-with-event-initially-populated input" );

            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

            // test 1
            edit.Input( string.Empty );
            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

            // test 2
            edit.Input( "b" );
            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );
        }

        [Fact]
        public void CanValidatePattern_InitiallyBlank()
        {
            // setup
            var comp = RenderComponent<ValidateTextEditComponent>();
            var paragraph = comp.Find( "#validate-text-using-pattern-initially-blank" );
            var edit = comp.Find( "#validate-text-using-pattern-initially-blank input" );

            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

            // test 1
            edit.Input( "a" );
            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

            // test 2
            edit.Input( "1" );
            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

            // test 3
            edit.Input( string.Empty );
            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );
        }

        [Fact]
        public void CanValidatePattern_InitiallyPopulated()
        {
            // setup
            var comp = RenderComponent<ValidateTextEditComponent>();
            var paragraph = comp.Find( "#validate-text-using-pattern-initially-populated" );
            var edit = comp.Find( "#validate-text-using-pattern-initially-populated input" );

            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

            // test 1
            edit.Input( string.Empty );
            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

            // test 2
            edit.Input( "b" );
            Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

            // test 3
            edit.Input( "2" );
            Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );
        }
    }
}
