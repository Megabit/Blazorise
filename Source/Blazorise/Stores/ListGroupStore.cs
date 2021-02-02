namespace Blazorise.Stores
{
    public record ListGroupStore
    {
        public bool Flush { get; init; }

        public ListGroupMode Mode { get; init; }

        public string SelectedItem { get; init; }
    }
}
