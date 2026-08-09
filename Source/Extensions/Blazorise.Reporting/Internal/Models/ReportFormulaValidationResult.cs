namespace Blazorise.Reporting.Internal;

internal readonly record struct ReportFormulaValidationResult( bool Success, string Message, int Position = 0, int Length = 0 );