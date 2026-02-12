using System;
using System.Globalization;

namespace Blazorise.Docs.Components.Commercial;

public partial class RoiCalculator
{
    private const int ComponentCountMin = 1;
    private const int ComponentCountMax = 50;
    private const int DeveloperCountMin = 1;
    private const int DeveloperCountMax = 50;
    private const int HoursPerComponentMin = 24;
    private const int HoursPerComponentMax = 320;
    private const int DeveloperRateMin = 35;
    private const int DeveloperRateMax = 250;
    private const int MonthsSavedMin = 0;
    private const int MonthsSavedMax = 24;
    private const int BusinessValuePerMonthMin = 0;
    private const int BusinessValuePerMonthMax = 500_000;
    private const int PlanningYearsMin = 1;
    private const int PlanningYearsMax = 5;
    private const int MaintenancePercentMin = 0;
    private const int MaintenancePercentMax = 40;
    private const int HardeningPercentMin = 0;
    private const int HardeningPercentMax = 50;
    private const int RiskPercentMin = 0;
    private const int RiskPercentMax = 50;

    private const decimal ProfessionalLicensePerDeveloperPerYear = 590m;
    private const decimal EnterpriseLicensePerDeveloperPerYear = 990m;
    private const decimal InternalPlatformOverheadPercent = 25m;
    private const decimal InternalCoordinationPercentPerAdditionalDeveloper = 3m;
    private const int CoordinationDeveloperThreshold = 3;
    private const decimal InternalCoordinationPercentCap = 30m;
    private const decimal BlazoriseCoordinationPercentPerAdditionalDeveloper = 1m;
    private const decimal BlazoriseCoordinationPercentCap = 10m;
    private const decimal BlazoriseDeliveryEffortPercent = 55m;
    private const decimal BlazoriseHardeningFactor = 0.65m;
    private const decimal BlazoriseRiskFactor = 0.55m;
    private const decimal WithBlazoriseMaintenanceFactor = 0.6m;

    private static readonly CultureInfo NumberCulture = CultureInfo.GetCultureInfo( "en-US" );

    private int componentCount = 12;
    private int developerCount = 6;
    private int hoursPerComponent = 140;
    private int developerCostPerHour = 85;
    private int monthsSaved = 4;
    private int businessValuePerMonth = 35_000;
    private int planningYears = 3;
    private int annualMaintenancePercent = 18;
    private int testingHardeningPercent = 15;
    private int riskComplexityPercent = 12;
    private LicenseTier selectedLicenseTier = LicenseTier.Enterprise;

    private RoiCalculationResult? roiResult;

    private decimal SelectedLicensePrice
        => selectedLicenseTier == LicenseTier.Professional
            ? ProfessionalLicensePerDeveloperPerYear
            : EnterpriseLicensePerDeveloperPerYear;

    private int ComponentCount
    {
        get => componentCount;
        set
        {
            componentCount = Clamp( value, ComponentCountMin, ComponentCountMax );
            RecalculateIfNeeded();
        }
    }

    private int HoursPerComponent
    {
        get => hoursPerComponent;
        set
        {
            hoursPerComponent = Clamp( value, HoursPerComponentMin, HoursPerComponentMax );
            RecalculateIfNeeded();
        }
    }

    private int DeveloperCount
    {
        get => developerCount;
        set
        {
            developerCount = Clamp( value, DeveloperCountMin, DeveloperCountMax );
            RecalculateIfNeeded();
        }
    }

    private int DeveloperCostPerHour
    {
        get => developerCostPerHour;
        set
        {
            developerCostPerHour = Clamp( value, DeveloperRateMin, DeveloperRateMax );
            RecalculateIfNeeded();
        }
    }

    private int MonthsSaved
    {
        get => monthsSaved;
        set
        {
            monthsSaved = Clamp( value, MonthsSavedMin, MonthsSavedMax );
            RecalculateIfNeeded();
        }
    }

    private int BusinessValuePerMonth
    {
        get => businessValuePerMonth;
        set
        {
            businessValuePerMonth = Clamp( value, BusinessValuePerMonthMin, BusinessValuePerMonthMax );
            RecalculateIfNeeded();
        }
    }

    private int PlanningYears
    {
        get => planningYears;
        set
        {
            planningYears = Clamp( value, PlanningYearsMin, PlanningYearsMax );
            RecalculateIfNeeded();
        }
    }

    private int AnnualMaintenancePercent
    {
        get => annualMaintenancePercent;
        set
        {
            annualMaintenancePercent = Clamp( value, MaintenancePercentMin, MaintenancePercentMax );
            RecalculateIfNeeded();
        }
    }

    private int TestingHardeningPercent
    {
        get => testingHardeningPercent;
        set
        {
            testingHardeningPercent = Clamp( value, HardeningPercentMin, HardeningPercentMax );
            RecalculateIfNeeded();
        }
    }

    private int RiskComplexityPercent
    {
        get => riskComplexityPercent;
        set
        {
            riskComplexityPercent = Clamp( value, RiskPercentMin, RiskPercentMax );
            RecalculateIfNeeded();
        }
    }

    private LicenseTier SelectedLicenseTier
    {
        get => selectedLicenseTier;
        set
        {
            selectedLicenseTier = value;
            RecalculateIfNeeded();
        }
    }

    private decimal ScopeAdjustmentMultiplier
        => 1m + ( TestingHardeningPercent + RiskComplexityPercent ) / 100m;

    private decimal PlanningHorizonYears
        => PlanningYears;

    protected override void OnInitialized()
    {
        Calculate();
    }

    private void Calculate()
    {
        decimal baseBuildHours = ComponentCount * HoursPerComponent;
        decimal internalCoordinationPercent = CalculateCoordinationPercent( DeveloperCount, InternalCoordinationPercentPerAdditionalDeveloper, InternalCoordinationPercentCap );
        decimal internalPlatformMultiplier = 1m + ( InternalPlatformOverheadPercent + internalCoordinationPercent ) / 100m;
        decimal adjustedBuildHours = baseBuildHours * ScopeAdjustmentMultiplier * internalPlatformMultiplier;
        decimal totalCostToBuildInternally = adjustedBuildHours * DeveloperCostPerHour;

        decimal annualMaintenanceCostInternal = totalCostToBuildInternally * ( AnnualMaintenancePercent / 100m );
        decimal totalCostOfOwnership = totalCostToBuildInternally + ( annualMaintenanceCostInternal * PlanningHorizonYears );

        decimal blazoriseScopeAdjustmentMultiplier = 1m + ( ( TestingHardeningPercent * BlazoriseHardeningFactor ) + ( RiskComplexityPercent * BlazoriseRiskFactor ) ) / 100m;
        decimal blazoriseCoordinationPercent = CalculateCoordinationPercent( DeveloperCount, BlazoriseCoordinationPercentPerAdditionalDeveloper, BlazoriseCoordinationPercentCap );
        decimal blazoriseCoordinationMultiplier = 1m + ( blazoriseCoordinationPercent / 100m );
        decimal blazoriseBuildHours = baseBuildHours * ( BlazoriseDeliveryEffortPercent / 100m ) * blazoriseScopeAdjustmentMultiplier * blazoriseCoordinationMultiplier;
        decimal blazoriseBuildCost = blazoriseBuildHours * DeveloperCostPerHour;
        decimal blazoriseAnnualMaintenanceCost = blazoriseBuildCost * ( AnnualMaintenancePercent / 100m ) * WithBlazoriseMaintenanceFactor;
        decimal blazoriseInternalCostForHorizon = blazoriseBuildCost + ( blazoriseAnnualMaintenanceCost * PlanningHorizonYears );
        int estimatedLicenseSeats = DeveloperCount;
        decimal licenseCostForHorizon = estimatedLicenseSeats * SelectedLicensePrice * PlanningHorizonYears;
        decimal costOfUsingBlazorise = blazoriseInternalCostForHorizon + licenseCostForHorizon;

        decimal estimatedSavings = totalCostOfOwnership - costOfUsingBlazorise;
        decimal timeToMarketAdvantage = MonthsSaved * BusinessValuePerMonth;

        roiResult = new RoiCalculationResult(
            totalCostToBuildInternally,
            totalCostOfOwnership,
            estimatedSavings,
            timeToMarketAdvantage,
            estimatedLicenseSeats,
            licenseCostForHorizon,
            adjustedBuildHours,
            blazoriseBuildHours,
            annualMaintenanceCostInternal,
            blazoriseAnnualMaintenanceCost );
    }

    private void RecalculateIfNeeded()
    {
        if ( roiResult is not null )
        {
            Calculate();
        }
    }

    private static int Clamp( int value, int min, int max )
        => Math.Min( max, Math.Max( min, value ) );

    private static decimal CalculateCoordinationPercent( int developerCount, decimal perAdditionalDeveloperPercent, decimal capPercent )
    {
        int additionalDevelopers = Math.Max( 0, developerCount - CoordinationDeveloperThreshold );
        decimal calculatedPercent = additionalDevelopers * perAdditionalDeveloperPercent;
        return Math.Min( capPercent, calculatedPercent );
    }

    private static string FormatCurrency( decimal value )
        => $"\u20AC{value.ToString( "n0", NumberCulture )}";

    private static string FormatSignedCurrency( decimal value )
        => value > 0m ? $"+{FormatCurrency( value )}" : value < 0m ? $"-{FormatCurrency( Math.Abs( value ) )}" : FormatCurrency( value );

    private static string FormatCompactNumber( int value )
    {
        if ( value >= 1_000_000 )
            return $"{value / 1_000_000m:0.#}M";

        if ( value >= 1_000 )
            return $"{value / 1_000m:0.#}K";

        return value.ToString( NumberCulture );
    }

    private static string FormatCompactCurrency( int value )
        => $"\u20AC{FormatCompactNumber( value )}";

    private static string FormatBreakdownHours( decimal value )
        => $"{value:n0}h";

    private enum LicenseTier
    {
        Professional,
        Enterprise,
    }

    private sealed record RoiCalculationResult(
        decimal TotalCostToBuildInternally,
        decimal TotalCostOfOwnership,
        decimal EstimatedSavings,
        decimal TimeToMarketAdvantage,
        int EstimatedLicenseSeats,
        decimal LicenseCostForHorizon,
        decimal InternalBuildHours,
        decimal BlazoriseBuildHours,
        decimal InternalAnnualMaintenanceCost,
        decimal BlazoriseAnnualMaintenanceCost );
}