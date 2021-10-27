namespace Blazorise.Shared.Models
{
    public class Country
    {
        public Country( string name, string iso, string capital )
        {
            Name = name;
            Iso = iso;
            Capital = capital;
        }

        public string Name { get; }
        public string Iso { get; }
        public string Capital { get; }
    }
}