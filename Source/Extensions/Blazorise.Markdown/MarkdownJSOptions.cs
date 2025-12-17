using System.Collections.Generic;

namespace Blazorise.Markdown;

/// <summary>
/// Represents JavaScript options for initializing a markdown editor component.
/// </summary>
public class MarkdownJSOptions
{
    /// <summary>
    /// Gets or sets the initial markdown content value in the editor.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether FontAwesome icons should be automatically downloaded.
    /// </summary>
    public bool? AutoDownloadFontAwesome { get; set; }

    /// <summary>
    /// Gets or sets an array of icon names to hide from the toolbar.
    /// </summary>
    public string[] HideIcons { get; set; }

    /// <summary>
    /// Gets or sets an array of icon names to show in the toolbar.
    /// </summary>
    public string[] ShowIcons { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether line numbers should be displayed in the editor.
    /// </summary>
    public bool LineNumbers { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether line wrapping is enabled in the editor.
    /// </summary>
    public bool LineWrapping { get; set; }

    /// <summary>
    /// Gets or sets the minimum height of the editor.
    /// </summary>
    public string MinHeight { get; set; }

    /// <summary>
    /// Gets or sets the maximum height of the editor.
    /// </summary>
    public string MaxHeight { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text displayed when the editor is empty.
    /// </summary>
    public string Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the tab size for indentation in the editor.
    /// </summary>
    public int TabSize { get; set; }

    /// <summary>
    /// Gets or sets the theme of the editor (e.g., "default", "dark").
    /// </summary>
    public string ThemeName { get; set; }

    /// <summary>
    /// Gets or sets the text direction of the editor (e.g., "ltr", "rtl").
    /// </summary>
    public string Direction { get; set; }

    /// <summary>
    /// Gets or sets the toolbar configuration, including items and customization options.
    /// </summary>
    public IEnumerable<object> Toolbar { get; set; }

    /// <summary>
    /// Gets or sets a prefix to the toolbar button classes when set. For example, a value of `"mde"` results in `"mde-bold"` for the Bold button.
    /// </summary>
    public string ToolbarButtonClassPrefix { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether tooltips are shown on the toolbar.
    /// </summary>
    public bool ToolbarTips { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether image uploads are allowed.
    /// </summary>
    public bool UploadImage { get; set; }

    /// <summary>
    /// Gets or sets the maximum size for uploaded images, in bytes.
    /// </summary>
    public long ImageMaxSize { get; set; }

    /// <summary>
    /// Gets or sets the accepted image MIME types for uploading.
    /// </summary>
    public string ImageAccept { get; set; }

    /// <summary>
    /// Gets or sets the endpoint URL for uploading images.
    /// </summary>
    public string ImageUploadEndpoint { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether image paths are absolute.
    /// </summary>
    public string ImagePathAbsolute { get; set; }

    /// <summary>
    /// Gets or sets the CSRF token for image uploads.
    /// </summary>
    public string ImageCSRFToken { get; set; }

    /// <summary>
    /// Gets or sets the texts shown during image upload events.
    /// </summary>
    public MarkdownImageTextsOptions ImageTexts { get; set; }

    /// <summary>
    /// Gets or sets the error messages displayed within the editor.
    /// </summary>
    public MarkdownErrorMessages ErrorMessages { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the editor should autofocus when initialized.
    /// </summary>
    public bool Autofocus { get; set; }

    /// <summary>
    /// Gets or sets the auto-refresh settings for the editor.
    /// </summary>
    public MarkdownAutoRefresh AutoRefresh { get; set; }

    /// <summary>
    /// Gets or sets the autosave configuration for the editor.
    /// </summary>
    public MarkdownAutosave Autosave { get; set; }

    /// <summary>
    /// Gets or sets the block style options for the editor.
    /// </summary>
    public MarkdownBlockStyles BlockStyles { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the editor content should be forcefully synchronized.
    /// </summary>
    public bool ForceSync { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether tabs should be used for indentation.
    /// </summary>
    public bool IndentWithTabs { get; set; }

    /// <summary>
    /// Gets or sets the input style for the editor.
    /// </summary>
    public string InputStyle { get; set; }

    /// <summary>
    /// Gets or sets the configuration for text insertion in the editor.
    /// </summary>
    public MarkdownInsertTexts InsertTexts { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether native spellcheck is enabled in the editor.
    /// </summary>
    public bool NativeSpellcheck { get; set; }

    /// <summary>
    /// Gets or sets the parsing configuration for the markdown content.
    /// </summary>
    public MarkdownParsingConfig ParsingConfig { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes for the preview panel.
    /// </summary>
    public string PreviewClass { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether images should be previewed within the editor.
    /// </summary>
    public bool PreviewImagesInEditor { get; set; }

    /// <summary>
    /// Gets or sets the prompt texts for various user actions within the editor.
    /// </summary>
    public MarkdownPromptTexts PromptTexts { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether URLs should prompt for confirmation before opening.
    /// </summary>
    public bool PromptURLs { get; set; }

    /// <summary>
    /// Gets or sets the rendering configuration for markdown output.
    /// </summary>
    public MarkdownRenderingConfig RenderingConfig { get; set; }

    /// <summary>
    /// Gets or sets the style for the scrollbar in the editor.
    /// </summary>
    public string ScrollbarStyle { get; set; }

    /// <summary>
    /// Gets or sets the keyboard shortcuts configuration for the editor.
    /// </summary>
    public MarkdownShortcuts Shortcuts { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the side-by-side preview is fullscreen.
    /// </summary>
    public bool SideBySideFullscreen { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether spell checking is enabled in the editor.
    /// </summary>
    public bool SpellChecker { get; set; }

    /// <summary>
    /// Gets or sets the status indicators to display within the editor.
    /// </summary>
    public string[] Status { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether selected text should be styled.
    /// </summary>
    public bool StyleSelectedText { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the side-by-side preview scroll should be synchronized.
    /// </summary>
    public bool SyncSideBySidePreviewScroll { get; set; }

    /// <summary>
    /// Gets or sets the style of unordered lists (e.g., "bullet", "dash").
    /// </summary>
    public string UnorderedListStyle { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a custom render function should be used for the preview.
    /// </summary>
    public bool UsePreviewRender { get; set; }
}

/// <summary>
/// Represents text options for image upload events in a markdown editor.
/// </summary>
public class MarkdownImageTextsOptions
{
    /// <summary>
    /// Gets or sets the initial text for the status bar.
    /// </summary>
    public string SbInit { get; set; }

    /// <summary>
    /// Gets or sets the text displayed when an item is dragged over the editor.
    /// </summary>
    public string SbOnDragEnter { get; set; }

    /// <summary>
    /// Gets or sets the text displayed when an item is dropped in the editor.
    /// </summary>
    public string SbOnDrop { get; set; }

    /// <summary>
    /// Gets or sets the text for the upload progress.
    /// </summary>
    public string SbProgress { get; set; }

    /// <summary>
    /// Gets or sets the text displayed after an image is uploaded.
    /// </summary>
    public string SbOnUploaded { get; set; }

    /// <summary>
    /// Gets or sets the units of size (e.g., "KB", "MB") displayed during image upload.
    /// </summary>
    public string SizeUnits { get; set; }
}