#region Using directives
using Blazorise.Modules;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class NumericPickerComponentTest : TestContext
{
    public NumericPickerComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();

        var module = JSInterop.SetupModule( new JSNumericPickerModule( JSInterop.JSRuntime, new MockVersionProvider(), new( null, ( Options ) => { } ) ).ModuleFileName );
        module.SetupVoid( "import", _ => true ).SetVoidResult();
        module.SetupVoid( "initialize", _ => true ).SetVoidResult();
        module.SetupVoid( "destroy", _ => true ).SetVoidResult();
        module.SetupVoid( "updateValue", _ => true ).SetVoidResult();
    }

    [Fact]
    public void SetParametersAndRender_Should_SendRawValue_ToJsUpdateValue()
    {
        // setup
        var comp = RenderComponent<NumericPicker<decimal>>( parameters =>
            parameters.Add( x => x.Value, 10m ) );

        // test
        comp.SetParametersAndRender( parameters =>
            parameters.Add( x => x.Value, 99m ) );

        // validate
        comp.WaitForAssertion( () =>
        {
            var invocation = this.JSInterop.VerifyInvoke( "updateValue" );
            Assert.Equal( 99m, Assert.IsType<decimal>( invocation.Arguments[2] ) );
        }, TestExtensions.WaitTime );
    }
}