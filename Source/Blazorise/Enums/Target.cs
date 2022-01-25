namespace Blazorise
{
    /// <summary>
    /// The target attribute specifies where to open the linked document.
    /// </summary>
    public record struct Target
    {
        /// <summary>
        /// Gets the enum named value.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// A default target contructor.
        /// </summary>
        /// <param name="name">Name value of the enum.</param>
        public Target( string name )
        {
            Name = name;
        }

        /// <summary>
        /// Creates the new custom target based on the supplied enum value.
        /// </summary>
        /// <param name="name">Name value of the enum.</param>
        public static implicit operator Target( string name )
        {
            return new Target( name );
        }

        /// <summary>
        /// No target will be applied. Usually this is the same as <see cref="Target.Self"/>.
        /// </summary>
        public static readonly Target None = new( (string)null );

        /// <summary>
        /// Opens the linked document in the same frame as it was clicked (this is default).
        /// </summary>
        public static readonly Target Self = new( "self" );

        /// <summary>
        /// Opens the linked document in a new window or tab.
        /// </summary>
        public static readonly Target Blank = new( "blank" );

        /// <summary>
        /// Opens the linked document in the parent frame.
        /// </summary>
        public static readonly Target Parent = new( "parent" );

        /// <summary>
        /// Opens the linked document in the full body of the window.
        /// </summary>
        public static readonly Target Top = new( "top" );
    }
}
