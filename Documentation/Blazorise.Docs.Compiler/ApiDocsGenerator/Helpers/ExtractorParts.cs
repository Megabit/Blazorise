#region Using directives
using System;
#endregion

namespace Blazorise.Docs.Compiler.ApiDocsGenerator.Helpers;

public enum ExtractorParts
{
    Summary, Remarks
}

public static class ExtractorPartsExtensions
{
    public static string GetXmlTag( this ExtractorParts part ) =>
        part switch
        {
            ExtractorParts.Summary => "summary",
            ExtractorParts.Remarks => "remarks",
            _ => throw new ArgumentOutOfRangeException( nameof( part ), $"No XML tag defined for {part}" )
        };

    public static string GetDefault( this ExtractorParts part ) =>
        part switch
        {
            ExtractorParts.Summary => "No summary found",
            ExtractorParts.Remarks => string.Empty,
            _ => throw new ArgumentOutOfRangeException( nameof( part ), $"No default value defined for {part}" )
        };

}
