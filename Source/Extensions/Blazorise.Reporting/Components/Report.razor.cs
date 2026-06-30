#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Pdf;
using Blazorise.Reporting.Internal;
using Blazorise.Utilities;
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

    private readonly HashSet<string> collapsedSectionIds = new( StringComparer.Ordinal );

    private readonly Dictionary<string, IReadOnlyList<object>> designerSectionRenderItems = new( StringComparer.Ordinal );

    private readonly IReadOnlyList<IReportDataSourceProvider> fallbackDataSourceProviders = [new ObjectReportDataSourceProvider()];

    private DotNetObjectReference<Report> dotNetObjectReference;

    private ReportDefinition declarativeDefinition;

    private ReportStudioMode currentMode;

    private ReportPreviewFormat currentPreviewFormat;

    private ReportDesignerPanelTab selectedDesignerPanelTab = ReportDesignerPanelTab.Properties;

    private ReportContextMenuState contextMenu;

    private _ReportDesignerContextMenuHost contextMenuHost;

    private string editingElementKey;

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

    private DateTime lastSelectionBoxRenderTime;

    private int designerSelectionVersion;

    private _ReportDesignerLayout designerLayoutRef;

    private _ReportDesignerPage designerPageRef;

    private ( ReportDefinition Definition, object Data ) observedParameters;

    private (
        ReportDefinition Definition,
        ReportBandMode BandMode,
        int CollapsedSectionsVersion,
        int SectionCount,
        int ResizeSectionIndex,
        double ResizeHeight,
        double[] SectionOffsets,
        double ContentHeight ) designerLayoutCache;

    private ( ReportDefinition Definition, object Data ) designerSectionRenderItemsCacheKey;

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
        if ( firstRender && Definition is null && DefinitionMode != ReportDefinitionMode.UseDefinitionOnly )
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
        return contextMenu?.Visible == true
            && contextMenu.Target == ReportContextMenuTarget.Element;
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
        if ( designMode )
            return ResolveDesignerSectionRenderItems( definition, section );

        var items = !designMode && section.Type == ReportSectionType.Detail
            ? ReportDataResolver.ResolveItems( definition, Data, section.DataSource ).ToList()
            : new List<object> { ReportDataResolver.ResolveItems( definition, Data, section.DataSource ).FirstOrDefault() };

        if ( items.Count == 0 )
            items.Add( null );

        return items;
    }

    private IReadOnlyList<object> ResolveDesignerSectionRenderItems( ReportDefinition definition, ReportSectionDefinition section )
    {
        if ( definition is null || section is null )
            return [null];

        if ( !ReferenceEquals( designerSectionRenderItemsCacheKey.Definition, definition ) || !ReferenceEquals( designerSectionRenderItemsCacheKey.Data, Data ) )
        {
            designerSectionRenderItems.Clear();
            designerSectionRenderItemsCacheKey = ( definition, Data );
        }

        string sectionId = ReportDefinitionHelper.EnsureSectionId( section );

        if ( designerSectionRenderItems.TryGetValue( sectionId, out IReadOnlyList<object> cachedItems ) )
            return cachedItems;

        object item = ReportDataResolver.ResolveItems( definition, Data, section.DataSource ).FirstOrDefault();
        IReadOnlyList<object> items = [item];
        designerSectionRenderItems[sectionId] = items;

        return items;
    }

    private double GetReportPageWidth( ReportDefinition definition )
    {
        definition.Page = ResolvePage( definition.Page );

        return definition.Page.Width;
    }

    private double GetReportPageContentWidth( ReportDefinition definition )
    {
        definition.Page = ResolvePage( definition.Page );

        return ReportPageDefinitionHelper.GetContentWidth( definition.Page );
    }

    private string GetPreviewPageContentStyle( ReportDefinition definition )
    {
        definition.Page = ResolvePage( definition.Page );

        var styleBuilder = new StyleBuilder( builder =>
        {
            builder.Append( $"left:{ReportMeasurementConverter.ToCssPixelString( definition.Page.Margins.Left )}" );
            builder.Append( $"top:{ReportMeasurementConverter.ToCssPixelString( definition.Page.Margins.Top )}" );
            builder.Append( $"right:{ReportMeasurementConverter.ToCssPixelString( definition.Page.Margins.Right )}" );
            builder.Append( $"bottom:{ReportMeasurementConverter.ToCssPixelString( definition.Page.Margins.Bottom )}" );
        } );

        return styleBuilder.Styles;
    }

    private string GetPreviewPageFooterStyle( ReportDefinition definition, ReportRenderPage renderPage )
    {
        definition.Page = ResolvePage( definition.Page );

        var footerHeight = renderPage.FooterSections.Sum( renderSection => GetDesignerSectionHeight( renderSection.SectionIndex, renderSection.Section ) );
        var styleBuilder = new StyleBuilder( builder =>
        {
            builder.Append( $"left:{ReportMeasurementConverter.ToCssPixelString( definition.Page.Margins.Left )}" );
            builder.Append( $"right:{ReportMeasurementConverter.ToCssPixelString( definition.Page.Margins.Right )}" );
            builder.Append( $"bottom:{ReportMeasurementConverter.ToCssPixelString( definition.Page.Margins.Bottom )}" );
            builder.Append( $"height:{ReportMeasurementConverter.ToCssPixelString( footerHeight )}" );
        } );

        return styleBuilder.Styles;
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
        return IsDesignerBandRailVisible() ? DesignerConstants.DesignerBandRailWidth : 0;
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
        await ExecuteDesignerCommandAsync( new( $"Set {mode} mode", async () =>
        {
            currentMode = mode;
            editingElementKey = null;

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
            editingElementKey = null;

            await ResolveDataSourcesAsync( EffectiveDefinition, loadData: true );

            if ( ModeChanged.HasDelegate )
                await ModeChanged.InvokeAsync( currentMode );
        }, trackHistory: false, notifyDefinitionChanged: false ) );
    }

    private async Task DownloadPdfAsync()
    {
        if ( PdfGenerator is null )
            return;

        ReportDefinition definition = EffectiveDefinition;
        await ResolveDataSourcesAsync( definition, true );

        PdfDocumentDefinition pdfDocument = ReportPdfDocumentBuilder.Build( definition, Data );
        PdfRenderResult result = await PdfGenerator.GenerateAsync( pdfDocument, new()
        {
            FileName = ResolvePdfFileName( definition ),
        } );

        EnsureReportingModule();
        await reportingModule.DownloadFile( result.FileName, result.ContentType, result.Content );
    }

    private static string ResolvePdfFileName( ReportDefinition definition )
    {
        string name = string.IsNullOrWhiteSpace( definition?.Name ) ? "report" : definition.Name.Trim();

        foreach ( char invalidCharacter in System.IO.Path.GetInvalidFileNameChars() )
        {
            name = name.Replace( invalidCharacter, '-' );
        }

        return name.EndsWith( ".pdf", StringComparison.OrdinalIgnoreCase ) ? name : $"{name}.pdf";
    }

    private async Task ResetDefinitionAsync()
    {
        await ExecuteDesignerCommandAsync( new( "Reset report", () =>
        {
            declarativeDefinition = BuildDeclarativeDefinition();
            SelectReport();
            CloseContextMenu();
            dragPreview = null;
            editingElementKey = null;

            return Task.CompletedTask;
        }, () => declarativeDefinition ) );
    }

    private Task CopySelectedElementAsync()
    {
        return ExecuteDesignerCommandAsync( new( "Copy element", () =>
        {
            ReportDefinition definition = EffectiveDefinition;

            if ( ReportDefinitionHelper.TryFindElementLocation( definition, selectionManager.SelectedElementKey, out var sectionIndex, out _, out var element ) )
            {
                clipboardElement = ReportContext.CloneElement( element );
                clipboardSectionId = ReportDefinitionHelper.EnsureSectionId( definition.Sections[sectionIndex] );
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
            var definition = EffectiveDefinition;

            if ( ReportDefinitionHelper.TryFindElementLocation( definition, selectionManager.SelectedElementKey, out ReportElementLocation location ) )
            {
                var sectionIndex = location.SectionIndex;
                var element = location.Element;

                clipboardElement = ReportContext.CloneElement( element );
                clipboardSectionId = ReportDefinitionHelper.EnsureSectionId( definition.Sections[sectionIndex] );
                location.OwnerElements.RemoveAt( location.ElementIndex );
                SelectSection( sectionIndex );
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
            ReportSectionDefinition targetSection = definition.Sections[targetSectionIndex];
            bool sameSection = clipboardSectionId == ReportDefinitionHelper.EnsureSectionId( targetSection );
            bool pasteIntoCell = TryResolveContextPasteCell( definition, out ReportElementDefinition pasteTable, out ReportTableCellDefinition pasteCell );

            ReportElementDefinition element = ReportContext.CloneElement( clipboardElement );
            element.Id = ReportDefinitionHelper.CreateDefinitionId();
            bool useSnapToGrid = IsSnapToGridEnabled( element );

            if ( TryResolveContextPastePosition( definition, element, out double pasteX, out double pasteY ) )
            {
                element.X = ClampElementX( definition, element, ApplyDesignerGrid( pasteX, useSnapToGrid ) );
                element.Y = ApplyDesignerGrid( pasteY, useSnapToGrid );
            }
            else
            {
                element.X = sameSection ? ApplyDesignerGrid( element.X + DesignerConstants.PasteElementOffset, useSnapToGrid ) : 0;
                element.Y = sameSection ? ApplyDesignerGrid( element.Y + DesignerConstants.PasteElementOffset, useSnapToGrid ) : 0;
            }

            if ( pasteIntoCell )
            {
                ReplaceTableCellElement( pasteTable, pasteCell, element );
                SelectTableCell( pasteCell.Id );
            }
            else
            {
                targetSection.Elements.Add( element );
                ReportLayoutGeometry.GrowSectionToFitElement( targetSection, element );
                SelectElement( ReportDefinitionHelper.EnsureElementId( element ) );
            }

            CloseContextMenu();

            return Task.CompletedTask;
        } ) );
    }

    private int ResolvePasteSectionIndex( ReportDefinition definition )
    {
        if ( contextMenu?.Target == ReportContextMenuTarget.Section
            && contextMenu.HasPastePosition
            && contextMenu.SectionIndex >= 0
            && contextMenu.SectionIndex < definition.Sections.Count )
        {
            return contextMenu.SectionIndex;
        }

        if ( contextMenu?.Target == ReportContextMenuTarget.Cell
            && contextMenu.SectionIndex >= 0
            && contextMenu.SectionIndex < definition.Sections.Count )
        {
            return contextMenu.SectionIndex;
        }

        return selectionManager.ResolvePasteSectionIndex( definition );
    }

    private bool TryResolveContextPastePosition( ReportDefinition definition, ReportElementDefinition element, out double x, out double y )
    {
        x = 0;
        y = 0;

        if ( contextMenu?.Target is not ( ReportContextMenuTarget.Section or ReportContextMenuTarget.Cell ) || !contextMenu.HasPastePosition )
            return false;

        x = contextMenu.PasteX;
        y = Math.Max( 0, contextMenu.PasteY );

        return true;
    }

    private bool TryResolveContextPasteCell( ReportDefinition definition, out ReportElementDefinition table, out ReportTableCellDefinition cell )
    {
        table = null;
        cell = null;

        return contextMenu?.Target == ReportContextMenuTarget.Cell
            && ReportDefinitionHelper.TryFindTableCellLocation( definition, contextMenu.CellKey, out _, out _, out table, out cell );
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
        definition = ReportDefinitionHelper.EnsureDefinitionIds( definition );

        return new()
        {
            Definition = ReportContext.CloneDefinition( definition ),
            Mode = CurrentMode,
            PreviewFormat = CurrentPreviewFormat,
            SnapToGrid = snapToGrid,
            Selection = selectionManager.CaptureState( definition ),
            ClipboardElement = ReportContext.CloneElement( clipboardElement ),
            ClipboardSectionId = clipboardSectionId,
            CanUndo = commandManager.CanUndo,
            CanRedo = commandManager.CanRedo,
        };
    }

    private async Task ApplyReportStateAsync( ReportState state, bool notifyDefinitionChanged )
    {
        var nextState = ReportContext.CloneState( state );
        var definition = ReportDefinitionHelper.EnsureDefinitionIds( nextState.Definition ?? BuildDeclarativeDefinition() );

        declarativeDefinition = definition;
        currentMode = nextState.Mode;
        currentPreviewFormat = nextState.PreviewFormat;
        snapToGrid = nextState.SnapToGrid;
        clipboardElement = ReportContext.CloneElement( nextState.ClipboardElement );
        clipboardSectionId = nextState.ClipboardSectionId;

        selectionManager.ApplyState( definition, nextState.Selection );
        designerSelectionVersion++;

        CloseContextMenu();
        dragPreview = null;
        editingElementKey = null;
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
        editingElementKey = null;

        if ( selectionChanged )
        {
            designerSelectionVersion++;
            RefreshDesignerSurface();
        }
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

    private void SuppressNextSelectionClick()
    {
        suppressNextSectionClick = true;
        suppressSelectionClickUntil = DateTime.UtcNow.AddMilliseconds( DesignerConstants.SuppressSelectionClickMilliseconds );
    }

    private void SelectElement( string key, bool preserveSelection = false )
    {
        bool selectionChanged = selectionManager.SelectElement( key, preserveSelection );
        CloseContextMenu();

        if ( selectionChanged )
        {
            designerSelectionVersion++;
            RefreshDesignerSurface();
        }
    }

    private void ToggleElementSelection( string key )
    {
        bool selectionChanged = selectionManager.ToggleElementSelection( key );
        CloseContextMenu();

        if ( selectionChanged )
        {
            designerSelectionVersion++;
            RefreshDesignerSurface();
        }
    }

    private void SelectElements( IEnumerable<string> elementKeys, string primaryElementKey = null )
    {
        bool selectionChanged = selectionManager.SelectElements( elementKeys, primaryElementKey );
        CloseContextMenu();

        if ( selectionChanged )
        {
            designerSelectionVersion++;
            RefreshDesignerSurface();
        }
    }

    private void SelectSection( int index )
    {
        bool selectionChanged = selectionManager.SelectSection( index );
        CloseContextMenu();

        if ( selectionChanged )
        {
            designerSelectionVersion++;
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
            designerSelectionVersion++;
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

        PopulateSectionContextMenuCapabilities( EffectiveDefinition, nextContextMenu );
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

        PopulateSectionContextMenuCapabilities( EffectiveDefinition, nextContextMenu );
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

        PopulateElementContextMenuCapabilities( EffectiveDefinition, nextContextMenu );
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

        PopulateTableCellContextMenuCapabilities( EffectiveDefinition, nextContextMenu );
        await ShowContextMenuAsync( nextContextMenu, selectionChanged );
    }

    private async Task ShowContextMenuAsync( ReportContextMenuState state, bool refreshDesignerSelection )
    {
        contextMenu = state;

        if ( contextMenuHost is not null )
            await contextMenuHost.ShowAsync( state );

        if ( refreshDesignerSelection )
            await InvokeAsync( StateHasChanged );
    }

    private void PopulateSectionContextMenuCapabilities( ReportDefinition definition, ReportContextMenuState state )
    {
        if ( definition is null
            || state is null
            || state.SectionIndex < 0
            || state.SectionIndex >= definition.Sections.Count )
        {
            return;
        }

        ReportSectionDefinition section = definition.Sections[state.SectionIndex];

        state.SectionSuppressed = section.Suppressed;
        state.CanPasteElement = CanContextPasteElement( definition, state );
        state.CanSelectAllSectionElements = section.Elements?.Count > 0;
        state.CanInsertSection = CanContextSectionInsertSection( definition, state.SectionIndex );
        state.CanInsertGroup = CanContextSectionInsertGroup( section );
        state.CanDeleteSection = ReportDefinitionHelper.CanDeleteSection( section );
        state.SectionKeepTogether = section.KeepTogether?.Value == true;
        state.SectionNewPageBefore = section.NewPageBefore?.Value == true;
        state.SectionNewPageAfter = section.NewPageAfter?.Value == true;
    }

    private void PopulateElementContextMenuCapabilities( ReportDefinition definition, ReportContextMenuState state )
    {
        if ( definition is null
            || state is null
            || !ReportDefinitionHelper.TryFindElementLocation( definition, state.ElementKey, out int sectionIndex, out _, out ReportElementDefinition element ) )
        {
            return;
        }

        state.CanEditText = CanEditElementText( element );
        state.CanEditFormula = CanEditFormulaFieldElement( definition, element );
        state.CanEditRunningTotal = CanEditRunningTotalElement( definition, element );
        state.CanPasteElement = clipboardElement is not null;
        state.ElementCanGrow = element.CanGrow?.Value == true;
        state.ElementSuppressed = element.Suppress?.Value == true;
        state.CanAlignOrSizeSelectedElements = state.SelectedElementCount >= DesignerConstants.MinimumBatchElementCount;
        state.CanInsertAggregate = sectionIndex >= 0
            && sectionIndex < definition.Sections.Count
            && definition.Sections[sectionIndex].Type == ReportSectionType.Detail
            && element.Type == ReportElementType.Field;
    }

    private void PopulateTableCellContextMenuCapabilities( ReportDefinition definition, ReportContextMenuState state )
    {
        if ( definition is null
            || state is null
            || !ReportDefinitionHelper.TryFindTableCellLocation( definition, state.CellKey, out _, out _, out ReportElementDefinition table, out ReportTableCellDefinition cell ) )
        {
            return;
        }

        state.CanPasteElement = clipboardElement is not null;
        state.CanMergeCellRight = CanMergeTableCellRight( table, cell );
        state.CanMergeCellDown = CanMergeTableCellDown( table, cell );
        state.CanUnmergeCell = cell.RowSpan > 1 || cell.ColumnSpan > 1;
    }

    private bool CanContextPasteElement( ReportDefinition definition, ReportContextMenuState state = null )
    {
        state ??= contextMenu;

        return clipboardElement is not null
            && state?.Target is ReportContextMenuTarget.Section or ReportContextMenuTarget.Cell
            && state.HasPastePosition
            && state.SectionIndex >= 0
            && state.SectionIndex < definition.Sections.Count;
    }

    private IReadOnlyList<ReportDesignerFieldOption> GetContextElementAggregateFieldOptions( ReportDefinition definition )
    {
        if ( !IsElementContextMenuVisible()
            || !ReportDefinitionHelper.TryFindElementLocation( definition, contextMenu.ElementKey, out var sectionIndex, out _, out var element )
            || sectionIndex < 0
            || sectionIndex >= definition.Sections.Count )
        {
            return [];
        }

        var section = definition.Sections[sectionIndex];

        if ( section.Type != ReportSectionType.Detail || element?.Type != ReportElementType.Field )
            return [];

        var dataSourceName = section.DataSource ?? element.DataSource;
        var dataSourceValue = ReportDataResolver.ResolveDataSourceValue( definition, Data, dataSourceName );
        var fields = ReportDataSourceExplorer.ResolveDataSourceFields( dataSourceValue ).ToList();
        var fieldOptions = FlattenDesignerFieldOptions( sectionIndex, dataSourceName, fields ).ToList();

        if ( fieldOptions.Count == 0 && !string.IsNullOrWhiteSpace( element.Field ) )
        {
            fieldOptions.Add( new()
            {
                SourceSectionIndex = sectionIndex,
                DataSourceName = dataSourceName,
                FieldName = element.Field,
                DisplayName = element.Field,
            } );
        }

        return fieldOptions
            .Where( option => ReportAggregateResolver.GetSupportedFunctions( definition, Data, option.DataSourceName, option.FieldName, option.DataType ).Count > 0 )
            .ToList();
    }

    private static bool CanEditElementText( ReportElementDefinition element )
    {
        return element?.Type == ReportElementType.Text;
    }

    private static bool CanEditFormulaFieldElement( ReportDefinition definition, ReportElementDefinition element )
    {
        return element?.Type == ReportElementType.Field
            && !string.IsNullOrWhiteSpace( element.Field )
            && FindFormulaField( definition, ReportFormulaFieldResolver.NormalizeFieldName( element.Field ) ) is not null;
    }

    private static bool CanEditRunningTotalElement( ReportDefinition definition, ReportElementDefinition element )
    {
        return element?.Type == ReportElementType.Field
            && !string.IsNullOrWhiteSpace( element.Field )
            && FindRunningTotal( definition, ReportRunningTotalResolver.NormalizeFieldName( element.Field ) ) is not null;
    }

    private bool TryGetContextElementFormulaFieldName( ReportDefinition definition, out string formulaFieldName )
    {
        formulaFieldName = null;

        if ( !IsElementContextMenuVisible()
            || !ReportDefinitionHelper.TryFindElementLocation( definition, contextMenu.ElementKey, out _, out _, out ReportElementDefinition element ) )
        {
            return false;
        }

        if ( element?.Type != ReportElementType.Field || string.IsNullOrWhiteSpace( element.Field ) )
            return false;

        string normalizedFieldName = ReportFormulaFieldResolver.NormalizeFieldName( element.Field );

        if ( FindFormulaField( definition, normalizedFieldName ) is null )
        {
            return false;
        }

        formulaFieldName = normalizedFieldName;

        return true;
    }

    private bool TryGetContextElementRunningTotalName( ReportDefinition definition, out string runningTotalName )
    {
        runningTotalName = null;

        if ( !IsElementContextMenuVisible()
            || !ReportDefinitionHelper.TryFindElementLocation( definition, contextMenu.ElementKey, out _, out _, out ReportElementDefinition element ) )
        {
            return false;
        }

        if ( element?.Type != ReportElementType.Field || string.IsNullOrWhiteSpace( element.Field ) )
            return false;

        string normalizedFieldName = ReportRunningTotalResolver.NormalizeFieldName( element.Field );

        if ( FindRunningTotal( definition, normalizedFieldName ) is null )
        {
            return false;
        }

        runningTotalName = normalizedFieldName;

        return true;
    }

    private void BeginContextElementTextEdit()
    {
        if ( IsElementContextMenuVisible() )
            BeginElementTextEdit( contextMenu.ElementKey );
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

        ReportRunningTotalDefinition runningTotal = FindRunningTotal( EffectiveDefinition, runningTotalName );

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
            || !CanEditElementText( element ) )
        {
            return;
        }

        SelectElement( elementKey );
        editingElementKey = elementKey;
        CloseContextMenu();
    }

    private void CancelElementTextEdit( string elementKey )
    {
        if ( string.Equals( editingElementKey, elementKey, StringComparison.Ordinal ) )
            editingElementKey = null;
    }

    private Task CancelElementTextEditAsync( string elementKey )
    {
        CancelElementTextEdit( elementKey );

        return Task.CompletedTask;
    }

    private async Task CommitElementTextEditAsync( string elementKey, string text )
    {
        editingElementKey = null;

        if ( !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, elementKey, out _, out _, out var currentElement )
            || !CanEditElementText( currentElement )
            || string.Equals( currentElement.Text, text, StringComparison.Ordinal ) )
        {
            return;
        }

        await ExecuteDesignerCommandAsync( new( "Edit text", () =>
        {
            if ( ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, elementKey, out _, out _, out var element )
                && CanEditElementText( element ) )
            {
                element.Text = text;
            }

            return Task.CompletedTask;
        } ) );
    }

    private async Task OpenContextElementAggregateDialogAsync()
    {
        if ( !IsElementContextMenuVisible() || aggregateDialogRef is null )
            return;

        var elementKey = contextMenu.ElementKey;
        var fieldOptions = GetContextElementAggregateFieldOptions( EffectiveDefinition );

        if ( fieldOptions.Count == 0 )
            return;

        var sourceSectionIndex = fieldOptions[0].SourceSectionIndex;
        var summaryLocations = GetAggregateSummaryLocations( EffectiveDefinition, sourceSectionIndex );

        CloseContextMenu();

        var selectedFieldName = ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, elementKey, out _, out _, out var element )
            ? element.Field
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
        string cellKey = contextMenu?.CellKey ?? selectionManager.SelectedCellKey;

        if ( string.IsNullOrWhiteSpace( cellKey ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Merge table cell", () =>
        {
            if ( !ReportDefinitionHelper.TryFindTableCellLocation( EffectiveDefinition, cellKey, out _, out _, out ReportElementDefinition table, out ReportTableCellDefinition cell ) )
                return Task.CompletedTask;

            if ( columnSpanDelta > 0 && !CanMergeTableCellRight( table, cell ) )
                return Task.CompletedTask;

            if ( rowSpanDelta > 0 && !CanMergeTableCellDown( table, cell ) )
                return Task.CompletedTask;

            int oldRowSpan = Math.Max( 1, cell.RowSpan );
            int oldColumnSpan = Math.Max( 1, cell.ColumnSpan );
            int newRowSpan = oldRowSpan + rowSpanDelta;
            int newColumnSpan = oldColumnSpan + columnSpanDelta;

            List<ReportTableCellDefinition> coveredCells = table.Cells
                .Where( item => item != cell
                    && item.RowIndex >= cell.RowIndex
                    && item.RowIndex < cell.RowIndex + newRowSpan
                    && item.ColumnIndex >= cell.ColumnIndex
                    && item.ColumnIndex < cell.ColumnIndex + newColumnSpan )
                .ToList();

            foreach ( ReportTableCellDefinition coveredCell in coveredCells )
            {
                if ( coveredCell.Elements?.Count > 0 )
                    cell.Elements.AddRange( coveredCell.Elements );

                table.Cells.Remove( coveredCell );
            }

            cell.RowSpan = newRowSpan;
            cell.ColumnSpan = newColumnSpan;
            ReportDefinitionHelper.FitElementsToTableCell( table, cell );

            SelectTableCell( cell.Id );

            return Task.CompletedTask;
        } ) );
    }

    private async Task UnmergeSelectedTableCellAsync()
    {
        string cellKey = contextMenu?.CellKey ?? selectionManager.SelectedCellKey;

        if ( string.IsNullOrWhiteSpace( cellKey ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Unmerge table cell", () =>
        {
            if ( !ReportDefinitionHelper.TryFindTableCellLocation( EffectiveDefinition, cellKey, out _, out _, out ReportElementDefinition table, out ReportTableCellDefinition cell ) )
                return Task.CompletedTask;

            int oldRowSpan = Math.Max( 1, cell.RowSpan );
            int oldColumnSpan = Math.Max( 1, cell.ColumnSpan );

            if ( oldRowSpan == 1 && oldColumnSpan == 1 )
                return Task.CompletedTask;

            cell.RowSpan = 1;
            cell.ColumnSpan = 1;
            ReportDefinitionHelper.FitElementsToTableCell( table, cell );

            for ( int rowIndex = cell.RowIndex; rowIndex < cell.RowIndex + oldRowSpan; rowIndex++ )
            {
                for ( int columnIndex = cell.ColumnIndex; columnIndex < cell.ColumnIndex + oldColumnSpan; columnIndex++ )
                {
                    if ( rowIndex == cell.RowIndex && columnIndex == cell.ColumnIndex )
                        continue;

                    if ( table.Cells.Any( item => item.RowIndex == rowIndex && item.ColumnIndex == columnIndex ) )
                        continue;

                    table.Cells.Add( new()
                    {
                        RowIndex = rowIndex,
                        ColumnIndex = columnIndex,
                    } );
                }
            }

            SelectTableCell( cell.Id );

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
            var sourceElement = FindDetailFieldElement( sourceSection, result.FieldName ) ?? new ReportElementDefinition
            {
                Name = result.FieldName,
                Type = ReportElementType.Field,
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
                : EnsureAggregateTargetSection( definition, sourceSectionIndex );
            var targetSection = definition.Sections[targetSectionIndex];
            var aggregateElement = CreateAggregateElement( sourceSection, sourceElement, result.Function, targetSection, targetSection.Type == ReportSectionType.GroupFooter );

            targetSection.Elements.Add( aggregateElement );
            ReportLayoutGeometry.GrowSectionToFitElement( targetSection, aggregateElement );
            SelectElement( ReportDefinitionHelper.EnsureElementId( aggregateElement ) );

            return Task.CompletedTask;
        } ) );
    }

    private bool CanSelectedSectionInsertGroup( ReportDefinition definition )
    {
        return CanContextSectionInsertGroup( selectionManager.FindSelectedSection( definition ) );
    }

    private bool CanSelectedSectionInsertSection( ReportDefinition definition )
    {
        return selectionManager.SelectedSectionIndex is { } sectionIndex
            && CanContextSectionInsertSection( definition, sectionIndex );
    }

    private static bool CanContextSectionInsertSection( ReportDefinition definition, int sectionIndex )
    {
        if ( definition?.Sections is null || sectionIndex < 0 || sectionIndex >= definition.Sections.Count )
            return false;

        var section = definition.Sections[sectionIndex];

        if ( section.Type is ReportSectionType.Group or ReportSectionType.GroupHeader )
            return !section.Suppressed
                && !string.IsNullOrWhiteSpace( section.GroupBy )
                && TryFindAggregateGroupLocation( definition, ResolveDetailSectionIndexForGroupHeader( definition, sectionIndex ), out _, out _ );

        if ( section.Type == ReportSectionType.GroupFooter )
            return !section.Suppressed
                && TryFindGroupHeaderForGroupFooter( definition, sectionIndex, out _ );

        return section is not null
            && !section.Suppressed;
    }

    private static bool CanContextSectionInsertGroup( ReportSectionDefinition section )
    {
        return section is not null
            && !section.Suppressed
            && section.Type == ReportSectionType.Detail;
    }

    private async Task OpenSelectedDetailGroupDialogAsync()
    {
        if ( groupDialogRef is null )
            return;

        var definition = EffectiveDefinition;
        var fieldOptions = GetSelectedDetailGroupFieldOptions( definition );

        if ( fieldOptions.Count == 0 )
            return;

        var selectedFieldName = selectionManager.SelectedSectionIndex is { } sectionIndex
            && TryFindAggregateGroupLocation( definition, sectionIndex, out var groupHeader, out _ )
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

            var groupHeader = CreateGroupHeaderSection( definition, groupBy );
            var groupFooter = CreateGroupFooterSection( definition, groupBy );

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
            var definition = EffectiveDefinition;

            if ( definition.DataSources is null )
                definition.DataSources = [];

            if ( string.Equals( dataSource.Type, ObjectReportDataSourceProvider.ProviderType, StringComparison.OrdinalIgnoreCase )
                && dataSource.Data is null )
            {
                dataSource.Data = Data;
            }

            var existingIndex = definition.DataSources.FindIndex( source =>
                string.Equals( source.Id, dataSource.Id, StringComparison.Ordinal )
                || string.Equals( source.Name, dataSource.Name, StringComparison.OrdinalIgnoreCase ) );

            if ( existingIndex >= 0 )
                definition.DataSources[existingIndex] = dataSource;
            else
                definition.DataSources.Add( dataSource );

            ReportDefinitionHelper.EnsureDefinitionIds( definition );
            await ResolveDataSourcesAsync( definition, loadData: false );

            return;
        } ) );
    }

    private async Task OnDataSourceRefreshedAsync( string dataSourceName )
    {
        if ( string.IsNullOrWhiteSpace( dataSourceName ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Refresh data source", async () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            ReportDataSourceDefinition dataSource = FindDataSource( definition, dataSourceName );

            if ( dataSource is null )
                return;

            IReportDataSourceProvider provider = DataSourceProviderRegistry?.FindProvider( dataSource.Type );

            if ( provider is null )
                return;

            try
            {
                dataSource.Schema = await provider.GetSchemaAsync( dataSource );
            }
            catch
            {
            }

            if ( !string.Equals( dataSource.Type, ObjectReportDataSourceProvider.ProviderType, StringComparison.OrdinalIgnoreCase ) )
                dataSource.Data = null;
        } ) );
    }

    private async Task OnDataSourceDeletedAsync( string dataSourceName )
    {
        if ( string.IsNullOrWhiteSpace( dataSourceName ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Delete data source", () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            ReportDataSourceDefinition dataSource = FindDataSource( definition, dataSourceName );

            if ( dataSource is not null )
                definition.DataSources.Remove( dataSource );

            return Task.CompletedTask;
        } ) );
    }

    private async Task OnFormulaFieldConfirmedAsync( ReportFormulaFieldDefinition formulaField )
    {
        if ( formulaField is null || string.IsNullOrWhiteSpace( formulaField.Name ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Save formula field", () =>
        {
            ReportDefinition definition = EffectiveDefinition;

            if ( definition.FormulaFields is null )
                definition.FormulaFields = [];

            ReportFormulaFieldDefinition existingFormulaField = definition.FormulaFields.FirstOrDefault( field =>
                string.Equals( field.Name, formulaField.Name, StringComparison.OrdinalIgnoreCase ) );

            if ( existingFormulaField is null )
            {
                formulaField.Name = formulaField.Name.Trim();
                definition.FormulaFields.Add( formulaField );
            }
            else
            {
                existingFormulaField.Formula = formulaField.Formula;
            }

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
            ReportDefinition definition = EffectiveDefinition;
            ReportFormulaFieldDefinition formulaField = FindFormulaField( definition, oldName );

            if ( formulaField is null || FindFormulaField( definition, newName ) is not null )
                return Task.CompletedTask;

            formulaField.Name = newName;
            ReplaceFormulaFieldReferences( definition, oldName, newName );

            return Task.CompletedTask;
        } ) );
    }

    private async Task OnFormulaFieldDeletedAsync( string formulaFieldName )
    {
        if ( string.IsNullOrWhiteSpace( formulaFieldName ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Delete formula field", () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            ReportFormulaFieldDefinition formulaField = FindFormulaField( definition, formulaFieldName );

            if ( formulaField is not null )
                definition.FormulaFields.Remove( formulaField );

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
            int sectionIndex = GetFormulaFieldInsertionSectionIndex( definition );

            if ( sectionIndex < 0 || sectionIndex >= definition.Sections.Count || FindFormulaField( definition, formulaFieldName ) is null )
                return Task.CompletedTask;

            ReportSectionDefinition section = definition.Sections[sectionIndex];
            double y = GetNextFormulaFieldInsertionY( section );
            ReportElementDefinition element = new()
            {
                Name = formulaFieldName,
                Type = ReportElementType.Field,
                DataSource = ReportFormulaFieldResolver.DataSourceName,
                Field = formulaFieldName,
                X = 0,
                Y = y,
                Width = DesignerConstants.DefaultDroppedFieldWidth,
                Height = DesignerConstants.DefaultDroppedFieldHeight,
            };

            section.Elements.Add( element );
            section.Height = Math.Max( section.Height, y + DesignerConstants.DefaultDroppedFieldHeight );
            SelectElement( ReportDefinitionHelper.EnsureElementId( element ) );

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
            int sectionIndex = GetFormulaFieldInsertionSectionIndex( definition );

            if ( sectionIndex < 0 || sectionIndex >= definition.Sections.Count )
                return Task.CompletedTask;

            ReportSectionDefinition section = definition.Sections[sectionIndex];
            (string DataSourceName, string FieldName) fieldBinding = ReportDefinitionHelper.NormalizeFieldBindingForSection( definition, section, field.DataSourceName, field.FieldName );
            double y = GetNextFormulaFieldInsertionY( section );
            ReportElementDefinition element = new()
            {
                Name = fieldBinding.FieldName,
                Type = ReportElementType.Field,
                Field = fieldBinding.FieldName,
                DataSource = fieldBinding.DataSourceName,
                X = 0,
                Y = y,
                Width = DesignerConstants.DefaultDroppedFieldWidth,
                Height = DesignerConstants.DefaultDroppedFieldHeight,
            };

            section.Elements.Add( element );

            if ( !ReportSpecialFieldResolver.IsSpecialDataSource( fieldBinding.DataSourceName )
                && !ReportFormulaFieldResolver.IsFormulaDataSource( fieldBinding.DataSourceName )
                && !ReportRunningTotalResolver.IsRunningTotalDataSource( fieldBinding.DataSourceName ) )
                ReportDetailHeaderSynchronizer.AddPageHeaderForDetailField( definition, sectionIndex, section, fieldBinding.FieldName, element.X, element.Width );

            section.Height = Math.Max( section.Height, y + DesignerConstants.DefaultDroppedFieldHeight );
            SelectElement( ReportDefinitionHelper.EnsureElementId( element ) );

            return Task.CompletedTask;
        } ) );
    }

    private async Task OnRunningTotalConfirmedAsync( ReportRunningTotalDefinition runningTotal )
    {
        if ( runningTotal is null || string.IsNullOrWhiteSpace( runningTotal.Name ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Save running total", () =>
        {
            ReportDefinition definition = EffectiveDefinition;

            if ( definition.RunningTotals is null )
                definition.RunningTotals = [];

            runningTotal.Name = runningTotal.Name.Trim();

            ReportRunningTotalDefinition existingRunningTotal = definition.RunningTotals.FirstOrDefault( field =>
                string.Equals( field.Id, runningTotal.Id, StringComparison.Ordinal )
                || string.Equals( field.Name, runningTotal.Name, StringComparison.OrdinalIgnoreCase ) );

            if ( existingRunningTotal is null )
            {
                definition.RunningTotals.Add( runningTotal );
            }
            else
            {
                existingRunningTotal.Name = runningTotal.Name;
                existingRunningTotal.DataSource = runningTotal.DataSource;
                existingRunningTotal.Field = runningTotal.Field;
                existingRunningTotal.Function = runningTotal.Function;
                existingRunningTotal.EvaluateMode = runningTotal.EvaluateMode;
                existingRunningTotal.EvaluateFormula = runningTotal.EvaluateFormula;
                existingRunningTotal.ResetMode = runningTotal.ResetMode;
                existingRunningTotal.ResetGroupId = runningTotal.ResetGroupId;
            }

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
            ReportDefinition definition = EffectiveDefinition;
            ReportRunningTotalDefinition runningTotal = FindRunningTotal( definition, oldName );

            if ( runningTotal is null || FindRunningTotal( definition, newName ) is not null )
                return Task.CompletedTask;

            runningTotal.Name = newName;
            ReplaceRunningTotalReferences( definition, oldName, newName );

            return Task.CompletedTask;
        } ) );
    }

    private async Task OnRunningTotalDeletedAsync( string runningTotalName )
    {
        if ( string.IsNullOrWhiteSpace( runningTotalName ) )
            return;

        await ExecuteDesignerCommandAsync( new( "Delete running total", () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            ReportRunningTotalDefinition runningTotal = FindRunningTotal( definition, runningTotalName );

            if ( runningTotal is not null )
                definition.RunningTotals.Remove( runningTotal );

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
            int sectionIndex = GetFormulaFieldInsertionSectionIndex( definition );

            if ( sectionIndex < 0 || sectionIndex >= definition.Sections.Count || FindRunningTotal( definition, runningTotalName ) is null )
                return Task.CompletedTask;

            ReportSectionDefinition section = definition.Sections[sectionIndex];
            double y = GetNextFormulaFieldInsertionY( section );
            ReportElementDefinition element = new()
            {
                Name = runningTotalName,
                Type = ReportElementType.Field,
                DataSource = ReportRunningTotalResolver.DataSourceName,
                Field = runningTotalName,
                X = 0,
                Y = y,
                Width = DesignerConstants.DefaultDroppedFieldWidth,
                Height = DesignerConstants.DefaultDroppedFieldHeight,
                Font = new()
                {
                    Bold = true,
                },
            };

            section.Elements.Add( element );
            section.Height = Math.Max( section.Height, y + DesignerConstants.DefaultDroppedFieldHeight );
            SelectElement( ReportDefinitionHelper.EnsureElementId( element ) );

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

        ReportFormulaFieldDefinition formulaField = FindFormulaField( EffectiveDefinition, formulaFieldName );

        if ( formulaField is null )
            return;

        editingFormulaFieldName = formulaField.Name;
        await formulaDialogRef.ShowAsync( formulaField.Name, formulaField.Formula );
    }

    private int GetFormulaFieldInsertionSectionIndex( ReportDefinition definition )
    {
        if ( definition?.Sections is null )
            return -1;

        if ( selectionManager.SelectedSectionIndex is { } selectedSectionIndex
            && selectedSectionIndex >= 0
            && selectedSectionIndex < definition.Sections.Count )
        {
            return selectedSectionIndex;
        }

        if ( !string.IsNullOrWhiteSpace( selectionManager.SelectedElementKey )
            && ReportDefinitionHelper.TryFindElementLocation( definition, selectionManager.SelectedElementKey, out int elementSectionIndex, out _, out _ ) )
        {
            return elementSectionIndex;
        }

        return -1;
    }

    private double GetNextFormulaFieldInsertionY( ReportSectionDefinition section )
    {
        if ( section?.Elements is null || section.Elements.Count == 0 )
            return 0;

        double y = section.Elements.Max( element => element.Y + element.Height ) + DesignerConstants.DefaultDroppedFieldHeight;

        return ApplyDesignerGrid( y );
    }

    private static ReportFormulaFieldDefinition FindFormulaField( ReportDefinition definition, string formulaFieldName )
    {
        if ( definition?.FormulaFields is null || string.IsNullOrWhiteSpace( formulaFieldName ) )
            return null;

        string normalizedFormulaFieldName = ReportFormulaFieldResolver.NormalizeFieldName( formulaFieldName );

        return definition.FormulaFields.FirstOrDefault( field =>
            string.Equals( field.Name, normalizedFormulaFieldName, StringComparison.OrdinalIgnoreCase ) );
    }

    private static ReportRunningTotalDefinition FindRunningTotal( ReportDefinition definition, string runningTotalName )
    {
        return ReportRunningTotalResolver.FindRunningTotal( definition, runningTotalName );
    }

    private static ReportDataSourceDefinition FindDataSource( ReportDefinition definition, string dataSourceName )
    {
        if ( definition?.DataSources is null || string.IsNullOrWhiteSpace( dataSourceName ) )
            return null;

        return definition.DataSources.FirstOrDefault( dataSource =>
            string.Equals( dataSource.Name, dataSourceName, StringComparison.OrdinalIgnoreCase ) );
    }

    private static void ReplaceFormulaFieldReferences( ReportDefinition definition, string oldName, string newName )
    {
        if ( definition is null )
            return;

        foreach ( ReportFormulaFieldDefinition formulaField in definition.FormulaFields ?? [] )
        {
            formulaField.Formula = ReplaceFormulaFieldExpressionToken( formulaField.Formula, oldName, newName );
        }

        foreach ( ReportSectionDefinition section in definition.Sections ?? [] )
        {
            ReplaceFormulaFieldReference( section.Suppress, oldName, newName );
            ReplaceFormulaFieldReference( section.KeepTogether, oldName, newName );
            ReplaceFormulaFieldReference( section.NewPageBefore, oldName, newName );
            ReplaceFormulaFieldReference( section.NewPageAfter, oldName, newName );

            foreach ( ReportElementDefinition element in section.Elements ?? [] )
            {
                if ( element.Type == ReportElementType.Field
                    && !string.IsNullOrWhiteSpace( element.Field )
                    && string.Equals( ReportFormulaFieldResolver.NormalizeFieldName( element.Field ), oldName, StringComparison.OrdinalIgnoreCase )
                    && ( ReportFormulaFieldResolver.IsFormulaDataSource( element.DataSource ) || string.IsNullOrWhiteSpace( element.DataSource ) ) )
                {
                    element.Field = newName;
                }

                element.Text = ReplaceFormulaFieldExpressionToken( element.Text, oldName, newName );
                ReplaceFormulaFieldReference( element.CanGrow, oldName, newName );
                ReplaceFormulaFieldReference( element.Suppress, oldName, newName );
                ReplaceFormulaFieldReference( element.SnapToGrid, oldName, newName );
            }
        }
    }

    private static void ReplaceRunningTotalReferences( ReportDefinition definition, string oldName, string newName )
    {
        if ( definition is null )
            return;

        foreach ( ReportFormulaFieldDefinition formulaField in definition.FormulaFields ?? [] )
        {
            formulaField.Formula = ReplaceRunningTotalExpressionToken( formulaField.Formula, oldName, newName );
        }

        foreach ( ReportRunningTotalDefinition runningTotal in definition.RunningTotals ?? [] )
        {
            runningTotal.EvaluateFormula = ReplaceRunningTotalExpressionToken( runningTotal.EvaluateFormula, oldName, newName );
        }

        foreach ( ReportSectionDefinition section in definition.Sections ?? [] )
        {
            ReplaceRunningTotalReference( section.Suppress, oldName, newName );
            ReplaceRunningTotalReference( section.KeepTogether, oldName, newName );
            ReplaceRunningTotalReference( section.NewPageBefore, oldName, newName );
            ReplaceRunningTotalReference( section.NewPageAfter, oldName, newName );

            foreach ( ReportElementDefinition element in section.Elements ?? [] )
            {
                if ( element.Type == ReportElementType.Field
                    && !string.IsNullOrWhiteSpace( element.Field )
                    && string.Equals( ReportRunningTotalResolver.NormalizeFieldName( element.Field ), oldName, StringComparison.OrdinalIgnoreCase )
                    && ( ReportRunningTotalResolver.IsRunningTotalDataSource( element.DataSource ) || string.IsNullOrWhiteSpace( element.DataSource ) ) )
                {
                    element.Field = newName;
                }

                element.Text = ReplaceRunningTotalExpressionToken( element.Text, oldName, newName );
                ReplaceRunningTotalReference( element.CanGrow, oldName, newName );
                ReplaceRunningTotalReference( element.Suppress, oldName, newName );
                ReplaceRunningTotalReference( element.SnapToGrid, oldName, newName );
            }
        }
    }

    private static void ReplaceRunningTotalReference( ReportValue<bool> value, string oldName, string newName )
    {
        if ( value is not null )
            value.Formula = ReplaceRunningTotalExpressionToken( value.Formula, oldName, newName );
    }

    private static void ReplaceRunningTotalReference( ReportValue<bool?> value, string oldName, string newName )
    {
        if ( value is not null )
            value.Formula = ReplaceRunningTotalExpressionToken( value.Formula, oldName, newName );
    }

    private static string ReplaceRunningTotalExpressionToken( string value, string oldName, string newName )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
            return value;

        string oldToken = ReportExpressionFormatter.FormatFieldExpression( null, oldName );
        string newToken = ReportExpressionFormatter.FormatFieldExpression( null, newName );
        string oldQualifiedToken = $"{{{ReportRunningTotalResolver.DataSourceName}.{oldName}}}";

        return value
            .Replace( oldQualifiedToken, newToken, StringComparison.OrdinalIgnoreCase )
            .Replace( oldToken, newToken, StringComparison.OrdinalIgnoreCase );
    }

    private static void ReplaceFormulaFieldReference( ReportValue<bool> value, string oldName, string newName )
    {
        if ( value is not null )
            value.Formula = ReplaceFormulaFieldExpressionToken( value.Formula, oldName, newName );
    }

    private static void ReplaceFormulaFieldReference( ReportValue<bool?> value, string oldName, string newName )
    {
        if ( value is not null )
            value.Formula = ReplaceFormulaFieldExpressionToken( value.Formula, oldName, newName );
    }

    private static string ReplaceFormulaFieldExpressionToken( string value, string oldName, string newName )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
            return value;

        string oldToken = ReportExpressionFormatter.FormatFieldExpression( null, oldName );
        string newToken = ReportExpressionFormatter.FormatFieldExpression( null, newName );
        string oldQualifiedToken = $"{{{ReportFormulaFieldResolver.DataSourceName}.{oldName}}}";

        return value
            .Replace( oldQualifiedToken, newToken, StringComparison.OrdinalIgnoreCase )
            .Replace( oldToken, newToken, StringComparison.OrdinalIgnoreCase );
    }

    private static IReadOnlyList<ReportAggregateSummaryLocation> GetAggregateSummaryLocations( ReportDefinition definition, int sourceSectionIndex )
    {
        var locations = new List<ReportAggregateSummaryLocation>
        {
            new()
            {
                TargetSectionIndex = -1,
                Name = "Grand Total (Report Footer)",
            },
        };

        if ( definition?.Sections is null || sourceSectionIndex < 0 || sourceSectionIndex >= definition.Sections.Count )
            return locations;

        if ( !TryFindAggregateGroupLocation( definition, sourceSectionIndex, out var groupHeader, out var groupFooterIndex ) )
            return locations;

        locations.Add( new()
        {
            TargetSectionIndex = groupFooterIndex,
            Name = $"Group Total ({ReportDefinitionHelper.GetSectionDisplayName( groupHeader )})",
        } );

        return locations;
    }

    private static bool TryFindAggregateGroupLocation( ReportDefinition definition, int detailSectionIndex, out ReportSectionDefinition groupHeader, out int groupFooterIndex )
    {
        groupHeader = null;
        groupFooterIndex = -1;

        for ( var sectionIndex = detailSectionIndex - 1; sectionIndex >= 0; sectionIndex-- )
        {
            var section = definition.Sections[sectionIndex];

            if ( section.Suppressed )
                continue;

            if ( section.Type is ReportSectionType.Group or ReportSectionType.GroupHeader )
            {
                if ( string.IsNullOrWhiteSpace( section.GroupBy ) )
                    return false;

                groupHeader = section;
                break;
            }

            if ( section.Type is ReportSectionType.Detail or ReportSectionType.ReportHeader or ReportSectionType.Header or ReportSectionType.PageHeader )
                return false;
        }

        if ( groupHeader is null )
            return false;

        for ( var sectionIndex = detailSectionIndex + 1; sectionIndex < definition.Sections.Count; sectionIndex++ )
        {
            var section = definition.Sections[sectionIndex];

            if ( section.Suppressed )
                continue;

            if ( section.Type == ReportSectionType.GroupFooter )
            {
                groupFooterIndex = sectionIndex;
                return true;
            }

            if ( section.Type is ReportSectionType.Detail or ReportSectionType.ReportFooter or ReportSectionType.Footer or ReportSectionType.PageFooter or ReportSectionType.Group or ReportSectionType.GroupHeader )
                return false;
        }

        return false;
    }

    private static int ResolveDetailSectionIndexForGroupHeader( ReportDefinition definition, int groupHeaderIndex )
    {
        if ( definition?.Sections is null || groupHeaderIndex < 0 || groupHeaderIndex >= definition.Sections.Count )
            return -1;

        for ( var sectionIndex = groupHeaderIndex + 1; sectionIndex < definition.Sections.Count; sectionIndex++ )
        {
            var section = definition.Sections[sectionIndex];

            if ( section.Suppressed )
                continue;

            if ( section.Type == ReportSectionType.Detail )
                return sectionIndex;

            if ( section.Type is ReportSectionType.ReportFooter or ReportSectionType.Footer or ReportSectionType.PageFooter or ReportSectionType.GroupFooter )
                return -1;

            if ( ( section.Type is ReportSectionType.Group or ReportSectionType.GroupHeader )
                && !string.Equals( section.GroupBy, definition.Sections[groupHeaderIndex].GroupBy, StringComparison.OrdinalIgnoreCase ) )
            {
                return -1;
            }
        }

        return -1;
    }

    private static bool TryFindGroupHeaderForGroupFooter( ReportDefinition definition, int groupFooterIndex, out ReportSectionDefinition groupHeader )
    {
        groupHeader = null;

        if ( definition?.Sections is null || groupFooterIndex < 0 || groupFooterIndex >= definition.Sections.Count )
            return false;

        var detailSectionIndex = -1;

        for ( var sectionIndex = groupFooterIndex - 1; sectionIndex >= 0; sectionIndex-- )
        {
            var section = definition.Sections[sectionIndex];

            if ( section.Suppressed )
                continue;

            if ( section.Type == ReportSectionType.Detail )
            {
                detailSectionIndex = sectionIndex;
                break;
            }

            if ( section.Type is ReportSectionType.ReportFooter or ReportSectionType.Footer or ReportSectionType.PageFooter or ReportSectionType.Group or ReportSectionType.GroupHeader )
                return false;
        }

        return detailSectionIndex >= 0
            && TryFindAggregateGroupLocation( definition, detailSectionIndex, out groupHeader, out var foundGroupFooterIndex )
            && foundGroupFooterIndex <= groupFooterIndex;
    }

    private IReadOnlyList<ReportDesignerFieldOption> GetSelectedDetailGroupFieldOptions( ReportDefinition definition )
    {
        if ( definition?.Sections is null || selectionManager.SelectedSectionIndex is not { } sectionIndex )
            return [];

        var section = definition.Sections[sectionIndex];

        if ( section.Type != ReportSectionType.Detail )
            return [];

        var dataSourceName = section.DataSource;
        var dataSourceValue = ReportDataResolver.ResolveDataSourceValue( definition, Data, dataSourceName );
        var fields = ReportDataSourceExplorer.ResolveDataSourceFields( dataSourceValue ).ToList();
        var fieldOptions = FlattenDesignerFieldOptions( sectionIndex, dataSourceName, fields )
            .OrderBy( field => field.DisplayName )
            .ToList();

        foreach ( var fieldElement in section.Elements.Where( element => element.Type == ReportElementType.Field && !string.IsNullOrWhiteSpace( element.Field ) ) )
        {
            if ( fieldOptions.Any( field => string.Equals( field.FieldName, fieldElement.Field, StringComparison.OrdinalIgnoreCase ) ) )
                continue;

            fieldOptions.Add( new()
            {
                SourceSectionIndex = sectionIndex,
                DataSourceName = dataSourceName,
                FieldName = fieldElement.Field,
                DisplayName = fieldElement.Field,
            } );
        }

        return fieldOptions;
    }

    private static IEnumerable<ReportDesignerFieldOption> FlattenDesignerFieldOptions( int sourceSectionIndex, string dataSourceName, IEnumerable<ReportDesignerFieldNode> fields )
    {
        foreach ( var field in fields ?? [] )
        {
            if ( field.Children.Count == 0 )
            {
                yield return new()
                {
                    SourceSectionIndex = sourceSectionIndex,
                    DataSourceName = dataSourceName,
                    FieldName = field.Path,
                    DisplayName = field.Path,
                    DataType = field.DataType,
                };

                continue;
            }

            foreach ( var child in FlattenDesignerFieldOptions( sourceSectionIndex, dataSourceName, field.Children ) )
            {
                yield return child;
            }
        }
    }

    private static ReportElementDefinition FindDetailFieldElement( ReportSectionDefinition section, string fieldName )
    {
        return section?.Elements?.FirstOrDefault( element =>
            element.Type == ReportElementType.Field
            && string.Equals( element.Field, fieldName, StringComparison.OrdinalIgnoreCase ) );
    }

    private IReadOnlyList<ReportAggregateFunction> ResolveAggregateDialogSupportedFunctions( ReportDesignerFieldOption field )
    {
        return field is null
            ? []
            : ReportAggregateResolver.GetSupportedFunctions( EffectiveDefinition, Data, field.DataSourceName, field.FieldName, field.DataType );
    }

    private bool IsElementTextEditing( string elementKey = null )
    {
        return string.IsNullOrWhiteSpace( elementKey )
            ? !string.IsNullOrWhiteSpace( editingElementKey )
            : string.Equals( editingElementKey, elementKey, StringComparison.Ordinal );
    }

    private void CloseContextMenu()
    {
        contextMenu = null;

        if ( contextMenuHost is not null )
            _ = contextMenuHost.CloseAsync();
    }

    private async Task CloseContextMenuAsync()
    {
        contextMenu = null;

        if ( contextMenuHost is not null )
            await contextMenuHost.CloseAsync();
    }

    private static ReportSectionDefinition CreateGroupHeaderSection( ReportDefinition definition, string groupBy )
    {
        var groupName = ResolveGroupName( groupBy );

        return new()
        {
            Name = ReportDefinitionHelper.CreateUniqueSectionName( definition, $"{groupName} group header" ),
            Type = ReportSectionType.GroupHeader,
            Height = DesignerConstants.DefaultGroupSectionHeight,
            GroupBy = groupBy,
            Default = false,
            Suppressed = false,
            Elements =
            [
                new()
                {
                    Name = groupName,
                    Type = ReportElementType.Text,
                    Text = ReportExpressionFormatter.FormatFieldExpression( null, groupBy ),
                    X = DesignerConstants.DefaultGroupHeaderElementX,
                    Y = DesignerConstants.DefaultGroupHeaderElementY,
                    Width = DesignerConstants.DefaultGroupHeaderElementWidth,
                    Height = DesignerConstants.DefaultGroupHeaderElementHeight,
                    Font = new()
                    {
                        Bold = true,
                    },
                },
            ],
        };
    }

    private static ReportSectionDefinition CreateGroupFooterSection( ReportDefinition definition, string groupBy )
    {
        var groupName = ResolveGroupName( groupBy );

        return new()
        {
            Name = ReportDefinitionHelper.CreateUniqueSectionName( definition, $"{groupName} group footer" ),
            Type = ReportSectionType.GroupFooter,
            Height = DesignerConstants.DefaultGroupSectionHeight,
            GroupBy = groupBy,
            Default = false,
            Suppressed = false,
            Elements =
            [
                new()
                {
                    Name = $"{groupName} separator",
                    Type = ReportElementType.Line,
                    X = DesignerConstants.DefaultGroupFooterLineX,
                    Y = DesignerConstants.DefaultGroupFooterLineY,
                    Width = Math.Max( DesignerConstants.DefaultGroupFooterLineMinimumWidth, ( definition?.Page?.Width ?? DesignerConstants.DefaultPageWidthFallback ) - DesignerConstants.DefaultGroupFooterLinePagePadding ),
                    Height = DesignerConstants.DefaultGroupFooterLineHeight,
                },
            ],
        };
    }

    private static string ResolveGroupName( string groupBy )
    {
        if ( string.IsNullOrWhiteSpace( groupBy ) )
            return "Group";

        var normalizedGroupBy = groupBy.Trim();
        var lastSeparatorIndex = normalizedGroupBy.LastIndexOf( '.' );

        return lastSeparatorIndex >= 0 && lastSeparatorIndex < normalizedGroupBy.Length - 1
            ? normalizedGroupBy[( lastSeparatorIndex + 1 )..]
            : normalizedGroupBy;
    }

    private static ReportElementDefinition CreateAggregateElement( ReportSectionDefinition sourceSection, ReportElementDefinition sourceElement, ReportAggregateFunction function, ReportSectionDefinition targetSection, bool groupScoped )
    {
        var fieldName = sourceElement.Field;
        var functionName = ReportAggregateResolver.GetFunctionDisplayName( function );

        return new()
        {
            Name = $"{functionName} of {fieldName}",
            Type = ReportElementType.Field,
            Field = fieldName,
            Format = sourceElement.Format,
            DataSource = groupScoped ? null : string.IsNullOrWhiteSpace( sourceSection.DataSource ) ? sourceElement.DataSource : sourceSection.DataSource,
            X = sourceElement.X,
            Y = GetAggregateElementY( targetSection ),
            Width = sourceElement.Width,
            Height = Math.Max( sourceElement.Height, DesignerConstants.AggregateElementMinimumHeight ),
            Font = new()
            {
                Bold = true,
                Alignment = sourceElement.Font?.Alignment ?? TextAlignment.Default,
            },
            Aggregate = new()
            {
                Function = function,
            },
        };
    }

    private static int EnsureAggregateTargetSection( ReportDefinition definition, int sourceSectionIndex )
    {
        for ( var sectionIndex = sourceSectionIndex + 1; sectionIndex < definition.Sections.Count; sectionIndex++ )
        {
            var sectionType = definition.Sections[sectionIndex].Type;

            if ( sectionType is ReportSectionType.ReportFooter or ReportSectionType.Footer )
                return sectionIndex;

            if ( sectionType == ReportSectionType.PageFooter )
                return InsertAggregateReportFooter( definition, sectionIndex );
        }

        return InsertAggregateReportFooter( definition, definition.Sections.Count );
    }

    private static int InsertAggregateReportFooter( ReportDefinition definition, int sectionIndex )
    {
        var reportFooter = new ReportSectionDefinition
        {
            Name = "Aggregates",
            Type = ReportSectionType.ReportFooter,
            Height = DesignerConstants.AggregateReportFooterHeight,
        };

        definition.Sections.Insert( sectionIndex, reportFooter );

        return sectionIndex;
    }

    private static double GetAggregateElementY( ReportSectionDefinition targetSection )
    {
        if ( targetSection?.Elements is null || targetSection.Elements.Count == 0 )
            return DesignerConstants.PasteElementOffset;

        var aggregateElements = targetSection.Elements.Where( element => element.Aggregate is not null ).ToList();

        return aggregateElements.Count == 0
            ? Math.Max( DesignerConstants.PasteElementOffset, targetSection.Elements.Max( element => element.Y + element.Height ) + DesignerConstants.KeyboardMoveStep )
            : aggregateElements.Min( element => element.Y );
    }

    private async Task MoveSelectedElementAsync( double x, double y, double width, double height )
    {
        var element = selectionManager.FindSelectedElement( EffectiveDefinition );

        if ( element is null )
            return;

        await ExecuteDesignerCommandAsync( new( "Move element", () =>
        {
            var definition = EffectiveDefinition;
            var element = selectionManager.FindSelectedElement( definition );

            if ( element is not null && element.Suppress?.Value != true )
            {
                ReportDefinitionHelper.TryFindElementLocation( definition, ReportDefinitionHelper.EnsureElementId( element ), out var sectionIndex, out _, out _ );
                var originalX = element.X;
                var originalWidth = element.Width;
                var originalHeight = element.Height;
                bool useSnapToGrid = IsSnapToGridEnabled( element );

                element.X = ApplyDesignerGrid( element.X + x, useSnapToGrid );
                element.Y = ApplyDesignerGrid( element.Y + y, useSnapToGrid );
                element.Width = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, width == 0 ? element.Width : ApplyDesignerGrid( element.Width + width, useSnapToGrid ) );
                element.Height = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, height == 0 ? element.Height : ApplyDesignerGrid( element.Height + height, useSnapToGrid ) );

                ReportDefinitionHelper.ScaleTableLayout( element, originalWidth, originalHeight );
                ReportDetailHeaderSynchronizer.SyncMatchingPageHeaderForDetailElement( definition, sectionIndex, sectionIndex, element, originalX, originalWidth, element.X, element.Width );
            }

            return Task.CompletedTask;
        }, refreshSurface: false ) );
    }

    private async Task MoveSelectedElementsAsync( double x, double y )
    {
        var definition = EffectiveDefinition;
        var element = selectionManager.FindSelectedElement( definition );

        if ( element is null )
            return;

        bool useSnapToGrid = IsSnapToGridEnabled( element );
        var selectedElements = CaptureElementPointerItems( definition, ReportDefinitionHelper.EnsureElementId( element ) ).ToList();

        if ( selectedElements.Count == 0 )
            return;

        string commandName = selectedElements.Count == 1 ? "Move element" : "Move elements";

        await ExecuteDesignerCommandAsync( new( commandName, () =>
        {
            var definition = EffectiveDefinition;
            var selectedElementKeys = selectedElements.Select( item => item.ElementKey ).ToList();
            var affectedSectionIndexes = new HashSet<int>();

            foreach ( var item in selectedElements )
            {
                if ( !ReportDefinitionHelper.TryFindElementLocation( definition, item.ElementKey, out var sectionIndex, out _, out var element ) )
                    continue;

                if ( element.Suppress?.Value == true )
                    continue;

                element.X = ReportLayoutGeometry.Clamp( ApplyDesignerGrid( item.OriginalX + x, useSnapToGrid ), 0, Math.Max( 0, definition.Page.Width - element.Width ) );
                element.Y = ApplyDesignerGrid( item.OriginalY + y, useSnapToGrid );

                ReportDetailHeaderSynchronizer.SyncMatchingPageHeaderForDetailElement(
                    definition,
                    sectionIndex,
                    sectionIndex,
                    element,
                    item.OriginalX,
                    item.OriginalWidth,
                    element.X,
                    element.Width,
                    selectedElementKeys );

                affectedSectionIndexes.Add( sectionIndex );
            }

            foreach ( var sectionIndex in affectedSectionIndexes )
            {
                ReportLayoutGeometry.GrowSectionToFitElements( definition.Sections[sectionIndex] );
            }

            SelectElements( selectedElementKeys, ReportDefinitionHelper.EnsureElementId( element ) );

            return Task.CompletedTask;
        }, refreshSurface: false ) );
    }

    private async Task AlignSelectedElementsAsync( ReportElementAlignment alignment )
    {
        List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( EffectiveDefinition );

        if ( selectedElements.Count < DesignerConstants.MinimumBatchElementCount )
            return;

        string commandName = $"Align {GetAlignmentDisplayName( alignment )}";

        await ExecuteDesignerCommandAsync( new( commandName, () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( definition );

            if ( selectedElements.Count < DesignerConstants.MinimumBatchElementCount )
                return Task.CompletedTask;

            ReportSelectedElementContext anchor = selectedElements[0];
            List<string> selectedElementKeys = selectedElements.Select( item => item.ElementKey ).ToList();
            HashSet<int> affectedSectionIndexes = [];
            IEnumerable<ReportSelectedElementContext> elementsToAlign = alignment == ReportElementAlignment.ToGrid
                ? selectedElements
                : selectedElements.Skip( 1 );

            foreach ( ReportSelectedElementContext item in elementsToAlign )
            {
                ReportElementDefinition element = item.Element;

                if ( element.Suppress?.Value == true )
                    continue;

                double originalX = element.X;
                double originalWidth = element.Width;

                ApplyElementAlignment( definition, anchor.Element, element, alignment );

                ReportDetailHeaderSynchronizer.SyncMatchingPageHeaderForDetailElement(
                    definition,
                    item.SectionIndex,
                    item.SectionIndex,
                    element,
                    originalX,
                    originalWidth,
                    element.X,
                    element.Width,
                    selectedElementKeys );

                affectedSectionIndexes.Add( item.SectionIndex );
            }

            foreach ( int sectionIndex in affectedSectionIndexes )
            {
                ReportLayoutGeometry.GrowSectionToFitElements( definition.Sections[sectionIndex] );
            }

            SelectElements( selectedElementKeys, anchor.ElementKey );

            return Task.CompletedTask;
        }, refreshSurface: false ) );
    }

    private async Task SizeSelectedElementsAsync( ReportElementSizeMode sizeMode )
    {
        List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( EffectiveDefinition );

        if ( selectedElements.Count < DesignerConstants.MinimumBatchElementCount )
            return;

        string commandName = $"Size {GetSizeDisplayName( sizeMode )}";

        await ExecuteDesignerCommandAsync( new( commandName, () =>
        {
            ReportDefinition definition = EffectiveDefinition;
            List<ReportSelectedElementContext> selectedElements = GetSelectedElementContexts( definition );

            if ( selectedElements.Count < DesignerConstants.MinimumBatchElementCount )
                return Task.CompletedTask;

            ReportSelectedElementContext anchor = selectedElements[0];
            List<string> selectedElementKeys = selectedElements.Select( item => item.ElementKey ).ToList();
            HashSet<int> affectedSectionIndexes = [];

            foreach ( ReportSelectedElementContext item in selectedElements.Skip( 1 ) )
            {
                ReportElementDefinition element = item.Element;

                if ( element.Suppress?.Value == true )
                    continue;

                double originalX = element.X;
                double originalWidth = element.Width;
                double originalHeight = element.Height;

                ApplyElementSize( definition, anchor.Element, element, sizeMode );
                ReportDefinitionHelper.ScaleTableLayout( element, originalWidth, originalHeight );

                ReportDetailHeaderSynchronizer.SyncMatchingPageHeaderForDetailElement(
                    definition,
                    item.SectionIndex,
                    item.SectionIndex,
                    element,
                    originalX,
                    originalWidth,
                    element.X,
                    element.Width,
                    selectedElementKeys );

                affectedSectionIndexes.Add( item.SectionIndex );
            }

            foreach ( int sectionIndex in affectedSectionIndexes )
            {
                ReportLayoutGeometry.GrowSectionToFitElements( definition.Sections[sectionIndex] );
            }

            SelectElements( selectedElementKeys, anchor.ElementKey );

            return Task.CompletedTask;
        }, refreshSurface: false ) );
    }

    private void ApplyElementAlignment( ReportDefinition definition, ReportElementDefinition anchor, ReportElementDefinition element, ReportElementAlignment alignment )
    {
        switch ( alignment )
        {
            case ReportElementAlignment.Tops:
                element.Y = Math.Max( 0, anchor.Y );
                break;
            case ReportElementAlignment.Middles:
                element.Y = Math.Max( 0, anchor.Y + ( anchor.Height - element.Height ) / 2 );
                break;
            case ReportElementAlignment.Bottoms:
                element.Y = Math.Max( 0, anchor.Y + anchor.Height - element.Height );
                break;
            case ReportElementAlignment.Baseline:
                element.Y = Math.Max( 0, anchor.Y + GetElementBaselineOffset( anchor ) - GetElementBaselineOffset( element ) );
                break;
            case ReportElementAlignment.Lefts:
                element.X = ClampElementX( definition, element, anchor.X );
                break;
            case ReportElementAlignment.Centers:
                element.X = ClampElementX( definition, element, anchor.X + ( anchor.Width - element.Width ) / 2 );
                break;
            case ReportElementAlignment.Rights:
                element.X = ClampElementX( definition, element, anchor.X + anchor.Width - element.Width );
                break;
            case ReportElementAlignment.ToGrid:
                element.Width = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, ApplyDesignerGrid( element.Width, true ) );
                element.Height = Math.Max( ReportLayoutGeometry.GetMinimumElementHeight( element ), ApplyDesignerGrid( element.Height, true ) );
                element.X = ClampElementX( definition, element, ApplyDesignerGrid( element.X, true ) );
                element.Y = ApplyDesignerGrid( element.Y, true );
                break;
        }
    }

    private static void ApplyElementSize( ReportDefinition definition, ReportElementDefinition anchor, ReportElementDefinition element, ReportElementSizeMode sizeMode )
    {
        switch ( sizeMode )
        {
            case ReportElementSizeMode.SameWidth:
                element.Width = ClampElementWidth( definition, element, anchor.Width );
                break;
            case ReportElementSizeMode.SameHeight:
                element.Height = Math.Max( ReportLayoutGeometry.GetMinimumElementHeight( element ), anchor.Height );
                break;
            case ReportElementSizeMode.SameSize:
                element.Width = ClampElementWidth( definition, element, anchor.Width );
                element.Height = Math.Max( ReportLayoutGeometry.GetMinimumElementHeight( element ), anchor.Height );
                break;
        }
    }

    private List<ReportSelectedElementContext> GetSelectedElementContexts( ReportDefinition definition )
    {
        if ( definition is null )
            return [];

        List<string> elementKeys = selectionManager.SelectedElementKeys.Count > 0
            ? selectionManager.SelectedElementKeys.ToList()
            : string.IsNullOrWhiteSpace( selectionManager.SelectedElementKey ) ? [] : [selectionManager.SelectedElementKey];

        if ( !string.IsNullOrWhiteSpace( selectionManager.SelectedElementKey ) && elementKeys.Remove( selectionManager.SelectedElementKey ) )
            elementKeys.Insert( 0, selectionManager.SelectedElementKey );

        List<ReportSelectedElementContext> selectedElements = [];

        foreach ( string elementKey in elementKeys.Distinct( StringComparer.Ordinal ) )
        {
            if ( ReportDefinitionHelper.TryFindElementLocation( definition, elementKey, out int sectionIndex, out _, out ReportElementDefinition element ) )
            {
                selectedElements.Add( new()
                {
                    ElementKey = elementKey,
                    SectionIndex = sectionIndex,
                    Element = element,
                } );
            }
        }

        return selectedElements;
    }

    private static double ClampElementX( ReportDefinition definition, ReportElementDefinition element, double x )
    {
        double pageWidth = definition?.Page?.Width ?? DesignerConstants.DefaultPageWidthFallback;
        double maximum = Math.Max( 0, pageWidth - element.Width );

        return ReportLayoutGeometry.Clamp( x, 0, maximum );
    }

    private static double ClampElementWidth( ReportDefinition definition, ReportElementDefinition element, double width )
    {
        double pageWidth = definition?.Page?.Width ?? DesignerConstants.DefaultPageWidthFallback;
        double maximum = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, pageWidth - element.X );

        return ReportLayoutGeometry.Clamp( width, ReportLayoutGeometry.DefaultMinimumElementSize, maximum );
    }

    private static double GetElementBaselineOffset( ReportElementDefinition element )
    {
        if ( element?.Type is ReportElementType.Text or ReportElementType.Field )
        {
            double fontSize = element.Font?.Size ?? Math.Min( DesignerConstants.DefaultDroppedFieldHeight, element.Height );

            return Math.Min( element.Height, fontSize * DesignerConstants.ElementBaselineFontRatio );
        }

        return element?.Height ?? 0;
    }

    private static string GetAlignmentDisplayName( ReportElementAlignment alignment )
    {
        return alignment switch
        {
            ReportElementAlignment.Tops => "tops",
            ReportElementAlignment.Middles => "middles",
            ReportElementAlignment.Bottoms => "bottoms",
            ReportElementAlignment.Baseline => "baseline",
            ReportElementAlignment.Lefts => "lefts",
            ReportElementAlignment.Centers => "centers",
            ReportElementAlignment.Rights => "rights",
            ReportElementAlignment.ToGrid => "to grid",
            _ => alignment.ToString(),
        };
    }

    private static string GetSizeDisplayName( ReportElementSizeMode sizeMode )
    {
        return sizeMode switch
        {
            ReportElementSizeMode.SameWidth => "same width",
            ReportElementSizeMode.SameHeight => "same height",
            ReportElementSizeMode.SameSize => "same size",
            _ => sizeMode.ToString(),
        };
    }

    private async Task UpdateSelectedElementAsync( Action<ReportElementDefinition> update )
    {
        var element = selectionManager.FindSelectedElement( EffectiveDefinition );

        if ( element is null )
            return;

        await ExecuteDesignerCommandAsync( new( "Update element", () =>
        {
            var definition = EffectiveDefinition;
            var element = selectionManager.FindSelectedElement( definition );

            if ( element is not null )
            {
                ReportDefinitionHelper.TryFindElementLocation( definition, ReportDefinitionHelper.EnsureElementId( element ), out var sectionIndex, out _, out _ );
                var originalX = element.X;
                var originalWidth = element.Width;
                var originalHeight = element.Height;

                update?.Invoke( element );

                ReportDefinitionHelper.ScaleTableLayout( element, originalWidth, originalHeight );
                ReportDetailHeaderSynchronizer.SyncMatchingPageHeaderForDetailElement( definition, sectionIndex, sectionIndex, element, originalX, originalWidth, element.X, element.Width );
            }

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

            if ( selectedElements.Count == 0 )
                return Task.CompletedTask;

            List<string> selectedElementKeys = selectedElements.Select( item => item.ElementKey ).ToList();
            string primaryElementKey = selectedElements[0].ElementKey;

            foreach ( ReportSelectedElementContext item in selectedElements )
            {
                update?.Invoke( item.Element );
            }

            SelectElements( selectedElementKeys, primaryElementKey );

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
            || !CanContextSectionInsertSection( definition, selectedSectionIndex ) )
            return;

        await ExecuteDesignerCommandAsync( new( insertAfter ? "Insert band after" : "Insert band before", () =>
        {
            var definition = EffectiveDefinition;
            var sourceSection = selectionManager.FindSelectedSection( definition );

            if ( selectionManager.SelectedSectionIndex is not { } selectedSectionIndex
                || !CanContextSectionInsertSection( definition, selectedSectionIndex ) )
                return Task.CompletedTask;

            var insertIndex = insertAfter ? selectedSectionIndex + 1 : selectedSectionIndex;
            var section = new ReportSectionDefinition
            {
                Name = ReportDefinitionHelper.CreateUniqueSectionName( definition, $"{ReportDefinitionHelper.GetSectionTypeDisplayName( sourceSection.Type )} band" ),
                Type = sourceSection.Type,
                Layout = sourceSection.Layout,
                Height = sourceSection.Height,
                DataSource = sourceSection.DataSource,
                GroupBy = sourceSection.GroupBy,
                Default = false,
                Suppressed = false,
                ReserveSpaceWhenSuppressed = sourceSection.ReserveSpaceWhenSuppressed,
                PrintOnFirstPage = sourceSection.PrintOnFirstPage,
                PrintOnLastPage = sourceSection.PrintOnLastPage,
                RepeatOnEveryPage = sourceSection.RepeatOnEveryPage,
                KeepTogether = ReportValue.Create( sourceSection.KeepTogether?.Value ?? false, sourceSection.KeepTogether?.Formula ),
                NewPageBefore = ReportValue.Create( sourceSection.NewPageBefore?.Value ?? false, sourceSection.NewPageBefore?.Formula ),
                NewPageAfter = ReportValue.Create( sourceSection.NewPageAfter?.Value ?? false, sourceSection.NewPageAfter?.Formula ),
                Appearance = new()
                {
                    BackgroundColor = sourceSection.Appearance?.BackgroundColor ?? ReportColor.Default,
                    Opacity = sourceSection.Appearance?.Opacity,
                },
                Border = new()
                {
                    Color = sourceSection.Border?.Color ?? ReportColor.Default,
                    Width = sourceSection.Border?.Width,
                    Radius = sourceSection.Border?.Radius,
                },
            };

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

            var section = definition.Sections[selectionManager.SelectedSectionIndex.Value];

            if ( !ReportDefinitionHelper.CanDeleteSection( section ) )
                return Task.CompletedTask;

            collapsedSectionIds.Remove( ReportDefinitionHelper.EnsureSectionId( section ) );
            definition.Sections.RemoveAt( selectionManager.SelectedSectionIndex.Value );

            if ( definition.Sections.Count == 0 )
            {
                SelectReport();
            }
            else
            {
                SelectSection( Math.Min( selectionManager.SelectedSectionIndex.Value, definition.Sections.Count - 1 ) );
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
        if ( contextMenu?.SectionIndex is not { } sectionIndex )
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
                section.Suppressed = suppressed;

                if ( suppressed )
                {
                    collapsedSectionIds.Remove( ReportDefinitionHelper.EnsureSectionId( section ) );
                }
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
        var definition = EffectiveDefinition;

        var elementKeys = selectionManager.GetSelectedElementIds( definition ).ToList();

        if ( elementKeys.Count == 0 )
            return;

        await ExecuteDesignerCommandAsync( new( elementKeys.Count == 1 ? "Delete element" : "Delete elements", () =>
        {
            var definition = EffectiveDefinition;
            var selectedIds = selectionManager.GetSelectedElementIds( definition ).ToHashSet( StringComparer.Ordinal );
            var lastSectionIndex = ReportDefinitionHelper.RemoveElementsByIds( definition, selectedIds );

            if ( lastSectionIndex >= 0 )
                SelectSection( lastSectionIndex );

            CloseContextMenu();
            editingElementKey = null;

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
        lastDragPreviewRenderTime = DateTime.MinValue;
        editingElementKey = null;
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
        lastDragPreviewRenderTime = DateTime.MinValue;
        editingElementKey = null;
        selectionBox = null;
        elementPointerDrag = null;
        elementPointerResize = null;
    }

    private bool IsExternalDesignerDragActive()
    {
        return draggedKind is ReportDesignerDragKind.Field or ReportDesignerDragKind.ToolboxElement;
    }

    private void BeginElementPointerDrag( string elementKey, PointerEventArgs eventArgs )
    {
        if ( eventArgs.CtrlKey )
        {
            ToggleElementSelection( elementKey );
            suppressNextElementClickKey = elementKey;
            return;
        }

        if ( !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, elementKey, out var sectionIndex, out _, out var element )
            || element.Suppress?.Value == true )
            return;

        draggedKind = ReportDesignerDragKind.Element;
        draggedElementKey = elementKey;
        draggedElement = element;
        draggedDataSourceName = null;
        draggedFieldName = null;
        draggedElementType = null;
        draggedElementText = null;
        dragPreview = null;
        lastDragPreviewRenderTime = DateTime.MinValue;
        elementPointerDrag = new()
        {
            ElementKey = elementKey,
            SourceSectionIndex = sectionIndex,
            TargetSectionIndex = sectionIndex,
            OriginalX = element.X,
            OriginalY = element.Y,
            StartClientX = eventArgs.ClientX,
            StartClientY = eventArgs.ClientY,
            PointerOffsetX = ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetX ),
            PointerOffsetY = ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetY ),
            TargetX = element.X,
            TargetY = element.Y,
            SnapToGrid = IsSnapToGridEnabled( element ),
            SelectedElements = CaptureElementPointerItems( EffectiveDefinition, elementKey ).ToList(),
        };

        SelectElement( elementKey, preserveSelection: selectionManager.IsElementSelected( elementKey ) && selectionManager.SelectedElementKeys.Count > 1 );
    }

    private void BeginElementPointerResize( string elementKey, ReportElementResizeHandle handle, PointerEventArgs eventArgs )
    {
        if ( !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, elementKey, out var sectionIndex, out _, out var element )
            || element.Suppress?.Value == true )
            return;

        draggedKind = ReportDesignerDragKind.Element;
        draggedElementKey = elementKey;
        draggedElement = element;
        draggedDataSourceName = null;
        draggedFieldName = null;
        draggedElementType = null;
        draggedElementText = null;
        dragPreview = null;
        lastDragPreviewRenderTime = DateTime.MinValue;
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
            MinimumHeight = ReportLayoutGeometry.GetMinimumElementHeight( element ),
            SnapToGrid = IsSnapToGridEnabled( element ),
            SelectedElements = CaptureElementPointerItems( EffectiveDefinition, elementKey ).ToList(),
        };

        SelectElement( elementKey, preserveSelection: selectionManager.IsElementSelected( elementKey ) && selectionManager.SelectedElementKeys.Count > 1 );
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

        await StartDocumentSectionResizeAsync( eventArgs.ClientY, eventArgs.PointerId );
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

        var section = ReportLayoutGeometry.GetSection( EffectiveDefinition, sectionIndex );

        if ( section is null || section.Suppressed )
            return;

        CloseContextMenu();

        var x = ReportLayoutGeometry.Clamp( ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetX ), 0, EffectiveDefinition.Page.Width );
        var y = ReportLayoutGeometry.Clamp( GetSectionOffsetY( EffectiveDefinition, sectionIndex ) + ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetY ), 0, GetDesignerContentHeight( EffectiveDefinition ) );

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

        lastSelectionBoxRenderTime = DateTime.MinValue;
    }

    private async Task PreviewSelectionBoxAsync( PointerEventArgs eventArgs )
    {
        if ( selectionBox is null )
            return;

        double previousX = selectionBox.X;
        double previousY = selectionBox.Y;
        double previousWidth = selectionBox.Width;
        double previousHeight = selectionBox.Height;

        UpdateSelectionBox( eventArgs );

        if ( !CanRenderSelectionBoxPreview( previousX, previousY, previousWidth, previousHeight ) )
            return;

        await UpdateDesignerSelectionOverlayAsync();
    }

    private Task PreviewPageSelectionBoxAsync( PointerEventArgs eventArgs )
    {
        return selectionBox is null
            ? Task.CompletedTask
            : PreviewSelectionBoxAsync( eventArgs );
    }

    private async Task CompleteSelectionBoxAsync( PointerEventArgs eventArgs )
    {
        if ( selectionBox is null )
            return;

        UpdateSelectionBox( eventArgs );

        var completedSelectionBox = selectionBox;
        selectionBox = null;
        lastSelectionBoxRenderTime = DateTime.MinValue;
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
        return selectionBox is null
            ? Task.CompletedTask
            : CompleteSelectionBoxAsync( eventArgs );
    }

    private async Task CancelSelectionBoxAsync()
    {
        selectionBox = null;
        lastSelectionBoxRenderTime = DateTime.MinValue;
        await ClearDesignerInteractionOverlaysAsync();

        await InvokeAsync( StateHasChanged );
    }

    private Task CancelPageSelectionBoxAsync()
    {
        return selectionBox is null
            ? Task.CompletedTask
            : CancelSelectionBoxAsync();
    }

    private bool CanRenderSelectionBoxPreview( double previousX, double previousY, double previousWidth, double previousHeight )
    {
        if ( selectionBox is null )
            return false;

        if ( Math.Abs( selectionBox.X - previousX ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( selectionBox.Y - previousY ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( selectionBox.Width - previousWidth ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( selectionBox.Height - previousHeight ) < DesignerConstants.DragPreviewChangeTolerance )
        {
            return false;
        }

        DateTime now = DateTime.UtcNow;

        if ( now - lastSelectionBoxRenderTime < DesignerConstants.SelectionBoxFrameThrottle )
            return false;

        lastSelectionBoxRenderTime = now;

        return true;
    }

    private void UpdateSelectionBox( PointerEventArgs eventArgs )
    {
        if ( selectionBox is null )
            return;

        selectionBox.CurrentX = ReportLayoutGeometry.Clamp( selectionBox.StartX + ReportMeasurementConverter.FromCssPixelValue( eventArgs.ClientX - selectionBox.StartClientX ), 0, EffectiveDefinition.Page.Width );
        selectionBox.CurrentY = ReportLayoutGeometry.Clamp( selectionBox.StartY + ReportMeasurementConverter.FromCssPixelValue( eventArgs.ClientY - selectionBox.StartClientY ), 0, GetDesignerContentHeight( EffectiveDefinition ) );
        selectionBox.HasMoved = selectionBox.HasMoved
            || Math.Abs( ReportMeasurementConverter.ToCssPixelValue( selectionBox.CurrentX - selectionBox.StartX ) ) > 2
            || Math.Abs( ReportMeasurementConverter.ToCssPixelValue( selectionBox.CurrentY - selectionBox.StartY ) ) > 2;
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
            && Math.Abs( dragPreview.X - preview.X ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( dragPreview.Y - preview.Y ) < DesignerConstants.DragPreviewChangeTolerance;

        if ( samePreviewPosition )
            return;

        var now = DateTime.UtcNow;

        if ( !elementPointerDrag.SnapToGrid
            && dragPreview is not null
            && dragPreview.SectionIndex == preview.SectionIndex
            && now - lastDragPreviewRenderTime < DesignerConstants.DragPreviewFrameThrottle )
        {
            return;
        }

        elementPointerDrag.TargetSectionIndex = preview.SectionIndex;
        elementPointerDrag.TargetX = preview.X;
        elementPointerDrag.TargetY = preview.Y;
        elementPointerDrag.HasMoved = true;
        dragPreview = preview;
        lastDragPreviewRenderTime = now;

        await UpdateDesignerDragOverlayAsync( preview );
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
            dragPreview = null;
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
        ReplaceTableCellElement( tableCellDropTarget.Table, tableCellDropTarget.Cell, element );
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

        if ( !TryFindTableCellAt( definition.Sections[pointerDrag.TargetSectionIndex], pointerX, pointerY, out target ) )
            return false;

        return !ReportDefinitionHelper.TryFindElementLocation( definition, pointerDrag.ElementKey, out ReportElementLocation location )
            || !ReferenceEquals( target.Table, location.Element );
    }

    private static void ReplaceTableCellElement( ReportElementDefinition table, ReportTableCellDefinition cell, ReportElementDefinition element )
    {
        if ( cell is null || element is null )
            return;

        cell.Elements.Clear();
        ReportDefinitionHelper.FitElementToTableCell( table, cell, element );
        cell.Elements.Add( element );
    }

    private async Task CancelElementPointerDragAsync()
    {
        if ( elementPointerDrag is null )
            return;

        ClearDragState();
        await ClearDesignerInteractionOverlaysAsync();

        await InvokeAsync( StateHasChanged );
    }

    private ReportDesignerDragPreview CreateElementPointerDragPreview( int targetSectionIndex, PointerEventArgs eventArgs )
    {
        return ReportDesignerInteractionService.CreateElementDragPreview(
            EffectiveDefinition,
            elementPointerDrag,
            draggedElement,
            targetSectionIndex,
            eventArgs.ClientX,
            eventArgs.ClientY,
            sectionIndex => GetSectionOffsetY( EffectiveDefinition, sectionIndex ),
            value => ApplyDesignerGrid( value, elementPointerDrag?.SnapToGrid ?? snapToGrid ) );
    }

    private async Task PreviewElementPointerResizeAsync( PointerEventArgs eventArgs )
    {
        if ( elementPointerResize is null || draggedElement is null || draggedKind != ReportDesignerDragKind.Element )
            return;

        var preview = CreateElementPointerResizePreview( eventArgs );

        if ( preview is null )
            return;

        var samePreviewSize = dragPreview is not null
            && Math.Abs( dragPreview.X - preview.X ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( dragPreview.Y - preview.Y ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( dragPreview.Width - preview.Width ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( dragPreview.Height - preview.Height ) < DesignerConstants.DragPreviewChangeTolerance;

        if ( samePreviewSize )
            return;

        var now = DateTime.UtcNow;

        if ( !elementPointerResize.SnapToGrid
            && dragPreview is not null
            && now - lastDragPreviewRenderTime < DesignerConstants.DragPreviewFrameThrottle )
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

        await UpdateDesignerDragOverlayAsync( preview );
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
                    ReportDefinitionHelper.ScaleTableLayout( resizedElement, item.OriginalWidth, item.OriginalHeight );
            }

            SelectElements( pointerResize.SelectedElements.Select( item => item.ElementKey ), pointerResize.ElementKey );
            SuppressNextSelectionClick();
            dragPreview = null;
            ClearDragState();

            return Task.CompletedTask;
        }, refreshSurface: false ) );
    }

    private async Task CancelElementPointerResizeAsync()
    {
        if ( elementPointerResize is null )
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
        if ( sectionPointerResize is null )
            return;

        var height = CreateSectionPointerResizeHeight( clientY );

        if ( Math.Abs( sectionPointerResize.TargetHeight - height ) < DesignerConstants.DragPreviewChangeTolerance )
            return;

        sectionPointerResize.TargetHeight = height;

        await UpdateDesignerSectionResizePreviewAsync( sectionPointerResize );
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
        if ( sectionPointerResize is null )
            return;

        sectionPointerResize = null;
        await ClearDesignerSectionResizePreviewAsync();

        await InvokeAsync( StateHasChanged );
    }

    private double CreateSectionPointerResizeHeight( double clientY )
    {
        if ( sectionPointerResize is null )
            return 0;

        return CreateSectionPointerResizeHeight( sectionPointerResize, clientY );
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

    private async Task StartDocumentSectionResizeAsync( double startClientY, long pointerId )
    {
        EnsureReportingModule();
        dotNetObjectReference ??= DotNetObjectReference.Create( this );

        await DocumentObserver.EnsureInitializedAsync();
        await reportingModule.StartSectionResize( dotNetObjectReference, startClientY, pointerId );
    }

    private void EnsureReportingModule()
    {
        reportingModule ??= new( JSRuntime, VersionProvider, BlazoriseOptions );
    }

    private async Task UpdateDesignerSelectionOverlayAsync()
    {
        if ( selectionBox is null )
            return;

        EnsureReportingModule();

        await reportingModule.UpdateDesignerSelectionOverlay(
            designerPageRef.Element,
            ReportMeasurementConverter.ToCssPixelValue( selectionBox.X ) + GetSelectionBoxLeftOffset(),
            ReportMeasurementConverter.ToCssPixelValue( selectionBox.Y ),
            ReportMeasurementConverter.ToCssPixelValue( selectionBox.Width ),
            ReportMeasurementConverter.ToCssPixelValue( selectionBox.Height ) );
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
            ReportMeasurementConverter.ToCssPixelValue( GetSectionOffsetY( EffectiveDefinition, preview.SectionIndex ) + preview.Y ),
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

    private ReportDesignerDragPreview CreateElementPointerResizePreview( PointerEventArgs eventArgs )
    {
        return ReportDesignerInteractionService.CreateElementResizePreview(
            EffectiveDefinition,
            elementPointerResize,
            draggedElement,
            eventArgs.ClientX,
            eventArgs.ClientY,
            value => ApplyDesignerGrid( value, elementPointerResize?.SnapToGrid ?? snapToGrid ) );
    }

    private async Task PreviewDesignerDragAsync( int targetSectionIndex, ElementReference sectionBodyElement, DragEventArgs eventArgs )
    {
        if ( draggedKind == ReportDesignerDragKind.None )
            return;

        var offset = await GetDesignerDragOffsetAsync( sectionBodyElement, eventArgs );
        bool useSnapToGrid = draggedKind == ReportDesignerDragKind.Element
            ? IsSnapToGridEnabled( draggedElement )
            : snapToGrid;
        var preview = CreateDragPreview( targetSectionIndex, ApplyDesignerGrid( offset.X, useSnapToGrid ), ApplyDesignerGrid( offset.Y, useSnapToGrid ) );

        if ( preview is null )
            return;

        var samePreviewPosition = dragPreview is not null
            && dragPreview.SectionIndex == preview.SectionIndex
            && Math.Abs( dragPreview.X - preview.X ) < DesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( dragPreview.Y - preview.Y ) < DesignerConstants.DragPreviewChangeTolerance;

        if ( samePreviewPosition )
        {
            return;
        }

        var now = DateTime.UtcNow;

        if ( !snapToGrid
            && dragPreview is not null
            && dragPreview.SectionIndex == preview.SectionIndex
            && now - lastDragPreviewRenderTime < DesignerConstants.DragPreviewFreeDropThrottle )
        {
            return;
        }

        dragPreview = preview;
        lastDragPreviewRenderTime = now;

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
        bool useSnapToGrid = draggedKind == ReportDesignerDragKind.Element
            ? IsSnapToGridEnabled( draggedElement )
            : snapToGrid;
        var x = ApplyDesignerGrid( offset.X, useSnapToGrid );
        var y = ApplyDesignerGrid( offset.Y, useSnapToGrid );
        var tableDropTarget = TryFindTableCellAt( definition.Sections[targetSectionIndex], x, y, out ReportTableCellDropTarget cellDropTarget )
            ? cellDropTarget
            : null;
        var fieldDropTarget = draggedKind == ReportDesignerDragKind.Field
            ? FindTextElementAt( definition.Sections[targetSectionIndex], x, y )
            : null;

        var commandName = draggedKind switch
        {
            ReportDesignerDragKind.Field when !string.IsNullOrWhiteSpace( draggedFieldName ) && tableDropTarget is not null => "Add field to table cell",
            ReportDesignerDragKind.Field when !string.IsNullOrWhiteSpace( draggedFieldName ) && fieldDropTarget is not null => "Insert field into text",
            ReportDesignerDragKind.Field when !string.IsNullOrWhiteSpace( draggedFieldName ) => "Add field",
            ReportDesignerDragKind.ToolboxElement when draggedElementType is not null && tableDropTarget is not null => "Add element to table cell",
            ReportDesignerDragKind.ToolboxElement when draggedElementType is not null => "Add element",
            ReportDesignerDragKind.Element when tableDropTarget is not null && ReportDefinitionHelper.TryFindElementLocation( definition, draggedElementKey, out _, out _, out _ ) => "Move element to table cell",
            ReportDesignerDragKind.Element when ReportDefinitionHelper.TryFindElementLocation( definition, draggedElementKey, out _, out _, out _ ) => "Move element",
            _ => null,
        };

        if ( commandName is null )
            return;

        await ClearDesignerInteractionOverlaysAsync();

        await ExecuteDesignerCommandAsync( new( commandName, () =>
        {
            var definition = EffectiveDefinition;
            var targetSection = definition.Sections[targetSectionIndex];
            TryFindTableCellAt( targetSection, x, y, out ReportTableCellDropTarget tableCellDropTarget );

            switch ( draggedKind )
            {
                case ReportDesignerDragKind.Field when !string.IsNullOrWhiteSpace( draggedFieldName ):
                    var fieldBinding = ReportDefinitionHelper.NormalizeFieldBindingForSection( definition, targetSection, draggedDataSourceName, draggedFieldName );

                    var fieldElement = new ReportElementDefinition
                    {
                        Name = fieldBinding.FieldName,
                        Type = ReportElementType.Field,
                        Field = fieldBinding.FieldName,
                        DataSource = fieldBinding.DataSourceName,
                        X = tableCellDropTarget?.X ?? x,
                        Y = tableCellDropTarget?.Y ?? y,
                        Width = DesignerConstants.DefaultDroppedFieldWidth,
                        Height = DesignerConstants.DefaultDroppedFieldHeight,
                    };

                    if ( tableCellDropTarget is not null )
                    {
                        ReplaceTableCellElement( tableCellDropTarget.Table, tableCellDropTarget.Cell, fieldElement );
                        SelectTableCell( tableCellDropTarget.Cell.Id );
                    }
                    else
                    {
                        var textDropTarget = FindTextElementAt( targetSection, x, y );

                        if ( textDropTarget is not null )
                        {
                            ReportExpressionFormatter.AppendFieldExpressionToText( textDropTarget, fieldBinding.DataSourceName, fieldBinding.FieldName );
                            SelectElement( ReportDefinitionHelper.EnsureElementId( textDropTarget ) );
                            break;
                        }

                        targetSection.Elements.Add( fieldElement );

                        if ( !ReportSpecialFieldResolver.IsSpecialDataSource( fieldBinding.DataSourceName )
                        && !ReportFormulaFieldResolver.IsFormulaDataSource( fieldBinding.DataSourceName )
                        && !ReportRunningTotalResolver.IsRunningTotalDataSource( fieldBinding.DataSourceName ) )
                            ReportDetailHeaderSynchronizer.AddPageHeaderForDetailField( definition, targetSectionIndex, targetSection, fieldBinding.FieldName, x, fieldElement.Width );

                        SelectElement( ReportDefinitionHelper.EnsureElementId( fieldElement ) );
                    }
                    break;
                case ReportDesignerDragKind.ToolboxElement when draggedElementType is not null:
                    var toolboxElement = ReportDefinitionHelper.CreateElementFromToolbox( draggedElementType.Value, draggedElementText, tableCellDropTarget?.X ?? x, tableCellDropTarget?.Y ?? y );

                    if ( tableCellDropTarget is not null )
                    {
                        ReplaceTableCellElement( tableCellDropTarget.Table, tableCellDropTarget.Cell, toolboxElement );
                        SelectTableCell( tableCellDropTarget.Cell.Id );
                    }
                    else
                    {
                        targetSection.Elements.Add( toolboxElement );
                        SelectElement( ReportDefinitionHelper.EnsureElementId( toolboxElement ) );
                    }
                    break;
                case ReportDesignerDragKind.Element when ReportDefinitionHelper.TryFindElementLocation( definition, draggedElementKey, out ReportElementLocation location ):
                    var sourceSectionIndex = location.SectionIndex;
                    var element = location.Element;
                    var originalX = element.X;
                    var originalWidth = element.Width;

                    location.OwnerElements.RemoveAt( location.ElementIndex );
                    element.X = tableCellDropTarget?.X ?? x;
                    element.Y = tableCellDropTarget?.Y ?? y;

                    if ( tableCellDropTarget is not null )
                    {
                        ReplaceTableCellElement( tableCellDropTarget.Table, tableCellDropTarget.Cell, element );
                        SelectTableCell( tableCellDropTarget.Cell.Id );
                    }
                    else
                    {
                        targetSection.Elements.Add( element );
                        ReportDetailHeaderSynchronizer.SyncMatchingPageHeaderForDetailElement( definition, sourceSectionIndex, targetSectionIndex, element, originalX, originalWidth, element.X, element.Width );
                        SelectElement( ReportDefinitionHelper.EnsureElementId( element ) );
                    }
                    break;
            }

            selectionManager.SelectedSectionIndex = null;
            dragPreview = null;
            ClearDragState();

            return Task.CompletedTask;
        } ) );
    }

    private static bool TryFindTableCellAt( ReportSectionDefinition section, double x, double y, out ReportTableCellDropTarget target )
    {
        target = null;

        if ( section?.Elements is null )
            return false;

        for ( int elementIndex = section.Elements.Count - 1; elementIndex >= 0; elementIndex-- )
        {
            ReportElementDefinition table = section.Elements[elementIndex];

            if ( table.Type != ReportElementType.Table
                || table.Suppress?.Value == true
                || x < table.X
                || x > table.X + table.Width
                || y < table.Y
                || y > table.Y + table.Height )
            {
                continue;
            }

            ReportDefinitionHelper.EnsureTableLayout(
                table,
                table.Rows?.Count > 0 ? table.Rows.Count : 2,
                table.Columns?.Count > 0 ? table.Columns.Count : 2 );

            double localX = x - table.X;
            double localY = y - table.Y;

            foreach ( ReportTableCellDefinition cell in table.Cells.OrderBy( cell => cell.RowIndex ).ThenBy( cell => cell.ColumnIndex ) )
            {
                double cellX = GetTableColumnOffset( table, cell.ColumnIndex );
                double cellY = GetTableRowOffset( table, cell.RowIndex );
                double cellWidth = ReportDefinitionHelper.GetTableCellWidth( table, cell );
                double cellHeight = ReportDefinitionHelper.GetTableCellHeight( table, cell );

                if ( localX >= cellX
                    && localX <= cellX + cellWidth
                    && localY >= cellY
                    && localY <= cellY + cellHeight )
                {
                    target = new()
                    {
                        Table = table,
                        Cell = cell,
                        X = Math.Max( 0, localX - cellX ),
                        Y = Math.Max( 0, localY - cellY ),
                    };

                    return true;
                }
            }
        }

        return false;
    }

    private static bool CanMergeTableCellRight( ReportElementDefinition table, ReportTableCellDefinition cell )
    {
        if ( table?.Columns is null || cell is null )
            return false;

        int targetColumnIndex = cell.ColumnIndex + Math.Max( 1, cell.ColumnSpan );

        if ( targetColumnIndex >= table.Columns.Count )
            return false;

        int rowSpan = Math.Max( 1, cell.RowSpan );

        for ( int rowIndex = cell.RowIndex; rowIndex < cell.RowIndex + rowSpan; rowIndex++ )
        {
            ReportTableCellDefinition targetCell = table.Cells?.FirstOrDefault( item => item.RowIndex == rowIndex && item.ColumnIndex == targetColumnIndex );

            if ( targetCell is null || targetCell.RowSpan != 1 || targetCell.ColumnSpan != 1 )
                return false;
        }

        return true;
    }

    private static bool CanMergeTableCellDown( ReportElementDefinition table, ReportTableCellDefinition cell )
    {
        if ( table?.Rows is null || cell is null )
            return false;

        int targetRowIndex = cell.RowIndex + Math.Max( 1, cell.RowSpan );

        if ( targetRowIndex >= table.Rows.Count )
            return false;

        int columnSpan = Math.Max( 1, cell.ColumnSpan );

        for ( int columnIndex = cell.ColumnIndex; columnIndex < cell.ColumnIndex + columnSpan; columnIndex++ )
        {
            ReportTableCellDefinition targetCell = table.Cells?.FirstOrDefault( item => item.RowIndex == targetRowIndex && item.ColumnIndex == columnIndex );

            if ( targetCell is null || targetCell.RowSpan != 1 || targetCell.ColumnSpan != 1 )
                return false;
        }

        return true;
    }

    private static double GetTableColumnOffset( ReportElementDefinition table, int columnIndex )
    {
        return table.Columns.Take( Math.Max( 0, columnIndex ) ).Sum( column => Math.Max( 1, column.Width ) );
    }

    private static double GetTableRowOffset( ReportElementDefinition table, int rowIndex )
    {
        return table.Rows.Take( Math.Max( 0, rowIndex ) ).Sum( row => Math.Max( 1, row.Height ) );
    }

    private static ReportElementDefinition FindTextElementAt( ReportSectionDefinition section, double x, double y )
    {
        if ( section is null )
            return null;

        for ( var i = section.Elements.Count - 1; i >= 0; i-- )
        {
            var element = section.Elements[i];

            if ( element.Suppress?.Value != true
                && element.Type == ReportElementType.Text
                && x >= element.X
                && x <= element.X + element.Width
                && y >= element.Y
                && y <= element.Y + element.Height )
            {
                return element;
            }
        }

        return null;
    }

    private ReportDesignerDragPreview CreateDragPreview( int targetSectionIndex, double x, double y )
    {
        return draggedKind switch
        {
            ReportDesignerDragKind.Field when !string.IsNullOrWhiteSpace( draggedFieldName ) => CreateFieldDragPreview( targetSectionIndex, x, y ),
            ReportDesignerDragKind.ToolboxElement when draggedElementType is not null => CreateDragPreview( targetSectionIndex, ReportDefinitionHelper.CreateElementFromToolbox( draggedElementType.Value, draggedElementText, x, y ) ),
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
        var fieldBinding = ReportDefinitionHelper.NormalizeFieldBindingForSection( EffectiveDefinition, targetSection, draggedDataSourceName, draggedFieldName );

        return new()
        {
            SectionIndex = targetSectionIndex,
            ElementType = ReportElementType.Field,
            Text = ReportExpressionFormatter.FormatFieldExpression( fieldBinding.DataSourceName, fieldBinding.FieldName ),
            X = x,
            Y = y,
            Width = DesignerConstants.DefaultDroppedFieldWidth,
            Height = DesignerConstants.DefaultDroppedFieldHeight,
        };
    }

    private ReportDesignerDragPreview CreateDragPreview( int targetSectionIndex, ReportElementDefinition element, double? x = null, double? y = null )
    {
        return new()
        {
            SectionIndex = targetSectionIndex,
            ElementType = element.Type,
            Text = ReportElementDefinitionHelper.GetDisplayText( EffectiveDefinition, element ),
            X = x ?? element.X,
            Y = y ?? element.Y,
            Width = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, element.Width ),
            Height = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, element.Height ),
        };
    }

    private async Task ClearDesignerDragAsync()
    {
        var requiresRender = draggedKind != ReportDesignerDragKind.None || dragPreview is not null;

        ClearDragState();
        await ClearDesignerInteractionOverlaysAsync();

        if ( requiresRender )
            await InvokeAsync( StateHasChanged );
    }

    private double ApplyDesignerGrid( double value )
    {
        return ApplyDesignerGrid( value, snapToGrid );
    }

    private double ApplyDesignerGrid( double value, bool useSnapToGrid )
    {
        return useSnapToGrid ? ReportLayoutGeometry.SnapToGrid( value ) : Math.Max( 0, value );
    }

    private bool IsSnapToGridEnabled( ReportElementDefinition element )
    {
        return element?.SnapToGrid?.Value ?? snapToGrid;
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
        EnsureDesignerLayoutCache( definition );

        return designerLayoutCache.SectionOffsets is not null && sectionIndex >= 0 && sectionIndex < designerLayoutCache.SectionOffsets.Length
            ? designerLayoutCache.SectionOffsets[sectionIndex]
            : ReportLayoutGeometry.GetSectionOffsetY( definition, sectionIndex, GetDesignerSectionHeightCore );
    }

    private double GetDesignerContentHeight( ReportDefinition definition )
    {
        EnsureDesignerLayoutCache( definition );

        return designerLayoutCache.SectionOffsets is not null
            ? designerLayoutCache.ContentHeight
            : ReportLayoutGeometry.GetContentHeight( definition, GetDesignerSectionHeightCore );
    }

    private double GetDesignerSectionHeight( int sectionIndex, ReportSectionDefinition section )
    {
        return GetDesignerSectionHeightCore( sectionIndex, section );
    }

    private double GetDesignerSectionHeightCore( int sectionIndex, ReportSectionDefinition section )
    {
        if ( sectionPointerResize is not null && sectionPointerResize.SectionIndex == sectionIndex )
            return sectionPointerResize.TargetHeight;

        return BandMode == ReportBandMode.Rail && section is not null && !section.Suppressed && IsSectionCollapsed( section )
            ? ReportMeasurementConverter.FromCssPixelValue( DesignerConstants.DesignerCollapsedBandHeight )
            : section?.Height ?? 0;
    }

    private void EnsureDesignerLayoutCache( ReportDefinition definition )
    {
        if ( definition is null )
            return;

        int resizeSectionIndex = sectionPointerResize?.SectionIndex ?? -1;
        double resizeHeight = sectionPointerResize?.TargetHeight ?? 0;
        int sectionCount = definition.Sections.Count;

        if ( ReferenceEquals( designerLayoutCache.Definition, definition )
             && designerLayoutCache.SectionOffsets is not null
             && designerLayoutCache.BandMode == BandMode
             && designerLayoutCache.CollapsedSectionsVersion == collapsedSectionsVersion
             && designerLayoutCache.SectionCount == sectionCount
             && designerLayoutCache.ResizeSectionIndex == resizeSectionIndex
             && Math.Abs( designerLayoutCache.ResizeHeight - resizeHeight ) < DesignerConstants.DragPreviewChangeTolerance )
        {
            return;
        }

        double[] sectionOffsets = new double[sectionCount];
        double offset = 0;

        for ( int i = 0; i < sectionCount; i++ )
        {
            sectionOffsets[i] = offset;
            offset += GetDesignerSectionHeightCore( i, definition.Sections[i] );
        }

        designerLayoutCache = ( definition, BandMode, collapsedSectionsVersion, sectionCount, resizeSectionIndex, resizeHeight, sectionOffsets, offset );
    }

    private void InvalidateDesignerCaches()
    {
        InvalidateDesignerLayoutCache();
        designerSectionRenderItems.Clear();
        designerSectionRenderItemsCacheKey = default;
    }

    private void RefreshDesignerSurface()
    {
        if ( designerLayoutRef is not null )
            _ = designerLayoutRef.RefreshSurface();
    }

    private void InvalidateDesignerLayoutCache()
    {
        designerLayoutCache = default;
    }

    private static double GetMinimumSectionHeight( ReportSectionDefinition section )
    {
        return ReportLayoutGeometry.GetMinimumSectionHeight( section );
    }

    private void OnSnapToGridChanged( bool value )
    {
        snapToGrid = value;
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
        lastDragPreviewRenderTime = DateTime.MinValue;
        elementPointerDrag = null;
        elementPointerResize = null;
    }

    private bool SupportsPreviewFormat( ReportPreviewFormat format )
    {
        return ( PreviewFormats ?? context.ViewerOptions.PreviewFormats ).HasFlag( format );
    }

    private bool ShouldRenderElement( ReportDefinition definition, ReportSectionDefinition section, ReportElementDefinition element, object item )
    {
        return !ReportValueResolver.ResolveSuppress( element, section, definition, Data, item );
    }

    #endregion

    #region Classes

    private sealed class ReportSelectedElementContext
    {
        internal string ElementKey { get; set; }

        internal int SectionIndex { get; set; }

        internal ReportElementDefinition Element { get; set; }
    }

    private sealed class ReportTableCellDropTarget
    {
        internal ReportElementDefinition Table { get; set; }

        internal ReportTableCellDefinition Cell { get; set; }

        internal double X { get; set; }

        internal double Y { get; set; }
    }

    #endregion

    #region Properties

    private ReportDefinition EffectiveDefinition
        => DefinitionMode == ReportDefinitionMode.AlwaysUseDeclarative
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
}