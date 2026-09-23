using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazorise.Docs.Pages.Home.Components;

public partial class HomeShowcaseContacts
{
    private const int PageSize = 4;
    private string search = string.Empty;
    private string status = string.Empty;
    private string role = string.Empty;
    private int page = 1;
    private bool descending;
    private bool showLocation = true;
    private bool newContactVisible;
    private string newName = string.Empty;
    private string newLocation = string.Empty;
    private string newRole = "Developer";
    private string newStatus = "Lead";
    private int nextContactId = 9;
    private readonly string[] roles = ["Developer", "Designer", "Manager"];
    private readonly string[] statuses = ["Lead", "Customer", "Subscriber"];
    private readonly HashSet<int> selected = [2];

    private readonly List<Contact> contacts =
    [
        new( 1, "Alex Morgan", "AM", "New York, US", "Developer", "Lead", Color.Warning ),
        new( 2, "Aino Lahtinen", "AL", "Turku, FI", "Designer", "Subscriber", Color.Primary ),
        new( 3, "Clara Müller", "CM", "Berlin, DE", "Manager", "Customer", Color.Success ),
        new( 4, "Luca Schneider", "LS", "Geneva, CH", "Developer", "Lead", Color.Warning ),
        new( 5, "Mia Larsen", "ML", "Copenhagen, DK", "Designer", "Customer", Color.Success ),
        new( 6, "Sam Lee", "SL", "London, UK", "Developer", "Subscriber", Color.Primary ),
        new( 7, "Elena Rossi", "ER", "Milan, IT", "Manager", "Customer", Color.Success ),
        new( 8, "Ivan Horvat", "IH", "Zagreb, HR", "Developer", "Lead", Color.Warning ),
    ];

    private IReadOnlyList<Contact> FilteredContacts
    {
        get
        {
            string term = search.Trim();
            IEnumerable<Contact> filtered = contacts.Where( contact =>
                ( string.IsNullOrEmpty( status ) || contact.Status == status )
                && ( string.IsNullOrEmpty( role ) || contact.Role == role )
                && ( contact.Name.Contains( term, StringComparison.OrdinalIgnoreCase )
                    || contact.Location.Contains( term, StringComparison.OrdinalIgnoreCase ) ) );

            return ( descending
                ? filtered.OrderByDescending( contact => contact.Name, StringComparer.OrdinalIgnoreCase )
                : filtered.OrderBy( contact => contact.Name, StringComparer.OrdinalIgnoreCase ) ).ToArray();
        }
    }

    private static IReadOnlyList<Contact> GetPage( IReadOnlyList<Contact> contacts, int page )
        => contacts.Skip( ( page - 1 ) * PageSize ).Take( PageSize ).ToArray();

    private void SearchChanged( string value )
    {
        search = value ?? string.Empty;
        page = 1;
    }

    private void StatusChanged( string value )
    {
        status = value ?? string.Empty;
        page = 1;
    }

    private void ToggleSort()
    {
        descending = !descending;
        page = 1;
    }

    private void RoleChanged( string value )
    {
        role = value;
        page = 1;
    }

    private void ResetFilters()
    {
        search = string.Empty;
        status = string.Empty;
        role = string.Empty;
        page = 1;
    }

    private void OpenNewContact()
    {
        newName = string.Empty;
        newLocation = string.Empty;
        newRole = "Developer";
        newStatus = "Lead";
        newContactVisible = true;
    }

    private void AddContact()
    {
        if ( string.IsNullOrWhiteSpace( newName ) || string.IsNullOrWhiteSpace( newLocation ) )
        {
            return;
        }

        string name = newName.Trim();
        string initials = string.Concat( name.Split( ' ', StringSplitOptions.RemoveEmptyEntries ).Take( 2 ).Select( part => part[0] ) ).ToUpperInvariant();
        Color color = newStatus switch
        {
            "Customer" => Color.Success,
            "Subscriber" => Color.Primary,
            _ => Color.Warning,
        };
        Contact contact = new( nextContactId++, name, initials, newLocation.Trim(), newRole, newStatus, color );
        contacts.Add( contact );
        ResetFilters();
        page = FilteredContacts.ToList().FindIndex( item => item.Id == contact.Id ) / PageSize + 1;
        newContactVisible = false;
    }

    private bool AreAllSelected( IReadOnlyList<Contact> contacts )
        => contacts.All( contact => selected.Contains( contact.Id ) );

    private bool AreSomeSelected( IReadOnlyList<Contact> contacts )
        => contacts.Any( contact => selected.Contains( contact.Id ) ) && !AreAllSelected( contacts );

    private void SelectPage( IReadOnlyList<Contact> contacts, bool value )
    {
        foreach ( Contact contact in contacts )
        {
            SelectContact( contact.Id, value );
        }
    }

    private void SelectContact( int id, bool value )
    {
        if ( value )
        {
            selected.Add( id );
        }
        else
        {
            selected.Remove( id );
        }
    }

    private sealed record Contact( int Id, string Name, string Initials, string Location, string Role, string Status, Color Color );
}