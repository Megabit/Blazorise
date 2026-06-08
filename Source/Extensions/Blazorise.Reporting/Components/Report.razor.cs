#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Reporting.Internal;
using Microsoft.AspNetCore.Components;
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

    private readonly ReportDesignerCommandManager commandManager = new();

    private readonly ReportSelectionManager selectionManager = new();

    private readonly HashSet<string> collapsedSectionIds = new( StringComparer.Ordinal );

    private DotNetObjectReference<Report<TItem>> dotNetObjectReference;

    private ReportDefinition declarativeDefinition;

    private ReportStudioMode currentMode;

    private ReportPreviewFormat currentPreviewFormat;

    private ReportDesignerPanelTab selectedDesignerPanelTab = ReportDesignerPanelTab.Properties;

    private ReportContextMenuState contextMenu;


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

    private JSReportingModule reportingModule;

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
                Data = Data,
            } );
        }

        definition.Page = ResolvePage( definition.Page );

        return ReportDefinitionHelper.EnsureDefinitionIds( definition );
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

    private Task CloseContextMenuAsync()
    {
        CloseContextMenu();

        return Task.CompletedTask;
    }

    private ReportSectionDefinition GetContextMenuSection( ReportDefinition definition )
    {
        return IsSectionContextMenuVisible( definition )
            ? definition.Sections[contextMenu.SectionIndex]
            : null;
    }

    private bool IsElementContextMenuVisible()
    {
        return contextMenu?.Visible == true
            && contextMenu.Target == ReportContextMenuTarget.Element;
    }

    private bool IsSectionContextMenuVisible( ReportDefinition definition )
    {
        return contextMenu?.Visible == true
            && contextMenu.Target == ReportContextMenuTarget.Section
            && contextMenu.SectionIndex >= 0
            && contextMenu.SectionIndex < definition.Sections.Count;
    }

    private ReportSectionDefinition GetSelectedPropertiesSection( ReportDefinition definition )
    {
        return string.IsNullOrWhiteSpace( selectionManager.SelectedElementKey )
            ? selectionManager.FindSelectedSection( definition )
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

        return Task.CompletedTask;
    }

    private Task OnReportTreeNodeContextMenu( ReportTreeNodeMouseEventArgs eventArgs )
    {
        if ( ReportDesignerTreeBuilder.TryResolveSectionTreeNode( eventArgs.Node, out var sectionIndex ) )
        {
            OpenSectionContextMenu( sectionIndex, eventArgs.MouseEventArgs );
        }
        else if ( ReportDesignerTreeBuilder.TryResolveElementTreeNode( eventArgs.Node, out var elementKey ) )
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

    private IReadOnlyList<object> ResolveSectionRenderItems( ReportDefinition definition, ReportSectionDefinition section, bool designMode )
    {
        var items = !designMode && section.Type == ReportSectionType.Detail
            ? ReportDataResolver.ResolveItems( definition, Data, section.DataSource ).ToList()
            : new List<object> { ReportDataResolver.ResolveItems( definition, Data, section.DataSource ).FirstOrDefault() };

        if ( items.Count == 0 )
            items.Add( null );

        return items;
    }

    private double GetReportPageWidth( ReportDefinition definition, bool designMode )
    {
        return designMode && IsDesignerBandRailVisible()
            ? definition.Page.Width + DesignerBandRailWidth
            : definition.Page.Width;
    }

    private double GetSectionRenderHeight( int sectionIndex, ReportSectionDefinition section, bool collapsed )
    {
        return collapsed
            ? DesignerCollapsedBandHeight
            : GetDesignerSectionHeight( sectionIndex, section );
    }

    private double GetSelectionBoxLeftOffset()
    {
        return IsDesignerBandRailVisible() ? DesignerBandRailWidth : 0;
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
    }

    private async Task ExecuteDesignerCommandAsync( ReportDesignerCommand command )
    {
        var result = await commandManager.ExecuteAsync( command, EffectiveDefinition, CaptureReportState );

        if ( result.NotifyDefinitionChanged && DefinitionChanged.HasDelegate )
            await DefinitionChanged.InvokeAsync( result.Definition );

        if ( result.NotifyDefinitionChanged )
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
            var element = selectionManager.FindSelectedElement( EffectiveDefinition );

            if ( element is not null )
                clipboardElement = ReportContext.CloneElement( element );

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

            if ( ReportDefinitionHelper.TryFindElementLocation( definition, selectionManager.SelectedElementKey, out var sectionIndex, out var elementIndex, out var element ) )
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
        if ( clipboardElement is null || selectionManager.ResolvePasteSectionIndex( EffectiveDefinition ) < 0 )
            return;

        await ExecuteDesignerCommandAsync( new( "Paste element", () =>
        {
            var definition = EffectiveDefinition;
            var targetSectionIndex = selectionManager.ResolvePasteSectionIndex( definition );

            var element = ReportContext.CloneElement( clipboardElement );
            element.Id = ReportDefinitionHelper.CreateDefinitionId();
            element.X = ApplyDesignerGrid( element.X + 16 );
            element.Y = ApplyDesignerGrid( element.Y + 16 );

            definition.Sections[targetSectionIndex].Elements.Add( element );

            SelectElement( ReportDefinitionHelper.EnsureElementId( element ) );
            contextMenu = null;

            return Task.CompletedTask;
        } ) );
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

        selectionManager.ApplyState( definition, nextState.Selection );

        contextMenu = null;
        dragPreview = null;
        ClearDragState();

        commandManager.SetState( CaptureReportState( definition ) );

        if ( notifyDefinitionChanged && DefinitionChanged.HasDelegate )
            await DefinitionChanged.InvokeAsync( definition );

        if ( notifyDefinitionChanged )
            designerSurfaceVersion++;

        await InvokeAsync( StateHasChanged );
    }

    private void SelectReport()
    {
        selectionManager.SelectReport();
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

        SelectElement( key, preserveSelection: selectionManager.IsElementSelected( key ) && selectionManager.SelectedElementKeys.Count > 1 );
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
        selectionManager.SelectElement( key, preserveSelection );
        contextMenu = null;
    }

    private void ToggleElementSelection( string key )
    {
        selectionManager.ToggleElementSelection( key );
        contextMenu = null;
    }

    private void SelectElements( IEnumerable<string> elementKeys, string primaryElementKey = null )
    {
        selectionManager.SelectElements( elementKeys, primaryElementKey );
        contextMenu = null;
    }

    private void SelectSection( int index )
    {
        selectionManager.SelectSection( index );
        contextMenu = null;
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
    }

    private bool IsSectionCollapsed( ReportSectionDefinition section )
    {
        return AllowBandCollapse
            && section is not null
            && collapsedSectionIds.Contains( ReportDefinitionHelper.EnsureSectionId( section ) );
    }

    private void OpenSectionContextMenu( int sectionIndex, MouseEventArgs eventArgs )
    {
        selectionManager.ReportSelected = false;
        selectionManager.SelectedSectionIndex = sectionIndex;
        selectionManager.SelectedElementKey = null;
        selectionManager.SelectedElementKeys.Clear();
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
        selectionManager.ReportSelected = false;
        selectionManager.SelectedSectionIndex = null;

        if ( !selectionManager.IsElementSelected( elementKey ) )
            selectionManager.SelectedElementKeys.Clear();

        selectionManager.SelectedElementKey = elementKey;
        selectionManager.SelectedElementKeys.Add( elementKey );
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
        var element = selectionManager.FindSelectedElement( EffectiveDefinition );

        if ( element is null )
            return;

        await ExecuteDesignerCommandAsync( new( "Move element", () =>
        {
            var definition = EffectiveDefinition;
            var element = selectionManager.FindSelectedElement( definition );

            if ( element is not null )
            {
                ReportDefinitionHelper.TryFindElementLocation( definition, ReportDefinitionHelper.EnsureElementId( element ), out var sectionIndex, out _, out _ );
                var originalX = element.X;
                var originalWidth = element.Width;

                element.X = Math.Max( 0, element.X + x );
                element.Y = Math.Max( 0, element.Y + y );
                element.Width = Math.Max( 8, element.Width + width );
                element.Height = Math.Max( 8, element.Height + height );

                ReportDetailHeaderSynchronizer.SyncMatchingPageHeaderForDetailElement( definition, sectionIndex, sectionIndex, element, originalX, originalWidth, element.X, element.Width );
            }

            return Task.CompletedTask;
        } ) );
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

                update?.Invoke( element );

                ReportDetailHeaderSynchronizer.SyncMatchingPageHeaderForDetailElement( definition, sectionIndex, sectionIndex, element, originalX, originalWidth, element.X, element.Width );
            }

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

            return Task.CompletedTask;
        } ) );
    }

    private async Task InsertSectionAsync( bool insertAfter )
    {
        var definition = EffectiveDefinition;
        var sourceSection = selectionManager.FindSelectedSection( definition );

        if ( sourceSection is null || selectionManager.SelectedSectionIndex is null )
            return;

        await ExecuteDesignerCommandAsync( new( insertAfter ? "Insert band after" : "Insert band before", () =>
        {
            var definition = EffectiveDefinition;
            var sourceSection = selectionManager.FindSelectedSection( definition );

            if ( sourceSection is null || selectionManager.SelectedSectionIndex is null )
                return Task.CompletedTask;

            var insertIndex = insertAfter ? selectionManager.SelectedSectionIndex.Value + 1 : selectionManager.SelectedSectionIndex.Value;
            var section = new ReportSectionDefinition
            {
                Name = ReportDefinitionHelper.CreateUniqueSectionName( definition, $"{ReportDefinitionHelper.GetSectionTypeDisplayName( sourceSection.Type )} band" ),
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

            contextMenu = null;
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

            contextMenu = null;

            return Task.CompletedTask;
        } ) );
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

        if ( !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, elementKey, out var sectionIndex, out _, out var element ) )
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

        SelectElement( elementKey, preserveSelection: selectionManager.IsElementSelected( elementKey ) && selectionManager.SelectedElementKeys.Count > 1 );
    }

    private void BeginElementPointerResize( string elementKey, ReportElementResizeHandle handle, PointerEventArgs eventArgs )
    {
        if ( !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, elementKey, out var sectionIndex, out _, out var element ) )
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
            MinimumHeight = ReportLayoutGeometry.GetMinimumElementHeight( element ),
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

        var section = ReportLayoutGeometry.GetSection( EffectiveDefinition, sectionIndex );

        if ( section is null || section.Suppressed )
            return;

        var x = ReportLayoutGeometry.Clamp( eventArgs.OffsetX, 0, EffectiveDefinition.Page.Width );
        var y = ReportLayoutGeometry.Clamp( GetSectionOffsetY( EffectiveDefinition, sectionIndex ) + eventArgs.OffsetY, 0, GetDesignerContentHeight( EffectiveDefinition ) );

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
            selectedKeys.InsertRange( 0, selectionManager.SelectedElementKeys );

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

        selectionBox.CurrentX = ReportLayoutGeometry.Clamp( selectionBox.StartX + eventArgs.ClientX - selectionBox.StartClientX, 0, EffectiveDefinition.Page.Width );
        selectionBox.CurrentY = ReportLayoutGeometry.Clamp( selectionBox.StartY + eventArgs.ClientY - selectionBox.StartClientY, 0, GetDesignerContentHeight( EffectiveDefinition ) );
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
            && ReportDefinitionHelper.TryFindElementLocation( definition, pointerDrag.ElementKey, out _, out _, out _ );

        if ( !moved || !canMove )
        {
            ClearDragState();
            await InvokeAsync( StateHasChanged );
            return;
        }

        await ExecuteDesignerCommandAsync( new( "Move element", () =>
        {
            var definition = EffectiveDefinition;

            ReportDesignerInteractionService.ApplyElementPointerDrag( definition, pointerDrag, sectionIndex => GetSectionOffsetY( definition, sectionIndex ) );
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
        return ReportDesignerInteractionService.CreateElementDragPreview(
            EffectiveDefinition,
            elementPointerDrag,
            draggedElement,
            targetSectionIndex,
            eventArgs.ClientX,
            eventArgs.ClientY,
            sectionIndex => GetSectionOffsetY( EffectiveDefinition, sectionIndex ),
            ApplyDesignerGrid );
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

        if ( !resized || !ReportDefinitionHelper.TryFindElementLocation( EffectiveDefinition, pointerResize.ElementKey, out _, out _, out _ ) )
        {
            ClearDragState();
            await InvokeAsync( StateHasChanged );
            return;
        }

        await ExecuteDesignerCommandAsync( new( "Resize element", () =>
        {
            ReportDesignerInteractionService.ApplyElementPointerResize( EffectiveDefinition, pointerResize );
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

    private async Task StartDocumentSectionResizeAsync( double startClientY )
    {
        EnsureReportingModule();
        dotNetObjectReference ??= DotNetObjectReference.Create( this );

        await reportingModule.StartSectionResize( dotNetObjectReference, startClientY );
    }

    private void EnsureReportingModule()
    {
        reportingModule ??= new( JSRuntime, VersionProvider, BlazoriseOptions );
    }

    private ReportDesignerDragPreview CreateElementPointerResizePreview( PointerEventArgs eventArgs )
    {
        return ReportDesignerInteractionService.CreateElementResizePreview(
            elementPointerResize,
            draggedElement,
            eventArgs.ClientX,
            eventArgs.ClientY,
            ApplyDesignerGrid );
    }

    private async Task PreviewDesignerDragAsync( int targetSectionIndex, ElementReference sectionBodyElement, DragEventArgs eventArgs )
    {
        if ( draggedKind == ReportDesignerDragKind.None )
            return;

        var offset = await GetDesignerDragOffsetAsync( sectionBodyElement, eventArgs );
        var preview = CreateDragPreview( targetSectionIndex, ApplyDesignerGrid( offset.X ), ApplyDesignerGrid( offset.Y ) );

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

    private async Task<(double X, double Y)> GetDesignerDragOffsetAsync( ElementReference sectionBodyElement, DragEventArgs eventArgs )
    {
        EnsureReportingModule();

        var offset = await reportingModule.GetElementOffset( sectionBodyElement, eventArgs.ClientX, eventArgs.ClientY );

        return offset is { Length: >= 2 }
            ? ( Math.Max( 0, offset[0] ), Math.Max( 0, offset[1] ) )
            : ( Math.Max( 0, eventArgs.OffsetX ), Math.Max( 0, eventArgs.OffsetY ) );
    }

    private async Task DropDesignerItemAsync( int targetSectionIndex, ElementReference sectionBodyElement, DragEventArgs eventArgs )
    {
        var definition = EffectiveDefinition;

        if ( targetSectionIndex < 0 || targetSectionIndex >= definition.Sections.Count )
            return;

        var offset = await GetDesignerDragOffsetAsync( sectionBodyElement, eventArgs );
        var x = ApplyDesignerGrid( offset.X );
        var y = ApplyDesignerGrid( offset.Y );

        var commandName = draggedKind switch
        {
            ReportDesignerDragKind.Field when !string.IsNullOrWhiteSpace( draggedFieldName ) => "Add field",
            ReportDesignerDragKind.ToolboxElement when draggedElementType is not null => "Add element",
            ReportDesignerDragKind.Element when ReportDefinitionHelper.TryFindElementLocation( definition, draggedElementKey, out _, out _, out _ ) => "Move element",
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
                    var fieldBinding = ReportDefinitionHelper.NormalizeFieldBindingForSection( targetSection, draggedDataSourceName, draggedFieldName );
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
                    ReportDetailHeaderSynchronizer.AddPageHeaderForDetailField( definition, targetSectionIndex, targetSection, fieldBinding.FieldName, x, fieldElement.Width );
                    SelectElement( ReportDefinitionHelper.EnsureElementId( fieldElement ) );
                    break;
                case ReportDesignerDragKind.ToolboxElement when draggedElementType is not null:
                    var toolboxElement = ReportDefinitionHelper.CreateElementFromToolbox( draggedElementType.Value, draggedElementText, x, y );
                    targetSection.Elements.Add( toolboxElement );
                    SelectElement( ReportDefinitionHelper.EnsureElementId( toolboxElement ) );
                    break;
                case ReportDesignerDragKind.Element when ReportDefinitionHelper.TryFindElementLocation( definition, draggedElementKey, out var sourceSectionIndex, out var sourceElementIndex, out var element ):
                    var originalX = element.X;
                    var originalWidth = element.Width;

                    definition.Sections[sourceSectionIndex].Elements.RemoveAt( sourceElementIndex );
                    element.X = x;
                    element.Y = y;
                    targetSection.Elements.Add( element );
                    ReportDetailHeaderSynchronizer.SyncMatchingPageHeaderForDetailElement( definition, sourceSectionIndex, targetSectionIndex, element, originalX, originalWidth, element.X, element.Width );
                    SelectElement( ReportDefinitionHelper.EnsureElementId( element ) );
                    break;
            }

            selectionManager.SelectedSectionIndex = null;
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
        var fieldBinding = ReportDefinitionHelper.NormalizeFieldBindingForSection( targetSection, draggedDataSourceName, draggedFieldName );

        return new()
        {
            SectionIndex = targetSectionIndex,
            ElementType = ReportElementType.Field,
            Text = ReportDefinitionHelper.FormatFieldExpression( fieldBinding.DataSourceName, fieldBinding.FieldName ),
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
            Text = element.Type == ReportElementType.Field ? ReportDefinitionHelper.FormatFieldExpression( element ) : element.Text ?? element.Name ?? element.Type.ToString(),
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

    private double ApplyDesignerGrid( double value )
    {
        return snapToGrid ? ReportLayoutGeometry.SnapToGrid( value ) : Math.Max( 0, value );
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
        return ReportLayoutGeometry.GetSectionOffsetY( definition, sectionIndex, GetDesignerSectionHeight );
    }

    private double GetDesignerContentHeight( ReportDefinition definition )
    {
        return ReportLayoutGeometry.GetContentHeight( definition, GetDesignerSectionHeight );
    }

    private double GetDesignerSectionHeight( int sectionIndex, ReportSectionDefinition section )
    {
        if ( sectionPointerResize is not null && sectionPointerResize.SectionIndex == sectionIndex )
            return sectionPointerResize.TargetHeight;

        return BandMode == ReportBandMode.Rail && section is not null && !section.Suppressed && IsSectionCollapsed( section )
            ? DesignerCollapsedBandHeight
            : section?.Height ?? 0;
    }

    private static double GetMinimumSectionHeight( ReportSectionDefinition section )
    {
        return ReportLayoutGeometry.GetMinimumSectionHeight( section );
    }

    private void OnSnapToGridChanged( ChangeEventArgs eventArgs )
    {
        snapToGrid = eventArgs.Value is bool value
            ? value
            : string.Equals( Convert.ToString( eventArgs.Value, CultureInfo.InvariantCulture ), "true", StringComparison.OrdinalIgnoreCase );
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

    private string SelectedDesignerPanelTabName => selectedDesignerPanelTab.ToString();

    private string ToolbarStateKey => $"{CurrentMode}|{CurrentPreviewFormat}|{selectionManager.SelectedElementKey}|{selectionManager.SelectedElementKeys.Count}|{selectionManager.SelectedSectionIndex}|{clipboardElement?.Id}|{commandManager.CanUndo}|{commandManager.CanRedo}";

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