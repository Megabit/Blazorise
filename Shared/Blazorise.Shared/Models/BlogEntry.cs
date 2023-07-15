namespace Blazorise.Shared.Models;

public class BlogEntry
{
    public string Category { get; set; }
    public string Url { get; set; }
    public string Text { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public string AuthorName { get; set; }
    public string AuthorImage { get; set; }
    public string PostedOn { get; set; }
    public string ReadTime { get; set; }
    public bool Pinned { get; set; }
}