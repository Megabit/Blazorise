namespace Blazorise.Docs.Models.ApiDocsDtos;

/// <summary>
/// Extracts common properies of Parameters, Events and Methods for api docs 
/// </summary>
public interface IApiDocsRecord
{
    string Summary { get; set; }
    string Name { get; set; }
}