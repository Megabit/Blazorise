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

    [Fact]
    public async Task Reports_radio_checked_parameter_rename()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazorise
{
    public class Radio : Microsoft.AspNetCore.Components.ComponentBase { }
}

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    public void Build( RenderTreeBuilder builder )
    {
        builder.OpenComponent<Blazorise.Radio>( 0 );
        builder.AddAttribute( 1, ""Checked"", true );
        builder.CloseComponent();
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        var diagnostic = Assert.Single( diagnostics );
        Assert.Equal( "BLZP001", diagnostic.Id );
        Assert.Equal( "Parameter 'Checked' was renamed to 'Value' for component 'Radio'", diagnostic.GetMessage() );
    }

    [Fact]
    public async Task Reports_dropdownlist_rightaligned_parameter_rename()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazorise
{
    public class DropdownList<TValue> : Microsoft.AspNetCore.Components.ComponentBase { }
}

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    public void Build( RenderTreeBuilder builder )
    {
        builder.OpenComponent<Blazorise.DropdownList<string>>( 0 );
        builder.AddAttribute( 1, ""RightAligned"", true );
        builder.CloseComponent();
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        var diagnostic = Assert.Single( diagnostics );
        Assert.Equal( "BLZP001", diagnostic.Id );
        Assert.Equal( "Parameter 'RightAligned' was renamed to 'EndAligned' for component 'DropdownList<string>'", diagnostic.GetMessage() );
    }

    [Fact]
    public async Task Reports_dropdown_rightaligned_parameter_rename()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazorise
{
    public class Dropdown : Microsoft.AspNetCore.Components.ComponentBase { }
}

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    public void Build( RenderTreeBuilder builder )
    {
        builder.OpenComponent<Blazorise.Dropdown>( 0 );
        builder.AddAttribute( 1, ""RightAligned"", true );
        builder.CloseComponent();
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        var diagnostic = Assert.Single( diagnostics );
        Assert.Equal( "BLZP001", diagnostic.Id );
        Assert.Equal( "Parameter 'RightAligned' was renamed to 'EndAligned' for component 'Dropdown'", diagnostic.GetMessage() );
    }

    [Fact]
    public async Task Reports_datagridcolumn_width_type_change()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazorise.DataGrid
{
    public class DataGridColumn<TValue> : Microsoft.AspNetCore.Components.ComponentBase { }
}

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    public void Build( RenderTreeBuilder builder )
    {
        builder.OpenComponent<Blazorise.DataGridColumn<int>>( 0 );
        builder.AddAttribute( 1, ""Width"", ""60px"" );
        builder.CloseComponent();
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        var diagnostic = Assert.Single( diagnostics );
        Assert.Equal( "BLZP002", diagnostic.Id );
        Assert.Equal( "Parameter 'Width' on component 'DataGridColumn<int>' has changed type: Width now expects IFluentSizing (e.g., Width.Px(60)).", diagnostic.GetMessage() );
    }

    [Fact]
    public async Task Reports_datagridcolumn_popupfieldcolumnsize_parameter_rename_like_razor_snippet()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazorise.DataGrid
{
    public class DataGridColumn<TItem> : Microsoft.AspNetCore.Components.ComponentBase { }
}

public class Employee { public string Email { get; set; } }

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    private static class GeneratedHelpers
    {
        public static void Create( RenderTreeBuilder builder )
        {
            builder.OpenComponent<Blazorise.DataGrid.DataGridColumn<Employee>>( 0 );
            builder.AddAttribute( 1, ""Field"", nameof( Employee.Email ) );
            builder.AddAttribute( 2, ""Caption"", ""Email"" );
            builder.AddAttribute( 3, ""Editable"", true );
            builder.AddComponentParameter( 4, ""PopupFieldColumnSize"", ""ColumnSize.IsFull.OnDesktop"" );
            builder.CloseComponent();
        }
    }

    public void Build( RenderTreeBuilder builder ) => GeneratedHelpers.Create( builder );
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        Assert.Contains( diagnostics, d => d.Id == "BLZP001" && d.GetMessage().Contains( "PopupFieldColumnSize" ) );
    }

    [Fact]
    public async Task Reports_row_gutter_tuple_type_change()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazorise
{
    public class Row : Microsoft.AspNetCore.Components.ComponentBase { }
}

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    public void Build( RenderTreeBuilder builder )
    {
        var gutter = ( 1, 2 );
        builder.OpenComponent<Blazorise.Row>( 0 );
        builder.AddAttribute( 1, ""Gutter"", gutter );
        builder.CloseComponent();
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        var diagnostic = Assert.Single( diagnostics );
        Assert.Equal( "BLZP002", diagnostic.Id );
        Assert.Equal( "Parameter 'Gutter' on component 'Row' has changed type: Gutter now expects IFluentGutter fluent API instead of tuple.", diagnostic.GetMessage() );
    }

    [Fact]
    public async Task Reports_blmouseeventargs_type_rename()
    {
        var source = @"
namespace Blazorise
{
    public class BLMouseEventArgs { }
}

public class MyComponent
{
    private Blazorise.BLMouseEventArgs args;
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        var diagnostic = Assert.Single( diagnostics );
        Assert.Equal( "BLZTYP001", diagnostic.Id );
        Assert.Equal( "Type 'Blazorise.BLMouseEventArgs' was renamed to 'Microsoft.AspNetCore.Components.Web.MouseEventArgs'", diagnostic.GetMessage() );
    }

    [Fact]
    public async Task Reports_datagridpagechangedeventargs_type_removed()
    {
        var source = @"
namespace Blazorise.DataGrid
{
    public class DataGridPageChangedEventArgs { }
}

public class MyComponent
{
    private Blazorise.DataGridPageChangedEventArgs args;
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        var diagnostic = Assert.Single( diagnostics );
        Assert.Equal( "BLZTYP002", diagnostic.Id );
        Assert.Equal( "Type 'Blazorise.DataGridPageChangedEventArgs' was removed in Blazorise 2.0", diagnostic.GetMessage() );
    }

    [Fact]
    public async Task Reports_dynamicreference_type_removed()
    {
        var source = @"
namespace Blazorise.RichTextEdit
{
    public record DynamicReference( string Url );
}

public class MyComponent
{
    private Blazorise.RichTextEdit.DynamicReference reference;
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        var diagnostic = Assert.Single( diagnostics );
        Assert.Equal( "BLZTYP002", diagnostic.Id );
        Assert.Equal( "Type 'Blazorise.RichTextEdit.DynamicReference' was removed in Blazorise 2.0", diagnostic.GetMessage() );
    }

    [Fact]
    public async Task Reports_richtexteditoptions_removed_members()
    {
        var source = @"
namespace Blazorise.RichTextEdit
{
    public class RichTextEditOptions
    {
        public string QuillJsVersion { get; set; }
        public bool DynamicallyLoadReferences { get; set; }
        public object DynamicReferences { get; set; }
    }
}

public class MyComponent
{
    public void Configure()
    {
        var options = new Blazorise.RichTextEdit.RichTextEditOptions();
        var v = options.QuillJsVersion;
        var d = options.DynamicallyLoadReferences;
        var r = options.DynamicReferences;
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        Assert.Equal( 3, diagnostics.Length );
        Assert.All( diagnostics, d => Assert.Equal( "BLZS002", d.Id ) );
    }

    [Fact]
    public async Task Reports_chartaxisgridline_member_renames()
    {
        var source = @"
using System.Collections.Generic;

namespace Blazorise.Charts
{
    public class ChartAxisGridLine
    {
        public bool DrawBorder { get; set; }
        public double BorderWidth { get; set; }
        public string BorderColor { get; set; }
        public List<int> BorderDash { get; set; }
        public double BorderDashOffset { get; set; }
    }
}

public class MyComponent
{
    public void Configure()
    {
        var grid = new Blazorise.Charts.ChartAxisGridLine();
        var a = grid.DrawBorder;
        var b = grid.BorderWidth;
        var c = grid.BorderColor;
        var d = grid.BorderDash;
        var e = grid.BorderDashOffset;
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        Assert.Equal( 5, diagnostics.Length );
        Assert.All( diagnostics, d => Assert.Equal( "BLZS001", d.Id ) );
    }

    [Fact]
    public async Task Reports_parameter_rename_when_using_extension_addcomponentparameter()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

namespace Microsoft.AspNetCore.Components.Rendering
{
    public static class RenderTreeBuilderExtensions
    {
        public static void AddComponentParameter<TValue>( this RenderTreeBuilder builder, int sequence, string name, TValue value ) { }
    }
}

namespace Blazorise.DataGrid
{
    public class DataGridColumn<TItem> : Microsoft.AspNetCore.Components.ComponentBase { }
}

public class Employee { public string Email { get; set; } }

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    public void Build( RenderTreeBuilder builder )
    {
        builder.OpenComponent<Blazorise.DataGrid.DataGridColumn<Employee>>( 0 );
        builder.AddComponentParameter( 1, \"PopupFieldColumnSize\", \"ColumnSize.IsFull.OnDesktop\" );
        builder.CloseComponent();
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        Assert.Contains( diagnostics, d => d.Id == "BLZP001" && d.GetMessage().Contains( "PopupFieldColumnSize" ) );
    }

    [Fact]
    public async Task Reports_parameter_rename_when_opencomponent_uses_typeof_overload()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

namespace Microsoft.AspNetCore.Components.Rendering
{
    public class RenderTreeBuilder
    {
        public void OpenComponent( int sequence, System.Type componentType ) { }
        public void CloseComponent() { }
        public void AddAttribute<TValue>(int sequence, string name, TValue value) { }
    }
}

namespace Blazorise.DataGrid
{
    public class DataGridColumn<TItem> : Microsoft.AspNetCore.Components.ComponentBase { }
}

public class Employee { public string Email { get; set; } }

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    public void Build( RenderTreeBuilder builder )
    {
        builder.OpenComponent( 0, typeof( Blazorise.DataGrid.DataGridColumn<Employee> ) );
        builder.AddAttribute( 1, \"PopupFieldColumnSize\", \"ColumnSize.IsFull.OnDesktop\" );
        builder.CloseComponent();
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        Assert.Contains( diagnostics, d => d.Id == \"BLZP001\" && d.GetMessage().Contains( \"PopupFieldColumnSize\" ) );
    }

    [Fact]
    public async Task Does_not_report_tvalueshape_when_selectionmode_unknown()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazorise
{
    public enum DateInputSelectionMode { Single = 0, Range = 1, Multiple = 2 }
}

namespace Blazorise
{
    public class DatePicker<TValue> : Microsoft.AspNetCore.Components.ComponentBase { }
}

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    private Blazorise.DateInputSelectionMode mode;
    private System.DateTime? value;

    public void Build( RenderTreeBuilder builder )
    {
        builder.OpenComponent<Blazorise.DatePicker<System.DateTime?>>( 0 );
        builder.AddAttribute( 1, \"SelectionMode\", mode ); // non-constant
        builder.AddAttribute( 2, \"Value\", value );
        builder.CloseComponent();
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        Assert.DoesNotContain( diagnostics, d => d.Id == \"BLZT001\" );
    }
}
