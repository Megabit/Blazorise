using System.Collections.Generic;

namespace Blazorise.Docs.Components;

public static class DocsCodeSources
{
    public static readonly IReadOnlyList<DocsCodeSource> CountrySharedData =
    [
        new( "CountryData", "CountryData.cs" ),
        new( "Country", "Country.cs" )
    ];

    public static readonly IReadOnlyList<DocsCodeSource> DataGridSharedData =
    [
        new( "DataGridEmployeeData", "EmployeeData.cs" ),
        new( "DataGridEmployee", "Employee.cs" ),
        new( "DataGridSalary", "Salary.cs" )
    ];

    public static readonly IReadOnlyList<DocsCodeSource> FluentValidationValidator =
    [
        new( "PersonValidator", "PersonValidator.cs" ),
        new( "Person", "Person.cs" )
    ];

    public static readonly IReadOnlyList<DocsCodeSource> ModalProviderCounter =
    [
        new( "CounterExample", "CounterExample.razor" )
    ];

    public static readonly IReadOnlyList<DocsCodeSource> ModalProviderCustomStructure =
    [
        new( "CustomStructureModalExample", "CustomStructureModalExample.razor" )
    ];

    public static readonly IReadOnlyList<DocsCodeSource> ModalProviderFormulary =
    [
        new( "FormularyModalExample", "FormularyModalExample.razor" )
    ];

    public static readonly IReadOnlyList<DocsCodeSource> OffcanvasProviderCounter =
    [
        new( "CounterExample", "CounterExample.razor" )
    ];

    public static readonly IReadOnlyList<DocsCodeSource> OffcanvasProviderCustomStructure =
    [
        new( "CustomStructureOffcanvasExample", "CustomStructureOffcanvasExample.razor" )
    ];

    public static readonly IReadOnlyList<DocsCodeSource> OffcanvasProviderFormulary =
    [
        new( "FormularyOffcanvasExample", "FormularyOffcanvasExample.razor" )
    ];
}