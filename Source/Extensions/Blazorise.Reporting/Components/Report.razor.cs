#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Pdf;
using Blazorise.Reporting.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using DesignerConstants = Blazorise.Reporting.Internal.ReportDesignerConstants;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Provides a declarative report designer and viewer for band-based report definitions.
/// </summary>
public partial class Report : ComponentBase, IReportCommandExecutor, IAsyncDisposable
{
    #region Members

    private readonly ReportContext context = new();

    private readonly ReportToolbarContext toolbarContext;

    private readonly ReportDesignerCommandManager commandManager = new();

    private readonly ReportSelectionManager selectionManager = new();

    private readonly ReportContextMenuService contextMenuService = new();

    private readonly string modalProviderName = $"blazorise-report-{Guid.NewGuid():N}";

    private readonly ReportClipboardService clipboardService = new();

    private readonly ReportAggregateService aggregateService = new();

    private readonly ReportDataDefinitionService dataDefinitionService = new();

    private readonly ReportDataCommandService dataCommandService = new();

    private readonly ReportDesignerLayoutService designerLayoutService = new();

    private readonly ReportDesignerDragDropService dragDropService = new();

    private readonly ReportElementLayoutService elementLayoutService = new();

    private readonly ReportElementCommandService elementCommandService;

    private readonly ReportTableEditor tableEditor = new();

    private readonly ReportTableCommandService tableCommandService;

    private readonly ReportTableResizeService tableResizeService = new();

    private readonly ReportRenderService renderService = new();

    private readonly ReportPreviewExportService previewExportService = new();

    private readonly ReportSectionCommandService sectionCommandService = new();

    private readonly ReportStateService stateService = new();

    private readonly ReportDesignerRulerService rulerService = new();

    private readonly ReportDesignerInteractionState designerState = new();

    private readonly Dictionary<string, (double Left, double Top)> designerPaneScrollPositions = new( StringComparer.Ordinal );

    private readonly HashSet<string> collapsedBandIds = new( StringComparer.Ordinal );

    private readonly IReadOnlyList<IReportDataSourceProvider> fallbackDataSourceProviders =
    [
        new ObjectReportDataSourceProvider(),
        new DataSetReportDataSourceProvider(),
    ];

    private const string MainReportDesignerTabKey = "__main";

    private const string DesignerSurfacePaneName = "report-designer";

    private DotNetObjectReference<Report> dotNetObjectReference;

    private ReportDefinition declarativeDefinition;

    private ReportMode currentMode;

    private ReportPreviewFormat currentPreviewFormat;

    private ReportDesignerPanelTab selectedDesignerPanelTab = ReportDesignerPanelTab.Properties;

    private int designerPaneScrollRestoreVersion;

    private _ReportDesignerContextMenuHost contextMenuHost;

    private _ReportDesignerLayout designerLayoutRef;

    private _ReportDesignerPage designerPageRef;

    private (ReportDefinition Definition, object Data) observedParameters;

    private int collapsedSectionsVersion;

    private List<ReportElementDefinition> clipboardElements = [];

    private string clipboardBandId;

    private ReportOptions globalOptions;

    private IReportDataSourceProviderRegistry dataSourceProviderRegistry;

    private JSReportingModule reportingModule;

    private _ReportDesignerAggregateDialog aggregateDialogRef;

    private _ReportDesignerRunningTotalDialog runningTotalDialogRef;

    private _ReportDesignerGroupDialog groupDialogRef;

    private _ReportDesignerDataSourceConnectionDialog dataSourceConnectionDialogRef;

    private _ReportDesignerFormulaDialog formulaDialogRef;

    private string editingFormulaFieldName;

    private string activeSubreportElementKey;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new report component instance.
    /// </summary>
    public Report()
    {
        toolbarContext = new( this );
        elementCommandService = new( elementLayoutService );
        tableCommandService = new( tableEditor );
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

        currentMode = IsDesignerEnabled ? ReportMode.Design : ReportMode.Preview;
        currentPreviewFormat = DefaultPreviewFormat ?? context.ViewerOptions.DefaultFormat;
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        if ( !ReferenceEquals( observedParameters.Definition, Definition ) || !ReferenceEquals( observedParameters.Data, Data ) )
        {
            observedParameters = (Definition, Data);
            InvalidateDesignerCaches();
        }

        if ( Definition is not null )
            await ResolveDataSources( Definition, CurrentMode == ReportMode.Preview );
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender && Definition is null && CurrentDefinitionMode != ReportDefinitionMode.UseDefinitionOnly )
        {
            declarativeDefinition = BuildDeclarativeDefinition();
            InvalidateDesignerCaches();

            await ResolveDataSources( declarativeDefinition, CurrentMode == ReportMode.Preview );

            if ( DefinitionChanged.HasDelegate )
            {
                await DefinitionChanged.InvokeAsync( declarativeDefinition );
            }

            StateHasChanged();
        }

        if ( firstRender && commandManager.State?.Definition is null )
            commandManager.SetState( CaptureReportState( RootDefinition ) );
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if ( reportingModule is not null )
        {
            try
            {
                await reportingModule.StopSectionResize();
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
                ProviderType = ObjectReportDataSourceProvider.ProviderType,
                Data = Data,
            } );
        }

        definition.Page = ResolvePage( definition.Page );

        return ReportDefinitionHelper.EnsureDefinitionIds( definition );
    }

    private ReportPageDefinition ResolvePage( ReportPageDefinition page )
    {
        return ReportPageDefinitionHelper.ResolvePage( page );
    }

    private async Task ResolveDataSources( ReportDefinition definition, bool loadData )
    {
        if ( definition?.DataSources is null )
            return;

        IReportDataSourceProviderRegistry registry = DataSourceProviderRegistry;

        if ( registry is null )
            return;

        foreach ( ReportDataSourceDefinition dataSource in definition.DataSources )
        {
            if ( dataSource is null )
                continue;

            IReportDataSourceProvider provider = registry.FindProvider( dataSource?.ProviderType );

            if ( provider is null )
                continue;

            try
            {
                if ( loadData && ShouldLoadDataSource( provider, dataSource ) )
                {
                    ReportDataSourceResult result = await provider.LoadDataAsync( dataSource, new()
                    {
                        DefaultData = Data,
                    } );

                    dataSource.Data = result?.Data;
                    dataSource.Schema = result?.Schema ?? dataSource.Schema;

                    continue;
                }

                if ( dataSource.Schema is null )
                    dataSource.Schema = await provider.GetSchemaAsync( dataSource );
            }
            catch
            {
            }
        }
    }

    private static bool ShouldLoadDataSource( IReportDataSourceProvider provider, ReportDataSourceDefinition dataSource )
    {
        return dataSource?.Data is null
            || string.Equals( provider?.Type, DataSetReportDataSourceProvider.ProviderType, StringComparison.OrdinalIgnoreCase );
    }

    private bool IsElementContextMenuVisible()
    {
        return designerState.ContextMenu?.Visible == true
            && designerState.ContextMenu.Target == ReportContextMenuTarget.Element;
    }

    private ReportBandDefinition GetSelectedPropertiesSection( ReportDefinition definition )
    {
        return selectionManager.SelectedElementKeys.Count == 0
            ? selectionManager.FindSelectedSection( definition )
            : null;
    }

    private ReportBandDefinition GetSelectedFormulaSection( ReportDefinition definition )
    {
        if ( selectionManager.SelectedElementKeys.Count == 0 )
            return selectionManager.FindSelectedSection( definition );

        return ReportDefinitionHelper.TryFindElementLocation( definition, selectionManager.PrimaryElementKey, out var sectionIndex, out _, out _ )
            ? definition.Bands[sectionIndex]
            : null;
    }

    private Task SelectDesignerPanelTab( string tab )
    {
        selectedDesignerPanelTab = string.Equals( tab, nameof( ReportDesignerPanelTab.Explorer ), StringComparison.Ordinal )
            ? ReportDesignerPanelTab.Explorer
            : ReportDesignerPanelTab.Properties;

        return Task.CompletedTask;
    }

    private Task OnReportTreeNodeClicked( ReportTreeNode node )
    {
        selectedDesignerPanelTab = ReportDesignerPanelTab.Properties;

        if ( string.Equals( node?.Key, "report", StringComparison.Ordinal ) )
        {
            SelectReport();
        }
        else if ( ReportDesignerTreeBuilder.TryResolveSectionTreeNode( node, out var sectionIndex ) )
        {
            SelectSection( sectionIndex );
        }
        else if ( ReportDesignerTreeBuilder.TryResolveElementTreeNode( node, out var elementKey ) )
        {
            if ( ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, elementKey, out var elementSectionIndex, out _, out _ )
                && ReportValueResolver.ResolveStaticSuppress( EffectiveDefinition.Bands[elementSectionIndex] ) )
            {
                SelectSection( elementSectionIndex );
            }
            else
            {
                SelectElement( elementKey );
            }
        }
        else if ( ReportDesignerTreeBuilder.TryResolveTableCellTreeNode( node, out var cellKey ) )
        {
            SelectTableCell( cellKey );
        }

        return Task.CompletedTask;
    }

    private async Task OnReportTreeNodeContextMenu( ReportTreeNodeMouseEventArgs eventArgs )
    {
        if ( ReportDesignerTreeBuilder.TryResolveSectionTreeNode( eventArgs.Node, out var sectionIndex ) )
        {
            await OpenSectionContextMenu( sectionIndex, eventArgs.MouseEventArgs );
        }
        else if ( ReportDesignerTreeBuilder.TryResolveElementTreeNode( eventArgs.Node, out var elementKey ) )
        {
            await OpenElementContextMenu( elementKey, eventArgs.MouseEventArgs );
        }
        else if ( ReportDesignerTreeBuilder.TryResolveTableCellTreeNode( eventArgs.Node, out var cellKey )
            && ReportDefinitionHelper.TryFindTableCellLocation( EffectiveDefinition, cellKey, out var cellSectionIndex, out _, out _, out _ ) )
        {
            await OpenTableCellContextMenu( cellSectionIndex, cellKey, eventArgs.MouseEventArgs );
        }
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

    private IReadOnlyList<object> ResolveSectionRenderItems( ReportDefinition definition, ReportBandDefinition section, bool designMode )
    {
        return renderService.ResolveSectionRenderItems( definition, section, Data, designMode );
    }

    private double GetReportPageWidth( ReportDefinition definition )
    {
        return renderService.GetPageWidth( definition );
    }

    private double GetReportPageContentWidth( ReportDefinition definition )
    {
        return renderService.GetPageContentWidth( definition );
    }

    private double GetDesignerSectionBodyLeft( ReportDefinition definition )
    {
        return GetReportPageWidthOffset() + ReportMeasurementConverter.ToCssPixelValue( GetReportPageMarginLeft( definition ) );
    }

    private double GetReportPageHeight( ReportDefinition definition )
    {
        return definition?.Page?.Height ?? 0;
    }

    private static ReportPageMarginsDefinition GetReportPageMargins( ReportDefinition definition )
    {
        return definition?.Page?.Margins ?? new();
    }

    private static double GetReportPageMarginBottom( ReportDefinition definition )
    {
        return Math.Max( 0, GetReportPageMargins( definition ).Bottom );
    }

    private static double GetReportPageMarginLeft( ReportDefinition definition )
    {
        return Math.Max( 0, GetReportPageMargins( definition ).Left );
    }

    private static double GetReportPageMarginRight( ReportDefinition definition )
    {
        return Math.Max( 0, GetReportPageMargins( definition ).Right );
    }

    private static double GetReportPageMarginTop( ReportDefinition definition )
    {
        return Math.Max( 0, GetReportPageMargins( definition ).Top );
    }

    private string GetPreviewPageContentStyle( ReportDefinition definition )
    {
        return renderService.GetPreviewPageContentStyle( definition );
    }

    private string GetPreviewPageFooterStyle( ReportDefinition definition, ReportRenderPage renderPage )
    {
        return renderService.GetPreviewPageFooterStyle( definition, renderPage, GetDesignerSectionHeight );
    }

    private double GetSectionRenderHeight( int sectionIndex, ReportBandDefinition section )
    {
        return GetDesignerSectionHeight( sectionIndex, section );
    }

    private double GetReportPageWidthOffset()
    {
        return IsDesignerBandRailVisible() ? DesignerConstants.DesignerBandRailWidth : 0;
    }

    private double GetSelectionBoxLeftOffset()
    {
        return GetDesignerSectionBodyLeft( EffectiveDefinition );
    }

    private double GetDesignerSectionBodyTopOffset()
    {
        return BandMode == ReportBandMode.Classic ? ReportMeasurementConverter.FromCssPixelValue( DesignerConstants.DesignerBandHeaderHeight ) : 0;
    }

    private bool IsDesignerBandRailVisible()
    {
        return BandMode == ReportBandMode.Rail;
    }

    private bool IsSectionCollapsedForRender( ReportBandDefinition section )
    {
        return IsDesignerBandRailVisible() && !ReportValueResolver.ResolveStaticSuppress( section ) && IsSectionCollapsed( section );
    }

    private bool IsSectionSelected( int sectionIndex )
    {
        return selectionManager.SelectedSectionIndex == sectionIndex
            && selectionManager.SelectedElementKeys.Count == 0;
    }

    private async Task HandleDesignerShortcut( ReportDesignerShortcut shortcut )
    {
        if ( CurrentMode != ReportMode.Design || !IsDesignerEnabled || IsElementTextEditing() )
            return;

        switch ( shortcut )
        {
            case ReportDesignerShortcut.Cut:
                await ExecuteCommandIfAvailable( ReportCommand.Cut );
                break;

            case ReportDesignerShortcut.Copy:
                await ExecuteCommandIfAvailable( ReportCommand.Copy );
                break;

            case ReportDesignerShortcut.Paste:
                await ExecuteCommandIfAvailable( ReportCommand.Paste );
                break;

            case ReportDesignerShortcut.Undo:
                await ExecuteCommandIfAvailable( ReportCommand.Undo );
                break;

            case ReportDesignerShortcut.Redo:
                await ExecuteCommandIfAvailable( ReportCommand.Redo );
                break;

            case ReportDesignerShortcut.Delete:
                if ( selectionManager.SelectedElementKeys.Count > 0 )
                    await DeleteSelectedElement();
                break;

            case ReportDesignerShortcut.EditText:
                BeginSelectedElementTextEdit();
                break;

            case ReportDesignerShortcut.MoveLeft:
                await MoveSelectedElements( -DesignerConstants.KeyboardMoveStep, 0 );
                break;

            case ReportDesignerShortcut.MoveUp:
                await MoveSelectedElements( 0, -DesignerConstants.KeyboardMoveStep );
                break;

            case ReportDesignerShortcut.MoveRight:
                await MoveSelectedElements( DesignerConstants.KeyboardMoveStep, 0 );
                break;

            case ReportDesignerShortcut.MoveDown:
                await MoveSelectedElements( 0, DesignerConstants.KeyboardMoveStep );
                break;
        }
    }

    private async Task ExecuteCommandIfAvailable( ReportCommand command )
    {
        if ( CanExecuteCommand( command ) )
            await ExecuteCommand( command );
    }

    /// <summary>
    /// Executes a report command against the current designer or viewer state.
    /// </summary>
    /// <param name="command">Command requested by a toolbar item or external caller.</param>
    public async Task ExecuteCommand( ReportCommand command )
    {
        await ( command switch
        {
            ReportCommand.Design => SetMode( ReportMode.Design ),
            ReportCommand.Preview => SetPreview( SupportsPreviewFormat( currentPreviewFormat ) ? currentPreviewFormat : context.ViewerOptions.DefaultFormat ),
            ReportCommand.PreviewHtml => SetPreview( ReportPreviewFormat.Html ),
            ReportCommand.PreviewPdf => SetPreview( ReportPreviewFormat.Pdf ),
            ReportCommand.ConnectDataSource => OpenDataSourceConnectionDialog(),
            ReportCommand.DownloadPdf => DownloadPdf(),
            ReportCommand.Cut => CutSelectedElement(),
            ReportCommand.Copy => CopySelectedElement(),
            ReportCommand.Paste => PasteElement(),
            ReportCommand.Delete => DeleteSelection(),
            ReportCommand.Undo => Undo(),
            ReportCommand.Redo => Redo(),
            ReportCommand.Reset => ResetDefinition(),
            _ => SetPreview( ReportPreviewFormat.Html ),
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
            ReportCommand.ConnectDataSource => CurrentMode == ReportMode.Design && IsDesignerEnabled && DataSourceProviders.Count > 0,
            ReportCommand.DownloadPdf => context.ViewerOptions.AllowDownload && SupportsPreviewFormat( ReportPreviewFormat.Pdf ) && PdfGenerator is not null,
            ReportCommand.Cut or ReportCommand.Copy => CurrentMode == ReportMode.Design && GetSelectedElementContexts( definition ).Count > 0,
            ReportCommand.Delete => CurrentMode == ReportMode.Design && selectionManager.CanDeleteSelection( definition ),
            ReportCommand.Paste => CurrentMode == ReportMode.Design && HasClipboardElements && definition.Bands.Count > 0,
            ReportCommand.Undo => commandManager.CanUndo,
            ReportCommand.Redo => commandManager.CanRedo,
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
            ReportCommand.Design => CurrentMode == ReportMode.Design,
            ReportCommand.Preview => CurrentMode == ReportMode.Preview,
            ReportCommand.PreviewHtml => CurrentMode == ReportMode.Preview && CurrentPreviewFormat == ReportPreviewFormat.Html,
            ReportCommand.PreviewPdf => CurrentMode == ReportMode.Preview && CurrentPreviewFormat == ReportPreviewFormat.Pdf,
            _ => false,
        };
    }

    /// <summary>
    /// Captures the current report definition, mode, selection, clipboard, and history availability.
    /// </summary>
    /// <returns>Serializable report designer state.</returns>
    public Task<ReportState> GetState()
    {
        return Task.FromResult( CaptureReportState( RootDefinition ) );
    }

    /// <summary>
    /// Restores a previously captured report designer state.
    /// </summary>
    /// <param name="state">State to apply to the report designer.</param>
    public async Task LoadState( ReportState state )
    {
        commandManager.Clear();
        await ApplyReportState( state, notifyDefinitionChanged: true );
        InvalidateDesignerCaches();
    }

    private async Task ExecuteDesignerCommand( ReportDesignerCommand command )
    {
        var result = await commandManager.Execute( command, RootDefinition, CaptureReportState );

        if ( result.NotifyDefinitionChanged && result.RefreshSurface && DefinitionChanged.HasDelegate )
            await DefinitionChanged.InvokeAsync( result.Definition );

        if ( result.NotifyDefinitionChanged )
        {
            InvalidateDesignerCaches();
            RefreshDesignerSurface();
        }

        await InvokeAsync( StateHasChanged );

        if ( result.NotifyDefinitionChanged && !result.RefreshSurface && DefinitionChanged.HasDelegate )
        {
            _ = NotifyDefinitionChangedLater( result.Definition );
        }
    }

    private Task NotifyDefinitionChangedLater( ReportDefinition definition )
    {
        return InvokeAsync( async () =>
        {
            await Task.Yield();
            await DefinitionChanged.InvokeAsync( definition );
        } );
    }

    private async Task SetMode( ReportMode mode )
    {
        if ( CurrentMode == ReportMode.Design && mode != ReportMode.Design )
            await CaptureDesignerPaneScrollPositions();

        await ExecuteDesignerCommand( new( $"Set {mode} mode", async () =>
        {
            currentMode = mode;
            designerState.EditingElementKey = null;

            if ( mode == ReportMode.Design )
                designerPaneScrollRestoreVersion++;

            if ( ModeChanged.HasDelegate )
                await ModeChanged.InvokeAsync( currentMode );
        }, trackHistory: false, notifyDefinitionChanged: false ) );
    }

    private async Task SetPreview( ReportPreviewFormat format )
    {
        if ( CurrentMode == ReportMode.Design )
            await CaptureDesignerPaneScrollPositions();

        await ExecuteDesignerCommand( new( $"Set {format} preview", async () =>
        {
            currentPreviewFormat = format;
            currentMode = ReportMode.Preview;
            designerState.EditingElementKey = null;

            await ResolveDataSources( RootDefinition, loadData: true );

            if ( ModeChanged.HasDelegate )
                await ModeChanged.InvokeAsync( currentMode );
        }, trackHistory: false, notifyDefinitionChanged: false ) );
    }

    private async Task CaptureDesignerPaneScrollPositions()
    {
        if ( designerLayoutRef is not null )
            await designerLayoutRef.CapturePaneScrollPositions( designerPaneScrollPositions );
    }

    private void ResetDesignerSurfaceScrollPosition()
    {
        designerPaneScrollPositions[DesignerSurfacePaneName] = (0, 0);
        designerPaneScrollRestoreVersion++;
    }

    private async Task DownloadPdf()
    {
        if ( PdfGenerator is null )
            return;

        ReportDefinition definition = RootDefinition;
        await ResolveDataSources( definition, true );

        PdfDocumentDefinition pdfDocument = previewExportService.BuildPdfDocument( definition, Data );
        PdfRenderResult result = await PdfGenerator.Generate( pdfDocument, new()
        {
            FileName = previewExportService.ResolvePdfFileName( definition ),
        } );

        EnsureReportingModule();
        await reportingModule.DownloadFile( result.FileName, result.ContentType, result.Content );
    }

    private async Task ResetDefinition()
    {
        await ExecuteDesignerCommand( new( "Reset report", () =>
        {
            declarativeDefinition = BuildDeclarativeDefinition();
            activeSubreportElementKey = null;
            SelectReport();
            _ = CloseContextMenu();
            designerState.DragPreview = null;
            designerState.EditingElementKey = null;

            return Task.CompletedTask;
        }, () => declarativeDefinition ) );
    }

    private Task CopySelectedElement()
    {
        List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( EffectiveDefinition );

        if ( selectedElements.Count == 0 )
            return Task.CompletedTask;

        string commandName = selectedElements.Count == 1 ? "Copy element" : "Copy elements";

        return ExecuteDesignerCommand( new( commandName, () =>
        {
            ReportClipboardResult result = clipboardService.CopyElements( EffectiveDefinition, GetSelectedElementContexts( EffectiveDefinition ) );

            if ( result.ClipboardElements.Count > 0 )
            {
                clipboardElements = result.ClipboardElements;
                clipboardBandId = result.ClipboardBandId;
                _ = CloseContextMenu();
            }

            return Task.CompletedTask;
        }, trackHistory: false, notifyDefinitionChanged: false ) );
    }

    private async Task CutSelectedElement()
    {
        List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( EffectiveDefinition );

        if ( selectedElements.Count == 0 )
            return;

        string commandName = selectedElements.Count == 1 ? "Cut element" : "Cut elements";

        await ExecuteDesignerCommand( new( commandName, () =>
        {
            ReportClipboardResult result = clipboardService.CutElements( EffectiveDefinition, GetSelectedElementContexts( EffectiveDefinition ) );

            if ( result.Changed )
            {
                clipboardElements = result.ClipboardElements;
                clipboardBandId = result.ClipboardBandId;
                SelectSection( result.SelectedSectionIndex.GetValueOrDefault() );
                _ = CloseContextMenu();
            }

            return Task.CompletedTask;
        } ) );
    }

    private async Task PasteElement()
    {
        if ( !HasClipboardElements || ResolvePasteSectionIndex( EffectiveDefinition ) < 0 )
            return;

        await ExecuteDesignerCommand( new( "Paste element", () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            int targetSectionIndex = ResolvePasteSectionIndex( definition );
            ReportClipboardResult result = clipboardService.PasteElements(
                definition,
                clipboardElements,
                clipboardBandId,
                designerState.ContextMenu,
                targetSectionIndex,
                IsSnapToGridEnabled,
                ApplyDesignerGrid,
                elementLayoutService,
                tableEditor );

            if ( !string.IsNullOrWhiteSpace( result.SelectedCellKey ) )
                SelectTableCell( result.SelectedCellKey );
            else if ( result.SelectedElementKeys.Count > 0 )
                SelectElements( result.SelectedElementKeys, result.PrimaryElementKey );

            _ = CloseContextMenu();

            return Task.CompletedTask;
        } ) );
    }

    private int ResolvePasteSectionIndex( ReportDefinition definition )
    {
        return clipboardService.ResolvePasteSectionIndex( definition, designerState.ContextMenu, selectionManager.ResolvePasteSectionIndex );
    }

    private async Task Undo()
    {
        var state = commandManager.Undo();

        if ( state is not null )
            await ApplyReportState( state, notifyDefinitionChanged: true );
    }

    private async Task Redo()
    {
        var state = commandManager.Redo();

        if ( state is not null )
            await ApplyReportState( state, notifyDefinitionChanged: true );
    }

    private ReportState CaptureReportState( ReportDefinition definition )
    {
        return stateService.Capture(
            definition,
            CurrentMode,
            CurrentPreviewFormat,
            designerState.SnapToGrid,
            selectionManager,
            clipboardElements,
            clipboardBandId,
            commandManager.CanUndo,
            commandManager.CanRedo );
    }

    private async Task ApplyReportState( ReportState state, bool notifyDefinitionChanged )
    {
        string previousActiveSubreportElementKey = activeSubreportElementKey;
        ReportState nextState = stateService.Apply( state, designerState, selectionManager, BuildDeclarativeDefinition, out ReportDefinition definition, out List<ReportElementDefinition> nextClipboardElements, out string nextClipboardBandId );

        declarativeDefinition = definition;
        currentMode = nextState.Mode;
        currentPreviewFormat = nextState.PreviewFormat;
        activeSubreportElementKey = ResolveActiveSubreportElementKey( definition, previousActiveSubreportElementKey );
        clipboardElements = nextClipboardElements;
        clipboardBandId = nextClipboardBandId;
        designerState.SelectionVersion++;

        if ( !string.Equals( previousActiveSubreportElementKey, activeSubreportElementKey, StringComparison.Ordinal ) )
            ResetDesignerSurfaceScrollPosition();

        _ = CloseContextMenu();
        designerState.DragPreview = null;
        designerState.EditingElementKey = null;
        ClearDragState();

        commandManager.SetState( CaptureReportState( definition ) );

        InvalidateDesignerCaches();

        if ( notifyDefinitionChanged && DefinitionChanged.HasDelegate )
            await DefinitionChanged.InvokeAsync( definition );

        RefreshDesignerSurface();

        await InvokeAsync( StateHasChanged );
    }

    private void SelectReport()
    {
        bool selectionChanged = selectionManager.SelectReport();
        _ = CloseContextMenu();
        designerState.EditingElementKey = null;

        if ( selectionChanged )
        {
            designerState.SelectionVersion++;
            RefreshDesignerSurface();
        }
    }

    private Task HandleElementClick( string key, MouseEventArgs eventArgs )
    {
        if ( IsSuppressingSelectionClick() )
            return Task.CompletedTask;

        if ( eventArgs.Detail >= 2 )
        {
            if ( TryOpenSubreportDesigner( key ) )
                return Task.CompletedTask;

            BeginElementTextEdit( key );
            return Task.CompletedTask;
        }

        if ( string.Equals( designerState.SuppressNextElementClickKey, key, StringComparison.Ordinal ) )
        {
            designerState.SuppressNextElementClickKey = null;
            return Task.CompletedTask;
        }

        if ( eventArgs.CtrlKey )
        {
            ToggleElementSelection( key );
            return Task.CompletedTask;
        }

        SelectElement( key, preserveSelection: selectionManager.IsElementSelected( key ) && selectionManager.SelectedElementKeys.Count > 1 );

        return Task.CompletedTask;
    }

    private Task HandleElementDoubleClick( string key, MouseEventArgs eventArgs )
    {
        if ( TryOpenSubreportDesigner( key ) )
            return Task.CompletedTask;

        BeginElementTextEdit( key );

        return Task.CompletedTask;
    }

    private void HandleSectionClick( int sectionIndex )
    {
        if ( IsSuppressingSelectionClick() )
            return;

        if ( designerState.SuppressNextSectionClick )
        {
            designerState.SuppressNextSectionClick = false;
            return;
        }

        SelectSection( sectionIndex );
    }

    private Task HandleSectionClick( int sectionIndex, MouseEventArgs eventArgs )
    {
        HandleSectionClick( sectionIndex );

        return Task.CompletedTask;
    }

    private bool IsSuppressingSelectionClick()
    {
        if ( DateTime.UtcNow > designerState.SuppressSelectionClickUntil )
            return false;

        designerState.SuppressNextSectionClick = false;
        designerState.SuppressNextElementClickKey = null;

        return true;
    }

    private void SuppressNextSelectionClick()
    {
        designerState.SuppressNextSectionClick = true;
        designerState.SuppressSelectionClickUntil = DateTime.UtcNow.AddMilliseconds( DesignerConstants.SuppressSelectionClickMilliseconds );
    }

    private void SelectElement( string key, bool preserveSelection = false )
    {
        bool selectionChanged = selectionManager.SelectElement( key, preserveSelection );
        _ = CloseContextMenu();

        if ( selectionChanged )
        {
            designerState.SelectionVersion++;
            RefreshDesignerSurface();
        }
    }

    private void ToggleElementSelection( string key )
    {
        bool selectionChanged = selectionManager.ToggleElementSelection( key );
        _ = CloseContextMenu();

        if ( selectionChanged )
        {
            designerState.SelectionVersion++;
            RefreshDesignerSurface();
        }
    }

    private void SelectElements( IEnumerable<string> elementKeys, string primaryElementKey = null )
    {
        bool selectionChanged = selectionManager.SelectElements( elementKeys, primaryElementKey );
        _ = CloseContextMenu();

        if ( selectionChanged )
        {
            designerState.SelectionVersion++;
            RefreshDesignerSurface();
        }
    }

    private void SelectSection( int index )
    {
        bool selectionChanged = selectionManager.SelectSection( index );
        _ = CloseContextMenu();

        if ( selectionChanged )
        {
            designerState.SelectionVersion++;
            RefreshDesignerSurface();
        }
    }

    private Task OnReportSelected()
    {
        SelectReport();

        return Task.CompletedTask;
    }

    private Task SelectSection( int index, MouseEventArgs eventArgs )
    {
        SelectSection( index );

        return Task.CompletedTask;
    }

    private Task HandleTableCellClick( string cellKey, MouseEventArgs eventArgs )
    {
        if ( IsSuppressingSelectionClick() )
            return Task.CompletedTask;

        SelectTableCell( cellKey );

        return Task.CompletedTask;
    }

    private void SelectTableCell( string cellKey )
    {
        bool selectionChanged = selectionManager.SelectCell( cellKey );
        _ = CloseContextMenu();

        if ( selectionChanged )
        {
            designerState.SelectionVersion++;
            RefreshDesignerSurface();
        }
    }

    private void ToggleSectionCollapsed( ReportBandDefinition section )
    {
        if ( !AllowBandCollapse || section is null )
            return;

        var sectionId = ReportDefinitionHelper.EnsureBandId( section );

        if ( collapsedBandIds.Contains( sectionId ) )
            collapsedBandIds.Remove( sectionId );
        else
            collapsedBandIds.Add( sectionId );

        collapsedSectionsVersion++;
        InvalidateDesignerLayoutCache();
        RefreshDesignerSurface();
    }

    private Task ToggleSectionCollapsed( int sectionIndex, MouseEventArgs eventArgs )
    {
        ReportDefinition definition = EffectiveDefinition;

        if ( sectionIndex >= 0 && sectionIndex < definition.Bands.Count )
            ToggleSectionCollapsed( definition.Bands[sectionIndex] );

        return Task.CompletedTask;
    }

    private bool IsSectionCollapsed( ReportBandDefinition section )
    {
        return AllowBandCollapse
            && section is not null
            && collapsedBandIds.Contains( ReportDefinitionHelper.EnsureBandId( section ) );
    }

    private async Task OpenSectionContextMenu( int sectionIndex, MouseEventArgs eventArgs )
    {
        bool selectionChanged = selectionManager.SelectSection( sectionIndex );

        ReportContextMenuState nextContextMenu = new()
        {
            Visible = true,
            Target = ReportContextMenuTarget.Section,
            SectionIndex = sectionIndex,
            ClientX = eventArgs.ClientX,
            ClientY = eventArgs.ClientY,
        };

        contextMenuService.PopulateSectionCapabilities( EffectiveDefinition, nextContextMenu, HasClipboardElements, aggregateService.CanInsertSection, aggregateService.CanInsertGroup );
        nextContextMenu.CanInsertSubreport = CanInsertSubreportElement && nextContextMenu.CanInsertSubreport;
        await ShowContextMenu( nextContextMenu, selectionChanged );
    }

    private async Task OpenSectionBodyContextMenu( int sectionIndex, MouseEventArgs eventArgs )
    {
        bool selectionChanged = selectionManager.SelectSection( sectionIndex );

        ReportContextMenuState nextContextMenu = new()
        {
            Visible = true,
            Target = ReportContextMenuTarget.Section,
            SectionIndex = sectionIndex,
            HasPastePosition = true,
            PasteX = ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetX ),
            PasteY = ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetY ),
            ClientX = eventArgs.ClientX,
            ClientY = eventArgs.ClientY,
        };

        contextMenuService.PopulateSectionCapabilities( EffectiveDefinition, nextContextMenu, HasClipboardElements, aggregateService.CanInsertSection, aggregateService.CanInsertGroup );
        nextContextMenu.CanInsertSubreport = CanInsertSubreportElement && nextContextMenu.CanInsertSubreport;
        await ShowContextMenu( nextContextMenu, selectionChanged );
    }

    private async Task OpenElementContextMenu( string elementKey, MouseEventArgs eventArgs )
    {
        bool selectionChanged = selectionManager.SelectElement( elementKey, preserveSelection: selectionManager.IsElementSelected( elementKey ) );
        ReportContextMenuState nextContextMenu = new()
        {
            Visible = true,
            Target = ReportContextMenuTarget.Element,
            ElementKey = elementKey,
            SelectedElementCount = selectionManager.SelectedElementKeys.Count,
            ClientX = eventArgs.ClientX,
            ClientY = eventArgs.ClientY,
        };

        contextMenuService.PopulateElementCapabilities( EffectiveDefinition, nextContextMenu, HasClipboardElements );
        await ShowContextMenu( nextContextMenu, selectionChanged );
    }

    private async Task OpenTableCellContextMenu( int sectionIndex, string cellKey, MouseEventArgs eventArgs )
    {
        bool selectionChanged = !string.Equals( selectionManager.SelectedCellKey, cellKey, StringComparison.Ordinal );

        selectionManager.SelectCell( cellKey );

        ReportContextMenuState nextContextMenu = new()
        {
            Visible = true,
            Target = ReportContextMenuTarget.Cell,
            SectionIndex = sectionIndex,
            CellKey = cellKey,
            HasPastePosition = true,
            PasteX = ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetX ),
            PasteY = ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetY ),
            ClientX = eventArgs.ClientX,
            ClientY = eventArgs.ClientY,
        };

        contextMenuService.PopulateTableCellCapabilities( EffectiveDefinition, nextContextMenu, HasClipboardElements );
        await ShowContextMenu( nextContextMenu, selectionChanged );
    }

    private async Task ShowContextMenu( ReportContextMenuState state, bool refreshDesignerSelection )
    {
        designerState.ContextMenu = state;

        if ( contextMenuHost is not null )
            await contextMenuHost.Show( state );

        if ( refreshDesignerSelection )
            await InvokeAsync( StateHasChanged );
    }

    private IReadOnlyList<ReportDesignerFieldOption> GetContextElementAggregateFieldOptions( ReportDefinition definition )
    {
        if ( !IsElementContextMenuVisible()
            || !ReportDefinitionHelper.TryFindElementLocation( definition, designerState.ContextMenu.ElementKey, out var sectionIndex, out _, out var element )
            || sectionIndex < 0
            || sectionIndex >= definition.Bands.Count )
        {
            return [];
        }

        var section = definition.Bands[sectionIndex];

        if ( section.Type != ReportBandType.Detail || element is not ReportFieldElementDefinition fieldElement )
            return [];

        var dataSourceName = section.DataSource ?? fieldElement.DataSource;
        var dataSourceValue = ReportDataResolver.ResolveDataSourceValue( definition, Data, dataSourceName );
        var fields = ReportDataSourceExplorer.ResolveDataSourceFields( dataSourceValue ).ToList();
        var fieldOptions = aggregateService.FlattenFieldOptions( sectionIndex, dataSourceName, fields ).ToList();

        if ( fieldOptions.Count == 0 && !string.IsNullOrWhiteSpace( fieldElement.Field ) )
        {
            fieldOptions.Add( new()
            {
                SourceSectionIndex = sectionIndex,
                DataSourceName = dataSourceName,
                FieldName = fieldElement.Field,
                DisplayName = fieldElement.Field,
            } );
        }

        return fieldOptions
            .Where( option => ReportAggregateResolver.GetSupportedFunctions( definition, Data, option.DataSourceName, option.FieldName, option.DataType ).Count > 0 )
            .ToList();
    }

    private bool TryGetContextElementFormulaFieldName( ReportDefinition definition, out string formulaFieldName )
    {
        formulaFieldName = null;

        if ( !IsElementContextMenuVisible() )
            return false;

        return contextMenuService.TryGetElementFormulaFieldName( definition, designerState.ContextMenu.ElementKey, out formulaFieldName );
    }

    private bool TryGetContextElementRunningTotalName( ReportDefinition definition, out string runningTotalName )
    {
        runningTotalName = null;

        if ( !IsElementContextMenuVisible() )
            return false;

        return contextMenuService.TryGetElementRunningTotalName( definition, designerState.ContextMenu.ElementKey, out runningTotalName );
    }

    private void BeginContextElementTextEdit()
    {
        if ( IsElementContextMenuVisible() )
            BeginElementTextEdit( designerState.ContextMenu.ElementKey );
    }

    private async Task OpenContextElementFormulaDialog()
    {
        if ( !TryGetContextElementFormulaFieldName( EffectiveDefinition, out string formulaFieldName ) )
            return;

        await OpenFormulaFieldDialog( formulaFieldName );
        _ = CloseContextMenu();
    }

    private async Task OpenContextElementRunningTotalDialog()
    {
        if ( !TryGetContextElementRunningTotalName( EffectiveDefinition, out string runningTotalName ) )
            return;

        ReportRunningTotalDefinition runningTotal = dataDefinitionService.FindRunningTotal( EffectiveDefinition, runningTotalName );

        if ( runningTotal is not null )
            await runningTotalDialogRef.Show( runningTotal );

        _ = CloseContextMenu();
    }

    private void BeginSelectedElementTextEdit()
    {
        if ( !string.IsNullOrWhiteSpace( selectionManager.PrimaryElementKey ) )
            BeginElementTextEdit( selectionManager.PrimaryElementKey );
    }

    private void BeginElementTextEdit( string elementKey )
    {
        if ( !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, elementKey, out _, out _, out var element )
            || element.Suppress?.Value == true
            || !contextMenuService.CanEditElementText( element ) )
        {
            return;
        }

        SelectElement( elementKey );
        designerState.EditingElementKey = elementKey;
        _ = CloseContextMenu();
        RefreshDesignerSurface();
    }

    private Task CancelElementTextEdit( string elementKey )
    {
        if ( string.Equals( designerState.EditingElementKey, elementKey, StringComparison.Ordinal ) )
        {
            designerState.EditingElementKey = null;
            RefreshDesignerSurface();
        }

        return Task.CompletedTask;
    }

    private async Task CommitElementTextEdit( string elementKey, string text )
    {
        designerState.EditingElementKey = null;
        RefreshDesignerSurface();

        if ( !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, elementKey, out _, out _, out var currentElement )
            || !contextMenuService.CanEditElementText( currentElement )
            || currentElement is not ReportTextElementDefinition currentTextElement
            || string.Equals( currentTextElement.Text, text, StringComparison.Ordinal ) )
        {
            return;
        }

        await ExecuteDesignerCommand( new( "Edit text", () =>
        {
            if ( ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, elementKey, out _, out _, out var element )
                && element is ReportTextElementDefinition textElement )
            {
                textElement.Text = text;
            }

            return Task.CompletedTask;
        } ) );
    }

    private async Task OpenContextElementAggregateDialog()
    {
        if ( !IsElementContextMenuVisible() || aggregateDialogRef is null )
            return;

        var elementKey = designerState.ContextMenu.ElementKey;
        var fieldOptions = GetContextElementAggregateFieldOptions( EffectiveDefinition );

        if ( fieldOptions.Count == 0 )
            return;

        var sourceSectionIndex = fieldOptions[0].SourceSectionIndex;
        var summaryLocations = aggregateService.GetSummaryLocations( EffectiveDefinition, sourceSectionIndex );

        _ = CloseContextMenu();

        var selectedFieldName = ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, elementKey, out _, out _, out var element )
            && element is ReportFieldElementDefinition fieldElement
            ? fieldElement.Field
            : null;

        await aggregateDialogRef.Show( fieldOptions, selectedFieldName, summaryLocations );
    }

    private Task MergeSelectedTableCellRight()
    {
        return MergeSelectedTableCell( columnSpanDelta: 1, rowSpanDelta: 0 );
    }

    private Task MergeSelectedTableCellDown()
    {
        return MergeSelectedTableCell( columnSpanDelta: 0, rowSpanDelta: 1 );
    }

    private async Task MergeSelectedTableCell( int columnSpanDelta, int rowSpanDelta )
    {
        await ExecuteSelectedTableCellCommand(
            "Merge table cell",
            cellKey => tableCommandService.MergeCell( EffectiveDefinition, cellKey, columnSpanDelta, rowSpanDelta ) );
    }

    private async Task UnmergeSelectedTableCell()
    {
        await ExecuteSelectedTableCellCommand(
            "Unmerge table cell",
            cellKey => tableCommandService.UnmergeCell( EffectiveDefinition, cellKey ) );
    }

    private Task InsertSelectedTableRow( bool insertBelow )
    {
        return ExecuteSelectedTableCellCommand(
            insertBelow ? "Insert table row below" : "Insert table row above",
            cellKey => tableCommandService.InsertRow( EffectiveDefinition, cellKey, insertBelow ) );
    }

    private Task InsertSelectedTableColumn( bool insertRight )
    {
        return ExecuteSelectedTableCellCommand(
            insertRight ? "Insert table column right" : "Insert table column left",
            cellKey => tableCommandService.InsertColumn( EffectiveDefinition, cellKey, insertRight ) );
    }

    private Task InsertSelectedTableCell()
    {
        return ExecuteSelectedTableCellCommand(
            "Insert table cell",
            cellKey => tableCommandService.InsertCell( EffectiveDefinition, cellKey ) );
    }

    private Task DeleteSelectedTableRow()
    {
        return ExecuteSelectedTableCellCommand(
            "Delete table row",
            cellKey => tableCommandService.DeleteRow( EffectiveDefinition, cellKey ) );
    }

    private Task DeleteSelectedTableColumn()
    {
        return ExecuteSelectedTableCellCommand(
            "Delete table column",
            cellKey => tableCommandService.DeleteColumn( EffectiveDefinition, cellKey ) );
    }

    private Task DeleteSelectedTableCell()
    {
        return ExecuteSelectedTableCellCommand(
            "Delete table cell",
            cellKey => tableCommandService.DeleteCell( EffectiveDefinition, cellKey ) );
    }

    private async Task ExecuteSelectedTableCellCommand( string commandName, Func<string, ReportTableCommandResult> execute )
    {
        string cellKey = designerState.ContextMenu?.CellKey ?? selectionManager.SelectedCellKey;

        if ( string.IsNullOrWhiteSpace( cellKey ) )
            return;

        await ExecuteDesignerCommand( new( commandName, () =>
        {
            ReportTableCommandResult result = execute( cellKey );

            if ( result.Changed && !string.IsNullOrWhiteSpace( result.SelectedCellKey ) )
                SelectTableCell( result.SelectedCellKey );

            return Task.CompletedTask;
        } ) );
    }

    private async Task OnAggregateDialogConfirmed( ReportAggregateDialogResult result )
    {
        if ( result is null )
            return;

        await ExecuteDesignerCommand( new( $"Insert {ReportAggregateResolver.GetFunctionDisplayName( result.Function )}", () =>
        {
            var definition = EffectiveDefinition;
            var sourceSectionIndex = result.SourceSectionIndex;

            if ( sourceSectionIndex < 0 || sourceSectionIndex >= definition.Bands.Count )
                return Task.CompletedTask;

            var sourceSection = definition.Bands[sourceSectionIndex];
            var sourceElement = aggregateService.FindDetailFieldElement( sourceSection, result.FieldName ) ?? new ReportFieldElementDefinition
            {
                Name = result.FieldName,
                Field = result.FieldName,
                DataSource = result.DataSourceName,
                X = DesignerConstants.DefaultGroupHeaderElementX,
                Width = DesignerConstants.DefaultDroppedFieldWidth,
                Height = DesignerConstants.DefaultDroppedFieldHeight,
            };

            if ( !ReportAggregateResolver.GetSupportedFunctions( definition, Data, result.DataSourceName, result.FieldName ).Contains( result.Function ) )
                return Task.CompletedTask;

            var targetSectionIndex = result.TargetSectionIndex >= 0 && result.TargetSectionIndex < definition.Bands.Count
                ? result.TargetSectionIndex
                : aggregateService.EnsureTargetSection( definition, sourceSectionIndex );
            var targetSection = definition.Bands[targetSectionIndex];
            var aggregateElement = aggregateService.CreateAggregateElement( sourceSection, sourceElement, result.Function, targetSection, targetSection.Type == ReportBandType.GroupFooter );

            targetSection.Elements.Add( aggregateElement );
            ReportLayoutGeometry.GrowSectionToFitElement( targetSection, aggregateElement );
            SelectElement( ReportDefinitionHelper.EnsureElementId( aggregateElement ) );

            return Task.CompletedTask;
        } ) );
    }

    private bool CanSelectedSectionInsertGroup( ReportDefinition definition )
    {
        return aggregateService.CanInsertGroup( selectionManager.FindSelectedSection( definition ) );
    }

    private bool CanSelectedSectionInsertSection( ReportDefinition definition )
    {
        return selectionManager.SelectedSectionIndex is { } sectionIndex
            && aggregateService.CanInsertSection( definition, sectionIndex );
    }

    private async Task OpenSelectedDetailGroupDialog()
    {
        if ( groupDialogRef is null )
            return;

        var definition = EffectiveDefinition;
        var fieldOptions = aggregateService.GetDetailGroupFieldOptions( definition, Data, selectionManager.SelectedSectionIndex );

        if ( fieldOptions.Count == 0 )
            return;

        var selectedFieldName = selectionManager.SelectedSectionIndex is { } sectionIndex
            && aggregateService.TryFindGroupLocation( definition, sectionIndex, out var groupHeader, out _ )
                ? groupHeader.GroupBy
                : fieldOptions[0].FieldName;

        _ = CloseContextMenu();

        await groupDialogRef.Show( fieldOptions, selectedFieldName );
    }

    private async Task OnGroupDialogConfirmed( string groupBy )
    {
        if ( string.IsNullOrWhiteSpace( groupBy ) || selectionManager.SelectedSectionIndex is null )
            return;

        var detailSectionIndex = selectionManager.SelectedSectionIndex.Value;

        await ExecuteDesignerCommand( new( "Insert group", () =>
        {
            var definition = EffectiveDefinition;

            if ( detailSectionIndex < 0 || detailSectionIndex >= definition.Bands.Count )
                return Task.CompletedTask;

            var detailSection = definition.Bands[detailSectionIndex];

            if ( detailSection.Type != ReportBandType.Detail || ReportValueResolver.ResolveStaticSuppress( detailSection ) )
                return Task.CompletedTask;

            var groupHeader = aggregateService.CreateGroupHeaderSection( definition, groupBy );
            var groupFooter = aggregateService.CreateGroupFooterSection( definition, groupBy );

            definition.Bands.Insert( detailSectionIndex, groupHeader );
            definition.Bands.Insert( detailSectionIndex + 2, groupFooter );

            SelectSection( detailSectionIndex );
            _ = CloseContextMenu();

            return Task.CompletedTask;
        } ) );
    }

    private async Task OpenDataSourceConnectionDialog()
    {
        if ( dataSourceConnectionDialogRef is null )
            return;

        await dataSourceConnectionDialogRef.Show( EffectiveDefinition, DataSourceProviders );
    }

    private async Task OnDataSourceConnectionConfirmed( ReportDataSourceDefinition dataSource )
    {
        if ( dataSource is null || string.IsNullOrWhiteSpace( dataSource.Name ) )
            return;

        await ExecuteDesignerCommand( new( "Connect data source", async () =>
        {
            await dataCommandService.ConnectDataSource( EffectiveDefinition, Data, dataSource, ResolveDataSources );
        } ) );
    }

    private async Task OnDataSourceRefreshed( string dataSourceName )
    {
        if ( string.IsNullOrWhiteSpace( dataSourceName ) )
            return;

        await ExecuteDesignerCommand( new( "Refresh data source", async () =>
        {
            await dataCommandService.RefreshDataSource( EffectiveDefinition, DataSourceProviderRegistry, dataSourceName );
        } ) );
    }

    private async Task OnDataSourceDeleted( string dataSourceName )
    {
        if ( string.IsNullOrWhiteSpace( dataSourceName ) )
            return;

        await ExecuteDesignerCommand( new( "Delete data source", () =>
        {
            dataCommandService.DeleteDataSource( EffectiveDefinition, dataSourceName );
            return Task.CompletedTask;
        } ) );
    }

    private async Task OnFormulaFieldConfirmed( ReportFormulaFieldDefinition formulaField )
    {
        if ( formulaField is null || string.IsNullOrWhiteSpace( formulaField.Name ) )
            return;

        await ExecuteDesignerCommand( new( "Save formula field", () =>
        {
            dataCommandService.SaveFormulaField( EffectiveDefinition, formulaField );
            return Task.CompletedTask;
        } ) );
    }

    private async Task OnFormulaFieldRenamed( (string OldName, string NewName) formulaFieldRename )
    {
        if ( string.IsNullOrWhiteSpace( formulaFieldRename.OldName ) || string.IsNullOrWhiteSpace( formulaFieldRename.NewName ) )
            return;

        string oldName = formulaFieldRename.OldName.Trim();
        string newName = formulaFieldRename.NewName.Trim();

        if ( string.Equals( oldName, newName, StringComparison.OrdinalIgnoreCase ) )
            return;

        await ExecuteDesignerCommand( new( "Rename formula field", () =>
        {
            dataCommandService.RenameFormulaField( EffectiveDefinition, oldName, newName );
            return Task.CompletedTask;
        } ) );
    }

    private async Task OnFormulaFieldDeleted( string formulaFieldName )
    {
        if ( string.IsNullOrWhiteSpace( formulaFieldName ) )
            return;

        await ExecuteDesignerCommand( new( "Delete formula field", () =>
        {
            dataCommandService.DeleteFormulaField( EffectiveDefinition, formulaFieldName );

            if ( string.Equals( editingFormulaFieldName, formulaFieldName, StringComparison.OrdinalIgnoreCase ) )
                editingFormulaFieldName = null;

            return Task.CompletedTask;
        } ) );
    }

    private async Task OnFormulaFieldInserted( string formulaFieldName )
    {
        if ( string.IsNullOrWhiteSpace( formulaFieldName ) )
            return;

        await ExecuteDesignerCommand( new( "Add formula field", () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            int sectionIndex = GetDataElementInsertionSectionIndex( definition );

            if ( sectionIndex >= 0 && sectionIndex < definition.Bands.Count )
            {
                double y = GetNextDataElementInsertionY( definition.Bands[sectionIndex] );
                ReportElementDefinition element = dataCommandService.CreateFormulaFieldElement( definition, sectionIndex, formulaFieldName, y );

                if ( element is not null )
                    SelectElement( ReportDefinitionHelper.EnsureElementId( element ) );
            }

            return Task.CompletedTask;
        } ) );
    }

    private async Task OnFieldInserted( (string DataSourceName, string FieldName) field )
    {
        if ( string.IsNullOrWhiteSpace( field.FieldName ) )
            return;

        await ExecuteDesignerCommand( new( "Add field", () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            int sectionIndex = GetDataElementInsertionSectionIndex( definition );

            if ( sectionIndex >= 0 && sectionIndex < definition.Bands.Count )
            {
                double y = GetNextDataElementInsertionY( definition.Bands[sectionIndex] );
                ReportElementDefinition element = dataCommandService.CreateFieldElement( definition, sectionIndex, field.DataSourceName, field.FieldName, y );

                if ( element is not null )
                    SelectElement( ReportDefinitionHelper.EnsureElementId( element ) );
            }

            return Task.CompletedTask;
        } ) );
    }

    private async Task OnRunningTotalConfirmed( ReportRunningTotalDefinition runningTotal )
    {
        if ( runningTotal is null || string.IsNullOrWhiteSpace( runningTotal.Name ) )
            return;

        await ExecuteDesignerCommand( new( "Save running total", () =>
        {
            dataCommandService.SaveRunningTotal( EffectiveDefinition, runningTotal );
            return Task.CompletedTask;
        } ) );
    }

    private async Task OnRunningTotalRenamed( (string OldName, string NewName) runningTotalRename )
    {
        if ( string.IsNullOrWhiteSpace( runningTotalRename.OldName ) || string.IsNullOrWhiteSpace( runningTotalRename.NewName ) )
            return;

        string oldName = runningTotalRename.OldName.Trim();
        string newName = runningTotalRename.NewName.Trim();

        if ( string.Equals( oldName, newName, StringComparison.OrdinalIgnoreCase ) )
            return;

        await ExecuteDesignerCommand( new( "Rename running total", () =>
        {
            dataCommandService.RenameRunningTotal( EffectiveDefinition, oldName, newName );
            return Task.CompletedTask;
        } ) );
    }

    private async Task OnRunningTotalDeleted( string runningTotalName )
    {
        if ( string.IsNullOrWhiteSpace( runningTotalName ) )
            return;

        await ExecuteDesignerCommand( new( "Delete running total", () =>
        {
            dataCommandService.DeleteRunningTotal( EffectiveDefinition, runningTotalName );
            return Task.CompletedTask;
        } ) );
    }

    private async Task OnRunningTotalInserted( string runningTotalName )
    {
        if ( string.IsNullOrWhiteSpace( runningTotalName ) )
            return;

        await ExecuteDesignerCommand( new( "Add running total", () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            int sectionIndex = GetDataElementInsertionSectionIndex( definition );

            if ( sectionIndex >= 0 && sectionIndex < definition.Bands.Count )
            {
                double y = GetNextDataElementInsertionY( definition.Bands[sectionIndex] );
                ReportElementDefinition element = dataCommandService.CreateRunningTotalElement( definition, sectionIndex, runningTotalName, y );

                if ( element is not null )
                    SelectElement( ReportDefinitionHelper.EnsureElementId( element ) );
            }

            return Task.CompletedTask;
        } ) );
    }

    private async Task OnFormulaDialogConfirmed( string formula )
    {
        if ( string.IsNullOrWhiteSpace( editingFormulaFieldName ) )
            return;

        await OnFormulaFieldConfirmed( new()
        {
            Name = editingFormulaFieldName,
            Formula = formula,
        } );
    }

    private async Task OpenFormulaFieldDialog( string formulaFieldName )
    {
        if ( string.IsNullOrWhiteSpace( formulaFieldName ) )
            return;

        ReportFormulaFieldDefinition formulaField = dataDefinitionService.FindFormulaField( EffectiveDefinition, formulaFieldName );

        if ( formulaField is null )
            return;

        editingFormulaFieldName = formulaField.Name;
        await formulaDialogRef.Show( formulaField.Name, formulaField.Formula );
    }

    private int GetDataElementInsertionSectionIndex( ReportDefinition definition )
    {
        return dataDefinitionService.GetInsertionSectionIndex( definition, selectionManager.SelectedSectionIndex, selectionManager.PrimaryElementKey );
    }

    private double GetNextDataElementInsertionY( ReportBandDefinition section )
    {
        return dataDefinitionService.GetNextInsertionY( section, ApplyDesignerGrid );
    }

    private IReadOnlyList<ReportAggregateFunction> ResolveAggregateDialogSupportedFunctions( ReportDesignerFieldOption field )
    {
        return aggregateService.ResolveSupportedFunctions( EffectiveDefinition, Data, field );
    }

    private bool IsElementTextEditing( string elementKey = null )
    {
        return string.IsNullOrWhiteSpace( elementKey )
            ? !string.IsNullOrWhiteSpace( designerState.EditingElementKey )
            : string.Equals( designerState.EditingElementKey, elementKey, StringComparison.Ordinal );
    }

    private Task CloseContextMenu()
    {
        designerState.ContextMenu = null;

        return contextMenuHost?.CloseMenu() ?? Task.CompletedTask;
    }

    private Task OnDesignerNodeDragEnded( ReportTreeNode node )
    {
        return ClearDesignerDrag();
    }

    private Task OnPageSelectionPointerCancel( PointerEventArgs eventArgs )
    {
        return CancelPageSelectionBox();
    }

    private Task OnElementPointerCancel( PointerEventArgs eventArgs )
    {
        return CancelElementPointerInteraction();
    }

    private Task OnContextMenuCutElement( MouseEventArgs eventArgs )
        => CutSelectedElement();

    private Task OnContextMenuCopyElement( MouseEventArgs eventArgs )
        => CopySelectedElement();

    private Task OnContextMenuPasteElement( MouseEventArgs eventArgs )
        => PasteElement();

    private Task OnContextMenuSelectAllSectionElements( MouseEventArgs eventArgs )
        => SelectAllContextSectionElements();

    private Task OnContextMenuShowProperties( MouseEventArgs eventArgs )
        => ShowContextProperties();

    private Task OnContextMenuInsertSectionBefore( MouseEventArgs eventArgs )
        => InsertSection( insertAfter: false );

    private Task OnContextMenuInsertSectionAfter( MouseEventArgs eventArgs )
        => InsertSection( insertAfter: true );

    private Task OnContextMenuInsertGroup( MouseEventArgs eventArgs )
        => OpenSelectedDetailGroupDialog();

    private Task OnContextMenuInsertSubreport( MouseEventArgs eventArgs )
        => InsertSubreport();

    private Task OnContextMenuToggleSectionSuppression( MouseEventArgs eventArgs )
        => ToggleSelectedSectionSuppression();

    private Task OnContextMenuToggleSectionKeepTogether( MouseEventArgs eventArgs )
        => ToggleSelectedSectionKeepTogether();

    private Task OnContextMenuToggleSectionNewPageBefore( MouseEventArgs eventArgs )
        => ToggleSelectedSectionNewPageBefore();

    private Task OnContextMenuToggleSectionNewPageAfter( MouseEventArgs eventArgs )
        => ToggleSelectedSectionNewPageAfter();

    private Task OnContextMenuDeleteSection( MouseEventArgs eventArgs )
        => DeleteSelectedSection();

    private Task OnContextMenuAlignTops( MouseEventArgs eventArgs )
        => AlignSelectedElements( ReportElementAlignment.Tops );

    private Task OnContextMenuAlignMiddles( MouseEventArgs eventArgs )
        => AlignSelectedElements( ReportElementAlignment.Middles );

    private Task OnContextMenuAlignBottoms( MouseEventArgs eventArgs )
        => AlignSelectedElements( ReportElementAlignment.Bottoms );

    private Task OnContextMenuAlignBaseline( MouseEventArgs eventArgs )
        => AlignSelectedElements( ReportElementAlignment.Baseline );

    private Task OnContextMenuAlignLefts( MouseEventArgs eventArgs )
        => AlignSelectedElements( ReportElementAlignment.Lefts );

    private Task OnContextMenuAlignCenters( MouseEventArgs eventArgs )
        => AlignSelectedElements( ReportElementAlignment.Centers );

    private Task OnContextMenuAlignRights( MouseEventArgs eventArgs )
        => AlignSelectedElements( ReportElementAlignment.Rights );

    private Task OnContextMenuAlignToGrid( MouseEventArgs eventArgs )
        => AlignSelectedElements( ReportElementAlignment.ToGrid );

    private Task OnContextMenuSizeSameWidth( MouseEventArgs eventArgs )
        => SizeSelectedElements( ReportElementSizeMode.SameWidth );

    private Task OnContextMenuSizeSameHeight( MouseEventArgs eventArgs )
        => SizeSelectedElements( ReportElementSizeMode.SameHeight );

    private Task OnContextMenuSizeSameSize( MouseEventArgs eventArgs )
        => SizeSelectedElements( ReportElementSizeMode.SameSize );

    private Task OnContextMenuBringToFront( MouseEventArgs eventArgs )
        => OrderSelectedElements( ReportElementOrderMode.BringToFront );

    private Task OnContextMenuSendToBack( MouseEventArgs eventArgs )
        => OrderSelectedElements( ReportElementOrderMode.SendToBack );

    private Task OnContextMenuMoveForward( MouseEventArgs eventArgs )
        => OrderSelectedElements( ReportElementOrderMode.MoveForward );

    private Task OnContextMenuMoveBackward( MouseEventArgs eventArgs )
        => OrderSelectedElements( ReportElementOrderMode.MoveBackward );

    private Task OnContextMenuInsertAggregate( MouseEventArgs eventArgs )
        => OpenContextElementAggregateDialog();

    private Task OnContextMenuEditText( MouseEventArgs eventArgs )
    {
        BeginContextElementTextEdit();

        return Task.CompletedTask;
    }

    private Task OnContextMenuEditFormula( MouseEventArgs eventArgs )
        => OpenContextElementFormulaDialog();

    private Task OnContextMenuEditRunningTotal( MouseEventArgs eventArgs )
        => OpenContextElementRunningTotalDialog();

    private Task OnContextMenuDeleteElement( MouseEventArgs eventArgs )
        => DeleteSelectedElement();

    private Task OnContextMenuToggleElementCanGrow( MouseEventArgs eventArgs )
        => ToggleSelectedElementCanGrow();

    private Task OnContextMenuToggleElementSuppression( MouseEventArgs eventArgs )
        => ToggleSelectedElementSuppression();

    private Task OnContextMenuMergeCellRight( MouseEventArgs eventArgs )
        => MergeSelectedTableCellRight();

    private Task OnContextMenuMergeCellDown( MouseEventArgs eventArgs )
        => MergeSelectedTableCellDown();

    private Task OnContextMenuUnmergeCell( MouseEventArgs eventArgs )
        => UnmergeSelectedTableCell();

    private Task OnContextMenuInsertTableRowAbove( MouseEventArgs eventArgs )
        => InsertSelectedTableRow( insertBelow: false );

    private Task OnContextMenuInsertTableRowBelow( MouseEventArgs eventArgs )
        => InsertSelectedTableRow( insertBelow: true );

    private Task OnContextMenuInsertTableColumnLeft( MouseEventArgs eventArgs )
        => InsertSelectedTableColumn( insertRight: false );

    private Task OnContextMenuInsertTableColumnRight( MouseEventArgs eventArgs )
        => InsertSelectedTableColumn( insertRight: true );

    private Task OnContextMenuInsertTableCell( MouseEventArgs eventArgs )
        => InsertSelectedTableCell();

    private Task OnContextMenuDeleteTableRow( MouseEventArgs eventArgs )
        => DeleteSelectedTableRow();

    private Task OnContextMenuDeleteTableColumn( MouseEventArgs eventArgs )
        => DeleteSelectedTableColumn();

    private Task OnContextMenuDeleteTableCell( MouseEventArgs eventArgs )
        => DeleteSelectedTableCell();

    private Task OnContextMenuClose( MouseEventArgs eventArgs )
        => CloseContextMenu();

    private async Task MoveSelectedElements( double x, double y )
    {
        ReportDefinition definition = EffectiveDefinition;
        ReportElementDefinition element = selectionManager.FindSelectedElement( definition );

        if ( element is null )
            return;

        bool useSnapToGrid = IsSnapToGridEnabled( element );
        List<ReportElementPointerItemState> selectedElements = CaptureElementPointerItems( definition, ReportDefinitionHelper.EnsureElementId( element ) ).ToList();

        if ( selectedElements.Count == 0 )
            return;

        string commandName = selectedElements.Count == 1 ? "Move element" : "Move elements";

        await ExecuteDesignerCommand( new( commandName, () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            ReportElementCommandResult result = elementCommandService.MoveElements( definition, selectedElements, ReportDefinitionHelper.EnsureElementId( element ), x, y, useSnapToGrid, ApplyDesignerGrid );

            if ( result.SelectedElementKeys.Count > 0 )
                SelectElements( result.SelectedElementKeys, result.PrimaryElementKey );

            return Task.CompletedTask;
        }, refreshSurface: false ) );
    }

    private async Task AlignSelectedElements( ReportElementAlignment alignment )
    {
        List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( EffectiveDefinition );

        if ( selectedElements.Count < DesignerConstants.MinimumBatchElementCount )
            return;

        string commandName = $"Align {elementLayoutService.GetAlignmentDisplayName( alignment )}";

        await ExecuteDesignerCommand( new( commandName, () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( definition );
            ReportElementCommandResult result = elementCommandService.AlignElements( definition, selectedElements, alignment, ApplyDesignerGrid );

            if ( result.SelectedElementKeys.Count > 0 )
                SelectElements( result.SelectedElementKeys, result.PrimaryElementKey );

            return Task.CompletedTask;
        }, refreshSurface: false ) );
    }

    private async Task SizeSelectedElements( ReportElementSizeMode sizeMode )
    {
        List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( EffectiveDefinition );

        if ( selectedElements.Count < DesignerConstants.MinimumBatchElementCount )
            return;

        string commandName = $"Size {elementLayoutService.GetSizeDisplayName( sizeMode )}";

        await ExecuteDesignerCommand( new( commandName, () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( definition );
            ReportElementCommandResult result = elementCommandService.SizeElements( definition, selectedElements, sizeMode );

            if ( result.SelectedElementKeys.Count > 0 )
                SelectElements( result.SelectedElementKeys, result.PrimaryElementKey );

            return Task.CompletedTask;
        }, refreshSurface: false ) );
    }

    private async Task OrderSelectedElements( ReportElementOrderMode orderMode )
    {
        List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( EffectiveDefinition );

        if ( selectedElements.Count == 0 )
            return;

        string commandName = elementLayoutService.GetOrderDisplayName( orderMode );

        await ExecuteDesignerCommand( new( commandName, () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( definition );
            ReportElementCommandResult result = elementCommandService.OrderElements( selectedElements, orderMode );

            if ( result.SelectedElementKeys.Count > 0 )
                SelectElements( result.SelectedElementKeys, result.PrimaryElementKey );

            return Task.CompletedTask;
        }, refreshSurface: false ) );
    }

    private List<ReportSelectedElementContext> GetSelectedElementContexts( ReportDefinition definition )
    {
        return elementLayoutService.GetSelectedElementContexts( definition, selectionManager.SelectedElementKeys, selectionManager.PrimaryElementKey );
    }

    private Task UpdateSelectedElementsFromProperties( Action<ReportElementDefinition> update )
        => UpdateSelectedElements( "Update elements", update );

    private async Task UpdateSelectedElements( string commandName, Action<ReportElementDefinition> update )
    {
        List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( EffectiveDefinition );

        if ( selectedElements.Count == 0 )
            return;

        await ExecuteDesignerCommand( new( commandName, () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( definition );
            ReportElementCommandResult result = elementCommandService.UpdateElements( definition, selectedElements, update );

            if ( result.SelectedElementKeys.Count > 0 )
                SelectElements( result.SelectedElementKeys, result.PrimaryElementKey );

            return Task.CompletedTask;
        } ) );
    }

    private async Task UpdateSelectedSection( Action<ReportBandDefinition> update )
    {
        var section = selectionManager.FindSelectedSection( EffectiveDefinition );

        if ( section is null )
            return;

        await ExecuteDesignerCommand( new( "Update band", () =>
        {
            var section = selectionManager.FindSelectedSection( EffectiveDefinition );

            if ( section is not null )
            {
                update?.Invoke( section );
                section.Height = Math.Max( section.Height, GetMinimumSectionHeight( section ) );
            }

            return Task.CompletedTask;
        } ) );
    }

    private async Task UpdateReportPage( Action<ReportPageDefinition> update )
    {
        await ExecuteDesignerCommand( new( "Update page", () =>
        {
            var definition = EffectiveDefinition;

            update?.Invoke( definition.Page );
            definition.Page = ResolvePage( definition.Page );

            return Task.CompletedTask;
        } ) );
    }

    private async Task InsertSection( bool insertAfter )
    {
        var definition = EffectiveDefinition;

        if ( selectionManager.SelectedSectionIndex is not { } selectedSectionIndex
            || !aggregateService.CanInsertSection( definition, selectedSectionIndex ) )
            return;

        await ExecuteDesignerCommand( new( insertAfter ? "Insert band after" : "Insert band before", () =>
        {
            var definition = EffectiveDefinition;
            var sourceSection = selectionManager.FindSelectedSection( definition );

            if ( selectionManager.SelectedSectionIndex is not { } selectedSectionIndex
                || !aggregateService.CanInsertSection( definition, selectedSectionIndex ) )
                return Task.CompletedTask;

            var insertIndex = insertAfter ? selectedSectionIndex + 1 : selectedSectionIndex;
            ReportBandDefinition section = sectionCommandService.CreateInsertedSection( definition, sourceSection );

            definition.Bands.Insert( insertIndex, section );

            SelectSection( insertIndex );
            _ = CloseContextMenu();

            return Task.CompletedTask;
        } ) );
    }

    private async Task InsertSubreport()
    {
        if ( !CanInsertSubreportElement )
            return;

        ReportDefinition activeDefinition = EffectiveDefinition;
        ReportContextMenuState currentContextMenu = designerState.ContextMenu;
        int currentSectionIndex = currentContextMenu?.SectionIndex ?? selectionManager.SelectedSectionIndex ?? -1;

        if ( currentSectionIndex < 0 || currentSectionIndex >= activeDefinition.Bands.Count )
            return;

        await ExecuteDesignerCommand( new( "Insert subreport", () =>
        {
            ReportDefinition workingDefinition = EffectiveDefinition;
            ReportDefinition rootDefinition = RootDefinition;
            ReportContextMenuState commandContextMenu = designerState.ContextMenu;
            int commandSectionIndex = commandContextMenu?.SectionIndex ?? selectionManager.SelectedSectionIndex ?? -1;

            if ( commandSectionIndex < 0 || commandSectionIndex >= workingDefinition.Bands.Count )
                return Task.CompletedTask;

            string subreportName = ReportDefinitionHelper.CreateUniqueSubreportName( rootDefinition );
            double x = commandContextMenu?.HasPastePosition == true
                ? ApplyDesignerGrid( commandContextMenu.PasteX )
                : ReportDesignerConstants.PasteElementOffset;
            double y = commandContextMenu?.HasPastePosition == true
                ? ApplyDesignerGrid( commandContextMenu.PasteY )
                : ReportDesignerConstants.PasteElementOffset;

            ReportSubreportElementDefinition subreport = (ReportSubreportElementDefinition)ReportDefinitionHelper.CreateElementFromToolbox( ReportElementType.Subreport, subreportName, x, y );
            subreport.Name = subreportName;
            subreport.Report = ReportDefinitionHelper.CreateDefaultSubreportDefinition( subreportName );

            workingDefinition.Bands[commandSectionIndex].Elements.Add( subreport );

            SelectElement( ReportDefinitionHelper.EnsureElementId( subreport ) );
            _ = CloseContextMenu();

            return Task.CompletedTask;
        } ) );
    }

    private Task DeleteSelection()
    {
        if ( selectionManager.SelectedElementKeys.Count > 0 )
            return DeleteSelectedElement();

        return DeleteSelectedSection();
    }

    private async Task DeleteSelectedSection()
    {
        var definition = EffectiveDefinition;

        if ( selectionManager.SelectedSectionIndex is null
            || selectionManager.SelectedSectionIndex < 0
            || selectionManager.SelectedSectionIndex >= definition.Bands.Count
            || !ReportDefinitionHelper.CanDeleteSection( definition.Bands[selectionManager.SelectedSectionIndex.Value] ) )
        {
            return;
        }

        await ExecuteDesignerCommand( new( "Delete band", () =>
        {
            var definition = EffectiveDefinition;

            if ( selectionManager.SelectedSectionIndex is null
                || selectionManager.SelectedSectionIndex < 0
                || selectionManager.SelectedSectionIndex >= definition.Bands.Count )
            {
                return Task.CompletedTask;
            }

            int nextSectionIndex = sectionCommandService.DeleteSection( definition, selectionManager.SelectedSectionIndex.Value, collapsedBandIds );

            if ( definition.Bands.Count == 0 )
            {
                SelectReport();
            }
            else
            {
                SelectSection( nextSectionIndex );
            }

            _ = CloseContextMenu();
            ClearDragState();

            return Task.CompletedTask;
        } ) );
    }

    private async Task ToggleSelectedSectionSuppression()
    {
        var section = selectionManager.FindSelectedSection( EffectiveDefinition );

        if ( section is null )
            return;

        await UpdateSelectedSectionSuppression( !ReportValueResolver.ResolveStaticSuppress( section ) );
    }

    private Task ShowContextProperties()
    {
        selectedDesignerPanelTab = ReportDesignerPanelTab.Properties;
        _ = CloseContextMenu();

        return Task.CompletedTask;
    }

    private Task SelectAllContextSectionElements()
    {
        if ( designerState.ContextMenu?.SectionIndex is not { } sectionIndex )
            return Task.CompletedTask;

        var definition = EffectiveDefinition;

        if ( sectionIndex < 0 || sectionIndex >= definition.Bands.Count )
            return Task.CompletedTask;

        var elementKeys = definition.Bands[sectionIndex].Elements
            .Select( ReportDefinitionHelper.EnsureElementId )
            .Where( key => !string.IsNullOrWhiteSpace( key ) )
            .ToList();

        SelectElements( elementKeys );
        selectedDesignerPanelTab = ReportDesignerPanelTab.Properties;

        return Task.CompletedTask;
    }

    private async Task ToggleSelectedSectionKeepTogether()
    {
        var section = selectionManager.FindSelectedSection( EffectiveDefinition );

        if ( section is null )
            return;

        bool value = section.KeepTogether?.Value != true;
        await UpdateSelectedSection( currentSection => currentSection.KeepTogether = ReportValue.Create( value, currentSection.KeepTogether?.Formula ) );
        _ = CloseContextMenu();
    }

    private async Task ToggleSelectedSectionNewPageBefore()
    {
        var section = selectionManager.FindSelectedSection( EffectiveDefinition );

        if ( section is null )
            return;

        bool value = section.NewPageBefore?.Value != true;
        await UpdateSelectedSection( currentSection => currentSection.NewPageBefore = ReportValue.Create( value, currentSection.NewPageBefore?.Formula ) );
        _ = CloseContextMenu();
    }

    private async Task ToggleSelectedSectionNewPageAfter()
    {
        var section = selectionManager.FindSelectedSection( EffectiveDefinition );

        if ( section is null )
            return;

        bool value = section.NewPageAfter?.Value != true;
        await UpdateSelectedSection( currentSection => currentSection.NewPageAfter = ReportValue.Create( value, currentSection.NewPageAfter?.Formula ) );
        _ = CloseContextMenu();
    }

    private async Task UpdateSelectedSectionSuppression( bool suppressed )
    {
        await ExecuteDesignerCommand( new( suppressed ? "Suppress" : "Don't suppress", () =>
        {
            var section = selectionManager.FindSelectedSection( EffectiveDefinition );

            if ( section is not null )
            {
                sectionCommandService.UpdateSectionSuppression( section, suppressed, collapsedBandIds );
            }

            _ = CloseContextMenu();

            return Task.CompletedTask;
        } ) );
    }

    private async Task ToggleSelectedElementCanGrow()
    {
        var element = selectionManager.FindSelectedElement( EffectiveDefinition );

        if ( element is null )
            return;

        bool value = element.CanGrow?.Value != true;
        await UpdateSelectedElements( value ? "Enable can grow" : "Disable can grow", currentElement => currentElement.CanGrow = ReportValue.Create( value, currentElement.CanGrow?.Formula ) );
        _ = CloseContextMenu();
    }

    private async Task ToggleSelectedElementSuppression()
    {
        var element = selectionManager.FindSelectedElement( EffectiveDefinition );

        if ( element is null )
            return;

        bool value = element.Suppress?.Value != true;
        await UpdateSelectedElements( value ? "Suppress elements" : "Don't suppress elements", currentElement => currentElement.Suppress = ReportValue.Create( value, currentElement.Suppress?.Formula ) );
        _ = CloseContextMenu();
    }

    private async Task DeleteSelectedElement()
    {
        ReportDefinition definition = EffectiveDefinition;

        List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( definition );

        if ( selectedElements.Count == 0 )
            return;

        await ExecuteDesignerCommand( new( selectedElements.Count == 1 ? "Delete element" : "Delete elements", () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( definition );
            ReportElementCommandResult result = elementCommandService.DeleteElements( definition, selectedElements );

            if ( result.SelectedSectionIndex is int selectedSectionIndex )
                SelectSection( selectedSectionIndex );

            _ = CloseContextMenu();
            designerState.EditingElementKey = null;

            return Task.CompletedTask;
        } ) );
    }

    private void BeginFieldDrag( string dataSourceName, string fieldName )
    {
        ReportDesignerInteractionService.BeginFieldDrag( designerState, dataSourceName, fieldName );
    }

    private void BeginToolboxElementDrag( ReportElementType elementType, string text )
    {
        if ( elementType == ReportElementType.Subreport && !CanInsertSubreportElement )
            return;

        ReportDesignerInteractionService.BeginToolboxElementDrag( designerState, elementType, text );
    }

    private bool IsExternalDesignerDragActive()
    {
        return ReportDesignerInteractionService.IsExternalDesignerDragActive( designerState );
    }

    private Task BeginElementPointerDrag( string elementKey, PointerEventArgs eventArgs )
    {
        if ( eventArgs.CtrlKey )
        {
            ToggleElementSelection( elementKey );
            designerState.SuppressNextElementClickKey = elementKey;
            return Task.CompletedTask;
        }

        if ( !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, elementKey, out var sectionIndex, out _, out var element )
            || element.Suppress?.Value == true )
            return Task.CompletedTask;

        ReportDesignerInteractionService.TryBeginElementPointerDrag(
            designerState,
            elementKey,
            element,
            sectionIndex,
            eventArgs,
            IsSnapToGridEnabled( element ),
            CaptureElementPointerItems( EffectiveDefinition, elementKey ).ToList() );

        SelectElement( elementKey, preserveSelection: selectionManager.IsElementSelected( elementKey ) && selectionManager.SelectedElementKeys.Count > 1 );

        return Task.CompletedTask;
    }

    private async Task BeginElementPointerResize( string elementKey, ReportElementResizeHandle handle, PointerEventArgs eventArgs )
    {
        if ( !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, elementKey, out var sectionIndex, out _, out var element )
            || element.Suppress?.Value == true )
            return;

        bool started = ReportDesignerInteractionService.TryBeginElementPointerResize(
            designerState,
            elementKey,
            element,
            sectionIndex,
            handle,
            eventArgs,
            IsSnapToGridEnabled( element ),
            CaptureElementPointerItems( EffectiveDefinition, elementKey ).ToList() );

        SelectElement( elementKey, preserveSelection: selectionManager.IsElementSelected( elementKey ) && selectionManager.SelectedElementKeys.Count > 1 );

        if ( started )
            await StartDocumentElementResize( eventArgs.ClientX, eventArgs.ClientY, eventArgs.PointerId );
    }

    private Task BeginElementPointerResize( string elementKey, int handle, PointerEventArgs eventArgs )
    {
        return BeginElementPointerResize( elementKey, (ReportElementResizeHandle)handle, eventArgs );
    }

    private Task BeginTablePointerResize( string tableKey, string cellKey, ReportTableResizeKind kind, int index, PointerEventArgs eventArgs )
    {
        if ( !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, tableKey, out int sectionIndex, out _, out ReportElementDefinition element )
            || element is not ReportTableElementDefinition table
            || table.Suppress?.Value == true )
        {
            return Task.CompletedTask;
        }

        tableEditor.EnsureGrid( table );

        if ( kind == ReportTableResizeKind.Column && ( index < 0 || index >= table.Columns.Count ) )
            return Task.CompletedTask;

        if ( kind == ReportTableResizeKind.Row && ( index < 0 || index >= table.Rows.Count ) )
            return Task.CompletedTask;

        bool resizesTable = kind == ReportTableResizeKind.Column
            ? index >= table.Columns.Count - 1
            : index >= table.Rows.Count - 1;
        double adjacentOriginalSize = !resizesTable
            ? kind == ReportTableResizeKind.Column
                ? table.Columns[index + 1].Width
                : table.Rows[index + 1].Height
            : 0;

        designerState.DraggedKind = ReportDesignerDragKind.None;
        designerState.DraggedDataSourceName = null;
        designerState.DraggedFieldName = null;
        designerState.DraggedElementType = null;
        designerState.DraggedElementText = null;
        designerState.DraggedElementKey = tableKey;
        designerState.DraggedElement = table;
        designerState.DragPreview = null;
        designerState.LastDragPreviewRenderTime = DateTime.MinValue;
        designerState.ElementPointerDrag = null;
        designerState.ElementPointerResize = null;
        designerState.SectionPointerResize = null;
        designerState.TablePointerResize = new()
        {
            TableKey = tableKey,
            CellKey = cellKey,
            SectionIndex = sectionIndex,
            Kind = kind,
            Index = index,
            OriginalSize = kind == ReportTableResizeKind.Column ? table.Columns[index].Width : table.Rows[index].Height,
            AdjacentOriginalSize = adjacentOriginalSize,
            TargetSize = kind == ReportTableResizeKind.Column ? table.Columns[index].Width : table.Rows[index].Height,
            StartClientX = eventArgs.ClientX,
            StartClientY = eventArgs.ClientY,
            SnapToGrid = IsSnapToGridEnabled( table ),
            ResizesTable = resizesTable,
        };

        if ( !string.IsNullOrWhiteSpace( cellKey ) )
            SelectTableCell( cellKey );
        else
            SelectElement( tableKey );

        return InvokeAsync( StateHasChanged );
    }

    private async Task BeginSectionPointerResize( int sectionIndex, PointerEventArgs eventArgs )
    {
        if ( TryResolveElementResizeFromSectionResize( sectionIndex, eventArgs, out string elementKey, out ReportElementResizeHandle handle ) )
        {
            await BeginElementPointerResize( elementKey, handle, eventArgs );
            return;
        }

        var definition = EffectiveDefinition;

        if ( sectionIndex < 0 || sectionIndex >= definition.Bands.Count )
            return;

        var section = definition.Bands[sectionIndex];

        if ( ReportValueResolver.ResolveStaticSuppress( section ) )
            return;

        designerState.DraggedKind = ReportDesignerDragKind.None;
        designerState.DraggedDataSourceName = null;
        designerState.DraggedFieldName = null;
        designerState.DraggedElementType = null;
        designerState.DraggedElementText = null;
        designerState.DraggedElementKey = null;
        designerState.DraggedElement = null;
        designerState.DragPreview = null;
        designerState.ElementPointerDrag = null;
        designerState.ElementPointerResize = null;
        designerState.TablePointerResize = null;
        designerState.SectionPointerResize = new()
        {
            SectionIndex = sectionIndex,
            OriginalHeight = section.Height,
            TargetHeight = section.Height,
            StartClientY = eventArgs.ClientY,
        };

        SelectSection( sectionIndex );

        await StartDocumentSectionResize( eventArgs.ClientY, eventArgs.PointerId );
        await InvokeAsync( StateHasChanged );
    }

    private bool TryResolveElementResizeFromSectionResize( int sectionIndex, PointerEventArgs eventArgs, out string elementKey, out ReportElementResizeHandle handle )
    {
        elementKey = null;
        handle = default;

        ReportDefinition definition = EffectiveDefinition;

        if ( definition is null || sectionIndex < 0 || sectionIndex >= definition.Bands.Count )
            return false;

        ReportBandDefinition section = definition.Bands[sectionIndex];
        double pointerX = ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetX );
        double handleTolerance = ReportMeasurementConverter.FromCssPixelValue( 8 );

        foreach ( string selectedElementKey in GetSelectedElementKeysForResizeHitTest() )
        {
            if ( !ReportDefinitionHelper.TryFindElementLocation( definition, selectedElementKey, out int selectedSectionIndex, out _, out ReportElementDefinition element )
                || selectedSectionIndex != sectionIndex
                || element.Type == ReportElementType.Line )
            {
                continue;
            }

            double elementBottom = element.Y + ReportLayoutGeometry.GetElementRenderHeight( element );

            if ( Math.Abs( elementBottom - section.Height ) > handleTolerance )
                continue;

            double elementLeft = element.X;
            double elementRight = element.X + element.Width;

            if ( pointerX < elementLeft - handleTolerance || pointerX > elementRight + handleTolerance )
                continue;

            if ( Math.Abs( pointerX - elementLeft ) <= handleTolerance )
            {
                elementKey = selectedElementKey;
                handle = ReportElementResizeHandle.SouthWest;
                return true;
            }

            if ( Math.Abs( pointerX - elementRight ) <= handleTolerance )
            {
                elementKey = selectedElementKey;
                handle = ReportElementResizeHandle.SouthEast;
                return true;
            }

            elementKey = selectedElementKey;
            handle = ReportElementResizeHandle.South;
            return true;
        }

        return false;
    }

    private IEnumerable<string> GetSelectedElementKeysForResizeHitTest()
    {
        foreach ( string selectedElementKey in selectionManager.SelectedElementKeys )
        {
            yield return selectedElementKey;
        }
    }

    private Task PreviewElementPointerInteraction( int targetSectionIndex, PointerEventArgs eventArgs )
    {
        if ( designerState.SelectionBox is not null )
            return PreviewSelectionBox( eventArgs );

        if ( designerState.SectionPointerResize is not null )
            return PreviewSectionPointerResize( eventArgs );

        if ( designerState.TablePointerResize is not null )
            return PreviewTablePointerResize( eventArgs );

        if ( designerState.ElementPointerResize is not null )
            return PreviewElementPointerResize( eventArgs );

        return PreviewElementPointerDrag( targetSectionIndex, eventArgs );
    }

    private Task CompleteElementPointerInteraction( int targetSectionIndex, PointerEventArgs eventArgs )
    {
        if ( designerState.SelectionBox is not null )
            return CompleteSelectionBox( eventArgs );

        if ( designerState.SectionPointerResize is not null )
            return CompleteSectionPointerResize( eventArgs );

        if ( designerState.TablePointerResize is not null )
            return CompleteTablePointerResize( eventArgs );

        if ( designerState.ElementPointerResize is not null )
            return CompleteElementPointerResize( eventArgs );

        return CompleteElementPointerDrag( targetSectionIndex, eventArgs );
    }

    private Task CancelElementPointerInteraction()
    {
        if ( designerState.SelectionBox is not null )
            return CancelSelectionBox();

        if ( designerState.SectionPointerResize is not null )
            return CancelSectionPointerResize();

        if ( designerState.TablePointerResize is not null )
            return CancelTablePointerResize();

        if ( designerState.ElementPointerResize is not null )
            return CancelElementPointerResize();

        return CancelElementPointerDrag();
    }

    private Task BeginSelectionBox( int sectionIndex, PointerEventArgs eventArgs )
    {
        bool selectionBoxStarted = ReportDesignerInteractionService.TryBeginSelectionBox(
            designerState,
            EffectiveDefinition,
            sectionIndex,
            eventArgs,
            GetSectionOffsetY( EffectiveDefinition, sectionIndex ),
            GetDesignerContentHeight( EffectiveDefinition ) );

        if ( selectionBoxStarted )
        {
            _ = CloseContextMenu();
        }

        return Task.CompletedTask;
    }

    private async Task PreviewSelectionBox( PointerEventArgs eventArgs )
    {
        if ( designerState.SelectionBox is null )
            return;

        double previousX = designerState.SelectionBox.X;
        double previousY = designerState.SelectionBox.Y;
        double previousWidth = designerState.SelectionBox.Width;
        double previousHeight = designerState.SelectionBox.Height;

        ReportDesignerInteractionService.UpdateSelectionBox( designerState, EffectiveDefinition, eventArgs, GetDesignerContentHeight( EffectiveDefinition ) );

        if ( !ReportDesignerInteractionService.CanRenderSelectionBoxPreview( designerState, previousX, previousY, previousWidth, previousHeight ) )
            return;

        await UpdateDesignerSelectionOverlay();
    }

    private Task PreviewPageSelectionBox( PointerEventArgs eventArgs )
    {
        return designerState.SelectionBox is null
            ? Task.CompletedTask
            : PreviewSelectionBox( eventArgs );
    }

    private async Task CompleteSelectionBox( PointerEventArgs eventArgs )
    {
        if ( designerState.SelectionBox is null )
            return;

        ReportDesignerInteractionService.UpdateSelectionBox( designerState, EffectiveDefinition, eventArgs, GetDesignerContentHeight( EffectiveDefinition ) );

        ReportDesignerSelectionBox completedSelectionBox = ReportDesignerInteractionService.CompleteSelectionBox( designerState );
        await ClearDesignerInteractionOverlays();

        if ( !completedSelectionBox.HasMoved )
        {
            await InvokeAsync( StateHasChanged );
            return;
        }

        var selectedKeys = FindElementsInsideSelectionBox( EffectiveDefinition, completedSelectionBox ).ToList();

        if ( completedSelectionBox.Additive )
            selectedKeys.InsertRange( 0, selectionManager.SelectedElementKeys );

        if ( selectedKeys.Count > 0 )
        {
            SelectElements( selectedKeys.Distinct( StringComparer.Ordinal ) );
        }
        else
        {
            SelectSection( completedSelectionBox.SectionIndex );
        }

        SuppressNextSelectionClick();

        await InvokeAsync( StateHasChanged );
    }

    private Task CompletePageSelectionBox( PointerEventArgs eventArgs )
    {
        return designerState.SelectionBox is null
            ? Task.CompletedTask
            : CompleteSelectionBox( eventArgs );
    }

    private async Task CancelSelectionBox()
    {
        ReportDesignerInteractionService.CompleteSelectionBox( designerState );
        await ClearDesignerInteractionOverlays();

        await InvokeAsync( StateHasChanged );
    }

    private Task CancelPageSelectionBox()
    {
        return designerState.SelectionBox is null
            ? Task.CompletedTask
            : CancelSelectionBox();
    }

    private async Task PreviewElementPointerDrag( int targetSectionIndex, PointerEventArgs eventArgs )
    {
        if ( designerState.ElementPointerDrag is null || designerState.DraggedKind != ReportDesignerDragKind.Element )
            return;

        var preview = CreateElementPointerDragPreview( targetSectionIndex, eventArgs );

        if ( preview is null )
            return;

        var samePreviewPosition = designerState.DragPreview is not null
            && designerState.DragPreview.SectionIndex == preview.SectionIndex
            && Math.Abs( designerState.DragPreview.X - preview.X ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( designerState.DragPreview.Y - preview.Y ) < DesignerConstants.DragPreviewChangeTolerance;

        if ( samePreviewPosition )
            return;

        var now = DateTime.UtcNow;

        if ( !designerState.ElementPointerDrag.SnapToGrid
            && designerState.DragPreview is not null
            && designerState.DragPreview.SectionIndex == preview.SectionIndex
            && now - designerState.LastDragPreviewRenderTime < DesignerConstants.DragPreviewFrameThrottle )
        {
            return;
        }

        designerState.ElementPointerDrag.TargetSectionIndex = preview.SectionIndex;
        designerState.ElementPointerDrag.TargetX = preview.X;
        designerState.ElementPointerDrag.TargetY = preview.Y;
        designerState.ElementPointerDrag.HasMoved = true;
        designerState.DragPreview = preview;
        designerState.LastDragPreviewRenderTime = now;

        await UpdateDesignerDragOverlay( preview );
    }

    private async Task CompleteElementPointerDrag( int targetSectionIndex, PointerEventArgs eventArgs )
    {
        if ( designerState.ElementPointerDrag is null || designerState.DraggedKind != ReportDesignerDragKind.Element )
            return;

        var pointerDrag = designerState.ElementPointerDrag;
        var preview = CreateElementPointerDragPreview( targetSectionIndex, eventArgs ) ?? designerState.DragPreview;

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
            && pointerDrag.SourceSectionIndex < definition.Bands.Count
            && pointerDrag.TargetSectionIndex >= 0
            && pointerDrag.TargetSectionIndex < definition.Bands.Count
            && ReportDefinitionHelper.TryFindElementLocation( definition, pointerDrag.ElementKey, out _, out _, out _ );

        if ( !moved || !canMove )
        {
            ClearDragState();
            await ClearDesignerInteractionOverlays();
            await InvokeAsync( StateHasChanged );
            return;
        }

        await ClearDesignerInteractionOverlays();

        bool moveToTableCell = TryFindElementPointerDragTableCellTarget( definition, pointerDrag, out _ );

        await ExecuteDesignerCommand( new( moveToTableCell ? "Move element to table cell" : "Move element", () =>
        {
            var definition = EffectiveDefinition;

            if ( !TryMoveElementPointerDragToTableCell( definition, pointerDrag ) )
            {
                ReportDesignerInteractionService.ApplyElementPointerDrag( definition, pointerDrag, sectionIndex => GetSectionOffsetY( definition, sectionIndex ) );
                SelectElements( pointerDrag.SelectedElements.Select( item => item.ElementKey ), pointerDrag.ElementKey );
            }

            SuppressNextSelectionClick();
            designerState.DragPreview = null;
            ClearDragState();

            return Task.CompletedTask;
        }, refreshSurface: false ) );
    }

    private bool TryMoveElementPointerDragToTableCell( ReportDefinition definition, ReportElementPointerDragState pointerDrag )
    {
        if ( !TryFindElementPointerDragTableCellTarget( definition, pointerDrag, out ReportTableCellDropTarget tableCellDropTarget )
             || !ReportDefinitionHelper.TryFindElementLocation( definition, pointerDrag.ElementKey, out ReportElementLocation location ) )
        {
            return false;
        }

        ReportElementDefinition element = location.Element;

        if ( ReferenceEquals( tableCellDropTarget.Table, element ) )
            return false;

        location.OwnerElements.RemoveAt( location.ElementIndex );
        element.X = tableCellDropTarget.X;
        element.Y = tableCellDropTarget.Y;
        tableEditor.ReplaceCellElement( tableCellDropTarget.Table, tableCellDropTarget.Cell, element );
        SelectTableCell( tableCellDropTarget.Cell.Id );

        return true;
    }

    private bool TryFindElementPointerDragTableCellTarget( ReportDefinition definition, ReportElementPointerDragState pointerDrag, out ReportTableCellDropTarget target )
    {
        target = null;

        if ( definition is null
             || pointerDrag is null
             || pointerDrag.SelectedElements.Count != 1
             || pointerDrag.TargetSectionIndex < 0
             || pointerDrag.TargetSectionIndex >= definition.Bands.Count )
        {
            return false;
        }

        double pointerX = pointerDrag.TargetX + pointerDrag.PointerOffsetX;
        double pointerY = pointerDrag.TargetY + pointerDrag.PointerOffsetY;

        if ( !tableEditor.TryFindCellAt( definition.Bands[pointerDrag.TargetSectionIndex], pointerX, pointerY, out target ) )
            return false;

        return !ReportDefinitionHelper.TryFindElementLocation( definition, pointerDrag.ElementKey, out ReportElementLocation location )
            || !ReferenceEquals( target.Table, location.Element );
    }

    private async Task CancelElementPointerDrag()
    {
        if ( designerState.ElementPointerDrag is null )
            return;

        ClearDragState();
        await ClearDesignerInteractionOverlays();

        await InvokeAsync( StateHasChanged );
    }

    private async Task PreviewTablePointerResize( PointerEventArgs eventArgs )
    {
        if ( designerState.TablePointerResize is null )
            return;

        ReportDesignerDragPreview preview = CreateTablePointerResizePreview( eventArgs );

        if ( preview is null )
            return;

        bool samePreviewSize = designerState.DragPreview is not null
            && Math.Abs( designerState.DragPreview.X - preview.X ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( designerState.DragPreview.Y - preview.Y ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( designerState.DragPreview.Width - preview.Width ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( designerState.DragPreview.Height - preview.Height ) < DesignerConstants.DragPreviewChangeTolerance;

        if ( samePreviewSize )
            return;

        DateTime now = DateTime.UtcNow;

        if ( !designerState.TablePointerResize.SnapToGrid
            && designerState.DragPreview is not null
            && now - designerState.LastDragPreviewRenderTime < DesignerConstants.DragPreviewFrameThrottle )
        {
            return;
        }

        designerState.TablePointerResize.TargetSize = ResolveTablePointerResizeTargetSize( eventArgs );
        designerState.TablePointerResize.HasResized = true;
        designerState.DragPreview = preview;
        designerState.LastDragPreviewRenderTime = now;

        await UpdateDesignerDragOverlay( preview );
    }

    private async Task CompleteTablePointerResize( PointerEventArgs eventArgs )
    {
        if ( designerState.TablePointerResize is null )
            return;

        ReportTablePointerResizeState pointerResize = designerState.TablePointerResize;
        pointerResize.TargetSize = ResolveTablePointerResizeTargetSize( eventArgs );

        bool resized = pointerResize.HasResized
            && Math.Abs( pointerResize.TargetSize - pointerResize.OriginalSize ) > .1;

        if ( !resized
            || !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, pointerResize.TableKey, out _, out _, out ReportElementDefinition element )
            || element is not ReportTableElementDefinition table )
        {
            ClearDragState();
            await ClearDesignerInteractionOverlays();
            await InvokeAsync( StateHasChanged );
            return;
        }

        await ClearDesignerInteractionOverlays();

        await ExecuteDesignerCommand( new( pointerResize.Kind == ReportTableResizeKind.Column ? "Resize table column" : "Resize table row", () =>
        {
            if ( ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, pointerResize.TableKey, out int sectionIndex, out _, out ReportElementDefinition element )
                && element is ReportTableElementDefinition table )
            {
                ApplyTablePointerResize( table, pointerResize );
                ReportLayoutGeometry.GrowSectionToFitElement( EffectiveDefinition.Bands[sectionIndex], table );

                if ( !string.IsNullOrWhiteSpace( pointerResize.CellKey ) )
                    SelectTableCell( pointerResize.CellKey );
                else
                    SelectElement( pointerResize.TableKey );
            }

            designerState.DragPreview = null;
            ClearDragState();

            return Task.CompletedTask;
        }, refreshSurface: false ) );
    }

    private async Task CancelTablePointerResize()
    {
        if ( designerState.TablePointerResize is null )
            return;

        ClearDragState();
        await ClearDesignerInteractionOverlays();

        await InvokeAsync( StateHasChanged );
    }

    private ReportDesignerDragPreview CreateTablePointerResizePreview( PointerEventArgs eventArgs )
    {
        if ( designerState.TablePointerResize is null
            || !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, designerState.TablePointerResize.TableKey, out _, out _, out ReportElementDefinition element )
            || element is not ReportTableElementDefinition table )
        {
            return null;
        }

        tableEditor.EnsureGrid( table );

        double targetSize = ResolveTablePointerResizeTargetSize( eventArgs );

        return tableResizeService.CreatePreview( table, designerState.TablePointerResize, targetSize, tableEditor );
    }

    private double ResolveTablePointerResizeTargetSize( PointerEventArgs eventArgs )
    {
        return tableResizeService.ResolveTargetSize( designerState.TablePointerResize, eventArgs.ClientX, eventArgs.ClientY, ApplyDesignerGrid );
    }

    private void ApplyTablePointerResize( ReportTableElementDefinition table, ReportTablePointerResizeState pointerResize )
    {
        tableResizeService.ApplyResize( table, pointerResize, tableEditor );
    }

    private ReportDesignerDragPreview CreateElementPointerDragPreview( int targetSectionIndex, PointerEventArgs eventArgs )
    {
        return ReportDesignerInteractionService.CreateElementDragPreview(
            EffectiveDefinition,
            designerState.ElementPointerDrag,
            designerState.DraggedElement,
            targetSectionIndex,
            eventArgs.ClientX,
            eventArgs.ClientY,
            sectionIndex => GetSectionOffsetY( EffectiveDefinition, sectionIndex ),
            value => ApplyDesignerGrid( value, designerState.ElementPointerDrag?.SnapToGrid ?? designerState.SnapToGrid ) );
    }

    private async Task PreviewElementPointerResize( PointerEventArgs eventArgs )
    {
        await PreviewElementPointerResize( eventArgs.ClientX, eventArgs.ClientY );
    }

    private async Task PreviewElementPointerResize( double clientX, double clientY )
    {
        if ( designerState.ElementPointerResize is null || designerState.DraggedElement is null || designerState.DraggedKind != ReportDesignerDragKind.Element )
            return;

        var preview = CreateElementPointerResizePreview( clientX, clientY );

        if ( preview is null )
            return;

        var samePreviewSize = designerState.DragPreview is not null
            && Math.Abs( designerState.DragPreview.X - preview.X ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( designerState.DragPreview.Y - preview.Y ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( designerState.DragPreview.Width - preview.Width ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( designerState.DragPreview.Height - preview.Height ) < DesignerConstants.DragPreviewChangeTolerance;

        if ( samePreviewSize )
            return;

        var now = DateTime.UtcNow;

        if ( !designerState.ElementPointerResize.SnapToGrid
            && designerState.DragPreview is not null
            && now - designerState.LastDragPreviewRenderTime < DesignerConstants.DragPreviewFrameThrottle )
        {
            return;
        }

        designerState.ElementPointerResize.TargetX = preview.X;
        designerState.ElementPointerResize.TargetY = preview.Y;
        designerState.ElementPointerResize.TargetWidth = preview.Width;
        designerState.ElementPointerResize.TargetHeight = preview.Height;
        designerState.ElementPointerResize.HasResized = true;
        designerState.DragPreview = preview;
        designerState.LastDragPreviewRenderTime = now;

        await UpdateDesignerDragOverlay( preview );
    }

    private async Task CompleteElementPointerResize( PointerEventArgs eventArgs )
    {
        await CompleteElementPointerResize( eventArgs.ClientX, eventArgs.ClientY );
    }

    private async Task CompleteElementPointerResize( double clientX, double clientY )
    {
        if ( designerState.ElementPointerResize is null || designerState.DraggedKind != ReportDesignerDragKind.Element )
            return;

        var pointerResize = designerState.ElementPointerResize;
        var preview = CreateElementPointerResizePreview( clientX, clientY ) ?? designerState.DragPreview;

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

        if ( !resized || !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, pointerResize.ElementKey, out _, out _, out _ ) )
        {
            ClearDragState();
            await ClearDesignerInteractionOverlays();
            await InvokeAsync( StateHasChanged );
            return;
        }

        await ClearDesignerInteractionOverlays();

        await ExecuteDesignerCommand( new( "Resize element", () =>
        {
            ReportDesignerInteractionService.ApplyElementPointerResize( EffectiveDefinition, pointerResize );

            foreach ( ReportElementPointerItemState item in pointerResize.SelectedElements )
            {
                if ( ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, item.ElementKey, out _, out _, out ReportElementDefinition resizedElement ) )
                    if ( resizedElement is ReportTableElementDefinition resizedTable )
                        ReportDefinitionHelper.ScaleTableLayout( resizedTable, item.OriginalWidth, item.OriginalHeight );
            }

            SelectElements( pointerResize.SelectedElements.Select( item => item.ElementKey ), pointerResize.ElementKey );
            SuppressNextSelectionClick();
            designerState.DragPreview = null;
            ClearDragState();

            return Task.CompletedTask;
        }, refreshSurface: false ) );
    }

    private async Task CancelElementPointerResize()
    {
        if ( designerState.ElementPointerResize is null )
            return;

        ClearDragState();
        await ClearDesignerInteractionOverlays();

        await InvokeAsync( StateHasChanged );
    }

    private async Task PreviewSectionPointerResize( PointerEventArgs eventArgs )
    {
        await PreviewSectionPointerResize( eventArgs.ClientY );
    }

    private async Task PreviewSectionPointerResize( double clientY )
    {
        if ( designerState.SectionPointerResize is null )
            return;

        var height = CreateSectionPointerResizeHeight( clientY );

        if ( Math.Abs( designerState.SectionPointerResize.TargetHeight - height ) < DesignerConstants.DragPreviewChangeTolerance )
            return;

        designerState.SectionPointerResize.TargetHeight = height;

        await UpdateDesignerSectionResizePreview( designerState.SectionPointerResize );
    }

    private async Task CompleteSectionPointerResize( PointerEventArgs eventArgs )
    {
        await CompleteSectionPointerResize( eventArgs.ClientY );
    }

    private async Task CompleteSectionPointerResize( double clientY )
    {
        if ( designerState.SectionPointerResize is null )
            return;

        var pointerResize = designerState.SectionPointerResize;
        pointerResize.TargetHeight = CreateSectionPointerResizeHeight( pointerResize, clientY );
        designerState.SectionPointerResize = null;

        try
        {
            var resized = Math.Abs( pointerResize.TargetHeight - pointerResize.OriginalHeight ) > .1;

            var definition = EffectiveDefinition;
            var canResize = pointerResize.SectionIndex >= 0
                && pointerResize.SectionIndex < definition.Bands.Count
                && !ReportValueResolver.ResolveStaticSuppress( definition.Bands[pointerResize.SectionIndex] );

            if ( !resized || !canResize )
                return;

            await ExecuteDesignerCommand( new( "Resize band", () =>
            {
                var definition = EffectiveDefinition;

                if ( pointerResize.SectionIndex >= 0
                    && pointerResize.SectionIndex < definition.Bands.Count
                    && !ReportValueResolver.ResolveStaticSuppress( definition.Bands[pointerResize.SectionIndex] ) )
                {
                    definition.Bands[pointerResize.SectionIndex].Height = pointerResize.TargetHeight;
                    SelectSection( pointerResize.SectionIndex );
                }

                return Task.CompletedTask;
            }, refreshSurface: false ) );
        }
        finally
        {
            await CommitDesignerSectionResizePreview();
            await InvokeAsync( StateHasChanged );
        }
    }

    private async Task CancelSectionPointerResize()
    {
        if ( designerState.SectionPointerResize is null )
            return;

        designerState.SectionPointerResize = null;
        await ClearDesignerSectionResizePreview();

        await InvokeAsync( StateHasChanged );
    }

    private double CreateSectionPointerResizeHeight( double clientY )
    {
        if ( designerState.SectionPointerResize is null )
            return 0;

        return CreateSectionPointerResizeHeight( designerState.SectionPointerResize, clientY );
    }

    private double CreateSectionPointerResizeHeight( ReportSectionPointerResizeState pointerResize, double clientY )
    {
        var section = pointerResize.SectionIndex >= 0 && pointerResize.SectionIndex < EffectiveDefinition.Bands.Count
            ? EffectiveDefinition.Bands[pointerResize.SectionIndex]
            : null;

        return ReportDesignerInteractionService.CreateSectionResizeHeight( pointerResize, clientY, GetMinimumSectionHeight( section ), ApplyDesignerGrid );
    }

    /// <summary>
    /// Previews a document-level band resize while the pointer is moving.
    /// </summary>
    /// <param name="clientY">Current document pointer Y coordinate.</param>
    [JSInvokable]
    public Task OnDocumentSectionResizeMove( double clientY )
    {
        return InvokeAsync( () => PreviewSectionPointerResize( clientY ) );
    }

    /// <summary>
    /// Completes a document-level band resize and commits the final band height.
    /// </summary>
    /// <param name="clientY">Final document pointer Y coordinate.</param>
    [JSInvokable]
    public Task OnDocumentSectionResizeEnd( double clientY )
    {
        return InvokeAsync( () => CompleteSectionPointerResize( clientY ) );
    }

    /// <summary>
    /// Cancels the active document-level band resize.
    /// </summary>
    [JSInvokable]
    public Task OnDocumentSectionResizeCancel()
    {
        return InvokeAsync( CancelSectionPointerResize );
    }

    /// <summary>
    /// Previews a document-level element resize while the pointer is moving.
    /// </summary>
    /// <param name="clientX">Current document pointer X coordinate.</param>
    /// <param name="clientY">Current document pointer Y coordinate.</param>
    [JSInvokable]
    public Task OnDocumentElementResizeMove( double clientX, double clientY )
    {
        return InvokeAsync( () => PreviewElementPointerResize( clientX, clientY ) );
    }

    /// <summary>
    /// Completes a document-level element resize and commits the final element size.
    /// </summary>
    /// <param name="clientX">Final document pointer X coordinate.</param>
    /// <param name="clientY">Final document pointer Y coordinate.</param>
    [JSInvokable]
    public Task OnDocumentElementResizeEnd( double clientX, double clientY )
    {
        return InvokeAsync( () => CompleteElementPointerResize( clientX, clientY ) );
    }

    /// <summary>
    /// Cancels the active document-level element resize.
    /// </summary>
    [JSInvokable]
    public Task OnDocumentElementResizeCancel()
    {
        return InvokeAsync( CancelElementPointerResize );
    }

    private async Task StartDocumentSectionResize( double startClientY, long pointerId )
    {
        EnsureReportingModule();
        dotNetObjectReference ??= DotNetObjectReference.Create( this );

        await DocumentObserver.EnsureInitializedAsync();
        await reportingModule.StartSectionResize( dotNetObjectReference, startClientY, pointerId );
    }

    private async Task StartDocumentElementResize( double startClientX, double startClientY, long pointerId )
    {
        EnsureReportingModule();
        dotNetObjectReference ??= DotNetObjectReference.Create( this );

        await DocumentObserver.EnsureInitializedAsync();
        await reportingModule.StartElementResize( dotNetObjectReference, startClientX, startClientY, pointerId );
    }

    private void EnsureReportingModule()
    {
        reportingModule ??= new( JSRuntime, VersionProvider, BlazoriseOptions );
    }

    private async Task UpdateDesignerSelectionOverlay()
    {
        if ( designerState.SelectionBox is null )
            return;

        EnsureReportingModule();

        await reportingModule.UpdateDesignerSelectionOverlay(
            designerPageRef.Element,
            ReportMeasurementConverter.ToCssPixelValue( designerState.SelectionBox.X ) + GetSelectionBoxLeftOffset(),
            ReportMeasurementConverter.ToCssPixelValue( designerState.SelectionBox.Y ),
            ReportMeasurementConverter.ToCssPixelValue( designerState.SelectionBox.Width ),
            ReportMeasurementConverter.ToCssPixelValue( designerState.SelectionBox.Height ) );
    }

    private async Task UpdateDesignerDragOverlay( ReportDesignerDragPreview preview )
    {
        if ( preview is null )
            return;

        EnsureReportingModule();

        await reportingModule.UpdateDesignerDragOverlay(
            designerPageRef.Element,
            preview.ElementType.ToString(),
            preview.Text,
            ReportMeasurementConverter.ToCssPixelValue( preview.X ) + GetSelectionBoxLeftOffset(),
            ReportMeasurementConverter.ToCssPixelValue( GetElementPageY( EffectiveDefinition, preview.SectionIndex, preview.Y ) ),
            ReportMeasurementConverter.ToCssPixelValue( preview.Width ),
            ReportMeasurementConverter.ToCssPixelValue( preview.Height ) );
    }

    private async Task ClearDesignerInteractionOverlays()
    {
        if ( reportingModule is null )
            return;

        await reportingModule.ClearDesignerInteractionOverlays( designerPageRef.Element );
    }

    private async Task UpdateDesignerSectionResizePreview( ReportSectionPointerResizeState pointerResize )
    {
        if ( pointerResize is null || EffectiveDefinition is null || pointerResize.SectionIndex < 0 || pointerResize.SectionIndex >= EffectiveDefinition.Bands.Count )
            return;

        EnsureReportingModule();

        string sectionId = ReportDefinitionHelper.EnsureBandId( EffectiveDefinition.Bands[pointerResize.SectionIndex] );
        double sectionOffsetY = GetSectionOffsetY( EffectiveDefinition, pointerResize.SectionIndex );
        double sectionHeight = GetDesignerSectionHeight( pointerResize.SectionIndex, EffectiveDefinition.Bands[pointerResize.SectionIndex] );

        await reportingModule.UpdateDesignerSectionResizePreview(
            designerPageRef.Element,
            sectionId,
            ReportMeasurementConverter.ToCssPixelValue( sectionHeight ),
            ReportMeasurementConverter.ToCssPixelValue( sectionOffsetY ) );
    }

    private async Task ClearDesignerSectionResizePreview()
    {
        if ( reportingModule is null )
            return;

        await reportingModule.ClearDesignerSectionResizePreview( designerPageRef.Element );
    }

    private async Task CommitDesignerSectionResizePreview()
    {
        if ( reportingModule is null )
            return;

        await reportingModule.CommitDesignerSectionResizePreview( designerPageRef.Element );
    }

    private ReportDesignerDragPreview CreateElementPointerResizePreview( double clientX, double clientY )
    {
        return ReportDesignerInteractionService.CreateElementResizePreview(
            EffectiveDefinition,
            designerState.ElementPointerResize,
            designerState.DraggedElement,
            clientX,
            clientY,
            value => ApplyDesignerGrid( value, designerState.ElementPointerResize?.SnapToGrid ?? designerState.SnapToGrid ) );
    }

    private async Task PreviewDesignerDrag( int targetSectionIndex, ElementReference sectionBodyElement, DragEventArgs eventArgs )
    {
        if ( designerState.DraggedKind == ReportDesignerDragKind.None )
            return;

        var offset = await GetDesignerDragOffset( sectionBodyElement, eventArgs );
        bool useSnapToGrid = designerState.DraggedKind == ReportDesignerDragKind.Element
            ? IsSnapToGridEnabled( designerState.DraggedElement )
            : designerState.SnapToGrid;
        var preview = CreateDragPreview( targetSectionIndex, ApplyDesignerGrid( offset.X, useSnapToGrid ), ApplyDesignerGrid( offset.Y, useSnapToGrid ) );

        if ( preview is null )
            return;

        var samePreviewPosition = designerState.DragPreview is not null
            && designerState.DragPreview.SectionIndex == preview.SectionIndex
            && Math.Abs( designerState.DragPreview.X - preview.X ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( designerState.DragPreview.Y - preview.Y ) < DesignerConstants.DragPreviewChangeTolerance;

        if ( samePreviewPosition )
        {
            return;
        }

        var now = DateTime.UtcNow;

        if ( !designerState.SnapToGrid
            && designerState.DragPreview is not null
            && designerState.DragPreview.SectionIndex == preview.SectionIndex
            && now - designerState.LastDragPreviewRenderTime < DesignerConstants.DragPreviewFreeDropThrottle )
        {
            return;
        }

        designerState.DragPreview = preview;
        designerState.LastDragPreviewRenderTime = now;

        await UpdateDesignerDragOverlay( preview );
    }

    private async Task<(double X, double Y)> GetDesignerDragOffset( ElementReference sectionBodyElement, DragEventArgs eventArgs )
    {
        EnsureReportingModule();

        var offset = await reportingModule.GetElementOffset( sectionBodyElement, eventArgs.ClientX, eventArgs.ClientY );

        return offset is { Length: >= 2 }
            ? (Math.Max( 0, ReportMeasurementConverter.FromCssPixelValue( offset[0] ) ), Math.Max( 0, ReportMeasurementConverter.FromCssPixelValue( offset[1] ) ))
            : (Math.Max( 0, ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetX ) ), Math.Max( 0, ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetY ) ));
    }

    private async Task DropDesignerItem( int targetSectionIndex, ElementReference sectionBodyElement, DragEventArgs eventArgs )
    {
        var definition = EffectiveDefinition;

        if ( targetSectionIndex < 0 || targetSectionIndex >= definition.Bands.Count )
            return;

        var offset = await GetDesignerDragOffset( sectionBodyElement, eventArgs );
        bool useSnapToGrid = designerState.DraggedKind == ReportDesignerDragKind.Element
            ? IsSnapToGridEnabled( designerState.DraggedElement )
            : designerState.SnapToGrid;
        var x = ApplyDesignerGrid( offset.X, useSnapToGrid );
        var y = ApplyDesignerGrid( offset.Y, useSnapToGrid );
        var tableDropTarget = tableEditor.TryFindCellAt( definition.Bands[targetSectionIndex], x, y, out ReportTableCellDropTarget cellDropTarget )
            ? cellDropTarget
            : null;
        var fieldDropTarget = designerState.DraggedKind == ReportDesignerDragKind.Field
            ? dragDropService.FindTextElementAt( definition.Bands[targetSectionIndex], x, y )
            : null;

        var commandName = dragDropService.ResolveCommandName( definition, designerState, tableDropTarget, fieldDropTarget );

        if ( commandName is null )
            return;

        await ClearDesignerInteractionOverlays();

        await ExecuteDesignerCommand( new( commandName, () =>
        {
            var definition = EffectiveDefinition;
            var targetSection = definition.Bands[targetSectionIndex];
            tableEditor.TryFindCellAt( targetSection, x, y, out ReportTableCellDropTarget tableCellDropTarget );
            ReportDropResult result = dragDropService.Drop( definition, designerState, targetSectionIndex, x, y, tableCellDropTarget, tableEditor );

            if ( !string.IsNullOrWhiteSpace( result.SelectedCellKey ) )
                SelectTableCell( result.SelectedCellKey );
            else if ( result.SelectedElementKeys.Count > 0 )
                SelectElements( result.SelectedElementKeys, result.PrimaryElementKey );

            selectionManager.SelectedSectionIndex = null;
            designerState.DragPreview = null;
            ClearDragState();

            return Task.CompletedTask;
        } ) );
    }

    private ReportDesignerDragPreview CreateDragPreview( int targetSectionIndex, double x, double y )
    {
        return dragDropService.CreateDragPreview( EffectiveDefinition, designerState, targetSectionIndex, x, y );
    }

    private async Task ClearDesignerDrag()
    {
        var requiresRender = designerState.DraggedKind != ReportDesignerDragKind.None || designerState.DragPreview is not null;

        ClearDragState();
        await ClearDesignerInteractionOverlays();

        if ( requiresRender )
            await InvokeAsync( StateHasChanged );
    }

    private double ApplyDesignerGrid( double value )
    {
        return ApplyDesignerGrid( value, designerState.SnapToGrid );
    }

    private double ApplyDesignerGrid( double value, bool useSnapToGrid )
    {
        return useSnapToGrid ? ReportLayoutGeometry.SnapToGrid( value ) : Math.Max( 0, value );
    }

    private bool IsSnapToGridEnabled( ReportElementDefinition element )
    {
        return element?.SnapToGrid?.Value ?? designerState.SnapToGrid;
    }

    private IEnumerable<string> FindElementsInsideSelectionBox( ReportDefinition definition, ReportDesignerSelectionBox selectionBox )
    {
        return ReportDesignerInteractionService.FindElementsInsideSelectionBox(
            definition,
            selectionBox,
            IsSectionCollapsed,
            sectionIndex => GetSectionOffsetY( definition, sectionIndex ) );
    }

    private IEnumerable<ReportElementPointerItemState> CaptureElementPointerItems( ReportDefinition definition, string activeElementKey )
    {
        if ( definition is null || string.IsNullOrWhiteSpace( activeElementKey ) )
            return Enumerable.Empty<ReportElementPointerItemState>();

        List<string> elementKeys = selectionManager.IsElementSelected( activeElementKey ) && selectionManager.SelectedElementKeys.Count > 1
            ? selectionManager.SelectedElementKeys.ToList()
            : [activeElementKey];

        return ReportDesignerInteractionService.CaptureElementPointerItems(
            definition,
            elementKeys,
            sectionIndex => GetSectionOffsetY( definition, sectionIndex ) );
    }

    private double GetSectionOffsetY( ReportDefinition definition, int sectionIndex )
    {
        return GetReportPageMarginTop( definition ) + designerLayoutService.GetSectionOffsetY( definition, sectionIndex, BandMode, collapsedSectionsVersion, designerState.SectionPointerResize, IsSectionCollapsed );
    }

    private double GetDesignerContentHeight( ReportDefinition definition )
    {
        return GetReportPageMarginTop( definition )
            + designerLayoutService.GetContentHeight( definition, BandMode, collapsedSectionsVersion, designerState.SectionPointerResize, IsSectionCollapsed )
            + GetReportPageMarginBottom( definition );
    }

    private double GetDesignerRulerHeight( ReportDefinition definition )
    {
        return Math.Max( definition?.Page?.Height ?? 0, GetDesignerContentHeight( definition ) );
    }

    private ReportDesignerRulerMarker GetDesignerRulerMarker( ReportDefinition definition )
    {
        return rulerService.CreateMarker(
            definition,
            designerState,
            GetSelectedElementContexts( definition ),
            selectionManager.SelectedSectionIndex,
            sectionIndex => GetSectionOffsetY( definition, sectionIndex ),
            sectionIndex => GetDesignerSectionHeight( sectionIndex, definition.Bands[sectionIndex] ),
            GetDesignerSectionBodyTopOffset() );
    }

    private double GetElementPageY( ReportDefinition definition, int sectionIndex, double elementY )
    {
        return GetSectionOffsetY( definition, sectionIndex ) + GetDesignerSectionBodyTopOffset() + elementY;
    }

    private double GetDesignerSectionHeight( int sectionIndex, ReportBandDefinition section )
    {
        return designerLayoutService.GetSectionHeight( sectionIndex, section, BandMode, designerState.SectionPointerResize, IsSectionCollapsed );
    }

    private void InvalidateDesignerCaches()
    {
        InvalidateDesignerLayoutCache();
        renderService.Invalidate();
    }

    private void RefreshDesignerSurface()
    {
        if ( designerLayoutRef is not null )
            _ = designerLayoutRef.RefreshSurface();
    }

    private void InvalidateDesignerLayoutCache()
    {
        designerLayoutService.Invalidate();
    }

    private static double GetMinimumSectionHeight( ReportBandDefinition section )
    {
        return ReportLayoutGeometry.GetMinimumSectionHeight( section );
    }

    private void OnSnapToGridChanged( bool value )
    {
        designerState.SnapToGrid = value;
    }

    private async Task OnShowRulersChanged( bool value )
    {
        if ( ShowRulers == value )
            return;

        ShowRulers = value;

        if ( ShowRulersChanged.HasDelegate )
            await ShowRulersChanged.InvokeAsync( value );

        await InvokeAsync( StateHasChanged );
    }

    private async Task OnShowFineRulerTicksChanged( bool value )
    {
        if ( ShowFineRulerTicks == value )
            return;

        ShowFineRulerTicks = value;

        if ( ShowFineRulerTicksChanged.HasDelegate )
            await ShowFineRulerTicksChanged.InvokeAsync( value );

        await InvokeAsync( StateHasChanged );
    }

    private async Task OnBandModeChanged( ReportBandMode value )
    {
        if ( BandMode == value )
            return;

        BandMode = value;
        InvalidateDesignerLayoutCache();
        RefreshDesignerSurface();

        if ( BandModeChanged.HasDelegate )
            await BandModeChanged.InvokeAsync( value );

        await InvokeAsync( StateHasChanged );
    }

    private void ClearDragState()
    {
        ReportDesignerInteractionService.ClearDragState( designerState );
    }

    private bool SupportsPreviewFormat( ReportPreviewFormat format )
    {
        return previewExportService.SupportsPreviewFormat( PreviewFormats, context.ViewerOptions, format );
    }

    private bool ShouldRenderElement( ReportDefinition definition, ReportBandDefinition section, ReportElementDefinition element, object item )
    {
        return !ReportValueResolver.ResolveSuppress( element, section, definition, Data, item );
    }

    private ReportDefinition ResolveActiveDesignerDefinition( ReportDefinition rootDefinition )
    {
        if ( CurrentMode != ReportMode.Design || !TryGetActiveSubreportElement( rootDefinition, out ReportSubreportElementDefinition subreportElement ) )
        {
            return rootDefinition;
        }

        return ReportDefinitionHelper.EnsureSubreportDefinition( subreportElement ) ?? rootDefinition;
    }

    private object GetFieldsExplorerData( ReportDefinition rootDefinition )
    {
        return TryGetActiveSubreportElement( rootDefinition, out ReportSubreportElementDefinition subreportElement )
            ? ReportSubreportResolver.ResolveData( rootDefinition, Data, null, subreportElement )
            : Data;
    }

    private IReadOnlyList<ReportDesignerDataSourceNode> GetFieldsExplorerDataSources( ReportDefinition rootDefinition )
    {
        if ( !TryGetActiveSubreportElement( rootDefinition, out ReportSubreportElementDefinition subreportElement ) )
            return null;

        List<ReportDesignerFieldNode> fields = ReportDataSourceExplorer.ResolveDataSourceFields( rootDefinition, Data, subreportElement.DataSource ).ToList();

        if ( fields.Count == 0 )
            return [];

        return
        [
            new()
            {
                Name = DataSourceName,
                BindingName = null,
                Fields = fields,
            },
        ];
    }

    private bool TryGetActiveSubreportElement( ReportDefinition rootDefinition, out ReportSubreportElementDefinition subreportElement )
    {
        subreportElement = null;

        if ( string.IsNullOrWhiteSpace( activeSubreportElementKey )
             || !ReportDefinitionHelper.TryFindElementLocation( rootDefinition, activeSubreportElementKey, out _, out _, out ReportElementDefinition element )
             || element is not ReportSubreportElementDefinition activeSubreportElement )
        {
            return false;
        }

        subreportElement = activeSubreportElement;

        return true;
    }

    private IReadOnlyList<ReportDesignerTabItem> GetDesignerTabs( ReportDefinition rootDefinition )
    {
        List<ReportDesignerTabItem> tabs =
        [
            new()
            {
                Key = MainReportDesignerTabKey,
                Text = string.IsNullOrWhiteSpace( rootDefinition?.Name ) ? "Main Report" : rootDefinition.Name,
                Active = string.IsNullOrWhiteSpace( activeSubreportElementKey ),
            },
        ];

        foreach ( ReportSubreportElementDefinition subreport in ReportDefinitionHelper.EnumerateSubreportElements( rootDefinition ) )
        {
            string key = ReportDefinitionHelper.EnsureElementId( subreport );

            tabs.Add( new()
            {
                Key = key,
                Text = ReportSubreportResolver.GetDisplayName( subreport ),
                Active = string.Equals( activeSubreportElementKey, key, StringComparison.Ordinal ),
            } );
        }

        if ( tabs.All( tab => !tab.Active ) )
        {
            tabs[0].Active = true;
        }

        return tabs;
    }

    private Task SelectDesignerTab( string key )
    {
        string nextActiveSubreportElementKey = string.Equals( key, MainReportDesignerTabKey, StringComparison.Ordinal )
            ? null
            : key;

        if ( string.Equals( activeSubreportElementKey, nextActiveSubreportElementKey, StringComparison.Ordinal ) )
            return Task.CompletedTask;

        activeSubreportElementKey = nextActiveSubreportElementKey;
        ResetDesignerSurfaceScrollPosition();
        SelectReport();
        InvalidateDesignerCaches();
        RefreshDesignerSurface();

        return InvokeAsync( StateHasChanged );
    }

    private bool TryOpenSubreportDesigner( string elementKey )
    {
        if ( string.IsNullOrWhiteSpace( elementKey )
             || !ReportDefinitionHelper.TryFindElementLocation( RootDefinition, elementKey, out _, out _, out ReportElementDefinition element )
             || element is not ReportSubreportElementDefinition subreportElement )
        {
            return false;
        }

        bool activeSubreportChanged = !string.Equals( activeSubreportElementKey, elementKey, StringComparison.Ordinal );

        ReportDefinitionHelper.EnsureSubreportDefinition( subreportElement );
        activeSubreportElementKey = elementKey;

        if ( activeSubreportChanged )
            ResetDesignerSurfaceScrollPosition();

        SelectReport();
        InvalidateDesignerCaches();
        RefreshDesignerSurface();
        StateHasChanged();

        return true;
    }

    private static string ResolveActiveSubreportElementKey( ReportDefinition definition, string elementKey )
    {
        return !string.IsNullOrWhiteSpace( elementKey )
            && ReportDefinitionHelper.TryFindElementLocation( definition, elementKey, out _, out _, out ReportElementDefinition element )
            && element is ReportSubreportElementDefinition
                ? elementKey
                : null;
    }

    #endregion

    #region Properties

    private ReportDefinition RootDefinition
        => CurrentDefinitionMode == ReportDefinitionMode.AlwaysUseDeclarative
            ? BuildDeclarativeDefinition()
            : Definition ?? declarativeDefinition ?? BuildDeclarativeDefinition();

    private ReportDefinition EffectiveDefinition
        => ResolveActiveDesignerDefinition( RootDefinition );

    private bool IsDesignerEnabled => DesignerEnabled || GlobalOptions.DesignerEnabled;

    private ReportOptions GlobalOptions => globalOptions ??= ServiceProvider.GetService<ReportOptions>() ?? new();

    private IReportDataSourceProviderRegistry DataSourceProviderRegistry
        => dataSourceProviderRegistry ??= ServiceProvider.GetService<IReportDataSourceProviderRegistry>();

    private IReadOnlyList<IReportDataSourceProvider> DataSourceProviders
        => DataSourceProviderRegistry?.Providers?.Count > 0
            ? DataSourceProviderRegistry.Providers
            : fallbackDataSourceProviders;

    private ReportMode CurrentMode => Mode ?? currentMode;

    private ReportPreviewFormat CurrentPreviewFormat => PreviewFormat ?? currentPreviewFormat;

    private ReportDefinitionMode CurrentDefinitionMode => DefinitionMode ?? GlobalOptions.DefinitionMode;

    private bool HasClipboardElements => clipboardElements.Count > 0;

    private bool CanInsertSubreportElement => string.IsNullOrWhiteSpace( activeSubreportElementKey );

    private string SelectedDesignerPanelTabName => selectedDesignerPanelTab.ToString();

    private string ToolbarStateKey => $"{CurrentMode}|{CurrentPreviewFormat}|{activeSubreportElementKey}|{selectionManager.PrimaryElementKey}|{selectionManager.SelectedCellKey}|{selectionManager.SelectedElementKeys.Count}|{selectionManager.SelectedSectionIndex}|{clipboardElements.Count}|{commandManager.CanUndo}|{commandManager.CanRedo}";

    private string DataSourceName => "Default";

    /// <summary>
    /// JavaScript runtime used to create the local Reporting module.
    /// </summary>
    [Inject] private IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// Version provider used to create the local Reporting module.
    /// </summary>
    [Inject] private IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Blazorise options used to create the local Reporting module.
    /// </summary>
    [Inject] private BlazoriseOptions BlazoriseOptions { get; set; }

    /// <summary>
    /// Shared document observer used by document-level Reporting interactions.
    /// </summary>
    [Inject] private IDocumentObserver DocumentObserver { get; set; }

    /// <summary>
    /// PDF generator used by the report viewer download command.
    /// </summary>
    [Inject] private IPdfGenerator PdfGenerator { get; set; }

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
    [Parameter] public ReportBandMode BandMode { get; set; } = ReportBandMode.Classic;

    /// <summary>
    /// Raised when the designer band presentation changes.
    /// </summary>
    [Parameter] public EventCallback<ReportBandMode> BandModeChanged { get; set; }

    /// <summary>
    /// Enables collapsing and expanding bands in the designer rail.
    /// </summary>
    [Parameter] public bool AllowBandCollapse { get; set; } = true;

    /// <summary>
    /// Shows data source names in band labels when available.
    /// </summary>
    [Parameter] public bool ShowBandDataSource { get; set; } = true;

    /// <summary>
    /// Shows measurement rulers around the report designer page.
    /// </summary>
    [Parameter] public bool ShowRulers { get; set; } = true;

    /// <summary>
    /// Raised when designer ruler visibility changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowRulersChanged { get; set; }

    /// <summary>
    /// Shows fine-grained measurement ruler ticks.
    /// </summary>
    [Parameter] public bool ShowFineRulerTicks { get; set; }

    /// <summary>
    /// Raised when fine-grained ruler tick visibility changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowFineRulerTicksChanged { get; set; }

    /// <summary>
    /// Enables image upload from Image element source properties.
    /// </summary>
    [Parameter] public bool UploadImage { get; set; } = true;

    /// <summary>
    /// A comma-separated list of image MIME types accepted by the image upload dialog.
    /// </summary>
    [Parameter] public string ImageAccept { get; set; } = "image/png, image/jpeg, image/webp, image/svg+xml";

    /// <summary>
    /// Maximum image size in bytes.
    /// </summary>
    [Parameter] public long ImageMaxSize { get; set; } = 1024 * 1024 * 5;

    /// <summary>
    /// Specifies the max chunk size when uploading the image.
    /// </summary>
    [Parameter] public int MaxUploadImageChunkSize { get; set; } = 20 * 1024;

    /// <summary>
    /// Specifies the segment fetch timeout when uploading the image.
    /// </summary>
    [Parameter] public TimeSpan ImageUploadSegmentFetchTimeout { get; set; } = TimeSpan.FromMinutes( 1 );

    /// <summary>
    /// Disables image upload progress callbacks.
    /// </summary>
    [Parameter] public bool DisableImageUploadProgressReport { get; set; }

    /// <summary>
    /// Raised when the selected image changes.
    /// </summary>
    [Parameter] public EventCallback<FileChangedEventArgs> ImageUploadChanged { get; set; }

    /// <summary>
    /// Raised when reading an image starts.
    /// </summary>
    [Parameter] public EventCallback<FileStartedEventArgs> ImageUploadStarted { get; set; }

    /// <summary>
    /// Raised when reading an image ends.
    /// </summary>
    [Parameter] public EventCallback<FileEndedEventArgs> ImageUploadEnded { get; set; }

    /// <summary>
    /// Raised when an image chunk is read.
    /// </summary>
    [Parameter] public EventCallback<FileWrittenEventArgs> ImageUploadWritten { get; set; }

    /// <summary>
    /// Raised when image read progress changes.
    /// </summary>
    [Parameter] public EventCallback<FileProgressedEventArgs> ImageUploadProgressed { get; set; }

    /// <summary>
    /// Raised when the image upload action is confirmed.
    /// </summary>
    [Parameter] public EventCallback<FileUploadEventArgs> ImageUpload { get; set; }

    /// <summary>
    /// Controls how declarative child content is used with persisted definitions. When not set, the value configured in <see cref="ReportOptions.DefinitionMode"/> is used.
    /// </summary>
    [Parameter] public ReportDefinitionMode? DefinitionMode { get; set; }

    /// <summary>
    /// Externally controlled design or preview mode.
    /// </summary>
    [Parameter] public ReportMode? Mode { get; set; }

    /// <summary>
    /// Raised when design or preview mode changes.
    /// </summary>
    [Parameter] public EventCallback<ReportMode> ModeChanged { get; set; }

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

    private sealed class ReportDesignerTabItem
    {
        public string Key { get; set; }

        public string Text { get; set; }

        public bool Active { get; set; }
    }
}