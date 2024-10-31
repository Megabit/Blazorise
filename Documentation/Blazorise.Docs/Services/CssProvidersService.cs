namespace Blazorise.Docs.Services;

using System.Collections.Generic;

public static class CssProvidersService
{
    public static readonly Dictionary<CssProvider, ProviderData> ProvidersData = new()
    {
        { CssProvider.Bootstrap, new ProviderData("bootstrapdemo", "bootstrap-4.svg", "Bootstrap 4 image", "Bootstrap 4", "bootstrap4") },
        { CssProvider.Bootstrap5, new ProviderData("bootstrap5demo", "bootstrap-5.svg", "Bootstrap 5 image", "Bootstrap 5", "bootstrap5") },
        { CssProvider.Tailwind, new ProviderData("tailwinddemo", "tailwind.svg", "Tailwind image", "Tailwind", "tailwind") },
        { CssProvider.Material, new ProviderData("materialdemo", "material.svg", "Material image", "Material", "material") },
        { CssProvider.AntDesign, new ProviderData("antdesigndemo", "ant-design.svg", "AntDesign image", "Ant Design", "ant-design") },
        { CssProvider.Bulma, new ProviderData("bulmademo", "bulma.svg", "Bulma image", "Bulma", "bulma") },
        { CssProvider.FluentUi2, new ProviderData("fluentuidemo", "fluent2.svg", "Fluent 2 image", "Fluent 2", "fluentui") }
    };

    public static ProviderData GetProvider( CssProvider provider ) => ProvidersData.TryGetValue( provider, out var data )
        ? data
        : throw new KeyNotFoundException( $"Provider data for {provider} was not found." );
}

public record ProviderData( string SubPage, string ImageFileName, string ImageAltText, string Title, string UsageLink )
{
    public string TitleDemo => $"{Title} Demo";
}

public enum CssProvider
{
    Bootstrap,
    Bootstrap5,
    Tailwind,
    Material,
    AntDesign,
    Bulma,
    FluentUi2
}