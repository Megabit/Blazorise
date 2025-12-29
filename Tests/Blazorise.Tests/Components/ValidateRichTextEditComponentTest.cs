using System.Linq;
using System.Threading.Tasks;
using Blazorise.RichTextEdit;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;

public class ValidateRichTextEditComponentTest : TestContext
{
    public ValidateRichTextEditComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        Services.AddBlazoriseRichTextEdit();
        JSInterop.AddBlazoriseRichTextEdit();
    }

    [Fact]
    public async Task CanValidateRichTextEdit_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateRichTextEditComponent>();
        var edit = comp.Find( "#validate-richtext-initially-blank-editor" );
        var rte = comp.FindComponents<RichTextEdit>()
            .Single( component => component.Instance.ElementId == "validate-richtext-initially-blank-editor" )
            .Instance;

        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

        // test
        await comp.InvokeAsync( () => rte.OnContentChanged( "<p>a</p>", "a" ) );

        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateRichTextEdit_InitiallyPopulated()
    {
        // setup
        var comp = RenderComponent<ValidateRichTextEditComponent>();
        var edit = comp.Find( "#validate-richtext-initially-populated-editor" );
        var rte = comp.FindComponents<RichTextEdit>()
            .Single( component => component.Instance.ElementId == "validate-richtext-initially-populated-editor" )
            .Instance;

        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

        // test
        await comp.InvokeAsync( () => rte.OnContentChanged( "<p><br></p>", string.Empty ) );

        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );
    }
}