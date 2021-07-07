using System.Collections.Generic;

namespace Blazorise.Demo.Pages.Tests
{
    public partial class TablesPage
    {
        bool fixedHeader = true;
        bool stripped;
        bool bordered = true;
        bool borderless;
        bool hoverable;
        bool small;
        bool resizable = true;

        TableResizeMode resizeMode;

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

        List<TableUser> tableUsers = new()
        {
            new("1", "Mark", "Otto", "@mdo"),
            new("2", "Jacob", "Thornton", "@fat"),
            new("3", "Larry", "the Bird", "@twitter"),
        };
    }
}
