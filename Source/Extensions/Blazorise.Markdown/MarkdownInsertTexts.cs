namespace Blazorise.Markdown;

/// <summary>
/// Customize how certain buttons that insert text behave. Takes an array with two elements.
/// The first element will be the text inserted before the cursor or highlight, and the second
/// element will be inserted after.
/// For example, this is the default link value: ["[", "](http://)"].
/// </summary>
public class MarkdownInsertTexts
{
    /// <summary>Text placed around the cursor for a horizontal divider.</summary>
    public string[] HorizontalRule { get; set; } = new[] { "", "\n\n-----\n\n" };

    /// <summary>Prefix and suffix surrounding an inserted image URL.</summary>
    public string[] Image { get; set; } = new[] { "![](", "#url#)" };

    /// <summary>Prefix and suffix surrounding hyperlink text and its URL.</summary>
    public string[] Link { get; set; } = new[] { "[", "](#url#)" };

    /// <summary>Template inserted when creating a Markdown table.</summary>
    public string[] Table { get; set; } = new[] { "", "\n\n| Column 1 | Column 2 | Column 3 |\n| -------- | -------- | -------- |\n| Text     | Text     | Text     |\n\n" };

    /// <summary>Template populated after an image upload succeeds.</summary>
    public string[] UploadedImage { get; set; } = new[] { "![](#url#)", "" };
}