#region Using directives
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Blazorise.Licensing;
using Xunit;
using License = Blazorise.Licensing.License;
#endregion

namespace Blazorise.Tests;

public class BlazoriseLicenseTest
{
    [Theory]
    [InlineData( false )]
    [InlineData( true )]
    public async Task IndieToken_AllowsEachRegisteredApplicationAssembly( bool useLibraryAssembly )
    {
        var applicationAssembly = useLibraryAssembly ? typeof( BlazoriseOptions ).Assembly : typeof( BlazoriseLicenseTest ).Assembly;
        var assemblies = JsonSerializer.Serialize( new[]
        {
            typeof( BlazoriseLicenseTest ).Assembly.GetName().Name,
            typeof( BlazoriseOptions ).Assembly.GetName().Name,
        } );
        var provider = CreateProvider( applicationAssembly );

        await provider.Initialization;

        provider.ApplyLicense( CreateLicense( "Indie", "MyProduct", assemblies ), true );

        var checker = new BlazoriseLicenseChecker( provider );

        Assert.Equal( BlazoriseLicenseResult.Licensed, provider.Result );
        Assert.Equal( BlazoriseLicensePrintResult.Licensed, provider.PrintResult );
        Assert.Equal( "MyProduct", provider.ProjectName );
        Assert.NotNull( provider.License );
        Assert.False( checker.ShouldPrint() );
        Assert.False( checker.ShowBanner() );
        Assert.False( checker.ShowCompactBanner() );
        Assert.Null( checker.GetDataGridRowsLimit() );
        Assert.Null( checker.GetAutocompleteRowsLimit() );
        Assert.Null( checker.GetChartsRowsLimit() );
        Assert.Null( checker.GetListViewRowsLimit() );
        Assert.Null( checker.GetTreeViewRowsLimit() );
        Assert.Null( checker.GetPivotGridRowsLimit() );
        Assert.Null( checker.GetGanttRowsLimit() );
        Assert.Null( checker.GetReportingRowsLimit() );
        Assert.Null( checker.GetTransferListRowsLimit() );
    }

    [Theory]
    [InlineData( null, "[\"Blazorise.Tests\"]" )]
    [InlineData( " ", "[\"Blazorise.Tests\"]" )]
    [InlineData( "MyProduct", null )]
    [InlineData( "MyProduct", "" )]
    [InlineData( "MyProduct", "[]" )]
    [InlineData( "MyProduct", "null" )]
    [InlineData( "MyProduct", "{}" )]
    [InlineData( "MyProduct", "\"Blazorise.Tests\"" )]
    [InlineData( "MyProduct", "[\"Blazorise.Tests\"" )]
    [InlineData( "MyProduct", "[\"Blazorise.Tests\", null]" )]
    [InlineData( "MyProduct", "[\"Blazorise.Tests\", 42]" )]
    [InlineData( "MyProduct", "[\"Blazorise.Tests\", \" \"]" )]
    [InlineData( "MyProduct", "[\"OtherProject\"]" )]
    [InlineData( "MyProduct", "[\"blazorise.tests\"]" )]
    [InlineData( "MyProduct", "[\"Blazorise.*\"]" )]
    [InlineData( "MyProduct", "[\"Blazorise.Tests.Client\"]" )]
    [InlineData( "MyProduct", "[\"Blazorise.Tests.dll\"]" )]
    public async Task IndieToken_RejectsMissingMalformedOrMismatchedProjectMetadata( string projectName, string assemblies )
    {
        var provider = CreateProvider( typeof( BlazoriseLicenseTest ).Assembly );

        await provider.Initialization;

        provider.ApplyLicense( CreateLicense( "Indie", projectName, assemblies ), true );

        var checker = new BlazoriseLicenseChecker( provider );

        Assert.Equal( BlazoriseLicenseResult.Unlicensed, provider.Result );
        Assert.Equal( BlazoriseLicensePrintResult.InvalidProject, provider.PrintResult );
        Assert.Null( provider.License );
        Assert.True( checker.ShouldPrint() );
        Assert.False( checker.ShouldPrint() );
        Assert.True( checker.ShowBanner() );
        Assert.False( checker.ShowCompactBanner() );
        Assert.Contains( "ProductAssembly", checker.GetPrintMessage() );
        AssertEvaluationLimits( checker );
    }

    [Fact]
    public async Task IndieToken_RequiresExplicitApplicationAssembly()
    {
        var provider = CreateProvider();

        await provider.Initialization;

        provider.ApplyLicense( CreateLicense( "Indie", "MyProduct", "[\"Blazorise.Tests\"]" ), true );

        Assert.Equal( BlazoriseLicensePrintResult.InvalidProject, provider.PrintResult );
        Assert.Equal( BlazoriseLicenseResult.Unlicensed, provider.Result );
    }

    [Theory]
    [InlineData( "Regular", BlazoriseLicenseResult.Licensed, BlazoriseLicensePrintResult.Licensed )]
    [InlineData( "Trial", BlazoriseLicenseResult.Trial, BlazoriseLicensePrintResult.Trial )]
    [InlineData( "Community", BlazoriseLicenseResult.Community, BlazoriseLicensePrintResult.Community )]
    [InlineData( null, BlazoriseLicenseResult.Licensed, BlazoriseLicensePrintResult.Licensed )]
    public async Task LegacyToken_DoesNotRequireProjectMetadataOrApplicationAssembly( string licenseType, BlazoriseLicenseResult result, BlazoriseLicensePrintResult printResult )
    {
        var provider = CreateProvider();
        var license = CreateLicense( licenseType, dataGridMaxRows: "123" );

        await provider.Initialization;

        provider.ApplyLicense( license, true );

        var checker = new BlazoriseLicenseChecker( provider );

        Assert.Equal( result, provider.Result );
        Assert.Equal( printResult, provider.PrintResult );
        Assert.Null( provider.ProjectName );
        Assert.Equal( 123, checker.GetDataGridRowsLimit() );
        Assert.False( checker.ShowBanner() );
        Assert.Equal( licenseType == "Community", checker.ShowCompactBanner() );
    }

    [Theory]
    [InlineData( "Regular", BlazoriseLicensePrintResult.LicensedExpired )]
    [InlineData( "Trial", BlazoriseLicensePrintResult.TrialExpired )]
    [InlineData( "Community", BlazoriseLicensePrintResult.CommunityExpired )]
    [InlineData( "Indie", BlazoriseLicensePrintResult.LicensedExpired )]
    public async Task UnverifiedToken_CannotGrantAccessEvenWithMatchingProject( string licenseType, BlazoriseLicensePrintResult printResult )
    {
        var provider = CreateProvider( typeof( BlazoriseLicenseTest ).Assembly );

        await provider.Initialization;

        provider.ApplyLicense( CreateLicense( licenseType, "MyProduct", "[\"Blazorise.Tests\"]" ), false );

        Assert.Equal( BlazoriseLicenseResult.Unlicensed, provider.Result );
        Assert.Equal( printResult, provider.PrintResult );
        Assert.Null( provider.License );
        Assert.Null( provider.ProjectName );
        AssertEvaluationLimits( new BlazoriseLicenseChecker( provider ) );
    }

    [Fact]
    public async Task ApplicationProviders_DoNotShareLicenseResults()
    {
        var matchingProvider = CreateProvider( typeof( BlazoriseLicenseTest ).Assembly );
        var otherProvider = CreateProvider( typeof( BlazoriseOptions ).Assembly );
        var license = CreateLicense( "Indie", "MyProduct", "[\"Blazorise.Tests\"]" );

        await Task.WhenAll( matchingProvider.Initialization, otherProvider.Initialization );

        matchingProvider.ApplyLicense( license, true );
        otherProvider.ApplyLicense( license, true );

        Assert.Equal( BlazoriseLicenseResult.Licensed, matchingProvider.Result );
        Assert.Equal( BlazoriseLicenseResult.Unlicensed, otherProvider.Result );
        Assert.Equal( BlazoriseLicensePrintResult.InvalidProject, otherProvider.PrintResult );
        Assert.Null( matchingProvider.GetChartsRowsLimit() );
        Assert.Equal( BlazoriseLicenseProvider.DEFAULT_UNLICENSED_LIMIT_CHARTS_MAX_ROWS, otherProvider.GetChartsRowsLimit() );
        Assert.Contains( "MyProduct", new BlazoriseLicenseChecker( otherProvider ).GetPrintMessage() );
    }

    [Fact]
    public async Task RejectedToken_DoesNotRetainPreviouslyAcceptedLicenseOrLimits()
    {
        var provider = CreateProvider( typeof( BlazoriseLicenseTest ).Assembly );
        var license = CreateLicense( "Community", dataGridMaxRows: "123" );

        await provider.Initialization;

        provider.ApplyLicense( license, true );
        Assert.Equal( 123, provider.GetDataGridRowsLimit() );

        provider.ApplyLicense( CreateLicense( "Indie", "MyProduct", "[\"OtherProject\"]" ), true );

        Assert.Null( provider.License );
        AssertEvaluationLimits( new BlazoriseLicenseChecker( provider ) );
    }

    [Theory]
    [InlineData( null, BlazoriseLicensePrintResult.None )]
    [InlineData( "not-a-product-token", BlazoriseLicensePrintResult.InvalidProductToken )]
    public async Task MissingOrMalformedToken_KeepsEvaluationLimits( string token, BlazoriseLicensePrintResult printResult )
    {
        var options = new BlazoriseOptions( null, configuredOptions => configuredOptions.ProductToken = token );
        var provider = new BlazoriseLicenseProvider( options, null, null );

        await provider.Initialization;

        Assert.Equal( BlazoriseLicenseResult.Unlicensed, provider.Result );
        Assert.Equal( printResult, provider.PrintResult );
        AssertEvaluationLimits( new BlazoriseLicenseChecker( provider ) );
    }

    private static BlazoriseLicenseProvider CreateProvider( Assembly productAssembly = null )
    {
        var options = new BlazoriseOptions( null, configuredOptions => configuredOptions.ProductAssembly = productAssembly );

        return new BlazoriseLicenseProvider( options, null, null );
    }

    private static License CreateLicense( string licenseType, string projectName = null, string applicationAssemblies = null, string dataGridMaxRows = null )
    {
        var properties = new Dictionary<string, string>();

        if ( licenseType is not null )
        {
            properties["LicenseType"] = licenseType;
        }

        if ( projectName is not null )
        {
            properties["PROJECT_NAME"] = projectName;
        }

        if ( applicationAssemblies is not null )
        {
            properties["APPLICATION_ASSEMBLIES"] = applicationAssemblies;
        }

        if ( dataGridMaxRows is not null )
        {
            properties["__DATAGRID__MAX__ROWS__"] = dataGridMaxRows;
        }

        return new License( DateTime.UtcNow, DateTime.UtcNow.AddYears( 1 ), properties );
    }

    private static void AssertEvaluationLimits( BlazoriseLicenseChecker checker )
    {
        Assert.Equal( BlazoriseLicenseProvider.DEFAULT_UNLICENSED_LIMIT_DATAGRID_MAX_ROWS, checker.GetDataGridRowsLimit() );
        Assert.Equal( BlazoriseLicenseProvider.DEFAULT_UNLICENSED_LIMIT_AUTOCOMPLETE_MAX_ROWS, checker.GetAutocompleteRowsLimit() );
        Assert.Equal( BlazoriseLicenseProvider.DEFAULT_UNLICENSED_LIMIT_CHARTS_MAX_ROWS, checker.GetChartsRowsLimit() );
        Assert.Equal( BlazoriseLicenseProvider.DEFAULT_UNLICENSED_LIMIT_LISTVIEW_MAX_ROWS, checker.GetListViewRowsLimit() );
        Assert.Equal( BlazoriseLicenseProvider.DEFAULT_UNLICENSED_LIMIT_TREEVIEW_MAX_ROWS, checker.GetTreeViewRowsLimit() );
        Assert.Equal( BlazoriseLicenseProvider.DEFAULT_UNLICENSED_LIMIT_PIVOTGRID_MAX_ROWS, checker.GetPivotGridRowsLimit() );
        Assert.Equal( BlazoriseLicenseProvider.DEFAULT_UNLICENSED_LIMIT_GANTT_MAX_ROWS, checker.GetGanttRowsLimit() );
        Assert.Equal( BlazoriseLicenseProvider.DEFAULT_UNLICENSED_LIMIT_REPORTING_MAX_ROWS, checker.GetReportingRowsLimit() );
        Assert.Equal( BlazoriseLicenseProvider.DEFAULT_UNLICENSED_LIMIT_TRANSFERVIEW_MAX_ROWS, checker.GetTransferListRowsLimit() );
    }
}