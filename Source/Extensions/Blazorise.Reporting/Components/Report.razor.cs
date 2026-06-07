#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Extensions;
using Blazorise.History;
using Blazorise.Reporting.Internal;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Provides a declarative report designer and viewer for band-based report definitions.
/// </summary>
/// <typeparam name="TItem">Data item type used by the default report data source.</typeparam>
public partial class Report<TItem> : ComponentBase, IReportCommandExecutor, IAsyncDisposable
{
    #region Members

    private const double DesignerBandRailWidth = 128;

    private const double DesignerCollapsedBandHeight = 28;

    private readonly ReportContext context = new();

    private readonly ReportToolbarContext toolbarContext;

    private readonly ReportDesignerState designerState = new();

    private readonly HistoryManager<ReportDesignerState> historyService = new();

    private readonly HashSet<string> collapsedSectionIds = new( StringComparer.Ordinal );

    private DotNetObjectReference<Report<TItem>> dotNetObjectReference;

    private ReportDefinition declarativeDefinition;

    private ReportStudioMode currentMode;

    private ReportPreviewFormat currentPreviewFormat;

    private ReportDesignerPanelTab selectedDesignerPanelTab = ReportDesignerPanelTab.Properties;

    private ReportContextMenuState contextMenu;

    private bool reportSelected = true;

    private string selectedElementKey;

    private readonly HashSet<string> selectedElementKeys = new( StringComparer.Ordinal );

    private int? selectedSectionIndex;

    private bool suppressNextSectionClick;

    private string suppressNextElementClickKey;

    private DateTime suppressSelectionClickUntil;

    private bool snapToGrid = true;

    private ReportDesignerDragKind draggedKind;

    private string draggedDataSourceName;

    private string draggedFieldName;

    private ReportElementType? draggedElementType;

    private string draggedElementText;

    private string draggedElementKey;

    private ReportElementDefinition draggedElement;

    private ReportDesignerDragPreview dragPreview;

    private ReportDesignerSelectionBox selectionBox;

    private ReportElementPointerDragState elementPointerDrag;

    private ReportElementPointerResizeState elementPointerResize;

    private ReportSectionPointerResizeState sectionPointerResize;

    private DateTime lastDragPreviewRenderTime;

    private int designerSurfaceVersion;

    private ReportElementDefinition clipboardElement;

    private ReportOptions globalOptions;

    private IJSObjectReference reportingModule;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new report component instance.
    /// </summary>
    public Report()
    {
        toolbarContext = new( this );
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        context.ViewerOptions.PreviewFormats = GlobalOptions.PreviewFormats;
        context.ViewerOptions.DefaultFormat = GlobalOptions.DefaultPreviewFormat;
        context.ViewerOptions.AllowPrint = GlobalOptions.AllowPrint;
        context.ViewerOptions.AllowDownload = GlobalOptions.AllowDownload;

        currentMode = IsDesignerEnabled ? ReportStudioMode.Design : ReportStudioMode.Preview;
        currentPreviewFormat = DefaultPreviewFormat ?? context.ViewerOptions.DefaultFormat;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if ( reportingModule is not null )
        {
            try
            {
                await reportingModule.InvokeVoidAsync( "stopSectionResize" );
                await reportingModule.DisposeAsync();
            }
            catch ( JSDisconnectedException )
            {
            }
        }

        dotNetObjectReference?.Dispose();
    }

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
            (var width, var height) = page.Size == ReportPageSize.Letter ? (816d, 1056d) : (794d, 1123d);

            if ( page.Orientation == ReportOrientation.Landscape )
            {
                (width, height) = (height, width);
            }

            page.Width = width;
            page.Height = height;
        }

        return page;
    }

    private IEnumerable<object> ResolveItems( ReportDefinition definition, string dataSource = null, object currentItem = null )
    {
        var source = ResolveDataSourceValue( definition, dataSource, currentItem );

        if ( source is IEnumerable enumerable and not string )
        {
            foreach ( var item in enumerable )
            {
                yield return item;
            }

            yield break;
        }

        if ( source is not null )
            yield return source;
    }

    private object ResolveDataSourceValue( ReportDefinition definition, string dataSource = null, object currentItem = null )
    {
        if ( string.IsNullOrWhiteSpace( dataSource ) )
            return currentItem ?? definition?.DataSources.FirstOrDefault()?.Data ?? Data;

        var trimmedDataSource = dataSource.Trim();
        var namedDataSource = definition?.DataSources.FirstOrDefault( x => string.Equals( x.Name, trimmedDataSource, StringComparison.OrdinalIgnoreCase ) );

        if ( namedDataSource is not null )
            return namedDataSource.Data;

        var pathSeparatorIndex = trimmedDataSource.IndexOf( '.', StringComparison.Ordinal );

        if ( pathSeparatorIndex > 0 )
        {
            var dataSourceName = trimmedDataSource[..pathSeparatorIndex];
            var dataSourcePath = trimmedDataSource[( pathSeparatorIndex + 1 )..];
            namedDataSource = definition?.DataSources.FirstOrDefault( x => string.Equals( x.Name, dataSourceName, StringComparison.OrdinalIgnoreCase ) );

            if ( namedDataSource is not null )
            {
                return ResolvePathValue( namedDataSource.Data, dataSourcePath );
            }
        }

        if ( currentItem is not null )
        {
            var relativeValue = ResolvePathValue( currentItem, trimmedDataSource );

            if ( relativeValue is not null )
                return relativeValue;
        }

        return ResolvePathValue( definition?.DataSources.FirstOrDefault()?.Data ?? Data, trimmedDataSource );
    }

    private object ResolveFieldValue( object item, string field )
    {
        return ResolvePathValue( item, field );
    }

    private static object ResolvePathValue( object item, string path )
    {
        if ( item is null || string.IsNullOrWhiteSpace( path ) )
            return null;

        var current = item;

        foreach ( var segment in path.Split( '.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries ) )
        {
            current = ResolvePathSegment( current, segment );

            if ( current is null )
                return null;
        }

        return current;
    }

    private static object ResolvePathSegment( object item, string segment )
    {
        if ( item is null || string.IsNullOrWhiteSpace( segment ) )
            return null;

        if ( item is IEnumerable enumerable and not string and not IDictionary )
        {
            var values = new List<object>();

            foreach ( var childItem in enumerable )
            {
                var value = ResolvePathSegment( childItem, segment );

                if ( value is IEnumerable childEnumerable and not string and not IDictionary )
                {
                    values.AddRange( childEnumerable.Cast<object>() );
                }
                else if ( value is not null )
                {
                    values.Add( value );
                }
            }

            return values;
        }

        if ( item is IDictionary<string, object> dictionary )
        {
            var key = dictionary.Keys.FirstOrDefault( x => string.Equals( x, segment, StringComparison.OrdinalIgnoreCase ) );

            return key is not null && dictionary.TryGetValue( key, out var dictionaryValue )
                ? dictionaryValue
                : null;
        }

        if ( item is IDictionary nonGenericDictionary )
        {
            foreach ( var key in nonGenericDictionary.Keys )
            {
                if ( string.Equals( Convert.ToString( key, CultureInfo.InvariantCulture ), segment, StringComparison.OrdinalIgnoreCase ) )
                    return nonGenericDictionary[key];
            }

            return null;
        }

        var property = item.GetType().GetProperty( segment, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase );

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

        builder.OpenElement( "div" );
        builder.Class( "b-report-designer" );

        builder.OpenComponent<Div>();
        builder.Class( "b-report-designer-dictionary" );
        builder.Attribute( "Padding", Padding.Is3 );
        builder.Attribute( "Background", Background.White );
        builder.Attribute( "Border", Border.Is1 );
        builder.Attribute( "Overflow", Overflow.Auto );
        builder.Attribute( "ChildContent", (RenderFragment)( childBuilder =>
        {
            RenderDataDictionary( childBuilder, definition );
        } ) );
        builder.CloseComponent();

        builder.OpenComponent<Div>();
        builder.Class( "b-report-designer-surface" );
        builder.Attribute( "Padding", Padding.Is3 );
        builder.Attribute( "Background", Background.Light );
        builder.Attribute( "Border", Border.Is1 );
        builder.Attribute( "Overflow", Overflow.Auto );
        builder.Attribute( "ChildContent", (RenderFragment)( childBuilder =>
        {
            RenderReportPage( childBuilder, definition, designMode: true );
        } ) );
        builder.CloseComponent();

        builder.OpenComponent<Div>();
        builder.Class( "b-report-designer-panel" );
        builder.Attribute( "Padding", Padding.Is3 );
        builder.Attribute( "Background", Background.White );
        builder.Attribute( "Border", Border.Is1 );
        builder.Attribute( "ChildContent", (RenderFragment)( childBuilder =>
        {
            RenderDesignerPanel( childBuilder, definition );
        } ) );
        builder.CloseComponent();
        RenderDesignerContextMenu( builder, definition );

        builder.CloseElement();
    };

    private void RenderDesignerPanel( RenderTreeBuilder builder, ReportDefinition definition )
    {
        RenderDesignerPanelTabs( builder );

        builder.OpenElement( "div" );
        builder.Class( "b-report-designer-panel-body" );

        if ( selectedDesignerPanelTab == ReportDesignerPanelTab.Explorer )
        {
            RenderReportExplorer( builder, definition );
        }
        else
        {
            RenderPropertiesPanel( builder, definition );
        }

        builder.CloseElement();
    }

    private void RenderDesignerPanelTabs( RenderTreeBuilder builder )
    {
        builder.OpenComponent<Div>();
        builder.Class( "b-report-designer-tabs" );
        builder.Attribute( "ChildContent", (RenderFragment)( childBuilder =>
        {
            RenderDesignerPanelTabButton( childBuilder, "Properties", ReportDesignerPanelTab.Properties );
            RenderDesignerPanelTabButton( childBuilder, "Report Explorer", ReportDesignerPanelTab.Explorer );
        } ) );
        builder.CloseComponent();
    }

    private void RenderDesignerPanelTabButton( RenderTreeBuilder builder, string text, ReportDesignerPanelTab tab )
    {
        builder.OpenComponent<Button>();
        builder.Attribute( "Color", selectedDesignerPanelTab == tab ? Color.Primary : Color.Light );
        builder.Attribute( "Clicked", EventCallback.Factory.Create<MouseEventArgs>( this, () => selectedDesignerPanelTab = tab ) );
        builder.Attribute( "ChildContent", (RenderFragment)( buttonBuilder => buttonBuilder.Content( text ) ) );
        builder.CloseComponent();
    }

    private void RenderPropertiesPanel( RenderTreeBuilder builder, ReportDefinition definition )
    {
        var hasSelection = reportSelected || selectedSectionIndex is not null || !string.IsNullOrWhiteSpace( selectedElementKey );

        RenderReportProperties( builder, definition );
        RenderSelectedSectionProperties( builder, definition );
        RenderSelectedElementProperties( builder, definition );
        RenderSelectedElementTools( builder, definition );

        if ( hasSelection )
            return;

        builder.OpenComponent<Paragraph>();
        builder.Attribute( "TextColor", TextColor.Secondary );
        builder.Attribute( "ChildContent", (RenderFragment)( paragraphBuilder => paragraphBuilder.Content( "Select a band or report element to edit its properties." ) ) );
        builder.CloseComponent();
    }

    private void RenderReportProperties( RenderTreeBuilder builder, ReportDefinition definition )
    {
        if ( !reportSelected )
            return;

        builder.OpenComponent<Div>();
        builder.Attribute( "Margin", Margin.Is3.FromBottom );
        builder.Attribute( "ChildContent", (RenderFragment)( childBuilder =>
        {
            childBuilder.OpenElement( "h6" );
            childBuilder.Content( "Report properties" );
            childBuilder.CloseElement();

            RenderDesignerCheckbox( childBuilder, "Snap to grid", snapToGrid, OnSnapToGridChanged );
            RenderDesignerNumberInput( childBuilder, "Page width", definition.Page.Width, value => UpdateReportPageAsync( page => page.Width = Math.Max( 1, value ) ) );
            RenderDesignerNumberInput( childBuilder, "Page height", definition.Page.Height, value => UpdateReportPageAsync( page => page.Height = Math.Max( 1, value ) ) );
        } ) );
        builder.CloseComponent();
    }

    private void RenderReportExplorer( RenderTreeBuilder builder, ReportDefinition definition )
    {
        builder.OpenElement( "h5" );
        builder.Content( "Report Explorer" );
        builder.CloseElement();

        builder.OpenComponent<_ReportTreeView>();
        builder.Attribute( "Nodes", BuildReportExplorerNodes( definition ) );
        builder.Attribute( "NodeClicked", EventCallback.Factory.Create<ReportTreeNode>( this, OnReportTreeNodeClicked ) );
        builder.Attribute( "NodeContextMenu", EventCallback.Factory.Create<ReportTreeNodeMouseEventArgs>( this, OnReportTreeNodeContextMenu ) );
        builder.CloseComponent();
    }

    private void RenderDesignerContextMenu( RenderTreeBuilder builder, ReportDefinition definition )
    {
        if ( contextMenu is null || !contextMenu.Visible )
            return;

        builder.OpenElement( "div" );
        builder.Class( "b-report-context-menu" );
        builder.Style( $"left:{contextMenu.ClientX}px;top:{contextMenu.ClientY}px;" );

        switch ( contextMenu.Target )
        {
            case ReportContextMenuTarget.Section when contextMenu.SectionIndex >= 0 && contextMenu.SectionIndex < definition.Sections.Count:
                var section = definition.Sections[contextMenu.SectionIndex];
                if ( !section.Suppressed )
                {
                    RenderContextMenuButton( builder, "Insert band before", () => InsertSectionAsync( insertAfter: false ) );
                    RenderContextMenuButton( builder, "Insert band after", () => InsertSectionAsync( insertAfter: true ) );
                    RenderContextMenuButton( builder, "Suppress", ToggleSelectedSectionSuppressionAsync );

                    if ( CanDeleteSection( section ) )
                    {
                        RenderContextMenuButton( builder, "Delete band", DeleteSelectedSectionAsync );
                    }
                }
                else
                {
                    RenderContextMenuButton( builder, "Don't suppress", ToggleSelectedSectionSuppressionAsync );
                }
                break;
            case ReportContextMenuTarget.Element:
                RenderContextMenuButton( builder, "Delete element", DeleteSelectedElementAsync );
                break;
        }

        RenderContextMenuButton( builder, "Close", () =>
        {
            CloseContextMenu();
            return Task.CompletedTask;
        } );

        builder.CloseElement();
    }

    private void RenderContextMenuButton( RenderTreeBuilder builder, string text, Func<Task> clicked )
    {
        builder.OpenElement( "button" );
        builder.Type( "button" );
        builder.Attribute( "onclick", EventCallback.Factory.Create<MouseEventArgs>( this, clicked ) );
        builder.Content( text );
        builder.CloseElement();
    }

    private void RenderDataDictionary( RenderTreeBuilder builder, ReportDefinition definition )
    {
        var dataSources = ResolveDataSourceDictionary( definition ).ToList();

        builder.OpenElement( "h5" );
        builder.Content( "Toolbox" );
        builder.CloseElement();

        builder.OpenComponent<_ReportTreeView>();
        builder.Attribute( "Nodes", BuildToolboxNodes() );
        builder.Attribute( "NodeDragStarted", EventCallback.Factory.Create<ReportTreeNodeDragEventArgs>( this, OnToolboxTreeNodeDragStarted ) );
        builder.Attribute( "NodeDragEnded", EventCallback.Factory.Create<ReportTreeNode>( this, _ => ClearDesignerDragAsync() ) );
        builder.CloseComponent();

        builder.OpenElement( "h5" );
        builder.Class( "b-report-dictionary-title" );
        builder.Content( "Fields explorer" );
        builder.CloseElement();

        if ( dataSources.Count == 0 )
        {
            builder.OpenComponent<Paragraph>();
            builder.Attribute( "TextColor", TextColor.Secondary );
            builder.Attribute( "ChildContent", (RenderFragment)( paragraphBuilder => paragraphBuilder.Content( "No data source fields." ) ) );
            builder.CloseComponent();
            return;
        }

        builder.OpenComponent<_ReportTreeView>();
        builder.Attribute( "Nodes", BuildFieldsExplorerNodes( dataSources ) );
        builder.Attribute( "NodeDragStarted", EventCallback.Factory.Create<ReportTreeNodeDragEventArgs>( this, OnFieldsTreeNodeDragStarted ) );
        builder.Attribute( "NodeDragEnded", EventCallback.Factory.Create<ReportTreeNode>( this, _ => ClearDesignerDragAsync() ) );
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
                    Children = dataSource.Fields.Select( field => BuildFieldExplorerNode( dataSource.Name, field ) ).ToList(),
                } ).ToList(),
            }
        ];
    }

    private static ReportTreeNode BuildFieldExplorerNode( string dataSourceName, ReportDesignerFieldNode field )
    {
        var hasChildren = field.Children.Count > 0;

        return new()
        {
            Key = $"fields:field:{dataSourceName}:{field.Path}",
            Text = field.Name,
            Detail = hasChildren ? null : GetDataTypeDisplayName( field.DataType ),
            Kind = hasChildren ? ReportTreeNodeKind.Folder : ReportTreeNodeKind.Field,
            Draggable = !hasChildren,
            Value = !hasChildren ? new ReportFieldTreeNodeValue( dataSourceName, field.Path ) : null,
            Children = field.Children.Select( child => BuildFieldExplorerNode( dataSourceName, child ) ).ToList(),
        };
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
                            Selected = IsElementSelected( elementKey ),
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
            if ( FindElementLocation( EffectiveDefinition, elementKey, out var elementSectionIndex, out _, out _ )
                && EffectiveDefinition.Sections[elementSectionIndex].Suppressed )
            {
                SelectSection( elementSectionIndex );
            }
            else
            {
                SelectElement( elementKey );
            }
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

        builder.OpenComponent<Div>();
        builder.Class( "b-report-viewer" );
        builder.Attribute( "Padding", Padding.Is3 );
        builder.Attribute( "Background", Background.Light );
        builder.Attribute( "Border", Border.Is1 );
        builder.Attribute( "Overflow", Overflow.Auto );
        builder.Attribute( "ChildContent", (RenderFragment)( childBuilder =>
        {
            if ( CurrentPreviewFormat == ReportPreviewFormat.Pdf )
            {
                childBuilder.OpenComponent<Div>();
                childBuilder.Attribute( "Padding", Padding.Is5 );
                childBuilder.Attribute( "Border", Border.Is1 );
                childBuilder.Attribute( "Background", Background.White );
                childBuilder.Attribute( "TextAlignment", TextAlignment.Center );
                childBuilder.Attribute( "ChildContent", (RenderFragment)( placeholderBuilder => placeholderBuilder.Content( "PDF preview is configured for this report. A PDF renderer can feed Blazorise.PdfViewer in the next implementation step." ) ) );
                childBuilder.CloseComponent();
            }
            else
            {
                RenderReportPage( childBuilder, definition, designMode: false );
            }
        } ) );

        builder.CloseComponent();
    };

    private void RenderReportPage( RenderTreeBuilder builder, ReportDefinition definition, bool designMode )
    {
        var pageWidth = designMode && BandMode == ReportBandMode.Rail
            ? definition.Page.Width + DesignerBandRailWidth
            : definition.Page.Width;

        builder.OpenElement( "div" );
        builder.Key( designMode ? $"{definition.Id}:{designerSurfaceVersion}" : definition.Id );
        builder.Class( designMode ? "b-report-page b-report-page-design" : "b-report-page" );
        builder.Style( $"width:{pageWidth}px;min-height:{definition.Page.Height}px;" );

        if ( designMode )
        {
            builder.Attribute( "onpointermove", EventUtil.AsNonRenderingEventHandler<PointerEventArgs>( PreviewPageSelectionBoxAsync ) );
            builder.EventPreventDefault( "onpointermove", true );
            builder.Attribute( "onpointerup", EventCallback.Factory.Create<PointerEventArgs>( this, CompletePageSelectionBoxAsync ) );
            builder.EventPreventDefault( "onpointerup", true );
            builder.Attribute( "onpointercancel", EventCallback.Factory.Create<PointerEventArgs>( this, _ => CancelPageSelectionBoxAsync() ) );
        }

        for ( var sectionIndex = 0; sectionIndex < definition.Sections.Count; sectionIndex++ )
        {
            RenderSection( builder, definition, definition.Sections[sectionIndex], sectionIndex, designMode );
        }

        if ( designMode && selectionBox is not null )
        {
            RenderDesignerSelectionBox( builder, selectionBox, BandMode == ReportBandMode.Rail ? DesignerBandRailWidth : 0 );
        }

        builder.CloseElement();
    }

    private void RenderSection( RenderTreeBuilder builder, ReportDefinition definition, ReportSectionDefinition section, int sectionIndex, bool designMode )
    {
        if ( section.Suppressed && !designMode )
            return;

        var items = !designMode && section.Type == ReportSectionType.Detail
            ? ResolveItems( definition, section.DataSource ).ToList()
            : new List<object> { ResolveItems( definition, section.DataSource ).FirstOrDefault() };

        if ( items.Count == 0 )
            items.Add( null );

        for ( var itemIndex = 0; itemIndex < items.Count; itemIndex++ )
        {
            var item = items[itemIndex];
            var railVisible = designMode && BandMode == ReportBandMode.Rail;
            var collapsed = designMode && railVisible && !section.Suppressed && IsSectionCollapsed( section );
            var sectionHeight = collapsed ? DesignerCollapsedBandHeight : GetDesignerSectionHeight( sectionIndex, section );
            var sectionClass = $"b-report-section {section.Class} {( designMode && selectedSectionIndex == sectionIndex && string.IsNullOrWhiteSpace( selectedElementKey ) ? "active" : string.Empty )} {( collapsed ? "collapsed" : string.Empty )} {( section.Suppressed ? "suppressed" : string.Empty )}".Trim();

            builder.OpenElement( "section" );
            builder.Key( designMode ? section.Id : $"{section.Id}:{itemIndex}" );
            builder.Class( sectionClass );
            builder.Style( $"height:{sectionHeight}px;{section.Style}" );

            if ( designMode )
            {
                if ( railVisible )
                {
                    RenderSectionRail( builder, section, sectionIndex, collapsed );
                }
                else
                {
                    RenderSectionLabel( builder, section );
                }
            }

            if ( !collapsed )
            {
                if ( designMode )
                {
                    builder.OpenElement( "div" );
                    builder.Class( $"{( railVisible ? "b-report-section-body b-report-section-body-rail" : "b-report-section-body" )} {( section.Suppressed ? "disabled" : string.Empty )}".Trim() );
                    builder.Style( railVisible ? $"left:{DesignerBandRailWidth}px;width:{definition.Page.Width}px;" : null );

                    if ( !section.Suppressed )
                    {
                        builder.Attribute( "onpointerdown", EventCallback.Factory.Create<PointerEventArgs>( this, eventArgs => BeginSelectionBox( sectionIndex, eventArgs ) ) );
                        builder.EventPreventDefault( "onpointerdown", true );
                        builder.Attribute( "ondragover", EventUtil.AsNonRenderingEventHandler<DragEventArgs>( eventArgs => PreviewDesignerDragAsync( sectionIndex, eventArgs ) ) );
                        builder.EventPreventDefault( "ondragover", true );
                        builder.Attribute( "ondrop", EventCallback.Factory.Create<DragEventArgs>( this, eventArgs => DropDesignerItemAsync( sectionIndex, eventArgs ) ) );
                        builder.EventPreventDefault( "ondrop", true );
                        builder.Attribute( "onpointermove", EventUtil.AsNonRenderingEventHandler<PointerEventArgs>( eventArgs => PreviewElementPointerInteractionAsync( sectionIndex, eventArgs ) ) );
                        builder.EventPreventDefault( "onpointermove", true );
                        builder.Attribute( "onpointerup", EventCallback.Factory.Create<PointerEventArgs>( this, eventArgs => CompleteElementPointerInteractionAsync( sectionIndex, eventArgs ) ) );
                        builder.EventPreventDefault( "onpointerup", true );
                        builder.Attribute( "onpointercancel", EventCallback.Factory.Create<PointerEventArgs>( this, _ => CancelElementPointerInteractionAsync() ) );
                        builder.Attribute( "onclick", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandleSectionClick( sectionIndex ) ) );
                        builder.Attribute( "oncontextmenu", EventCallback.Factory.Create<MouseEventArgs>( this, eventArgs => OpenSectionContextMenu( sectionIndex, eventArgs ) ) );
                        builder.EventPreventDefault( "oncontextmenu", true );
                    }
                }

                for ( var i = 0; i < section.Elements.Count; i++ )
                {
                    var element = section.Elements[i];
                    var key = GetDesignerElementKey( element );

                    RenderElement( builder, item, element, designMode, !section.Suppressed, key );
                }

                if ( designMode && !section.Suppressed && dragPreview?.SectionIndex == sectionIndex )
                {
                    RenderDesignerDragPreview( builder, dragPreview );
                }

                if ( designMode && !section.Suppressed )
                {
                    RenderSectionResizeHandle( builder, sectionIndex );
                }

                if ( designMode )
                {
                    builder.CloseElement();
                }
            }

            builder.CloseElement();
        }
    }

    private void RenderSectionRail( RenderTreeBuilder builder, ReportSectionDefinition section, int sectionIndex, bool collapsed )
    {
        builder.OpenElement( "div" );
        builder.Class( $"b-report-section-rail b-report-section-rail-{section.Type.ToString().ToLowerInvariant()}" );
        builder.Attribute( "onclick", EventCallback.Factory.Create<MouseEventArgs>( this, () => SelectSection( sectionIndex ) ) );
        builder.Attribute( "oncontextmenu", EventCallback.Factory.Create<MouseEventArgs>( this, eventArgs => OpenSectionContextMenu( sectionIndex, eventArgs ) ) );
        builder.EventPreventDefault( "oncontextmenu", true );

        builder.OpenElement( "button" );
        builder.Type( "button" );
        builder.Class( "b-report-section-rail-toggle" );
        builder.Attribute( "title", section.Suppressed ? "Band is suppressed" : collapsed ? "Expand band" : "Collapse band" );
        builder.Attribute( "disabled", !AllowBandCollapse || section.Suppressed );
        builder.Attribute( "onclick", EventCallback.Factory.Create<MouseEventArgs>( this, () => ToggleSectionCollapsed( section ) ) );
        builder.EventStopPropagation( "onclick", true );
        builder.Content( section.Suppressed ? "!" : collapsed ? "+" : "-" );
        builder.CloseElement();

        builder.OpenElement( "div" );
        builder.Class( "b-report-section-rail-text" );

        builder.OpenElement( "span" );
        builder.Class( "b-report-section-rail-title" );
        builder.Content( GetSectionDisplayName( section ) );
        builder.CloseElement();

        if ( ShowBandDataSource && !string.IsNullOrWhiteSpace( section.DataSource ) )
        {
            builder.OpenElement( "span" );
            builder.Class( "b-report-section-rail-source" );
            builder.Content( section.DataSource );
            builder.CloseElement();
        }

        if ( section.Suppressed )
        {
            builder.OpenElement( "span" );
            builder.Class( "b-report-section-rail-status" );
            builder.Content( "Suppressed" );
            builder.CloseElement();
        }

        builder.CloseElement();
        builder.CloseElement();
    }

    private void RenderSectionLabel( RenderTreeBuilder builder, ReportSectionDefinition section )
    {
        builder.OpenElement( "div" );
        builder.Class( "b-report-section-label" );
        builder.Content( $"{GetSectionTypeDisplayName( section.Type )}: {GetSectionDisplayName( section )}" );
        builder.CloseElement();
    }

    private void RenderSectionResizeHandle( RenderTreeBuilder builder, int sectionIndex )
    {
        builder.OpenElement( "span" );
        builder.Key( "section-resize" );
        builder.Class( "b-report-section-resize-handle" );
        builder.Attribute( "title", "Resize band" );
        builder.Attribute( "onpointerdown", EventCallback.Factory.Create<PointerEventArgs>( this, eventArgs => BeginSectionPointerResizeAsync( sectionIndex, eventArgs ) ) );
        builder.EventPreventDefault( "onpointerdown", true );
        builder.EventStopPropagation( "onpointerdown", true );
        builder.CloseElement();
    }

    private void RenderElement( RenderTreeBuilder builder, object item, ReportElementDefinition element, bool designMode, bool editable, string elementKey )
    {
        var style = BuildElementStyle( element );
        var cssClass = $"b-report-element b-report-element-{element.Type.ToString().ToLowerInvariant()} {element.Class}".Trim();

        builder.OpenElement( "div" );
        builder.Key( elementKey );
        builder.Class( designMode ? $"{cssClass} b-report-element-design {( editable ? string.Empty : "disabled" )} {( editable && IsElementSelected( elementKey ) ? "active" : string.Empty )}" : cssClass );
        builder.Style( style );

        if ( designMode && editable )
        {
            builder.Attribute( "onclick", EventCallback.Factory.Create<MouseEventArgs>( this, eventArgs => HandleElementClick( elementKey, eventArgs ) ) );
            builder.EventStopPropagation( "onclick", true );
            builder.Attribute( "oncontextmenu", EventCallback.Factory.Create<MouseEventArgs>( this, eventArgs => OpenElementContextMenu( elementKey, eventArgs ) ) );
            builder.EventPreventDefault( "oncontextmenu", true );
            builder.EventStopPropagation( "oncontextmenu", true );
            builder.Attribute( "onpointerdown", EventCallback.Factory.Create<PointerEventArgs>( this, eventArgs => BeginElementPointerDrag( elementKey, eventArgs ) ) );
            builder.EventPreventDefault( "onpointerdown", true );
            builder.EventStopPropagation( "onpointerdown", true );
        }

        switch ( element.Type )
        {
            case ReportElementType.Field:
                var fieldItem = string.IsNullOrWhiteSpace( element.DataSource )
                    ? item
                    : ResolveItems( EffectiveDefinition, element.DataSource, item ).FirstOrDefault() ?? item;

                builder.Content( designMode ? FormatFieldExpression( element ) : FormatValue( ResolveFieldValue( fieldItem, element.Field ), element.Format ) );
                break;
            case ReportElementType.Table:
                RenderTable( builder, element );
                break;
            case ReportElementType.Image:
                builder.OpenElement( "img" );
                builder.Attribute( "src", element.Source );
                builder.Attribute( "alt", element.Text ?? element.Name );
                builder.CloseElement();
                break;
            case ReportElementType.PageBreak:
                break;
            default:
                builder.Content( element.Text );
                break;
        }

        if ( designMode && editable && IsElementSelected( elementKey ) )
        {
            RenderElementResizeHandles( builder, elementKey );
        }

        builder.CloseElement();
    }

    private static string BuildElementStyle( ReportElementDefinition element )
    {
        var font = element.Font;
        var appearance = element.Appearance;
        var border = element.Border;
        var styles = new List<string>
        {
            $"left:{element.X}px",
            $"top:{element.Y}px",
            $"width:{element.Width}px",
            $"height:{element.Height}px",
        };

        if ( !string.IsNullOrWhiteSpace( font?.Family ) )
            styles.Add( $"font-family:{font.Family}" );

        if ( font?.Size is > 0 )
            styles.Add( $"font-size:{font.Size.Value}px" );

        if ( !string.IsNullOrWhiteSpace( font?.Color ) )
            styles.Add( $"color:{font.Color}" );

        if ( !string.IsNullOrWhiteSpace( appearance?.BackgroundColor ) )
            styles.Add( $"background-color:{appearance.BackgroundColor}" );

        if ( font?.Bold == true )
            styles.Add( "font-weight:700" );

        if ( font?.Italic == true )
            styles.Add( "font-style:italic" );

        if ( font?.Underline == true )
            styles.Add( "text-decoration:underline" );

        var textAlignment = ToCssTextAlignment( font?.Alignment ?? TextAlignment.Default );

        if ( textAlignment is not null )
            styles.Add( $"text-align:{textAlignment}" );

        if ( !string.IsNullOrWhiteSpace( border?.Color ) )
            styles.Add( $"border-color:{border.Color}" );

        if ( border?.Width is >= 0 )
        {
            styles.Add( $"border-width:{border.Width.Value}px" );
            styles.Add( "border-style:solid" );
        }

        if ( border?.Radius is >= 0 )
            styles.Add( $"border-radius:{border.Radius.Value}px" );

        if ( appearance?.Opacity is >= 0 and <= 1 )
            styles.Add( $"opacity:{appearance.Opacity.Value.ToString( CultureInfo.InvariantCulture )}" );

        if ( !string.IsNullOrWhiteSpace( element.Style ) )
            styles.Add( element.Style.Trim().TrimEnd( ';' ) );

        return string.Join( ";", styles ) + ";";
    }

    private void RenderElementResizeHandles( RenderTreeBuilder builder, string elementKey )
    {
        RenderElementResizeHandle( builder, elementKey, ReportElementResizeHandle.NorthWest, "nw" );
        RenderElementResizeHandle( builder, elementKey, ReportElementResizeHandle.North, "n" );
        RenderElementResizeHandle( builder, elementKey, ReportElementResizeHandle.NorthEast, "ne" );
        RenderElementResizeHandle( builder, elementKey, ReportElementResizeHandle.East, "e" );
        RenderElementResizeHandle( builder, elementKey, ReportElementResizeHandle.SouthEast, "se" );
        RenderElementResizeHandle( builder, elementKey, ReportElementResizeHandle.South, "s" );
        RenderElementResizeHandle( builder, elementKey, ReportElementResizeHandle.SouthWest, "sw" );
        RenderElementResizeHandle( builder, elementKey, ReportElementResizeHandle.West, "w" );
    }

    private void RenderElementResizeHandle( RenderTreeBuilder builder, string elementKey, ReportElementResizeHandle handle, string handleClass )
    {
        builder.OpenElement( "span" );
        builder.Key( handleClass );
        builder.Class( $"b-report-resize-handle b-report-resize-handle-{handleClass}" );
        builder.Attribute( "onpointerdown", EventCallback.Factory.Create<PointerEventArgs>( this, eventArgs => BeginElementPointerResize( elementKey, handle, eventArgs ) ) );
        builder.EventPreventDefault( "onpointerdown", true );
        builder.EventStopPropagation( "onpointerdown", true );
        builder.CloseElement();
    }

    private void RenderDesignerDragPreview( RenderTreeBuilder builder, ReportDesignerDragPreview preview )
    {
        builder.OpenElement( "div" );
        builder.Key( "drag-preview" );
        builder.Class( $"b-report-drag-preview b-report-element-{preview.ElementType.ToString().ToLowerInvariant()}" );
        builder.Style( $"left:{preview.X}px;top:{preview.Y}px;width:{preview.Width}px;height:{preview.Height}px;" );
        builder.Content( preview.Text );
        builder.CloseElement();
    }

    private void RenderDesignerSelectionBox( RenderTreeBuilder builder, ReportDesignerSelectionBox selectionBox, double leftOffset )
    {
        builder.OpenElement( "div" );
        builder.Key( "selection-box" );
        builder.Class( "b-report-selection-box" );
        builder.Style( $"left:{selectionBox.X + leftOffset}px;top:{selectionBox.Y}px;width:{selectionBox.Width}px;height:{selectionBox.Height}px;" );
        builder.CloseElement();
    }

    private void RenderSelectedElementTools( RenderTreeBuilder builder, ReportDefinition definition )
    {
        var selected = FindSelectedElement( definition );

        if ( selected is null )
            return;

        builder.OpenComponent<Div>();
        builder.Attribute( "Margin", Margin.Is3.FromTop );
        builder.Attribute( "ChildContent", (RenderFragment)( childBuilder =>
        {
            childBuilder.OpenElement( "h6" );
            childBuilder.Content( "Selected element" );
            childBuilder.CloseElement();

            childBuilder.OpenComponent<Div>();
            childBuilder.Attribute( "Flex", Flex.Wrap );
            childBuilder.Attribute( "Gap", Gap.Is2 );
            childBuilder.Attribute( "ChildContent", (RenderFragment)( toolsBuilder =>
            {
                RenderDesignerButton( toolsBuilder, "Left", () => MoveSelectedElementAsync( -8, 0, 0, 0 ) );
                RenderDesignerButton( toolsBuilder, "Up", () => MoveSelectedElementAsync( 0, -8, 0, 0 ) );
                RenderDesignerButton( toolsBuilder, "Down", () => MoveSelectedElementAsync( 0, 8, 0, 0 ) );
                RenderDesignerButton( toolsBuilder, "Right", () => MoveSelectedElementAsync( 8, 0, 0, 0 ) );
                RenderDesignerButton( toolsBuilder, "Wider", () => MoveSelectedElementAsync( 0, 0, 16, 0 ) );
                RenderDesignerButton( toolsBuilder, "Taller", () => MoveSelectedElementAsync( 0, 0, 0, 8 ) );
            } ) );
            childBuilder.CloseComponent();
        } ) );
        builder.CloseComponent();
    }

    private void RenderSelectedSectionProperties( RenderTreeBuilder builder, ReportDefinition definition )
    {
        var selected = FindSelectedSection( definition );

        if ( selected is null || !string.IsNullOrWhiteSpace( selectedElementKey ) )
            return;

        builder.OpenComponent<Div>();
        builder.Attribute( "Margin", Margin.Is3.FromBottom );
        builder.Attribute( "ChildContent", (RenderFragment)( childBuilder =>
        {
            childBuilder.OpenElement( "h6" );
            childBuilder.Content( "Band properties" );
            childBuilder.CloseElement();

            RenderDesignerPropertyGroup( childBuilder, "Status", groupBuilder =>
            {
                RenderDesignerCheckbox( groupBuilder, "Suppress", selected.Suppressed, eventArgs => _ = UpdateSelectedSectionSuppressionAsync( eventArgs.Value is bool value && value ) );
            } );

            if ( selected.Suppressed )
                return;

            RenderDesignerPropertyGroup( childBuilder, "General", groupBuilder =>
            {
                RenderDesignerInput( groupBuilder, "Name", selected.Name, value => UpdateSelectedSectionAsync( section => section.Name = value ) );
                RenderDesignerInput( groupBuilder, "Data source", selected.DataSource, value => UpdateSelectedSectionAsync( section => section.DataSource = value ) );
            } );

            RenderDesignerPropertyGroup( childBuilder, "Layout", groupBuilder =>
            {
                RenderDesignerNumberInput( groupBuilder, "Height", selected.Height, value => UpdateSelectedSectionAsync( section => section.Height = Math.Max( 8, value ) ) );
            } );

            RenderDesignerPropertyGroup( childBuilder, "Advanced", groupBuilder =>
            {
                RenderDesignerInput( groupBuilder, "Custom CSS", selected.Style, value => UpdateSelectedSectionAsync( section => section.Style = value ) );
            } );

            childBuilder.OpenComponent<Div>();
            childBuilder.Attribute( "Flex", Flex.Wrap );
            childBuilder.Attribute( "Gap", Gap.Is2 );
            childBuilder.Attribute( "Margin", Margin.Is3.FromTop );
            childBuilder.Attribute( "ChildContent", (RenderFragment)( toolsBuilder =>
            {
                RenderDesignerButton( toolsBuilder, "Insert before", () => InsertSectionAsync( insertAfter: false ) );
                RenderDesignerButton( toolsBuilder, "Insert after", () => InsertSectionAsync( insertAfter: true ) );

                if ( CanDeleteSection( selected ) )
                {
                    RenderDesignerButton( toolsBuilder, "Delete band", DeleteSelectedSectionAsync );
                }
            } ) );
            childBuilder.CloseComponent();
        } ) );
        builder.CloseComponent();
    }

    private void RenderSelectedElementProperties( RenderTreeBuilder builder, ReportDefinition definition )
    {
        var selected = FindSelectedElement( definition );

        if ( selected is null || IsSelectedElementSuppressed( definition ) )
            return;

        builder.OpenComponent<Div>();
        builder.Attribute( "Margin", Margin.Is3.FromTop );
        builder.Attribute( "ChildContent", (RenderFragment)( childBuilder =>
        {
            childBuilder.OpenElement( "h6" );
            childBuilder.Content( "Element properties" );
            childBuilder.CloseElement();

            RenderDesignerPropertyGroup( childBuilder, "General", groupBuilder =>
            {
                RenderDesignerInput( groupBuilder, "Name", selected.Name, value => UpdateSelectedElementAsync( element => element.Name = value ) );
            } );

            RenderElementContentProperties( childBuilder, selected );
            RenderElementLayoutProperties( childBuilder, selected );
            RenderElementTextProperties( childBuilder, selected );
            RenderElementAppearanceProperties( childBuilder, selected );

            RenderDesignerPropertyGroup( childBuilder, "Advanced", groupBuilder =>
            {
                RenderDesignerInput( groupBuilder, "CSS classes", selected.Class, value => UpdateSelectedElementAsync( element => element.Class = value ) );
                RenderDesignerInput( groupBuilder, "Custom CSS", selected.Style, value => UpdateSelectedElementAsync( element => element.Style = value ) );
            } );
        } ) );
        builder.CloseComponent();
    }

    private void RenderElementContentProperties( RenderTreeBuilder builder, ReportElementDefinition selected )
    {
        if ( selected.Type != ReportElementType.Text
            && selected.Type != ReportElementType.Field
            && selected.Type != ReportElementType.Image )
            return;

        RenderDesignerPropertyGroup( builder, selected.Type == ReportElementType.Field ? "Data" : "Content", groupBuilder =>
        {
            switch ( selected.Type )
            {
                case ReportElementType.Text:
                    RenderDesignerInput( groupBuilder, "Text", selected.Text, value => UpdateSelectedElementAsync( element => element.Text = value ) );
                    break;
                case ReportElementType.Field:
                    RenderDesignerInput( groupBuilder, "Expression", FormatFieldExpression( selected ), valueChanged: null, readOnly: true );
                    RenderDesignerInput( groupBuilder, "Format", selected.Format, value => UpdateSelectedElementAsync( element => element.Format = value ) );
                    break;
                case ReportElementType.Image:
                    RenderDesignerInput( groupBuilder, "Source", selected.Source, value => UpdateSelectedElementAsync( element => element.Source = value ) );
                    RenderDesignerInput( groupBuilder, "Alt text", selected.Text, value => UpdateSelectedElementAsync( element => element.Text = value ) );
                    break;
            }
        } );
    }

    private void RenderElementLayoutProperties( RenderTreeBuilder builder, ReportElementDefinition selected )
    {
        RenderDesignerPropertyGroup( builder, "Position and size", groupBuilder =>
        {
            RenderDesignerNumberInput( groupBuilder, "X", selected.X, value => UpdateSelectedElementAsync( element => element.X = value ) );
            RenderDesignerNumberInput( groupBuilder, "Y", selected.Y, value => UpdateSelectedElementAsync( element => element.Y = value ) );
            RenderDesignerNumberInput( groupBuilder, "Width", selected.Width, value => UpdateSelectedElementAsync( element => element.Width = value ) );
            RenderDesignerNumberInput( groupBuilder, "Height", selected.Height, value => UpdateSelectedElementAsync( element => element.Height = value ) );
        } );
    }

    private void RenderElementTextProperties( RenderTreeBuilder builder, ReportElementDefinition selected )
    {
        if ( selected.Type is ReportElementType.Image or ReportElementType.Line or ReportElementType.Rectangle or ReportElementType.PageBreak )
            return;

        var font = EnsureFont( selected );

        RenderDesignerPropertyGroup( builder, "Text", groupBuilder =>
        {
            RenderDesignerInput( groupBuilder, "Font family", font.Family, value => UpdateSelectedElementAsync( element => EnsureFont( element ).Family = value ) );
            RenderDesignerNullableNumberInput( groupBuilder, "Font size", font.Size, value => UpdateSelectedElementAsync( element => EnsureFont( element ).Size = NormalizeNullablePositiveNumber( value ) ) );
            RenderDesignerColorInput( groupBuilder, "Font color", font.Color, value => UpdateSelectedElementAsync( element => EnsureFont( element ).Color = value ) );
            RenderDesignerSelectInput( groupBuilder, "Alignment", font.Alignment, value => UpdateSelectedElementAsync( element => EnsureFont( element ).Alignment = value ) );
            RenderDesignerCheckbox( groupBuilder, "Bold", font.Bold, eventArgs => _ = UpdateSelectedElementAsync( element => EnsureFont( element ).Bold = eventArgs.Value is bool value && value ) );
            RenderDesignerCheckbox( groupBuilder, "Italic", font.Italic, eventArgs => _ = UpdateSelectedElementAsync( element => EnsureFont( element ).Italic = eventArgs.Value is bool value && value ) );
            RenderDesignerCheckbox( groupBuilder, "Underline", font.Underline, eventArgs => _ = UpdateSelectedElementAsync( element => EnsureFont( element ).Underline = eventArgs.Value is bool value && value ) );
        } );
    }

    private void RenderElementAppearanceProperties( RenderTreeBuilder builder, ReportElementDefinition selected )
    {
        var appearance = EnsureAppearance( selected );
        var border = EnsureBorder( selected );

        RenderDesignerPropertyGroup( builder, "Appearance", groupBuilder =>
        {
            RenderDesignerColorInput( groupBuilder, "Fill color", appearance.BackgroundColor, value => UpdateSelectedElementAsync( element => EnsureAppearance( element ).BackgroundColor = value ) );
            RenderDesignerColorInput( groupBuilder, "Border color", border.Color, value => UpdateSelectedElementAsync( element => EnsureBorder( element ).Color = value ) );
            RenderDesignerNullableNumberInput( groupBuilder, "Border width", border.Width, value => UpdateSelectedElementAsync( element => EnsureBorder( element ).Width = NormalizeNullablePositiveNumber( value ) ) );
            RenderDesignerNullableNumberInput( groupBuilder, "Corner radius", border.Radius, value => UpdateSelectedElementAsync( element => EnsureBorder( element ).Radius = NormalizeNullablePositiveNumber( value ) ) );
            RenderDesignerNullableNumberInput( groupBuilder, "Opacity", appearance.Opacity, value => UpdateSelectedElementAsync( element => EnsureAppearance( element ).Opacity = NormalizeOpacity( value ) ) );
        } );
    }

    private void RenderDesignerPropertyGroup( RenderTreeBuilder builder, string title, RenderFragment childContent )
    {
        builder.OpenComponent<Div>();
        builder.Attribute( "Margin", Margin.Is3.FromBottom );
        builder.Attribute( "ChildContent", (RenderFragment)( groupBuilder =>
        {
            groupBuilder.OpenElement( "h6" );
            groupBuilder.Content( title );
            groupBuilder.CloseElement();

            groupBuilder.Content( childContent );
        } ) );
        builder.CloseComponent();
    }

    private void RenderDesignerInput( RenderTreeBuilder builder, string label, string value, Func<string, Task> valueChanged, bool readOnly = false )
    {
        builder.OpenComponent<Field>();
        builder.Attribute( "Margin", Margin.Is2.FromBottom );
        builder.Attribute( "ChildContent", (RenderFragment)( fieldBuilder =>
        {
            fieldBuilder.OpenComponent<FieldLabel>();
            fieldBuilder.Attribute( "ChildContent", (RenderFragment)( labelBuilder => labelBuilder.Content( label ) ) );
            fieldBuilder.CloseComponent();

            fieldBuilder.OpenComponent<TextInput>();
            fieldBuilder.Attribute( "Value", value );
            fieldBuilder.Attribute( "ReadOnly", readOnly );

            if ( valueChanged is not null )
            {
                fieldBuilder.Attribute( "ValueChanged", EventCallback.Factory.Create<string>( this, valueChanged ) );
                fieldBuilder.Attribute( "Immediate", true );
            }

            fieldBuilder.CloseComponent();
        } ) );
        builder.CloseComponent();
    }

    private void RenderDesignerNullableNumberInput( RenderTreeBuilder builder, string label, double? value, Func<double?, Task> valueChanged )
    {
        builder.OpenComponent<Field>();
        builder.Attribute( "Margin", Margin.Is2.FromBottom );
        builder.Attribute( "ChildContent", (RenderFragment)( fieldBuilder =>
        {
            fieldBuilder.OpenComponent<FieldLabel>();
            fieldBuilder.Attribute( "ChildContent", (RenderFragment)( labelBuilder => labelBuilder.Content( label ) ) );
            fieldBuilder.CloseComponent();

            fieldBuilder.OpenComponent<NumericInput<double?>>();
            fieldBuilder.Attribute( "Value", value );
            fieldBuilder.Attribute( "ValueChanged", EventCallback.Factory.Create<double?>( this, valueChanged ) );
            fieldBuilder.Attribute( "Immediate", true );
            fieldBuilder.Attribute( "Step", 1m );
            fieldBuilder.CloseComponent();
        } ) );
        builder.CloseComponent();
    }

    private void RenderDesignerColorInput( RenderTreeBuilder builder, string label, string value, Func<string, Task> valueChanged )
    {
        builder.OpenComponent<Field>();
        builder.Attribute( "Margin", Margin.Is2.FromBottom );
        builder.Attribute( "ChildContent", (RenderFragment)( fieldBuilder =>
        {
            fieldBuilder.OpenComponent<FieldLabel>();
            fieldBuilder.Attribute( "ChildContent", (RenderFragment)( labelBuilder => labelBuilder.Content( label ) ) );
            fieldBuilder.CloseComponent();

            fieldBuilder.OpenComponent<Div>();
            fieldBuilder.Attribute( "Flex", Flex.Row );
            fieldBuilder.Attribute( "Gap", Gap.Is2 );
            fieldBuilder.Attribute( "ChildContent", (RenderFragment)( inputBuilder =>
            {
                inputBuilder.OpenElement( "input" );
                inputBuilder.Type( "color" );
                inputBuilder.Attribute( "value", NormalizeColorValue( value ) );
                inputBuilder.Attribute( "onchange", EventCallback.Factory.Create<ChangeEventArgs>( this, eventArgs => valueChanged( Convert.ToString( eventArgs.Value, CultureInfo.InvariantCulture ) ) ) );
                inputBuilder.CloseElement();

                RenderDesignerButton( inputBuilder, "Clear", () => valueChanged( null ) );
            } ) );
            fieldBuilder.CloseComponent();
        } ) );
        builder.CloseComponent();
    }

    private void RenderDesignerSelectInput<TValue>( RenderTreeBuilder builder, string label, TValue value, Func<TValue, Task> valueChanged )
        where TValue : struct, Enum
    {
        builder.OpenComponent<Field>();
        builder.Attribute( "Margin", Margin.Is2.FromBottom );
        builder.Attribute( "ChildContent", (RenderFragment)( fieldBuilder =>
        {
            fieldBuilder.OpenComponent<FieldLabel>();
            fieldBuilder.Attribute( "ChildContent", (RenderFragment)( labelBuilder => labelBuilder.Content( label ) ) );
            fieldBuilder.CloseComponent();

            fieldBuilder.OpenComponent<Select<TValue>>();
            fieldBuilder.Attribute( "Value", value );
            fieldBuilder.Attribute( "ValueChanged", EventCallback.Factory.Create<TValue>( this, valueChanged ) );
            fieldBuilder.Attribute( "ChildContent", (RenderFragment)( selectBuilder =>
            {
                foreach ( var option in Enum.GetValues<TValue>() )
                {
                    selectBuilder.OpenComponent<SelectItem<TValue>>();
                    selectBuilder.Attribute( "Value", option );
                    selectBuilder.Attribute( "ChildContent", (RenderFragment)( optionBuilder => optionBuilder.Content( option.ToString() ) ) );
                    selectBuilder.CloseComponent();
                }
            } ) );

            fieldBuilder.CloseComponent();
        } ) );
        builder.CloseComponent();
    }

    private void RenderDesignerNumberInput( RenderTreeBuilder builder, string label, double value, Func<double, Task> valueChanged )
    {
        builder.OpenComponent<Field>();
        builder.Attribute( "Margin", Margin.Is2.FromBottom );
        builder.Attribute( "ChildContent", (RenderFragment)( fieldBuilder =>
        {
            fieldBuilder.OpenComponent<FieldLabel>();
            fieldBuilder.Attribute( "ChildContent", (RenderFragment)( labelBuilder => labelBuilder.Content( label ) ) );
            fieldBuilder.CloseComponent();

            fieldBuilder.OpenComponent<NumericInput<double>>();
            fieldBuilder.Attribute( "Value", value );
            fieldBuilder.Attribute( "ValueChanged", EventCallback.Factory.Create<double>( this, valueChanged ) );
            fieldBuilder.Attribute( "Immediate", true );
            fieldBuilder.Attribute( "Step", 1m );
            fieldBuilder.CloseComponent();
        } ) );
        builder.CloseComponent();
    }

    private void RenderDesignerCheckbox( RenderTreeBuilder builder, string label, bool value, Action<ChangeEventArgs> valueChanged )
    {
        builder.OpenComponent<Field>();
        builder.Attribute( "Margin", Margin.Is2.FromBottom );
        builder.Attribute( "ChildContent", (RenderFragment)( fieldBuilder =>
        {
            fieldBuilder.OpenElement( "label" );
            fieldBuilder.Class( "b-report-designer-option" );
            fieldBuilder.OpenElement( "input" );
            fieldBuilder.Type( "checkbox" );
            fieldBuilder.Attribute( "checked", value );
            fieldBuilder.Attribute( "onchange", EventCallback.Factory.Create<ChangeEventArgs>( this, valueChanged ) );
            fieldBuilder.CloseElement();
            fieldBuilder.Content( label );
            fieldBuilder.CloseElement();
        } ) );
        builder.CloseComponent();
    }

    private void RenderDesignerButton( RenderTreeBuilder builder, string text, Func<Task> clicked )
    {
        builder.OpenComponent<Button>();
        builder.Attribute( "Color", Color.Light );
        builder.Attribute( "Clicked", EventCallback.Factory.Create<MouseEventArgs>( this, clicked ) );
        builder.Attribute( "ChildContent", (RenderFragment)( buttonBuilder => buttonBuilder.Content( text ) ) );
        builder.CloseComponent();
    }

    private void RenderTable( RenderTreeBuilder builder, ReportElementDefinition element )
    {
        builder.OpenElement( "table" );

        if ( element.Columns.Count == 0 )
        {
            builder.OpenElement( "tr" );
            builder.OpenElement( "td" );
            builder.CloseElement();
            builder.CloseElement();
        }
        else
        {
            builder.OpenElement( "tr" );
            foreach ( var column in element.Columns )
            {
                builder.OpenElement( "td" );
                builder.Style( $"width:{column.Width}px" );
                builder.Content( column.Title ?? column.Field );
                builder.CloseElement();
            }
            builder.CloseElement();
        }

        builder.CloseElement();
    }

    /// <summary>
    /// Executes a report command against the current designer or viewer state.
    /// </summary>
    /// <param name="command">Command requested by a toolbar item or external caller.</param>
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
            ReportCommand.Delete => DeleteSelectionAsync(),
            ReportCommand.Undo => UndoAsync(),
            ReportCommand.Redo => RedoAsync(),
            ReportCommand.Reset => ResetDefinitionAsync(),
            _ => SetPreviewAsync( ReportPreviewFormat.Html ),
        } );
    }

    /// <summary>
    /// Determines whether the supplied report command is currently available.
    /// </summary>
    /// <param name="command">Command to evaluate against the current report state.</param>
    /// <returns><c>true</c> when the command can be executed.</returns>
    public bool CanExecuteCommand( ReportCommand command )
    {
        var definition = EffectiveDefinition;

        return command switch
        {
            ReportCommand.Design => IsDesignerEnabled,
            ReportCommand.Preview => SupportsPreviewFormat( currentPreviewFormat ) || SupportsPreviewFormat( context.ViewerOptions.DefaultFormat ),
            ReportCommand.PreviewHtml => SupportsPreviewFormat( ReportPreviewFormat.Html ),
            ReportCommand.PreviewPdf => SupportsPreviewFormat( ReportPreviewFormat.Pdf ),
            ReportCommand.Cut or ReportCommand.Copy => CurrentMode == ReportStudioMode.Design && FindSelectedElement( definition ) is not null,
            ReportCommand.Delete => CurrentMode == ReportStudioMode.Design && CanDeleteSelection( definition ),
            ReportCommand.Paste => CurrentMode == ReportStudioMode.Design && clipboardElement is not null && definition.Sections.Count > 0,
            ReportCommand.Undo => historyService.CanUndo,
            ReportCommand.Redo => historyService.CanRedo,
            _ => true,
        };
    }

    /// <summary>
    /// Determines whether the supplied report command represents the active mode or preview format.
    /// </summary>
    /// <param name="command">Command to evaluate against the current report state.</param>
    /// <returns><c>true</c> when the command is active.</returns>
    public bool IsCommandActive( ReportCommand command )
    {
        return command switch
        {
            ReportCommand.Design => CurrentMode == ReportStudioMode.Design,
            ReportCommand.Preview => CurrentMode == ReportStudioMode.Preview,
            ReportCommand.PreviewHtml => CurrentMode == ReportStudioMode.Preview && CurrentPreviewFormat == ReportPreviewFormat.Html,
            ReportCommand.PreviewPdf => CurrentMode == ReportStudioMode.Preview && CurrentPreviewFormat == ReportPreviewFormat.Pdf,
            _ => false,
        };
    }

    /// <summary>
    /// Captures the current report definition, mode, selection, clipboard, and history availability.
    /// </summary>
    /// <returns>Serializable report designer state.</returns>
    public Task<ReportState> GetState()
    {
        return Task.FromResult( CaptureReportState( EffectiveDefinition ) );
    }

    /// <summary>
    /// Restores a previously captured report designer state.
    /// </summary>
    /// <param name="state">State to apply to the report designer.</param>
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

        if ( command.NotifyDefinitionChanged )
            designerSurfaceVersion++;

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
            SelectReport();
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
                SelectSection( sectionIndex );
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

            SelectElement( GetDesignerElementKey( element ) );
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
        var elementIds = GetSelectedElementIds( definition ).ToList();

        if ( elementIds.Count > 0 )
        {
            var primaryElementId = FindElementLocation( definition, selectedElementKey, out var sectionIndex, out _, out var element )
                ? element.Id
                : elementIds[0];

            if ( element is null )
                FindElementLocation( definition, primaryElementId, out sectionIndex, out _, out element );

            return new()
            {
                Type = ReportSelectionType.Element,
                SectionId = sectionIndex >= 0 ? definition.Sections[sectionIndex].Id : null,
                ElementId = primaryElementId,
                ElementIds = elementIds,
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

        if ( notifyDefinitionChanged )
            designerSurfaceVersion++;

        await InvokeAsync( StateHasChanged );
    }

    private void ApplySelectionState( ReportDefinition definition, ReportSelectionState selection )
    {
        reportSelected = true;
        selectedSectionIndex = null;
        selectedElementKey = null;
        selectedElementKeys.Clear();

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
            List<string> elementIds = selection.ElementIds is not null && selection.ElementIds.Count > 0
                ? selection.ElementIds
                : string.IsNullOrWhiteSpace( selection.ElementId ) ? [] : [selection.ElementId];

            foreach ( var section in definition.Sections )
            {
                foreach ( var element in section.Elements.Where( element => elementIds.Contains( element.Id ) ) )
                {
                    reportSelected = false;
                    selectedElementKeys.Add( GetDesignerElementKey( element ) );
                }
            }

            selectedElementKey = selectedElementKeys.Contains( selection.ElementId )
                ? selection.ElementId
                : selectedElementKeys.FirstOrDefault();
        }
    }

    private void SelectReport()
    {
        reportSelected = true;
        selectedSectionIndex = null;
        selectedElementKey = null;
        selectedElementKeys.Clear();
        contextMenu = null;
    }

    private void HandleElementClick( string key, MouseEventArgs eventArgs )
    {
        if ( IsSuppressingSelectionClick() )
            return;

        if ( string.Equals( suppressNextElementClickKey, key, StringComparison.Ordinal ) )
        {
            suppressNextElementClickKey = null;
            return;
        }

        if ( eventArgs.CtrlKey )
        {
            ToggleElementSelection( key );
            return;
        }

        SelectElement( key, preserveSelection: IsElementSelected( key ) && selectedElementKeys.Count > 1 );
    }

    private void HandleSectionClick( int sectionIndex )
    {
        if ( IsSuppressingSelectionClick() )
            return;

        if ( suppressNextSectionClick )
        {
            suppressNextSectionClick = false;
            return;
        }

        SelectSection( sectionIndex );
    }

    private bool IsSuppressingSelectionClick()
    {
        if ( DateTime.UtcNow > suppressSelectionClickUntil )
            return false;

        suppressNextSectionClick = false;
        suppressNextElementClickKey = null;

        return true;
    }

    private void SelectElement( string key, bool preserveSelection = false )
    {
        reportSelected = false;
        selectedSectionIndex = null;
        selectedElementKey = key;

        if ( !preserveSelection )
            selectedElementKeys.Clear();

        if ( !string.IsNullOrWhiteSpace( key ) )
            selectedElementKeys.Add( key );

        contextMenu = null;
    }

    private void ToggleElementSelection( string key )
    {
        if ( string.IsNullOrWhiteSpace( key ) )
            return;

        reportSelected = false;
        selectedSectionIndex = null;

        if ( selectedElementKeys.Contains( key ) )
        {
            selectedElementKeys.Remove( key );

            if ( string.Equals( selectedElementKey, key, StringComparison.Ordinal ) )
                selectedElementKey = selectedElementKeys.FirstOrDefault();
        }
        else
        {
            selectedElementKeys.Add( key );
            selectedElementKey = key;
        }

        if ( selectedElementKeys.Count == 0 )
        {
            reportSelected = true;
            selectedElementKey = null;
        }

        contextMenu = null;
    }

    private void SelectElements( IEnumerable<string> elementKeys, string primaryElementKey = null )
    {
        selectedElementKeys.Clear();

        foreach ( var elementKey in elementKeys.Where( key => !string.IsNullOrWhiteSpace( key ) ) )
        {
            selectedElementKeys.Add( elementKey );
        }

        selectedElementKey = !string.IsNullOrWhiteSpace( primaryElementKey ) && selectedElementKeys.Contains( primaryElementKey )
            ? primaryElementKey
            : selectedElementKeys.FirstOrDefault();
        selectedSectionIndex = null;
        reportSelected = selectedElementKeys.Count == 0;
        contextMenu = null;
    }

    private void SelectSection( int index )
    {
        reportSelected = false;
        selectedSectionIndex = index;
        selectedElementKey = null;
        selectedElementKeys.Clear();
        contextMenu = null;
    }

    private void ToggleSectionCollapsed( ReportSectionDefinition section )
    {
        if ( !AllowBandCollapse || section is null )
            return;

        var sectionId = GetSectionDesignerKey( section );

        if ( collapsedSectionIds.Contains( sectionId ) )
            collapsedSectionIds.Remove( sectionId );
        else
            collapsedSectionIds.Add( sectionId );
    }

    private bool IsSectionCollapsed( ReportSectionDefinition section )
    {
        return AllowBandCollapse
            && section is not null
            && collapsedSectionIds.Contains( GetSectionDesignerKey( section ) );
    }

    private void OpenSectionContextMenu( int sectionIndex, MouseEventArgs eventArgs )
    {
        reportSelected = false;
        selectedSectionIndex = sectionIndex;
        selectedElementKey = null;
        selectedElementKeys.Clear();
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
        selectedSectionIndex = null;

        if ( !IsElementSelected( elementKey ) )
            selectedElementKeys.Clear();

        selectedElementKey = elementKey;
        selectedElementKeys.Add( elementKey );
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
            var definition = EffectiveDefinition;
            var element = FindSelectedElement( definition );

            if ( element is not null )
            {
                FindElementLocation( definition, GetDesignerElementKey( element ), out var sectionIndex, out _, out _ );
                var originalX = element.X;
                var originalWidth = element.Width;

                element.X = Math.Max( 0, element.X + x );
                element.Y = Math.Max( 0, element.Y + y );
                element.Width = Math.Max( 8, element.Width + width );
                element.Height = Math.Max( 8, element.Height + height );

                SyncMatchingPageHeaderForDetailElement( definition, sectionIndex, sectionIndex, element, originalX, originalWidth, element.X, element.Width );
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
            var definition = EffectiveDefinition;
            var element = FindSelectedElement( definition );

            if ( element is not null )
            {
                FindElementLocation( definition, GetDesignerElementKey( element ), out var sectionIndex, out _, out _ );
                var originalX = element.X;
                var originalWidth = element.Width;

                update?.Invoke( element );

                SyncMatchingPageHeaderForDetailElement( definition, sectionIndex, sectionIndex, element, originalX, originalWidth, element.X, element.Width );
            }

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
                Default = false,
                Suppressed = false,
            };

            definition.Sections.Insert( insertIndex, section );

            SelectSection( insertIndex );
            contextMenu = null;

            return Task.CompletedTask;
        } ) );
    }

    private Task DeleteSelectionAsync()
    {
        if ( !string.IsNullOrWhiteSpace( selectedElementKey ) )
            return DeleteSelectedElementAsync();

        return DeleteSelectedSectionAsync();
    }

    private async Task DeleteSelectedSectionAsync()
    {
        var definition = EffectiveDefinition;

        if ( selectedSectionIndex is null
            || selectedSectionIndex < 0
            || selectedSectionIndex >= definition.Sections.Count
            || !CanDeleteSection( definition.Sections[selectedSectionIndex.Value] ) )
        {
            return;
        }

        await ExecuteDesignerCommandAsync( new( "Delete band", () =>
        {
            var definition = EffectiveDefinition;

            if ( selectedSectionIndex is null
                || selectedSectionIndex < 0
                || selectedSectionIndex >= definition.Sections.Count )
            {
                return Task.CompletedTask;
            }

            var section = definition.Sections[selectedSectionIndex.Value];

            if ( !CanDeleteSection( section ) )
                return Task.CompletedTask;

            collapsedSectionIds.Remove( GetSectionDesignerKey( section ) );
            definition.Sections.RemoveAt( selectedSectionIndex.Value );

            if ( definition.Sections.Count == 0 )
            {
                SelectReport();
            }
            else
            {
                SelectSection( Math.Min( selectedSectionIndex.Value, definition.Sections.Count - 1 ) );
            }

            contextMenu = null;
            ClearDragState();

            return Task.CompletedTask;
        } ) );
    }

    private async Task ToggleSelectedSectionSuppressionAsync()
    {
        var section = FindSelectedSection( EffectiveDefinition );

        if ( section is null )
            return;

        await UpdateSelectedSectionSuppressionAsync( !section.Suppressed );
    }

    private async Task UpdateSelectedSectionSuppressionAsync( bool suppressed )
    {
        await ExecuteDesignerCommandAsync( new( suppressed ? "Suppress" : "Don't suppress", () =>
        {
            var section = FindSelectedSection( EffectiveDefinition );

            if ( section is not null )
            {
                section.Suppressed = suppressed;

                if ( suppressed )
                {
                    collapsedSectionIds.Remove( GetSectionDesignerKey( section ) );
                }
            }

            contextMenu = null;

            return Task.CompletedTask;
        } ) );
    }

    private async Task DeleteSelectedElementAsync()
    {
        var definition = EffectiveDefinition;

        var elementKeys = GetSelectedElementIds( definition ).ToList();

        if ( elementKeys.Count == 0 )
            return;

        await ExecuteDesignerCommandAsync( new( elementKeys.Count == 1 ? "Delete element" : "Delete elements", () =>
        {
            var definition = EffectiveDefinition;
            var selectedIds = GetSelectedElementIds( definition ).ToHashSet( StringComparer.Ordinal );
            var lastSectionIndex = -1;

            for ( var sectionIndex = 0; sectionIndex < definition.Sections.Count; sectionIndex++ )
            {
                var section = definition.Sections[sectionIndex];
                var removed = section.Elements.RemoveAll( element => selectedIds.Contains( element.Id ) );

                if ( removed > 0 )
                    lastSectionIndex = sectionIndex;
            }

            if ( lastSectionIndex >= 0 )
                SelectSection( lastSectionIndex );

            contextMenu = null;

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
        elementPointerResize = null;
        sectionPointerResize = null;
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
        selectionBox = null;
        elementPointerDrag = null;
        elementPointerResize = null;
    }

    private void BeginElementPointerDrag( string elementKey, PointerEventArgs eventArgs )
    {
        if ( eventArgs.CtrlKey )
        {
            ToggleElementSelection( elementKey );
            suppressNextElementClickKey = elementKey;
            return;
        }

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
            TargetX = element.X,
            TargetY = element.Y,
            SelectedElements = CaptureElementPointerItems( EffectiveDefinition, elementKey ).ToList(),
        };

        SelectElement( elementKey, preserveSelection: IsElementSelected( elementKey ) && selectedElementKeys.Count > 1 );
    }

    private void BeginElementPointerResize( string elementKey, ReportElementResizeHandle handle, PointerEventArgs eventArgs )
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
        elementPointerDrag = null;
        elementPointerResize = new()
        {
            ElementKey = elementKey,
            SourceSectionIndex = sectionIndex,
            Handle = handle,
            OriginalX = element.X,
            OriginalY = element.Y,
            OriginalWidth = element.Width,
            OriginalHeight = element.Height,
            StartClientX = eventArgs.ClientX,
            StartClientY = eventArgs.ClientY,
            TargetX = element.X,
            TargetY = element.Y,
            TargetWidth = element.Width,
            TargetHeight = element.Height,
            MinimumHeight = GetMinimumElementHeight( element ),
            SelectedElements = CaptureElementPointerItems( EffectiveDefinition, elementKey ).ToList(),
        };

        SelectElement( elementKey, preserveSelection: IsElementSelected( elementKey ) && selectedElementKeys.Count > 1 );
    }

    private async Task BeginSectionPointerResizeAsync( int sectionIndex, PointerEventArgs eventArgs )
    {
        var definition = EffectiveDefinition;

        if ( sectionIndex < 0 || sectionIndex >= definition.Sections.Count )
            return;

        var section = definition.Sections[sectionIndex];

        if ( section.Suppressed )
            return;

        draggedKind = ReportDesignerDragKind.None;
        draggedDataSourceName = null;
        draggedFieldName = null;
        draggedElementType = null;
        draggedElementText = null;
        draggedElementKey = null;
        draggedElement = null;
        dragPreview = null;
        elementPointerDrag = null;
        elementPointerResize = null;
        sectionPointerResize = new()
        {
            SectionIndex = sectionIndex,
            OriginalHeight = section.Height,
            TargetHeight = section.Height,
            StartClientY = eventArgs.ClientY,
        };

        SelectSection( sectionIndex );

        await StartDocumentSectionResizeAsync( eventArgs.ClientY );
        await InvokeAsync( StateHasChanged );
    }

    private Task PreviewElementPointerInteractionAsync( int targetSectionIndex, PointerEventArgs eventArgs )
    {
        if ( selectionBox is not null )
            return PreviewSelectionBoxAsync( eventArgs );

        if ( sectionPointerResize is not null )
            return PreviewSectionPointerResizeAsync( eventArgs );

        if ( elementPointerResize is not null )
            return PreviewElementPointerResizeAsync( eventArgs );

        return PreviewElementPointerDragAsync( targetSectionIndex, eventArgs );
    }

    private Task CompleteElementPointerInteractionAsync( int targetSectionIndex, PointerEventArgs eventArgs )
    {
        if ( selectionBox is not null )
            return CompleteSelectionBoxAsync( eventArgs );

        if ( sectionPointerResize is not null )
            return CompleteSectionPointerResizeAsync( eventArgs );

        if ( elementPointerResize is not null )
            return CompleteElementPointerResizeAsync( eventArgs );

        return CompleteElementPointerDragAsync( targetSectionIndex, eventArgs );
    }

    private Task CancelElementPointerInteractionAsync()
    {
        if ( selectionBox is not null )
            return CancelSelectionBoxAsync();

        if ( sectionPointerResize is not null )
            return CancelSectionPointerResizeAsync();

        if ( elementPointerResize is not null )
            return CancelElementPointerResizeAsync();

        return CancelElementPointerDragAsync();
    }

    private void BeginSelectionBox( int sectionIndex, PointerEventArgs eventArgs )
    {
        if ( draggedKind != ReportDesignerDragKind.None
            || elementPointerDrag is not null
            || elementPointerResize is not null
            || sectionPointerResize is not null )
        {
            return;
        }

        var section = GetDesignerSection( EffectiveDefinition, sectionIndex );

        if ( section is null || section.Suppressed )
            return;

        var x = ClampDesignerValue( eventArgs.OffsetX, 0, EffectiveDefinition.Page.Width );
        var y = ClampDesignerValue( GetSectionOffsetY( EffectiveDefinition, sectionIndex ) + eventArgs.OffsetY, 0, GetDesignerContentHeight( EffectiveDefinition ) );

        selectionBox = new()
        {
            SectionIndex = sectionIndex,
            StartX = x,
            StartY = y,
            CurrentX = x,
            CurrentY = y,
            StartClientX = eventArgs.ClientX,
            StartClientY = eventArgs.ClientY,
            Additive = eventArgs.CtrlKey,
        };
    }

    private Task PreviewSelectionBoxAsync( PointerEventArgs eventArgs )
    {
        if ( selectionBox is null )
            return Task.CompletedTask;

        UpdateSelectionBox( eventArgs );

        return InvokeAsync( StateHasChanged );
    }

    private Task PreviewPageSelectionBoxAsync( PointerEventArgs eventArgs )
    {
        return selectionBox is null
            ? Task.CompletedTask
            : PreviewSelectionBoxAsync( eventArgs );
    }

    private Task CompleteSelectionBoxAsync( PointerEventArgs eventArgs )
    {
        if ( selectionBox is null )
            return Task.CompletedTask;

        UpdateSelectionBox( eventArgs );

        var completedSelectionBox = selectionBox;
        selectionBox = null;

        if ( !completedSelectionBox.HasMoved )
            return InvokeAsync( StateHasChanged );

        var selectedKeys = FindElementsInsideSelectionBox( EffectiveDefinition, completedSelectionBox ).ToList();

        if ( completedSelectionBox.Additive )
            selectedKeys.InsertRange( 0, selectedElementKeys );

        if ( selectedKeys.Count > 0 )
        {
            SelectElements( selectedKeys.Distinct( StringComparer.Ordinal ) );
        }
        else
        {
            SelectSection( completedSelectionBox.SectionIndex );
        }

        suppressNextSectionClick = true;
        suppressSelectionClickUntil = DateTime.UtcNow.AddMilliseconds( 300 );

        return InvokeAsync( StateHasChanged );
    }

    private Task CompletePageSelectionBoxAsync( PointerEventArgs eventArgs )
    {
        return selectionBox is null
            ? Task.CompletedTask
            : CompleteSelectionBoxAsync( eventArgs );
    }

    private Task CancelSelectionBoxAsync()
    {
        selectionBox = null;

        return InvokeAsync( StateHasChanged );
    }

    private Task CancelPageSelectionBoxAsync()
    {
        return selectionBox is null
            ? Task.CompletedTask
            : CancelSelectionBoxAsync();
    }

    private void UpdateSelectionBox( PointerEventArgs eventArgs )
    {
        if ( selectionBox is null )
            return;

        selectionBox.CurrentX = ClampDesignerValue( selectionBox.StartX + eventArgs.ClientX - selectionBox.StartClientX, 0, EffectiveDefinition.Page.Width );
        selectionBox.CurrentY = ClampDesignerValue( selectionBox.StartY + eventArgs.ClientY - selectionBox.StartClientY, 0, GetDesignerContentHeight( EffectiveDefinition ) );
        selectionBox.HasMoved = selectionBox.HasMoved
            || Math.Abs( selectionBox.CurrentX - selectionBox.StartX ) > 2
            || Math.Abs( selectionBox.CurrentY - selectionBox.StartY ) > 2;
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

        elementPointerDrag.TargetSectionIndex = preview.SectionIndex;
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
        var canMove = pointerDrag.SourceSectionIndex >= 0
            && pointerDrag.SourceSectionIndex < definition.Sections.Count
            && pointerDrag.TargetSectionIndex >= 0
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

            ApplyElementPointerDrag( definition, pointerDrag );
            SelectElements( pointerDrag.SelectedElements.Select( item => item.ElementKey ), pointerDrag.ElementKey );
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

        var x = elementPointerDrag.OriginalX + eventArgs.ClientX - elementPointerDrag.StartClientX;
        var pageY = GetSectionOffsetY( EffectiveDefinition, elementPointerDrag.SourceSectionIndex ) + elementPointerDrag.OriginalY + eventArgs.ClientY - elementPointerDrag.StartClientY;
        var y = pageY - GetSectionOffsetY( EffectiveDefinition, targetSectionIndex );

        x = ApplyDesignerGrid( x );
        y = ApplyDesignerGrid( y );

        return ConstrainDesignerDragPreview( EffectiveDefinition, CreateDragPreview( targetSectionIndex, draggedElement, x, y ) );
    }

    private void ApplyElementPointerDrag( ReportDefinition definition, ReportElementPointerDragState pointerDrag )
    {
        if ( definition is null || pointerDrag is null )
            return;

        if ( pointerDrag.TargetSectionIndex < 0 || pointerDrag.TargetSectionIndex >= definition.Sections.Count )
            return;

        var deltaX = pointerDrag.TargetX - pointerDrag.OriginalX;
        var targetSection = definition.Sections[pointerDrag.TargetSectionIndex];
        var activeOriginalPageY = GetSectionOffsetY( definition, pointerDrag.SourceSectionIndex ) + pointerDrag.OriginalY;

        foreach ( var item in pointerDrag.SelectedElements )
        {
            if ( !FindElementLocation( definition, item.ElementKey, out var sourceSectionIndex, out var sourceElementIndex, out var element ) )
                continue;

            var targetLocalY = pointerDrag.TargetY + item.OriginalPageY - activeOriginalPageY;

            element.X = ClampDesignerValue( item.OriginalX + deltaX, 0, Math.Max( 0, definition.Page.Width - element.Width ) );
            element.Y = Math.Max( 0, targetLocalY );

            SyncMatchingPageHeaderForDetailElement(
                definition,
                sourceSectionIndex,
                pointerDrag.TargetSectionIndex,
                element,
                item.OriginalX,
                item.OriginalWidth,
                element.X,
                element.Width,
                pointerDrag.SelectedElements.Select( selectedItem => selectedItem.ElementKey ) );

            if ( sourceSectionIndex != pointerDrag.TargetSectionIndex )
            {
                definition.Sections[sourceSectionIndex].Elements.RemoveAt( sourceElementIndex );
                targetSection.Elements.Add( element );
            }
        }

        GrowSectionToFitElements( targetSection );
    }

    private async Task PreviewElementPointerResizeAsync( PointerEventArgs eventArgs )
    {
        if ( elementPointerResize is null || draggedElement is null || draggedKind != ReportDesignerDragKind.Element )
            return;

        var preview = CreateElementPointerResizePreview( eventArgs );

        if ( preview is null )
            return;

        var samePreviewSize = dragPreview is not null
            && Math.Abs( dragPreview.X - preview.X ) < .1
            && Math.Abs( dragPreview.Y - preview.Y ) < .1
            && Math.Abs( dragPreview.Width - preview.Width ) < .1
            && Math.Abs( dragPreview.Height - preview.Height ) < .1;

        if ( samePreviewSize )
            return;

        var now = DateTime.UtcNow;

        if ( !snapToGrid
            && dragPreview is not null
            && now - lastDragPreviewRenderTime < TimeSpan.FromMilliseconds( 16 ) )
        {
            return;
        }

        elementPointerResize.TargetX = preview.X;
        elementPointerResize.TargetY = preview.Y;
        elementPointerResize.TargetWidth = preview.Width;
        elementPointerResize.TargetHeight = preview.Height;
        elementPointerResize.HasResized = true;
        dragPreview = preview;
        lastDragPreviewRenderTime = now;

        await InvokeAsync( StateHasChanged );
    }

    private async Task CompleteElementPointerResizeAsync( PointerEventArgs eventArgs )
    {
        if ( elementPointerResize is null || draggedKind != ReportDesignerDragKind.Element )
            return;

        var pointerResize = elementPointerResize;
        var preview = CreateElementPointerResizePreview( eventArgs ) ?? dragPreview;

        if ( preview is not null )
        {
            pointerResize.TargetX = preview.X;
            pointerResize.TargetY = preview.Y;
            pointerResize.TargetWidth = preview.Width;
            pointerResize.TargetHeight = preview.Height;
        }

        var resized = pointerResize.HasResized
            && ( Math.Abs( pointerResize.TargetX - pointerResize.OriginalX ) > .1
                || Math.Abs( pointerResize.TargetY - pointerResize.OriginalY ) > .1
                || Math.Abs( pointerResize.TargetWidth - pointerResize.OriginalWidth ) > .1
                || Math.Abs( pointerResize.TargetHeight - pointerResize.OriginalHeight ) > .1 );

        if ( !resized || !FindElementLocation( EffectiveDefinition, pointerResize.ElementKey, out _, out _, out _ ) )
        {
            ClearDragState();
            await InvokeAsync( StateHasChanged );
            return;
        }

        await ExecuteDesignerCommandAsync( new( "Resize element", () =>
        {
            ApplyElementPointerResize( EffectiveDefinition, pointerResize );
            SelectElements( pointerResize.SelectedElements.Select( item => item.ElementKey ), pointerResize.ElementKey );
            dragPreview = null;
            ClearDragState();

            return Task.CompletedTask;
        } ) );
    }

    private Task CancelElementPointerResizeAsync()
    {
        if ( elementPointerResize is null )
            return Task.CompletedTask;

        ClearDragState();

        return InvokeAsync( StateHasChanged );
    }

    private void ApplyElementPointerResize( ReportDefinition definition, ReportElementPointerResizeState pointerResize )
    {
        if ( definition is null || pointerResize is null )
            return;

        var deltaX = pointerResize.TargetX - pointerResize.OriginalX;
        var deltaY = pointerResize.TargetY - pointerResize.OriginalY;
        var deltaWidth = pointerResize.TargetWidth - pointerResize.OriginalWidth;
        var deltaHeight = pointerResize.TargetHeight - pointerResize.OriginalHeight;

        foreach ( var item in pointerResize.SelectedElements )
        {
            if ( !FindElementLocation( definition, item.ElementKey, out var sectionIndex, out _, out var element ) )
                continue;

            var section = definition.Sections[sectionIndex];
            var minimumHeight = GetMinimumElementHeight( element );
            var targetWidth = Math.Max( 8, item.OriginalWidth + deltaWidth );
            var targetHeight = Math.Max( minimumHeight, item.OriginalHeight + deltaHeight );
            var targetX = item.OriginalX + deltaX;
            var targetY = item.OriginalY + deltaY;

            element.Width = Math.Min( targetWidth, Math.Max( 8, definition.Page.Width ) );
            element.Height = targetHeight;
            element.X = ClampDesignerValue( targetX, 0, Math.Max( 0, definition.Page.Width - element.Width ) );
            element.Y = Math.Max( 0, targetY );

            SyncMatchingPageHeaderForDetailElement(
                definition,
                sectionIndex,
                sectionIndex,
                element,
                item.OriginalX,
                item.OriginalWidth,
                element.X,
                element.Width,
                pointerResize.SelectedElements.Select( selectedItem => selectedItem.ElementKey ) );

            GrowSectionToFitElement( section, element );
        }
    }

    private static void GrowSectionToFitElements( ReportSectionDefinition section )
    {
        if ( section is null )
            return;

        foreach ( var element in section.Elements )
        {
            GrowSectionToFitElement( section, element );
        }
    }

    private static void GrowSectionToFitElement( ReportSectionDefinition section, ReportElementDefinition element )
    {
        if ( section is null || element is null )
            return;

        section.Height = Math.Max( section.Height, element.Y + Math.Max( GetMinimumElementHeight( element ), element.Height ) );
    }

    private async Task PreviewSectionPointerResizeAsync( PointerEventArgs eventArgs )
    {
        await PreviewSectionPointerResizeAsync( eventArgs.ClientY );
    }

    private async Task PreviewSectionPointerResizeAsync( double clientY )
    {
        if ( sectionPointerResize is null )
            return;

        var height = CreateSectionPointerResizeHeight( clientY );

        if ( Math.Abs( sectionPointerResize.TargetHeight - height ) < .1 )
            return;

        sectionPointerResize.TargetHeight = height;

        await InvokeAsync( StateHasChanged );
    }

    private async Task CompleteSectionPointerResizeAsync( PointerEventArgs eventArgs )
    {
        await CompleteSectionPointerResizeAsync( eventArgs.ClientY );
    }

    private async Task CompleteSectionPointerResizeAsync( double clientY )
    {
        if ( sectionPointerResize is null )
            return;

        var pointerResize = sectionPointerResize;
        pointerResize.TargetHeight = CreateSectionPointerResizeHeight( pointerResize, clientY );
        sectionPointerResize = null;

        try
        {
            var resized = Math.Abs( pointerResize.TargetHeight - pointerResize.OriginalHeight ) > .1;

            var definition = EffectiveDefinition;
            var canResize = pointerResize.SectionIndex >= 0
                && pointerResize.SectionIndex < definition.Sections.Count
                && !definition.Sections[pointerResize.SectionIndex].Suppressed;

            if ( !resized || !canResize )
                return;

            await ExecuteDesignerCommandAsync( new( "Resize band", () =>
            {
                var definition = EffectiveDefinition;

                if ( pointerResize.SectionIndex >= 0
                    && pointerResize.SectionIndex < definition.Sections.Count
                    && !definition.Sections[pointerResize.SectionIndex].Suppressed )
                {
                    definition.Sections[pointerResize.SectionIndex].Height = pointerResize.TargetHeight;
                    SelectSection( pointerResize.SectionIndex );
                }

                return Task.CompletedTask;
            } ) );
        }
        finally
        {
            await InvokeAsync( StateHasChanged );
        }
    }

    private Task CancelSectionPointerResizeAsync()
    {
        if ( sectionPointerResize is null )
            return Task.CompletedTask;

        ClearDragState();

        return InvokeAsync( StateHasChanged );
    }

    private double CreateSectionPointerResizeHeight( double clientY )
    {
        if ( sectionPointerResize is null )
            return 0;

        return CreateSectionPointerResizeHeight( sectionPointerResize, clientY );
    }

    private double CreateSectionPointerResizeHeight( ReportSectionPointerResizeState pointerResize, double clientY )
    {
        return Math.Max( 8, ApplyDesignerGrid( pointerResize.OriginalHeight + clientY - pointerResize.StartClientY ) );
    }

    /// <summary>
    /// Previews a document-level band resize while the pointer is moving.
    /// </summary>
    /// <param name="clientY">Current document pointer Y coordinate.</param>
    [JSInvokable]
    public Task OnDocumentSectionResizeMove( double clientY )
    {
        return InvokeAsync( () => PreviewSectionPointerResizeAsync( clientY ) );
    }

    /// <summary>
    /// Completes a document-level band resize and commits the final band height.
    /// </summary>
    /// <param name="clientY">Final document pointer Y coordinate.</param>
    [JSInvokable]
    public Task OnDocumentSectionResizeEnd( double clientY )
    {
        return InvokeAsync( () => CompleteSectionPointerResizeAsync( clientY ) );
    }

    /// <summary>
    /// Cancels the active document-level band resize.
    /// </summary>
    [JSInvokable]
    public Task OnDocumentSectionResizeCancel()
    {
        return InvokeAsync( CancelSectionPointerResizeAsync );
    }

    private async Task StartDocumentSectionResizeAsync( double startClientY )
    {
        reportingModule ??= await JSRuntime.InvokeAsync<IJSObjectReference>( "import", "./_content/Blazorise.Reporting/blazorise.reporting.js" );
        dotNetObjectReference ??= DotNetObjectReference.Create( this );

        await reportingModule.InvokeVoidAsync( "startSectionResize", dotNetObjectReference, startClientY );
    }

    private ReportDesignerDragPreview CreateElementPointerResizePreview( PointerEventArgs eventArgs )
    {
        if ( elementPointerResize is null || draggedElement is null )
            return null;

        var deltaX = eventArgs.ClientX - elementPointerResize.StartClientX;
        var deltaY = eventArgs.ClientY - elementPointerResize.StartClientY;
        var left = elementPointerResize.OriginalX;
        var top = elementPointerResize.OriginalY;
        var right = elementPointerResize.OriginalX + elementPointerResize.OriginalWidth;
        var bottom = elementPointerResize.OriginalY + elementPointerResize.OriginalHeight;
        var resizingLeft = HasResizeHandle( elementPointerResize.Handle, ReportElementResizeHandle.West );
        var resizingTop = HasResizeHandle( elementPointerResize.Handle, ReportElementResizeHandle.North );

        if ( resizingLeft )
            left += deltaX;
        else if ( HasResizeHandle( elementPointerResize.Handle, ReportElementResizeHandle.East ) )
            right += deltaX;

        if ( resizingTop )
            top += deltaY;
        else if ( HasResizeHandle( elementPointerResize.Handle, ReportElementResizeHandle.South ) )
            bottom += deltaY;

        left = ApplyDesignerGrid( left );
        top = ApplyDesignerGrid( top );
        right = ApplyDesignerGrid( right );
        bottom = ApplyDesignerGrid( bottom );

        if ( right - left < 8 )
        {
            if ( resizingLeft )
                left = right - 8;
            else
                right = left + 8;
        }

        if ( bottom - top < elementPointerResize.MinimumHeight )
        {
            if ( resizingTop )
                top = bottom - elementPointerResize.MinimumHeight;
            else
                bottom = top + elementPointerResize.MinimumHeight;
        }

        left = Math.Max( 0, left );
        top = Math.Max( 0, top );

        return new()
        {
            SectionIndex = elementPointerResize.SourceSectionIndex,
            ElementType = draggedElement.Type,
            Text = draggedElement.Type == ReportElementType.Field ? FormatFieldExpression( draggedElement ) : draggedElement.Text ?? draggedElement.Name ?? draggedElement.Type.ToString(),
            X = left,
            Y = top,
            Width = Math.Max( 8, right - left ),
            Height = Math.Max( elementPointerResize.MinimumHeight, bottom - top ),
        };
    }

    private static bool HasResizeHandle( ReportElementResizeHandle handle, ReportElementResizeHandle flag )
        => ( handle & flag ) == flag;

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
                    var fieldBinding = NormalizeFieldBindingForSection( targetSection, draggedDataSourceName, draggedFieldName );
                    var fieldElement = new ReportElementDefinition
                    {
                        Name = fieldBinding.FieldName,
                        Type = ReportElementType.Field,
                        Field = fieldBinding.FieldName,
                        DataSource = fieldBinding.DataSourceName,
                        X = x,
                        Y = y,
                        Width = 160,
                        Height = 24,
                    };
                    targetSection.Elements.Add( fieldElement );
                    AddPageHeaderForDetailField( definition, targetSectionIndex, targetSection, fieldBinding.FieldName, x, fieldElement.Width );
                    SelectElement( GetDesignerElementKey( fieldElement ) );
                    break;
                case ReportDesignerDragKind.ToolboxElement when draggedElementType is not null:
                    var toolboxElement = CreateElementFromToolbox( draggedElementType.Value, draggedElementText, x, y );
                    targetSection.Elements.Add( toolboxElement );
                    SelectElement( GetDesignerElementKey( toolboxElement ) );
                    break;
                case ReportDesignerDragKind.Element when FindElementLocation( definition, draggedElementKey, out var sourceSectionIndex, out var sourceElementIndex, out var element ):
                    var originalX = element.X;
                    var originalWidth = element.Width;

                    definition.Sections[sourceSectionIndex].Elements.RemoveAt( sourceElementIndex );
                    element.X = x;
                    element.Y = y;
                    targetSection.Elements.Add( element );
                    SyncMatchingPageHeaderForDetailElement( definition, sourceSectionIndex, targetSectionIndex, element, originalX, originalWidth, element.X, element.Width );
                    SelectElement( GetDesignerElementKey( element ) );
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
            ReportDesignerDragKind.Field when !string.IsNullOrWhiteSpace( draggedFieldName ) => CreateFieldDragPreview( targetSectionIndex, x, y ),
            ReportDesignerDragKind.ToolboxElement when draggedElementType is not null => CreateDragPreview( targetSectionIndex, CreateElementFromToolbox( draggedElementType.Value, draggedElementText, x, y ) ),
            ReportDesignerDragKind.Element when draggedElement is not null => CreateDragPreview( targetSectionIndex, draggedElement, x, y ),
            _ => null,
        };
    }

    private ReportDesignerDragPreview CreateFieldDragPreview( int targetSectionIndex, double x, double y )
    {
        var definition = EffectiveDefinition;
        var targetSection = targetSectionIndex >= 0 && targetSectionIndex < definition.Sections.Count
            ? definition.Sections[targetSectionIndex]
            : null;
        var fieldBinding = NormalizeFieldBindingForSection( targetSection, draggedDataSourceName, draggedFieldName );

        return new()
        {
            SectionIndex = targetSectionIndex,
            ElementType = ReportElementType.Field,
            Text = FormatFieldExpression( fieldBinding.DataSourceName, fieldBinding.FieldName ),
            X = x,
            Y = y,
            Width = 160,
            Height = 24,
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

        foreach ( var field in ResolveDataSourceFields( item, null, 0, [] ) )
        {
            yield return field;
        }
    }

    private static IEnumerable<ReportDesignerFieldNode> ResolveDataSourceFields( object item, string parentPath, int depth, HashSet<Type> visitedTypes )
    {
        if ( item is null || depth > 4 )
            yield break;

        if ( item is IDictionary<string, object> dictionary )
        {
            foreach ( var key in dictionary.Keys.OrderBy( x => x ) )
            {
                dictionary.TryGetValue( key, out var value );

                yield return CreateDesignerFieldNode( key, parentPath, value?.GetType(), value, depth, visitedTypes );
            }

            yield break;
        }

        if ( item is IDictionary nonGenericDictionary )
        {
            foreach ( var key in nonGenericDictionary.Keys.OfType<object>().Select( key => new { Key = key, Name = Convert.ToString( key, CultureInfo.InvariantCulture ) } ).Where( x => !string.IsNullOrWhiteSpace( x.Name ) ).OrderBy( x => x.Name ) )
            {
                var value = nonGenericDictionary[key.Key];

                yield return CreateDesignerFieldNode( key.Name, parentPath, value?.GetType(), value, depth, visitedTypes );
            }

            yield break;
        }

        var itemType = item.GetType();

        if ( IsSimpleFieldType( itemType ) || !visitedTypes.Add( itemType ) )
            yield break;

        foreach ( var property in itemType.GetProperties( BindingFlags.Instance | BindingFlags.Public ).Where( x => x.CanRead && x.GetIndexParameters().Length == 0 ).OrderBy( x => x.Name ) )
        {
            var value = property.GetValue( item );

            yield return CreateDesignerFieldNode( property.Name, parentPath, property.PropertyType, value, depth, visitedTypes );
        }

        visitedTypes.Remove( itemType );
    }

    private static ReportDesignerFieldNode CreateDesignerFieldNode( string name, string parentPath, Type dataType, object value, int depth, HashSet<Type> visitedTypes )
    {
        var path = string.IsNullOrWhiteSpace( parentPath ) ? name : $"{parentPath}.{name}";
        var sampleValue = ResolveSampleItem( value );
        var node = new ReportDesignerFieldNode
        {
            Name = name,
            Path = path,
            DataType = GetDesignerFieldDataType( dataType, sampleValue ),
        };

        if ( sampleValue is not null && !IsSimpleFieldType( sampleValue.GetType() ) )
        {
            node.Children = ResolveDataSourceFields( sampleValue, path, depth + 1, visitedTypes ).ToList();
        }

        return node;
    }

    private static Type GetDesignerFieldDataType( Type declaredType, object sampleValue )
    {
        var enumerableItemType = GetEnumerableItemType( declaredType );

        return enumerableItemType ?? sampleValue?.GetType() ?? declaredType;
    }

    private static Type GetEnumerableItemType( Type type )
    {
        if ( type is null || type == typeof( string ) )
            return null;

        if ( type.IsArray )
            return type.GetElementType();

        if ( type.IsGenericType && type.GetGenericTypeDefinition() == typeof( IEnumerable<> ) )
            return type.GetGenericArguments()[0];

        return type.GetInterfaces()
            .FirstOrDefault( x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof( IEnumerable<> ) )
            ?.GetGenericArguments()[0];
    }

    private static bool IsSimpleFieldType( Type type )
    {
        type = Nullable.GetUnderlyingType( type ) ?? type;

        return type.IsPrimitive
            || type.IsEnum
            || type == typeof( string )
            || type == typeof( decimal )
            || type == typeof( DateTime )
            || type == typeof( DateTimeOffset )
            || type == typeof( TimeSpan )
            || type == typeof( Guid );
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

    private static ReportDesignerDragPreview ConstrainDesignerDragPreview( ReportDefinition definition, ReportDesignerDragPreview preview )
    {
        if ( preview is null )
            return null;

        var section = GetDesignerSection( definition, preview.SectionIndex );

        if ( definition?.Page is null || section is null )
            return preview;

        var minimumHeight = GetMinimumElementHeight( preview.ElementType );

        preview.Width = Math.Min( Math.Max( 8, preview.Width ), Math.Max( 8, definition.Page.Width ) );
        preview.Height = Math.Max( minimumHeight, preview.Height );
        preview.X = ClampDesignerValue( preview.X, 0, Math.Max( 0, definition.Page.Width - preview.Width ) );
        preview.Y = Math.Max( 0, preview.Y );

        return preview;
    }

    private IEnumerable<string> FindElementsInsideSelectionBox( ReportDefinition definition, ReportDesignerSelectionBox selectionBox )
    {
        if ( definition is null || selectionBox is null )
            yield break;

        for ( var sectionIndex = 0; sectionIndex < definition.Sections.Count; sectionIndex++ )
        {
            var section = definition.Sections[sectionIndex];

            if ( section.Suppressed || IsSectionCollapsed( section ) )
                continue;

            var sectionOffsetY = GetSectionOffsetY( definition, sectionIndex );

            foreach ( var element in section.Elements )
            {
                if ( Intersects( selectionBox.X, selectionBox.Y, selectionBox.Width, selectionBox.Height, element.X, sectionOffsetY + element.Y, element.Width, element.Height ) )
                    yield return GetDesignerElementKey( element );
            }
        }
    }

    private IEnumerable<ReportElementPointerItemState> CaptureElementPointerItems( ReportDefinition definition, string activeElementKey )
    {
        if ( definition is null || string.IsNullOrWhiteSpace( activeElementKey ) )
            yield break;

        List<string> elementKeys = IsElementSelected( activeElementKey ) && selectedElementKeys.Count > 1
            ? selectedElementKeys.ToList()
            : [activeElementKey];

        foreach ( var elementKey in elementKeys )
        {
            if ( !FindElementLocation( definition, elementKey, out var sectionIndex, out _, out var element ) )
                continue;

            yield return new()
            {
                ElementKey = elementKey,
                OriginalX = element.X,
                OriginalY = element.Y,
                OriginalPageY = GetSectionOffsetY( definition, sectionIndex ) + element.Y,
                OriginalWidth = element.Width,
                OriginalHeight = element.Height,
            };
        }
    }

    private static bool Intersects( double left, double top, double width, double height, double otherLeft, double otherTop, double otherWidth, double otherHeight )
    {
        return left < otherLeft + otherWidth
            && left + width > otherLeft
            && top < otherTop + otherHeight
            && top + height > otherTop;
    }

    private static double ClampDesignerValue( double value, double minimum, double maximum )
    {
        return Math.Min( Math.Max( value, minimum ), maximum );
    }

    private static ReportSectionDefinition GetDesignerSection( ReportDefinition definition, int sectionIndex )
    {
        if ( definition is null || sectionIndex < 0 || sectionIndex >= definition.Sections.Count )
            return null;

        return definition.Sections[sectionIndex];
    }

    private double GetSectionOffsetY( ReportDefinition definition, int sectionIndex )
    {
        if ( definition is null || sectionIndex <= 0 )
            return 0;

        var y = 0d;

        for ( var i = 0; i < sectionIndex && i < definition.Sections.Count; i++ )
        {
            y += GetDesignerSectionHeight( i, definition.Sections[i] );
        }

        return y;
    }

    private double GetDesignerContentHeight( ReportDefinition definition )
    {
        if ( definition is null )
            return 0;

        var height = 0d;

        for ( var i = 0; i < definition.Sections.Count; i++ )
        {
            height += GetDesignerSectionHeight( i, definition.Sections[i] );
        }

        return Math.Max( definition.Page?.Height ?? 0, height );
    }

    private double GetDesignerSectionHeight( int sectionIndex, ReportSectionDefinition section )
    {
        if ( sectionPointerResize is not null && sectionPointerResize.SectionIndex == sectionIndex )
            return sectionPointerResize.TargetHeight;

        return BandMode == ReportBandMode.Rail && section is not null && !section.Suppressed && IsSectionCollapsed( section )
            ? DesignerCollapsedBandHeight
            : section?.Height ?? 0;
    }

    private static double GetMinimumElementHeight( ReportElementDefinition element )
    {
        return element?.Type == ReportElementType.Line ? 1 : 8;
    }

    private static double GetMinimumElementHeight( ReportElementType elementType )
    {
        return elementType == ReportElementType.Line ? 1 : 8;
    }

    private void OnSnapToGridChanged( ChangeEventArgs eventArgs )
    {
        snapToGrid = eventArgs.Value is bool value
            ? value
            : string.Equals( Convert.ToString( eventArgs.Value, CultureInfo.InvariantCulture ), "true", StringComparison.OrdinalIgnoreCase );
    }

    private static double? NormalizeNullablePositiveNumber( double? value )
    {
        return value is > 0
            ? value
            : null;
    }

    private static double? NormalizeOpacity( double? value )
    {
        if ( value is null )
            return null;

        return Math.Clamp( value.Value, 0, 1 );
    }

    private static ReportFontDefinition EnsureFont( ReportElementDefinition element )
    {
        return element.Font ??= new();
    }

    private static ReportAppearanceDefinition EnsureAppearance( ReportElementDefinition element )
    {
        return element.Appearance ??= new();
    }

    private static ReportBorderDefinition EnsureBorder( ReportElementDefinition element )
    {
        return element.Border ??= new();
    }

    private static string NormalizeColorValue( string value )
    {
        return !string.IsNullOrWhiteSpace( value ) && value.StartsWith( "#", StringComparison.Ordinal ) && value.Length == 7
            ? value
            : "#000000";
    }

    private static string ToCssTextAlignment( TextAlignment alignment )
    {
        return alignment switch
        {
            TextAlignment.Start => "left",
            TextAlignment.End => "right",
            TextAlignment.Center => "center",
            TextAlignment.Justified => "justify",
            _ => null,
        };
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
        elementPointerDrag = null;
        elementPointerResize = null;
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

    private static (string DataSourceName, string FieldName) NormalizeFieldBindingForSection( ReportSectionDefinition section, string dataSourceName, string fieldName )
    {
        if ( section is null || string.IsNullOrWhiteSpace( section.DataSource ) || string.IsNullOrWhiteSpace( fieldName ) )
            return (dataSourceName, fieldName);

        var sectionDataSource = section.DataSource.Trim();
        var fieldPath = string.IsNullOrWhiteSpace( dataSourceName )
            ? fieldName
            : $"{dataSourceName}.{fieldName}";
        var sectionPrefix = $"{sectionDataSource}.";

        if ( fieldPath.StartsWith( sectionPrefix, StringComparison.OrdinalIgnoreCase ) )
            return (null, fieldPath[sectionPrefix.Length..]);

        if ( fieldName.StartsWith( sectionPrefix, StringComparison.OrdinalIgnoreCase ) )
            return (null, fieldName[sectionPrefix.Length..]);

        if ( string.Equals( dataSourceName, sectionDataSource, StringComparison.OrdinalIgnoreCase ) )
            return (null, fieldName);

        return (dataSourceName, fieldName);
    }

    private static void AddPageHeaderForDetailField( ReportDefinition definition, int detailSectionIndex, ReportSectionDefinition detailSection, string fieldName, double x, double width )
    {
        if ( detailSection?.Type != ReportSectionType.Detail || string.IsNullOrWhiteSpace( fieldName ) )
            return;

        var pageHeader = FindPageHeaderForDetail( definition, detailSectionIndex );

        if ( pageHeader is null || pageHeader.Suppressed )
            return;

        var headerText = GetFieldHeaderText( fieldName );
        var headerY = GetPageHeaderElementY( pageHeader );

        if ( HasPageHeaderElement( pageHeader, headerText, x ) )
            return;

        pageHeader.Elements.Add( new()
        {
            Name = headerText,
            Type = ReportElementType.Text,
            Text = headerText,
            X = x,
            Y = headerY,
            Width = width,
            Height = 24,
            Font = new()
            {
                Bold = true,
            },
        } );
    }

    private static ReportSectionDefinition FindPageHeaderForDetail( ReportDefinition definition, int detailSectionIndex )
    {
        if ( definition is null )
            return null;

        for ( var i = detailSectionIndex - 1; i >= 0; i-- )
        {
            if ( definition.Sections[i].Type == ReportSectionType.PageHeader )
                return definition.Sections[i];
        }

        return definition.Sections.FirstOrDefault( section => section.Type == ReportSectionType.PageHeader );
    }

    private static double GetPageHeaderElementY( ReportSectionDefinition pageHeader )
    {
        var firstElement = pageHeader.Elements
            .Where( element => element.Type is ReportElementType.Text or ReportElementType.Field )
            .OrderBy( element => element.Y )
            .ThenBy( element => element.X )
            .FirstOrDefault();

        return firstElement?.Y ?? 10;
    }

    private static bool HasPageHeaderElement( ReportSectionDefinition pageHeader, string headerText, double x )
    {
        return pageHeader.Elements.Any( element =>
            element.Type == ReportElementType.Text
            && string.Equals( element.Text, headerText, StringComparison.OrdinalIgnoreCase )
            && Math.Abs( element.X - x ) < 0.1 );
    }

    private void SyncMatchingPageHeaderForDetailElement(
        ReportDefinition definition,
        int sourceSectionIndex,
        int targetSectionIndex,
        ReportElementDefinition detailElement,
        double originalX,
        double originalWidth,
        double newX,
        double newWidth,
        IEnumerable<string> ignoredElementKeys = null )
    {
        if ( definition is null
            || detailElement?.Type != ReportElementType.Field
            || string.IsNullOrWhiteSpace( detailElement.Field )
            || ( Math.Abs( newX - originalX ) < 0.1 && Math.Abs( newWidth - originalWidth ) < 0.1 )
            || sourceSectionIndex < 0
            || sourceSectionIndex >= definition.Sections.Count
            || targetSectionIndex < 0
            || targetSectionIndex >= definition.Sections.Count
            || definition.Sections[sourceSectionIndex].Type != ReportSectionType.Detail
            || definition.Sections[targetSectionIndex].Type != ReportSectionType.Detail )
        {
            return;
        }

        var pageHeader = FindPageHeaderForDetail( definition, sourceSectionIndex );

        if ( pageHeader is null || pageHeader.Suppressed )
            return;

        HashSet<string> ignoredKeys = ignoredElementKeys is null
            ? null
            : new( ignoredElementKeys.Where( key => !string.IsNullOrWhiteSpace( key ) ), StringComparer.Ordinal );

        var headerElement = pageHeader.Elements.FirstOrDefault( element =>
            element.Type == ReportElementType.Text
            && Math.Abs( element.X - originalX ) < 0.1
            && Math.Abs( element.Width - originalWidth ) < 0.1
            && ( ignoredKeys is null || !ignoredKeys.Contains( GetDesignerElementKey( element ) ) ) );

        if ( headerElement is not null )
        {
            headerElement.X = newX;
            headerElement.Width = newWidth;
        }
    }

    private static string GetFieldHeaderText( string fieldName )
    {
        var segment = fieldName.Split( '.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries ).LastOrDefault() ?? fieldName;

        if ( string.IsNullOrWhiteSpace( segment ) )
            return fieldName;

        segment = segment.Replace( '_', ' ' );
        var characters = new List<char>();

        for ( var i = 0; i < segment.Length; i++ )
        {
            var character = segment[i];

            if ( i > 0
                && character != ' '
                && char.IsUpper( character )
                && segment[i - 1] != ' '
                && ( char.IsLower( segment[i - 1] ) || ( i + 1 < segment.Length && char.IsLower( segment[i + 1] ) ) ) )
            {
                characters.Add( ' ' );
            }

            characters.Add( character );
        }

        return new string( characters.ToArray() );
    }

    private ReportSectionDefinition FindSelectedSection( ReportDefinition definition )
    {
        if ( selectedSectionIndex is null || selectedSectionIndex < 0 || selectedSectionIndex >= definition.Sections.Count )
            return null;

        return definition.Sections[selectedSectionIndex.Value];
    }

    private bool CanDeleteSelection( ReportDefinition definition )
    {
        if ( definition is null )
            return false;

        if ( !string.IsNullOrWhiteSpace( selectedElementKey ) )
            return !IsSelectedElementSuppressed( definition ) && FindSelectedElement( definition ) is not null;

        return CanDeleteSection( FindSelectedSection( definition ) );
    }

    private static bool CanDeleteSection( ReportSectionDefinition section )
    {
        return section is not null && !section.Default;
    }

    private bool IsElementSelected( string elementKey )
    {
        return !string.IsNullOrWhiteSpace( elementKey )
            && ( string.Equals( selectedElementKey, elementKey, StringComparison.Ordinal ) || selectedElementKeys.Contains( elementKey ) );
    }

    private bool IsSelectedElementSuppressed( ReportDefinition definition )
    {
        return !string.IsNullOrWhiteSpace( selectedElementKey )
            && FindElementLocation( definition, selectedElementKey, out var sectionIndex, out _, out _ )
            && definition.Sections[sectionIndex].Suppressed;
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

    private IEnumerable<string> GetSelectedElementIds( ReportDefinition definition )
    {
        List<string> elementKeys = selectedElementKeys.Count > 0
            ? selectedElementKeys.ToList()
            : string.IsNullOrWhiteSpace( selectedElementKey ) ? [] : [selectedElementKey];

        foreach ( var elementKey in elementKeys )
        {
            if ( FindElementLocation( definition, elementKey, out _, out _, out var element ) )
                yield return element.Id;
        }
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

    private static string GetSectionDesignerKey( ReportSectionDefinition section )
    {
        if ( section is null )
            return null;

        if ( string.IsNullOrWhiteSpace( section.Id ) )
            section.Id = CreateDefinitionId();

        return section.Id;
    }

    private static ReportDefinition EnsureDefinitionIds( ReportDefinition definition )
    {
        if ( definition is null )
            return null;

        var definitionIds = new HashSet<string>( StringComparer.Ordinal );
        var dataSourceIds = new HashSet<string>( StringComparer.Ordinal );
        var sectionIds = new HashSet<string>( StringComparer.Ordinal );
        var elementIds = new HashSet<string>( StringComparer.Ordinal );
        var columnIds = new HashSet<string>( StringComparer.Ordinal );

        definition.Id = EnsureUniqueDefinitionId( definition.Id, definitionIds );

        foreach ( var dataSource in definition.DataSources )
        {
            dataSource.Id = EnsureUniqueDefinitionId( dataSource.Id, dataSourceIds );
        }

        foreach ( var section in definition.Sections )
        {
            section.Id = EnsureUniqueDefinitionId( section.Id, sectionIds );

            foreach ( var element in section.Elements )
            {
                element.Id = EnsureUniqueDefinitionId( element.Id, elementIds );

                foreach ( var column in element.Columns )
                {
                    column.Id = EnsureUniqueDefinitionId( column.Id, columnIds );
                }
            }
        }

        return definition;
    }

    private static string EnsureUniqueDefinitionId( string id, HashSet<string> usedIds )
    {
        if ( string.IsNullOrWhiteSpace( id ) || !usedIds.Add( id ) )
        {
            do
            {
                id = CreateDefinitionId();
            }
            while ( !usedIds.Add( id ) );
        }

        return id;
    }

    private static string CreateDefinitionId()
        => Guid.NewGuid().ToString( "N" );

    private bool SupportsPreviewFormat( ReportPreviewFormat format )
    {
        return ( PreviewFormats ?? context.ViewerOptions.PreviewFormats ).HasFlag( format );
    }

    #endregion

    #region Properties

    private ReportDefinition EffectiveDefinition
        => DefinitionMode == ReportDefinitionMode.AlwaysUseDeclarative
            ? BuildDeclarativeDefinition()
            : Definition ?? declarativeDefinition ?? BuildDeclarativeDefinition();

    private bool IsDesignerEnabled => DesignerEnabled || GlobalOptions.DesignerEnabled;

    private ReportOptions GlobalOptions => globalOptions ??= ServiceProvider.GetService<ReportOptions>() ?? new();

    private ReportStudioMode CurrentMode => Mode ?? currentMode;

    private ReportPreviewFormat CurrentPreviewFormat => PreviewFormat ?? currentPreviewFormat;

    private string ToolbarStateKey => $"{CurrentMode}|{CurrentPreviewFormat}|{selectedElementKey}|{selectedElementKeys.Count}|{selectedSectionIndex}|{clipboardElement?.Id}|{historyService.CanUndo}|{historyService.CanRedo}";

    private string DataSourceName => "Default";

    /// <summary>
    /// JavaScript runtime used for document-level designer interactions.
    /// </summary>
    [Inject] private IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// Persisted report definition used by the designer and viewer.
    /// </summary>
    [Parameter] public ReportDefinition Definition { get; set; }

    /// <summary>
    /// Raised when the report definition changes through designer commands.
    /// </summary>
    [Parameter] public EventCallback<ReportDefinition> DefinitionChanged { get; set; }

    /// <summary>
    /// Default data source object or enumerable used when no explicit <see cref="ReportDataSource"/> is declared.
    /// </summary>
    [Parameter] public object Data { get; set; }

    /// <summary>
    /// Page settings used by the declarative report seed.
    /// </summary>
    [Parameter] public ReportPageDefinition Page { get; set; }

    /// <summary>
    /// Enables the interactive designer surface for this report.
    /// </summary>
    [Parameter] public bool DesignerEnabled { get; set; }

    /// <summary>
    /// Shows the report toolbar above the designer or viewer surface.
    /// </summary>
    [Parameter] public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// Band presentation used by the designer.
    /// </summary>
    [Parameter] public ReportBandMode BandMode { get; set; } = ReportBandMode.Rail;

    /// <summary>
    /// Enables collapsing and expanding bands in the designer rail.
    /// </summary>
    [Parameter] public bool AllowBandCollapse { get; set; } = true;

    /// <summary>
    /// Shows data source names in band labels when available.
    /// </summary>
    [Parameter] public bool ShowBandDataSource { get; set; } = true;

    /// <summary>
    /// Controls how declarative child content is used with persisted definitions.
    /// </summary>
    [Parameter] public ReportDefinitionMode DefinitionMode { get; set; } = ReportDefinitionMode.SeedWhenEmpty;

    /// <summary>
    /// Externally controlled design or preview mode.
    /// </summary>
    [Parameter] public ReportStudioMode? Mode { get; set; }

    /// <summary>
    /// Raised when design or preview mode changes.
    /// </summary>
    [Parameter] public EventCallback<ReportStudioMode> ModeChanged { get; set; }

    /// <summary>
    /// Externally controlled preview format.
    /// </summary>
    [Parameter] public ReportPreviewFormat? PreviewFormat { get; set; }

    /// <summary>
    /// Preview formats available for this report.
    /// </summary>
    [Parameter] public ReportPreviewFormat? PreviewFormats { get; set; }

    /// <summary>
    /// Preview format selected when preview mode is first opened.
    /// </summary>
    [Parameter] public ReportPreviewFormat? DefaultPreviewFormat { get; set; }

    /// <summary>
    /// Declarative report content used as the initial report definition.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion

    #region Data structures

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

    [Flags]
    private enum ReportElementResizeHandle
    {
        North = 1,
        East = 2,
        South = 4,
        West = 8,
        NorthEast = North | East,
        SouthEast = South | East,
        SouthWest = South | West,
        NorthWest = North | West
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

    private sealed class ReportDesignerSelectionBox
    {
        public int SectionIndex { get; set; }

        public double StartX { get; set; }

        public double StartY { get; set; }

        public double CurrentX { get; set; }

        public double CurrentY { get; set; }

        public double StartClientX { get; set; }

        public double StartClientY { get; set; }

        public bool Additive { get; set; }

        public bool HasMoved { get; set; }

        public double X => Math.Min( StartX, CurrentX );

        public double Y => Math.Min( StartY, CurrentY );

        public double Width => Math.Abs( CurrentX - StartX );

        public double Height => Math.Abs( CurrentY - StartY );
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

        public double TargetX { get; set; }

        public double TargetY { get; set; }

        public bool HasMoved { get; set; }

        public List<ReportElementPointerItemState> SelectedElements { get; set; } = [];
    }

    private sealed class ReportElementPointerResizeState
    {
        public string ElementKey { get; set; }

        public int SourceSectionIndex { get; set; }

        public ReportElementResizeHandle Handle { get; set; }

        public double OriginalX { get; set; }

        public double OriginalY { get; set; }

        public double OriginalWidth { get; set; }

        public double OriginalHeight { get; set; }

        public double StartClientX { get; set; }

        public double StartClientY { get; set; }

        public double TargetX { get; set; }

        public double TargetY { get; set; }

        public double TargetWidth { get; set; }

        public double TargetHeight { get; set; }

        public double MinimumHeight { get; set; }

        public bool HasResized { get; set; }

        public List<ReportElementPointerItemState> SelectedElements { get; set; } = [];
    }

    private sealed class ReportElementPointerItemState
    {
        public string ElementKey { get; set; }

        public double OriginalX { get; set; }

        public double OriginalY { get; set; }

        public double OriginalPageY { get; set; }

        public double OriginalWidth { get; set; }

        public double OriginalHeight { get; set; }
    }

    private sealed class ReportSectionPointerResizeState
    {
        public int SectionIndex { get; set; }

        public double OriginalHeight { get; set; }

        public double TargetHeight { get; set; }

        public double StartClientY { get; set; }
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

        public string Path { get; set; }

        public Type DataType { get; set; }

        public List<ReportDesignerFieldNode> Children { get; set; } = [];
    }

    #endregion
}