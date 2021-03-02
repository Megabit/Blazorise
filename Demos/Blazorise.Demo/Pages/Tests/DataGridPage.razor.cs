#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.DataGrid;
#endregion

namespace Blazorise.Demo.Pages.Tests
{
    public partial class DataGridPage
    {
        #region Types

        public class Employee
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string EMail { get; set; }
            public string City { get; set; }
            public string Zip { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public int? Childrens { get; set; }
            public string Gender { get; set; }
            public decimal Salary { get; set; }
            public bool IsActive { get; set; }

            public List<Salary> Salaries { get; set; } = new List<Salary>();
        }

        public class Salary
        {
            public DateTime Date { get; set; }
            public decimal Total { get; set; }
        }

        #endregion

        #region Members

        DataGridEditMode editMode = DataGridEditMode.Form;
        DataGridSortMode sortMode = DataGridSortMode.Multiple;
        DataGridSelectionMode selectionMode = DataGridSelectionMode.Single;
        DataGridCommandMode commandsMode = DataGridCommandMode.Commands;
        DataGridResizeMode resizableMode = DataGridResizeMode.Header;

        DataGrid<Employee> dataGrid;
        public int currentPage { get; set; } = 1;

        bool editable = true;
        bool resizable = true;
        bool sortable = true;
        bool filterable = true;
        bool showPager = true;
        bool showPageSizes = true;
        bool largeDataMode = false;
        bool showButtonRow = true;

        Employee selectedEmployee;
        List<Employee> selectedEmployees;

        List<Employee> employeeList;
        int totalEmployees;

        Random random = new Random();

        // generated with https://mockaroo.com/
        List<Employee> dataModels = new List<Employee>{
            new Employee {Id = 1,FirstName = "Caro",LastName = "Nizard",EMail = "cnizard0@hc360.com",City = "Faīẕābād",Zip = null,Salary = 51724.19m, DateOfBirth = new DateTime(1983,5,8),
                Salaries = new List<Salary> {
                    new Salary { Date = new DateTime(2019,1,6), Total = 6000 },
                    new Salary { Date = new DateTime(2019,2,7), Total = 5005 },
                    new Salary { Date = new DateTime(2019,3,5), Total = 3000 }
                }
            },
            new Employee {Id = 2,FirstName = "Matthew",LastName = "Labb",EMail = "mlabb1@ca.gov",City = "Xinxi",Zip = null,Salary = 65176.6m, Childrens=2},
            new Employee {Id = 3,FirstName = "Enos",LastName = "Clendennen",EMail = "eclendennen2@shareasale.com",City = "Listvyanskiy",Zip = "633224",Salary = 75602.48m, Childrens=1,
                Salaries = new List<Salary> {
                    new Salary { Date = new DateTime(2019,2,7), Total = 4005 },
                    new Salary { Date = new DateTime(2019,3,5), Total = 8000 }
                }
            },
            new Employee {Id = 4,FirstName = "Cirilo",LastName = "Douch",EMail = "cdouch3@thetimes.co.uk",City = "Wiset Chaichan",Zip = "84280",Salary = 88511.38m, IsActive = true },
            new Employee {Id = 5,FirstName = "Bibbie",LastName = "Prahm",EMail = "bprahm4@dropbox.com",City = "Nkandla",Zip = "3859",Salary = 41665.0m },
            new Employee {Id = 6,FirstName = "Ferd",LastName = "Bizzey",EMail = "fbizzey5@vimeo.com",City = "Arroyo Seco",Zip = "5196",Salary = 58632.74m, IsActive = true },
            new Employee {Id = 7,FirstName = "Annalee",LastName = "Mathie",EMail = "amathie6@qq.com",City = "Qi’an",Zip = null,Salary = 38622.71m },
            new Employee {Id = 8,FirstName = "Sarajane",LastName = "Sarney",EMail = "ssarney7@phoca.cz",City = "Wagini",Zip = null,Salary = 67163.94m },
            new Employee {Id = 9,FirstName = "Lissa",LastName = "Clemenzi",EMail = "lclemenzi8@si.edu",City = "Lijiang",Zip = null,Salary = 67078.77m },
            new Employee {Id = 10,FirstName = "Taber",LastName = "Kowal",EMail = "tkowal9@ustream.tv",City = "Muhos",Zip = "91501",Salary = 70385.0m },
            new Employee {Id = 11,FirstName = "Christyna",LastName = "Blaylock",EMail = "cblaylocka@gov.uk",City = "Kruševo",Zip = "34320",Salary = 20626.15m, Childrens=4 },
            new Employee {Id = 12,FirstName = "Honoria",LastName = "Stirtle",EMail = "hstirtleb@ox.ac.uk",City = "Muang Phôn-Hông",Zip = null,Salary = 48999.42m, Childrens=1 },
            new Employee {Id = 13,FirstName = "Gregory",LastName = "Sinden",EMail = "gsindenc@go.com",City = "Kampunglistrik",Zip = null,Salary = 38097.16m, Childrens=2 },
            new Employee {Id = 14,FirstName = "Obediah",LastName = "Stroban",EMail = "ostroband@nbcnews.com",City = "Almoínhas Velhas",Zip = "2755-163",Salary = 83997.47m },
            new Employee {Id = 15,FirstName = "Kellen",LastName = "Zanotti",EMail = "kzanottie@123-reg.co.uk",City = "Türkmenabat",Zip = null,Salary = 37339.0m },
            new Employee {Id = 16,FirstName = "Luelle",LastName = "Mowles",EMail = "lmowlesf@wikimedia.org",City = "Durham",Zip = "27717",Salary = 89879.64m },
            new Employee {Id = 17,FirstName = "Venita",LastName = "Petkovic",EMail = "vpetkovicg@twitpic.com",City = "Radoboj",Zip = "49232",Salary = 22979.32m },
            new Employee {Id = 18,FirstName = "Gates",LastName = "Neat",EMail = "gneath@youtu.be",City = "Solna",Zip = "170 77",Salary = 75811.63m },
            new Employee {Id = 19,FirstName = "Roland",LastName = "Frangleton",EMail = "rfrangletoni@umich.edu",City = "Tío Pujio",Zip = "5936",Salary = 58971.76m, Childrens=3 },
            new Employee {Id = 20,FirstName = "Ferdinande",LastName = "Pidcock",EMail = "fpidcockj@independent.co.uk",City = "Paris 11",Zip = "75547 CEDEX 11",Salary = 82223.65m },
            new Employee {Id = 21,FirstName = "Clarie",LastName = "Crippin",EMail = "ccrippink@lycos.com",City = "Gostyń",Zip = "63-816",Salary = 79390.13m },
            new Employee {Id = 22,FirstName = "Israel",LastName = "Carlin",EMail = "icarlinl@washingtonpost.com",City = "Poitiers",Zip = "86042 CEDEX 9",Salary = 36875.18m },
            new Employee {Id = 23,FirstName = "Christoper",LastName = "Moorton",EMail = "cmoortonm@gizmodo.com",City = "Jambangan",Zip = null,Salary = 76787.57m },
            new Employee {Id = 24,FirstName = "Trina",LastName = "Seamen",EMail = "tseamenn@foxnews.com",City = "Song",Zip = "54120",Salary = 43598.06m },
            new Employee {Id = 25,FirstName = "Douglass",LastName = "Amor",EMail = "damoro@house.gov",City = "Castillos",Zip = null,Salary = 49865.8m, Childrens=2, IsActive = true },
            new Employee {Id = 26,FirstName = "Reeta",LastName = "Acom",EMail = "racomp@fc2.com",City = "Baoping",Zip = null,Salary = 61296.4m },
            new Employee {Id = 27,FirstName = "Chandler",LastName = "Franzonetti",EMail = "cfranzonettiq@archive.org",City = "Emin",Zip = null,Salary = 67458.07m, Childrens=1 }
        };

        #endregion

        #region Methods

        public void CheckEMail( ValidatorEventArgs validationArgs )
        {
            ValidationRule.IsEmail( validationArgs );

            if ( validationArgs.Status == ValidationStatus.Error )
            {
                validationArgs.ErrorText = "EMail has to be valid email";
            }
        }

        public void CheckFirstName( ValidatorEventArgs validationArgs )
        {
            ValidationRule.IsNotEmpty( validationArgs );

            if ( validationArgs.Status == ValidationStatus.Error )
            {
                validationArgs.ErrorText = "First name has to be provided";
            }
        }

        void OnEmployeeNewItemDefaultSetter( Employee employee )
        {
            employee.Salary = 100.0M;
            employee.IsActive = true;
        }

        void OnRowInserted( SavedRowItem<Employee, Dictionary<string, object>> e )
        {
            //var employee = e.Item;

            //employee.Id = dataModels?.Max( x => x.Id ) + 1 ?? 1;

            //dataModels.Add( employee );
        }

        void OnRowUpdated( SavedRowItem<Employee, Dictionary<string, object>> e )
        {
            //var employee = e.Item;

            //employee.FirstName = (string)e.Values["FirstName"];
            //employee.LastName = (string)e.Values["LastName"];
            //employee.EMail = (string)e.Values["EMail"];
            //employee.City = (string)e.Values["City"];
            //employee.Zip = (string)e.Values["Zip"];
            //employee.DateOfBirth = (DateTime?)e.Values["DateOfBirth"];
            //employee.Childrens = (int?)e.Values["Childrens"];
            //employee.Gender = (string)e.Values["Gender"];
            //employee.Salary = (decimal)e.Values["Salary"];
        }

        void OnRowRemoved( Employee model )
        {
            //if ( dataModels.Contains( model ) )
            //{
            //    dataModels.Remove( model );
            //}
        }

        string customFilterValue;

        bool OnCustomFilter( Employee model )
        {
            if ( string.IsNullOrEmpty( customFilterValue ) )
                return true;

            return
                model.FirstName?.Contains( customFilterValue, StringComparison.OrdinalIgnoreCase ) == true
                || model.LastName?.Contains( customFilterValue, StringComparison.OrdinalIgnoreCase ) == true
                || model.EMail?.Contains( customFilterValue, StringComparison.OrdinalIgnoreCase ) == true;
        }

        async Task OnReadData( DataGridReadDataEventArgs<Employee> e )
        {
            await Task.Delay( random.Next( 800 ) );

            if ( !e.CancellationToken.IsCancellationRequested )
            {
                // this can be call to anything, in this case we're calling a fictional api
                var response = dataModels.Skip( ( e.Page - 1 ) * e.PageSize ).Take( e.PageSize ).ToList();

                employeeList = new List<Employee>( response ); // an actual data for the current page
                totalEmployees = dataModels.Count; // this is used to tell datagrid how many items are available so that pagination will work

                // always call StateHasChanged!
                await InvokeAsync( StateHasChanged );
            }
        }

        Task Reload()
        {
            currentPage = 1;
            return dataGrid.Reload();
        }

        #endregion
    }
}
