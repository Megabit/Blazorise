using System.Collections.Generic;

namespace Blazorise.Markdown;

public class MarkdownInitializeJSOptions
{
    public string Value { get; set; }
    public bool? AutoDownloadFontAwesome { get; set; }
    public string[] HideIcons { get; set; }
    public string[] ShowIcons { get; set; }
    public bool LineNumbers { get; set; }
    public bool LineWrapping { get; set; }
    public string MinHeight { get; set; }
    public string MaxHeight { get; set; }
    public string Placeholder { get; set; }
    public int TabSize { get; set; }
    public string Theme { get; set; }
    public string Direction { get; set; }
    public IEnumerable<object> Toolbar { get; set; }
    public bool ToolbarTips { get; set; }
    public bool UploadImage { get; set; }
    public long ImageMaxSize { get; set; }
    public string ImageAccept { get; set; }
    public string ImageUploadEndpoint { get; set; }
    public string ImagePathAbsolute { get; set; }
    public string ImageCSRFToken { get; set; }
    public MarkdownImageTextsOptions ImageTexts { get; set; }
    public MarkdownErrorMessages ErrorMessages { get; set; }
    public bool Autofocus { get; set; }
    public MarkdownAutoRefresh AutoRefresh { get; set; }
    public MarkdownAutosave Autosave { get; set; }
    public MarkdownBlockStyles BlockStyles { get; set; }
    public bool ForceSync { get; set; }
    public bool IndentWithTabs { get; set; }
    public string InputStyle { get; set; }
    public MarkdownInsertTexts InsertTexts { get; set; }
    public bool NativeSpellcheck { get; set; }
    public MarkdownParsingConfig ParsingConfig { get; set; }
    public string PreviewClass { get; set; }
    public bool PreviewImagesInEditor { get; set; }
    public MarkdownPromptTexts PromptTexts { get; set; }
    public bool PromptURLs { get; set; }
    public MarkdownRenderingConfig RenderingConfig { get; set; }
    public string ScrollbarStyle { get; set; }
    public MarkdownShortcuts Shortcuts { get; set; }
    public bool SideBySideFullscreen { get; set; }
    public bool SpellChecker { get; set; }
    public string[] Status { get; set; }
    public bool StyleSelectedText { get; set; }
    public bool SyncSideBySidePreviewScroll { get; set; }
    public string UnorderedListStyle { get; set; }
    public bool UsePreviewRender { get; set; }
}

public class MarkdownImageTextsOptions
{
    public string SbInit { get; set; }
    public string SbOnDragEnter { get; set; }
    public string SbOnDrop { get; set; }
    public string SbProgress { get; set; }
    public string SbOnUploaded { get; set; }
    public string SizeUnits { get; set; }
}
