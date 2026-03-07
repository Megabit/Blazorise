namespace Blazorise.Docs.Services;

using System.Collections.Generic;

public static class CssProvidersService
{
    public static readonly Dictionary<CssProvider, ProviderData> ProvidersData = new()
    {
        { CssProvider.Bootstrap, new ProviderData("wasm/bootstrap4", "bootstrap-4.svg", "Bootstrap 4 image", "Bootstrap 4", "bootstrap4") },
        { CssProvider.Bootstrap5, new ProviderData("wasm/bootstrap5", "bootstrap-5.svg", "Bootstrap 5 image", "Bootstrap 5", "bootstrap5") },
        { CssProvider.Tailwind, new ProviderData("wasm/tailwind", "tailwind.svg", "Tailwind image", "Tailwind", "tailwind") },
        { CssProvider.Material, new ProviderData("wasm/material", "material.svg", "Material image", "Material", "material") },
        { CssProvider.AntDesign, new ProviderData("wasm/antdesign", "ant-design.svg", "AntDesign image", "Ant Design", "ant-design") },
        { CssProvider.Bulma, new ProviderData("wasm/bulma", "bulma.svg", "Bulma image", "Bulma", "bulma") },
        { CssProvider.FluentUi2, new ProviderData("wasm/fluentui2", "fluent2.svg", "Fluent 2 image", "Fluent 2", "fluent2") }
    };

    public static ProviderData GetProvider( CssProvider provider ) => ProvidersData.TryGetValue( provider, out var data )
        ? data
        : throw new KeyNotFoundException( $"Provider data for {provider} was not found." );
}

public record ProviderData( string DemoPath, string ImageFileName, string ImageAltText, string Title, string UsageLink )
{
    public string DemoUrl => $"https://demos.blazorise.com/{DemoPath}/";
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