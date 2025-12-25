using System.Linq;
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
    public async Task Reports_tag_rename_for_removed_components()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    public void Build( RenderTreeBuilder builder )
    {
        builder.OpenElement( 0, ""TextEdit"" );
        builder.AddAttribute( 1, ""Text"", ""test"" );
        builder.CloseElement();
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        var diagnostic = Assert.Single( diagnostics );
        Assert.Equal( "BLZR001", diagnostic.Id );
        Assert.Equal( "Tag 'TextEdit' was renamed to 'TextInput'", diagnostic.GetMessage() );
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
    public async Task Reports_autocomplete_parameter_renames()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazorise.Components
{
    public class Autocomplete<TItem, TValue> : Microsoft.AspNetCore.Components.ComponentBase { }
}

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    public void Build( RenderTreeBuilder builder )
    {
        builder.OpenComponent<Blazorise.Components.Autocomplete<int, string>>( 0 );
        builder.AddAttribute( 1, ""CurrentSearch"", ""abc"" );
        builder.AddAttribute( 2, ""Multiple"", true );
        builder.CloseComponent();
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        var renameDiagnostics = diagnostics.Where( d => d.Id == "BLZP001" ).ToArray();
        Assert.Equal( 2, renameDiagnostics.Length );
        Assert.Contains( renameDiagnostics, d => d.GetMessage() == "Parameter 'CurrentSearch' was renamed to 'Search' for component 'Autocomplete<int, string>'" );
        Assert.Contains( renameDiagnostics, d => d.GetMessage() == "Parameter 'Multiple' was renamed to 'SelectionMode' for component 'Autocomplete<int, string>'" );
    }

    [Fact]
    public async Task Reports_autocomplete_parameter_removals()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazorise.Components
{
    public class Autocomplete<TItem, TValue> : Microsoft.AspNetCore.Components.ComponentBase { }
}

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    public void Build( RenderTreeBuilder builder )
    {
        builder.OpenComponent<Blazorise.Components.Autocomplete<int, string>>( 0 );
        builder.AddAttribute( 1, ""Validator"", ""validator"" );
        builder.AddAttribute( 2, ""AsyncValidator"", ""asyncValidator"" );
        builder.CloseComponent();
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        var removalDiagnostics = diagnostics.Where( d => d.Id == "BLZP003" ).ToArray();
        Assert.Equal( 2, removalDiagnostics.Length );
        Assert.Contains( removalDiagnostics, d => d.GetMessage() == "Parameter 'Validator' was removed from component 'Autocomplete<int, string>': Wrap Autocomplete in Validation instead of using the Validator parameter." );
        Assert.Contains( removalDiagnostics, d => d.GetMessage() == "Parameter 'AsyncValidator' was removed from component 'Autocomplete<int, string>': Wrap Autocomplete in Validation instead of using the AsyncValidator parameter." );
    }

    [Fact]
    public async Task Does_not_report_tvalueshape_for_autocomplete()
    {
        var source = @"
using System.Linq;
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazorise.Components
{
    public class Autocomplete<TItem, TValue> : Microsoft.AspNetCore.Components.ComponentBase { }
}

public class Country { }

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    public void Build( RenderTreeBuilder builder )
    {
        builder.OpenComponent<Blazorise.Components.Autocomplete<Country, string>>( 0 );
        builder.AddAttribute( 1, ""Multiple"", true );
        builder.CloseComponent();
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        Assert.DoesNotContain( diagnostics, d => d.Id == "BLZT001" );
    }

    [Fact]
    public async Task Reports_chartjs_static_scripts_in_index_html_only()
    {
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(
            sources: new[] { string.Empty },
            additionalFiles: new[]
            {
                (path: "wwwroot/index.html", content: @"
<script src=""https://cdnjs.cloudflare.com/ajax/libs/Chart.js/3.7.1/chart.min.js""></script>
<script src=""https://cdn.jsdelivr.net/npm/luxon@1.28.1""></script>
<script src=""https://cdn.jsdelivr.net/npm/chartjs-plugin-datalabels@2.0.0""></script>
<script src=""https://cdn.jsdelivr.net/npm/hammerjs@2.0.8""></script>
"),
            } );

        var chartJsDiagnostics = diagnostics.Where( d => d.Id == "BLZJS001" ).ToArray();
        Assert.Equal( 2, chartJsDiagnostics.Length );
        Assert.Contains( chartJsDiagnostics, d => d.GetMessage().Contains( "chart.min.js" ) );
        Assert.Contains( chartJsDiagnostics, d => d.GetMessage().Contains( "chartjs-plugin-datalabels" ) );

        Assert.DoesNotContain( chartJsDiagnostics, d => d.GetMessage().Contains( "luxon" ) );
        Assert.DoesNotContain( chartJsDiagnostics, d => d.GetMessage().Contains( "hammerjs" ) );
    }

    [Fact]
    public async Task Does_not_report_when_only_luxon_is_used()
    {
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(
            sources: new[] { string.Empty },
            additionalFiles: new[]
            {
                (path: "wwwroot/index.html", content: @"<script src=""https://cdn.jsdelivr.net/npm/luxon@1.28.1""></script>"),
            } );

        Assert.DoesNotContain( diagnostics, d => d.Id == "BLZJS001" );
    }

    [Fact]
    public async Task Reports_chartjs_static_scripts_in_host_files()
    {
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(
            sources: new[] { string.Empty },
            additionalFiles: new[]
            {
                (path: "Pages/_Host.cshtml", content: @"<script src=""https://cdn.jsdelivr.net/npm/chartjs-plugin-zoom@2.2.0/dist/chartjs-plugin-zoom.min.js""></script>"),
                (path: "App.razor", content: @"<script src=""https://cdn.jsdelivr.net/npm/chartjs-adapter-luxon@1.0.0""></script>"),
            } );

        var chartJsDiagnostics = diagnostics.Where( d => d.Id == "BLZJS001" ).ToArray();
        Assert.Equal( 2, chartJsDiagnostics.Length );
        Assert.Contains( chartJsDiagnostics, d => d.GetMessage().Contains( "_Host.cshtml" ) );
        Assert.Contains( chartJsDiagnostics, d => d.GetMessage().Contains( "App.razor" ) );
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
        builder.OpenComponent<Blazorise.DataGrid.DataGridColumn<int>>( 0 );
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
namespace Blazorise
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
            builder.AddComponentParameter( 1, ""PopupFieldColumnSize"", ""ColumnSize.IsFull.OnDesktop"" );
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
            builder.AddAttribute( 1, ""PopupFieldColumnSize"", ""ColumnSize.IsFull.OnDesktop"" );
            builder.CloseComponent();
        }
    }";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        Assert.Contains( diagnostics, d => d.Id == "BLZP001" && d.GetMessage().Contains( "PopupFieldColumnSize" ) );
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
        builder.AddAttribute( 1, ""SelectionMode"", mode ); // non-constant
        builder.AddAttribute( 2, ""Value"", value );
        builder.CloseComponent();
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        Assert.DoesNotContain( diagnostics, d => d.Id == "BLZT001" );
    }

    [Fact]
    public async Task Reports_tvalueshape_for_typeinference_callsite_constants()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazorise
{
    public enum DateInputSelectionMode { Single = 0, Range = 1, Multiple = 2 }

    public class DatePicker<TValue> : Microsoft.AspNetCore.Components.ComponentBase
    {
        public DateInputSelectionMode SelectionMode { get; set; }
        public TValue Value { get; set; }
    }
}

namespace Microsoft.AspNetCore.Components.CompilerServices
{
    public static class RuntimeHelpers
    {
        public static T TypeCheck<T>( T value ) => value;
    }
}

namespace __Blazor.SomeNamespace
{
    internal static class TypeInference
    {
        public static void CreateDatePicker_0<TValue>( RenderTreeBuilder builder, int seq, int __seq0, Blazorise.DateInputSelectionMode __arg0, int __seq1, TValue __arg1 )
        {
            builder.OpenComponent<Blazorise.DatePicker<TValue>>( seq );
            builder.AddComponentParameter( __seq0, nameof( Blazorise.DatePicker<TValue>.SelectionMode ), Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck( __arg0 ) );
            builder.AddComponentParameter( __seq1, nameof( Blazorise.DatePicker<TValue>.Value ), __arg1 );
            builder.CloseComponent();
        }
    }
}

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    private System.DateTime? value;

    public void Build( RenderTreeBuilder builder )
    {
#line 20 ""PickersPage.razor""
        __Blazor.SomeNamespace.TypeInference.CreateDatePicker_0( builder, 0, 1, Blazorise.DateInputSelectionMode.Multiple, 2, value );
#line default
#line hidden
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        var diagnostic = Assert.Single( diagnostics, d => d.Id == "BLZT001" );
        Assert.Contains( "SingleOrMultiListOrArray", diagnostic.GetMessage() );
        var mappedSpan = diagnostic.Location.GetMappedLineSpan();
        Assert.Equal( "PickersPage.razor", mappedSpan.Path );
        Assert.Equal( 20, mappedSpan.StartLinePosition.Line + 1 );
    }

    [Fact]
    public async Task Reports_tvalueshape_for_typeinference_multi_value_used_for_single_selection()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazorise
{
    public class DatePicker<TValue> : Microsoft.AspNetCore.Components.ComponentBase
    {
        public TValue Value { get; set; }
    }
}

namespace __Blazor.SomeNamespace
{
    internal static class TypeInference
    {
        public static void CreateDatePicker_0<TValue>( RenderTreeBuilder builder, int seq, int __seq0, TValue __arg0 )
        {
            builder.OpenComponent<Blazorise.DatePicker<TValue>>( seq );
            builder.AddComponentParameter( __seq0, nameof( Blazorise.DatePicker<TValue>.Value ), __arg0 );
            builder.CloseComponent();
        }
    }
}

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    public void Build( RenderTreeBuilder builder )
    {
        System.Collections.Generic.IReadOnlyList<System.DateTime?> values = new System.DateTime?[] { System.DateTime.Now };
#line 25 ""PickersPage.razor""
        __Blazor.SomeNamespace.TypeInference.CreateDatePicker_0( builder, 0, 1, values );
#line default
#line hidden
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        var diagnostic = Assert.Single( diagnostics, d => d.Id == "BLZT001" );
        Assert.Contains( "expects TValue shape 'Single'", diagnostic.GetMessage() );
        var mappedSpan = diagnostic.Location.GetMappedLineSpan();
        Assert.Equal( "PickersPage.razor", mappedSpan.Path );
        Assert.Equal( 25, mappedSpan.StartLinePosition.Line + 1 );
    }

    [Fact]
    public async Task Reports_tvalueshape_for_typeinference_pickerspage_invalid_lines_20_to_25()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazorise
{
    public enum DateInputSelectionMode { Single = 0, Range = 1, Multiple = 2 }

    public class DatePicker<TValue> : Microsoft.AspNetCore.Components.ComponentBase
    {
        public DateInputSelectionMode SelectionMode { get; set; }
        public TValue Value { get; set; }
    }
}

namespace Microsoft.AspNetCore.Components.CompilerServices
{
    public static class RuntimeHelpers
    {
        public static T TypeCheck<T>( T value ) => value;
    }
}

namespace __Blazor.SomeNamespace
{
    internal static class TypeInference
    {
        public static void CreateDatePicker_WithMode<TValue>( RenderTreeBuilder builder, int seq, int __seq0, Blazorise.DateInputSelectionMode __arg0, int __seq1, TValue __arg1 )
        {
            builder.OpenComponent<Blazorise.DatePicker<TValue>>( seq );
            builder.AddComponentParameter( __seq0, nameof( Blazorise.DatePicker<TValue>.SelectionMode ), Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck( __arg0 ) );
            builder.AddComponentParameter( __seq1, nameof( Blazorise.DatePicker<TValue>.Value ), __arg1 );
            builder.CloseComponent();
        }

        public static void CreateDatePicker_NoMode<TValue>( RenderTreeBuilder builder, int seq, int __seq0, TValue __arg0 )
        {
            builder.OpenComponent<Blazorise.DatePicker<TValue>>( seq );
            builder.AddComponentParameter( __seq0, nameof( Blazorise.DatePicker<TValue>.Value ), __arg0 );
            builder.CloseComponent();
        }
    }
}

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    public void Build( RenderTreeBuilder builder )
    {
        System.DateTime? singleValue = System.DateTime.Now;
        System.Collections.Generic.IReadOnlyList<System.DateTime?> multiValue = new System.DateTime?[] { System.DateTime.Now };

#line 20 ""PickersPage.razor""
        __Blazor.SomeNamespace.TypeInference.CreateDatePicker_WithMode( builder, 0, 1, Blazorise.DateInputSelectionMode.Multiple, 2, singleValue );
        __Blazor.SomeNamespace.TypeInference.CreateDatePicker_WithMode( builder, 0, 1, Blazorise.DateInputSelectionMode.Range, 2, singleValue );
        __Blazor.SomeNamespace.TypeInference.CreateDatePicker_NoMode( builder, 0, 1, multiValue );
        __Blazor.SomeNamespace.TypeInference.CreateDatePicker_WithMode( builder, 0, 1, Blazorise.DateInputSelectionMode.Single, 2, multiValue );
        __Blazor.SomeNamespace.TypeInference.CreateDatePicker_WithMode( builder, 0, 1, Blazorise.DateInputSelectionMode.Single, 2, multiValue );
        __Blazor.SomeNamespace.TypeInference.CreateDatePicker_NoMode( builder, 0, 1, multiValue );
#line default
#line hidden
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        var tvalueDiagnostics = diagnostics.Where( d => d.Id == "BLZT001" ).ToArray();
        Assert.Equal( 6, tvalueDiagnostics.Length );

        foreach ( var expectedLine in Enumerable.Range( 20, 6 ) )
        {
            Assert.Contains( tvalueDiagnostics, d =>
            {
                var mappedSpan = d.Location.GetMappedLineSpan();
                return mappedSpan.Path == "PickersPage.razor"
                       && mappedSpan.StartLinePosition.Line + 1 == expectedLine;
            } );
        }
    }

    [Fact]
    public async Task Does_not_duplicate_tvalueshape_diagnostics_for_nested_blocks()
    {
        var source = @"
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazorise
{
    public class Select<TValue> : Microsoft.AspNetCore.Components.ComponentBase
    {
        public bool Multiple { get; set; }
        public TValue Value { get; set; }
    }
}

public class MyComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    public void Build( RenderTreeBuilder builder )
    {
        if ( true )
        {
            builder.OpenComponent<Blazorise.Select<string>>( 0 );
            builder.AddAttribute( 1, ""Multiple"", true );
            builder.AddAttribute( 2, ""Value"", ""single"" );
            builder.CloseComponent();
        }
    }
}";

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync( source );

        Assert.Single( diagnostics, d => d.Id == "BLZT001" );
    }
}
