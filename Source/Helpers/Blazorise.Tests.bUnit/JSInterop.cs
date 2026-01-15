#region Using directives
using Blazorise.Charts;
using Blazorise.DataGrid;
using Blazorise.Modules;
using Blazorise.RichTextEdit;
using Bunit;
#endregion

namespace Blazorise.Tests.bUnit;

public static class JSInterop
{
    public static BunitJSInterop AddBlazoriseButton( this BunitJSInterop jsInterop )
    {
        AddBlazoriseUtilities( jsInterop );

        var module = jsInterop.SetupModule( new JSButtonModule( jsInterop.JSRuntime, new MockVersionProvider(), new( null, ( Options ) => { } ) ).ModuleFileName );
        module.SetupVoid( "import", _ => true ).SetVoidResult();
        module.SetupVoid( "initialize", _ => true ).SetVoidResult();
        module.SetupVoid( "destroy", _ => true ).SetVoidResult();

        return jsInterop;
    }

    public static BunitJSInterop AddBlazoriseBreakpoint( this BunitJSInterop jsInterop )
    {
        AddBlazoriseUtilities( jsInterop );

        var module = jsInterop.SetupModule( new JSBreakpointModule( jsInterop.JSRuntime, new MockVersionProvider(), new( null, ( Options ) => { } ) ).ModuleFileName );
        module.SetupVoid( "import", _ => true ).SetVoidResult();
        module.SetupVoid( "initialize", _ => true ).SetVoidResult();
        module.SetupVoid( "destroy", _ => true ).SetVoidResult();
        module.SetupVoid( "registerBreakpointComponent", _ => true ).SetVoidResult();
        module.SetupVoid( "unregisterBreakpointComponent", _ => true ).SetVoidResult();
        module.SetupVoid( "getBreakpoint", _ => true ).SetVoidResult();

        return jsInterop;
    }

    public static BunitJSInterop AddBlazoriseTextInput( this BunitJSInterop jsInterop )
    {
        AddBlazoriseUtilities( jsInterop );

        var module = jsInterop.SetupModule( new JSTextInputModule( jsInterop.JSRuntime, new MockVersionProvider(), new( null, ( Options ) => { } ) ).ModuleFileName );
        module.SetupVoid( "import", _ => true ).SetVoidResult();
        module.SetupVoid( "initialize", _ => true ).SetVoidResult();
        module.SetupVoid( "destroy", _ => true ).SetVoidResult();

        return jsInterop;
    }

    public static BunitJSInterop AddBlazoriseDatePicker( this BunitJSInterop jsInterop )
    {
        AddBlazoriseUtilities( jsInterop );

        jsInterop.SetupModule( new JSDatePickerModule( jsInterop.JSRuntime, new MockVersionProvider(), new( null, ( Options ) => { } ) ).ModuleFileName )
            .SetupVoid( "initialize", _ => true ).SetVoidResult();

        return jsInterop;
    }

    public static BunitJSInterop AddBlazoriseClosable( this BunitJSInterop jsInterop )
    {
        AddBlazoriseUtilities( jsInterop );

        var module = jsInterop.SetupModule( new JSClosableModule( jsInterop.JSRuntime, new MockVersionProvider(), new( null, ( Options ) => { } ) ).ModuleFileName );
        module.SetupVoid( "import", _ => true ).SetVoidResult();
        module.SetupVoid( "registerClosableComponent", _ => true ).SetVoidResult();
        module.SetupVoid( "unregisterClosableComponent", _ => true ).SetVoidResult();
        module.SetupVoid( "registerClosableLightComponent", _ => true ).SetVoidResult();
        module.SetupVoid( "unregisterClosableLightComponent", _ => true ).SetVoidResult();

        return jsInterop;
    }

    public static BunitJSInterop AddBlazoriseNumericInput( this BunitJSInterop jsInterop )
    {
        AddBlazoriseUtilities( jsInterop );

        var module = jsInterop.SetupModule( new JSNumericPickerModule( jsInterop.JSRuntime, new MockVersionProvider(), new( null, ( Options ) => { } ) ).ModuleFileName );
        module.SetupVoid( "import", _ => true ).SetVoidResult();
        module.SetupVoid( "initialize", _ => true ).SetVoidResult();
        module.SetupVoid( "destroy", _ => true ).SetVoidResult();

        return jsInterop;
    }

    public static BunitJSInterop AddBlazoriseRichTextEdit( this BunitJSInterop jsInterop )
    {
        AddBlazoriseUtilities( jsInterop );

        var module = jsInterop.SetupModule( new JSRichTextEditModule( jsInterop.JSRuntime, new MockVersionProvider(), new( null, ( Options ) => { } ), new RichTextEditOptions() ).ModuleFileName );
        module.SetupVoid( "import", _ => true ).SetVoidResult();
        module.SetupVoid( "loadStylesheets", _ => true ).SetVoidResult();
        module.SetupVoid( "initialize", _ => true ).SetVoidResult();
        module.SetupVoid( "destroy", _ => true ).SetVoidResult();
        module.SetupVoid( "setHtml", _ => true ).SetVoidResult();
        module.Setup<string>( "getHtml", _ => true ).SetResult( string.Empty );
        module.SetupVoid( "setDelta", _ => true ).SetVoidResult();
        module.Setup<string>( "getDelta", _ => true ).SetResult( string.Empty );
        module.SetupVoid( "setText", _ => true ).SetVoidResult();
        module.Setup<string>( "getText", _ => true ).SetResult( string.Empty );
        module.SetupVoid( "clearContent", _ => true ).SetVoidResult();
        module.SetupVoid( "setReadOnly", _ => true ).SetVoidResult();

        return jsInterop;
    }

    public static BunitJSInterop AddBlazoriseUtilities( this BunitJSInterop jsInterop )
    {
        var module = jsInterop.SetupModule( new JSUtilitiesModule( jsInterop.JSRuntime, new MockVersionProvider(), new( null, ( Options ) => { } ) ).ModuleFileName );
        module.SetupVoid( "import", _ => true ).SetVoidResult();
        module.SetupVoid( "setProperty", _ => true ).SetVoidResult();
        module.Setup<string>( "getUserAgent", _ => true ).SetResult( String.Empty );
        module.SetupVoid( "scrollElementIntoView", _ => true ).SetVoidResult();
        module.SetupVoid( "focus", _ => true ).SetVoidResult();
        module.SetupVoid( "log", _ => true ).SetVoidResult();

        return jsInterop;
    }

    public static BunitJSInterop AddBlazoriseModal( this BunitJSInterop jsInterop )
    {
        AddBlazoriseUtilities( jsInterop );

        var module = jsInterop.SetupModule( new MockJsModalModule( jsInterop.JSRuntime, new MockVersionProvider(), new( null, ( Options ) => { } ) ).ModuleFileName );
        module.SetupVoid( "import", _ => true ).SetVoidResult();
        module.SetupVoid( "open", _ => true ).SetVoidResult();
        module.SetupVoid( "close", _ => true ).SetVoidResult();

        return jsInterop;
    }


    public static BunitJSInterop AddBlazoriseTable( this BunitJSInterop jsInterop )
    {
        AddBlazoriseUtilities( jsInterop );

        var module = jsInterop.SetupModule( new JSTableModule( jsInterop.JSRuntime, new MockVersionProvider(), new( null, ( Options ) => { } ) ).ModuleFileName );
        module.SetupVoid( "initializeTableFixedHeader", _ => true ).SetVoidResult();
        module.SetupVoid( "destroyTableFixedHeader", _ => true ).SetVoidResult();
        module.SetupVoid( "fixedHeaderScrollTableToPixels", _ => true ).SetVoidResult();
        module.SetupVoid( "fixedHeaderScrollTableToRow", _ => true ).SetVoidResult();
        module.SetupVoid( "initializeResizable", _ => true ).SetVoidResult();
        module.SetupVoid( "destroyResizable", _ => true ).SetVoidResult();

        return jsInterop;
    }

    public static BunitJSInterop AddBlazoriseDataGrid( this BunitJSInterop jsInterop )
    {
        AddBlazoriseButton( jsInterop );
        AddBlazoriseTextInput( jsInterop );
        AddBlazoriseModal( jsInterop );
        AddBlazoriseTable( jsInterop );
        AddBlazoriseClosable( jsInterop );
        AddBlazoriseDropdown( jsInterop );
        AddBlazoriseDragDrop( jsInterop );

        var module = jsInterop.SetupModule( new JSDataGridModule( jsInterop.JSRuntime, new MockVersionProvider(), new( null, ( Options ) => { } ) ).ModuleFileName );
        module.SetupVoid( "initialize", _ => true ).SetVoidResult();
        module.SetupVoid( "scrollTo", _ => true ).SetVoidResult();
        module.SetupVoid( "blurActiveCellEditor", _ => true ).SetVoidResult();

        return jsInterop;
    }

    public static BunitJSInterop AddBlazoriseDropdown( this BunitJSInterop jsInterop )
    {
        AddBlazoriseUtilities( jsInterop );

        var module = jsInterop.SetupModule( new JSDropdownModule( jsInterop.JSRuntime, new MockVersionProvider(), new( null, ( Options ) => { } ) ).ModuleFileName );
        module.SetupVoid( "initialize", _ => true );
        module.SetupVoid( "destroy", _ => true );
        module.SetupVoid( "show", _ => true );
        module.SetupVoid( "hide", _ => true );

        return jsInterop;
    }

    public static BunitJSInterop AddBlazoriseDragDrop( this BunitJSInterop jsInterop )
    {
        AddBlazoriseUtilities( jsInterop );

        var module = jsInterop.SetupModule( new JSDragDropModule( jsInterop.JSRuntime, new MockVersionProvider(), new( null, ( Options ) => { } ) ).ModuleFileName );
        module.SetupVoid( "initialize", _ => true ).SetVoidResult();
        module.SetupVoid( "initializeThrottledDragEvents", _ => true ).SetVoidResult();
        module.SetupVoid( "destroy", _ => true ).SetVoidResult();
        module.SetupVoid( "destroyThrottledDragEvents", _ => true ).SetVoidResult();

        return jsInterop;
    }

    public static BunitJSInterop AddBlazoriseCharts( this BunitJSInterop jsInterop )
    {
        AddBlazoriseUtilities( jsInterop );

        var module = jsInterop.SetupModule( new JSChartModule( jsInterop.JSRuntime, new MockVersionProvider(), new( null, ( Options ) => { } ) ).ModuleFileName );
        module.SetupVoid( "initialize", _ => true ).SetVoidResult();
        module.SetupVoid( "update", _ => true ).SetVoidResult();
        module.SetupVoid( "destroy", _ => true ).SetVoidResult();
        module.SetupVoid( "changeChartType", _ => true ).SetVoidResult();
        module.SetupVoid( "setOptions", _ => true ).SetVoidResult();
        module.SetupVoid( "clear", _ => true ).SetVoidResult();
        module.SetupVoid( "addLabel", _ => true ).SetVoidResult();
        module.SetupVoid( "addDataset", _ => true ).SetVoidResult();
        module.SetupVoid( "removeDataset", _ => true ).SetVoidResult();
        module.SetupVoid( "addDatasetsAndUpdate", _ => true ).SetVoidResult();
        module.SetupVoid( "addLabelsDatasetsAndUpdate", _ => true ).SetVoidResult();
        module.SetupVoid( "setData", _ => true ).SetVoidResult();
        module.SetupVoid( "addData", _ => true ).SetVoidResult();
        module.SetupVoid( "shiftLabel", _ => true ).SetVoidResult();
        module.SetupVoid( "shiftData", _ => true ).SetVoidResult();
        module.SetupVoid( "popLabel", _ => true ).SetVoidResult();
        module.SetupVoid( "popData", _ => true ).SetVoidResult();
        module.SetupVoid( "resize", _ => true ).SetVoidResult();

        return jsInterop;
    }
}