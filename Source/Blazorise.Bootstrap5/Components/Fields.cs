namespace Blazorise.Bootstrap5.Components;

public class Fields : Blazorise.Fields
{
    /// <summary>
    /// Represents the default horizontal gutter configuration with a size of 3 for layout spacing.
    /// </summary>
    private static readonly IFluentGutter DefaultGutter = new Blazorise.FluentGutter().WithSize( GutterSize.Is3 ).OnX;

    protected override void OnInitialized()
    {
        if ( Gutter is null )
            Gutter = DefaultGutter;

        base.OnInitialized();
    }
}
