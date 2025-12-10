namespace Blazorise.Docs.Options;

public class BrevoApiOptions
{
    public const string SectionName = "BrevoApi";

    public string ApiKey { get; set; }

    public string BaseUrl { get; set; } = "https://api.brevo.com/v3/";
}