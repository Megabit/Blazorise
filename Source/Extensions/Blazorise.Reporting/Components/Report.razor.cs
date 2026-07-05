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

    private readonly Dictionary<string, ( double Left, double Top )> designerPaneScrollPositions = new( StringComparer.Ordinal );

    private readonly HashSet<string> collapsedSectionIds = new( StringComparer.Ordinal );

    private readonly IReadOnlyList<IReportDataSourceProvider> fallbackDataSourceProviders = [new ObjectReportDataSourceProvider()];

    private DotNetObjectReference<Report> dotNetObjectReference;

    private ReportDefinition declarativeDefinition;

    private ReportStudioMode currentMode;

    private ReportPreviewFormat currentPreviewFormat;

    private ReportDesignerPanelTab selectedDesignerPanelTab = ReportDesignerPanelTab.Properties;

    private int designerPaneScrollRestoreVersion;

    private _ReportDesignerContextMenuHost contextMenuHost;

    private _ReportDesignerLayout designerLayoutRef;

    private _ReportDesignerPage designerPageRef;

    private ( ReportDefinition Definition, object Data ) observedParameters;

    private int collapsedSectionsVersion;

    private ReportElementDefinition clipboardElement;

    private string clipboardSectionId;

    private ReportOptions globalOptions;

    private IReportDataSourceProviderRegistry dataSourceProviderRegistry;

    private JSReportingModule reportingModule;

    private _ReportDesignerAggregateDialog aggregateDialogRef;

    private _ReportDesignerRunningTotalDialog runningTotalDialogRef;

    private _ReportDesignerGroupDialog groupDialogRef;

    private _ReportDesignerDataSourceConnectionDialog dataSourceConnectionDialogRef;

    private _ReportDesignerFormulaDialog formulaDialogRef;

    private string editingFormulaFieldName;

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

        currentMode = IsDesignerEnabled ? ReportStudioMode.Design : ReportStudioMode.Preview;
        currentPreviewFormat = DefaultPreviewFormat ?? context.ViewerOptions.DefaultFormat;
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        if ( !ReferenceEquals( observedParameters.Definition, Definition ) || !ReferenceEquals( observedParameters.Data, Data ) )
        {
            observedParameters = ( Definition, Data );
            InvalidateDesignerCaches();
        }

        if ( Definition is not null )
            await ResolveDataSourcesAsync( Definition, CurrentMode == ReportStudioMode.Preview );
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender && Definition is null && CurrentDefinitionMode != ReportDefinitionMode.UseDefinitionOnly )
        {
            declarativeDefinition = BuildDeclarativeDefinition();
            InvalidateDesignerCaches();

            await ResolveDataSourcesAsync( declarativeDefinition, CurrentMode == ReportStudioMode.Preview );

            if ( DefinitionChanged.HasDelegate )
            {
                await DefinitionChanged.InvokeAsync( declarativeDefinition );
            }

            StateHasChanged();
        }

        if ( firstRender && commandManager.State?.Definition is null )
            commandManager.SetState( CaptureReportState( EffectiveDefinition ) );
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
                Type = ObjectReportDataSourceProvider.ProviderType,
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

    private async Task ResolveDataSourcesAsync( ReportDefinition definition, bool loadData )
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

            IReportDataSourceProvider provider = registry.FindProvider( dataSource?.Type );

            if ( provider is null )
                continue;

            try
            {
                if ( loadData && dataSource.Data is null )
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

    private bool IsElementContextMenuVisible()
    {
        return designerState.ContextMenu?.Visible == true
            && designerState.ContextMenu.Target == ReportContextMenuTarget.Element;
    }

    private ReportSectionDefinition GetSelectedPropertiesSection( ReportDefinition definition )
    {
        return string.IsNullOrWhiteSpace( selectionManager.SelectedElementKey )
            ? selectionManager.FindSelectedSection( definition )
            : null;
    }

    private ReportSectionDefinition GetSelectedFormulaSection( ReportDefinition definition )
    {
        if ( string.IsNullOrWhiteSpace( selectionManager.SelectedElementKey ) )
            return selectionManager.FindSelectedSection( definition );

        return ReportDefinitionHelper.TryFindElementLocation( definition, selectionManager.SelectedElementKey, out var sectionIndex, out _, out _ )
            ? definition.Sections[sectionIndex]
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
                && EffectiveDefinition.Sections[elementSectionIndex].Suppressed )
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
            await OpenSectionContextMenuAsync( sectionIndex, eventArgs.MouseEventArgs );
        }
        else if ( ReportDesignerTreeBuilder.TryResolveElementTreeNode( eventArgs.Node, out var elementKey ) )
        {
            await OpenElementContextMenuAsync( elementKey, eventArgs.MouseEventArgs );
        }
        else if ( ReportDesignerTreeBuilder.TryResolveTableCellTreeNode( eventArgs.Node, out var cellKey )
            && ReportDefinitionHelper.TryFindTableCellLocation( EffectiveDefinition, cellKey, out var cellSectionIndex, out _, out _, out _ ) )
        {
            await OpenTableCellContextMenuAsync( cellSectionIndex, cellKey, eventArgs.MouseEventArgs );
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

    private IReadOnlyList<object> ResolveSectionRenderItems( ReportDefinition definition, ReportSectionDefinition section, bool designMode )
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

    private double GetSectionRenderHeight( int sectionIndex, ReportSectionDefinition section )
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

    private bool IsSectionCollapsedForRender( ReportSectionDefinition section )
    {
        return IsDesignerBandRailVisible() && !section.Suppressed && IsSectionCollapsed( section );
    }

    private bool IsSectionSelected( int sectionIndex )
    {
        return selectionManager.SelectedSectionIndex == sectionIndex
            && string.IsNullOrWhiteSpace( selectionManager.SelectedElementKey );
    }

    private async Task HandleDesignerShortcutAsync( ReportDesignerShortcut shortcut )
    {
        if ( CurrentMode != ReportStudioMode.Design || !IsDesignerEnabled || IsElementTextEditing() )
            return;

        switch ( shortcut )
        {
            case ReportDesignerShortcut.Cut:
                await ExecuteCommandIfAvailableAsync( ReportCommand.Cut );
                break;

            case ReportDesignerShortcut.Copy:
                await ExecuteCommandIfAvailableAsync( ReportCommand.Copy );
                break;

            case ReportDesignerShortcut.Paste:
                await ExecuteCommandIfAvailableAsync( ReportCommand.Paste );
                break;

            case ReportDesignerShortcut.Undo:
                await ExecuteCommandIfAvailableAsync( ReportCommand.Undo );
                break;

            case ReportDesignerShortcut.Redo:
                await ExecuteCommandIfAvailableAsync( ReportCommand.Redo );
                break;

            case ReportDesignerShortcut.Delete:
                if ( !string.IsNullOrWhiteSpace( selectionManager.SelectedElementKey ) )
                    await DeleteSelectedElementAsync();
                break;

            case ReportDesignerShortcut.EditText:
                BeginSelectedElementTextEdit();
                break;

            case ReportDesignerShortcut.MoveLeft:
                await MoveSelectedElementsAsync( -DesignerConstants.KeyboardMoveStep, 0 );
                break;

            case ReportDesignerShortcut.MoveUp:
                await MoveSelectedElementsAsync( 0, -DesignerConstants.KeyboardMoveStep );
                break;

            case ReportDesignerShortcut.MoveRight:
                await MoveSelectedElementsAsync( DesignerConstants.KeyboardMoveStep, 0 );
                break;

            case ReportDesignerShortcut.MoveDown:
                await MoveSelectedElementsAsync( 0, DesignerConstants.KeyboardMoveStep );
                break;
        }
    }

    private async Task ExecuteCommandIfAvailableAsync( ReportCommand command )
    {
        if ( CanExecuteCommand( command ) )
            await ExecuteCommandAsync( command );
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
            ReportCommand.ConnectDataSource => OpenDataSourceConnectionDialogAsync(),
            ReportCommand.DownloadPdf => DownloadPdfAsync(),
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
            ReportCommand.ConnectDataSource => CurrentMode == ReportStudioMode.Design && IsDesignerEnabled && DataSourceProviders.Count > 0,
            ReportCommand.DownloadPdf => context.ViewerOptions.AllowDownload && SupportsPreviewFormat( ReportPreviewFormat.Pdf ) && PdfGenerator is not null,
            ReportCommand.Cut or ReportCommand.Copy => CurrentMode == ReportStudioMode.Design && selectionManager.FindSelectedElement( definition ) is not null,
            ReportCommand.Delete => CurrentMode == ReportStudioMode.Design && selectionManager.CanDeleteSelection( definition ),
            ReportCommand.Paste => CurrentMode == ReportStudioMode.Design && clipboardElement is not null && definition.Sections.Count > 0,
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
        commandManager.Clear();
        await ApplyReportStateAsync( state, notifyDefinitionChanged: true );
        InvalidateDesignerCaches();
    }

    private async Task ExecuteDesignerCommandAsync( ReportDesignerCommand command )
    {
        var result = await commandManager.ExecuteAsync( command, EffectiveDefinition, CaptureReportState );

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
            _ = NotifyDefinitionChangedLaterAsync( result.Definition );
        }
    }

    private Task NotifyDefinitionChangedLaterAsync( ReportDefinition definition )
    {
        return InvokeAsync( async () =>
        {
            await Task.Yield();
            await DefinitionChanged.InvokeAsync( definition );
        } );
    }

    private async Task SetModeAsync( ReportStudioMode mode )
    {
        if ( CurrentMode == ReportStudioMode.Design && mode != ReportStudioMode.Design )
            await CaptureDesignerPaneScrollPositionsAsync();

        await ExecuteDesignerCommandAsync( new( $"Set {mode} mode", async () =>
        {
            currentMode = mode;
            designerState.EditingElementKey = null;

            if ( mode == ReportStudioMode.Design )
                designerPaneScrollRestoreVersion++;

            if ( ModeChanged.HasDelegate )
                await ModeChanged.InvokeAsync( currentMode );
        }, trackHistory: false, notifyDefinitionChanged: false ) );
    }

    private async Task SetPreviewAsync( ReportPreviewFormat format )
    {
        if ( CurrentMode == ReportStudioMode.Design )
            await CaptureDesignerPaneScrollPositionsAsync();

        await ExecuteDesignerCommandAsync( new( $"Set {format} preview", async () =>
        {
            currentPreviewFormat = format;
            currentMode = ReportStudioMode.Preview;
            designerState.EditingElementKey = null;

            await ResolveDataSourcesAsync( EffectiveDefinition, loadData: true );

            if ( ModeChanged.HasDelegate )
                await ModeChanged.InvokeAsync( currentMode );
        }, trackHistory: false, notifyDefinitionChanged: false ) );
    }

    private async Task CaptureDesignerPaneScrollPositionsAsync()
    {
        if ( designerLayoutRef is not null )
            await designerLayoutRef.CapturePaneScrollPositions( designerPaneScrollPositions );
    }

    private async Task DownloadPdfAsync()
    {
        if ( PdfGenerator is null )
            return;

        ReportDefinition definition = EffectiveDefinition;
        await ResolveDataSourcesAsync( definition, true );

        PdfDocumentDefinition pdfDocument = previewExportService.BuildPdfDocument( definition, Data );
        PdfRenderResult result = await PdfGenerator.GenerateAsync( pdfDocument, new()
        {
            FileName = previewExportService.ResolvePdfFileName( definition ),
        } );

        EnsureReportingModule();
        await reportingModule.DownloadFile( result.FileName, result.ContentType, result.Content );
    }

    private async Task ResetDefinitionAsync()
    {
        await ExecuteDesignerCommandAsync( new( "Reset report", () =>
        {
            declarativeDefinition = BuildDeclarativeDefinition();
            SelectReport();
            CloseContextMenu();
            designerState.DragPreview = null;
            designerState.EditingElementKey = null;

            return Task.CompletedTask;
        }, () => declarativeDefinition ) );
    }

    private Task CopySelectedElementAsync()
    {
        return ExecuteDesignerCommandAsync( new( "Copy element", () =>
        {
            ReportClipboardResult result = clipboardService.CopyElement( EffectiveDefinition, selectionManager.SelectedElementKey );

            if ( result.ClipboardElement is not null )
            {
                clipboardElement = result.ClipboardElement;
                clipboardSectionId = result.ClipboardSectionId;
                CloseContextMenu();
            }

            return Task.CompletedTask;
        }, trackHistory: false, notifyDefinitionChanged: false ) );
    }

    private async Task CutSelectedElementAsync()
    {
        var definition = EffectiveDefinition;

        if ( !ReportDefinitionHelper.TryFindElementLocation( definition, selectionManager.SelectedElementKey, out _, out _, out _ ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Cut element", () =>
        {
            ReportClipboardResult result = clipboardService.CutElement( EffectiveDefinition, selectionManager.SelectedElementKey );

            if ( result.Changed )
            {
                clipboardElement = result.ClipboardElement;
                clipboardSectionId = result.ClipboardSectionId;
                SelectSection( result.SelectedSectionIndex.GetValueOrDefault() );
                CloseContextMenu();
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
            ReportDefinition definition = EffectiveDefinition;
            int targetSectionIndex = ResolvePasteSectionIndex( definition );
            ReportClipboardResult result = clipboardService.PasteElement(
                definition,
                clipboardElement,
                clipboardSectionId,
                designerState.ContextMenu,
                targetSectionIndex,
                IsSnapToGridEnabled,
                ApplyDesignerGrid,
                elementLayoutService,
                tableEditor );

            if ( !string.IsNullOrWhiteSpace( result.SelectedCellKey ) )
                SelectTableCell( result.SelectedCellKey );
            else if ( !string.IsNullOrWhiteSpace( result.SelectedElementKey ) )
                SelectElement( result.SelectedElementKey );

            CloseContextMenu();

            return Task.CompletedTask;
        } ) );
    }

    private int ResolvePasteSectionIndex( ReportDefinition definition )
    {
        return clipboardService.ResolvePasteSectionIndex( definition, designerState.ContextMenu, selectionManager.ResolvePasteSectionIndex );
    }

    private async Task UndoAsync()
    {
        var state = commandManager.Undo();

        if ( state is not null )
            await ApplyReportStateAsync( state, notifyDefinitionChanged: true );
    }

    private async Task RedoAsync()
    {
        var state = commandManager.Redo();

        if ( state is not null )
            await ApplyReportStateAsync( state, notifyDefinitionChanged: true );
    }

    private ReportState CaptureReportState( ReportDefinition definition )
    {
        return stateService.Capture(
            definition,
            CurrentMode,
            CurrentPreviewFormat,
            designerState.SnapToGrid,
            selectionManager,
            clipboardElement,
            clipboardSectionId,
            commandManager.CanUndo,
            commandManager.CanRedo );
    }

    private async Task ApplyReportStateAsync( ReportState state, bool notifyDefinitionChanged )
    {
        ReportState nextState = stateService.Apply( state, designerState, selectionManager, BuildDeclarativeDefinition, out ReportDefinition definition, out ReportElementDefinition nextClipboardElement, out string nextClipboardSectionId );

        declarativeDefinition = definition;
        currentMode = nextState.Mode;
        currentPreviewFormat = nextState.PreviewFormat;
        clipboardElement = nextClipboardElement;
        clipboardSectionId = nextClipboardSectionId;
        designerState.SelectionVersion++;

        CloseContextMenu();
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
        CloseContextMenu();
        designerState.EditingElementKey = null;

        if ( selectionChanged )
        {
            designerState.SelectionVersion++;
            RefreshDesignerSurface();
        }
    }

    private void HandleElementClick( string key, MouseEventArgs eventArgs )
    {
        if ( IsSuppressingSelectionClick() )
            return;

        if ( eventArgs.Detail >= 2 )
        {
            BeginElementTextEdit( key );
            return;
        }

        if ( string.Equals( designerState.SuppressNextElementClickKey, key, StringComparison.Ordinal ) )
        {
            designerState.SuppressNextElementClickKey = null;
            return;
        }

        if ( eventArgs.CtrlKey )
        {
            ToggleElementSelection( key );
            return;
        }

        SelectElement( key, preserveSelection: selectionManager.IsElementSelected( key ) && selectionManager.SelectedElementKeys.Count > 1 );
    }

    private Task HandleElementClickAsync( string key, MouseEventArgs eventArgs )
    {
        HandleElementClick( key, eventArgs );

        return Task.CompletedTask;
    }

    private Task HandleElementDoubleClickAsync( string key, MouseEventArgs eventArgs )
    {
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
        CloseContextMenu();

        if ( selectionChanged )
        {
            designerState.SelectionVersion++;
            RefreshDesignerSurface();
        }
    }

    private void ToggleElementSelection( string key )
    {
        bool selectionChanged = selectionManager.ToggleElementSelection( key );
        CloseContextMenu();

        if ( selectionChanged )
        {
            designerState.SelectionVersion++;
            RefreshDesignerSurface();
        }
    }

    private void SelectElements( IEnumerable<string> elementKeys, string primaryElementKey = null )
    {
        bool selectionChanged = selectionManager.SelectElements( elementKeys, primaryElementKey );
        CloseContextMenu();

        if ( selectionChanged )
        {
            designerState.SelectionVersion++;
            RefreshDesignerSurface();
        }
    }

    private void SelectSection( int index )
    {
        bool selectionChanged = selectionManager.SelectSection( index );
        CloseContextMenu();

        if ( selectionChanged )
        {
            designerState.SelectionVersion++;
            RefreshDesignerSurface();
        }
    }

    private Task HandleTableCellClickAsync( string cellKey, MouseEventArgs eventArgs )
    {
        if ( IsSuppressingSelectionClick() )
            return Task.CompletedTask;

        SelectTableCell( cellKey );

        return Task.CompletedTask;
    }

    private void SelectTableCell( string cellKey )
    {
        bool selectionChanged = selectionManager.SelectCell( cellKey );
        CloseContextMenu();

        if ( selectionChanged )
        {
            designerState.SelectionVersion++;
            RefreshDesignerSurface();
        }
    }

    private void ToggleSectionCollapsed( ReportSectionDefinition section )
    {
        if ( !AllowBandCollapse || section is null )
            return;

        var sectionId = ReportDefinitionHelper.EnsureSectionId( section );

        if ( collapsedSectionIds.Contains( sectionId ) )
            collapsedSectionIds.Remove( sectionId );
        else
            collapsedSectionIds.Add( sectionId );

        collapsedSectionsVersion++;
        InvalidateDesignerLayoutCache();
        RefreshDesignerSurface();
    }

    private bool IsSectionCollapsed( ReportSectionDefinition section )
    {
        return AllowBandCollapse
            && section is not null
            && collapsedSectionIds.Contains( ReportDefinitionHelper.EnsureSectionId( section ) );
    }

    private async Task OpenSectionContextMenuAsync( int sectionIndex, MouseEventArgs eventArgs )
    {
        bool selectionChanged = selectionManager.ReportSelected
            || selectionManager.SelectedSectionIndex != sectionIndex
            || !string.IsNullOrWhiteSpace( selectionManager.SelectedElementKey )
            || !string.IsNullOrWhiteSpace( selectionManager.SelectedCellKey )
            || selectionManager.SelectedElementKeys.Count > 0;

        selectionManager.ReportSelected = false;
        selectionManager.SelectedSectionIndex = sectionIndex;
        selectionManager.SelectedElementKey = null;
        selectionManager.SelectedCellKey = null;
        selectionManager.SelectedElementKeys.Clear();

        ReportContextMenuState nextContextMenu = new()
        {
            Visible = true,
            Target = ReportContextMenuTarget.Section,
            SectionIndex = sectionIndex,
            ClientX = eventArgs.ClientX,
            ClientY = eventArgs.ClientY,
        };

        contextMenuService.PopulateSectionCapabilities( EffectiveDefinition, nextContextMenu, clipboardElement is not null, aggregateService.CanInsertSection, aggregateService.CanInsertGroup );
        await ShowContextMenuAsync( nextContextMenu, selectionChanged );
    }

    private async Task OpenSectionBodyContextMenuAsync( int sectionIndex, MouseEventArgs eventArgs )
    {
        bool selectionChanged = selectionManager.ReportSelected
            || selectionManager.SelectedSectionIndex != sectionIndex
            || !string.IsNullOrWhiteSpace( selectionManager.SelectedElementKey )
            || !string.IsNullOrWhiteSpace( selectionManager.SelectedCellKey )
            || selectionManager.SelectedElementKeys.Count > 0;

        selectionManager.ReportSelected = false;
        selectionManager.SelectedSectionIndex = sectionIndex;
        selectionManager.SelectedElementKey = null;
        selectionManager.SelectedCellKey = null;
        selectionManager.SelectedElementKeys.Clear();

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

        contextMenuService.PopulateSectionCapabilities( EffectiveDefinition, nextContextMenu, clipboardElement is not null, aggregateService.CanInsertSection, aggregateService.CanInsertGroup );
        await ShowContextMenuAsync( nextContextMenu, selectionChanged );
    }

    private async Task OpenElementContextMenuAsync( string elementKey, MouseEventArgs eventArgs )
    {
        bool selectionChanged = selectionManager.ReportSelected
            || selectionManager.SelectedSectionIndex is not null
            || !string.IsNullOrWhiteSpace( selectionManager.SelectedCellKey )
            || !selectionManager.IsElementSelected( elementKey );

        selectionManager.ReportSelected = false;
        selectionManager.SelectedSectionIndex = null;
        selectionManager.SelectedCellKey = null;

        if ( !selectionManager.IsElementSelected( elementKey ) )
            selectionManager.SelectedElementKeys.Clear();

        selectionManager.SelectedElementKey = elementKey;
        selectionManager.SelectedElementKeys.Add( elementKey );
        ReportContextMenuState nextContextMenu = new()
        {
            Visible = true,
            Target = ReportContextMenuTarget.Element,
            ElementKey = elementKey,
            SelectedElementCount = selectionManager.SelectedElementKeys.Count,
            ClientX = eventArgs.ClientX,
            ClientY = eventArgs.ClientY,
        };

        contextMenuService.PopulateElementCapabilities( EffectiveDefinition, nextContextMenu, clipboardElement is not null );
        await ShowContextMenuAsync( nextContextMenu, selectionChanged );
    }

    private async Task OpenTableCellContextMenuAsync( int sectionIndex, string cellKey, MouseEventArgs eventArgs )
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

        contextMenuService.PopulateTableCellCapabilities( EffectiveDefinition, nextContextMenu, clipboardElement is not null );
        await ShowContextMenuAsync( nextContextMenu, selectionChanged );
    }

    private async Task ShowContextMenuAsync( ReportContextMenuState state, bool refreshDesignerSelection )
    {
        designerState.ContextMenu = state;

        if ( contextMenuHost is not null )
            await contextMenuHost.ShowAsync( state );

        if ( refreshDesignerSelection )
            await InvokeAsync( StateHasChanged );
    }

    private IReadOnlyList<ReportDesignerFieldOption> GetContextElementAggregateFieldOptions( ReportDefinition definition )
    {
        if ( !IsElementContextMenuVisible()
            || !ReportDefinitionHelper.TryFindElementLocation( definition, designerState.ContextMenu.ElementKey, out var sectionIndex, out _, out var element )
            || sectionIndex < 0
            || sectionIndex >= definition.Sections.Count )
        {
            return [];
        }

        var section = definition.Sections[sectionIndex];

        if ( section.Type != ReportSectionType.Detail || element is not ReportFieldElementDefinition fieldElement )
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

    private async Task OpenContextElementFormulaDialogAsync()
    {
        if ( !TryGetContextElementFormulaFieldName( EffectiveDefinition, out string formulaFieldName ) )
            return;

        await OpenFormulaFieldDialogAsync( formulaFieldName );
        CloseContextMenu();
    }

    private async Task OpenContextElementRunningTotalDialogAsync()
    {
        if ( !TryGetContextElementRunningTotalName( EffectiveDefinition, out string runningTotalName ) )
            return;

        ReportRunningTotalDefinition runningTotal = dataDefinitionService.FindRunningTotal( EffectiveDefinition, runningTotalName );

        if ( runningTotal is not null )
            await runningTotalDialogRef.ShowAsync( runningTotal );

        CloseContextMenu();
    }

    private void BeginSelectedElementTextEdit()
    {
        if ( !string.IsNullOrWhiteSpace( selectionManager.SelectedElementKey ) )
            BeginElementTextEdit( selectionManager.SelectedElementKey );
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
        CloseContextMenu();
        RefreshDesignerSurface();
    }

    private void CancelElementTextEdit( string elementKey )
    {
        if ( string.Equals( designerState.EditingElementKey, elementKey, StringComparison.Ordinal ) )
        {
            designerState.EditingElementKey = null;
            RefreshDesignerSurface();
        }
    }

    private Task CancelElementTextEditAsync( string elementKey )
    {
        CancelElementTextEdit( elementKey );

        return Task.CompletedTask;
    }

    private async Task CommitElementTextEditAsync( string elementKey, string text )
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

        await ExecuteDesignerCommandAsync( new( "Edit text", () =>
        {
            if ( ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, elementKey, out _, out _, out var element )
                && element is ReportTextElementDefinition textElement )
            {
                textElement.Text = text;
            }

            return Task.CompletedTask;
        } ) );
    }

    private async Task OpenContextElementAggregateDialogAsync()
    {
        if ( !IsElementContextMenuVisible() || aggregateDialogRef is null )
            return;

        var elementKey = designerState.ContextMenu.ElementKey;
        var fieldOptions = GetContextElementAggregateFieldOptions( EffectiveDefinition );

        if ( fieldOptions.Count == 0 )
            return;

        var sourceSectionIndex = fieldOptions[0].SourceSectionIndex;
        var summaryLocations = aggregateService.GetSummaryLocations( EffectiveDefinition, sourceSectionIndex );

        CloseContextMenu();

        var selectedFieldName = ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, elementKey, out _, out _, out var element )
            && element is ReportFieldElementDefinition fieldElement
            ? fieldElement.Field
            : null;

        await aggregateDialogRef.ShowAsync( fieldOptions, selectedFieldName, summaryLocations );
    }

    private Task MergeSelectedTableCellRightAsync()
    {
        return MergeSelectedTableCellAsync( columnSpanDelta: 1, rowSpanDelta: 0 );
    }

    private Task MergeSelectedTableCellDownAsync()
    {
        return MergeSelectedTableCellAsync( columnSpanDelta: 0, rowSpanDelta: 1 );
    }

    private async Task MergeSelectedTableCellAsync( int columnSpanDelta, int rowSpanDelta )
    {
        await ExecuteSelectedTableCellCommandAsync(
            "Merge table cell",
            cellKey => tableCommandService.MergeCell( EffectiveDefinition, cellKey, columnSpanDelta, rowSpanDelta ) );
    }

    private async Task UnmergeSelectedTableCellAsync()
    {
        await ExecuteSelectedTableCellCommandAsync(
            "Unmerge table cell",
            cellKey => tableCommandService.UnmergeCell( EffectiveDefinition, cellKey ) );
    }

    private Task InsertSelectedTableRowAsync( bool insertBelow )
    {
        return ExecuteSelectedTableCellCommandAsync(
            insertBelow ? "Insert table row below" : "Insert table row above",
            cellKey => tableCommandService.InsertRow( EffectiveDefinition, cellKey, insertBelow ) );
    }

    private Task InsertSelectedTableColumnAsync( bool insertRight )
    {
        return ExecuteSelectedTableCellCommandAsync(
            insertRight ? "Insert table column right" : "Insert table column left",
            cellKey => tableCommandService.InsertColumn( EffectiveDefinition, cellKey, insertRight ) );
    }

    private Task InsertSelectedTableCellAsync()
    {
        return ExecuteSelectedTableCellCommandAsync(
            "Insert table cell",
            cellKey => tableCommandService.InsertCell( EffectiveDefinition, cellKey ) );
    }

    private Task DeleteSelectedTableRowAsync()
    {
        return ExecuteSelectedTableCellCommandAsync(
            "Delete table row",
            cellKey => tableCommandService.DeleteRow( EffectiveDefinition, cellKey ) );
    }

    private Task DeleteSelectedTableColumnAsync()
    {
        return ExecuteSelectedTableCellCommandAsync(
            "Delete table column",
            cellKey => tableCommandService.DeleteColumn( EffectiveDefinition, cellKey ) );
    }

    private Task DeleteSelectedTableCellAsync()
    {
        return ExecuteSelectedTableCellCommandAsync(
            "Delete table cell",
            cellKey => tableCommandService.DeleteCell( EffectiveDefinition, cellKey ) );
    }

    private async Task ExecuteSelectedTableCellCommandAsync( string commandName, Func<string, ReportTableCommandResult> execute )
    {
        string cellKey = designerState.ContextMenu?.CellKey ?? selectionManager.SelectedCellKey;

        if ( string.IsNullOrWhiteSpace( cellKey ) )
            return;

        await ExecuteDesignerCommandAsync( new( commandName, () =>
        {
            ReportTableCommandResult result = execute( cellKey );

            if ( result.Changed && !string.IsNullOrWhiteSpace( result.SelectedCellKey ) )
                SelectTableCell( result.SelectedCellKey );

            return Task.CompletedTask;
        } ) );
    }

    private async Task OnAggregateDialogConfirmedAsync( ReportAggregateDialogResult result )
    {
        if ( result is null )
            return;

        await ExecuteDesignerCommandAsync( new( $"Insert {ReportAggregateResolver.GetFunctionDisplayName( result.Function )}", () =>
        {
            var definition = EffectiveDefinition;
            var sourceSectionIndex = result.SourceSectionIndex;

            if ( sourceSectionIndex < 0 || sourceSectionIndex >= definition.Sections.Count )
                return Task.CompletedTask;

            var sourceSection = definition.Sections[sourceSectionIndex];
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

            var targetSectionIndex = result.TargetSectionIndex >= 0 && result.TargetSectionIndex < definition.Sections.Count
                ? result.TargetSectionIndex
                : aggregateService.EnsureTargetSection( definition, sourceSectionIndex );
            var targetSection = definition.Sections[targetSectionIndex];
            var aggregateElement = aggregateService.CreateAggregateElement( sourceSection, sourceElement, result.Function, targetSection, targetSection.Type == ReportSectionType.GroupFooter );

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

    private async Task OpenSelectedDetailGroupDialogAsync()
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

        CloseContextMenu();

        await groupDialogRef.ShowAsync( fieldOptions, selectedFieldName );
    }

    private async Task OnGroupDialogConfirmedAsync( string groupBy )
    {
        if ( string.IsNullOrWhiteSpace( groupBy ) || selectionManager.SelectedSectionIndex is null )
            return;

        var detailSectionIndex = selectionManager.SelectedSectionIndex.Value;

        await ExecuteDesignerCommandAsync( new( "Insert group", () =>
        {
            var definition = EffectiveDefinition;

            if ( detailSectionIndex < 0 || detailSectionIndex >= definition.Sections.Count )
                return Task.CompletedTask;

            var detailSection = definition.Sections[detailSectionIndex];

            if ( detailSection.Type != ReportSectionType.Detail || detailSection.Suppressed )
                return Task.CompletedTask;

            var groupHeader = aggregateService.CreateGroupHeaderSection( definition, groupBy );
            var groupFooter = aggregateService.CreateGroupFooterSection( definition, groupBy );

            definition.Sections.Insert( detailSectionIndex, groupHeader );
            definition.Sections.Insert( detailSectionIndex + 2, groupFooter );

            SelectSection( detailSectionIndex );
            CloseContextMenu();

            return Task.CompletedTask;
        } ) );
    }

    private async Task OpenDataSourceConnectionDialogAsync()
    {
        if ( dataSourceConnectionDialogRef is null )
            return;

        await dataSourceConnectionDialogRef.ShowAsync( EffectiveDefinition, DataSourceProviders );
    }

    private async Task OnDataSourceConnectionConfirmedAsync( ReportDataSourceDefinition dataSource )
    {
        if ( dataSource is null || string.IsNullOrWhiteSpace( dataSource.Name ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Connect data source", async () =>
        {
            await dataCommandService.ConnectDataSourceAsync( EffectiveDefinition, Data, dataSource, ResolveDataSourcesAsync );
        } ) );
    }

    private async Task OnDataSourceRefreshedAsync( string dataSourceName )
    {
        if ( string.IsNullOrWhiteSpace( dataSourceName ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Refresh data source", async () =>
        {
            await dataCommandService.RefreshDataSourceAsync( EffectiveDefinition, DataSourceProviderRegistry, dataSourceName );
        } ) );
    }

    private async Task OnDataSourceDeletedAsync( string dataSourceName )
    {
        if ( string.IsNullOrWhiteSpace( dataSourceName ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Delete data source", () =>
        {
            dataCommandService.DeleteDataSource( EffectiveDefinition, dataSourceName );
            return Task.CompletedTask;
        } ) );
    }

    private async Task OnFormulaFieldConfirmedAsync( ReportFormulaFieldDefinition formulaField )
    {
        if ( formulaField is null || string.IsNullOrWhiteSpace( formulaField.Name ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Save formula field", () =>
        {
            dataCommandService.SaveFormulaField( EffectiveDefinition, formulaField );
            return Task.CompletedTask;
        } ) );
    }

    private async Task OnFormulaFieldRenamedAsync( (string OldName, string NewName) formulaFieldRename )
    {
        if ( string.IsNullOrWhiteSpace( formulaFieldRename.OldName ) || string.IsNullOrWhiteSpace( formulaFieldRename.NewName ) )
            return;

        string oldName = formulaFieldRename.OldName.Trim();
        string newName = formulaFieldRename.NewName.Trim();

        if ( string.Equals( oldName, newName, StringComparison.OrdinalIgnoreCase ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Rename formula field", () =>
        {
            dataCommandService.RenameFormulaField( EffectiveDefinition, oldName, newName );
            return Task.CompletedTask;
        } ) );
    }

    private async Task OnFormulaFieldDeletedAsync( string formulaFieldName )
    {
        if ( string.IsNullOrWhiteSpace( formulaFieldName ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Delete formula field", () =>
        {
            dataCommandService.DeleteFormulaField( EffectiveDefinition, formulaFieldName );

            if ( string.Equals( editingFormulaFieldName, formulaFieldName, StringComparison.OrdinalIgnoreCase ) )
                editingFormulaFieldName = null;

            return Task.CompletedTask;
        } ) );
    }

    private async Task OnFormulaFieldInsertedAsync( string formulaFieldName )
    {
        if ( string.IsNullOrWhiteSpace( formulaFieldName ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Add formula field", () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            int sectionIndex = GetDataElementInsertionSectionIndex( definition );

            if ( sectionIndex >= 0 && sectionIndex < definition.Sections.Count )
            {
                double y = GetNextDataElementInsertionY( definition.Sections[sectionIndex] );
                ReportElementDefinition element = dataCommandService.CreateFormulaFieldElement( definition, sectionIndex, formulaFieldName, y );

                if ( element is not null )
                    SelectElement( ReportDefinitionHelper.EnsureElementId( element ) );
            }

            return Task.CompletedTask;
        } ) );
    }

    private async Task OnFieldInsertedAsync( (string DataSourceName, string FieldName) field )
    {
        if ( string.IsNullOrWhiteSpace( field.FieldName ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Add field", () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            int sectionIndex = GetDataElementInsertionSectionIndex( definition );

            if ( sectionIndex >= 0 && sectionIndex < definition.Sections.Count )
            {
                double y = GetNextDataElementInsertionY( definition.Sections[sectionIndex] );
                ReportElementDefinition element = dataCommandService.CreateFieldElement( definition, sectionIndex, field.DataSourceName, field.FieldName, y );

                if ( element is not null )
                    SelectElement( ReportDefinitionHelper.EnsureElementId( element ) );
            }

            return Task.CompletedTask;
        } ) );
    }

    private async Task OnRunningTotalConfirmedAsync( ReportRunningTotalDefinition runningTotal )
    {
        if ( runningTotal is null || string.IsNullOrWhiteSpace( runningTotal.Name ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Save running total", () =>
        {
            dataCommandService.SaveRunningTotal( EffectiveDefinition, runningTotal );
            return Task.CompletedTask;
        } ) );
    }

    private async Task OnRunningTotalRenamedAsync( (string OldName, string NewName) runningTotalRename )
    {
        if ( string.IsNullOrWhiteSpace( runningTotalRename.OldName ) || string.IsNullOrWhiteSpace( runningTotalRename.NewName ) )
            return;

        string oldName = runningTotalRename.OldName.Trim();
        string newName = runningTotalRename.NewName.Trim();

        if ( string.Equals( oldName, newName, StringComparison.OrdinalIgnoreCase ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Rename running total", () =>
        {
            dataCommandService.RenameRunningTotal( EffectiveDefinition, oldName, newName );
            return Task.CompletedTask;
        } ) );
    }

    private async Task OnRunningTotalDeletedAsync( string runningTotalName )
    {
        if ( string.IsNullOrWhiteSpace( runningTotalName ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Delete running total", () =>
        {
            dataCommandService.DeleteRunningTotal( EffectiveDefinition, runningTotalName );
            return Task.CompletedTask;
        } ) );
    }

    private async Task OnRunningTotalInsertedAsync( string runningTotalName )
    {
        if ( string.IsNullOrWhiteSpace( runningTotalName ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Add running total", () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            int sectionIndex = GetDataElementInsertionSectionIndex( definition );

            if ( sectionIndex >= 0 && sectionIndex < definition.Sections.Count )
            {
                double y = GetNextDataElementInsertionY( definition.Sections[sectionIndex] );
                ReportElementDefinition element = dataCommandService.CreateRunningTotalElement( definition, sectionIndex, runningTotalName, y );

                if ( element is not null )
                    SelectElement( ReportDefinitionHelper.EnsureElementId( element ) );
            }

            return Task.CompletedTask;
        } ) );
    }

    private async Task OnFormulaDialogConfirmedAsync( string formula )
    {
        if ( string.IsNullOrWhiteSpace( editingFormulaFieldName ) )
            return;

        await OnFormulaFieldConfirmedAsync( new()
        {
            Name = editingFormulaFieldName,
            Formula = formula,
        } );
    }

    private async Task OpenFormulaFieldDialogAsync( string formulaFieldName )
    {
        if ( string.IsNullOrWhiteSpace( formulaFieldName ) )
            return;

        ReportFormulaFieldDefinition formulaField = dataDefinitionService.FindFormulaField( EffectiveDefinition, formulaFieldName );

        if ( formulaField is null )
            return;

        editingFormulaFieldName = formulaField.Name;
        await formulaDialogRef.ShowAsync( formulaField.Name, formulaField.Formula );
    }

    private int GetDataElementInsertionSectionIndex( ReportDefinition definition )
    {
        return dataDefinitionService.GetInsertionSectionIndex( definition, selectionManager.SelectedSectionIndex, selectionManager.SelectedElementKey );
    }

    private double GetNextDataElementInsertionY( ReportSectionDefinition section )
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

    private void CloseContextMenu()
    {
        designerState.ContextMenu = null;

        if ( contextMenuHost is not null )
            _ = contextMenuHost.CloseAsync();
    }

    private async Task CloseContextMenuAsync()
    {
        designerState.ContextMenu = null;

        if ( contextMenuHost is not null )
            await contextMenuHost.CloseAsync();
    }

    private Task OnDesignerNodeDragEnded( ReportTreeNode node )
    {
        return ClearDesignerDragAsync();
    }

    private Task OnPageSelectionPointerCancel( PointerEventArgs eventArgs )
    {
        return CancelPageSelectionBoxAsync();
    }

    private Task OnElementPointerCancel( PointerEventArgs eventArgs )
    {
        return CancelElementPointerInteractionAsync();
    }

    private Task OnContextMenuCutElement( MouseEventArgs eventArgs )
        => CutSelectedElementAsync();

    private Task OnContextMenuCopyElement( MouseEventArgs eventArgs )
        => CopySelectedElementAsync();

    private Task OnContextMenuPasteElement( MouseEventArgs eventArgs )
        => PasteElementAsync();

    private Task OnContextMenuSelectAllSectionElements( MouseEventArgs eventArgs )
        => SelectAllContextSectionElementsAsync();

    private Task OnContextMenuShowProperties( MouseEventArgs eventArgs )
        => ShowContextPropertiesAsync();

    private Task OnContextMenuInsertSectionBefore( MouseEventArgs eventArgs )
        => InsertSectionAsync( insertAfter: false );

    private Task OnContextMenuInsertSectionAfter( MouseEventArgs eventArgs )
        => InsertSectionAsync( insertAfter: true );

    private Task OnContextMenuInsertGroup( MouseEventArgs eventArgs )
        => OpenSelectedDetailGroupDialogAsync();

    private Task OnContextMenuToggleSectionSuppression( MouseEventArgs eventArgs )
        => ToggleSelectedSectionSuppressionAsync();

    private Task OnContextMenuToggleSectionKeepTogether( MouseEventArgs eventArgs )
        => ToggleSelectedSectionKeepTogetherAsync();

    private Task OnContextMenuToggleSectionNewPageBefore( MouseEventArgs eventArgs )
        => ToggleSelectedSectionNewPageBeforeAsync();

    private Task OnContextMenuToggleSectionNewPageAfter( MouseEventArgs eventArgs )
        => ToggleSelectedSectionNewPageAfterAsync();

    private Task OnContextMenuDeleteSection( MouseEventArgs eventArgs )
        => DeleteSelectedSectionAsync();

    private Task OnContextMenuAlignTops( MouseEventArgs eventArgs )
        => AlignSelectedElementsAsync( ReportElementAlignment.Tops );

    private Task OnContextMenuAlignMiddles( MouseEventArgs eventArgs )
        => AlignSelectedElementsAsync( ReportElementAlignment.Middles );

    private Task OnContextMenuAlignBottoms( MouseEventArgs eventArgs )
        => AlignSelectedElementsAsync( ReportElementAlignment.Bottoms );

    private Task OnContextMenuAlignBaseline( MouseEventArgs eventArgs )
        => AlignSelectedElementsAsync( ReportElementAlignment.Baseline );

    private Task OnContextMenuAlignLefts( MouseEventArgs eventArgs )
        => AlignSelectedElementsAsync( ReportElementAlignment.Lefts );

    private Task OnContextMenuAlignCenters( MouseEventArgs eventArgs )
        => AlignSelectedElementsAsync( ReportElementAlignment.Centers );

    private Task OnContextMenuAlignRights( MouseEventArgs eventArgs )
        => AlignSelectedElementsAsync( ReportElementAlignment.Rights );

    private Task OnContextMenuAlignToGrid( MouseEventArgs eventArgs )
        => AlignSelectedElementsAsync( ReportElementAlignment.ToGrid );

    private Task OnContextMenuSizeSameWidth( MouseEventArgs eventArgs )
        => SizeSelectedElementsAsync( ReportElementSizeMode.SameWidth );

    private Task OnContextMenuSizeSameHeight( MouseEventArgs eventArgs )
        => SizeSelectedElementsAsync( ReportElementSizeMode.SameHeight );

    private Task OnContextMenuSizeSameSize( MouseEventArgs eventArgs )
        => SizeSelectedElementsAsync( ReportElementSizeMode.SameSize );

    private Task OnContextMenuBringToFront( MouseEventArgs eventArgs )
        => OrderSelectedElementsAsync( ReportElementOrderMode.BringToFront );

    private Task OnContextMenuSendToBack( MouseEventArgs eventArgs )
        => OrderSelectedElementsAsync( ReportElementOrderMode.SendToBack );

    private Task OnContextMenuMoveForward( MouseEventArgs eventArgs )
        => OrderSelectedElementsAsync( ReportElementOrderMode.MoveForward );

    private Task OnContextMenuMoveBackward( MouseEventArgs eventArgs )
        => OrderSelectedElementsAsync( ReportElementOrderMode.MoveBackward );

    private Task OnContextMenuInsertAggregate( MouseEventArgs eventArgs )
        => OpenContextElementAggregateDialogAsync();

    private Task OnContextMenuEditText( MouseEventArgs eventArgs )
    {
        BeginContextElementTextEdit();

        return Task.CompletedTask;
    }

    private Task OnContextMenuEditFormula( MouseEventArgs eventArgs )
        => OpenContextElementFormulaDialogAsync();

    private Task OnContextMenuEditRunningTotal( MouseEventArgs eventArgs )
        => OpenContextElementRunningTotalDialogAsync();

    private Task OnContextMenuDeleteElement( MouseEventArgs eventArgs )
        => DeleteSelectedElementAsync();

    private Task OnContextMenuToggleElementCanGrow( MouseEventArgs eventArgs )
        => ToggleSelectedElementCanGrowAsync();

    private Task OnContextMenuToggleElementSuppression( MouseEventArgs eventArgs )
        => ToggleSelectedElementSuppressionAsync();

    private Task OnContextMenuMergeCellRight( MouseEventArgs eventArgs )
        => MergeSelectedTableCellRightAsync();

    private Task OnContextMenuMergeCellDown( MouseEventArgs eventArgs )
        => MergeSelectedTableCellDownAsync();

    private Task OnContextMenuUnmergeCell( MouseEventArgs eventArgs )
        => UnmergeSelectedTableCellAsync();

    private Task OnContextMenuInsertTableRowAbove( MouseEventArgs eventArgs )
        => InsertSelectedTableRowAsync( insertBelow: false );

    private Task OnContextMenuInsertTableRowBelow( MouseEventArgs eventArgs )
        => InsertSelectedTableRowAsync( insertBelow: true );

    private Task OnContextMenuInsertTableColumnLeft( MouseEventArgs eventArgs )
        => InsertSelectedTableColumnAsync( insertRight: false );

    private Task OnContextMenuInsertTableColumnRight( MouseEventArgs eventArgs )
        => InsertSelectedTableColumnAsync( insertRight: true );

    private Task OnContextMenuInsertTableCell( MouseEventArgs eventArgs )
        => InsertSelectedTableCellAsync();

    private Task OnContextMenuDeleteTableRow( MouseEventArgs eventArgs )
        => DeleteSelectedTableRowAsync();

    private Task OnContextMenuDeleteTableColumn( MouseEventArgs eventArgs )
        => DeleteSelectedTableColumnAsync();

    private Task OnContextMenuDeleteTableCell( MouseEventArgs eventArgs )
        => DeleteSelectedTableCellAsync();

    private Task OnContextMenuClose( MouseEventArgs eventArgs )
        => CloseContextMenuAsync();

    private async Task MoveSelectedElementAsync( double x, double y, double width, double height )
    {
        ReportElementDefinition element = selectionManager.FindSelectedElement( EffectiveDefinition );

        if ( element is null )
            return;

        await ExecuteDesignerCommandAsync( new( "Move element", () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            ReportElementDefinition element = selectionManager.FindSelectedElement( definition );

            elementCommandService.MoveElement( definition, element, x, y, width, height, IsSnapToGridEnabled, ApplyDesignerGrid );

            return Task.CompletedTask;
        }, refreshSurface: false ) );
    }

    private async Task MoveSelectedElementsAsync( double x, double y )
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

        await ExecuteDesignerCommandAsync( new( commandName, () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            ReportElementCommandResult result = elementCommandService.MoveElements( definition, selectedElements, ReportDefinitionHelper.EnsureElementId( element ), x, y, useSnapToGrid, ApplyDesignerGrid );

            if ( result.SelectedElementKeys.Count > 0 )
                SelectElements( result.SelectedElementKeys, result.PrimaryElementKey );

            return Task.CompletedTask;
        }, refreshSurface: false ) );
    }

    private async Task AlignSelectedElementsAsync( ReportElementAlignment alignment )
    {
        List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( EffectiveDefinition );

        if ( selectedElements.Count < DesignerConstants.MinimumBatchElementCount )
            return;

        string commandName = $"Align {elementLayoutService.GetAlignmentDisplayName( alignment )}";

        await ExecuteDesignerCommandAsync( new( commandName, () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( definition );
            ReportElementCommandResult result = elementCommandService.AlignElements( definition, selectedElements, alignment, ApplyDesignerGrid );

            if ( result.SelectedElementKeys.Count > 0 )
                SelectElements( result.SelectedElementKeys, result.PrimaryElementKey );

            return Task.CompletedTask;
        }, refreshSurface: false ) );
    }

    private async Task SizeSelectedElementsAsync( ReportElementSizeMode sizeMode )
    {
        List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( EffectiveDefinition );

        if ( selectedElements.Count < DesignerConstants.MinimumBatchElementCount )
            return;

        string commandName = $"Size {elementLayoutService.GetSizeDisplayName( sizeMode )}";

        await ExecuteDesignerCommandAsync( new( commandName, () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( definition );
            ReportElementCommandResult result = elementCommandService.SizeElements( definition, selectedElements, sizeMode );

            if ( result.SelectedElementKeys.Count > 0 )
                SelectElements( result.SelectedElementKeys, result.PrimaryElementKey );

            return Task.CompletedTask;
        }, refreshSurface: false ) );
    }

    private async Task OrderSelectedElementsAsync( ReportElementOrderMode orderMode )
    {
        List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( EffectiveDefinition );

        if ( selectedElements.Count == 0 )
            return;

        string commandName = elementLayoutService.GetOrderDisplayName( orderMode );

        await ExecuteDesignerCommandAsync( new( commandName, () =>
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
        return elementLayoutService.GetSelectedElementContexts( definition, selectionManager.SelectedElementKeys, selectionManager.SelectedElementKey );
    }

    private async Task UpdateSelectedElementAsync( Action<ReportElementDefinition> update )
    {
        ReportElementDefinition element = selectionManager.FindSelectedElement( EffectiveDefinition );

        if ( element is null )
            return;

        await ExecuteDesignerCommandAsync( new( "Update element", () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            ReportElementDefinition element = selectionManager.FindSelectedElement( definition );

            elementCommandService.UpdateElement( definition, element, update );

            return Task.CompletedTask;
        } ) );
    }

    private async Task UpdateSelectedElementsAsync( string commandName, Action<ReportElementDefinition> update )
    {
        List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( EffectiveDefinition );

        if ( selectedElements.Count == 0 )
            return;

        await ExecuteDesignerCommandAsync( new( commandName, () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( definition );
            ReportElementCommandResult result = elementCommandService.UpdateElements( definition, selectedElements, update );

            if ( result.SelectedElementKeys.Count > 0 )
                SelectElements( result.SelectedElementKeys, result.PrimaryElementKey );

            return Task.CompletedTask;
        } ) );
    }

    private async Task UpdateSelectedSectionAsync( Action<ReportSectionDefinition> update )
    {
        var section = selectionManager.FindSelectedSection( EffectiveDefinition );

        if ( section is null )
            return;

        await ExecuteDesignerCommandAsync( new( "Update band", () =>
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

    private async Task UpdateReportPageAsync( Action<ReportPageDefinition> update )
    {
        await ExecuteDesignerCommandAsync( new( "Update page", () =>
        {
            var definition = EffectiveDefinition;

            update?.Invoke( definition.Page );
            definition.Page = ResolvePage( definition.Page );

            return Task.CompletedTask;
        } ) );
    }

    private async Task InsertSectionAsync( bool insertAfter )
    {
        var definition = EffectiveDefinition;

        if ( selectionManager.SelectedSectionIndex is not { } selectedSectionIndex
            || !aggregateService.CanInsertSection( definition, selectedSectionIndex ) )
            return;

        await ExecuteDesignerCommandAsync( new( insertAfter ? "Insert band after" : "Insert band before", () =>
        {
            var definition = EffectiveDefinition;
            var sourceSection = selectionManager.FindSelectedSection( definition );

            if ( selectionManager.SelectedSectionIndex is not { } selectedSectionIndex
                || !aggregateService.CanInsertSection( definition, selectedSectionIndex ) )
                return Task.CompletedTask;

            var insertIndex = insertAfter ? selectedSectionIndex + 1 : selectedSectionIndex;
            ReportSectionDefinition section = sectionCommandService.CreateInsertedSection( definition, sourceSection );

            definition.Sections.Insert( insertIndex, section );

            SelectSection( insertIndex );
            CloseContextMenu();

            return Task.CompletedTask;
        } ) );
    }

    private Task DeleteSelectionAsync()
    {
        if ( !string.IsNullOrWhiteSpace( selectionManager.SelectedElementKey ) )
            return DeleteSelectedElementAsync();

        return DeleteSelectedSectionAsync();
    }

    private async Task DeleteSelectedSectionAsync()
    {
        var definition = EffectiveDefinition;

        if ( selectionManager.SelectedSectionIndex is null
            || selectionManager.SelectedSectionIndex < 0
            || selectionManager.SelectedSectionIndex >= definition.Sections.Count
            || !ReportDefinitionHelper.CanDeleteSection( definition.Sections[selectionManager.SelectedSectionIndex.Value] ) )
        {
            return;
        }

        await ExecuteDesignerCommandAsync( new( "Delete band", () =>
        {
            var definition = EffectiveDefinition;

            if ( selectionManager.SelectedSectionIndex is null
                || selectionManager.SelectedSectionIndex < 0
                || selectionManager.SelectedSectionIndex >= definition.Sections.Count )
            {
                return Task.CompletedTask;
            }

            int nextSectionIndex = sectionCommandService.DeleteSection( definition, selectionManager.SelectedSectionIndex.Value, collapsedSectionIds );

            if ( definition.Sections.Count == 0 )
            {
                SelectReport();
            }
            else
            {
                SelectSection( nextSectionIndex );
            }

            CloseContextMenu();
            ClearDragState();

            return Task.CompletedTask;
        } ) );
    }

    private async Task ToggleSelectedSectionSuppressionAsync()
    {
        var section = selectionManager.FindSelectedSection( EffectiveDefinition );

        if ( section is null )
            return;

        await UpdateSelectedSectionSuppressionAsync( !section.Suppressed );
    }

    private Task ShowContextPropertiesAsync()
    {
        selectedDesignerPanelTab = ReportDesignerPanelTab.Properties;
        CloseContextMenu();

        return Task.CompletedTask;
    }

    private Task SelectAllContextSectionElementsAsync()
    {
        if ( designerState.ContextMenu?.SectionIndex is not { } sectionIndex )
            return Task.CompletedTask;

        var definition = EffectiveDefinition;

        if ( sectionIndex < 0 || sectionIndex >= definition.Sections.Count )
            return Task.CompletedTask;

        var elementKeys = definition.Sections[sectionIndex].Elements
            .Select( ReportDefinitionHelper.EnsureElementId )
            .Where( key => !string.IsNullOrWhiteSpace( key ) )
            .ToList();

        SelectElements( elementKeys );
        selectedDesignerPanelTab = ReportDesignerPanelTab.Properties;

        return Task.CompletedTask;
    }

    private async Task ToggleSelectedSectionKeepTogetherAsync()
    {
        var section = selectionManager.FindSelectedSection( EffectiveDefinition );

        if ( section is null )
            return;

        bool value = section.KeepTogether?.Value != true;
        await UpdateSelectedSectionAsync( currentSection => currentSection.KeepTogether = ReportValue.Create( value, currentSection.KeepTogether?.Formula ) );
        CloseContextMenu();
    }

    private async Task ToggleSelectedSectionNewPageBeforeAsync()
    {
        var section = selectionManager.FindSelectedSection( EffectiveDefinition );

        if ( section is null )
            return;

        bool value = section.NewPageBefore?.Value != true;
        await UpdateSelectedSectionAsync( currentSection => currentSection.NewPageBefore = ReportValue.Create( value, currentSection.NewPageBefore?.Formula ) );
        CloseContextMenu();
    }

    private async Task ToggleSelectedSectionNewPageAfterAsync()
    {
        var section = selectionManager.FindSelectedSection( EffectiveDefinition );

        if ( section is null )
            return;

        bool value = section.NewPageAfter?.Value != true;
        await UpdateSelectedSectionAsync( currentSection => currentSection.NewPageAfter = ReportValue.Create( value, currentSection.NewPageAfter?.Formula ) );
        CloseContextMenu();
    }

    private async Task UpdateSelectedSectionSuppressionAsync( bool suppressed )
    {
        await ExecuteDesignerCommandAsync( new( suppressed ? "Suppress" : "Don't suppress", () =>
        {
            var section = selectionManager.FindSelectedSection( EffectiveDefinition );

            if ( section is not null )
            {
                sectionCommandService.UpdateSectionSuppression( section, suppressed, collapsedSectionIds );
            }

            CloseContextMenu();

            return Task.CompletedTask;
        } ) );
    }

    private async Task ToggleSelectedElementCanGrowAsync()
    {
        var element = selectionManager.FindSelectedElement( EffectiveDefinition );

        if ( element is null )
            return;

        bool value = element.CanGrow?.Value != true;
        await UpdateSelectedElementsAsync( value ? "Enable can grow" : "Disable can grow", currentElement => currentElement.CanGrow = ReportValue.Create( value, currentElement.CanGrow?.Formula ) );
        CloseContextMenu();
    }

    private async Task ToggleSelectedElementSuppressionAsync()
    {
        var element = selectionManager.FindSelectedElement( EffectiveDefinition );

        if ( element is null )
            return;

        bool value = element.Suppress?.Value != true;
        await UpdateSelectedElementsAsync( value ? "Suppress elements" : "Don't suppress elements", currentElement => currentElement.Suppress = ReportValue.Create( value, currentElement.Suppress?.Formula ) );
        CloseContextMenu();
    }

    private async Task DeleteSelectedElementAsync()
    {
        ReportDefinition definition = EffectiveDefinition;

        List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( definition );

        if ( selectedElements.Count == 0 )
            return;

        await ExecuteDesignerCommandAsync( new( selectedElements.Count == 1 ? "Delete element" : "Delete elements", () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( definition );
            ReportElementCommandResult result = elementCommandService.DeleteElements( definition, selectedElements );

            if ( result.SelectedSectionIndex is int selectedSectionIndex )
                SelectSection( selectedSectionIndex );

            CloseContextMenu();
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
        ReportDesignerInteractionService.BeginToolboxElementDrag( designerState, elementType, text );
    }

    private bool IsExternalDesignerDragActive()
    {
        return ReportDesignerInteractionService.IsExternalDesignerDragActive( designerState );
    }

    private void BeginElementPointerDrag( string elementKey, PointerEventArgs eventArgs )
    {
        if ( eventArgs.CtrlKey )
        {
            ToggleElementSelection( elementKey );
            designerState.SuppressNextElementClickKey = elementKey;
            return;
        }

        if ( !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, elementKey, out var sectionIndex, out _, out var element )
            || element.Suppress?.Value == true )
            return;

        ReportDesignerInteractionService.TryBeginElementPointerDrag(
            designerState,
            elementKey,
            element,
            sectionIndex,
            eventArgs,
            IsSnapToGridEnabled( element ),
            CaptureElementPointerItems( EffectiveDefinition, elementKey ).ToList() );

        SelectElement( elementKey, preserveSelection: selectionManager.IsElementSelected( elementKey ) && selectionManager.SelectedElementKeys.Count > 1 );
    }

    private async Task BeginElementPointerResizeAsync( string elementKey, ReportElementResizeHandle handle, PointerEventArgs eventArgs )
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
            await StartDocumentElementResizeAsync( eventArgs.ClientX, eventArgs.ClientY, eventArgs.PointerId );
    }

    private Task BeginTablePointerResizeAsync( string tableKey, string cellKey, ReportTableResizeKind kind, int index, PointerEventArgs eventArgs )
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

    private async Task BeginSectionPointerResizeAsync( int sectionIndex, PointerEventArgs eventArgs )
    {
        if ( TryResolveElementResizeFromSectionResize( sectionIndex, eventArgs, out string elementKey, out ReportElementResizeHandle handle ) )
        {
            await BeginElementPointerResizeAsync( elementKey, handle, eventArgs );
            return;
        }

        var definition = EffectiveDefinition;

        if ( sectionIndex < 0 || sectionIndex >= definition.Sections.Count )
            return;

        var section = definition.Sections[sectionIndex];

        if ( section.Suppressed )
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

        await StartDocumentSectionResizeAsync( eventArgs.ClientY, eventArgs.PointerId );
        await InvokeAsync( StateHasChanged );
    }

    private bool TryResolveElementResizeFromSectionResize( int sectionIndex, PointerEventArgs eventArgs, out string elementKey, out ReportElementResizeHandle handle )
    {
        elementKey = null;
        handle = default;

        ReportDefinition definition = EffectiveDefinition;

        if ( definition is null || sectionIndex < 0 || sectionIndex >= definition.Sections.Count )
            return false;

        ReportSectionDefinition section = definition.Sections[sectionIndex];
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
        if ( !string.IsNullOrWhiteSpace( selectionManager.SelectedElementKey ) )
            yield return selectionManager.SelectedElementKey;

        foreach ( string selectedElementKey in selectionManager.SelectedElementKeys )
        {
            if ( !string.Equals( selectedElementKey, selectionManager.SelectedElementKey, StringComparison.Ordinal ) )
                yield return selectedElementKey;
        }
    }

    private Task PreviewElementPointerInteractionAsync( int targetSectionIndex, PointerEventArgs eventArgs )
    {
        if ( designerState.SelectionBox is not null )
            return PreviewSelectionBoxAsync( eventArgs );

        if ( designerState.SectionPointerResize is not null )
            return PreviewSectionPointerResizeAsync( eventArgs );

        if ( designerState.TablePointerResize is not null )
            return PreviewTablePointerResizeAsync( eventArgs );

        if ( designerState.ElementPointerResize is not null )
            return PreviewElementPointerResizeAsync( eventArgs );

        return PreviewElementPointerDragAsync( targetSectionIndex, eventArgs );
    }

    private Task CompleteElementPointerInteractionAsync( int targetSectionIndex, PointerEventArgs eventArgs )
    {
        if ( designerState.SelectionBox is not null )
            return CompleteSelectionBoxAsync( eventArgs );

        if ( designerState.SectionPointerResize is not null )
            return CompleteSectionPointerResizeAsync( eventArgs );

        if ( designerState.TablePointerResize is not null )
            return CompleteTablePointerResizeAsync( eventArgs );

        if ( designerState.ElementPointerResize is not null )
            return CompleteElementPointerResizeAsync( eventArgs );

        return CompleteElementPointerDragAsync( targetSectionIndex, eventArgs );
    }

    private Task CancelElementPointerInteractionAsync()
    {
        if ( designerState.SelectionBox is not null )
            return CancelSelectionBoxAsync();

        if ( designerState.SectionPointerResize is not null )
            return CancelSectionPointerResizeAsync();

        if ( designerState.TablePointerResize is not null )
            return CancelTablePointerResizeAsync();

        if ( designerState.ElementPointerResize is not null )
            return CancelElementPointerResizeAsync();

        return CancelElementPointerDragAsync();
    }

    private void BeginSelectionBox( int sectionIndex, PointerEventArgs eventArgs )
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
            CloseContextMenu();
        }
    }

    private async Task PreviewSelectionBoxAsync( PointerEventArgs eventArgs )
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

        await UpdateDesignerSelectionOverlayAsync();
    }

    private Task PreviewPageSelectionBoxAsync( PointerEventArgs eventArgs )
    {
        return designerState.SelectionBox is null
            ? Task.CompletedTask
            : PreviewSelectionBoxAsync( eventArgs );
    }

    private async Task CompleteSelectionBoxAsync( PointerEventArgs eventArgs )
    {
        if ( designerState.SelectionBox is null )
            return;

        ReportDesignerInteractionService.UpdateSelectionBox( designerState, EffectiveDefinition, eventArgs, GetDesignerContentHeight( EffectiveDefinition ) );

        ReportDesignerSelectionBox completedSelectionBox = ReportDesignerInteractionService.CompleteSelectionBox( designerState );
        await ClearDesignerInteractionOverlaysAsync();

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

    private Task CompletePageSelectionBoxAsync( PointerEventArgs eventArgs )
    {
        return designerState.SelectionBox is null
            ? Task.CompletedTask
            : CompleteSelectionBoxAsync( eventArgs );
    }

    private async Task CancelSelectionBoxAsync()
    {
        ReportDesignerInteractionService.CompleteSelectionBox( designerState );
        await ClearDesignerInteractionOverlaysAsync();

        await InvokeAsync( StateHasChanged );
    }

    private Task CancelPageSelectionBoxAsync()
    {
        return designerState.SelectionBox is null
            ? Task.CompletedTask
            : CancelSelectionBoxAsync();
    }

    private async Task PreviewElementPointerDragAsync( int targetSectionIndex, PointerEventArgs eventArgs )
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

        await UpdateDesignerDragOverlayAsync( preview );
    }

    private async Task CompleteElementPointerDragAsync( int targetSectionIndex, PointerEventArgs eventArgs )
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
            && pointerDrag.SourceSectionIndex < definition.Sections.Count
            && pointerDrag.TargetSectionIndex >= 0
            && pointerDrag.TargetSectionIndex < definition.Sections.Count
            && ReportDefinitionHelper.TryFindElementLocation( definition, pointerDrag.ElementKey, out _, out _, out _ );

        if ( !moved || !canMove )
        {
            ClearDragState();
            await ClearDesignerInteractionOverlaysAsync();
            await InvokeAsync( StateHasChanged );
            return;
        }

        await ClearDesignerInteractionOverlaysAsync();

        bool moveToTableCell = TryFindElementPointerDragTableCellTarget( definition, pointerDrag, out _ );

        await ExecuteDesignerCommandAsync( new( moveToTableCell ? "Move element to table cell" : "Move element", () =>
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
             || pointerDrag.TargetSectionIndex >= definition.Sections.Count )
        {
            return false;
        }

        double pointerX = pointerDrag.TargetX + pointerDrag.PointerOffsetX;
        double pointerY = pointerDrag.TargetY + pointerDrag.PointerOffsetY;

        if ( !tableEditor.TryFindCellAt( definition.Sections[pointerDrag.TargetSectionIndex], pointerX, pointerY, out target ) )
            return false;

        return !ReportDefinitionHelper.TryFindElementLocation( definition, pointerDrag.ElementKey, out ReportElementLocation location )
            || !ReferenceEquals( target.Table, location.Element );
    }

    private async Task CancelElementPointerDragAsync()
    {
        if ( designerState.ElementPointerDrag is null )
            return;

        ClearDragState();
        await ClearDesignerInteractionOverlaysAsync();

        await InvokeAsync( StateHasChanged );
    }

    private async Task PreviewTablePointerResizeAsync( PointerEventArgs eventArgs )
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

        await UpdateDesignerDragOverlayAsync( preview );
    }

    private async Task CompleteTablePointerResizeAsync( PointerEventArgs eventArgs )
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
            await ClearDesignerInteractionOverlaysAsync();
            await InvokeAsync( StateHasChanged );
            return;
        }

        await ClearDesignerInteractionOverlaysAsync();

        await ExecuteDesignerCommandAsync( new( pointerResize.Kind == ReportTableResizeKind.Column ? "Resize table column" : "Resize table row", () =>
        {
            if ( ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, pointerResize.TableKey, out int sectionIndex, out _, out ReportElementDefinition element )
                && element is ReportTableElementDefinition table )
            {
                ApplyTablePointerResize( table, pointerResize );
                ReportLayoutGeometry.GrowSectionToFitElement( EffectiveDefinition.Sections[sectionIndex], table );

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

    private async Task CancelTablePointerResizeAsync()
    {
        if ( designerState.TablePointerResize is null )
            return;

        ClearDragState();
        await ClearDesignerInteractionOverlaysAsync();

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

    private async Task PreviewElementPointerResizeAsync( PointerEventArgs eventArgs )
    {
        await PreviewElementPointerResizeAsync( eventArgs.ClientX, eventArgs.ClientY );
    }

    private async Task PreviewElementPointerResizeAsync( double clientX, double clientY )
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

        await UpdateDesignerDragOverlayAsync( preview );
    }

    private async Task CompleteElementPointerResizeAsync( PointerEventArgs eventArgs )
    {
        await CompleteElementPointerResizeAsync( eventArgs.ClientX, eventArgs.ClientY );
    }

    private async Task CompleteElementPointerResizeAsync( double clientX, double clientY )
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
            await ClearDesignerInteractionOverlaysAsync();
            await InvokeAsync( StateHasChanged );
            return;
        }

        await ClearDesignerInteractionOverlaysAsync();

        await ExecuteDesignerCommandAsync( new( "Resize element", () =>
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

    private async Task CancelElementPointerResizeAsync()
    {
        if ( designerState.ElementPointerResize is null )
            return;

        ClearDragState();
        await ClearDesignerInteractionOverlaysAsync();

        await InvokeAsync( StateHasChanged );
    }

    private async Task PreviewSectionPointerResizeAsync( PointerEventArgs eventArgs )
    {
        await PreviewSectionPointerResizeAsync( eventArgs.ClientY );
    }

    private async Task PreviewSectionPointerResizeAsync( double clientY )
    {
        if ( designerState.SectionPointerResize is null )
            return;

        var height = CreateSectionPointerResizeHeight( clientY );

        if ( Math.Abs( designerState.SectionPointerResize.TargetHeight - height ) < DesignerConstants.DragPreviewChangeTolerance )
            return;

        designerState.SectionPointerResize.TargetHeight = height;

        await UpdateDesignerSectionResizePreviewAsync( designerState.SectionPointerResize );
    }

    private async Task CompleteSectionPointerResizeAsync( PointerEventArgs eventArgs )
    {
        await CompleteSectionPointerResizeAsync( eventArgs.ClientY );
    }

    private async Task CompleteSectionPointerResizeAsync( double clientY )
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
            }, refreshSurface: false ) );
        }
        finally
        {
            await CommitDesignerSectionResizePreviewAsync();
            await InvokeAsync( StateHasChanged );
        }
    }

    private async Task CancelSectionPointerResizeAsync()
    {
        if ( designerState.SectionPointerResize is null )
            return;

        designerState.SectionPointerResize = null;
        await ClearDesignerSectionResizePreviewAsync();

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
        var section = pointerResize.SectionIndex >= 0 && pointerResize.SectionIndex < EffectiveDefinition.Sections.Count
            ? EffectiveDefinition.Sections[pointerResize.SectionIndex]
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

    /// <summary>
    /// Previews a document-level element resize while the pointer is moving.
    /// </summary>
    /// <param name="clientX">Current document pointer X coordinate.</param>
    /// <param name="clientY">Current document pointer Y coordinate.</param>
    [JSInvokable]
    public Task OnDocumentElementResizeMove( double clientX, double clientY )
    {
        return InvokeAsync( () => PreviewElementPointerResizeAsync( clientX, clientY ) );
    }

    /// <summary>
    /// Completes a document-level element resize and commits the final element size.
    /// </summary>
    /// <param name="clientX">Final document pointer X coordinate.</param>
    /// <param name="clientY">Final document pointer Y coordinate.</param>
    [JSInvokable]
    public Task OnDocumentElementResizeEnd( double clientX, double clientY )
    {
        return InvokeAsync( () => CompleteElementPointerResizeAsync( clientX, clientY ) );
    }

    /// <summary>
    /// Cancels the active document-level element resize.
    /// </summary>
    [JSInvokable]
    public Task OnDocumentElementResizeCancel()
    {
        return InvokeAsync( CancelElementPointerResizeAsync );
    }

    private async Task StartDocumentSectionResizeAsync( double startClientY, long pointerId )
    {
        EnsureReportingModule();
        dotNetObjectReference ??= DotNetObjectReference.Create( this );

        await DocumentObserver.EnsureInitializedAsync();
        await reportingModule.StartSectionResize( dotNetObjectReference, startClientY, pointerId );
    }

    private async Task StartDocumentElementResizeAsync( double startClientX, double startClientY, long pointerId )
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

    private async Task UpdateDesignerSelectionOverlayAsync()
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

    private async Task UpdateDesignerDragOverlayAsync( ReportDesignerDragPreview preview )
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

    private async Task ClearDesignerInteractionOverlaysAsync()
    {
        if ( reportingModule is null )
            return;

        await reportingModule.ClearDesignerInteractionOverlays( designerPageRef.Element );
    }

    private async Task UpdateDesignerSectionResizePreviewAsync( ReportSectionPointerResizeState pointerResize )
    {
        if ( pointerResize is null || EffectiveDefinition is null || pointerResize.SectionIndex < 0 || pointerResize.SectionIndex >= EffectiveDefinition.Sections.Count )
            return;

        EnsureReportingModule();

        string sectionId = ReportDefinitionHelper.EnsureSectionId( EffectiveDefinition.Sections[pointerResize.SectionIndex] );

        await reportingModule.UpdateDesignerSectionResizePreview(
            designerPageRef.Element,
            sectionId,
            ReportMeasurementConverter.ToCssPixelValue( pointerResize.TargetHeight ) );
    }

    private async Task ClearDesignerSectionResizePreviewAsync()
    {
        if ( reportingModule is null )
            return;

        await reportingModule.ClearDesignerSectionResizePreview( designerPageRef.Element );
    }

    private async Task CommitDesignerSectionResizePreviewAsync()
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

    private async Task PreviewDesignerDragAsync( int targetSectionIndex, ElementReference sectionBodyElement, DragEventArgs eventArgs )
    {
        if ( designerState.DraggedKind == ReportDesignerDragKind.None )
            return;

        var offset = await GetDesignerDragOffsetAsync( sectionBodyElement, eventArgs );
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

        await UpdateDesignerDragOverlayAsync( preview );
    }

    private async Task<(double X, double Y)> GetDesignerDragOffsetAsync( ElementReference sectionBodyElement, DragEventArgs eventArgs )
    {
        EnsureReportingModule();

        var offset = await reportingModule.GetElementOffset( sectionBodyElement, eventArgs.ClientX, eventArgs.ClientY );

        return offset is { Length: >= 2 }
            ? (Math.Max( 0, ReportMeasurementConverter.FromCssPixelValue( offset[0] ) ), Math.Max( 0, ReportMeasurementConverter.FromCssPixelValue( offset[1] ) ))
            : (Math.Max( 0, ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetX ) ), Math.Max( 0, ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetY ) ));
    }

    private async Task DropDesignerItemAsync( int targetSectionIndex, ElementReference sectionBodyElement, DragEventArgs eventArgs )
    {
        var definition = EffectiveDefinition;

        if ( targetSectionIndex < 0 || targetSectionIndex >= definition.Sections.Count )
            return;

        var offset = await GetDesignerDragOffsetAsync( sectionBodyElement, eventArgs );
        bool useSnapToGrid = designerState.DraggedKind == ReportDesignerDragKind.Element
            ? IsSnapToGridEnabled( designerState.DraggedElement )
            : designerState.SnapToGrid;
        var x = ApplyDesignerGrid( offset.X, useSnapToGrid );
        var y = ApplyDesignerGrid( offset.Y, useSnapToGrid );
        var tableDropTarget = tableEditor.TryFindCellAt( definition.Sections[targetSectionIndex], x, y, out ReportTableCellDropTarget cellDropTarget )
            ? cellDropTarget
            : null;
        var fieldDropTarget = designerState.DraggedKind == ReportDesignerDragKind.Field
            ? dragDropService.FindTextElementAt( definition.Sections[targetSectionIndex], x, y )
            : null;

        var commandName = dragDropService.ResolveCommandName( definition, designerState, tableDropTarget, fieldDropTarget );

        if ( commandName is null )
            return;

        await ClearDesignerInteractionOverlaysAsync();

        await ExecuteDesignerCommandAsync( new( commandName, () =>
        {
            var definition = EffectiveDefinition;
            var targetSection = definition.Sections[targetSectionIndex];
            tableEditor.TryFindCellAt( targetSection, x, y, out ReportTableCellDropTarget tableCellDropTarget );
            ReportDropResult result = dragDropService.Drop( definition, designerState, targetSectionIndex, x, y, tableCellDropTarget, tableEditor );

            if ( !string.IsNullOrWhiteSpace( result.SelectedCellKey ) )
                SelectTableCell( result.SelectedCellKey );
            else if ( !string.IsNullOrWhiteSpace( result.SelectedElementKey ) )
                SelectElement( result.SelectedElementKey );

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

    private async Task ClearDesignerDragAsync()
    {
        var requiresRender = designerState.DraggedKind != ReportDesignerDragKind.None || designerState.DragPreview is not null;

        ClearDragState();
        await ClearDesignerInteractionOverlaysAsync();

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
            GetDesignerSectionBodyTopOffset() );
    }

    private double GetElementPageY( ReportDefinition definition, int sectionIndex, double elementY )
    {
        return GetSectionOffsetY( definition, sectionIndex ) + GetDesignerSectionBodyTopOffset() + elementY;
    }

    private double GetDesignerSectionHeight( int sectionIndex, ReportSectionDefinition section )
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

    private static double GetMinimumSectionHeight( ReportSectionDefinition section )
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

    private bool ShouldRenderElement( ReportDefinition definition, ReportSectionDefinition section, ReportElementDefinition element, object item )
    {
        return !ReportValueResolver.ResolveSuppress( element, section, definition, Data, item );
    }

    #endregion

    #region Properties

    private ReportDefinition EffectiveDefinition
        => CurrentDefinitionMode == ReportDefinitionMode.AlwaysUseDeclarative
            ? BuildDeclarativeDefinition()
            : Definition ?? declarativeDefinition ?? BuildDeclarativeDefinition();

    private bool IsDesignerEnabled => DesignerEnabled || GlobalOptions.DesignerEnabled;

    private ReportOptions GlobalOptions => globalOptions ??= ServiceProvider.GetService<ReportOptions>() ?? new();

    private IReportDataSourceProviderRegistry DataSourceProviderRegistry
        => dataSourceProviderRegistry ??= ServiceProvider.GetService<IReportDataSourceProviderRegistry>();

    private IReadOnlyList<IReportDataSourceProvider> DataSourceProviders
        => DataSourceProviderRegistry?.Providers?.Count > 0
            ? DataSourceProviderRegistry.Providers
            : fallbackDataSourceProviders;

    private ReportStudioMode CurrentMode => Mode ?? currentMode;

    private ReportPreviewFormat CurrentPreviewFormat => PreviewFormat ?? currentPreviewFormat;

    private ReportDefinitionMode CurrentDefinitionMode => DefinitionMode ?? GlobalOptions.DefinitionMode;

    private string SelectedDesignerPanelTabName => selectedDesignerPanelTab.ToString();

    private string ToolbarStateKey => $"{CurrentMode}|{CurrentPreviewFormat}|{selectionManager.SelectedElementKey}|{selectionManager.SelectedCellKey}|{selectionManager.SelectedElementKeys.Count}|{selectionManager.SelectedSectionIndex}|{clipboardElement?.Id}|{commandManager.CanUndo}|{commandManager.CanRedo}";

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
}