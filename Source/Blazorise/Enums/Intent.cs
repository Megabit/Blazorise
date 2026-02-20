namespace Blazorise;

/// <summary>
/// Predefined set of semantic intents.
/// </summary>
public record Intent : Enumeration<Intent>
{
    /// <inheritdoc/>
    public Intent( string name ) : base( name )
    {
    }

    /// <inheritdoc/>
    private Intent( Intent parent, string name ) : base( parent, name )
    {
    }

    /// <summary>
    /// Creates the new custom intent based on the supplied enum value.
    /// </summary>
    /// <param name="name">Name value of the enum.</param>
    public static implicit operator Intent( string name )
    {
        return new Intent( name );
    }

    /// <summary>
    /// No intent will be applied to an element.
    /// </summary>
    public static readonly Intent Default = new( null );

    /// <summary>
    /// Primary intent.
    /// </summary>
    public static readonly Intent Primary = new( "primary" );

    /// <summary>
    /// Secondary intent.
    /// </summary>
    public static readonly Intent Secondary = new( "secondary" );

    /// <summary>
    /// Success intent.
    /// </summary>
    public static readonly Intent Success = new( "success" );

    /// <summary>
    /// Danger intent.
    /// </summary>
    public static readonly Intent Danger = new( "danger" );

    /// <summary>
    /// Warning intent.
    /// </summary>
    public static readonly Intent Warning = new( "warning" );

    /// <summary>
    /// Info intent.
    /// </summary>
    public static readonly Intent Info = new( "info" );

    /// <summary>
    /// Light intent.
    /// </summary>
    public static readonly Intent Light = new( "light" );

    /// <summary>
    /// Dark intent.
    /// </summary>
    public static readonly Intent Dark = new( "dark" );
}