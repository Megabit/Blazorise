using System.Collections.Generic;

namespace Blazorise.Demo.Pages.Tests
{
    public partial class TablesPage
    {
        bool fixedHeader = true;
        bool stripped;
        bool bordered;
        bool borderless;
        bool hoverable;
        bool small;

        class TableUser
        {
            public string Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Social { get; set; }

            public TableUser( string id, string firstName, string lastName, string social )
            {
                Id = id;
                FirstName = firstName;
                LastName = lastName;
                Social = social;
            }
        }

        List<TableUser> tableUsers = new List<TableUser>
        {
            new TableUser("1", "Mark", "Otto", "@mdo"),
            new TableUser("2", "Jacob", "Thornton", "@fat"),
            new TableUser("3", "Larry", "the Bird", "@twitter"),
        };
    }
}
