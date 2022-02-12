namespace Blazorise
{
    /// <summary>
    /// The target attribute specifies where to open the linked document.
    /// </summary>
    public record Target : Enumeration<Target>
    {
        /// <inheritdoc/>
        public Target( string name ) : base( name )
        {
        }

        /// <inheritdoc/>
        private Target( Target parent, string name ) : base( parent, name )
        {
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
