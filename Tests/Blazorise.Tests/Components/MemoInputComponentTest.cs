using AngleSharp.Dom;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blazorise.Tests.Components;

public class MemoInputComponentTest : BunitContext
{
    public MemoInputComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseMemoInput();
    }

    [Fact]
    public void AutoSize_RendersMarkerAndMinimumRowsStyle()
    {
        IRenderedComponent<MemoInput> component = Render<MemoInput>( parameters => parameters
            .Add( parameter => parameter.AutoSize, true )
            .Add( parameter => parameter.Rows, 4 ) );

        IElement textarea = component.Find( "textarea" );

        Assert.Equal( "true", textarea.GetAttribute( "data-autosize" ) );
        Assert.Equal( "4", textarea.GetAttribute( "rows" ) );
        Assert.Contains( "--textarea-min-block-size: 4lh", textarea.GetAttribute( "style" ) );
    }

    [Fact]
    public void AutoSize_UsesTwoRowsAsTheDefaultMinimum()
    {
        IRenderedComponent<MemoInput> component = Render<MemoInput>( parameters => parameters
            .Add( parameter => parameter.AutoSize, true ) );

        IElement textarea = component.Find( "textarea" );

        Assert.Contains( "--textarea-min-block-size: 2lh", textarea.GetAttribute( "style" ) );
    }

    [Fact]
    public void AutoSizeDisabled_OmitsMarkerAndMinimumRowsStyle()
    {
        IRenderedComponent<MemoInput> component = Render<MemoInput>();

        IElement textarea = component.Find( "textarea" );

        Assert.Null( textarea.GetAttribute( "data-autosize" ) );
        Assert.DoesNotContain( "textarea-min-block-size", textarea.GetAttribute( "style" ) ?? string.Empty );
    }

    [Fact]
    public void AutoSizeAndRows_CanChangeAtRuntime()
    {
        IRenderedComponent<MemoInput> component = Render<MemoInput>();

        component.Render( parameters => parameters
            .Add( parameter => parameter.AutoSize, true )
            .Add( parameter => parameter.Rows, 5 ) );

        IElement textarea = component.Find( "textarea" );

        Assert.Equal( "true", textarea.GetAttribute( "data-autosize" ) );
        Assert.Contains( "--textarea-min-block-size: 5lh", textarea.GetAttribute( "style" ) );

        component.Render( parameters => parameters
            .Add( parameter => parameter.AutoSize, false )
            .Add( parameter => parameter.Rows, 3 ) );

        textarea = component.Find( "textarea" );

        Assert.Null( textarea.GetAttribute( "data-autosize" ) );
        Assert.DoesNotContain( "textarea-min-block-size", textarea.GetAttribute( "style" ) ?? string.Empty );
    }
}