namespace Blazorise
{
    /// <summary>
    /// Predefined set of contextual text colors.
    /// </summary>
    public record TextColor : Enumeration<TextColor>
    {
        /// <inheritdoc/>
        public TextColor( string name ) : base( name )
        {
        }

        /// <inheritdoc/>
        private TextColor( TextColor parent, string name ) : base( parent, name )
        {
        }

        /// <summary>
        /// Creates the new custom text color based on the supplied enum value.
        /// </summary>
        /// <param name="name">Name value of the enum.</param>
        public static implicit operator TextColor( string name )
        {
            return new TextColor( name );
        }

        /// <summary>
        /// No color will be applied to an element, meaning it will appear as default to whatever current theme is set to.
        /// </summary>
        public static readonly TextColor Default = new( (string)null );

        /// <summary>
        /// Primary color.
        /// </summary>
        public static readonly TextColor Primary = new( "primary" );

        /// <summary>
        /// Secondary color.
        /// </summary>
        public static readonly TextColor Secondary = new( "secondary" );

        /// <summary>
        /// Success color.
        /// </summary>
        public static readonly TextColor Success = new( "success" );

        /// <summary>
        /// Danger color.
        /// </summary>
        public static readonly TextColor Danger = new( "danger" );

        /// <summary>
        /// Warning color.
        /// </summary>
        public static readonly TextColor Warning = new( "warning" );

        /// <summary>
        /// Info color.
        /// </summary>
        public static readonly TextColor Info = new( "info" );

        /// <summary>
        /// Light color.
        /// </summary>
        public static readonly TextColor Light = new( "light" );

        /// <summary>
        /// Dark color.
        /// </summary>
        public static readonly TextColor Dark = new( "dark" );

        /// <summary>
        /// Body color.
        /// </summary>
        public static readonly TextColor Body = new( "body" );

        /// <summary>
        /// Muted color.
        /// </summary>
        public static readonly TextColor Muted = new( "muted" );

        /// <summary>
        /// White color.
        /// </summary>
        public static readonly TextColor White = new( "white" );

        /// <summary>
        /// Black text with 50% opacity on white background.
        /// </summary>
        public static readonly TextColor Black50 = new( "black-50" );

        /// <summary>
        /// White text with 50% opacity on black background.
        /// </summary>
        public static readonly TextColor White50 = new( "white-50" );
    }
}
