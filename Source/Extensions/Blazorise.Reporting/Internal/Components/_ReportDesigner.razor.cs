#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Licensing;
using Blazorise.Pdf;
using Blazorise.Reporting.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using DesignerConstants = Blazorise.Reporting.Internal.ReportDesignerConstants;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Provides a declarative report designer and viewer for band-based report definitions.
/// </summary>
public partial class _ReportDesigner : ComponentBase, IReportCommandExecutor, IAsyncDisposable
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

    private readonly ReportDataCommandService dataCommandService = new();

    private readonly ReportElementLayoutService elementLayoutService = new();

    private readonly ReportElementCommandService elementCommandService;

    private readonly ReportTableEditor tableEditor = new();

    private readonly ReportTableCommandService tableCommandService;

    private readonly ReportRenderService renderService = new();

    private readonly ReportSectionCommandService sectionCommandService = new();

    private readonly ReportDesignerRulerService rulerService = new();

    private readonly ReportDesignerInteractionState designerState = new();

    private readonly DockLayoutState designerDockLayoutState = new();

    private readonly Dictionary<string, (double Left, double Top)> designerPaneScrollPositions = new( StringComparer.Ordinal );

    private readonly HashSet<string> collapsedBandIds = new( StringComparer.Ordinal );

    private readonly IReadOnlyList<IReportDataSourceProvider> fallbackDataSourceProviders =
    [
        new ObjectReportDataSourceProvider(),
        new DataSetReportDataSourceProvider(),
    ];

    private const string MainReportDesignerTabKey = "__main";

    private const string DesignerSurfacePaneName = "report-designer";

    private ReportDefinition declarativeDefinition;

    private ReportMode currentMode;

    private ReportPreviewFormat currentPreviewFormat;

    private ReportDesignerPanelTab selectedDesignerPanelTab = ReportDesignerPanelTab.Properties;

    private int designerPaneScrollRestoreVersion;

    private _ReportDesignerWorkspace workspaceRef;

    private int renderMutationVersion;

    private ReportDesignerRefreshState designerRefreshState;

    private List<ReportElementDefinition> clipboardElements = [];

    private string clipboardBandId;

    private ReportOptions globalOptions;

    private IReportDataSourceProviderRegistry dataSourceProviderRegistry;

    private JSReportingModule reportingModule;

    private PdfGenerationResult pdfPreviewResult;

    private ReportPdfPreviewContext pdfPreviewContext;

    private int pdfPreviewMutationVersion = -1;

    private string editingFormulaFieldName;

    private string activeSubreportElementKey;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new report component instance.
    /// </summary>
    public _ReportDesigner()
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
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( !firstRender )
            return;

        bool declarativeDefinitionCreated = false;

        if ( Definition is null && CurrentDefinitionMode != ReportDefinitionMode.UseDefinitionOnly )
        {
            declarativeDefinition = BuildDeclarativeDefinition();
            declarativeDefinitionCreated = true;
        }

        ReportDefinition definition = Definition ?? declarativeDefinition;

        if ( definition is not null )
        {
            await ResolveDataSources( definition, CurrentMode == ReportMode.Preview );
            if ( CurrentMode == ReportMode.Preview && CurrentPreviewFormat == ReportPreviewFormat.Pdf )
                await ResolvePdfPreview();
        }
        else
            InvalidateDesignerCaches();

        if ( declarativeDefinitionCreated )
            await DefinitionChanged.InvokeAsync( declarativeDefinition );

        if ( definition is not null )
            RefreshDesigner( ReportDesignerRefreshTarget.Surface | ReportDesignerRefreshTarget.FieldsExplorer );

        if ( commandManager.State?.Definition is null )
            commandManager.SetState( CaptureReportState( RootDefinition ) );

        if ( definition is not null )
            StateHasChanged();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if ( reportingModule is not null )
        {
            try
            {
                await reportingModule.DisposeAsync();
            }
            catch ( JSDisconnectedException )
            {
            }
        }

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
        definition.Designer = new()
        {
            BandMode = BandMode,
            ShowRulers = ShowRulers,
            ShowFineRulerTicks = ShowFineRulerTicks,
            ShowCursorGuides = ShowCursorGuides,
            ShowCollisionWarnings = ShowCollisionWarnings,
        };

        return ReportDefinitionHelper.EnsureDefinitionIds( definition );
    }

    private ReportPageDefinition ResolvePage( ReportPageDefinition page )
    {
        return ReportPageDefinitionHelper.ResolvePage( page );
    }

    private async Task ResolveDataSources( ReportDefinition definition, bool loadData )
    {
        InvalidateDesignerCaches();

        ReportDefinitionHelper.ApplyRowsLimit( definition, BlazoriseLicenseLimitsHelper.GetReportingRowsLimit( LicenseChecker ) );

        if ( definition?.DataSources is null )
            return;

        IReportDataSourceProviderRegistry registry = DataSourceProviderRegistry;

        if ( registry is null )
            return;

        foreach ( ReportDataSourceDefinition dataSource in definition.DataSources )
        {
            if ( dataSource is null )
                continue;

            IReportDataSourceProvider provider = registry.FindProvider( dataSource.ProviderType );

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
                }
                else if ( dataSource.Schema is null )
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

    internal ReportBandDefinition GetSelectedPropertiesSection( ReportDefinition definition )
    {
        return selectionManager.SelectedElementKeys.Count == 0
            ? selectionManager.FindSelectedSection( definition )
            : null;
    }

    internal ReportBandDefinition GetSelectedFormulaSection( ReportDefinition definition )
    {
        if ( selectionManager.SelectedElementKeys.Count == 0 )
            return selectionManager.FindSelectedSection( definition );

        return ReportDefinitionHelper.TryFindElementLocation( definition, selectionManager.PrimaryElementKey, out var sectionIndex, out _, out _ )
            ? definition.Bands[sectionIndex]
            : null;
    }

    internal Task SelectDesignerPanelTab( string tab )
    {
        selectedDesignerPanelTab = string.Equals( tab, nameof( ReportDesignerPanelTab.Explorer ), StringComparison.Ordinal )
            ? ReportDesignerPanelTab.Explorer
            : ReportDesignerPanelTab.Properties;

        return Task.CompletedTask;
    }

    internal Task OnReportTreeNodeClicked( ReportTreeNode node )
    {
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

    internal async Task OnReportTreeNodeContextMenu( ReportTreeNodeMouseEventArgs eventArgs )
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

    internal Task OnFieldsTreeNodeDragStarted( ReportTreeNodeDragEventArgs eventArgs )
    {
        if ( eventArgs.Node?.Value is ReportFieldTreeNodeValue value )
        {
            workspaceRef?.BeginFieldDrag( value.DataSourceName, value.FieldName );
        }

        return Task.CompletedTask;
    }

    internal Task OnToolboxTreeNodeDragStarted( ReportTreeNodeDragEventArgs eventArgs )
    {
        if ( eventArgs.Node?.Value is ReportToolboxTreeNodeValue value )
        {
            workspaceRef?.BeginToolboxElementDrag( value.ElementType, value.Text );
        }

        return Task.CompletedTask;
    }

    internal IReadOnlyList<object> ResolveSectionRenderItems( ReportDefinition definition, ReportBandDefinition section, bool designMode )
    {
        return renderService.ResolveSectionRenderItems( definition, section, Data, designMode );
    }

    internal double GetReportPageWidth( ReportDefinition definition )
    {
        return renderService.GetPageWidth( definition );
    }

    internal double GetReportPageContentWidth( ReportDefinition definition )
    {
        return renderService.GetPageContentWidth( definition );
    }

    internal double GetDesignerSectionBodyLeft( ReportDefinition definition )
    {
        return GetReportPageWidthOffset() + ReportMeasurementConverter.ToCssPixelValue( GetReportPageMarginLeft( definition ) );
    }

    internal double GetReportPageHeight( ReportDefinition definition )
    {
        return definition?.Page?.Height ?? 0;
    }

    private static ReportPageMarginsDefinition GetReportPageMargins( ReportDefinition definition )
    {
        return definition?.Page?.Margins ?? new();
    }

    internal static double GetReportPageMarginBottom( ReportDefinition definition )
    {
        return Math.Max( 0, GetReportPageMargins( definition ).Bottom );
    }

    internal static double GetReportPageMarginLeft( ReportDefinition definition )
    {
        return Math.Max( 0, GetReportPageMargins( definition ).Left );
    }

    internal static double GetReportPageMarginRight( ReportDefinition definition )
    {
        return Math.Max( 0, GetReportPageMargins( definition ).Right );
    }

    internal static double GetReportPageMarginTop( ReportDefinition definition )
    {
        return Math.Max( 0, GetReportPageMargins( definition ).Top );
    }

    internal string GetPreviewPageContentStyle( ReportDefinition definition )
    {
        return renderService.GetPreviewPageContentStyle( definition );
    }

    internal string GetPreviewPageFooterStyle( ReportDefinition definition, ReportRenderPage renderPage )
    {
        return renderService.GetPreviewPageFooterStyle( definition, renderPage, GetDesignerSectionHeight );
    }

    internal double GetSectionRenderHeight( int sectionIndex, ReportBandDefinition section )
    {
        return GetDesignerSectionHeight( sectionIndex, section );
    }

    internal double GetReportPageWidthOffset()
    {
        return IsDesignerBandRailVisible() ? DesignerConstants.DesignerBandRailWidth : 0;
    }

    internal double GetSelectionBoxLeftOffset()
    {
        return GetDesignerSectionBodyLeft( EffectiveDefinition );
    }

    internal double GetDesignerSectionBodyTopOffset()
    {
        return CurrentBandMode == ReportBandMode.Classic ? ReportMeasurementConverter.FromCssPixelValue( DesignerConstants.DesignerBandHeaderHeight ) : 0;
    }

    internal bool IsDesignerBandRailVisible()
    {
        return CurrentBandMode == ReportBandMode.Rail;
    }

    internal bool IsSectionCollapsedForRender( ReportBandDefinition section )
    {
        return IsDesignerBandRailVisible() && !ReportValueResolver.ResolveStaticSuppress( section ) && IsSectionCollapsed( section );
    }

    internal bool IsSectionSelected( int sectionIndex )
    {
        return selectionManager.SelectedSectionIndex == sectionIndex
            && selectionManager.SelectedElementKeys.Count == 0;
    }

    internal async Task HandleDesignerShortcut( ReportDesignerShortcut shortcut )
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

            case ReportDesignerShortcut.Duplicate:
                await ExecuteCommandIfAvailable( ReportCommand.Duplicate );
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
                await MoveSelectedElements( -1, 0 );
                break;

            case ReportDesignerShortcut.MoveUp:
                await MoveSelectedElements( 0, -1 );
                break;

            case ReportDesignerShortcut.MoveRight:
                await MoveSelectedElements( 1, 0 );
                break;

            case ReportDesignerShortcut.MoveDown:
                await MoveSelectedElements( 0, 1 );
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
            ReportCommand.Save => SaveDefinition(),
            ReportCommand.Load => LoadRequestedDefinition(),
            ReportCommand.ConnectDataSource => OpenDataSourceConnectionDialog(),
            ReportCommand.DownloadPdf => DownloadPdf(),
            ReportCommand.Cut => CutSelectedElement(),
            ReportCommand.Copy => CopySelectedElement(),
            ReportCommand.Duplicate => DuplicateSelectedElement(),
            ReportCommand.Paste => PasteElement(),
            ReportCommand.Delete => DeleteSelection(),
            ReportCommand.Undo => Undo(),
            ReportCommand.Redo => Redo(),
            ReportCommand.Reset => ResetDefinition(),
            _ => Task.CompletedTask,
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
            ReportCommand.Save => SaveRequested is not null,
            ReportCommand.Load => LoadRequested is not null && CurrentDefinitionMode != ReportDefinitionMode.AlwaysUseDeclarative,
            ReportCommand.ConnectDataSource => CurrentMode == ReportMode.Design && IsDesignerEnabled && DataSourceProviders.Count > 0,
            ReportCommand.DownloadPdf => context.ViewerOptions.AllowDownload && SupportsPreviewFormat( ReportPreviewFormat.Pdf ) && PdfGenerator is not null,
            ReportCommand.Cut or ReportCommand.Copy or ReportCommand.Duplicate => CurrentMode == ReportMode.Design && GetSelectedElementContexts( definition ).Count > 0,
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
    /// Captures the current report definition, mode, selection, and clipboard.
    /// </summary>
    /// <returns>Interactive report designer state.</returns>
    public Task<ReportState> GetState()
    {
        return Task.FromResult( CaptureReportState( RootDefinition ) );
    }

    /// <summary>
    /// Gets a copy of the current persistent report definition.
    /// </summary>
    public Task<ReportDefinition> GetDefinition()
    {
        return Task.FromResult( ReportContext.CloneDefinition( RootDefinition ) );
    }

    /// <summary>
    /// Loads a persistent report definition.
    /// </summary>
    /// <param name="definition">Definition to load.</param>
    public async Task LoadDefinition( ReportDefinition definition )
    {
        if ( definition is null || CurrentDefinitionMode == ReportDefinitionMode.AlwaysUseDeclarative )
            return;

        ReportDefinition loadedDefinition = ReportContext.CloneDefinition( definition );

        await ResolveDataSources( loadedDefinition, CurrentMode == ReportMode.Preview );

        commandManager.Clear();
        await ApplyReportState( new()
        {
            Definition = loadedDefinition,
            Mode = CurrentMode,
            PreviewFormat = CurrentPreviewFormat,
        }, notifyDefinitionChanged: true, ReportDesignerRefreshTarget.All );
    }

    /// <summary>
    /// Restores a previously captured report designer state.
    /// </summary>
    /// <param name="state">State to apply to the report designer.</param>
    public async Task LoadState( ReportState state )
    {
        commandManager.Clear();
        await ApplyReportState( state, notifyDefinitionChanged: true, ReportDesignerRefreshTarget.All );
    }

    internal async Task ExecuteDesignerCommand( ReportDesignerCommand command )
    {
        ReportDefinition definition = await commandManager.Execute( command, RootDefinition, CaptureReportState );

        if ( command.NotifyDefinitionChanged && command.RefreshSurface )
            await DefinitionChanged.InvokeAsync( definition );

        if ( command.NotifyDefinitionChanged )
        {
            InvalidateDesignerCaches();
            RefreshDesigner( command.RefreshTargets );
        }

        await InvokeAsync( StateHasChanged );

        if ( command.NotifyDefinitionChanged && !command.RefreshSurface )
            _ = NotifyDefinitionChangedLater( definition );
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

            await ModeChanged.InvokeAsync( currentMode );
        }, TrackHistory: false, NotifyDefinitionChanged: false ) );
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

            if ( format == ReportPreviewFormat.Pdf )
                await ResolvePdfPreview();

            await ModeChanged.InvokeAsync( currentMode );
        }, TrackHistory: false, NotifyDefinitionChanged: false ) );
    }

    private async Task CaptureDesignerPaneScrollPositions()
    {
        if ( workspaceRef is not null )
            await workspaceRef.CapturePaneScrollPositions( designerPaneScrollPositions );
    }

    private void ResetDesignerSurfaceScrollPosition()
    {
        designerPaneScrollPositions[DesignerSurfacePaneName] = (0, 0);
        designerPaneScrollRestoreVersion++;
    }

    internal async Task DownloadPdf()
    {
        if ( PdfGenerator is null )
            return;

        PdfGenerationResult result = CurrentPdfPreviewResult;

        if ( result is null )
        {
            await ResolveDataSources( RootDefinition, true );
            result = await ResolvePdfPreview();
        }

        if ( result is null )
            return;

        reportingModule ??= new( JSRuntime, VersionProvider, BlazoriseOptions );
        await reportingModule.DownloadFile( result.FileName, result.ContentType, result.Content );
    }

    private async Task<PdfGenerationResult> ResolvePdfPreview()
    {
        PdfGenerationResult result = CurrentPdfPreviewResult;

        if ( result is not null || PdfGenerator is null )
            return result;

        ReportDefinition definition = RootDefinition;
        PdfDocumentDefinition pdfDocument = ReportPdfDocumentBuilder.Build( definition, Data );

        result = await PdfGenerator.Generate( pdfDocument, new()
        {
            FileName = ResolvePdfFileName( definition ),
        } );

        pdfPreviewResult = result;
        pdfPreviewContext = new(
            result.Content,
            result.ContentType,
            result.FileName,
            context.ViewerOptions.AllowPrint,
            context.ViewerOptions.AllowDownload,
            EventCallback.Factory.Create( this, DownloadPdf ) );
        pdfPreviewMutationVersion = renderMutationVersion;

        return result;
    }

    private async Task ResetDefinition()
    {
        if ( !await ConfirmDestructiveAction( "Are you sure you want to reset the report to its initial definition?", "Reset report", "Reset" ) )
            return;

        await ExecuteDesignerCommand( new( "Reset report", () =>
        {
            declarativeDefinition = BuildDeclarativeDefinition();
            activeSubreportElementKey = null;
            SelectReport();
            _ = CloseContextMenu();
            designerState.DragPreview = null;
            designerState.EditingElementKey = null;

            return Task.CompletedTask;
        }, () => declarativeDefinition, RefreshTargets: ReportDesignerRefreshTarget.All ) );
    }

    private Task<bool> ConfirmDestructiveAction( string message, string title, string confirmButtonText )
    {
        return MessageService.Confirm( message, title, options =>
        {
            options.ShowCloseButton = false;
            options.ShowMessageIcon = false;
            options.CancelButtonText = "Cancel";
            options.ConfirmButtonText = confirmButtonText;
            options.ConfirmButtonColor = Color.Danger;
        } );
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
                if ( !HasClipboardElements )
                    RefreshDesignerToolbar();

                clipboardElements = result.ClipboardElements;
                clipboardBandId = result.ClipboardBandId;
                _ = CloseContextMenu();
            }

            return Task.CompletedTask;
        }, TrackHistory: false, NotifyDefinitionChanged: false ) );
    }

    private async Task DuplicateSelectedElement()
    {
        await CopySelectedElement();
        await PasteElement();
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
            await ApplyReportState( state, notifyDefinitionChanged: true, commandManager.RefreshTargets );
    }

    private async Task Redo()
    {
        var state = commandManager.Redo();

        if ( state is not null )
            await ApplyReportState( state, notifyDefinitionChanged: true, commandManager.RefreshTargets );
    }

    private ReportState CaptureReportState( ReportDefinition definition )
    {
        definition = ReportDefinitionHelper.EnsureDefinitionIds( definition );

        return new()
        {
            Definition = ReportContext.CloneDefinition( definition ),
            Mode = CurrentMode,
            PreviewFormat = CurrentPreviewFormat,
            Selection = selectionManager.CaptureState( definition ),
            ClipboardElements = clipboardElements?.Select( ReportContext.CloneElement ).ToList() ?? [],
            ClipboardBandId = clipboardBandId,
        };
    }

    private async Task ApplyReportState( ReportState state, bool notifyDefinitionChanged, ReportDesignerRefreshTarget refreshTargets )
    {
        string previousActiveSubreportElementKey = activeSubreportElementKey;
        ReportState nextState = ReportContext.CloneState( state );
        ReportDefinition definition = ReportDefinitionHelper.EnsureDefinitionIds( nextState.Definition ?? BuildDeclarativeDefinition() );

        ReportDefinitionHelper.ApplyRowsLimit( definition, BlazoriseLicenseLimitsHelper.GetReportingRowsLimit( LicenseChecker ) );

        declarativeDefinition = definition;
        currentMode = nextState.Mode;
        currentPreviewFormat = nextState.PreviewFormat;
        activeSubreportElementKey = ResolveActiveSubreportElementKey( definition, previousActiveSubreportElementKey );
        clipboardElements = nextState.ClipboardElements?.Select( ReportContext.CloneElement ).ToList() ?? [];
        clipboardBandId = nextState.ClipboardBandId;
        selectionManager.ApplyState( definition, nextState.Selection );
        if ( !string.Equals( previousActiveSubreportElementKey, activeSubreportElementKey, StringComparison.Ordinal ) )
            ResetDesignerSurfaceScrollPosition();

        _ = CloseContextMenu();
        designerState.DragPreview = null;
        designerState.EditingElementKey = null;
        ClearDragState();

        commandManager.SetState( CaptureReportState( definition ) );

        InvalidateDesignerCaches();

        if ( CurrentMode == ReportMode.Preview && CurrentPreviewFormat == ReportPreviewFormat.Pdf )
            await ResolvePdfPreview();

        if ( notifyDefinitionChanged )
            await DefinitionChanged.InvokeAsync( definition );

        RefreshDesigner( refreshTargets );

        await InvokeAsync( StateHasChanged );
    }

    private void SelectReport()
    {
        bool selectionChanged = selectionManager.SelectReport();
        _ = CloseContextMenu();
        designerState.EditingElementKey = null;

        if ( selectionChanged )
        {
            RefreshDesignerElementSelection();
            _ = InvokeAsync( StateHasChanged );
        }
    }

    internal Task HandleElementClick( ReportDesignerSelectionMouseEventArgs eventArgs )
    {
        string key = eventArgs.Key;
        MouseEventArgs mouseEventArgs = eventArgs.MouseEventArgs;

        if ( IsSuppressingSelectionClick() )
            return Task.CompletedTask;

        if ( mouseEventArgs.Detail >= 2 )
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

        if ( mouseEventArgs.CtrlKey )
        {
            ToggleElementSelection( key );
            return Task.CompletedTask;
        }

        SelectElement( key, preserveSelection: selectionManager.IsElementSelected( key ) && selectionManager.SelectedElementKeys.Count > 1 );

        return Task.CompletedTask;
    }

    internal Task HandleElementDoubleClick( string key, MouseEventArgs eventArgs )
    {
        if ( TryOpenSubreportDesigner( key ) )
            return Task.CompletedTask;

        BeginElementTextEdit( key );

        return Task.CompletedTask;
    }

    internal void HandleSectionClick( int sectionIndex )
    {
        if ( IsSuppressingSelectionClick() )
            return;

        SelectSection( sectionIndex );
    }

    private bool IsSuppressingSelectionClick()
    {
        if ( DateTime.UtcNow > designerState.SuppressSelectionClickUntil )
            return false;

        designerState.SuppressSelectionClickUntil = default;
        designerState.SuppressNextElementClickKey = null;

        return true;
    }

    internal void SuppressNextSelectionClick()
    {
        designerState.SuppressSelectionClickUntil = DateTime.UtcNow.AddMilliseconds( DesignerConstants.SuppressSelectionClickMilliseconds );
    }

    internal void SelectElement( string key, bool preserveSelection = false )
    {
        bool selectionChanged = selectionManager.SelectElement( key, preserveSelection );
        _ = CloseContextMenu();

        if ( selectionChanged )
        {
            RefreshDesignerElementSelection();
            _ = InvokeAsync( StateHasChanged );
        }
    }

    internal void ToggleElementSelection( string key )
    {
        bool selectionChanged = selectionManager.ToggleElementSelection( key );
        _ = CloseContextMenu();

        if ( selectionChanged )
        {
            RefreshDesignerElementSelection();
            _ = InvokeAsync( StateHasChanged );
        }
    }

    internal void SelectElements( IEnumerable<string> elementKeys, string primaryElementKey = null )
    {
        bool selectionChanged = selectionManager.SelectElements( elementKeys, primaryElementKey );
        _ = CloseContextMenu();

        if ( selectionChanged )
        {
            RefreshDesignerElementSelection();
            _ = InvokeAsync( StateHasChanged );
        }
    }

    internal void SelectSection( int index )
    {
        bool selectionChanged = selectionManager.SelectSection( index );
        _ = CloseContextMenu();

        if ( selectionChanged )
        {
            RefreshDesignerElementSelection();
            _ = InvokeAsync( StateHasChanged );
        }
    }

    internal Task OnReportSelected()
    {
        SelectReport();

        return Task.CompletedTask;
    }

    internal Task HandleTableCellClick( ReportDesignerSelectionMouseEventArgs eventArgs )
    {
        if ( IsSuppressingSelectionClick() )
            return Task.CompletedTask;

        SelectTableCell( eventArgs.Key );

        return Task.CompletedTask;
    }

    internal void SelectTableCell( string cellKey )
    {
        bool selectionChanged = selectionManager.SelectCell( cellKey );
        _ = CloseContextMenu();

        if ( selectionChanged )
        {
            RefreshDesignerElementSelection();
            _ = InvokeAsync( StateHasChanged );
        }
    }

    internal void ToggleSectionCollapsed( ReportBandDefinition section )
    {
        if ( !AllowBandCollapse || section is null )
            return;

        var sectionId = ReportDefinitionHelper.EnsureBandId( section );

        if ( collapsedBandIds.Contains( sectionId ) )
            collapsedBandIds.Remove( sectionId );
        else
            collapsedBandIds.Add( sectionId );

        RefreshDesignerSurface();
    }

    internal Task ToggleSectionCollapsed( int sectionIndex, MouseEventArgs eventArgs )
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

    internal async Task OpenSectionContextMenu( int sectionIndex, MouseEventArgs eventArgs )
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

    internal async Task OpenSectionBodyContextMenu( int sectionIndex, MouseEventArgs eventArgs )
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

    internal async Task OpenElementContextMenu( string elementKey, MouseEventArgs eventArgs )
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

    internal async Task OpenTableCellContextMenu( int sectionIndex, string cellKey, MouseEventArgs eventArgs )
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

        if ( workspaceRef is not null )
            await workspaceRef.ShowContextMenu( state );

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

        ReportRunningTotalDefinition runningTotal = ReportRunningTotalResolver.FindRunningTotal( EffectiveDefinition, runningTotalName );

        if ( runningTotal is not null )
            await workspaceRef.ShowRunningTotalDialog( runningTotal );

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

    internal Task CancelElementTextEdit( string elementKey )
    {
        if ( string.Equals( designerState.EditingElementKey, elementKey, StringComparison.Ordinal ) )
        {
            designerState.EditingElementKey = null;
            RefreshDesignerSurface();
        }

        return Task.CompletedTask;
    }

    internal async Task CommitElementTextEdit( string elementKey, string text )
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
        if ( !IsElementContextMenuVisible() || workspaceRef is null )
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

        await workspaceRef.ShowAggregateDialog( fieldOptions, selectedFieldName, summaryLocations );
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

    internal async Task OnAggregateDialogConfirmed( ReportAggregateDialogResult result )
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

    internal bool CanSelectedSectionInsertGroup( ReportDefinition definition )
    {
        return aggregateService.CanInsertGroup( selectionManager.FindSelectedSection( definition ) );
    }

    internal bool CanSelectedSectionInsertSection( ReportDefinition definition )
    {
        return selectionManager.SelectedSectionIndex is { } sectionIndex
            && aggregateService.CanInsertSection( definition, sectionIndex );
    }

    internal async Task OpenSelectedDetailGroupDialog()
    {
        if ( workspaceRef is null )
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

        await workspaceRef.ShowGroupDialog( fieldOptions, selectedFieldName );
    }

    internal async Task OnGroupDialogConfirmed( string groupBy )
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
        if ( workspaceRef is null )
            return;

        await workspaceRef.ShowDataSourceConnectionDialog( EffectiveDefinition, DataSourceProviders );
    }

    internal async Task OnDataSourceConnectionConfirmed( ReportDataSourceDefinition dataSource )
    {
        if ( dataSource is null || string.IsNullOrWhiteSpace( dataSource.Name ) )
            return;

        await ExecuteDesignerCommand( new( "Connect data source", async () =>
        {
            await dataCommandService.ConnectDataSource( EffectiveDefinition, Data, dataSource, ResolveDataSources );
        }, RefreshTargets: ReportDesignerRefreshTarget.DesignerWithFieldsExplorer ) );
    }

    internal async Task OnDataSourceRefreshed( string dataSourceName )
    {
        if ( string.IsNullOrWhiteSpace( dataSourceName ) )
            return;

        await ExecuteDesignerCommand( new( "Refresh data source", async () =>
        {
            await dataCommandService.RefreshDataSource( EffectiveDefinition, DataSourceProviderRegistry, dataSourceName );
            await ResolveDataSources( EffectiveDefinition, CurrentMode == ReportMode.Preview );
        }, RefreshTargets: ReportDesignerRefreshTarget.DesignerWithFieldsExplorer ) );
    }

    internal async Task OnDataSourceDeleted( string dataSourceName )
    {
        if ( string.IsNullOrWhiteSpace( dataSourceName ) )
            return;

        if ( !await ConfirmDestructiveAction( "Are you sure you want to delete this data source?", "Delete data source", "Delete" ) )
            return;

        await ExecuteDesignerCommand( new( "Delete data source", () =>
        {
            dataCommandService.DeleteDataSource( RootDefinition, dataSourceName );
            return Task.CompletedTask;
        }, RefreshTargets: ReportDesignerRefreshTarget.DesignerWithFieldsExplorer ) );
    }

    internal async Task OnFormulaFieldConfirmed( ReportFormulaFieldDefinition formulaField )
    {
        if ( formulaField is null || string.IsNullOrWhiteSpace( formulaField.Name ) )
            return;

        await ExecuteDesignerCommand( new( "Save formula field", () =>
        {
            dataCommandService.SaveFormulaField( EffectiveDefinition, formulaField );
            return Task.CompletedTask;
        }, RefreshTargets: ReportDesignerRefreshTarget.DesignerWithFieldsExplorer ) );
    }

    internal async Task OnFormulaFieldRenamed( (string OldName, string NewName) formulaFieldRename )
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
        }, RefreshTargets: ReportDesignerRefreshTarget.DesignerWithFieldsExplorer ) );
    }

    internal async Task OnFormulaFieldDeleted( string formulaFieldName )
    {
        if ( string.IsNullOrWhiteSpace( formulaFieldName ) )
            return;

        await ExecuteDesignerCommand( new( "Delete formula field", () =>
        {
            dataCommandService.DeleteFormulaField( EffectiveDefinition, formulaFieldName );

            if ( string.Equals( editingFormulaFieldName, formulaFieldName, StringComparison.OrdinalIgnoreCase ) )
                editingFormulaFieldName = null;

            return Task.CompletedTask;
        }, RefreshTargets: ReportDesignerRefreshTarget.DesignerWithFieldsExplorer ) );
    }

    internal async Task OnFormulaFieldInserted( string formulaFieldName )
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

    internal async Task OnFieldInserted( (string DataSourceName, string FieldName) field )
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

    internal async Task OnRunningTotalConfirmed( ReportRunningTotalDefinition runningTotal )
    {
        if ( runningTotal is null || string.IsNullOrWhiteSpace( runningTotal.Name ) )
            return;

        await ExecuteDesignerCommand( new( "Save running total", () =>
        {
            dataCommandService.SaveRunningTotal( EffectiveDefinition, runningTotal );
            return Task.CompletedTask;
        }, RefreshTargets: ReportDesignerRefreshTarget.DesignerWithFieldsExplorer ) );
    }

    internal async Task OnRunningTotalRenamed( (string OldName, string NewName) runningTotalRename )
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
        }, RefreshTargets: ReportDesignerRefreshTarget.DesignerWithFieldsExplorer ) );
    }

    internal async Task OnRunningTotalDeleted( string runningTotalName )
    {
        if ( string.IsNullOrWhiteSpace( runningTotalName ) )
            return;

        await ExecuteDesignerCommand( new( "Delete running total", () =>
        {
            dataCommandService.DeleteRunningTotal( EffectiveDefinition, runningTotalName );
            return Task.CompletedTask;
        }, RefreshTargets: ReportDesignerRefreshTarget.DesignerWithFieldsExplorer ) );
    }

    internal async Task OnRunningTotalInserted( string runningTotalName )
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

    internal async Task OnFormulaDialogConfirmed( string formula )
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

        ReportFormulaFieldDefinition formulaField = ReportFormulaFieldResolver.FindFormulaField( EffectiveDefinition, formulaFieldName );

        if ( formulaField is null )
            return;

        editingFormulaFieldName = formulaField.Name;
        if ( workspaceRef is not null )
            await workspaceRef.ShowFormulaDialog( formulaField.Name, formulaField.Formula );
    }

    private int GetDataElementInsertionSectionIndex( ReportDefinition definition )
    {
        if ( definition?.Bands is null )
            return -1;

        if ( selectionManager.SelectedSectionIndex is { } sectionIndex
             && sectionIndex >= 0
             && sectionIndex < definition.Bands.Count )
        {
            return sectionIndex;
        }

        return !string.IsNullOrWhiteSpace( selectionManager.PrimaryElementKey )
               && ReportDefinitionHelper.TryFindElementLocation( definition, selectionManager.PrimaryElementKey, out int elementSectionIndex, out _, out _ )
            ? elementSectionIndex
            : -1;
    }

    private double GetNextDataElementInsertionY( ReportBandDefinition section )
    {
        if ( section?.Elements is null || section.Elements.Count == 0 )
            return 0;

        double y = section.Elements.Max( element => element.Y + element.Height ) + ReportDesignerConstants.DefaultDroppedFieldHeight;

        return ApplyDesignerGrid( y );
    }

    internal IReadOnlyList<ReportAggregateFunction> ResolveAggregateDialogSupportedFunctions( ReportDesignerFieldOption field )
    {
        return aggregateService.ResolveSupportedFunctions( EffectiveDefinition, Data, field );
    }

    internal bool IsElementTextEditing( string elementKey = null )
    {
        return string.IsNullOrWhiteSpace( elementKey )
            ? !string.IsNullOrWhiteSpace( designerState.EditingElementKey )
            : string.Equals( designerState.EditingElementKey, elementKey, StringComparison.Ordinal );
    }

    internal Task CloseContextMenu()
    {
        designerState.ContextMenu = null;

        return workspaceRef?.CloseContextMenu() ?? Task.CompletedTask;
    }

    internal Task OnDesignerNodeDragEnded( ReportTreeNode node )
    {
        return workspaceRef?.CompleteExternalDrag() ?? Task.CompletedTask;
    }

    internal Task ExecuteContextMenuCommand( object value )
    {
        if ( value is not ReportDesignerContextMenuCommand command )
            return Task.CompletedTask;

        switch ( command )
        {
            case ReportDesignerContextMenuCommand.CutElement:
                return CutSelectedElement();
            case ReportDesignerContextMenuCommand.CopyElement:
                return CopySelectedElement();
            case ReportDesignerContextMenuCommand.DuplicateElement:
                return DuplicateSelectedElement();
            case ReportDesignerContextMenuCommand.PasteElement:
                return PasteElement();
            case ReportDesignerContextMenuCommand.SelectAllSectionElements:
                return SelectAllContextSectionElements();
            case ReportDesignerContextMenuCommand.ShowProperties:
                return ShowContextProperties();
            case ReportDesignerContextMenuCommand.InsertSectionBefore:
                return InsertSection( insertAfter: false );
            case ReportDesignerContextMenuCommand.InsertSectionAfter:
                return InsertSection( insertAfter: true );
            case ReportDesignerContextMenuCommand.InsertGroup:
                return OpenSelectedDetailGroupDialog();
            case ReportDesignerContextMenuCommand.InsertSubreport:
                return InsertSubreport();
            case ReportDesignerContextMenuCommand.ToggleSectionSuppression:
                return ToggleSelectedSectionSuppression();
            case ReportDesignerContextMenuCommand.ToggleSectionKeepTogether:
                return ToggleSelectedSectionKeepTogether();
            case ReportDesignerContextMenuCommand.ToggleSectionNewPageBefore:
                return ToggleSelectedSectionNewPageBefore();
            case ReportDesignerContextMenuCommand.ToggleSectionNewPageAfter:
                return ToggleSelectedSectionNewPageAfter();
            case ReportDesignerContextMenuCommand.DeleteSection:
                return DeleteSelectedSection();
            case ReportDesignerContextMenuCommand.AlignTops:
                return AlignSelectedElements( ReportElementAlignment.Tops );
            case ReportDesignerContextMenuCommand.AlignMiddles:
                return AlignSelectedElements( ReportElementAlignment.Middles );
            case ReportDesignerContextMenuCommand.AlignBottoms:
                return AlignSelectedElements( ReportElementAlignment.Bottoms );
            case ReportDesignerContextMenuCommand.AlignBaseline:
                return AlignSelectedElements( ReportElementAlignment.Baseline );
            case ReportDesignerContextMenuCommand.AlignLefts:
                return AlignSelectedElements( ReportElementAlignment.Lefts );
            case ReportDesignerContextMenuCommand.AlignCenters:
                return AlignSelectedElements( ReportElementAlignment.Centers );
            case ReportDesignerContextMenuCommand.AlignRights:
                return AlignSelectedElements( ReportElementAlignment.Rights );
            case ReportDesignerContextMenuCommand.AlignToGrid:
                return AlignSelectedElements( ReportElementAlignment.ToGrid );
            case ReportDesignerContextMenuCommand.SizeSameWidth:
                return SizeSelectedElements( ReportElementSizeMode.SameWidth );
            case ReportDesignerContextMenuCommand.SizeSameHeight:
                return SizeSelectedElements( ReportElementSizeMode.SameHeight );
            case ReportDesignerContextMenuCommand.SizeSameSize:
                return SizeSelectedElements( ReportElementSizeMode.SameSize );
            case ReportDesignerContextMenuCommand.BringToFront:
                return OrderSelectedElements( ReportElementOrderMode.BringToFront );
            case ReportDesignerContextMenuCommand.SendToBack:
                return OrderSelectedElements( ReportElementOrderMode.SendToBack );
            case ReportDesignerContextMenuCommand.MoveForward:
                return OrderSelectedElements( ReportElementOrderMode.MoveForward );
            case ReportDesignerContextMenuCommand.MoveBackward:
                return OrderSelectedElements( ReportElementOrderMode.MoveBackward );
            case ReportDesignerContextMenuCommand.InsertAggregate:
                return OpenContextElementAggregateDialog();
            case ReportDesignerContextMenuCommand.EditText:
                BeginContextElementTextEdit();
                return Task.CompletedTask;
            case ReportDesignerContextMenuCommand.EditFormula:
                return OpenContextElementFormulaDialog();
            case ReportDesignerContextMenuCommand.EditRunningTotal:
                return OpenContextElementRunningTotalDialog();
            case ReportDesignerContextMenuCommand.DeleteElement:
                return DeleteSelectedElement();
            case ReportDesignerContextMenuCommand.ToggleElementCanGrow:
                return ToggleSelectedElementCanGrow();
            case ReportDesignerContextMenuCommand.ToggleElementSuppression:
                return ToggleSelectedElementSuppression();
            case ReportDesignerContextMenuCommand.MergeCellRight:
                return MergeSelectedTableCellRight();
            case ReportDesignerContextMenuCommand.MergeCellDown:
                return MergeSelectedTableCellDown();
            case ReportDesignerContextMenuCommand.UnmergeCell:
                return UnmergeSelectedTableCell();
            case ReportDesignerContextMenuCommand.InsertTableRowAbove:
                return InsertSelectedTableRow( insertBelow: false );
            case ReportDesignerContextMenuCommand.InsertTableRowBelow:
                return InsertSelectedTableRow( insertBelow: true );
            case ReportDesignerContextMenuCommand.InsertTableColumnLeft:
                return InsertSelectedTableColumn( insertRight: false );
            case ReportDesignerContextMenuCommand.InsertTableColumnRight:
                return InsertSelectedTableColumn( insertRight: true );
            case ReportDesignerContextMenuCommand.InsertTableCell:
                return InsertSelectedTableCell();
            case ReportDesignerContextMenuCommand.DeleteTableRow:
                return DeleteSelectedTableRow();
            case ReportDesignerContextMenuCommand.DeleteTableColumn:
                return DeleteSelectedTableColumn();
            case ReportDesignerContextMenuCommand.DeleteTableCell:
                return DeleteSelectedTableCell();
            default:
                return Task.CompletedTask;
        }
    }

    private async Task MoveSelectedElements( double x, double y )
    {
        ReportDefinition definition = EffectiveDefinition;
        ReportElementDefinition element = selectionManager.FindSelectedElement( definition );

        if ( element is null )
            return;

        bool useSnapToGrid = IsSnapToGridEnabled( element );
        double moveStep = useSnapToGrid ? GridSize : DesignerConstants.KeyboardMoveStep;
        List<ReportElementPointerItemState> selectedElements = CaptureElementPointerItems( definition, ReportDefinitionHelper.EnsureElementId( element ) ).ToList();

        if ( selectedElements.Count == 0 )
            return;

        string commandName = selectedElements.Count == 1 ? "Move element" : "Move elements";

        await ExecuteDesignerCommand( new( commandName, () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            ReportElementCommandResult result = elementCommandService.MoveElements( definition, selectedElements, ReportDefinitionHelper.EnsureElementId( element ), x * moveStep, y * moveStep, useSnapToGrid, ApplyDesignerGrid );

            if ( result.SelectedElementKeys.Count > 0 )
                SelectElements( result.SelectedElementKeys, result.PrimaryElementKey );

            return Task.CompletedTask;
        }, RefreshSurface: false ) );
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
        }, RefreshSurface: false ) );
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
        }, RefreshSurface: false ) );
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
        }, RefreshSurface: false ) );
    }

    private List<ReportSelectedElementContext> GetSelectedElementContexts( ReportDefinition definition )
    {
        return elementLayoutService.GetSelectedElementContexts( definition, selectionManager.SelectedElementKeys, selectionManager.PrimaryElementKey );
    }

    internal Task UpdateSelectedElementsFromProperties( Action<ReportElementDefinition> update )
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

    internal async Task UpdateSelectedSection( Action<ReportBandDefinition> update )
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

    internal async Task UpdateReportPage( Action<ReportPageDefinition> update )
    {
        await ExecuteDesignerCommand( new( "Update page", () =>
        {
            var definition = EffectiveDefinition;

            update?.Invoke( definition.Page );
            definition.Page = ResolvePage( definition.Page );

            return Task.CompletedTask;
        } ) );
    }

    internal async Task InsertSection( bool insertAfter )
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
            subreport.Report.RowsLimit = rootDefinition.RowsLimit;

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

    internal async Task DeleteSelectedSection()
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

    private async Task ShowContextProperties()
    {
        selectedDesignerPanelTab = ReportDesignerPanelTab.Properties;
        await CloseContextMenu();
        await ( workspaceRef?.ShowPropertiesPane() ?? Task.CompletedTask );
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

    internal async Task UpdateSelectedSectionSuppression( bool suppressed )
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

    internal double ApplyDesignerGrid( double value )
    {
        return ApplyDesignerGrid( value, SnapToGrid );
    }

    private Task SaveDefinition()
    {
        return SaveRequested?.Invoke( ReportContext.CloneDefinition( RootDefinition ) ) ?? Task.CompletedTask;
    }

    private async Task LoadRequestedDefinition()
    {
        if ( LoadRequested is null )
            return;

        ReportDefinition definition = await LoadRequested();

        if ( definition is not null )
            await LoadDefinition( definition );
    }

    internal double ApplyDesignerGrid( double value, bool useSnapToGrid )
    {
        return useSnapToGrid ? ReportLayoutGeometry.SnapToGrid( value, GridSize ) : Math.Max( 0, value );
    }

    internal bool IsSnapToGridEnabled( ReportElementDefinition element )
    {
        return element?.SnapToGrid?.Value ?? SnapToGrid;
    }

    internal IEnumerable<string> FindElementsInsideSelectionBox( ReportDefinition definition, ReportDesignerSelectionBox selectionBox )
    {
        return ReportDesignerInteractionService.FindElementsInsideSelectionBox(
            definition,
            selectionBox,
            IsSectionCollapsed,
            sectionIndex => GetSectionOffsetY( definition, sectionIndex ) );
    }

    internal IEnumerable<ReportElementPointerItemState> CaptureElementPointerItems( ReportDefinition definition, string activeElementKey )
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

    internal double GetSectionOffsetY( ReportDefinition definition, int sectionIndex )
    {
        return GetReportPageMarginTop( definition ) + ReportLayoutGeometry.GetSectionOffsetY( definition, sectionIndex, GetDesignerSectionHeight );
    }

    internal double GetDesignerContentHeight( ReportDefinition definition )
    {
        return GetReportPageMarginTop( definition )
            + ReportLayoutGeometry.GetContentHeight( definition, GetDesignerSectionHeight )
            + GetReportPageMarginBottom( definition );
    }

    internal double GetDesignerRulerHeight( ReportDefinition definition )
    {
        return Math.Max( definition?.Page?.Height ?? 0, GetDesignerContentHeight( definition ) );
    }

    internal ReportDesignerRulerMarker GetDesignerRulerMarker( ReportDefinition definition )
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

    internal double GetElementPageY( ReportDefinition definition, int sectionIndex, double elementY )
    {
        return GetSectionOffsetY( definition, sectionIndex ) + GetDesignerSectionBodyTopOffset() + elementY;
    }

    internal double GetDesignerSectionHeight( int sectionIndex, ReportBandDefinition section )
    {
        if ( designerState.SectionPointerResize?.SectionIndex == sectionIndex )
            return designerState.SectionPointerResize.TargetHeight + GetDesignerSectionBodyTopOffset();

        if ( CurrentBandMode == ReportBandMode.Rail
             && !ReportValueResolver.ResolveStaticSuppress( section )
             && IsSectionCollapsed( section ) )
        {
            return ReportMeasurementConverter.FromCssPixelValue( ReportDesignerConstants.DesignerCollapsedBandHeight );
        }

        return ( section?.Height ?? 0 ) + GetDesignerSectionBodyTopOffset();
    }

    private void InvalidateDesignerCaches()
    {
        renderMutationVersion++;
        renderService.Invalidate();
    }

    private void RefreshDesignerSurface()
    {
        workspaceRef?.RefreshSurface();
    }

    private void RefreshDesignerElementSelection()
        => RefreshDesigner( ReportDesignerRefreshTarget.Designer | ReportDesignerRefreshTarget.ElementSelection );

    private void RefreshDesignerSelection()
        => RefreshDesigner( ReportDesignerRefreshTarget.Designer );

    private void RefreshDesignerToolbar()
        => RefreshDesigner( ReportDesignerRefreshTarget.Toolbar );

    private void RefreshDesigner( ReportDesignerRefreshTarget targets )
    {
        designerRefreshState = designerRefreshState with
        {
            Surface = designerRefreshState.Surface + ( ( targets & ReportDesignerRefreshTarget.Surface ) != 0 ? 1 : 0 ),
            SelectedPanel = designerRefreshState.SelectedPanel + ( ( targets & ReportDesignerRefreshTarget.SelectedPanel ) != 0 ? 1 : 0 ),
            ElementSelection = designerRefreshState.ElementSelection + ( ( targets & ReportDesignerRefreshTarget.ElementSelection ) != 0 ? 1 : 0 ),
            FieldsExplorer = designerRefreshState.FieldsExplorer + ( ( targets & ReportDesignerRefreshTarget.FieldsExplorer ) != 0 ? 1 : 0 ),
            Toolbar = designerRefreshState.Toolbar + ( ( targets & ReportDesignerRefreshTarget.Toolbar ) != 0 ? 1 : 0 ),
        };
    }

    internal static double GetMinimumSectionHeight( ReportBandDefinition section )
    {
        return ReportLayoutGeometry.GetMinimumSectionHeight( section );
    }

    internal Task OnSnapToGridChanged( bool value )
        => SnapToGrid == value
            ? Task.CompletedTask
            : UpdateDesignerDefinition( "Update snap to grid", designer => designer.SnapToGrid = value, ReportDesignerRefreshTarget.SelectedPanel );

    internal Task OnGridSizeChanged( double value )
    {
        double gridSize = Math.Max( 1, value );

        return GridSize == gridSize
            ? Task.CompletedTask
            : UpdateDesignerDefinition( "Update grid size", designer => designer.GridSize = gridSize, ReportDesignerRefreshTarget.Surface | ReportDesignerRefreshTarget.SelectedPanel );
    }

    internal async Task OnShowRulersChanged( bool value )
    {
        if ( CurrentShowRulers == value )
            return;

        await UpdateDesignerDefinition( "Update ruler visibility", designer => designer.ShowRulers = value, ReportDesignerRefreshTarget.Surface | ReportDesignerRefreshTarget.SelectedPanel );

        await ShowRulersChanged.InvokeAsync( value );
    }

    internal async Task OnShowFineRulerTicksChanged( bool value )
    {
        if ( CurrentShowFineRulerTicks == value )
            return;

        await UpdateDesignerDefinition( "Update fine ruler ticks", designer => designer.ShowFineRulerTicks = value, ReportDesignerRefreshTarget.Surface | ReportDesignerRefreshTarget.SelectedPanel );

        await ShowFineRulerTicksChanged.InvokeAsync( value );
    }

    internal async Task OnShowCursorGuidesChanged( bool value )
    {
        if ( CurrentShowCursorGuides == value )
            return;

        await UpdateDesignerDefinition( "Update cursor guides", designer => designer.ShowCursorGuides = value, ReportDesignerRefreshTarget.Surface | ReportDesignerRefreshTarget.SelectedPanel );

        await ShowCursorGuidesChanged.InvokeAsync( value );
    }

    internal async Task OnShowCollisionWarningsChanged( bool value )
    {
        if ( CurrentShowCollisionWarnings == value )
            return;

        await UpdateDesignerDefinition( "Update collision warnings", designer => designer.ShowCollisionWarnings = value, ReportDesignerRefreshTarget.Surface | ReportDesignerRefreshTarget.SelectedPanel );

        await ShowCollisionWarningsChanged.InvokeAsync( value );
    }

    internal async Task OnBandModeChanged( ReportBandMode value )
    {
        if ( CurrentBandMode == value )
            return;

        await UpdateDesignerDefinition( "Update band mode", designer => designer.BandMode = value, ReportDesignerRefreshTarget.Surface | ReportDesignerRefreshTarget.SelectedPanel );

        await BandModeChanged.InvokeAsync( value );
    }

    private Task UpdateDesignerDefinition( string commandName, Action<ReportDesignerDefinition> update, ReportDesignerRefreshTarget refreshTargets )
    {
        return ExecuteDesignerCommand( new( commandName, () =>
        {
            update( DesignerDefinition );

            return Task.CompletedTask;
        }, RefreshTargets: refreshTargets ) );
    }

    internal void ClearDragState()
    {
        ReportDesignerInteractionService.ClearDragState( designerState );
    }

    private bool SupportsPreviewFormat( ReportPreviewFormat format )
    {
        return ( PreviewFormats ?? context.ViewerOptions.PreviewFormats ).HasFlag( format );
    }

    private static string ResolvePdfFileName( ReportDefinition definition )
    {
        string name = string.IsNullOrWhiteSpace( definition?.Name ) ? "report" : definition.Name.Trim();

        foreach ( char invalidCharacter in Path.GetInvalidFileNameChars() )
            name = name.Replace( invalidCharacter, '-' );

        return name.EndsWith( ".pdf", StringComparison.OrdinalIgnoreCase ) ? name : $"{name}.pdf";
    }

    internal bool ShouldRenderElement( ReportDefinition definition, ReportBandDefinition section, ReportElementDefinition element, object item )
    {
        return !ReportValueResolver.ResolveSuppress( element, section, definition, Data, item );
    }

    internal ReportDefinition ResolveActiveDesignerDefinition( ReportDefinition rootDefinition )
    {
        if ( CurrentMode != ReportMode.Design || !TryGetActiveSubreportElement( rootDefinition, out ReportSubreportElementDefinition subreportElement ) )
        {
            return rootDefinition;
        }

        return ReportDefinitionHelper.EnsureSubreportDefinition( subreportElement ) ?? rootDefinition;
    }

    internal object GetFieldsExplorerData( ReportDefinition rootDefinition )
    {
        return TryGetActiveSubreportElement( rootDefinition, out ReportSubreportElementDefinition subreportElement )
            ? ReportSubreportResolver.ResolveData( rootDefinition, Data, null, subreportElement )
            : Data;
    }

    internal IReadOnlyList<ReportDesignerDataSourceNode> GetFieldsExplorerDataSources( ReportDefinition rootDefinition )
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

    internal IReadOnlyList<ReportDesignerTabItem> GetDesignerTabs( ReportDefinition rootDefinition )
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

    internal Task SelectDesignerTab( string key )
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
        RefreshDesignerSelection();

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
        RefreshDesignerSelection();
        StateHasChanged();

        return true;
    }

    internal bool IsToolbarCommandVisible( ReportCommand command )
        => context.HiddenToolbarCommands?.Contains( command ) != true;

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
            : declarativeDefinition ?? Definition ?? BuildDeclarativeDefinition();

    private ReportDefinition EffectiveDefinition
        => ResolveActiveDesignerDefinition( RootDefinition );

    private ReportDesignerDefinition DesignerDefinition
        => RootDefinition.Designer ??= new();

    internal ReportDefinition DesignerRootDefinition => RootDefinition;

    internal ReportDesignerInteractionState InteractionState => designerState;

    internal bool SnapToGrid => DesignerDefinition.SnapToGrid;

    internal double GridSize => DesignerDefinition.GridSize;

    internal bool CurrentShowRulers => DesignerDefinition.ShowRulers;

    internal bool CurrentShowFineRulerTicks => DesignerDefinition.ShowFineRulerTicks;

    internal bool CurrentShowCursorGuides => DesignerDefinition.ShowCursorGuides;

    internal bool CurrentShowCollisionWarnings => DesignerDefinition.ShowCollisionWarnings;

    internal ReportBandMode CurrentBandMode => DesignerDefinition.BandMode;

    internal ReportSelectionManager Selection => selectionManager;

    internal ReportDesignerRefreshState RefreshState => designerRefreshState;

    internal ReportTableEditor TableEditor => tableEditor;

    internal bool CanInsertSubreport => CanInsertSubreportElement;

    internal DockLayoutState DesignerDockLayoutState => designerDockLayoutState;

    internal Dictionary<string, (double Left, double Top)> DesignerPaneScrollPositions => designerPaneScrollPositions;

    internal int DesignerPaneScrollRestoreVersion => designerPaneScrollRestoreVersion;

    internal string SelectedDesignerPanel => SelectedDesignerPanelTabName;

    internal string DefaultDataSourceName => DataSourceName;

    internal ReportDefinition PreviewDefinition => RootDefinition;

    internal ReportPreviewFormat ActivePreviewFormat => CurrentPreviewFormat;

    internal ReportPdfPreviewContext PdfPreviewContext => pdfPreviewMutationVersion == renderMutationVersion ? pdfPreviewContext : null;

    internal RenderFragment<ReportPdfPreviewContext> PdfPreviewTemplate => context.ViewerOptions.PdfPreviewTemplate;

    internal int RenderMutationVersion => renderMutationVersion;

    internal string ToolbarRenderKey => ToolbarStateKey;

    internal ReportToolbarContext Toolbar => toolbarContext;

    internal RenderFragment ToolbarContent => context.ToolbarContent;

    internal RenderFragment<ReportToolbarItemContext> ToolbarButtonTemplate => context.ToolbarButtonTemplate;

    internal bool ShowToolbarPanesMenu => context.ShowToolbarPanesMenu;

    internal bool ShowToolbarPersistenceButtons => context.ShowToolbarPersistenceButtons;

    internal bool ShowToolbarEditButtons => context.ShowToolbarEditButtons;

    internal bool ShowToolbarHistoryButtons => context.ShowToolbarHistoryButtons;

    internal bool ShowToolbarDataSourceButtons => context.ShowToolbarDataSourceButtons;

    internal bool ShowToolbarExportButtons => context.ShowToolbarExportButtons;

    internal bool ShowToolbarModeButtons => context.ShowToolbarModeButtons;

    internal bool DesignerAvailable => IsDesignerEnabled;

    internal IReadOnlyList<ReportRenderPage> ResolvePreviewRenderPages( ReportDefinition definition )
        => renderService.ResolvePreviewRenderPages( definition, Data, renderMutationVersion );

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

    private PdfGenerationResult CurrentPdfPreviewResult => pdfPreviewMutationVersion == renderMutationVersion ? pdfPreviewResult : null;

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
    /// PDF generator used by the report viewer download command.
    /// </summary>
    [Inject] private IPdfGenerator PdfGenerator { get; set; }

    /// <summary>
    /// Message service used to confirm destructive report commands.
    /// </summary>
    [Inject] private IMessageService MessageService { get; set; }

    /// <summary>
    /// License checker used to resolve the maximum number of report rows.
    /// </summary>
    [Inject] private BlazoriseLicenseChecker LicenseChecker { get; set; }

    /// <summary>
    /// Persisted report definition used by the designer and viewer.
    /// </summary>
    [Parameter] public ReportDefinition Definition { get; set; }

    /// <summary>
    /// Raised when the report definition changes through designer commands.
    /// </summary>
    [Parameter] public EventCallback<ReportDefinition> DefinitionChanged { get; set; }

    /// <summary>
    /// Handles requests to save the current report definition.
    /// </summary>
    [Parameter] public Func<ReportDefinition, Task> SaveRequested { get; set; }

    /// <summary>
    /// Handles requests to load a report definition.
    /// </summary>
    [Parameter] public Func<Task<ReportDefinition>> LoadRequested { get; set; }

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
    /// Band presentation used when constructing a report from declarative content. Persisted definitions retain their configured value.
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
    /// Defines collision warning visibility when constructing a report from declarative content. Persisted definitions retain their configured value.
    /// </summary>
    [Parameter] public bool ShowCollisionWarnings { get; set; } = true;

    /// <summary>
    /// Raised when collision warning visibility changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowCollisionWarningsChanged { get; set; }

    /// <summary>
    /// Defines ruler visibility when constructing a report from declarative content. Persisted definitions retain their configured value.
    /// </summary>
    [Parameter] public bool ShowRulers { get; set; } = true;

    /// <summary>
    /// Raised when designer ruler visibility changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowRulersChanged { get; set; }

    /// <summary>
    /// Defines fine ruler tick visibility when constructing a report from declarative content. Persisted definitions retain their configured value.
    /// </summary>
    [Parameter] public bool ShowFineRulerTicks { get; set; }

    /// <summary>
    /// Raised when fine-grained ruler tick visibility changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowFineRulerTicksChanged { get; set; }

    /// <summary>
    /// Defines cursor guide visibility when constructing a report from declarative content. Persisted definitions retain their configured value.
    /// </summary>
    [Parameter] public bool ShowCursorGuides { get; set; }

    /// <summary>
    /// Raised when cursor guide visibility changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowCursorGuidesChanged { get; set; }

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

    internal sealed class ReportDesignerTabItem
    {
        public string Key { get; set; }

        public string Text { get; set; }

        public bool Active { get; set; }
    }
}