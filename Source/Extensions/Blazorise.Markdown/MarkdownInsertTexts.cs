namespace Blazorise.Markdown
{
    /// <summary>
    /// Customize how certain buttons that insert text behave. Takes an array with two elements.
    /// The first element will be the text inserted before the cursor or highlight, and the second
    /// element will be inserted after.
    /// For example, this is the default link value: ["[", "](http://)"].
    /// </summary>
    public class MarkdownInsertTexts
    {
        public string[] HorizontalRule { get; set; } = new[] { "", "\n\n-----\n\n" };

        public string[] Image { get; set; } = new[] { "![](", "#url#)" };

        public string[] Link { get; set; } = new[] { "[", "](#url#)" };

        public string[] Table { get; set; } = new[] { "", "\n\n| Column 1 | Column 2 | Column 3 |\n| -------- | -------- | -------- |\n| Text     | Text     | Text     |\n\n" };

        public string[] UploadedImage { get; set; } = new[] { "![](#url#)", "" };
    }
}
