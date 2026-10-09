#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

    private readonly StyleBuilder documentStyleBuilder;

    private string fontColor = "#222222";

    private string highlightColor = "#ffffff";

    private MemoInput documentInputRef;

    private RibbonBackstage backstageRef;

    private Modal launcherModalRef;

    private string selectedTab = "home";

    private string selectedBackstageItem = "info";

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

    private void OnSaveHandler() => ShowCommandStatus( "Save" );

    private void OnSaveWordHandler() => ShowCommandStatus( "Save As Word Document" );

    private void OnSavePdfHandler() => ShowCommandStatus( "Save As PDF" );

    private void OnSaveTextHandler() => ShowCommandStatus( "Save As Plain Text" );

    private void OnUndoHandler() => ShowCommandStatus( "Undo" );

    private void OnRedoHandler() => ShowCommandStatus( "Redo" );

    private void OnCopyHandler() => ShowCommandStatus( "Copy" );

    private void OnCutHandler() => ShowCommandStatus( "Cut" );

    private Task OnNewDocumentHandler() => ShowBackstageCommand( "New document" );

    private Task OnBackstageSaveHandler() => ShowBackstageCommand( "Save" );

    private Task OnSaveAsHandler() => ShowBackstageCommand( "Save As" );

    private Task OnNewNotesHandler() => ShowBackstageCommand( "New meeting notes" );

    private Task OnOpenSampleHandler() => ShowBackstageCommand( "Open quarterly overview" );

    private Task OnOpenNotesHandler() => ShowBackstageCommand( "Open project notes" );

    private void OnPrintHandler() => ShowCommandStatus( "Print" );

    private Task OnIncreaseFontSizeHandler() => OnFontSizeChangedHandler( Math.Min( 72, Document.FontSize + 2 ) );

    private Task OnDecreaseFontSizeHandler() => OnFontSizeChangedHandler( Math.Max( 8, Document.FontSize - 2 ) );

    private void OnClearFormattingHandler() => ShowCommandStatus( "Clear Formatting" );

    private void OnBulletsHandler() => ShowCommandStatus( "Bullets" );

    private void OnNumberingHandler() => ShowCommandStatus( "Numbering" );

    private void OnPageBreakHandler() => ShowCommandStatus( "Page Break" );

    private void OnDateAndTimeHandler() => ShowCommandStatus( "Date and Time" );

    private void OnSymbolHandler() => ShowCommandStatus( "Symbol" );

    private void OnTableHandler()
    {
        tableToolsVisible = true;
        selectedTab = "table-layout";
        collapsed = false;
        ShowCommandStatus( "Insert Table" );
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

    private void OnAdvancedFindHandler() => ShowCommandStatus( "Advanced Find" );

    private void OnGoToHandler() => ShowCommandStatus( "Go To" );

    private async Task OnSelectAllHandler()
    {
        await documentInputRef.Focus();
        status = "Document focused; press Ctrl+A to select its text";
    }

    private void OnSelectObjectsHandler() => ShowCommandStatus( "Select Objects" );

    private void OnWordCountHandler() => ShowCommandStatus( "Word Count" );

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

    private void OnDevicePicturesHandler() => ShowCommandStatus( "Pictures from This Device" );

    private void OnStockPicturesHandler() => ShowCommandStatus( "Stock Images" );

    private void OnOnlinePicturesHandler() => ShowCommandStatus( "Online Pictures" );

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

    private void OnAutomaticFontColorHandler() => OnFontColorChangedHandler( "#000000" );

    private void OnFontColorChangedHandler( string color )
    {
        fontColor = color;
    }

    private void OnHighlightChangedHandler( string color )
    {
        highlightColor = color;
    }

    private Task OnAlignLeftHandler( bool value ) => UpdateDocument( Document with { Alignment = TextAlignment.Start } );

    private Task OnAlignCenterHandler( bool value ) => UpdateDocument( Document with { Alignment = TextAlignment.Center } );

    private Task OnAlignRightHandler( bool value ) => UpdateDocument( Document with { Alignment = TextAlignment.End } );

    private Task OnAlignJustifyHandler( bool value ) => UpdateDocument( Document with { Alignment = TextAlignment.Justified } );

    private Task OnStyleChangedHandler( string style ) => UpdateDocument( Document with { Style = style } );

    private void OnAutoSaveChangedHandler( bool value )
    {
        autoSave = value;
    }

    private void OnSearchChangedHandler( string text )
    {
        searchText = text;
    }

    private void OnPasteHandler() => ShowCommandStatus( "Paste" );

    private void OnPasteTextHandler() => ShowCommandStatus( "Paste Text" );

    private Task OnCloseLauncherHandler() => launcherModalRef.Hide();

    private Task OnFontLauncherHandler() => ShowLauncher( "Font", "Choose a font size using the dialog or the ribbon controls." );

    private Task OnParagraphLauncherHandler() => ShowLauncher( "Paragraph", "Use the alignment controls in the ribbon to change the sample document's paragraph alignment." );

    private Task OnClipboardLauncherHandler() => ShowLauncher( "Clipboard", "Use grouped commands and split buttons to present clipboard actions." );

    private Task OnStylesLauncherHandler() => ShowLauncher( "Styles", "Select a style preview in the gallery." );

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

        var stylesChanged = Document.FontFamily != document.FontFamily
            || Document.FontSize != document.FontSize
            || Document.Bold != document.Bold
            || Document.Italic != document.Italic
            || Document.Underline != document.Underline
            || Document.Strikethrough != document.Strikethrough
            || Document.FontColor != document.FontColor
            || Document.HighlightColor != document.HighlightColor
            || Document.Alignment != document.Alignment;

        Document = document;

        if ( stylesChanged )
        {
            documentStyleBuilder.Dirty();
        }

        return Task.CompletedTask;
    }

    private Task ShowBackstageCommand( string command )
    {
        ShowCommandStatus( command );

        return backstageRef.Hide();
    }

    private void SetZoom( int value )
    {
        if ( zoom == value )
        {
            return;
        }

        zoom = value;
        documentStyleBuilder.Dirty();
        status = $"Zoom set to {zoom}%";
    }

    #endregion

    #region Properties

    private IReadOnlyList<string> ActiveContextualGroups => tableToolsVisible ? ["table"] : [];

    private WordRibbonDocument Document { get; set; } = new();

    private bool CanUndo => false;

    private bool CanRedo => false;

    private string SaveStatus => "Saved in demo session";

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

    private string SearchStatus => string.IsNullOrWhiteSpace( searchText ) ? "Find in document" : $"Find command invoked: {searchText}";

    private Action<string> NonRenderingFontColorChangedHandler
        => EventUtil.AsNonRenderingEventHandler<string>( OnFontColorChangedHandler );

    private Action<string> NonRenderingHighlightChangedHandler
        => EventUtil.AsNonRenderingEventHandler<string>( OnHighlightChangedHandler );

    private string FormattingMarksText => Document.Text.Replace( " ", "·", StringComparison.Ordinal ).Replace( "\n", "¶ ", StringComparison.Ordinal );

    #endregion
}