using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazorise.Demo.Pages.Tests;

public partial class TablesPage
{
    private Table tableRef;
    private int scrollToValue;
    private bool fixedHeader = true;
    private bool stripped;
    private bool bordered = true;
    private bool borderless;
    private bool hoverable;
    private bool small;
    private bool resizable = true;

    private TableResizeMode resizeMode;
    private TableResponsiveMode responsiveMode;

    private Task ScrollToRow()
        => tableRef.ScrollToRow( scrollToValue ).AsTask();

    private class TableUser
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

    private List<TableUser> tableUsers = new()
    {
        new( "1", "Mark", "Otto", "@mdo" ),
        new( "2", "Jacob", "Thornton", "@fat" ),
        new( "3", "Larry", "the Bird", "@twitter" ),
    };
}