using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.History;
using Blazorise.Reporting.Internal;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Blazorise.Reporting;

public partial class Report<TItem> : ComponentBase, IReportCommandExecutor
{
    private readonly ReportContext context = new();

    private readonly ReportToolbarContext toolbarContext;

    private readonly ReportDesignerState designerState = new();

    private readonly HistoryManager<ReportDesignerState> historyService = new();

    private ReportDefinition declarativeDefinition;

    private ReportStudioMode currentMode;

    private ReportPreviewFormat currentPreviewFormat;

    private ReportDesignerPanelTab selectedDesignerPanelTab = ReportDesignerPanelTab.Properties;

    private ReportContextMenuState contextMenu;

    private bool reportSelected = true;

    private string selectedElementKey;

    private int? selectedSectionIndex;

    private bool snapToGrid = true;

    private ReportDesignerDragKind draggedKind;

    private string draggedDataSourceName;

    private string draggedFieldName;

    private ReportElementType? draggedElementType;

    private string draggedElementText;

    private string draggedElementKey;

    private ReportElementDefinition draggedElement;

    private ReportDesignerDragPreview dragPreview;

    private ReportElementPointerDragState elementPointerDrag;

    private DateTime lastDragPreviewRenderTime;

    private ReportElementDefinition clipboardElement;

    private ReportOptions globalOptions;

    public Report()
    {
        toolbarContext = new( this );
    }

    protected override void OnInitialized()
    {
        context.ViewerOptions.PreviewFormats = GlobalOptions.PreviewFormats;
        context.ViewerOptions.DefaultFormat = GlobalOptions.DefaultPreviewFormat;
        context.ViewerOptions.AllowPrint = GlobalOptions.AllowPrint;
        context.ViewerOptions.AllowDownload = GlobalOptions.AllowDownload;

        currentMode = IsDesignerEnabled ? ReportStudioMode.Design : ReportStudioMode.Preview;
        currentPreviewFormat = DefaultPreviewFormat ?? context.ViewerOptions.DefaultFormat;
    }

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender && Definition is null && DefinitionMode != ReportDefinitionMode.UseDefinitionOnly )
        {
            declarativeDefinition = BuildDeclarativeDefinition();

            if ( DefinitionChanged.HasDelegate )
            {
                await DefinitionChanged.InvokeAsync( declarativeDefinition );
            }

            StateHasChanged();
        }
    }

    private ReportDefinition EffectiveDefinition
        => DefinitionMode == ReportDefinitionMode.AlwaysUseDeclarative
            ? BuildDeclarativeDefinition()
            : Definition ?? declarativeDefinition ?? BuildDeclarativeDefinition();

    private ReportDefinition BuildDeclarativeDefinition()
    {
        var definition = context.BuildDefinition( Page ?? context.Page );

        if ( !definition.DataSources.Any() && Data is not null )
        {
            definition.DataSources.Add( new()
            {
                Name = DataSourceName,
                Data = Data,
            } );
        }

        definition.Page = ResolvePage( definition.Page );

        return EnsureDefinitionIds( definition );
    }

    private ReportPageDefinition ResolvePage( ReportPageDefinition page )
    {
        page ??= new();

        if ( page.Width <= 0 || page.Height <= 0 )
        {
            ( var width, var height ) = page.Size == ReportPageSize.Letter ? ( 816d, 1056d ) : ( 794d, 1123d );

            if ( page.Orientation == ReportOrientation.Landscape )
            {
                ( width, height ) = ( height, width );
            }

            page.Width = width;
            page.Height = height;
        }

        return page;
    }

    private IEnumerable<object> ResolveItems( ReportDefinition definition, string dataSource = null )
    {
        object source = null;

        if ( !string.IsNullOrWhiteSpace( dataSource ) )
        {
            source = definition.DataSources.FirstOrDefault( x => string.Equals( x.Name, dataSource, StringComparison.OrdinalIgnoreCase ) )?.Data;
        }

        source ??= definition.DataSources.FirstOrDefault()?.Data ?? Data;

        if ( source is IEnumerable enumerable and not string )
        {
            foreach ( var item in enumerable )
            {
                yield return item;
            }
        }
    }

    private object ResolveFieldValue( object item, string field )
    {
        if ( item is null || string.IsNullOrWhiteSpace( field ) )
            return null;

        if ( item is IDictionary<string, object> dictionary && dictionary.TryGetValue( field, out var dictionaryValue ) )
            return dictionaryValue;

        var property = item.GetType().GetProperty( field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase );

        return property?.GetValue( item );
    }

    private string FormatValue( object value, string format )
    {
        if ( value is null )
            return string.Empty;

        if ( string.IsNullOrWhiteSpace( format ) )
            return Convert.ToString( value, CultureInfo.CurrentCulture );

        return value is IFormattable formattable
            ? formattable.ToString( format, CultureInfo.CurrentCulture )
            : Convert.ToString( value, CultureInfo.CurrentCulture );
    }

    private RenderFragment RenderDesigner() => builder =>
    {
        var definition = EffectiveDefinition;
        var sequence = 0;

        builder.OpenElement( sequence++, "div" );
        builder.AddAttribute( sequence++, "class", "b-report-designer" );

        builder.OpenComponent<Div>( sequence++ );
        builder.AddAttribute( sequence++, "class", "b-report-designer-dictionary" );
        builder.AddAttribute( sequence++, "Padding", Padding.Is3 );
        builder.AddAttribute( sequence++, "Background", Background.White );
        builder.AddAttribute( sequence++, "Border", Border.Is1 );
        builder.AddAttribute( sequence++, "Overflow", Overflow.Auto );
        builder.AddAttribute( sequence++, "ChildContent", (RenderFragment)( childBuilder =>
        {
            var childSequence = 0;
            RenderDataDictionary( childBuilder, ref childSequence, definition );
        } ) );
        builder.CloseComponent();

        builder.OpenComponent<Div>( sequence++ );
        builder.AddAttribute( sequence++, "class", "b-report-designer-surface" );
        builder.AddAttribute( sequence++, "Padding", Padding.Is3 );
        builder.AddAttribute( sequence++, "Background", Background.Light );
        builder.AddAttribute( sequence++, "Border", Border.Is1 );
        builder.AddAttribute( sequence++, "Overflow", Overflow.Auto );
        builder.AddAttribute( sequence++, "ChildContent", (RenderFragment)( childBuilder =>
        {
            var childSequence = 0;
            RenderReportPage( childBuilder, ref childSequence, definition, designMode: true );
        } ) );
        builder.CloseComponent();

        builder.OpenComponent<Div>( sequence++ );
        builder.AddAttribute( sequence++, "class", "b-report-designer-panel" );
        builder.AddAttribute( sequence++, "Padding", Padding.Is3 );
        builder.AddAttribute( sequence++, "Background", Background.White );
        builder.AddAttribute( sequence++, "Border", Border.Is1 );
        builder.AddAttribute( sequence++, "ChildContent", (RenderFragment)( childBuilder =>
        {
            var childSequence = 0;
            RenderDesignerPanel( childBuilder, ref childSequence, definition );
        } ) );
        builder.CloseComponent();
        RenderDesignerContextMenu( builder, ref sequence, definition );
        builder.CloseElement();
    };

    private void RenderDesignerPanel( RenderTreeBuilder builder, ref int sequence, ReportDefinition definition )
    {
        RenderDesignerPanelTabs( builder, ref sequence );

        if ( selectedDesignerPanelTab == ReportDesignerPanelTab.Explorer )
        {
            RenderReportExplorer( builder, ref sequence, definition );
        }
        else
        {
            RenderPropertiesPanel( builder, ref sequence, definition );
        }
    }

    private void RenderDesignerPanelTabs( RenderTreeBuilder builder, ref int sequence )
    {
        builder.OpenComponent<Div>( sequence++ );
        builder.AddAttribute( sequence++, "class", "b-report-designer-tabs" );
        builder.AddAttribute( sequence++, "ChildContent", (RenderFragment)( childBuilder =>
        {
            var childSequence = 0;
            RenderDesignerPanelTabButton( childBuilder, ref childSequence, "Properties", ReportDesignerPanelTab.Properties );
            RenderDesignerPanelTabButton( childBuilder, ref childSequence, "Report Explorer", ReportDesignerPanelTab.Explorer );
        } ) );
        builder.CloseComponent();
    }

    private void RenderDesignerPanelTabButton( RenderTreeBuilder builder, ref int sequence, string text, ReportDesignerPanelTab tab )
    {
        builder.OpenComponent<Button>( sequence++ );
        builder.AddAttribute( sequence++, "Color", selectedDesignerPanelTab == tab ? Color.Primary : Color.Light );
        builder.AddAttribute( sequence++, "Clicked", EventCallback.Factory.Create<MouseEventArgs>( this, () => selectedDesignerPanelTab = tab ) );
        builder.AddAttribute( sequence++, "ChildContent", (RenderFragment)( buttonBuilder => buttonBuilder.AddContent( 0, text ) ) );
        builder.CloseComponent();
    }

    private void RenderPropertiesPanel( RenderTreeBuilder builder, ref int sequence, ReportDefinition definition )
    {
        var hasSelection = reportSelected || selectedSectionIndex is not null || !string.IsNullOrWhiteSpace( selectedElementKey );

        RenderReportProperties( builder, ref sequence, definition );
        RenderSelectedSectionProperties( builder, ref sequence, definition );
        RenderSelectedElementProperties( builder, ref sequence, definition );
        RenderSelectedElementTools( builder, ref sequence, definition );

        if ( hasSelection )
            return;

        builder.OpenComponent<Paragraph>( sequence++ );
        builder.AddAttribute( sequence++, "TextColor", TextColor.Secondary );
        builder.AddAttribute( sequence++, "ChildContent", (RenderFragment)( paragraphBuilder => paragraphBuilder.AddContent( 0, "Select a band or report element to edit its properties." ) ) );
        builder.CloseComponent();
    }

    private void RenderReportProperties( RenderTreeBuilder builder, ref int sequence, ReportDefinition definition )
    {
        if ( !reportSelected )
            return;

        builder.OpenComponent<Div>( sequence++ );
        builder.AddAttribute( sequence++, "Margin", Margin.Is3.FromBottom );
        builder.AddAttribute( sequence++, "ChildContent", (RenderFragment)( childBuilder =>
        {
            var childSequence = 0;

            childBuilder.OpenElement( childSequence++, "h6" );
            childBuilder.AddContent( childSequence++, "Report properties" );
            childBuilder.CloseElement();

            RenderDesignerCheckbox( childBuilder, ref childSequence, "Snap to grid", snapToGrid, OnSnapToGridChanged );
            RenderDesignerNumberInput( childBuilder, ref childSequence, "Page width", definition.Page.Width, value => UpdateReportPageAsync( page => page.Width = Math.Max( 1, value ) ) );
            RenderDesignerNumberInput( childBuilder, ref childSequence, "Page height", definition.Page.Height, value => UpdateReportPageAsync( page => page.Height = Math.Max( 1, value ) ) );
        } ) );
        builder.CloseComponent();
    }

    private void RenderReportExplorer( RenderTreeBuilder builder, ref int sequence, ReportDefinition definition )
    {
        builder.OpenElement( sequence++, "h5" );
        builder.AddContent( sequence++, "Report Explorer" );
        builder.CloseElement();

        builder.OpenComponent<_ReportTreeView>( sequence++ );
        builder.AddAttribute( sequence++, "Nodes", BuildReportExplorerNodes( definition ) );
        builder.AddAttribute( sequence++, "NodeClicked", EventCallback.Factory.Create<ReportTreeNode>( this, OnReportTreeNodeClicked ) );
        builder.AddAttribute( sequence++, "NodeContextMenu", EventCallback.Factory.Create<ReportTreeNodeMouseEventArgs>( this, OnReportTreeNodeContextMenu ) );
        builder.CloseComponent();
    }

    private void RenderDesignerContextMenu( RenderTreeBuilder builder, ref int sequence, ReportDefinition definition )
    {
        if ( contextMenu is null || !contextMenu.Visible )
            return;

        builder.OpenElement( sequence++, "div" );
        builder.AddAttribute( sequence++, "class", "b-report-context-menu" );
        builder.AddAttribute( sequence++, "style", $"left:{contextMenu.ClientX}px;top:{contextMenu.ClientY}px;" );

        switch ( contextMenu.Target )
        {
            case ReportContextMenuTarget.Section when contextMenu.SectionIndex >= 0 && contextMenu.SectionIndex < definition.Sections.Count:
                RenderContextMenuButton( builder, ref sequence, "Insert band before", () => InsertSectionAsync( insertAfter: false ) );
                RenderContextMenuButton( builder, ref sequence, "Insert band after", () => InsertSectionAsync( insertAfter: true ) );
                break;
            case ReportContextMenuTarget.Element:
                RenderContextMenuButton( builder, ref sequence, "Delete element", DeleteSelectedElementAsync );
                break;
        }

        RenderContextMenuButton( builder, ref sequence, "Close", () =>
        {
            CloseContextMenu();
            return Task.CompletedTask;
        } );

        builder.CloseElement();
    }

    private void RenderContextMenuButton( RenderTreeBuilder builder, ref int sequence, string text, Func<Task> clicked )
    {
        builder.OpenElement( sequence++, "button" );
        builder.AddAttribute( sequence++, "type", "button" );
        builder.AddAttribute( sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>( this, clicked ) );
        builder.AddContent( sequence++, text );
        builder.CloseElement();
    }

    private void RenderDataDictionary( RenderTreeBuilder builder, ref int sequence, ReportDefinition definition )
    {
        var dataSources = ResolveDataSourceDictionary( definition ).ToList();

        builder.OpenElement( sequence++, "h5" );
        builder.AddContent( sequence++, "Toolbox" );
        builder.CloseElement();

        builder.OpenComponent<_ReportTreeView>( sequence++ );
        builder.AddAttribute( sequence++, "Nodes", BuildToolboxNodes() );
        builder.AddAttribute( sequence++, "NodeDragStarted", EventCallback.Factory.Create<ReportTreeNodeDragEventArgs>( this, OnToolboxTreeNodeDragStarted ) );
        builder.AddAttribute( sequence++, "NodeDragEnded", EventCallback.Factory.Create<ReportTreeNode>( this, _ => ClearDesignerDragAsync() ) );
        builder.CloseComponent();

        builder.OpenElement( sequence++, "h5" );
        builder.AddAttribute( sequence++, "class", "b-report-dictionary-title" );
        builder.AddContent( sequence++, "Fields explorer" );
        builder.CloseElement();

        if ( dataSources.Count == 0 )
        {
            builder.OpenComponent<Paragraph>( sequence++ );
            builder.AddAttribute( sequence++, "TextColor", TextColor.Secondary );
            builder.AddAttribute( sequence++, "ChildContent", (RenderFragment)( paragraphBuilder => paragraphBuilder.AddContent( 0, "No data source fields." ) ) );
            builder.CloseComponent();
            return;
        }

        builder.OpenComponent<_ReportTreeView>( sequence++ );
        builder.AddAttribute( sequence++, "Nodes", BuildFieldsExplorerNodes( dataSources ) );
        builder.AddAttribute( sequence++, "NodeDragStarted", EventCallback.Factory.Create<ReportTreeNodeDragEventArgs>( this, OnFieldsTreeNodeDragStarted ) );
        builder.AddAttribute( sequence++, "NodeDragEnded", EventCallback.Factory.Create<ReportTreeNode>( this, _ => ClearDesignerDragAsync() ) );
        builder.CloseComponent();
    }

    private IReadOnlyList<ReportTreeNode> BuildToolboxNodes()
    {
        return
        [
            new()
            {
                Key = "toolbox",
                Text = "Report Items",
                Kind = ReportTreeNodeKind.Folder,
                Children =
                [
                    CreateToolboxNode( "toolbox:label", "Label", ReportElementType.Text, "Label" ),
                    CreateToolboxNode( "toolbox:text", "Text", ReportElementType.Text, "Text" ),
                    CreateToolboxNode( "toolbox:image", "Image", ReportElementType.Image, null ),
                    CreateToolboxNode( "toolbox:line", "Line", ReportElementType.Line, null ),
                    CreateToolboxNode( "toolbox:rectangle", "Rectangle", ReportElementType.Rectangle, null ),
                ],
            }
        ];
    }

    private static ReportTreeNode CreateToolboxNode( string key, string text, ReportElementType elementType, string elementText )
    {
        return new()
        {
            Key = key,
            Text = text,
            Kind = GetElementTreeNodeKind( elementType ),
            Draggable = true,
            Value = new ReportToolboxTreeNodeValue( elementType, elementText ?? text ),
        };
    }

    private IReadOnlyList<ReportTreeNode> BuildFieldsExplorerNodes( IEnumerable<ReportDesignerDataSourceNode> dataSources )
    {
        return
        [
            new()
            {
                Key = "fields:data-sources",
                Text = "Data Sources",
                Kind = ReportTreeNodeKind.Folder,
                Children = dataSources.Select( dataSource => new ReportTreeNode
                {
                    Key = $"fields:data-source:{dataSource.Name}",
                    Text = dataSource.Name,
                    Kind = ReportTreeNodeKind.DataSource,
                    Children = dataSource.Fields.Select( field => new ReportTreeNode
                    {
                        Key = $"fields:field:{dataSource.Name}:{field.Name}",
                        Text = field.Name,
                        Detail = GetDataTypeDisplayName( field.DataType ),
                        Kind = ReportTreeNodeKind.Field,
                        Draggable = true,
                        Value = new ReportFieldTreeNodeValue( dataSource.Name, field.Name ),
                    } ).ToList(),
                } ).ToList(),
            }
        ];
    }

    private IReadOnlyList<ReportTreeNode> BuildReportExplorerNodes( ReportDefinition definition )
    {
        return
        [
            new()
            {
                Key = "report",
                Text = "Report",
                Kind = ReportTreeNodeKind.Report,
                Selectable = true,
                Selected = reportSelected,
                Children = definition.Sections.Select( ( section, sectionIndex ) => new ReportTreeNode
                {
                    Key = CreateSectionTreeNodeKey( sectionIndex ),
                    Text = GetSectionDisplayName( section ),
                    Detail = GetSectionTypeDisplayName( section.Type ),
                    Kind = ReportTreeNodeKind.Band,
                    Selectable = true,
                    Selected = selectedSectionIndex == sectionIndex && string.IsNullOrWhiteSpace( selectedElementKey ),
                    Children = section.Elements.Select( ( element, elementIndex ) =>
                    {
                        var elementKey = GetDesignerElementKey( element );

                        return new ReportTreeNode
                        {
                            Key = CreateElementTreeNodeKey( elementKey ),
                            Text = element.Name ?? element.Text ?? element.Field ?? element.Type.ToString(),
                            Detail = element.Type.ToString(),
                            Kind = GetElementTreeNodeKind( element.Type ),
                            Selectable = true,
                            Selected = elementKey == selectedElementKey,
                        };
                    } ).ToList(),
                } ).ToList(),
            }
        ];
    }

    private Task OnReportTreeNodeClicked( ReportTreeNode node )
    {
        selectedDesignerPanelTab = ReportDesignerPanelTab.Properties;

        if ( string.Equals( node?.Key, "report", StringComparison.Ordinal ) )
        {
            SelectReport();
        }
        else if ( TryResolveSectionTreeNode( node, out var sectionIndex ) )
        {
            SelectSection( sectionIndex );
        }
        else if ( TryResolveElementTreeNode( node, out var elementKey ) )
        {
            SelectElement( elementKey );
        }

        return Task.CompletedTask;
    }

    private Task OnReportTreeNodeContextMenu( ReportTreeNodeMouseEventArgs eventArgs )
    {
        if ( TryResolveSectionTreeNode( eventArgs.Node, out var sectionIndex ) )
        {
            OpenSectionContextMenu( sectionIndex, eventArgs.MouseEventArgs );
        }
        else if ( TryResolveElementTreeNode( eventArgs.Node, out var elementKey ) )
        {
            OpenElementContextMenu( elementKey, eventArgs.MouseEventArgs );
        }

        return Task.CompletedTask;
    }

    private Task OnFieldsTreeNodeDragStarted( ReportTreeNodeDragEventArgs eventArgs )
    {
        if ( eventArgs.Node?.Value is ReportFieldTreeNodeValue value )
        {
            BeginFieldDrag( value.DataSourceName, value.FieldName );
        }

        return Task.CompletedTask;
    }

    private Task OnToolboxTreeNodeDragStarted( ReportTreeNodeDragEventArgs eventArgs )
    {
        if ( eventArgs.Node?.Value is ReportToolboxTreeNodeValue value )
        {
            BeginToolboxElementDrag( value.ElementType, value.Text );
        }

        return Task.CompletedTask;
    }

    private RenderFragment RenderViewer() => builder =>
    {
        var definition = EffectiveDefinition;
        var sequence = 0;

        builder.OpenComponent<Div>( sequence++ );
        builder.AddAttribute( sequence++, "class", "b-report-viewer" );
        builder.AddAttribute( sequence++, "Padding", Padding.Is3 );
        builder.AddAttribute( sequence++, "Background", Background.Light );
        builder.AddAttribute( sequence++, "Border", Border.Is1 );
        builder.AddAttribute( sequence++, "Overflow", Overflow.Auto );
        builder.AddAttribute( sequence++, "ChildContent", (RenderFragment)( childBuilder =>
        {
            var childSequence = 0;

            if ( CurrentPreviewFormat == ReportPreviewFormat.Pdf )
            {
                childBuilder.OpenComponent<Div>( childSequence++ );
                childBuilder.AddAttribute( childSequence++, "Padding", Padding.Is5 );
                childBuilder.AddAttribute( childSequence++, "Border", Border.Is1 );
                childBuilder.AddAttribute( childSequence++, "Background", Background.White );
                childBuilder.AddAttribute( childSequence++, "TextAlignment", TextAlignment.Center );
                childBuilder.AddAttribute( childSequence++, "ChildContent", (RenderFragment)( placeholderBuilder => placeholderBuilder.AddContent( 0, "PDF preview is configured for this report. A PDF renderer can feed Blazorise.PdfViewer in the next implementation step." ) ) );
                childBuilder.CloseComponent();
            }
            else
            {
                RenderReportPage( childBuilder, ref childSequence, definition, designMode: false );
            }
        } ) );

        builder.CloseComponent();
    };

    private void RenderReportPage( RenderTreeBuilder builder, ref int sequence, ReportDefinition definition, bool designMode )
    {
        builder.OpenElement( sequence++, "div" );
        builder.AddAttribute( sequence++, "class", designMode ? "b-report-page b-report-page-design" : "b-report-page" );
        builder.AddAttribute( sequence++, "style", $"width:{definition.Page.Width}px;min-height:{definition.Page.Height}px;" );

        for ( var sectionIndex = 0; sectionIndex < definition.Sections.Count; sectionIndex++ )
        {
            RenderSection( builder, ref sequence, definition, definition.Sections[sectionIndex], sectionIndex, designMode );
        }

        builder.CloseElement();
    }

    private void RenderSection( RenderTreeBuilder builder, ref int sequence, ReportDefinition definition, ReportSectionDefinition section, int sectionIndex, bool designMode )
    {
        var items = !designMode && section.Type == ReportSectionType.Detail
            ? ResolveItems( definition, section.DataSource ).ToList()
            : new List<object> { ResolveItems( definition, section.DataSource ).FirstOrDefault() };

        if ( items.Count == 0 )
            items.Add( null );

        foreach ( var item in items )
        {
            builder.OpenElement( sequence++, "section" );
            builder.AddAttribute( sequence++, "class", $"b-report-section {section.Class}".Trim() );
            builder.AddAttribute( sequence++, "style", $"height:{section.Height}px;{section.Style}" );

            if ( designMode )
            {
                builder.AddAttribute( sequence++, "ondragover", EventUtil.AsNonRenderingEventHandler<DragEventArgs>( eventArgs => PreviewDesignerDragAsync( sectionIndex, eventArgs ) ) );
                builder.AddEventPreventDefaultAttribute( sequence++, "ondragover", true );
                builder.AddAttribute( sequence++, "ondrop", EventCallback.Factory.Create<DragEventArgs>( this, eventArgs => DropDesignerItemAsync( sectionIndex, eventArgs ) ) );
                builder.AddEventPreventDefaultAttribute( sequence++, "ondrop", true );
                builder.AddAttribute( sequence++, "onpointermove", EventUtil.AsNonRenderingEventHandler<PointerEventArgs>( eventArgs => PreviewElementPointerDragAsync( sectionIndex, eventArgs ) ) );
                builder.AddEventPreventDefaultAttribute( sequence++, "onpointermove", true );
                builder.AddAttribute( sequence++, "onpointerup", EventCallback.Factory.Create<PointerEventArgs>( this, eventArgs => CompleteElementPointerDragAsync( sectionIndex, eventArgs ) ) );
                builder.AddEventPreventDefaultAttribute( sequence++, "onpointerup", true );
                builder.AddAttribute( sequence++, "onpointercancel", EventCallback.Factory.Create<PointerEventArgs>( this, _ => CancelElementPointerDragAsync() ) );
                builder.AddAttribute( sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>( this, () => SelectSection( sectionIndex ) ) );
                builder.AddAttribute( sequence++, "oncontextmenu", EventCallback.Factory.Create<MouseEventArgs>( this, eventArgs => OpenSectionContextMenu( sectionIndex, eventArgs ) ) );
                builder.AddEventPreventDefaultAttribute( sequence++, "oncontextmenu", true );

                builder.OpenElement( sequence++, "div" );
                builder.AddAttribute( sequence++, "class", "b-report-section-label" );
                builder.AddContent( sequence++, $"{GetSectionTypeDisplayName( section.Type )}: {GetSectionDisplayName( section )}" );
                builder.CloseElement();
            }

            for ( var i = 0; i < section.Elements.Count; i++ )
            {
                var element = section.Elements[i];
                var key = GetDesignerElementKey( element );

                RenderElement( builder, ref sequence, item, element, designMode, key );
            }

            if ( designMode && dragPreview?.SectionIndex == sectionIndex )
            {
                RenderDesignerDragPreview( builder, ref sequence, dragPreview );
            }

            builder.CloseElement();
        }
    }

    private void RenderElement( RenderTreeBuilder builder, ref int sequence, object item, ReportElementDefinition element, bool designMode, string elementKey )
    {
        var style = $"left:{element.X}px;top:{element.Y}px;width:{element.Width}px;height:{element.Height}px;{element.Style}";
        var cssClass = $"b-report-element b-report-element-{element.Type.ToString().ToLowerInvariant()} {element.Class}".Trim();

        builder.OpenElement( sequence++, "div" );
        builder.SetKey( elementKey );
        builder.AddAttribute( sequence++, "class", designMode ? $"{cssClass} b-report-element-design {( elementKey == selectedElementKey ? "active" : string.Empty )}" : cssClass );
        builder.AddAttribute( sequence++, "style", style );

        if ( designMode )
        {
            builder.AddAttribute( sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>( this, () => SelectElement( elementKey ) ) );
            builder.AddEventStopPropagationAttribute( sequence++, "onclick", true );
            builder.AddAttribute( sequence++, "oncontextmenu", EventCallback.Factory.Create<MouseEventArgs>( this, eventArgs => OpenElementContextMenu( elementKey, eventArgs ) ) );
            builder.AddEventPreventDefaultAttribute( sequence++, "oncontextmenu", true );
            builder.AddEventStopPropagationAttribute( sequence++, "oncontextmenu", true );
            builder.AddAttribute( sequence++, "onpointerdown", EventCallback.Factory.Create<PointerEventArgs>( this, eventArgs => BeginElementPointerDrag( elementKey, eventArgs ) ) );
            builder.AddEventPreventDefaultAttribute( sequence++, "onpointerdown", true );
            builder.AddEventStopPropagationAttribute( sequence++, "onpointerdown", true );
        }

        switch ( element.Type )
        {
            case ReportElementType.Field:
                var fieldItem = string.IsNullOrWhiteSpace( element.DataSource )
                    ? item
                    : ResolveItems( EffectiveDefinition, element.DataSource ).FirstOrDefault() ?? item;

                builder.AddContent( sequence++, designMode ? FormatFieldExpression( element ) : FormatValue( ResolveFieldValue( fieldItem, element.Field ), element.Format ) );
                break;
            case ReportElementType.Table:
                RenderTable( builder, ref sequence, element );
                break;
            case ReportElementType.Image:
                builder.OpenElement( sequence++, "img" );
                builder.AddAttribute( sequence++, "src", element.Source );
                builder.AddAttribute( sequence++, "alt", element.Text ?? element.Name );
                builder.CloseElement();
                break;
            case ReportElementType.PageBreak:
                break;
            default:
                builder.AddContent( sequence++, element.Text );
                break;
        }

        builder.CloseElement();
    }

    private void RenderDesignerDragPreview( RenderTreeBuilder builder, ref int sequence, ReportDesignerDragPreview preview )
    {
        builder.OpenElement( sequence++, "div" );
        builder.AddAttribute( sequence++, "class", $"b-report-drag-preview b-report-element-{preview.ElementType.ToString().ToLowerInvariant()}" );
        builder.AddAttribute( sequence++, "style", $"left:{preview.X}px;top:{preview.Y}px;width:{preview.Width}px;height:{preview.Height}px;" );
        builder.AddContent( sequence++, preview.Text );
        builder.CloseElement();
    }

    private void RenderSelectedElementTools( RenderTreeBuilder builder, ref int sequence, ReportDefinition definition )
    {
        var selected = FindSelectedElement( definition );

        if ( selected is null )
            return;

        builder.OpenComponent<Div>( sequence++ );
        builder.AddAttribute( sequence++, "Margin", Margin.Is3.FromTop );
        builder.AddAttribute( sequence++, "ChildContent", (RenderFragment)( childBuilder =>
        {
            var childSequence = 0;

            childBuilder.OpenElement( childSequence++, "h6" );
            childBuilder.AddContent( childSequence++, "Selected element" );
            childBuilder.CloseElement();

            childBuilder.OpenComponent<Div>( childSequence++ );
            childBuilder.AddAttribute( childSequence++, "Flex", Flex.Wrap );
            childBuilder.AddAttribute( childSequence++, "Gap", Gap.Is2 );
            childBuilder.AddAttribute( childSequence++, "ChildContent", (RenderFragment)( toolsBuilder =>
            {
                var toolSequence = 0;
                RenderDesignerButton( toolsBuilder, ref toolSequence, "Left", () => MoveSelectedElementAsync( -8, 0, 0, 0 ) );
                RenderDesignerButton( toolsBuilder, ref toolSequence, "Up", () => MoveSelectedElementAsync( 0, -8, 0, 0 ) );
                RenderDesignerButton( toolsBuilder, ref toolSequence, "Down", () => MoveSelectedElementAsync( 0, 8, 0, 0 ) );
                RenderDesignerButton( toolsBuilder, ref toolSequence, "Right", () => MoveSelectedElementAsync( 8, 0, 0, 0 ) );
                RenderDesignerButton( toolsBuilder, ref toolSequence, "Wider", () => MoveSelectedElementAsync( 0, 0, 16, 0 ) );
                RenderDesignerButton( toolsBuilder, ref toolSequence, "Taller", () => MoveSelectedElementAsync( 0, 0, 0, 8 ) );
            } ) );
            childBuilder.CloseComponent();
        } ) );
        builder.CloseComponent();
    }

    private void RenderSelectedSectionProperties( RenderTreeBuilder builder, ref int sequence, ReportDefinition definition )
    {
        var selected = FindSelectedSection( definition );

        if ( selected is null || !string.IsNullOrWhiteSpace( selectedElementKey ) )
            return;

        builder.OpenComponent<Div>( sequence++ );
        builder.AddAttribute( sequence++, "Margin", Margin.Is3.FromBottom );
        builder.AddAttribute( sequence++, "ChildContent", (RenderFragment)( childBuilder =>
        {
            var childSequence = 0;

            childBuilder.OpenElement( childSequence++, "h6" );
            childBuilder.AddContent( childSequence++, "Band properties" );
            childBuilder.CloseElement();

            RenderDesignerInput( childBuilder, ref childSequence, "Name", selected.Name, value => UpdateSelectedSectionAsync( section => section.Name = value ) );
            RenderDesignerNumberInput( childBuilder, ref childSequence, "Height", selected.Height, value => UpdateSelectedSectionAsync( section => section.Height = Math.Max( 8, value ) ) );
            RenderDesignerInput( childBuilder, ref childSequence, "Data source", selected.DataSource, value => UpdateSelectedSectionAsync( section => section.DataSource = value ) );
            RenderDesignerInput( childBuilder, ref childSequence, "Style", selected.Style, value => UpdateSelectedSectionAsync( section => section.Style = value ) );

            childBuilder.OpenComponent<Div>( childSequence++ );
            childBuilder.AddAttribute( childSequence++, "Flex", Flex.Wrap );
            childBuilder.AddAttribute( childSequence++, "Gap", Gap.Is2 );
            childBuilder.AddAttribute( childSequence++, "Margin", Margin.Is3.FromTop );
            childBuilder.AddAttribute( childSequence++, "ChildContent", (RenderFragment)( toolsBuilder =>
            {
                var toolsSequence = 0;
                RenderDesignerButton( toolsBuilder, ref toolsSequence, "Insert before", () => InsertSectionAsync( insertAfter: false ) );
                RenderDesignerButton( toolsBuilder, ref toolsSequence, "Insert after", () => InsertSectionAsync( insertAfter: true ) );
            } ) );
            childBuilder.CloseComponent();
        } ) );
        builder.CloseComponent();
    }

    private void RenderSelectedElementProperties( RenderTreeBuilder builder, ref int sequence, ReportDefinition definition )
    {
        var selected = FindSelectedElement( definition );

        if ( selected is null )
            return;

        builder.OpenComponent<Div>( sequence++ );
        builder.AddAttribute( sequence++, "Margin", Margin.Is3.FromTop );
        builder.AddAttribute( sequence++, "ChildContent", (RenderFragment)( childBuilder =>
        {
            var childSequence = 0;

            childBuilder.OpenElement( childSequence++, "h6" );
            childBuilder.AddContent( childSequence++, "Properties" );
            childBuilder.CloseElement();

            RenderDesignerInput( childBuilder, ref childSequence, "Name", selected.Name, value => UpdateSelectedElementAsync( element => element.Name = value ) );
            RenderDesignerNumberInput( childBuilder, ref childSequence, "X", selected.X, value => UpdateSelectedElementAsync( element => element.X = value ) );
            RenderDesignerNumberInput( childBuilder, ref childSequence, "Y", selected.Y, value => UpdateSelectedElementAsync( element => element.Y = value ) );
            RenderDesignerNumberInput( childBuilder, ref childSequence, "Width", selected.Width, value => UpdateSelectedElementAsync( element => element.Width = value ) );
            RenderDesignerNumberInput( childBuilder, ref childSequence, "Height", selected.Height, value => UpdateSelectedElementAsync( element => element.Height = value ) );

            switch ( selected.Type )
            {
                case ReportElementType.Text:
                    RenderDesignerInput( childBuilder, ref childSequence, "Text", selected.Text, value => UpdateSelectedElementAsync( element => element.Text = value ) );
                    break;
                case ReportElementType.Field:
                    RenderDesignerInput( childBuilder, ref childSequence, "Expression", FormatFieldExpression( selected ), valueChanged: null, readOnly: true );
                    RenderDesignerInput( childBuilder, ref childSequence, "Format", selected.Format, value => UpdateSelectedElementAsync( element => element.Format = value ) );
                    break;
                case ReportElementType.Image:
                    RenderDesignerInput( childBuilder, ref childSequence, "Source", selected.Source, value => UpdateSelectedElementAsync( element => element.Source = value ) );
                    RenderDesignerInput( childBuilder, ref childSequence, "Alt text", selected.Text, value => UpdateSelectedElementAsync( element => element.Text = value ) );
                    break;
            }

            RenderDesignerInput( childBuilder, ref childSequence, "Style", selected.Style, value => UpdateSelectedElementAsync( element => element.Style = value ) );
        } ) );
        builder.CloseComponent();
    }

    private void RenderDesignerInput( RenderTreeBuilder builder, ref int sequence, string label, string value, Func<string, Task> valueChanged, bool readOnly = false )
    {
        builder.OpenComponent<Field>( sequence++ );
        builder.AddAttribute( sequence++, "Margin", Margin.Is2.FromBottom );
        builder.AddAttribute( sequence++, "ChildContent", (RenderFragment)( fieldBuilder =>
        {
            var fieldSequence = 0;

            fieldBuilder.OpenComponent<FieldLabel>( fieldSequence++ );
            fieldBuilder.AddAttribute( fieldSequence++, "ChildContent", (RenderFragment)( labelBuilder => labelBuilder.AddContent( 0, label ) ) );
            fieldBuilder.CloseComponent();

            fieldBuilder.OpenComponent<TextInput>( fieldSequence++ );
            fieldBuilder.AddAttribute( fieldSequence++, "Value", value );
            fieldBuilder.AddAttribute( fieldSequence++, "ReadOnly", readOnly );

            if ( valueChanged is not null )
            {
                fieldBuilder.AddAttribute( fieldSequence++, "ValueChanged", EventCallback.Factory.Create<string>( this, valueChanged ) );
                fieldBuilder.AddAttribute( fieldSequence++, "Immediate", true );
            }

            fieldBuilder.CloseComponent();
        } ) );
        builder.CloseComponent();
    }

    private void RenderDesignerNumberInput( RenderTreeBuilder builder, ref int sequence, string label, double value, Func<double, Task> valueChanged )
    {
        builder.OpenComponent<Field>( sequence++ );
        builder.AddAttribute( sequence++, "Margin", Margin.Is2.FromBottom );
        builder.AddAttribute( sequence++, "ChildContent", (RenderFragment)( fieldBuilder =>
        {
            var fieldSequence = 0;

            fieldBuilder.OpenComponent<FieldLabel>( fieldSequence++ );
            fieldBuilder.AddAttribute( fieldSequence++, "ChildContent", (RenderFragment)( labelBuilder => labelBuilder.AddContent( 0, label ) ) );
            fieldBuilder.CloseComponent();

            fieldBuilder.OpenComponent<NumericInput<double>>( fieldSequence++ );
            fieldBuilder.AddAttribute( fieldSequence++, "Value", value );
            fieldBuilder.AddAttribute( fieldSequence++, "ValueChanged", EventCallback.Factory.Create<double>( this, valueChanged ) );
            fieldBuilder.AddAttribute( fieldSequence++, "Immediate", true );
            fieldBuilder.AddAttribute( fieldSequence++, "Step", 1m );
            fieldBuilder.CloseComponent();
        } ) );
        builder.CloseComponent();
    }

    private void RenderDesignerCheckbox( RenderTreeBuilder builder, ref int sequence, string label, bool value, Action<ChangeEventArgs> valueChanged )
    {
        builder.OpenComponent<Field>( sequence++ );
        builder.AddAttribute( sequence++, "Margin", Margin.Is2.FromBottom );
        builder.AddAttribute( sequence++, "ChildContent", (RenderFragment)( fieldBuilder =>
        {
            var fieldSequence = 0;

            fieldBuilder.OpenElement( fieldSequence++, "label" );
            fieldBuilder.AddAttribute( fieldSequence++, "class", "b-report-designer-option" );
            fieldBuilder.OpenElement( fieldSequence++, "input" );
            fieldBuilder.AddAttribute( fieldSequence++, "type", "checkbox" );
            fieldBuilder.AddAttribute( fieldSequence++, "checked", value );
            fieldBuilder.AddAttribute( fieldSequence++, "onchange", EventCallback.Factory.Create<ChangeEventArgs>( this, valueChanged ) );
            fieldBuilder.CloseElement();
            fieldBuilder.AddContent( fieldSequence++, label );
            fieldBuilder.CloseElement();
        } ) );
        builder.CloseComponent();
    }

    private void RenderDesignerButton( RenderTreeBuilder builder, ref int sequence, string text, Func<Task> clicked )
    {
        builder.OpenComponent<Button>( sequence++ );
        builder.AddAttribute( sequence++, "Color", Color.Light );
        builder.AddAttribute( sequence++, "Clicked", EventCallback.Factory.Create<MouseEventArgs>( this, clicked ) );
        builder.AddAttribute( sequence++, "ChildContent", (RenderFragment)( buttonBuilder => buttonBuilder.AddContent( 0, text ) ) );
        builder.CloseComponent();
    }

    private void RenderTable( RenderTreeBuilder builder, ref int sequence, ReportElementDefinition element )
    {
        builder.OpenElement( sequence++, "table" );

        if ( element.Columns.Count == 0 )
        {
            builder.OpenElement( sequence++, "tr" );
            builder.OpenElement( sequence++, "td" );
            builder.CloseElement();
            builder.CloseElement();
        }
        else
        {
            builder.OpenElement( sequence++, "tr" );
            foreach ( var column in element.Columns )
            {
                builder.OpenElement( sequence++, "td" );
                builder.AddAttribute( sequence++, "style", $"width:{column.Width}px" );
                builder.AddContent( sequence++, column.Title ?? column.Field );
                builder.CloseElement();
            }
            builder.CloseElement();
        }

        builder.CloseElement();
    }

    public async Task ExecuteCommandAsync( ReportCommand command )
    {
        await ( command switch
        {
            ReportCommand.Design => SetModeAsync( ReportStudioMode.Design ),
            ReportCommand.Preview => SetPreviewAsync( SupportsPreviewFormat( currentPreviewFormat ) ? currentPreviewFormat : context.ViewerOptions.DefaultFormat ),
            ReportCommand.PreviewHtml => SetPreviewAsync( ReportPreviewFormat.Html ),
            ReportCommand.PreviewPdf => SetPreviewAsync( ReportPreviewFormat.Pdf ),
            ReportCommand.Cut => CutSelectedElementAsync(),
            ReportCommand.Copy => CopySelectedElementAsync(),
            ReportCommand.Paste => PasteElementAsync(),
            ReportCommand.Delete => DeleteSelectedElementAsync(),
            ReportCommand.Undo => UndoAsync(),
            ReportCommand.Redo => RedoAsync(),
            ReportCommand.Reset => ResetDefinitionAsync(),
            _ => SetPreviewAsync( ReportPreviewFormat.Html ),
        } );
    }

    public bool CanExecuteCommand( ReportCommand command )
    {
        var definition = EffectiveDefinition;

        return command switch
        {
            ReportCommand.Design => IsDesignerEnabled,
            ReportCommand.Preview => SupportsPreviewFormat( currentPreviewFormat ) || SupportsPreviewFormat( context.ViewerOptions.DefaultFormat ),
            ReportCommand.PreviewHtml => SupportsPreviewFormat( ReportPreviewFormat.Html ),
            ReportCommand.PreviewPdf => SupportsPreviewFormat( ReportPreviewFormat.Pdf ),
            ReportCommand.Cut or ReportCommand.Copy or ReportCommand.Delete => CurrentMode == ReportStudioMode.Design && FindSelectedElement( definition ) is not null,
            ReportCommand.Paste => CurrentMode == ReportStudioMode.Design && clipboardElement is not null && definition.Sections.Count > 0,
            ReportCommand.Undo => historyService.CanUndo,
            ReportCommand.Redo => historyService.CanRedo,
            _ => true,
        };
    }

    public Task<ReportState> GetState()
    {
        return Task.FromResult( CaptureReportState( EffectiveDefinition ) );
    }

    public async Task LoadState( ReportState state )
    {
        historyService.Clear();
        await ApplyReportStateAsync( state, notifyDefinitionChanged: true );
    }

    private async Task ExecuteDesignerCommandAsync( ReportDesignerCommand command )
    {
        if ( command is null )
            return;

        var beforeState = command.TrackHistory ? CaptureReportState( EffectiveDefinition ) : null;

        if ( command.Execute is not null )
            await command.Execute.Invoke();

        var definition = command.GetDefinition?.Invoke() ?? EffectiveDefinition;

        if ( command.TrackHistory )
        {
            var afterState = CaptureReportState( definition );
            var action = new ReportStateHistoryAction( command.Name, beforeState, afterState );
            historyService.Record( action );
            afterState.CanUndo = historyService.CanUndo;
            afterState.CanRedo = historyService.CanRedo;
            designerState.State = ReportContext.CloneState( afterState );
        }
        else
        {
            designerState.State = CaptureReportState( definition );
        }

        if ( command.NotifyDefinitionChanged && DefinitionChanged.HasDelegate )
            await DefinitionChanged.InvokeAsync( definition );

        await InvokeAsync( StateHasChanged );
    }

    private async Task SetModeAsync( ReportStudioMode mode )
    {
        await ExecuteDesignerCommandAsync( new( $"Set {mode} mode", async () =>
        {
            currentMode = mode;

            if ( ModeChanged.HasDelegate )
                await ModeChanged.InvokeAsync( currentMode );
        }, trackHistory: false, notifyDefinitionChanged: false ) );
    }

    private async Task SetPreviewAsync( ReportPreviewFormat format )
    {
        await ExecuteDesignerCommandAsync( new( $"Set {format} preview", async () =>
        {
            currentPreviewFormat = format;
            currentMode = ReportStudioMode.Preview;

            if ( ModeChanged.HasDelegate )
                await ModeChanged.InvokeAsync( currentMode );
        }, trackHistory: false, notifyDefinitionChanged: false ) );
    }

    private async Task ResetDefinitionAsync()
    {
        await ExecuteDesignerCommandAsync( new( "Reset report", () =>
        {
            declarativeDefinition = BuildDeclarativeDefinition();
            reportSelected = true;
            selectedElementKey = null;
            selectedSectionIndex = null;
            contextMenu = null;
            dragPreview = null;

            return Task.CompletedTask;
        }, () => declarativeDefinition ) );
    }

    private Task CopySelectedElementAsync()
    {
        return ExecuteDesignerCommandAsync( new( "Copy element", () =>
        {
            var element = FindSelectedElement( EffectiveDefinition );

            if ( element is not null )
                clipboardElement = ReportContext.CloneElement( element );

            return Task.CompletedTask;
        }, trackHistory: false, notifyDefinitionChanged: false ) );
    }

    private async Task CutSelectedElementAsync()
    {
        var definition = EffectiveDefinition;

        if ( !FindElementLocation( definition, selectedElementKey, out _, out _, out _ ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Cut element", () =>
        {
            var definition = EffectiveDefinition;

            if ( FindElementLocation( definition, selectedElementKey, out var sectionIndex, out var elementIndex, out var element ) )
            {
                clipboardElement = ReportContext.CloneElement( element );
                definition.Sections[sectionIndex].Elements.RemoveAt( elementIndex );
                selectedSectionIndex = sectionIndex;
                selectedElementKey = null;
                contextMenu = null;
            }

            return Task.CompletedTask;
        } ) );
    }

    private async Task PasteElementAsync()
    {
        if ( clipboardElement is null || ResolvePasteSectionIndex( EffectiveDefinition ) < 0 )
            return;

        await ExecuteDesignerCommandAsync( new( "Paste element", () =>
        {
            var definition = EffectiveDefinition;
            var targetSectionIndex = ResolvePasteSectionIndex( definition );

            var element = ReportContext.CloneElement( clipboardElement );
            element.Id = CreateDefinitionId();
            element.X = ApplyDesignerGrid( element.X + 16 );
            element.Y = ApplyDesignerGrid( element.Y + 16 );

            definition.Sections[targetSectionIndex].Elements.Add( element );

            reportSelected = false;
            selectedSectionIndex = null;
            selectedElementKey = GetDesignerElementKey( element );
            contextMenu = null;

            return Task.CompletedTask;
        } ) );
    }

    private async Task UndoAsync()
    {
        if ( !historyService.CanUndo )
            return;

        historyService.Undo( designerState );
        await ApplyReportStateAsync( designerState.State, notifyDefinitionChanged: true );
    }

    private async Task RedoAsync()
    {
        if ( !historyService.CanRedo )
            return;

        historyService.Redo( designerState );
        await ApplyReportStateAsync( designerState.State, notifyDefinitionChanged: true );
    }

    private ReportState CaptureReportState( ReportDefinition definition )
    {
        definition = EnsureDefinitionIds( definition );

        return new()
        {
            Definition = ReportContext.CloneDefinition( definition ),
            Mode = CurrentMode,
            PreviewFormat = CurrentPreviewFormat,
            SnapToGrid = snapToGrid,
            Selection = CaptureSelectionState( definition ),
            ClipboardElement = ReportContext.CloneElement( clipboardElement ),
            CanUndo = historyService.CanUndo,
            CanRedo = historyService.CanRedo,
        };
    }

    private ReportSelectionState CaptureSelectionState( ReportDefinition definition )
    {
        if ( FindElementLocation( definition, selectedElementKey, out var sectionIndex, out _, out var element ) )
        {
            return new()
            {
                Type = ReportSelectionType.Element,
                SectionId = definition.Sections[sectionIndex].Id,
                ElementId = element.Id,
            };
        }

        if ( selectedSectionIndex is not null
            && selectedSectionIndex.Value >= 0
            && selectedSectionIndex.Value < definition.Sections.Count )
        {
            return new()
            {
                Type = ReportSelectionType.Section,
                SectionId = definition.Sections[selectedSectionIndex.Value].Id,
            };
        }

        return new()
        {
            Type = ReportSelectionType.Report,
        };
    }

    private async Task ApplyReportStateAsync( ReportState state, bool notifyDefinitionChanged )
    {
        var nextState = ReportContext.CloneState( state );
        var definition = EnsureDefinitionIds( nextState.Definition ?? BuildDeclarativeDefinition() );

        declarativeDefinition = definition;
        currentMode = nextState.Mode;
        currentPreviewFormat = nextState.PreviewFormat;
        snapToGrid = nextState.SnapToGrid;
        clipboardElement = ReportContext.CloneElement( nextState.ClipboardElement );

        ApplySelectionState( definition, nextState.Selection );

        contextMenu = null;
        dragPreview = null;
        ClearDragState();

        designerState.State = CaptureReportState( definition );

        if ( notifyDefinitionChanged && DefinitionChanged.HasDelegate )
            await DefinitionChanged.InvokeAsync( definition );

        await InvokeAsync( StateHasChanged );
    }

    private void ApplySelectionState( ReportDefinition definition, ReportSelectionState selection )
    {
        reportSelected = true;
        selectedSectionIndex = null;
        selectedElementKey = null;

        if ( selection is null )
            return;

        if ( selection.Type == ReportSelectionType.Section )
        {
            var sectionIndex = definition.Sections.FindIndex( section => string.Equals( section.Id, selection.SectionId, StringComparison.Ordinal ) );

            if ( sectionIndex >= 0 )
            {
                reportSelected = false;
                selectedSectionIndex = sectionIndex;
            }

            return;
        }

        if ( selection.Type == ReportSelectionType.Element )
        {
            foreach ( var section in definition.Sections )
            {
                var element = section.Elements.FirstOrDefault( element => string.Equals( element.Id, selection.ElementId, StringComparison.Ordinal ) );

                if ( element is not null )
                {
                    reportSelected = false;
                    selectedElementKey = GetDesignerElementKey( element );
                    return;
                }
            }
        }
    }

    private void SelectReport()
    {
        reportSelected = true;
        selectedSectionIndex = null;
        selectedElementKey = null;
        contextMenu = null;
    }

    private void SelectElement( string key )
    {
        reportSelected = false;
        selectedElementKey = key;
        contextMenu = null;
    }

    private void SelectSection( int index )
    {
        reportSelected = false;
        selectedSectionIndex = index;
        selectedElementKey = null;
        contextMenu = null;
    }

    private void OpenSectionContextMenu( int sectionIndex, MouseEventArgs eventArgs )
    {
        reportSelected = false;
        selectedSectionIndex = sectionIndex;
        selectedElementKey = null;
        contextMenu = new()
        {
            Visible = true,
            Target = ReportContextMenuTarget.Section,
            SectionIndex = sectionIndex,
            ClientX = eventArgs.ClientX,
            ClientY = eventArgs.ClientY,
        };
    }

    private void OpenElementContextMenu( string elementKey, MouseEventArgs eventArgs )
    {
        reportSelected = false;
        selectedElementKey = elementKey;
        contextMenu = new()
        {
            Visible = true,
            Target = ReportContextMenuTarget.Element,
            ElementKey = elementKey,
            ClientX = eventArgs.ClientX,
            ClientY = eventArgs.ClientY,
        };
    }

    private void CloseContextMenu()
    {
        contextMenu = null;
    }

    private async Task MoveSelectedElementAsync( double x, double y, double width, double height )
    {
        var element = FindSelectedElement( EffectiveDefinition );

        if ( element is null )
            return;

        await ExecuteDesignerCommandAsync( new( "Move element", () =>
        {
            var element = FindSelectedElement( EffectiveDefinition );

            if ( element is not null )
            {
                element.X = Math.Max( 0, element.X + x );
                element.Y = Math.Max( 0, element.Y + y );
                element.Width = Math.Max( 8, element.Width + width );
                element.Height = Math.Max( 8, element.Height + height );
            }

            return Task.CompletedTask;
        } ) );
    }

    private async Task UpdateSelectedElementAsync( Action<ReportElementDefinition> update )
    {
        var element = FindSelectedElement( EffectiveDefinition );

        if ( element is null )
            return;

        await ExecuteDesignerCommandAsync( new( "Update element", () =>
        {
            var element = FindSelectedElement( EffectiveDefinition );

            if ( element is not null )
                update?.Invoke( element );

            return Task.CompletedTask;
        } ) );
    }

    private async Task UpdateSelectedSectionAsync( Action<ReportSectionDefinition> update )
    {
        var section = FindSelectedSection( EffectiveDefinition );

        if ( section is null )
            return;

        await ExecuteDesignerCommandAsync( new( "Update band", () =>
        {
            var section = FindSelectedSection( EffectiveDefinition );

            if ( section is not null )
                update?.Invoke( section );

            return Task.CompletedTask;
        } ) );
    }

    private async Task UpdateReportPageAsync( Action<ReportPageDefinition> update )
    {
        await ExecuteDesignerCommandAsync( new( "Update page", () =>
        {
            var definition = EffectiveDefinition;

            update?.Invoke( definition.Page );

            return Task.CompletedTask;
        } ) );
    }

    private async Task InsertSectionAsync( bool insertAfter )
    {
        var definition = EffectiveDefinition;
        var sourceSection = FindSelectedSection( definition );

        if ( sourceSection is null || selectedSectionIndex is null )
            return;

        await ExecuteDesignerCommandAsync( new( insertAfter ? "Insert band after" : "Insert band before", () =>
        {
            var definition = EffectiveDefinition;
            var sourceSection = FindSelectedSection( definition );

            if ( sourceSection is null || selectedSectionIndex is null )
                return Task.CompletedTask;

            var insertIndex = insertAfter ? selectedSectionIndex.Value + 1 : selectedSectionIndex.Value;
            var section = new ReportSectionDefinition
            {
                Name = CreateUniqueSectionName( definition, $"{GetSectionTypeDisplayName( sourceSection.Type )} band" ),
                Type = sourceSection.Type,
                Layout = sourceSection.Layout,
                Height = sourceSection.Height,
                DataSource = sourceSection.DataSource,
            };

            definition.Sections.Insert( insertIndex, section );

            selectedSectionIndex = insertIndex;
            selectedElementKey = null;
            contextMenu = null;

            return Task.CompletedTask;
        } ) );
    }

    private async Task DeleteSelectedElementAsync()
    {
        var definition = EffectiveDefinition;

        if ( !FindElementLocation( definition, selectedElementKey, out _, out _, out _ ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Delete element", () =>
        {
            var definition = EffectiveDefinition;

            if ( FindElementLocation( definition, selectedElementKey, out var sectionIndex, out var elementIndex, out _ ) )
            {
                definition.Sections[sectionIndex].Elements.RemoveAt( elementIndex );
                selectedSectionIndex = sectionIndex;
                selectedElementKey = null;
                contextMenu = null;
            }

            return Task.CompletedTask;
        } ) );
    }

    private void BeginFieldDrag( string dataSourceName, string fieldName )
    {
        draggedKind = ReportDesignerDragKind.Field;
        draggedDataSourceName = dataSourceName;
        draggedFieldName = fieldName;
        draggedElementType = null;
        draggedElementText = null;
        draggedElementKey = null;
        draggedElement = null;
        dragPreview = null;
        elementPointerDrag = null;
    }

    private void BeginToolboxElementDrag( ReportElementType elementType, string text )
    {
        draggedKind = ReportDesignerDragKind.ToolboxElement;
        draggedElementType = elementType;
        draggedElementText = text;
        draggedDataSourceName = null;
        draggedFieldName = null;
        draggedElementKey = null;
        draggedElement = null;
        dragPreview = null;
        elementPointerDrag = null;
    }

    private void BeginElementPointerDrag( string elementKey, PointerEventArgs eventArgs )
    {
        if ( !FindElementLocation( EffectiveDefinition, elementKey, out var sectionIndex, out _, out var element ) )
            return;

        draggedKind = ReportDesignerDragKind.Element;
        draggedElementKey = elementKey;
        draggedElement = element;
        draggedDataSourceName = null;
        draggedFieldName = null;
        draggedElementType = null;
        draggedElementText = null;
        dragPreview = null;
        elementPointerDrag = new()
        {
            ElementKey = elementKey,
            SourceSectionIndex = sectionIndex,
            TargetSectionIndex = sectionIndex,
            OriginalX = element.X,
            OriginalY = element.Y,
            StartClientX = eventArgs.ClientX,
            StartClientY = eventArgs.ClientY,
            GrabOffsetX = Math.Max( 0, eventArgs.OffsetX ),
            GrabOffsetY = Math.Max( 0, eventArgs.OffsetY ),
            TargetX = element.X,
            TargetY = element.Y,
        };

        SelectElement( elementKey );
    }

    private async Task PreviewElementPointerDragAsync( int targetSectionIndex, PointerEventArgs eventArgs )
    {
        if ( elementPointerDrag is null || draggedKind != ReportDesignerDragKind.Element )
            return;

        var preview = CreateElementPointerDragPreview( targetSectionIndex, eventArgs );

        if ( preview is null )
            return;

        var samePreviewPosition = dragPreview is not null
            && dragPreview.SectionIndex == preview.SectionIndex
            && Math.Abs( dragPreview.X - preview.X ) < .1
            && Math.Abs( dragPreview.Y - preview.Y ) < .1;

        if ( samePreviewPosition )
            return;

        var now = DateTime.UtcNow;

        if ( !snapToGrid
            && dragPreview is not null
            && dragPreview.SectionIndex == preview.SectionIndex
            && now - lastDragPreviewRenderTime < TimeSpan.FromMilliseconds( 16 ) )
        {
            return;
        }

        elementPointerDrag.TargetSectionIndex = targetSectionIndex;
        elementPointerDrag.TargetX = preview.X;
        elementPointerDrag.TargetY = preview.Y;
        elementPointerDrag.HasMoved = true;
        dragPreview = preview;
        lastDragPreviewRenderTime = now;

        await InvokeAsync( StateHasChanged );
    }

    private async Task CompleteElementPointerDragAsync( int targetSectionIndex, PointerEventArgs eventArgs )
    {
        if ( elementPointerDrag is null || draggedKind != ReportDesignerDragKind.Element )
            return;

        var pointerDrag = elementPointerDrag;
        var preview = CreateElementPointerDragPreview( targetSectionIndex, eventArgs ) ?? dragPreview;

        if ( preview is not null )
        {
            pointerDrag.TargetSectionIndex = preview.SectionIndex;
            pointerDrag.TargetX = preview.X;
            pointerDrag.TargetY = preview.Y;
        }

        var moved = pointerDrag.HasMoved
            && ( pointerDrag.TargetSectionIndex != pointerDrag.SourceSectionIndex
                || Math.Abs( pointerDrag.TargetX - pointerDrag.OriginalX ) > .1
                || Math.Abs( pointerDrag.TargetY - pointerDrag.OriginalY ) > .1 );

        var definition = EffectiveDefinition;
        var canMove = pointerDrag.TargetSectionIndex >= 0
            && pointerDrag.TargetSectionIndex < definition.Sections.Count
            && FindElementLocation( definition, pointerDrag.ElementKey, out _, out _, out _ );

        if ( !moved || !canMove )
        {
            ClearDragState();
            await InvokeAsync( StateHasChanged );
            return;
        }

        await ExecuteDesignerCommandAsync( new( "Move element", () =>
        {
            var definition = EffectiveDefinition;

            if ( !FindElementLocation( definition, pointerDrag.ElementKey, out var sourceSectionIndex, out var sourceElementIndex, out var element ) )
                return Task.CompletedTask;

            element.X = pointerDrag.TargetX;
            element.Y = pointerDrag.TargetY;

            if ( sourceSectionIndex != pointerDrag.TargetSectionIndex )
            {
                definition.Sections[sourceSectionIndex].Elements.RemoveAt( sourceElementIndex );
                definition.Sections[pointerDrag.TargetSectionIndex].Elements.Add( element );
            }

            selectedElementKey = GetDesignerElementKey( element );
            selectedSectionIndex = null;
            reportSelected = false;
            dragPreview = null;
            ClearDragState();

            return Task.CompletedTask;
        } ) );
    }

    private Task CancelElementPointerDragAsync()
    {
        if ( elementPointerDrag is null )
            return Task.CompletedTask;

        ClearDragState();

        return InvokeAsync( StateHasChanged );
    }

    private ReportDesignerDragPreview CreateElementPointerDragPreview( int targetSectionIndex, PointerEventArgs eventArgs )
    {
        if ( elementPointerDrag is null || draggedElement is null )
            return null;

        double x;
        double y;

        if ( targetSectionIndex == elementPointerDrag.SourceSectionIndex )
        {
            x = elementPointerDrag.OriginalX + eventArgs.ClientX - elementPointerDrag.StartClientX;
            y = elementPointerDrag.OriginalY + eventArgs.ClientY - elementPointerDrag.StartClientY;
        }
        else
        {
            x = eventArgs.OffsetX - elementPointerDrag.GrabOffsetX;
            y = eventArgs.OffsetY - elementPointerDrag.GrabOffsetY;
        }

        x = ApplyDesignerGrid( x );
        y = ApplyDesignerGrid( y );

        return CreateDragPreview( targetSectionIndex, draggedElement, x, y );
    }

    private async Task PreviewDesignerDragAsync( int targetSectionIndex, DragEventArgs eventArgs )
    {
        if ( draggedKind == ReportDesignerDragKind.None )
            return;

        var preview = CreateDragPreview( targetSectionIndex, ApplyDesignerGrid( eventArgs.OffsetX ), ApplyDesignerGrid( eventArgs.OffsetY ) );

        if ( preview is null )
            return;

        var samePreviewPosition = dragPreview is not null
            && dragPreview.SectionIndex == preview.SectionIndex
            && Math.Abs( dragPreview.X - preview.X ) < .1
            && Math.Abs( dragPreview.Y - preview.Y ) < .1;

        if ( samePreviewPosition )
        {
            return;
        }

        var now = DateTime.UtcNow;

        if ( !snapToGrid
            && dragPreview is not null
            && dragPreview.SectionIndex == preview.SectionIndex
            && now - lastDragPreviewRenderTime < TimeSpan.FromMilliseconds( 40 ) )
        {
            return;
        }

        dragPreview = preview;
        lastDragPreviewRenderTime = now;

        await InvokeAsync( StateHasChanged );
    }

    private async Task DropDesignerItemAsync( int targetSectionIndex, DragEventArgs eventArgs )
    {
        var definition = EffectiveDefinition;

        if ( targetSectionIndex < 0 || targetSectionIndex >= definition.Sections.Count )
            return;

        var x = ApplyDesignerGrid( eventArgs.OffsetX );
        var y = ApplyDesignerGrid( eventArgs.OffsetY );

        var commandName = draggedKind switch
        {
            ReportDesignerDragKind.Field when !string.IsNullOrWhiteSpace( draggedFieldName ) => "Add field",
            ReportDesignerDragKind.ToolboxElement when draggedElementType is not null => "Add element",
            ReportDesignerDragKind.Element when FindElementLocation( definition, draggedElementKey, out _, out _, out _ ) => "Move element",
            _ => null,
        };

        if ( commandName is null )
            return;

        await ExecuteDesignerCommandAsync( new( commandName, () =>
        {
            var definition = EffectiveDefinition;
            var targetSection = definition.Sections[targetSectionIndex];

            switch ( draggedKind )
            {
                case ReportDesignerDragKind.Field when !string.IsNullOrWhiteSpace( draggedFieldName ):
                    var fieldElement = new ReportElementDefinition
                    {
                        Name = draggedFieldName,
                        Type = ReportElementType.Field,
                        Field = draggedFieldName,
                        DataSource = draggedDataSourceName,
                        X = x,
                        Y = y,
                        Width = 160,
                        Height = 24,
                    };
                    targetSection.Elements.Add( fieldElement );
                    selectedElementKey = GetDesignerElementKey( fieldElement );
                    reportSelected = false;
                    break;
                case ReportDesignerDragKind.ToolboxElement when draggedElementType is not null:
                    var toolboxElement = CreateElementFromToolbox( draggedElementType.Value, draggedElementText, x, y );
                    targetSection.Elements.Add( toolboxElement );
                    selectedElementKey = GetDesignerElementKey( toolboxElement );
                    reportSelected = false;
                    break;
                case ReportDesignerDragKind.Element when FindElementLocation( definition, draggedElementKey, out var sourceSectionIndex, out var sourceElementIndex, out var element ):
                    definition.Sections[sourceSectionIndex].Elements.RemoveAt( sourceElementIndex );
                    element.X = x;
                    element.Y = y;
                    targetSection.Elements.Add( element );
                    selectedElementKey = GetDesignerElementKey( element );
                    reportSelected = false;
                    break;
            }

            selectedSectionIndex = null;
            dragPreview = null;
            ClearDragState();

            return Task.CompletedTask;
        } ) );
    }

    private ReportDesignerDragPreview CreateDragPreview( int targetSectionIndex, double x, double y )
    {
        return draggedKind switch
        {
            ReportDesignerDragKind.Field when !string.IsNullOrWhiteSpace( draggedFieldName ) => new()
            {
                SectionIndex = targetSectionIndex,
                ElementType = ReportElementType.Field,
                Text = FormatFieldExpression( draggedDataSourceName, draggedFieldName ),
                X = x,
                Y = y,
                Width = 160,
                Height = 24,
            },
            ReportDesignerDragKind.ToolboxElement when draggedElementType is not null => CreateDragPreview( targetSectionIndex, CreateElementFromToolbox( draggedElementType.Value, draggedElementText, x, y ) ),
            ReportDesignerDragKind.Element when draggedElement is not null => CreateDragPreview( targetSectionIndex, draggedElement, x, y ),
            _ => null,
        };
    }

    private static ReportDesignerDragPreview CreateDragPreview( int targetSectionIndex, ReportElementDefinition element, double? x = null, double? y = null )
    {
        return new()
        {
            SectionIndex = targetSectionIndex,
            ElementType = element.Type,
            Text = element.Type == ReportElementType.Field ? FormatFieldExpression( element ) : element.Text ?? element.Name ?? element.Type.ToString(),
            X = x ?? element.X,
            Y = y ?? element.Y,
            Width = Math.Max( 8, element.Width ),
            Height = Math.Max( 8, element.Height ),
        };
    }

    private Task ClearDesignerDragAsync()
    {
        var requiresRender = draggedKind != ReportDesignerDragKind.None || dragPreview is not null;

        ClearDragState();

        return requiresRender
            ? InvokeAsync( StateHasChanged )
            : Task.CompletedTask;
    }

    private IEnumerable<ReportDesignerDataSourceNode> ResolveDataSourceDictionary( ReportDefinition definition )
    {
        foreach ( var dataSource in definition.DataSources )
        {
            var fields = ResolveDataSourceFields( dataSource.Data ).ToList();

            if ( fields.Count == 0 )
                continue;

            yield return new()
            {
                Name = string.IsNullOrWhiteSpace( dataSource.Name ) ? DataSourceName : dataSource.Name,
                Fields = fields,
            };
        }
    }

    private static IEnumerable<ReportDesignerFieldNode> ResolveDataSourceFields( object data )
    {
        var item = ResolveSampleItem( data );

        if ( item is null )
            yield break;

        if ( item is IDictionary<string, object> dictionary )
        {
            foreach ( var key in dictionary.Keys.OrderBy( x => x ) )
            {
                yield return new()
                {
                    Name = key,
                    DataType = dictionary.TryGetValue( key, out var value ) ? value?.GetType() : null,
                };
            }

            yield break;
        }

        if ( item is IDictionary nonGenericDictionary )
        {
            foreach ( var key in nonGenericDictionary.Keys.OfType<object>().Select( key => new { Key = key, Name = Convert.ToString( key ) } ).Where( x => !string.IsNullOrWhiteSpace( x.Name ) ).OrderBy( x => x.Name ) )
            {
                yield return new()
                {
                    Name = key.Name,
                    DataType = nonGenericDictionary[key.Key]?.GetType(),
                };
            }

            yield break;
        }

        foreach ( var property in item.GetType().GetProperties( BindingFlags.Instance | BindingFlags.Public ).Where( x => x.CanRead && x.GetIndexParameters().Length == 0 ).OrderBy( x => x.Name ) )
        {
            yield return new()
            {
                Name = property.Name,
                DataType = property.PropertyType,
            };
        }
    }

    private static object ResolveSampleItem( object data )
    {
        if ( data is null || data is string )
            return data;

        if ( data is IEnumerable enumerable )
        {
            foreach ( var item in enumerable )
            {
                if ( item is not null )
                    return item;
            }

            return null;
        }

        return data;
    }

    private static double SnapToGrid( double value )
    {
        return Math.Max( 0, Math.Round( value / 8d ) * 8d );
    }

    private double ApplyDesignerGrid( double value )
    {
        return snapToGrid ? SnapToGrid( value ) : Math.Max( 0, value );
    }

    private void OnSnapToGridChanged( ChangeEventArgs eventArgs )
    {
        snapToGrid = eventArgs.Value is bool value
            ? value
            : string.Equals( Convert.ToString( eventArgs.Value, CultureInfo.InvariantCulture ), "true", StringComparison.OrdinalIgnoreCase );
    }

    private static ReportElementDefinition CreateElementFromToolbox( ReportElementType elementType, string text, double x, double y )
    {
        var definition = new ReportElementDefinition
        {
            Name = elementType.ToString(),
            Type = elementType,
            X = x,
            Y = y,
            Width = 160,
            Height = 24,
        };

        switch ( elementType )
        {
            case ReportElementType.Text:
                definition.Name = text;
                definition.Text = text;
                break;
            case ReportElementType.Image:
                definition.Height = 96;
                definition.Text = "Image";
                break;
            case ReportElementType.Line:
                definition.Height = 1;
                break;
            case ReportElementType.Rectangle:
                definition.Height = 64;
                break;
        }

        return definition;
    }

    private void ClearDragState()
    {
        draggedKind = ReportDesignerDragKind.None;
        draggedDataSourceName = null;
        draggedFieldName = null;
        draggedElementType = null;
        draggedElementText = null;
        draggedElementKey = null;
        draggedElement = null;
        dragPreview = null;
    }

    private static string FormatFieldExpression( ReportElementDefinition element )
    {
        if ( element is null || string.IsNullOrWhiteSpace( element.Field ) )
            return string.Empty;

        return FormatFieldExpression( element.DataSource, element.Field );
    }

    private static string FormatFieldExpression( string dataSourceName, string fieldName )
    {
        if ( string.IsNullOrWhiteSpace( fieldName ) )
            return string.Empty;

        var expression = string.IsNullOrWhiteSpace( dataSourceName )
            ? fieldName
            : $"{dataSourceName}.{fieldName}";

        return $"{{{expression}}}";
    }

    private ReportSectionDefinition FindSelectedSection( ReportDefinition definition )
    {
        if ( selectedSectionIndex is null || selectedSectionIndex < 0 || selectedSectionIndex >= definition.Sections.Count )
            return null;

        return definition.Sections[selectedSectionIndex.Value];
    }

    private static string CreateUniqueSectionName( ReportDefinition definition, string baseName )
    {
        if ( string.IsNullOrWhiteSpace( baseName ) )
            baseName = "Band copy";

        if ( definition.Sections.All( section => !string.Equals( section.Name, baseName, StringComparison.OrdinalIgnoreCase ) ) )
            return baseName;

        for ( var i = 2; ; i++ )
        {
            var candidate = $"{baseName} {i}";

            if ( definition.Sections.All( section => !string.Equals( section.Name, candidate, StringComparison.OrdinalIgnoreCase ) ) )
                return candidate;
        }
    }

    private static string GetSectionDisplayName( ReportSectionDefinition section )
    {
        return string.IsNullOrWhiteSpace( section.Name ) ? GetSectionTypeDisplayName( section.Type ) : section.Name;
    }

    private static string GetSectionTypeDisplayName( ReportSectionType type )
    {
        return type switch
        {
            ReportSectionType.Header => "Report Header",
            ReportSectionType.PageHeader => "Page Header",
            ReportSectionType.Detail => "Detail",
            ReportSectionType.Group => "Group Header",
            ReportSectionType.GroupHeader => "Group Header",
            ReportSectionType.GroupFooter => "Group Footer",
            ReportSectionType.PageFooter => "Page Footer",
            ReportSectionType.Footer => "Report Footer",
            _ => type.ToString(),
        };
    }

    private static ReportTreeNodeKind GetElementTreeNodeKind( ReportElementType type )
    {
        return type switch
        {
            ReportElementType.Field => ReportTreeNodeKind.Field,
            ReportElementType.Table => ReportTreeNodeKind.Table,
            ReportElementType.Image => ReportTreeNodeKind.Image,
            ReportElementType.Line => ReportTreeNodeKind.Line,
            ReportElementType.Rectangle => ReportTreeNodeKind.Rectangle,
            ReportElementType.PageBreak => ReportTreeNodeKind.PageBreak,
            _ => ReportTreeNodeKind.Text,
        };
    }

    private static string CreateSectionTreeNodeKey( int sectionIndex )
        => $"report:section:{sectionIndex}";

    private static string CreateElementTreeNodeKey( string elementKey )
        => $"report:element:{elementKey}";

    private static bool TryResolveSectionTreeNode( ReportTreeNode node, out int sectionIndex )
    {
        sectionIndex = -1;

        return node?.Key is not null
            && node.Key.StartsWith( "report:section:", StringComparison.Ordinal )
            && int.TryParse( node.Key["report:section:".Length..], NumberStyles.Integer, CultureInfo.InvariantCulture, out sectionIndex );
    }

    private static bool TryResolveElementTreeNode( ReportTreeNode node, out string elementKey )
    {
        elementKey = null;

        if ( node?.Key is null || !node.Key.StartsWith( "report:element:", StringComparison.Ordinal ) )
            return false;

        elementKey = node.Key["report:element:".Length..];

        return !string.IsNullOrWhiteSpace( elementKey );
    }

    private static string GetDataTypeDisplayName( Type dataType )
    {
        if ( dataType is null )
            return null;

        var nullableType = Nullable.GetUnderlyingType( dataType );

        return nullableType is null
            ? dataType.Name
            : $"{nullableType.Name}?";
    }

    private ReportElementDefinition FindSelectedElement( ReportDefinition definition )
    {
        if ( string.IsNullOrWhiteSpace( selectedElementKey ) )
            return null;

        return FindElementLocation( definition, selectedElementKey, out _, out _, out var element )
            ? element
            : null;
    }

    private int ResolvePasteSectionIndex( ReportDefinition definition )
    {
        if ( definition.Sections.Count == 0 )
            return -1;

        if ( selectedSectionIndex is not null
            && selectedSectionIndex.Value >= 0
            && selectedSectionIndex.Value < definition.Sections.Count )
            return selectedSectionIndex.Value;

        if ( FindElementLocation( definition, selectedElementKey, out var sectionIndex, out _, out _ ) )
            return sectionIndex;

        return 0;
    }

    private bool FindElementLocation( ReportDefinition definition, string key, out int sectionIndex, out int elementIndex, out ReportElementDefinition element )
    {
        sectionIndex = -1;
        elementIndex = -1;
        element = null;

        if ( string.IsNullOrWhiteSpace( key ) )
            return false;

        foreach ( var section in definition.Sections )
        {
            sectionIndex++;

            for ( var i = 0; i < section.Elements.Count; i++ )
            {
                if ( GetDesignerElementKey( section.Elements[i] ) == key )
                {
                    elementIndex = i;
                    element = section.Elements[i];
                    return true;
                }
            }
        }

        return false;
    }

    private string GetDesignerElementKey( ReportElementDefinition element )
    {
        if ( element is null )
            return null;

        if ( string.IsNullOrWhiteSpace( element.Id ) )
            element.Id = CreateDefinitionId();

        return element.Id;
    }

    private static ReportDefinition EnsureDefinitionIds( ReportDefinition definition )
    {
        if ( definition is null )
            return null;

        if ( string.IsNullOrWhiteSpace( definition.Id ) )
            definition.Id = CreateDefinitionId();

        foreach ( var dataSource in definition.DataSources )
        {
            if ( string.IsNullOrWhiteSpace( dataSource.Id ) )
                dataSource.Id = CreateDefinitionId();
        }

        foreach ( var section in definition.Sections )
        {
            if ( string.IsNullOrWhiteSpace( section.Id ) )
                section.Id = CreateDefinitionId();

            foreach ( var element in section.Elements )
            {
                if ( string.IsNullOrWhiteSpace( element.Id ) )
                    element.Id = CreateDefinitionId();

                foreach ( var column in element.Columns )
                {
                    if ( string.IsNullOrWhiteSpace( column.Id ) )
                        column.Id = CreateDefinitionId();
                }
            }
        }

        return definition;
    }

    private static string CreateDefinitionId()
        => Guid.NewGuid().ToString( "N" );

    private bool SupportsPreviewFormat( ReportPreviewFormat format )
    {
        return ( PreviewFormats ?? context.ViewerOptions.PreviewFormats ).HasFlag( format );
    }

    private bool IsDesignerEnabled => DesignerEnabled || GlobalOptions.DesignerEnabled;

    private ReportOptions GlobalOptions => globalOptions ??= ServiceProvider.GetService<ReportOptions>() ?? new();

    private ReportStudioMode CurrentMode => Mode ?? currentMode;

    private ReportPreviewFormat CurrentPreviewFormat => PreviewFormat ?? currentPreviewFormat;

    private string DataSourceName => "Default";

    private enum ReportDesignerDragKind
    {
        None,
        Field,
        ToolboxElement,
        Element
    }

    private enum ReportDesignerPanelTab
    {
        Properties,
        Explorer
    }

    private enum ReportContextMenuTarget
    {
        Section,
        Element
    }

    private sealed class ReportContextMenuState
    {
        public bool Visible { get; set; }

        public ReportContextMenuTarget Target { get; set; }

        public int SectionIndex { get; set; } = -1;

        public string ElementKey { get; set; }

        public double ClientX { get; set; }

        public double ClientY { get; set; }
    }

    private sealed class ReportDesignerDragPreview
    {
        public int SectionIndex { get; set; }

        public ReportElementType ElementType { get; set; }

        public string Text { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }
    }

    private sealed class ReportElementPointerDragState
    {
        public string ElementKey { get; set; }

        public int SourceSectionIndex { get; set; }

        public int TargetSectionIndex { get; set; }

        public double OriginalX { get; set; }

        public double OriginalY { get; set; }

        public double StartClientX { get; set; }

        public double StartClientY { get; set; }

        public double GrabOffsetX { get; set; }

        public double GrabOffsetY { get; set; }

        public double TargetX { get; set; }

        public double TargetY { get; set; }

        public bool HasMoved { get; set; }
    }

    private sealed record ReportToolboxTreeNodeValue( ReportElementType ElementType, string Text );

    private sealed record ReportFieldTreeNodeValue( string DataSourceName, string FieldName );

    private sealed class ReportDesignerDataSourceNode
    {
        public string Name { get; set; }

        public List<ReportDesignerFieldNode> Fields { get; set; } = [];
    }

    private sealed class ReportDesignerFieldNode
    {
        public string Name { get; set; }

        public Type DataType { get; set; }
    }

    [Parameter] public ReportDefinition Definition { get; set; }

    [Parameter] public EventCallback<ReportDefinition> DefinitionChanged { get; set; }

    [Parameter] public IEnumerable<TItem> Data { get; set; }

    [Parameter] public ReportPageDefinition Page { get; set; }

    [Parameter] public bool DesignerEnabled { get; set; }

    [Parameter] public bool ShowToolbar { get; set; } = true;

    [Parameter] public ReportDefinitionMode DefinitionMode { get; set; } = ReportDefinitionMode.SeedWhenEmpty;

    [Parameter] public ReportStudioMode? Mode { get; set; }

    [Parameter] public EventCallback<ReportStudioMode> ModeChanged { get; set; }

    [Parameter] public ReportPreviewFormat? PreviewFormat { get; set; }

    [Parameter] public ReportPreviewFormat? PreviewFormats { get; set; }

    [Parameter] public ReportPreviewFormat? DefaultPreviewFormat { get; set; }

    [Parameter] public RenderFragment ChildContent { get; set; }
}