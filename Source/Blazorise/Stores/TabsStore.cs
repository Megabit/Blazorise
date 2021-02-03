namespace Blazorise.Stores
{
    public record TabsStore
    {
        public bool Pills { get; init; }

        public bool FullWidth { get; init; }

        public bool Justified { get; init; }

        public TabPosition TabPosition { get; init; }

        public string SelectedTab { get; init; }
    }
}
