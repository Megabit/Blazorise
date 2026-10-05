#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blazorise.Demo.Models;
using Blazorise.Ribbon;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Demo.Pages.Tests;

public partial class RibbonPage : ComponentBase
{
    #region Members

    private static readonly string[] fonts = ["Segoe UI", "Arial", "Calibri", "Cambria", "Georgia", "Times New Roman"];

    private static readonly int[] fontSizes = [8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 28, 32, 36, 48, 72];

    private static readonly string[] styles = ["Normal", "No Spacing", "Heading 1", "Heading 2", "Title", "Subtitle"];

    private readonly List<WordRibbonDocument> history = [new()];

    private readonly StyleBuilder documentStyleBuilder;

    private int historyIndex;

    private WordRibbonDocument savedDocument = new();

    private MemoInput documentInputRef;

    private RibbonBackstage backstageRef;

    private RibbonApplicationButton fileButtonRef;

    private Modal launcherModalRef;

    private string selectedTab = "home";

    private string selectedBackstageItem = "info";

    private bool backstageVisible;

    private string saveAsTitle = "Quarterly overview copy";

    private int printCopies = 1;

    private string printOrientation = "Portrait";

    private bool collapsed;

    private RibbonDisplayMode displayMode;

    private bool autoSave = true;

    private bool showFormattingMarks;

    private bool trackChanges;

    private bool showRuler;

    private bool showGridlines;

    private bool showNavigation;

    private bool tableToolsVisible;

    private int zoom = 100;

    private string searchText;

    private string clipboardText = "Pasted content from the sample clipboard.";

    private string status = "Ready";

    private string launcherTitle;

    private string launcherDescription;

    #endregion

    #region Constructors

    public RibbonPage()
    {
        documentStyleBuilder = new( BuildDocumentStyles );
    }

    #endregion

    #region Methods

    private void BuildDocumentStyles( StyleBuilder builder )
    {
        builder.Append( $"font-family:{Document.FontFamily}" );
        builder.Append( $"font-size:{( Document.FontSize * zoom / 100d ).ToString( CultureInfo.InvariantCulture )}px" );
        builder.Append( $"font-weight:{( Document.Bold ? "700" : "400" )}" );
        builder.Append( $"font-style:{( Document.Italic ? "italic" : "normal" )}" );
        builder.Append( $"text-decoration:{DocumentDecorationString}" );
        builder.Append( $"color:{Document.FontColor}" );
        builder.Append( $"background-color:{Document.HighlightColor}" );
        builder.Append( $"text-align:{DocumentAlignmentString}" );
        builder.Append( "line-height:1.65" );
    }

    private void OnSaveHandler()
    {
        savedDocument = Document;
        status = "Saved in this demo session";
    }

    private void OnSaveWordHandler() => ShowCommandStatus( "Save As Word Document" );

    private void OnSavePdfHandler() => ShowCommandStatus( "Save As PDF" );

    private void OnSaveTextHandler() => ShowCommandStatus( "Save As Plain Text" );

    private void OnUndoHandler()
    {
        if ( !CanUndo )
        {
            return;
        }

        historyIndex--;
        OnHistoryChanged();
        status = "Change undone";
    }

    private void OnRedoHandler()
    {
        if ( !CanRedo )
        {
            return;
        }

        historyIndex++;
        OnHistoryChanged();
        status = "Change restored";
    }

    private void OnCopyHandler()
    {
        clipboardText = Document.Text;
        status = "Copied to the sample clipboard";
    }

    private async Task OnCutHandler()
    {
        clipboardText = Document.Text;
        await UpdateDocument( Document with { Text = string.Empty } );
        status = "Cut to the sample clipboard";
    }

    private async Task OnNewDocumentHandler()
    {
        await UpdateDocument( new() { Title = "Untitled document", Text = string.Empty } );
        await backstageRef.Hide();
    }

    private async Task OnBackstageSaveHandler()
    {
        OnSaveHandler();

        await backstageRef.Hide();
    }

    private async Task OnSaveAsHandler()
    {
        if ( string.IsNullOrWhiteSpace( saveAsTitle ) )
        {
            return;
        }

        await UpdateDocument( Document with { Title = saveAsTitle.Trim() } );
        OnSaveHandler();

        await backstageRef.Hide();
    }

    private async Task OnNewNotesHandler()
    {
        await UpdateDocument( new()
        {
            Title = "Meeting notes",
            Text = "Meeting notes\n\nAttendees\n\nAgenda\n\nDecisions\n\nAction items",
        } );

        await backstageRef.Hide();
    }

    private async Task OnOpenSampleHandler()
    {
        await UpdateDocument( new() );

        await backstageRef.Hide();
    }

    private async Task OnOpenNotesHandler()
    {
        await UpdateDocument( new()
        {
            Title = "Project notes",
            Text = "Project notes\n\nGoals\nReview the first release and gather feedback.\n\nNext steps\nPlan the next iteration and agree on priorities.",
        } );

        await backstageRef.Hide();
    }

    private void OnPrintHandler()
    {
        status = $"Print command invoked: {printCopies} copies, {printOrientation.ToLowerInvariant()} orientation";
    }

    private Task OnIncreaseFontSizeHandler() => OnFontSizeChangedHandler( Math.Min( 72, Document.FontSize + 2 ) );

    private Task OnDecreaseFontSizeHandler() => OnFontSizeChangedHandler( Math.Max( 8, Document.FontSize - 2 ) );

    private Task OnClearFormattingHandler() => UpdateDocument( new() { Title = Document.Title, Text = Document.Text } );

    private Task OnBulletsHandler() => UpdateDocument( Document with { Text = $"{Document.Text}\n• New item" } );

    private Task OnNumberingHandler() => UpdateDocument( Document with { Text = $"{Document.Text}\n1. New item" } );

    private Task OnPageBreakHandler() => UpdateDocument( Document with { Text = $"{Document.Text}\n\n— Page break —\n\n" } );

    private Task OnDateAndTimeHandler() => UpdateDocument( Document with { Text = $"{Document.Text}\n{DateTime.Now:D}" } );

    private Task OnSymbolHandler() => UpdateDocument( Document with { Text = $"{Document.Text} ∞" } );

    private async Task OnTableHandler()
    {
        tableToolsVisible = true;
        await UpdateDocument( Document with { Text = $"{Document.Text}\n\nName\tOwner\tStatus\nPortal\tProduct\tComplete\nResearch\tDesign\tIn progress" } );

        selectedTab = "table-layout";
        collapsed = false;
    }

    private void OnDeleteTableHandler()
    {
        tableToolsVisible = false;
        status = "Table Tools hidden";
    }

    private void OnFindHandler()
    {
        status = "Use the Find in document field above the ribbon";
    }

    private async Task OnSelectAllHandler()
    {
        await documentInputRef.Focus();
        status = "Document focused; press Ctrl+A to select its text";
    }

    private void OnWordCountHandler()
    {
        status = $"Document contains {WordCount} words";
    }

    private void OnZoomInHandler() => SetZoom( Math.Min( 200, zoom + 10 ) );

    private void OnZoomOutHandler() => SetZoom( Math.Max( 50, zoom - 10 ) );

    private void OnResetZoomHandler() => SetZoom( 100 );

    private void OnHelpHandler()
    {
        status = "Use arrow keys to move between tabs, then Enter to select. Collapse the ribbon with the chevron.";
    }

    private void OnFormatPainterHandler() => ShowCommandStatus( "Format Painter" );

    private void OnSubscriptHandler() => ShowCommandStatus( "Subscript" );

    private void OnSuperscriptHandler() => ShowCommandStatus( "Superscript" );

    private void OnDecreaseIndentHandler() => ShowCommandStatus( "Decrease Indent" );

    private void OnIncreaseIndentHandler() => ShowCommandStatus( "Increase Indent" );

    private void OnSortHandler() => ShowCommandStatus( "Sort" );

    private void OnBordersHandler() => ShowCommandStatus( "Borders" );

    private void OnReplaceHandler() => ShowCommandStatus( "Replace" );

    private void OnDictateHandler() => ShowCommandStatus( "Dictate" );

    private void OnEditorHandler() => ShowCommandStatus( "Editor" );

    private void OnCoverPageHandler() => ShowCommandStatus( "Cover Page" );

    private void OnPicturesHandler() => ShowCommandStatus( "Pictures" );

    private void OnShapesHandler() => ShowCommandStatus( "Shapes" );

    private void OnIconsHandler() => ShowCommandStatus( "Icons" );

    private void OnChartHandler() => ShowCommandStatus( "Chart" );

    private void OnScreenshotHandler() => ShowCommandStatus( "Screenshot" );

    private void OnLinkHandler() => ShowCommandStatus( "Link" );

    private void OnBookmarkHandler() => ShowCommandStatus( "Bookmark" );

    private void OnTextBoxHandler() => ShowCommandStatus( "Text Box" );

    private void OnThemesHandler() => ShowCommandStatus( "Themes" );

    private void OnColorsHandler() => ShowCommandStatus( "Colors" );

    private void OnWatermarkHandler() => ShowCommandStatus( "Watermark" );

    private void OnPageColorHandler() => ShowCommandStatus( "Page Color" );

    private void OnPageBordersHandler() => ShowCommandStatus( "Page Borders" );

    private void OnMarginsHandler() => ShowCommandStatus( "Margins" );

    private void OnOrientationHandler() => ShowCommandStatus( "Orientation" );

    private void OnSizeHandler() => ShowCommandStatus( "Size" );

    private void OnColumnsHandler() => ShowCommandStatus( "Columns" );

    private void OnBreaksHandler() => ShowCommandStatus( "Breaks" );

    private void OnLineNumbersHandler() => ShowCommandStatus( "Line Numbers" );

    private void OnIndentLeftHandler() => ShowCommandStatus( "Indent Left" );

    private void OnIndentRightHandler() => ShowCommandStatus( "Indent Right" );

    private void OnPositionHandler() => ShowCommandStatus( "Position" );

    private void OnWrapTextHandler() => ShowCommandStatus( "Wrap Text" );

    private void OnBringForwardHandler() => ShowCommandStatus( "Bring Forward" );

    private void OnSendBackwardHandler() => ShowCommandStatus( "Send Backward" );

    private void OnThesaurusHandler() => ShowCommandStatus( "Thesaurus" );

    private void OnTranslateHandler() => ShowCommandStatus( "Translate" );

    private void OnLanguageHandler() => ShowCommandStatus( "Language" );

    private void OnDeleteHandler() => ShowCommandStatus( "Delete" );

    private void OnPreviousHandler() => ShowCommandStatus( "Previous" );

    private void OnNextHandler() => ShowCommandStatus( "Next" );

    private void OnRestrictEditingHandler() => ShowCommandStatus( "Restrict Editing" );

    private void OnReadModeHandler() => ShowCommandStatus( "Read Mode" );

    private void OnPrintLayoutHandler() => ShowCommandStatus( "Print Layout" );

    private void OnWebLayoutHandler() => ShowCommandStatus( "Web Layout" );

    private void OnFeedbackHandler() => ShowCommandStatus( "Feedback" );

    private void OnInsertAboveHandler() => ShowCommandStatus( "Insert Above" );

    private void OnInsertBelowHandler() => ShowCommandStatus( "Insert Below" );

    private void OnMergeCellsHandler() => ShowCommandStatus( "Merge Cells" );

    private void OnSplitCellsHandler() => ShowCommandStatus( "Split Cells" );

    private Task OnDocumentChangedHandler( string text ) => UpdateDocument( Document with { Text = text } );

    private Task OnTitleChangedHandler( string title ) => UpdateDocument( Document with { Title = title } );

    private Task OnFontFamilyChangedHandler( string font ) => UpdateDocument( Document with { FontFamily = font } );

    private Task OnFontSizeChangedHandler( int size ) => UpdateDocument( Document with { FontSize = Math.Clamp( size, 8, 72 ) } );

    private Task OnBoldChangedHandler( bool value ) => UpdateDocument( Document with { Bold = value } );

    private Task OnItalicChangedHandler( bool value ) => UpdateDocument( Document with { Italic = value } );

    private Task OnUnderlineChangedHandler( bool value ) => UpdateDocument( Document with { Underline = value } );

    private Task OnStrikethroughChangedHandler( bool value ) => UpdateDocument( Document with { Strikethrough = value } );

    private Task OnFontColorChangedHandler( string color ) => UpdateDocument( Document with { FontColor = color } );

    private Task OnHighlightChangedHandler( string color ) => UpdateDocument( Document with { HighlightColor = color } );

    private Task OnAlignLeftHandler( bool value ) => UpdateDocument( Document with { Alignment = TextAlignment.Start } );

    private Task OnAlignCenterHandler( bool value ) => UpdateDocument( Document with { Alignment = TextAlignment.Center } );

    private Task OnAlignRightHandler( bool value ) => UpdateDocument( Document with { Alignment = TextAlignment.End } );

    private Task OnAlignJustifyHandler( bool value ) => UpdateDocument( Document with { Alignment = TextAlignment.Justified } );

    private Task OnStyleChangedHandler( string style )
    {
        var size = style switch
        {
            "Heading 1" => 24,
            "Heading 2" => 20,
            "Title" => 32,
            "Subtitle" => 18,
            _ => 14,
        };

        return UpdateDocument( Document with
        {
            Style = style,
            FontSize = size,
            Bold = style.StartsWith( "Heading", StringComparison.Ordinal ),
            FontColor = style.StartsWith( "Heading", StringComparison.Ordinal ) ? "#2f5496" : "#222222",
        } );
    }

    private void OnAutoSaveChangedHandler( bool value )
    {
        autoSave = value;

        if ( autoSave )
        {
            savedDocument = Document;
        }
    }

    private void OnSearchChangedHandler( string text )
    {
        searchText = text;
    }

    private Task OnPasteHandler() => UpdateDocument( Document with { Text = $"{Document.Text}\n{clipboardText}" } );

    private async Task OnPasteTextHandler()
    {
        await OnPasteHandler();
        status = "Pasted text from the sample clipboard";
    }

    private Task OnFileHandler() => backstageRef.Show();

    private Task OnBackstageClosedHandler() => fileButtonRef.Focus( false );

    private Task OnCloseLauncherHandler() => launcherModalRef.Hide();

    private Task OnFontLauncherHandler() => ShowLauncher( "Font", "Change the sample document's font size. The ribbon controls also update its font family, emphasis, and colors." );

    private Task OnParagraphLauncherHandler() => ShowLauncher( "Paragraph", "Use the alignment controls in the ribbon to change the sample document's paragraph alignment." );

    private Task OnClipboardLauncherHandler() => ShowLauncher( "Clipboard", $"The sample clipboard contains:\n{clipboardText}" );

    private Task OnStylesLauncherHandler() => ShowLauncher( "Styles", "Select a preview in the Styles gallery to apply its formatting to the sample document." );

    private Task OnPageSetupLauncherHandler() => ShowLauncher( "Page Setup", "This sample uses a fixed document sheet inside a horizontally scrollable workspace." );

    private Task OnCommentsHandler() => ShowLauncher( "Comments", "The ribbon delegates application-specific commands and dialogs to the application." );

    private void OnShareHandler()
    {
        status = "Share command invoked";
    }

    private void OnCopyLinkHandler()
    {
        status = "Copy link command invoked";
    }

    private void ShowCommandStatus( string command )
    {
        status = $"{command} command invoked";
    }

    private Task ShowLauncher( string title, string description )
    {
        launcherTitle = title;
        launcherDescription = description;

        return launcherModalRef.Show();
    }

    private Task UpdateDocument( WordRibbonDocument document )
    {
        if ( Document == document )
        {
            return Task.CompletedTask;
        }

        if ( CanRedo )
        {
            history.RemoveRange( historyIndex + 1, history.Count - historyIndex - 1 );
        }

        history.Add( document );
        historyIndex++;
        OnHistoryChanged();
        status = "Document updated";

        return Task.CompletedTask;
    }

    private void OnHistoryChanged()
    {
        documentStyleBuilder.Dirty();

        if ( autoSave )
        {
            savedDocument = Document;
        }
    }

    private void SetZoom( int value )
    {
        zoom = value;
        documentStyleBuilder.Dirty();
        status = $"Zoom set to {zoom}%";
    }

    #endregion

    #region Properties

    private WordRibbonDocument Document => history[historyIndex];

    private bool CanUndo => historyIndex > 0;

    private bool CanRedo => historyIndex < history.Count - 1;

    private string SaveStatus => Document == savedDocument ? "Saved in demo session" : "Unsaved changes";

    private int WordCount => Document.Text.Split( (char[])null, StringSplitOptions.RemoveEmptyEntries ).Count( word => word.Any( char.IsLetterOrDigit ) );

    private string DocumentStyleNames => documentStyleBuilder.Styles;

    private string DocumentDecorationString
        => Document.Underline && Document.Strikethrough ? "underline line-through"
            : Document.Underline ? "underline"
            : Document.Strikethrough ? "line-through"
            : "none";

    private string DocumentAlignmentString => Document.Alignment switch
    {
        TextAlignment.Center => "center",
        TextAlignment.End => "end",
        TextAlignment.Justified => "justify",
        _ => "start",
    };

    private string SearchStatus => $"{Regex.Matches( Document.Text, Regex.Escape( searchText ?? string.Empty ), RegexOptions.IgnoreCase ).Count} matches for “{searchText}”";

    private string FormattingMarksText => Document.Text.Replace( " ", "·", StringComparison.Ordinal ).Replace( "\n", "¶ ", StringComparison.Ordinal );

    #endregion
}