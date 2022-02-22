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
        public static readonly Target Default = new( (string)null );

        /// <summary>
        /// Opens the linked document in the same frame as it was clicked (this is default).
        /// </summary>
        public static readonly Target Self = new( "_self" );

        /// <summary>
        /// Opens the linked document in a new window or tab.
        /// </summary>
        public static readonly Target Blank = new( "_blank" );

        /// <summary>
        /// Opens the linked document in the parent frame.
        /// </summary>
        public static readonly Target Parent = new( "_parent" );

        /// <summary>
        /// Opens the linked document in the full body of the window.
        /// </summary>
        public static readonly Target Top = new( "_top" );

        /// <summary>
        /// Opens the linked document in the named iframe.
        /// </summary>
        /// <param name="name">Name of the frame element.</param>
        /// <returns>Returns the new target.</returns>
        public static Target Frame( string name ) => new( name );
    }
}
