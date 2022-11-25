namespace Blazorise.Shared.Models;

public class PageEntry
{
    public PageEntry( string url, string name )
    {
        Url = url;
        Name = name;
    }

    public PageEntry( string url, string name, string description )
        : this( url, name )
    {
        Description = description;
    }

    public string Url { get; }

    public string Name { get; }

    public string Description { get; }
}