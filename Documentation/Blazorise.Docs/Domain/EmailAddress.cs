namespace Blazorise.Docs.Domain
{
    public class EmailAddress
    {
        public EmailAddress( string name, string address )
        {
            Name = name;
            Address = address;
        }

        public string Name { get; set; }

        public string Address { get; set; }
    }
}
