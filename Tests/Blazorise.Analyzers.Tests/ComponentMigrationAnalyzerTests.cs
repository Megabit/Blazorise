using System.Threading.Tasks;
using Xunit;

namespace Blazorise.Analyzers.Tests;

public class ComponentMigrationAnalyzerTests
{
    [Fact]
    public async Task Reports_component_rename()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazorise
{
    public class ColorEdit<TValue> : Microsoft.AspNetCore.Components.ComponentBase { }
    public class ColorInput<TValue> : Microsoft.AspNetCore.Components.ComponentBase { }
}

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    public void Build( RenderTreeBuilder builder )
    {
        builder.OpenComponent<Blazorise.ColorEdit<string>>( 0 );
        builder.CloseComponent();
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        var diagnostic = Assert.Single( diagnostics );
        Assert.Equal( "BLZC001", diagnostic.Id );
        Assert.Equal( "Component 'Blazorise.ColorEdit`1' was renamed to 'Blazorise.ColorInput`1'", diagnostic.GetMessage() );
    }

    [Fact]
    public async Task Reports_parameter_rename()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazorise
{
    public class Check<TValue> : Microsoft.AspNetCore.Components.ComponentBase { }
}

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    private bool value;

    public void Build( RenderTreeBuilder builder )
    {
        builder.OpenComponent<Blazorise.Check<bool>>( 0 );
        builder.AddAttribute( 1, ""Checked"", value );
        builder.CloseComponent();
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        var diagnostic = Assert.Single( diagnostics );
        Assert.Equal( "BLZP001", diagnostic.Id );
        Assert.Equal( "Parameter 'Checked' was renamed to 'Value' for component 'Check<bool>'", diagnostic.GetMessage() );
    }

    [Fact]
    public async Task Reports_multi_value_used_for_single_shape()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazorise
{
    public class NumericInput<TValue> : Microsoft.AspNetCore.Components.ComponentBase { }
}

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    public void Build( RenderTreeBuilder builder )
    {
        var values = new int[] { 1, 2, 3 };
        builder.OpenComponent<Blazorise.NumericInput<int>>( 0 );
        builder.AddAttribute( 1, ""Value"", values );
        builder.CloseComponent();
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        var diagnostic = Assert.Single( diagnostics );
        Assert.Equal( "BLZT001", diagnostic.Id );
        Assert.Equal( "Component 'NumericInput<int>' expects TValue shape 'Single', but value expression is of type 'int[]'", diagnostic.GetMessage() );
    }

    [Fact]
    public async Task Reports_single_value_used_for_multi_selection()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazorise
{
    public class Select<TValue> : Microsoft.AspNetCore.Components.ComponentBase { }
}

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    public void Build( RenderTreeBuilder builder )
    {
        builder.OpenComponent<Blazorise.Select<string>>( 0 );
        builder.AddAttribute( 1, ""Multiple"", true );
        builder.AddAttribute( 2, ""Value"", ""single"" );
        builder.CloseComponent();
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        var diagnostic = Assert.Single( diagnostics );
        Assert.Equal( "BLZT001", diagnostic.Id );
        Assert.Equal( "Component 'Select<string>' expects TValue shape 'SingleOrMultiListOrArray', but value expression is of type 'string'", diagnostic.GetMessage() );
    }
}